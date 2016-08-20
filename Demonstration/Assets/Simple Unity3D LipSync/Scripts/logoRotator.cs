using UnityEngine;
using System.Collections;

[AddComponentMenu("Kiavash2k/Simple Unity3D LipSync/Additional - Logo Rotator")]
public class logoRotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.eulerAngles -= new Vector3(0,1,0);
	}
}
