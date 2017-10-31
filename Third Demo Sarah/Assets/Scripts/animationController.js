#pragma strict

private var anim : Animation;
private var hips : Transform;
private var hips_pos : Vector3;
private var difference : float;
private var currenttime: float;

function Awake()
{
	hips = transform.Find("Armature/mixamorig_Hips");
	hips_pos = hips.position;
}

function Start() {
	anim = GetComponent.<Animation>();

	currenttime = 0.0;
}


function Update () {
	/*if (currenttime - Time.time == 3.0)
	{
		transform.position.z = hips.position.z;
		anim.CrossFade("idle", 0.5);
	}
	if (anim.IsPlaying("walking"))
	{
		print(hips.position);
		//anim.CrossFadeQueued("idle", 3.0, QueueMode.PlayNow);
	}*/
	hips.position = hips_pos;
	print(hips.position);
}