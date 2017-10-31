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
        public int score1, score2, score3;
        public GameObject mySlider = GameObject.Find("Slider");
//        public Slider mySlider1;

        public GameObject myInputText = GameObject.Find("InputField1");
        public GameObject myInputTextUserName = GameObject.Find("InputField");

        string dayofWeek = System.DateTime.Now.DayOfWeek.ToString();
        
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
//          mySlider1 = GameObject.Find("Slider").GetComponent<Slider>();

            mySlider.SetActive(false);
            myInputText.SetActive(false);
            myInputTextUserName.SetActive(false);

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

            if (currentState.ToString() == "conv2" || currentState.ToString() == "conv46" || currentState.ToString() == "conv77")
            {
                mySlider.SetActive(true);
                //mySlider
            }
            

            if (currentState.ToString() == "conv3" || currentState.ToString() == "conv4" || currentState.ToString() == "conv46_1" || currentState.ToString() == "conv46_2" || currentState.ToString() == "conv46_3" || currentState.ToString() == "conv78") 
                 mySlider.SetActive(false);

            if (currentState.ToString() == "conv45_no")
                myInputText.SetActive(true);

            if (currentState.ToString() == "conv45")
                myInputText.SetActive(false);

            if (currentState.ToString() == "pre_greeting")
                myInputTextUserName.SetActive(true);

            if (currentState.ToString() == "greeting1")
                myInputTextUserName.SetActive(false);

            //if (currentState.ToString() == "conv5")
            //{
            //    mySlider.SetActive(false);
            //    score1 = (int) mySlider1.value;
            //    if (score1 >= 4)
            //    {                    
            //        speakAction.Parameters[0] = (Name)"test1";
            //        speakAction.Parameters[1] = (Name)"conv3";
            //    }
            //    else
            //    {
            //        speakAction.Parameters[0] = (Name)"test2";
            //        speakAction.Parameters[1] = (Name)"conv4";
            //    }
            //}
            if (dayofWeek == "Monday" && nextState.ToString() == "whichday")
            {
                speakAction.Parameters[0] = (Name)"whichday";
                speakAction.Parameters[1] = (Name)"greeting2_Mon";
            }
            else if ((dayofWeek == "Tuesday" || dayofWeek == "Wednesday") && nextState.ToString() == "whichday" && TimeOfDay() == "Day")
            {
                speakAction.Parameters[0] = (Name)"whichday";
                speakAction.Parameters[1] = (Name)"greeting2_Tue_Wed_day";
            }
            else if ((dayofWeek == "Tuesday" || dayofWeek == "Wednesday") && nextState.ToString() == "whichday" && TimeOfDay() == "Evening")
            {
                speakAction.Parameters[0] = (Name)"whichday";
                speakAction.Parameters[1] = (Name)"greeting2_Tue_Wed_eve";
            }
            else if ((dayofWeek == "Thursday" || dayofWeek == "Friday") && nextState.ToString() == "whichday")
            {
                speakAction.Parameters[0] = (Name)"whichday";
                speakAction.Parameters[1] = (Name)"greeting2_Thu_Fri";
            }
            else if ((dayofWeek == "Saturday" || dayofWeek == "Sunday") && nextState.ToString() == "whichday")
            {
                speakAction.Parameters[0] = (Name)"whichday";
                speakAction.Parameters[1] = (Name)"greeting2_Sat_Sun";
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
        string TimeOfDay()
        {
            int hours = System.DateTime.Now.Hour;
            if (hours > 16)
                return ("Evening");
            else
                return ("Day");
        }

    }
}