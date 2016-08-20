using UnityEngine;
using System.Collections;
using ION.Core.Events;
using ION.Core.Extensions;

public class Grant : IONAction {

	protected override string ActionName { get { return "Grant";} }
	
	public override void OnStart(IStarted<EntityAction<ActionParameters>> startEvt){
		Debug.Log("[IONAction] Grant start.");
	}
	
	public override void OnStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
		Debug.Log("[IONAction] Grant single step.");
		this.Action.Stop(true);
	}
	
	public override void OnStop(IStopped<EntityAction<ActionParameters>> stoppedEvt){
		Debug.Log("[IONAction] Grant stop.");	
	}
}