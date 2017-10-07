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

public class FatimaDemo : MonoBehaviour
{
    string groupName;
    int questionNumber = 0;
    string userReply;

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

    // Use this for initialization
    private IEnumerator Start()
    {
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

                b.GetComponentInChildren<Text>().text = d.Utterance;

                b.GetComponentInChildren<Text>().verticalOverflow = VerticalWrapMode.Overflow;

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


        if (reply.Utterance == "BYE")
        {
            Application.Quit();
        }

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
        WWWForm form = new WWWForm();
        form.AddField("username", groupName);
        form.AddField("question", questionNumber);
        form.AddField("answer", userReply);
        form.AddField("day", 1);
        //    WWW w = new WWW ("comp.mq.edu.au/vworlds/save_sarah_data.php", form);

        WWW w = new WWW("10.1.2.3/save_sarah_data.php", form);
        yield return w;
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