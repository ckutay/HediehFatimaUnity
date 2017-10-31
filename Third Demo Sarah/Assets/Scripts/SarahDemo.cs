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
using RolePlayCharacter;
using WellFormedNames;

public class SarahDemo : MonoBehaviour
{
    //  public GameObject mySlider = GameObject.Find("Slider");
    public Slider mySlider;
    public int isSliderDragged = 0;
    public InputField myInputFeild;
    public InputField myInputFieldUserName;

    public float globalScore1 = 0;
    public static int mytest = 0;

    string groupName;
    int questionNumber = 0;
    string userReply;
    string studyWeekend;

    [Serializable]
    private struct CharacterDefinition
    {
        public DialogController dialogController;
        public UnityBodyImplement CharaterArchtype;
        public Transform SceneAnchor;
    }

    public struct ScenarioData
    {
        public readonly string ScenarioPath;
        public readonly string TTSFolder;

        private IntegratedAuthoringToolAsset _iat;

        public IntegratedAuthoringToolAsset IAT { get { return _iat; } }

        public ScenarioData(string path, string tts)
        {
            ScenarioPath = path;
            TTSFolder = tts;

            _iat = IntegratedAuthoringToolAsset.LoadFromFile(ScenarioPath);
        }
    }

    [SerializeField]
    private CharacterDefinition m_character01;

    [Space]
    [SerializeField]
    private Button m_dialogButtonArchetype = null;
    [SerializeField]
    private Transform m_dialogButtonZone = null;

    [Space]
    [SerializeField]
    [Range(1, 60)]
    private float m_agentProblemReminderRepeatTime = 3;

    private List<Button> m_buttonList = new List<Button>();
    private IntegratedAuthoringToolAsset _iat;

    private bool Initialized;
    private bool waitingforReply;

    private AgentControler _agentController;

    public GameObject VersionMenu;

    public List<Button> characterSelectionButtons;
    private ScenarioData[] m_scenarios;

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        isSliderDragged = 1;
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        
        //      mySlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        mySlider = GameObject.Find("Slider").GetComponent<Slider>();

        //Adds a listener to the main slider and invokes a method when the value changes.
        mySlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        myInputFeild = GameObject.Find("InputField1").GetComponent<InputField>();

        myInputFieldUserName = GameObject.Find("InputField").GetComponent<InputField>();

        waitingforReply = false;
        Initialized = false;
        AssetManager.Instance.Bridge = new AssetManagerBridge();
        m_character01.dialogController.Clear();
        //removing the start button

        var streamingAssetsPath = Application.streamingAssetsPath;

#if UNITY_EDITOR || UNITY_STANDALONE
        streamingAssetsPath = "file://" + streamingAssetsPath;
#endif
        var www = new WWW(streamingAssetsPath + "/scenarioList.txt");
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            m_character01.dialogController.AddDialogLine("Error: " + www.error);
            yield break;
        }

        var entries = www.text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        if ((entries.Length % 2) != 0)
        {
            m_character01.dialogController.AddDialogLine("Error: Scenario entries must in groups of 2, to identify the scenario file, and TTS directory");
            yield break;
        }

        {
            List<ScenarioData> data = new List<ScenarioData>();

            for (int i = 0; i < entries.Length; i += 2)
            {
                var path = entries[i].Trim();
                var tts = entries[i + 1].Trim();
                data.Add(new ScenarioData(path, tts));
            }

            m_scenarios = data.ToArray();
        }

        m_character01.dialogController.Clear();

        this.LoadScenario(m_scenarios[0]); //will always load the first scenario

    }
    private void LoadScenario(ScenarioData data)
    {
        _iat = data.IAT;
        var rpcSource = _iat.GetAllCharacterSources().ToList()[0];
        string error = "";
        var rpc = new RolePlayCharacterAsset();
        rpc = RolePlayCharacterAsset.LoadFromFile(rpcSource.Source, out error);
        rpc.LoadAssociatedAssets();
        _iat.BindToRegistry(rpc.DynamicPropertiesRegistry);

        _agentController = new AgentControler(rpc, _iat, gameObject.GetComponent(typeof(UnityBodyImplement)) as UnityBodyImplement, m_character01.SceneAnchor, m_character01.dialogController, data.TTSFolder);

        StopAllCoroutines();
        _agentController.Start(this, VersionMenu);
    }

    public void SaveState()
    {
        _agentController.SaveOutput();
    }

    private void UpdateButtonTexts(bool hide, IEnumerable<DialogueStateActionDTO> dialogOptions)
    {
        if (hide)
        {
            if (!m_buttonList.Any())
                return;
            foreach (var b in m_buttonList)
            {
                Destroy(b.gameObject);
            }
            m_buttonList.Clear();
        }
        else
        {
            if (m_buttonList.Count == dialogOptions.Count())
                return;

            foreach (var d in dialogOptions)
            {
                //bypass start
                var b = Instantiate(m_dialogButtonArchetype);
                var t = b.transform;
                t.SetParent(m_dialogButtonZone, false);

                //b.GetComponentInChildren<Text>().color = Color.yellow;
                //GameObject.Find("DialogButton(Clone)").GetComponent <Text>().color = Color.yellow;

                //Changing the width??
              //  GameObject.Find("DialogButton(Clone)").GetComponent <Text>().;

                b.GetComponentInChildren<Text>().text = d.Utterance;

                b.GetComponentInChildren<Text>().verticalOverflow = VerticalWrapMode.Overflow;
               // b.GetComponentInChildren<Text>().flexibleWidth = 200;
                b.GetComponentInChildren<Text>().resizeTextMaxSize = 12;

                //This doesn't change the color of the text to yellow?!
                //b.GetComponentInChildren<Text>().color = Color.yellow;

                var dialogId = d.Id;

                b.onClick.AddListener((() => Reply(dialogId)));
                m_buttonList.Add(b);
                GameObject.Find("MenuZone").GetComponent<Image>().enabled = true;
            }
        }
        //Debug.LogWarning(m_character01);
    }


    public void Reply(Guid dialogId)
    {
        var diagStateProperty = string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER);
        var state = _agentController.RPC.GetBeliefValue(diagStateProperty);
        if (state == IATConsts.TERMINAL_DIALOGUE_STATE)
        {
            return;
        }
        var reply = _iat.GetDialogActionById(IATConsts.PLAYER, dialogId);

        var actionFormat = string.Format(IATConsts.DIALOG_ACTION_KEY + "({0},{1},{2},{3})", reply.CurrentState, reply.NextState, reply.GetMeaningName(), reply.GetStylesName());
        
        // Check if the user id is entered
        if (reply.CurrentState == "re_pre_greeting" && reply.NextState == "greeting1" && reply.Utterance == "BEGIN" && myInputFieldUserName.text == "")
        {
            reply.CurrentState = "re_pre_greeting";
            reply.NextState = "greeting1";
            return;
        }
        //Check if the user id is valid
        if ((reply.CurrentState == "re_pre_greeting" && reply.NextState == "greeting1" && reply.Utterance == "BEGIN" && System.Convert.ToInt32(myInputFieldUserName.text)<10000) || (reply.CurrentState == "re_pre_greeting" && reply.NextState == "greeting1" && reply.Utterance == "BEGIN" && System.Convert.ToInt32(myInputFieldUserName.text) > 99999))
        {
            reply.CurrentState = "re_pre_greeting";
            reply.NextState = "greeting1";
            return;
        }
//Check if the slider value changed
        if (reply.CurrentState == "re_conv2" && reply.NextState == "re_conv3" && reply.Utterance == "Next" && isSliderDragged == 0)
        {
            reply.CurrentState = "re_conv2";
            reply.NextState = "re_conv3";
            return;
        }


        //*******Check if student studying on weekends*************//
        if (reply.CurrentState == "re_conv21" && reply.NextState == "conv22_yes")
            studyWeekend = "Yes";
        else if ((reply.CurrentState == "re_conv21" && reply.NextState == "conv22_no"))
            studyWeekend = "No";

        //        if (studyWeekend = "Yes" && ) 

        //  Debug.LogWarning(mySlider.value);
        if (reply.CurrentState == "re_conv2" && reply.NextState == "re_conv3" && reply.Utterance == "Next")
        {
            userReply = "Score1 =" + mySlider.value;
            globalScore1 = mySlider.value;
        }
        else if
            (reply.CurrentState == "re_conv45_no" && reply.NextState == "conv45" && reply.Utterance == "Next")
            userReply = "Why=" + myInputFeild.text;
        else if (reply.CurrentState == "re_conv46" && reply.NextState == "re_conv47" && reply.Utterance == "Next")
            userReply = "Score2 =" + mySlider.value;
        else if (reply.CurrentState == "re_conv77" && reply.NextState == "conv78" && reply.Utterance == "Next")
            userReply = "Score3 =" + mySlider.value;

        else
            userReply = reply.Utterance;

        //*******Check the score1 if it is over 4 or not. Based on the user 
        if (reply.CurrentState == "re_conv2" && reply.NextState == "re_conv3")
        {
            if (mySlider.value > 4)
            {
                reply.CurrentState = "re_conv3";
                reply.NextState = "conv3";
            }
            else
            {
                reply.CurrentState = "re_conv3";
                reply.NextState = "conv4";
            }
        }

        if (reply.CurrentState == "re_conv46" && reply.NextState == "re_conv47")
        {
            if (mySlider.value > globalScore1)
            {
                reply.CurrentState = "re_conv47";
                reply.NextState = "conv46_1";
            }
            else if (mySlider.value >= 0 && mySlider.value < 4)
            {
                reply.CurrentState = "re_conv47";
                reply.NextState = "conv46_2";
            }
            else if (mySlider.value >= 4)
            {
                reply.CurrentState = "re_conv47";
                reply.NextState = "conv46_3";
            }


        }



        //make group number textbox invisible after group number entered
        questionNumber = questionNumber + 1;
        if (questionNumber == 2)
        {
            if (groupName == "")
            {
                questionNumber = questionNumber - 1;
                //return;
            }
            else
            {

                GameObject.Find("InputField").transform.localScale = new Vector3(0, 0, 0);
                GameObject.Find("Text").transform.localScale = new Vector3(0, 0, 0);
            }
        }


        if (reply.Utterance == "Bye")
        {
            Application.Quit();
        }

        StartCoroutine(SaveToDB());

        StartCoroutine(PlayerReplyAction(actionFormat, reply.NextState));
    }


    private IEnumerator PlayerReplyAction(string replyActionName, string nextState)
    {
        const float WAIT_TIME = 0.5f;
        _agentController.AddEvent(EventHelper.ActionStart(IATConsts.PLAYER, replyActionName, _agentController.RPC.CharacterName.ToString()).ToString());
        yield return new WaitForSeconds(WAIT_TIME);
        _agentController.AddEvent(EventHelper.ActionEnd(IATConsts.PLAYER, replyActionName, _agentController.RPC.CharacterName.ToString()).ToString());
        _agentController.AddEvent(EventHelper.PropertyChange(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER), nextState, "Player").ToString());
    }


    IEnumerator SaveToDB()
    {
        string url = "http://comp.mq.edu.au/vworlds/save_stress_data.php";
        WWWForm form = new WWWForm();
        form.AddField("question", questionNumber); //question_number should be an integer (e.g. 1 for Q1, 2 for Q2 etc)
        form.AddField("answer", userReply); //answer_value should be a string e.g. "They're not good. I just can't seem to get anything done."
        form.AddField("username", myInputFieldUserName.text); /*username_value should be a string. If you don't want to record a user ID, just put an empty string here: ""*/

        WWW w = new WWW(url, form); //if you are making a WebGL build you might have to put the full URL like: http://comp.mq.edu.au/vworlds/save_stress_data.php
        yield return w;
        //        print(w.text);
        //      Debug.LogWarning(userReply);
        //              Debug.LogWarning(w.text);
        //      Debug.LogWarning("Hedieh");

    }

    // Update is called once per frame
    void Update()
    {
        if (_agentController == null)
            return;

        if (!_agentController.IsRunning)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            this.SaveState();
        }

        _agentController.UpdateEmotionExpression();

        var state = Name.BuildName(_agentController.RPC.GetBeliefValue(string.Format(IATConsts.DIALOGUE_STATE_PROPERTY, IATConsts.PLAYER)));
        var possibleOptions = _iat.GetDialogueActionsByState(IATConsts.PLAYER, state.ToString());

        if (!possibleOptions.Any())
        {
            UpdateButtonTexts(true, null);
        }
        else
        {
            UpdateButtonTexts(false, possibleOptions);
        }
    }


    private void LateUpdate()
    {
        if (_agentController != null)
            _agentController.UpdateFields();
    }
}
