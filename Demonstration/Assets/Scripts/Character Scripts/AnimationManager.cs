using UnityEngine;
using System.Collections;

public class AnimationManager
: MonoBehaviour
{
    static int idle = Animator.StringToHash ("Base.IDLE");
    SpeechBubble _sp;
    SpeechBubble[] _sps;
    GameObject[] _gos;
    Animator[] _anims;
    private string _name;
    public GameObject graphicalRepresentation;
    public GameObject graphicalComponent;
    public GameObject head;
    Vector3  _originalPos = new Vector3 (0, 0, 0);
    public Vector3 _motion = new Vector3 (0, .001f, 0);
    public Quaternion _original = new Quaternion (0, 0, 0, 0);
    int i = 0;
    public      Animator animator;
    public bool returns;
    GameObject User;
    // Returns the animation state (playing or not)
    public bool IsPlaying {
        get {
          
            if (animator) {
               
             
                if (animator.GetCurrentAnimatorStateInfo (0).nameHash == idle) {

                    return false;
                } else {
                   


                   
                    return true;
                }
            } else
                return false;
            
        }
    }

    IEnumerator Wait ()
    {
        
        yield return new WaitForSeconds (100.0f);
        
    }

    public void WaitSleep ()
    {
        
        System.Threading.Thread.Sleep (300);
    }
    
    // Talk animation
    public void AnimateTalk (string speech, string target)
    { 
        foreach (Animator anim in _anims) {
            anim.SetBool ("talk", false);
            anim.SetBool ("happy", false);
        }
        GameObject _target = graphicalRepresentation;
       
        if (_gos != null) {
            foreach (GameObject go in _gos) {
                if (go.name == target)
                    _target = go;
            }
        }

     
        
        // Play "Talk" animation from the characters' graphical representation
        if (graphicalRepresentation != null) {
            animator = (Animator)graphicalRepresentation.GetComponent (typeof(Animator));
            if (animator)
                animator.SetBool ("talk", true);
            //clear users's talk
            User = GameObject.Find ("User");
            Component[] comp = User.GetComponentsInChildren<SpeechBubble> ();
        
            foreach (SpeechBubble sb in comp)
                sb.enabled = false;

        }
        // Get motion started, so not back to start until finished
        if (graphicalComponent != null)
            graphicalComponent.transform.position += _motion;
        i = 0;
       
        // Debug.Log (_name + " says:" + speech);
        if (_sp != null) {
            _sp._speech = speech;
         
            Debug.Log (_target);
            if (head & _target.name != "Robert_The_Doctor_S1@talking_5")
                head.transform.LookAt (User.transform);

            _sp.enabled = true;
            foreach (SpeechBubble spl in _sps) {
                if (spl != _sp)
                    spl.enabled = false;
            }
        }
       
    }

    public void Stop ()
    {
        if (animator != null) {
            
            animator.SetBool ("talk", false);
            animator.SetBool ("happy", false);
            
        }
    }
    // Happy animation
    public void AnimateHappy ()
    {
        foreach (Animator anim in _anims) {
            anim.SetBool ("talk", false);
            anim.SetBool ("happy", false);
        }
        Animator animator = null;
        // Write emotional expression to Debug log
        if (graphicalRepresentation != null) {
            animator = (Animator)graphicalRepresentation.GetComponent (typeof(Animator));
            animator.SetBool ("happy", true);
        }
        string speech = (_name + " is happy you talk to him");
       
        // Debug.Log (speech);
        if (_sp != null) {
            _sp._speech = speech;
               
          
            
            _sp.enabled = true;

        }
      
    }
    
    // Sad animation
    public void AnimateSad ()
    {
        foreach (Animator anim in _anims) {
            anim.SetBool ("talk", false);
            anim.SetBool ("happy", false);
        }
        // Write emotional expression to Debug log
        if (graphicalRepresentation != null) {
            Animator animator = (Animator)graphicalRepresentation.GetComponent (typeof(Animator));
            animator.SetBool ("happy", false);
        }
        Debug.Log (_name + " is sad :(");
    }
    
    void Start ()
    {
        _sps = FindObjectsOfType (typeof(SpeechBubble)) as SpeechBubble[];
        _gos = GameObject.FindGameObjectsWithTag ("Actor");
        _anims = FindObjectsOfType (typeof(Animator)) as Animator[];
        if (graphicalRepresentation != null) {
            _sp = graphicalRepresentation.GetComponentInChildren<SpeechBubble> ();
        } else if (graphicalComponent != null) {
            _sp = graphicalComponent.GetComponentInChildren<SpeechBubble> ();
        }
     
        _name = this.transform.parent.name;
        if (graphicalComponent != null)
            _originalPos = graphicalComponent.transform.position;
        if (head)
            _original = head.transform.rotation;
        User = GameObject.Find ("User");
        
    }
    
    void Update ()
    {
       
        if (IsPlaying) {
       
            if (graphicalComponent != null) {
                
                if (i < 10)
                    graphicalComponent.transform.position += _motion;
                else if (i < 19) {
                    if (returns)
                        graphicalComponent.transform.position -= _motion;
                    else
                        graphicalComponent.transform.position += _motion;
                } else {
                    if (returns)
                        graphicalComponent.transform.position = _originalPos;
                    else 
                        _originalPos = graphicalComponent.transform.position;
                    
                }
                i += 1;
        
            }
        
        } else if (animator != null) {
            Debug.Log ("Stopping");
            animator.SetBool ("talk", false);
            animator.SetBool ("happy", false);

        }
    }
}
