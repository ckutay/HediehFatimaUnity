
using UnityEngine;
using System.Collections;
using ION.Core.Events;
using ION.Core.Extensions;

public class ShowJoy : IONAction {
    
    protected override string ActionName { get { return "ShowJoy";} }
    
    private string _subject;
    private AnimationManager _animationManager;
    
    public override void OnStart(IStarted<EntityAction<ActionParameters>> startEvt){
        
        // Get Action's parameters
        ActionParameters args = startEvt.Action.StartArguments;
        
        _subject = args.Subject;
        
        // Get character controller
        Debug.Log (_subject +" is happy");
        _animationManager = UnityController.Controller.GetComponent<AnimationManager>();
        _animationManager.AnimateHappy();
        
        
        // Play happy animation
    }
    
    public override void OnStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
        // Check if animation stopped.. 
        if(!_animationManager.IsPlaying){
            this.Action.Stop(true);
        }
    }
    
    public override void OnStop(IStopped<EntityAction<ActionParameters>> stoppedEvt){
        this.Action.Stop(true);
        Debug.Log (_subject +" is finished happy");
        _animationManager.Stop();
    }
}