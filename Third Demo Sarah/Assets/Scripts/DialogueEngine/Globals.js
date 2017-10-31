static var MainKeys	: crGameKeys;
static var playMode	: ePlaymode = ePlaymode.Playing;
var guiskin : GUISkin;
function Start()
{
	MainKeys = new crGameKeys();
	MainKeys.Clear();

}

function Awake()
{
	

}

/*function OnGUI()
{
	GUI.skin = guiskin;
	var greeting = "你好，你好嗎";
	GUI.Label (Rect (Screen.width/2-90,Screen.height/2 - 60, 180, 120), greeting);
}*/