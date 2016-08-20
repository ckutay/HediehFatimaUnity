using System.Collections;
using ION.Meta;
using ION.Meta.Events;
using ION.Core;
using ION.Core.Extensions;
using FAtiMA.RemoteAgent;
using UnityEngine;

public class IONSimulation : MonoBehaviour {
		
	void Awake(){
		DontDestroyOnLoad (this.gameObject);
	}
		
	// Use this for initialization
	void Start () 
	{
		Simulation s = Simulation.Instance;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Simulation.Instance.Update();
	}
	
	void OnIONUpdate(IEvent evt)
	{
		Debug.Log("ION Event: " + evt.ToString());
	}
}
