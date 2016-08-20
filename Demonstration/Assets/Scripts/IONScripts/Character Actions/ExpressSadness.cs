using UnityEngine;
using System.Collections;
using ION.Core.Events;
using ION.Core.Extensions;

public class ExpressSadness : IONAction {

	protected override string ActionName { get { return "ExpressSadness";} }
	
	private string _subject;
	private AnimationManager _animationManager;
	
	public override void OnStart(IStarted<EntityAction<ActionParameters>> startEvt){
		// Get Action's parameters
		ActionParameters args = startEvt.Action.StartArguments;
		
		_subject = args.Subject;
		
		_animationManager = UnityController.Controller.GetComponent<AnimationManager>();
		// Play sad animation
		_animationManager.AnimateSad();
	}
	
	public override void OnStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
		// Check if animation stopped.. 
		if(!_animationManager.IsPlaying){
			this.Action.Stop(true);
		}
	}
	
	public override void OnStop(IStopped<EntityAction<ActionParameters>> stoppedEvt){
	}
}