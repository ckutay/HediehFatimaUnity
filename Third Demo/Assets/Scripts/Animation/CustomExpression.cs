/*************
This script activates the Groups of Custom Shapes set in the RandomEyes script attached to Dr Sarah. 
Attach this to the same object the RandomEyes script is attached to and you can call any of the functions.
*************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyMinnow.SALSA; 
public class CustomExpression : MonoBehaviour {

	public RandomEyes3D randomEyes; 
	
	void Start()
	{
		randomEyes = GetComponent<RandomEyes3D>();
		
		Smile(5.0f); //make Sarah smile for 5 seconds
		
	}
	
	public void Smile(float duration)
	{
			randomEyes.SetGroup("Smile", duration);
		
	
	}
	
	public void Frown(float duration)
	{
			randomEyes.SetGroup("Frown", duration);
		
	
	}
	
	public void Sympathy(float duration)
	{
			randomEyes.SetGroup("Sympathy", duration);
		
	
	}
	
	public void BigSmile(float duration)
	{
			randomEyes.SetGroup("BigSmile", duration); // Activate multi-BlendShape smile for 1.5 seconds*/
		
	
	}
}