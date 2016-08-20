using UnityEngine;
using System.Collections;

[AddComponentMenu("Kiavash2k/Simple Unity3D LipSync/LipSync PlayAgain Button")]
public class SimpleRepeatLipSync : MonoBehaviour {
	
	public Transform LipLink = null;
	public int SoundIndex = 0;
	public float Volume = 100.0f;
	
	void Start()
	{
		if(LipLink.GetComponent<simpleLipSync>() == null)
		{
			LipLink = LipLink.transform.root;
		}
	}
	
	void OnMouseEnter () 
	{
		GetComponent<Renderer>().material.color = Color.blue;
	}
	
	void OnMouseDown()
	{
		if (LipLink != null)
		{
			LipLink.GetComponent<simpleLipSync>().isTalking = true;
			LipLink.GetComponent<simpleLipSync>().SetVolume(Volume);
			LipLink.GetComponent<simpleLipSync>().setSound(SoundIndex);
		}
	}
	
	void OnMouseExit() {
		GetComponent<Renderer>().material.color = Color.red;
	}
}
