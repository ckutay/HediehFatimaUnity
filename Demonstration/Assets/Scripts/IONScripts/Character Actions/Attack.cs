using UnityEngine;
using System.Collections;
using ION.Core.Events;
using ION.Core.Extensions;

public class Attack : IONAction
{

    protected override string ActionName { get { return "attack"; } }
    
    private string _subject;
    private string _target;
    private AnimationManager _animationManager;
    
    public override void OnStart (IStarted<EntityAction<ActionParameters>> startEvt)
    {
        // Get Action's parameters
        ActionParameters args = startEvt.Action.StartArguments;
        
        _subject = args.Subject;
        _target = args.Target;
    
  
    
        string speech = "I am attacking " + _target;
        
        
        if (_subject != "User") {
            // Get animation manager
            _animationManager = UnityController.Controller.GetComponent<AnimationManager> ();
            // Play "talk" animation with speech ..
            if (_animationManager != null)_animationManager.AnimateTalk (speech, _target);    
        } else {
            //User
            SpeechBubble _sp = GetComponentInChildren<SpeechBubble> ();
            if (_sp != null) {
                _sp._speech = speech;
                _sp.enabled = true;
            
            }
           // 
          
            
        }
    }
    public void WaitSleep(){

        System.Threading.Thread.Sleep(3000);
    }
    IEnumerator Wait ()
    {
      
        yield return new WaitForSeconds (100.0f);
        
    }

    public override void OnStep (IStepped<EntityAction<ActionParameters>> steppedEvt)
    {
        
        // Check if animation stopped.. 
        if (_animationManager != null && !_animationManager.IsPlaying) {
            this.Action.Stop (true);
        } else {
            this.Action.Stop (true);
        }
    
    }
    
    public override void OnStop (IStopped<EntityAction<ActionParameters>> stoppedEvt)
    {
    }
}
