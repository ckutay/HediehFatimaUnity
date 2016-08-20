using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object


public class FPSInputControllerSolo : MonoBehaviour
{
  
	public Vector3 directionVector;
	public float speed=1;
	public float maxSpeed=.05f;
    public    Vector3 rotation;
    // Use this for initialization
    void Awake()
    {
		
       
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKey(KeyCode.LeftShift))speed=4;
			else speed=1;
        // Get the input vector from kayboard or analog stick
      
      rotation=Vector3.up*Input.GetAxis("Horizontal");
        float directionLength=Input.GetAxis("Vertical") ;
       
	
        // Apply the direction to the CharacterMotor

        this.transform.Rotate(rotation*10);
    
        directionVector.y=0;
   transform.Translate (Vector3.forward * (directionLength*speed) * Time.deltaTime, Space.Self);
   
    }
}