enum eAvailableActions		{None = 0, Look = 1, Interact = 2, SpeakTo = 4, Collect = 8}

private var	Actions			: int = eAvailableActions.None;

var	CanCollect				: boolean	= false;
var CanSpeakTo				: boolean	= false;
var CanInteractWith			: boolean	= false;
var CanInvestigate			: boolean	= false;
var talked :boolean = false;

var touch_cursor : Texture2D;
var talk_cursor : Texture2D;
var closeEnough = false;

private var cursorImage : Texture2D;
private var change_cursor = false;

private var mouse_click = false;
private var hasFocus				: boolean	= false;
private var FPC;
private var mouseLook;
private var first_mouse_over : boolean = true;

//private var buttonText = "Play";
private var buttonText = "Start \nSession";
private var at_beginning = true;

var guiskin : GUISkin;

var model : Transform;

function Start()
{

	UpdateState();
	/*if ((Actions & 4) == 4)
			PerformAction(4);*/
	
}

function UpdateState() : int
{
	var state : int = 0;
	if (CanCollect) 		state = state | 8;
	if (CanSpeakTo) 		state = state | 4;
	if (CanInteractWith)	state = state | 2;
	if (CanInvestigate) 	state = state | 1;
	
	Actions = state;
	//print (state);
	return Actions;
}

function HasFocus(setTo: boolean) 
{
	hasFocus = setTo;
}

function HasAction( action : eAvailableActions) : boolean
{
	var a : int = action;
	return ((Actions & a) == a);
}

function OnMouseOver () {

}

function OnMouseExit () {

}

function OnMouseDown () {
	mouse_click = true;
	
	
		if ((Actions & 2) == 2)
		{
			if(model.gameObject.GetComponent.<Renderer>().enabled)
				PerformAction(2);
		}
		
		if ((Actions & 4) == 4)
			PerformAction(4);
	
	
}

function OnGUI() {
	GUI.skin = guiskin;
	
	if(change_cursor)
	{
		var mousePos : Vector3 = Input.mousePosition;
		var pos : Rect = Rect(mousePos.x,Screen.height - mousePos.y,cursorImage.width,cursorImage.height);
		GUI.Label(pos,cursorImage);
	}
	
	if(at_beginning)
	{
		if (GUI.Button (Rect (Screen.width/2-90,Screen.height/2 - 65, 180, 130), buttonText))
		{
			
			at_beginning = false;
			//var blur = Camera.main.gameObject.GetComponent("BlurEffect");
			//blur.enabled = false;


			if ((Actions & 4) == 4)
					PerformAction(4);
			/*if(buttonText == "Start")
			{
					buttonText = "Stop";
				if ((Actions & 4) == 4)
					PerformAction(4);
				}
			
			else
			{
				buttonText = "Start";
				gameObject.BroadcastMessage("OnDialogueEnded");
			}*/
		}
	}
}

function ShowRestart()
{
	at_beginning = true;
	//buttonText = "Replay";
	buttonText = "Restart \nSession";
	//var blur = Camera.main.gameObject.GetComponent("BlurEffect");
	//blur.enabled = true;
}

function PerformAction(state : int)
{	
print (Actions);
	
	if ((Actions & state) == state)
	switch(state)
	{
		case 1:
			gameObject.BroadcastMessage("Investigate", mouse_click);
			break;
			
		case 2:
			gameObject.BroadcastMessage("Interact", mouse_click);
			break;
			
		case 4:
			
			gameObject.BroadcastMessage("SpeakTo", mouse_click);
			print("speaking");
			break;
			
		case 8:
			gameObject.BroadcastMessage("Collect", mouse_click);
			break;
			
	}
}


function Update()
{
	if (Input.GetMouseButton(1))
	{
		change_cursor = false;
	}
	if(Input.GetKeyDown(KeyCode.Tab))
	{
		change_cursor = false;
	}
}