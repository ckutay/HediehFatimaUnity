using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionLibrary;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;
using WellFormedNames;
using Utilities;
using IntegratedAuthoringTool.DTOs;

namespace Assets.Scripts
{
	public class AgentControler
	{
        public GameObject myslider = GameObject.Find("Slider");

        public RolePlayCharacterAsset RPC;
		private DialogController m_dialogController;
	    private IntegratedAuthoringToolAsset m_iat;
		private UnityBodyImplement _body;
        private string m_ttsFolder;
        private DialogueStateActionDTO reply;
        private List<Name> _events=new List<Name>();
		private string lastEmotionRPC;
		private float _previousMood;

        private float _moodThresold = 0.001f;

        private MonoBehaviour m_activeController;
	    private GameObject m_versionMenu;
		private Coroutine _currentCoroutine = null;
        private bool just_talked;

        public AgentControler(RolePlayCharacterAsset rpc, IntegratedAuthoringToolAsset iat, UnityBodyImplement archetype, Transform anchor, DialogController dialogCrt, string ttsFolder)
		{
			RPC = rpc;
		    m_iat = iat;
			m_dialogController = dialogCrt;
            m_ttsFolder = ttsFolder;
			if (anchor){
				_body = GameObject.Instantiate(archetype);

            just_talked = false;

			var t = _body.transform;
			t.SetParent(anchor, false);
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			}else{
				_body=archetype;
			}

            m_dialogController.SetCharacterLabel(RPC.CharacterName.ToString());
        }

		public void AddEvent(string eventName)
		{
            _events.Add(Name.BuildName(eventName));
        }

		public void SetExpression(string emotion, float amount)
		{
			_body.SetExpression(emotion,amount);
		}

        public void SaveOutput()
        {
            const string datePattern = "dd-MM-yyyy-H-mm-ss";
            RPC.SaveToFile(Application.streamingAssetsPath + "\\Output\\" + RPC.CharacterName + "-" + DateTime.Now.ToString(datePattern) + ".ea");
        }

        public bool IsRunning {
			get { return _currentCoroutine != null; }
		}

		public void Start(MonoBehaviour controller, GameObject versionMenu)
		{
            myslider.SetActive(false);

            m_activeController = controller;
		    m_versionMenu = versionMenu;
			_currentCoroutine = controller.StartCoroutine(UpdateCoroutine());
		}

		public void UpdateFields()
		{	
			m_dialogController.UpdateFields(RPC);
		}

		public void UpdateEmotionExpression()
		{
			var emotion = RPC.GetStrongestActiveEmotion();
			if(emotion==null)
				return;

			_body.SetExpression(emotion.EmotionType, emotion.Intensity / 10f);
		}

        private IEnumerator UpdateCoroutine()
        {
            _events.Clear();
            while (RPC.GetBeliefValue(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER)) != IATConsts.TERMINAL_DIALOGUE_STATE)
            {
                yield return new WaitForSeconds(1);
                if (_events.Count == 0)
                {
                    RPC.Update();
                    continue;
                }

                RPC.Perceive(_events);
                var action = RPC.Decide().Shuffle().FirstOrDefault();

                _events.Clear();
                RPC.Update();

                if (action == null)
                    continue;

                Debug.Log("Action Key: " + action.Key);

                switch (action.Key.ToString())
                {
                    case "Speak":
                        m_activeController.StartCoroutine(HandleSpeak(action));

                        break;
                    case "Disconnect":
                        m_activeController.StartCoroutine(HandleDisconnect());
                        m_dialogController.AddDialogLine(string.Format("- {0} disconnects -", RPC.CharacterName));

                        _currentCoroutine = null;
                        UnityEngine.Object.Destroy(_body.Body);
                        break;
                    default:
                        Debug.LogWarning("Unknown action: " + action.Key);
                        break;
                }
            }
        }


        private IEnumerator HandleSpeak(IAction speakAction)
        {

            Name currentState = speakAction.Parameters[0];
            Name nextState = speakAction.Parameters[1];
            Name meaning = speakAction.Parameters[2];
            Name style = speakAction.Parameters[3];



            if (currentState.ToString() == "conv2")
            {
                
                myslider.SetActive(true);

//                nextState = (Name)"re_conv5";
  //              currentState = (Name)"conv5";
    //            Debug.LogWarning(nextState.ToString());
      //          speakAction.Parameters[1] = (Name)"con1";
            }

            var dialog = m_iat.GetDialogueActions(IATConsts.AGENT, currentState, nextState, meaning, style).FirstOrDefault();

            if (dialog == null)
            {
                Debug.LogWarning("Unknown dialog action.");
                m_dialogController.AddDialogLine("... (unkown dialogue) ...");
            }
            else
            {
                string subFolder = m_ttsFolder;
                if (subFolder != "<none>")
                {
                    var path = string.Format("/TTS-Dialogs/{0}/{1}/{2}", subFolder, RPC.VoiceName, dialog.UtteranceId);
                    var absolutePath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE
                    absolutePath = "file://" + absolutePath;
#endif
                    string audioUrl = absolutePath + path + ".wav";
                    string xmlUrl = absolutePath + path + ".xml";

                    var audio = new WWW(audioUrl);
                    var xml = new WWW(xmlUrl);

                    yield return audio;
                    yield return xml;

                    var xmlError = !string.IsNullOrEmpty(xml.error);
                    var audioError = !string.IsNullOrEmpty(audio.error);

                    if (xmlError)
                        Debug.LogError(xml.error);
                    if (audioError)
                        Debug.LogError(audio.error);

                    m_dialogController.AddDialogLine(dialog.Utterance);

                    if (xmlError || audioError)
                    {
                        yield return new WaitForSeconds(2);
                    }
                    else
                    {
                        var clip = audio.GetAudioClip(false);
                        yield return _body.PlaySpeech(clip, xml.text);
                        clip.UnloadAudioData();
                    }

                    reply = dialog;
                    just_talked = true;
                }
                else
                {
                    m_dialogController.AddDialogLine(dialog.Utterance);
                    yield return new WaitForSeconds(2);

                    reply = dialog;
                    just_talked = true;
                }

                if (nextState.ToString() != "-")
                {
                    var evt = EventHelper.PropertyChange("DialogueState(Player)", nextState.ToString(), this.RPC.CharacterName.ToString());
                    AddEvent(evt.ToString());
                }
                    
            }

            if (speakAction.Parameters[1].ToString() != "-") //todo: replace with a constant
            {
                var dialogueStateUpdateEvent = EventHelper.PropertyChange(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER), speakAction.Parameters[1].ToString(),RPC.CharacterName.ToString());
                Debug.Log("State now is: " + dialogueStateUpdateEvent);
                AddEvent(dialogueStateUpdateEvent.ToString());
            }
            if (nextState.ToString() == "Disconnect")
            {
                this.HandleDisconnect();
            }
            RPC.Perceive(new Name[] { EventHelper.ActionEnd(RPC.CharacterName.ToString(), speakAction.Name.ToString(), IATConsts.PLAYER) });
        }



        private IEnumerator HandleDisconnect()
		{
            if (_body)
                _body.Hide();           
            m_dialogController.Clear();
            yield return new WaitForSeconds(2);
			m_dialogController.Clear();
			m_versionMenu.SetActive(true);
		}
	}
}