using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;
	public Vector3 directionVector;
	public float speed=1;
	public float maxSpeed=1;
    // Use this for initialization
    void Awake()
    {
		
        motor = GetComponent<CharacterMotor>();
		maxSpeed=motor.maxForwardSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKey(KeyCode.LeftShift))speed=4;
			else speed=1;
        // Get the input vector from kayboard or analog stick
        directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = -directionVector * directionLength;
            directionVector.y=0;
        }
		motor.maxForwardSpeed=maxSpeed*speed;

        // Apply the direction to the CharacterMotor
        motor.inputMoveDirection = directionVector;
        motor.desiredMovementDirection=directionVector*speed;
   
    }
}