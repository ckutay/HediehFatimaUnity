using UnityEngine;
using System.Collections;
using ION.Core.Events;
using ION.Core.Extensions;

public class UserSpeech : IONAction {

	protected override string ActionName { get { return "UserSpeech";} }
	
	private string _subject;
	private string _target;
	private string _strategy;
	private string _type;
	private AnimationManager _animationManager;
	
	public override void OnStart(IStarted<EntityAction<ActionParameters>> startEvt){
		// Get Action's parameters
		ActionParameters args = startEvt.Action.StartArguments;
		
		_subject = args.Subject;
		_target = args.Target;
		//type = yes or givereason
		_type = args.Parameters[0];
		_strategy = args.Parameters[1];
		
		// Speech generator ..
		string speech;
		switch(_strategy){
			case "insult": 
				speech = _target + " you are dumb!"; 
				break;
			case "hello":
				speech = "Hello, " + _target + "!";
				break;
			case "ask-why":
				speech = "Why did you say that, " + _target + "?";
				break;
			default: 
				speech = _subject+ " talks to " + _target + " about "+_strategy;
				break;
		}
		if(	UnityController.Controller!=null){
		// Get animation manager
		_animationManager = UnityController.Controller.GetComponent<AnimationManager> ();
		// Play "talk" animation with speech ..
		//if (_animationManager != null)
              //  _animationManager.AnimateTalk (speech,_target);	
		}
		else
			Debug.Log(speech);
		
		
		
	}
	
	public override void OnStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
		
		// Check if animation stopped.. 
		if(_animationManager != null && !_animationManager.IsPlaying){
			this.Action.Stop(true);
		}else{
			this.Action.Stop(true);
		}
	}
	
	public override void OnStop(IStopped<EntityAction<ActionParameters>> stoppedEvt){
	}
}
