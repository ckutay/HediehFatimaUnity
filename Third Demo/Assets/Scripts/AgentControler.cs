using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionLibrary;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using UnityEngine;
using WellFormedNames;

namespace Assets.Scripts
{
	public class AgentControler
	{
		private RolePlayCharacterAsset _rpc;
		private DialogController m_dialogController;
	    private IntegratedAuthoringToolAsset m_iat;
		private UnityBodyImplement _body;

		private List<string> _events=new List<string>();
		private string lastEmotionRPC;
		private float _previousMood;

		private float _moodThresold = 0.001f;

		private MonoBehaviour m_activeController;
	    private GameObject m_versionMenu;
		private Coroutine _currentCoroutine = null;

        public AgentControler(RolePlayCharacterAsset rpc, IntegratedAuthoringToolAsset iat, UnityBodyImplement archetype, Transform anchor, DialogController dialogCrt)
		{
			_rpc = rpc;
		    m_iat = iat;
			m_dialogController = dialogCrt;
	        _body = GameObject.Instantiate(archetype);

			var t = _body.transform;
			t.SetParent(anchor, false);
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;

			m_dialogController.SetCharacterLabel("Client");
		}

		public void AddEvent(string eventName)
		{
			_events.Add(eventName);
		}

		public void SetExpression(string emotion, float amount)
		{
			_body.SetExpression(emotion,amount);
		}

		public void SaveOutput()
		{
			const string datePattern = "dd-MM-yyyy-H-mm-ss";

			_rpc.SaveOutput(Application.streamingAssetsPath + "\\Output\\", _rpc.CharacterName + "-" + DateTime.Now.ToString(datePattern) + ".ea");
		}

		public bool IsRunning {
			get { return _currentCoroutine != null; }
		}

		public void Start(MonoBehaviour controller, GameObject versionMenu)
		{
			m_activeController = controller;
		    m_versionMenu = versionMenu;
			_currentCoroutine = controller.StartCoroutine(UpdateCoroutine());
		}

		public void UpdateFields()
		{	
			m_dialogController.UpdateFields(_rpc);
		}

		public void UpdateEmotionExpression()
		{
			var emotion = _rpc.GetStrongestActiveEmotion();
			if(emotion==null)
				return;

			_body.SetExpression(emotion.EmotionType, emotion.Intensity / 10f);
		}
		
		private IEnumerator UpdateCoroutine()
		{
			_events.Clear();
			var enterEventRpcOne = string.Format("Event(Property-Change,{0},Front(Self),Computer)",_rpc.Perspective);
			AddEvent(enterEventRpcOne);

			while (m_iat.GetCurrentDialogueState("Client") != "Disconnected")
			{
				yield return new WaitForSeconds(1);

				var actionRpc = _rpc.PerceptionActionLoop(_events);
				_events.Clear();
				_rpc.Update();

				if (actionRpc == null)
					continue;

				string actionKey = actionRpc.ActionName.ToString();
				Debug.Log("Action Key: " + actionKey);

				switch (actionKey)
				{
					case "Speak":
						m_activeController.StartCoroutine(HandleSpeak(actionRpc));
                        break;
					case "Fix":
						m_activeController.StartCoroutine(HandleFix(actionRpc));
						break;
					case "Disconnect":
						m_activeController.StartCoroutine(HandleDisconnectAction(actionRpc));
						break;
					default:
						Debug.LogWarning("Unknown action: " + actionKey);
						break;
				}
			}

			m_dialogController.AddDialogLine(string.Format("- {0} disconnects -",_rpc.CharacterName));
			_currentCoroutine = null;
		}

		private IEnumerator HandleSpeak(IAction speakAction)
		{
			Name nextState = speakAction.Parameters[1];
			var dialog = m_iat.GetDialogueActions(IntegratedAuthoringToolAsset.AGENT, speakAction.Parameters[0].ToString()).FirstOrDefault(a => string.Equals(a.Meaning, speakAction.Parameters[2].ToString(), StringComparison.CurrentCultureIgnoreCase) && string.Equals(a.Style, speakAction.Parameters[3].ToString(), StringComparison.CurrentCultureIgnoreCase));

			if (dialog == null)
			{
				Debug.LogWarning("Unknown dialog action.");
				m_dialogController.AddDialogLine("... (unkown dialogue) ...");
			}
			else
			{
				var id = dialog.Id.ToString("N");
				var p = "TTS-Dialogs/" + id + "/speech";
				var clip = Resources.Load<AudioClip>(p);
				var data = Resources.Load<TextAsset>(p);

				m_dialogController.AddDialogLine(dialog.Utterance);

				if (clip != null && data != null)
				{
					yield return _body.PlaySpeech(clip, data);
					Resources.UnloadAsset(clip);
					Resources.UnloadAsset(data);
				}
				else
				{
					Debug.LogWarning("Could not found speech assets for dialog id \""+id+"\"");
					yield return new WaitForSeconds(2);
				}

				if (nextState.ToString() != "-") //todo: replace with a constant
				{
					m_iat.SetDialogueState("Client", nextState.ToString());
				}
			}

			if (speakAction.Parameters[1].ToString() != "-")//todo: replace with a constant
			{
				var dialogueStateUpdateEvent = string.Format("Event(Property-Change,SELF,DialogueState({0}),{1})", speakAction.Target, speakAction.Parameters[1]);
				AddEvent(dialogueStateUpdateEvent);
			}
			_rpc.ActionFinished(speakAction);
		}

		private IEnumerator HandleFix(IAction actionRpc)
		{
			var leaveEvt = string.Format("Event(Property-change,{0},Front(Self),Socket)", _rpc.Perspective);
			_events.Add(leaveEvt);

			yield return new WaitForSeconds(1.5f);

			var fixedEvt = string.Format("Event(Property-change,{0},IsBroken({1}),false)", _rpc.Perspective, actionRpc.Target);
			_events.Add(fixedEvt);
			var enterEvt = string.Format("Event(Property-change,{0},Front(Self),Computer)", _rpc.Perspective);
			_events.Add(enterEvt);
			_rpc.ActionFinished(actionRpc);
		}

		private IEnumerator HandleDisconnectAction(IAction actionRpc)
		{
			var exitEvtOne = string.Format("Event(Property-change,{0},Front(Self),-)", _rpc.Perspective);
			_events.Add(exitEvtOne);
			_rpc.PerceptionActionLoop(_events);
			yield return null;
			_rpc.ActionFinished(actionRpc);
			m_iat.SetDialogueState("Client", "Disconnected");
			_body.Hide();
			yield return new WaitForSeconds(2);
			m_dialogController.Clear();
			m_versionMenu.SetActive(true);
		}
	}
}