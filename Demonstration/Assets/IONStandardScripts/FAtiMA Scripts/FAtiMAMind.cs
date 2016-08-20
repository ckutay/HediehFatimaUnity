using UnityEngine;
using System.Collections;

using ION.Core;
using FAtiMA.RemoteAgent;
using ION.Core.Extensions;
using System;
using System.IO;

[RequireComponent (typeof(IONEntity))]
public class FAtiMAMind : MonoBehaviour {
	
	protected IONEntity _body;
	protected RemoteMind _mind;
	public bool AgentLaunched { get; protected set; }
	public bool Initialized { get; protected set; }
	
	public string _sex = "M";
	public string _role = "Role";
	public string _scenario = "ConflictScenario";
	public string _scenariosFile = "ConflictScenarios.xml";
	public string _scenarioPath = "data/characters/minds/";
	
	public float _timeForLaunch = 5.0f;
	
	private void OnLevelWasLoaded (int level) {
    }
	
	private void Awake()
	{
		this._body = this.GetComponent<IONEntity>();
	}
	
	// Use this for initialization
	void Start () {
		Initialized = false;
	}
	
	void Update(){
		if(!this.Initialized && _body.Initialized & !this.AgentLaunched){
			this._mind = new RemoteMind(_body.Entity,_sex,_role);
			RemoteAgentLoader loader = new RemoteAgentLoader(_mind);
			ScenarioParser.Scenario scenario = ScenarioParser.Instance.GetScenario(_scenarioPath + _scenariosFile, _scenario);
			int port = scenario.GetCharacter(_body.entityName).Port;
			
			LoadRemoteAgentArgs arguments = new LoadRemoteAgentArgs(false,LoadRemoteAgentArgs.LoadMode.CREATENEW, port, _scenarioPath, _scenariosFile, _scenario,"", "", _role, _scenarioPath);
						
			loader.Launch(arguments);
			
			StartCoroutine(WaitForAgentProcess());
			
			this.AgentLaunched = true;
		}
	}	
	
	// GAIPS - Henrique - A little hack for the time being...
	protected IEnumerator WaitForAgentProcess(){
		yield return new WaitForSeconds(_timeForLaunch);
		this.Initialized = true;
		yield return null;
	}
}
