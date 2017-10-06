using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetManagerPackage;
using Assets.Scripts;
using IntegratedAuthoringTool;
using IntegratedAuthoringTool.DTOs;
using UnityEngine;
using UnityEngine.UI;

public class SarahDemo : MonoBehaviour
{
	string groupName;
	int questionNumber = 0;
	string userReply;

	public enum DemoName
	{
		EmotionalAppraisal,
		EmotionalDecisionMaking,
		SocialImportance,
	}

	[Serializable]
	private struct CharacterDefinition
	{
		public DialogController dialogController;
		public UnityBodyImplement CharaterArchtype;
		public Transform SceneAnchor;
	}

	[SerializeField]
	private DemoName m_demoName;
  
    
	[SerializeField]
	private CharacterDefinition m_character01;

	[Space]
	[SerializeField]
	private Button m_dialogButtonArchetype = null;
	[SerializeField]
	private Transform m_dialogButtonZone = null;

	[Space]
	[SerializeField]
	[Range (1, 60)]
	private float m_agentProblemReminderRepeatTime = 3;
    
	private List<Button> m_buttonList = new List<Button> ();
	private IntegratedAuthoringToolAsset _iat;

	private AgentControler _agentController;

	private string _scenarioFile;

	public GameObject VersionMenu;

	public List<Button> characterSelectionButtons;

	// Use this for initialization
	void Start ()
	{
		switch (m_demoName) {
		case DemoName.EmotionalAppraisal:
			_scenarioFile = "Scenarios/EADemo.iat";
			break;
		case DemoName.EmotionalDecisionMaking:
			_scenarioFile = "Scenarios/EDMDemo.iat";
			break;
		case DemoName.SocialImportance:
			_scenarioFile = "Scenarios/SIDemo.iat";
			break;
		}

		AssetManager.Instance.Bridge = new AssetManagerBridge ();
		m_character01.dialogController.Clear ();
		_iat = IntegratedAuthoringToolAsset.LoadFromFile (StorageProvider.CurrentProvider, _scenarioFile);

		//removing the start button
		this.StartVersion (0);

//        Time.timeScale = 0;

		/*        var characterList = _iat.GetAllCharacters().ToList();
                for (int i = 0; i < characterSelectionButtons.Count; i++)
                {
                    if (i < characterList.Count)
                    {
                        characterSelectionButtons[i].gameObject.SetActive(true);
                        characterSelectionButtons[i].GetComponentInChildren<Text>().text = "Start";// characterList[i].CharacterName;
                    }
                    else
                    {
                        characterSelectionButtons[i].gameObject.SetActive(false);
                    }
                }
                */
	}

	public void StartVersion (int charNumber)
	{
		_iat = IntegratedAuthoringToolAsset.LoadFromFile (StorageProvider.CurrentProvider, _scenarioFile);

		_agentController = new AgentControler (_iat.GetAllCharacters ().ToList () [charNumber], _iat, gameObject.GetComponent (typeof(UnityBodyImplement)) as UnityBodyImplement, m_character01.SceneAnchor, m_character01.dialogController);

		StopAllCoroutines ();
        
		_agentController.Start (this, VersionMenu);
	}

	public void SaveState ()
	{
		_agentController.SaveOutput ();
	}

	private void UpdateButtonTexts (bool hide, IEnumerable<DialogueStateActionDTO> dialogOptions)
	{       
		
	
		if (hide) {
			if (!m_buttonList.Any ())
				return;
			foreach (var b in m_buttonList) {
				Destroy (b.gameObject);
			}
			m_buttonList.Clear ();
		} else {
			if (m_buttonList.Count == dialogOptions.Count ())
				return;

			foreach (var d in dialogOptions) {

				//bypass start
				if (d.Utterance == "START") {
					Reply (d.Style);
				} else {
					var b = Instantiate (m_dialogButtonArchetype);
					var t = b.transform;
					t.SetParent (m_dialogButtonZone, false);

					//b.GetComponentInChildren<Text>().color = Color.yellow;
					//GameObject.Find("DialogButton(Clone)").GetComponent <Text>().color = Color.yellow;

					b.GetComponentInChildren<Text> ().text = d.Utterance;

					b.GetComponentInChildren<Text> ().verticalOverflow = VerticalWrapMode.Overflow;

					b.GetComponentInChildren<Text> ().resizeTextMaxSize = 12;

					//This doesn't change the color of the text to yellow?!
					//b.GetComponentInChildren<Text>().color = Color.yellow;

					var style = d.Style;

					b.onClick.AddListener ((() => Reply (style)));
					m_buttonList.Add (b);
				
				//	GameObject.Find ("MenuZone").GetComponent<Image> ().enabled = true;
				}
			}

		}
		//Debug.LogWarning(m_character01);
	}

	public void Reply (string type)
	{
		//recording Group Name
	//	groupName = GameObject.Find ("InputField").GetComponent<InputField> ().text;

		var state = _iat.GetCurrentDialogueState ("Client");
		if (state == IntegratedAuthoringToolAsset.TERMINAL_DIALOGUE_STATE)
			return;

		var reply = _iat.GetDialogueActions (IntegratedAuthoringToolAsset.PLAYER, state).FirstOrDefault (a => String.Equals (a.Style, type, StringComparison.CurrentCultureIgnoreCase));
		userReply = reply.Utterance;

		//make group number textbox invisible after group number entered
		questionNumber = questionNumber + 1;
		if (questionNumber == 2) {
			if (groupName == "") {
				questionNumber = questionNumber - 1;
				return;
			} else {

//				GameObject.Find ("InputField").transform.localScale = new Vector3 (0, 0, 0);
				GameObject.Find ("Text").transform.localScale = new Vector3 (0, 0, 0);
			}
		}


		if (reply.Utterance == "BYE") {
			Application.Quit ();
		}

		var actionFormat = string.Format ("Speak({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.Meaning, reply.Style);
		Debug.LogWarning(reply.Meaning);
		StartCoroutine (SaveToDB ());


		StartCoroutine (PlayerReplyAction (actionFormat, reply.NextState));
	}

	IEnumerator SaveToDB ()
	{
		WWWForm form = new WWWForm ();
		form.AddField ("username", groupName);
		form.AddField ("question", questionNumber);
		form.AddField ("answer", userReply);
        form.AddField("day", 1);
    //    WWW w = new WWW ("comp.mq.edu.au/vworlds/save_sarah_data.php", form);

          WWW w = new WWW ("10.1.2.3/save_sarah_data.php", form);
        yield return w;
	}



	private IEnumerator PlayerReplyAction (string replyActionName, string nextState)
	{
		const float WAIT_TIME = 0.5f;
		_agentController.AddEvent (string.Format ("Event(Action-Start,Player,{0},Client)", replyActionName));
		yield return new WaitForSeconds (WAIT_TIME);
		_agentController.AddEvent (string.Format ("Event(Action-Finished,Player,{0},Client)", replyActionName));
		_agentController.AddEvent (string.Format ("Event(Property-change,Player,DialogueState(Player),{0})", nextState));
		_iat.SetDialogueState ("Client", nextState);
	}

	// Update is called once per frame
	void Update ()
	{
		if (_agentController == null)
			return;

		if (!_agentController.IsRunning)
			return;

		if (Input.GetKeyDown (KeyCode.P)) {
			if (Time.timeScale > 0)
				Time.timeScale = 0;
			else
				Time.timeScale = 1;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			this.SaveState ();
		}
        
		_agentController.UpdateEmotionExpression ();
		
		var state = _iat.GetCurrentDialogueState ("Client");

		if (!_iat.GetDialogueActions (IntegratedAuthoringToolAsset.PLAYER, state).Any ()) {
			UpdateButtonTexts (true, null);
		} else {
			var possibleOptions = _iat.GetDialogueActions (IntegratedAuthoringToolAsset.PLAYER, state);
			UpdateButtonTexts (false, possibleOptions);
		}
	}


	private void LateUpdate ()
	{
		if (_agentController != null)
			_agentController.UpdateFields ();
	}
}