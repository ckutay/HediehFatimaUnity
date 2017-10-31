using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {
	public Animator anim;
	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown("1"))
			anim.SetInteger("animation_type",1);
		if(Input.GetKeyDown("2"))
			anim.SetInteger("animation_type",2);
		if(Input.GetKeyDown("3"))
			anim.SetInteger("animation_type",3);
	
	}
}
