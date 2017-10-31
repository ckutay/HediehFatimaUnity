/*
Display type 2
==============
This display type is designed for screen resolutions of min 800 x 600 and larger.
All items are views in the bottom right hand corner of the screen. Ideal for inspecting items...

About this display
==================
This display was created by creating a subclass from _crDialogue.
Variables that need to be set before use are:
	1. FileName			: A String containing the name of the dialogue file to use ADDENDUM BY MEREDITH: should be the first file in the dialogueFiles array if there are several files to play
	2. highlightBar		: An image to show behind text to indicate this line is the currently selected line
	3. textBackground	: An image to show behind the text
	4. avatarBG			: an image to show behind avatar images.
This display was built for the express purpose of inspecting items and giving users the option
of interacting with items, but there is nothing preventing you from using this is a very basic 
dialogue system. Imagine a 2D side scrolling adventure game with NPC's in the background.
Just walk up to them, hit a key, they say something, the dialogue ends. Very simple requirements, so why
not use a very simple display?
*/

class talkingHeadDialogueStyle_ extends _crDialogue
{

// Built for 800x600
var avatarDialogPos		: Transform;
var ResponseDialogPos	: Transform;
var textBackground		: Texture2D;
var avatarBG			: Texture2D;
var highlightBar		: Texture2D;
var callout_tail		: Texture2D;
var WINpointer			: Texture2D;
var OSXpointer			: Texture2D;

var dialogueFiles		: String[];

var guiSkin				: GUISkin;

private var pointer		:Texture2D;


var displaychoice = false;

var displayFormItem = false;
var displaySingleAnswer = false;
var displayMultipleAnswer = false;
var displayQuestion : String[];
var displayImage : Texture2D;
var checkboxAnswers :boolean[];
var currentQuestionNum = "";


private var keep_last_speech_bubble = false;
private var prev_line = "";

private var selGridInt = 0;

function talkingHeadDialogueStyle_()
{
}

function ActivateDisplay()
{
}

function SetPointer(OS)
{

	if(OS == "OSX")
		pointer = OSXpointer;
	else	
		pointer = WINpointer;

}


	
function Draw()
{
	GUI.skin = guiSkin;
	var speechBubblePos = Camera.main.WorldToScreenPoint (avatarDialogPos.position);
	var choicePos = Camera.main.WorldToScreenPoint (ResponseDialogPos.position);
	if (!fileIsLoaded())
	{
		return;
	}
	
	
	
	var avatarRect	: Rect = Rect(Screen.width - 185, Screen.height - 185, 180, 180);
	var textBGRect	: Rect = Rect(choicePos.x, Screen.height - choicePos.y, 615, 200);
	var textPos		: Rect = Rect(choicePos.x+10, Screen.height - choicePos.y+5, 585, 300);
	
	if(whoIsSpeakingID() == 1)
	{
		displaychoice = false;
		keep_last_speech_bubble = true;
		
	}
	
	if (whoIsSpeakingID() == 0)
	{	
		keep_last_speech_bubble = false;
		avatarRect	= Rect(005, 05, 198, 213);
		
		/********size for regular evie *******/
		var speech_bubble_height = GUI.skin.label.CalcHeight(GUIContent(currentText()),505);
		
		textBGRect	= Rect(speechBubblePos.x, Screen.height - speechBubblePos.y, 505, speech_bubble_height+15);
		var calloutTail = Rect(speechBubblePos.x, Screen.height - speechBubblePos.y + speech_bubble_height+15, callout_tail.width, callout_tail.height);
		textPos		= Rect(speechBubblePos.x + 10, Screen.height - speechBubblePos.y + 15, 490, speech_bubble_height);
		
		/**********end regular evie **************/
		
		/*********size for small evie ************
		var speech_bubble_height = GUI.skin.label.CalcHeight(GUIContent(currentText()),325);
		
		textBGRect	= Rect(speechBubblePos.x, Screen.height - speechBubblePos.y, 325, speech_bubble_height+15);
		calloutTail = Rect(speechBubblePos.x, Screen.height - speechBubblePos.y + speech_bubble_height+15, callout_tail.width, callout_tail.height);
		textPos		= Rect(speechBubblePos.x + 10, Screen.height - speechBubblePos.y + 15, 310, speech_bubble_height);
		/******* end small evie***********/
		
		GUI.DrawTexture(calloutTail, callout_tail);
		GUI.DrawTexture(textBGRect, textBackground);
		
		
	}
	if(displaychoice) //if the user is choosing options, we still want to display what Dr Evie last said
	{
		prev_line = currentText(); //keep a record of what was just shown
			
	}
	if(keep_last_speech_bubble)
	{
		var prev_speech_bubble_height = GUI.skin.label.CalcHeight(GUIContent(prev_line),505);
		
		var prevtextBGRect	= Rect(speechBubblePos.x, Screen.height - speechBubblePos.y, 505, prev_speech_bubble_height+15);
		calloutTail = Rect(speechBubblePos.x, Screen.height - speechBubblePos.y + prev_speech_bubble_height+15, callout_tail.width, callout_tail.height);
		var prevtextPos		= Rect(speechBubblePos.x + 10, Screen.height - speechBubblePos.y + 15, 490, prev_speech_bubble_height);
		GUI.DrawTexture(calloutTail, callout_tail);
		GUI.DrawTexture(prevtextBGRect, textBackground);
		GUI.Label(prevtextPos, prev_line);
	}
	
	
	
	if (avatarBG)
	GUI.DrawTexture(avatarRect, avatarBG);
//	GUI.DrawTexture(avatarRect, whoIsSpeakingAvatar());	
	//GUI.DrawTexture(textBGRect, textBackground);	
		
	
	GUI.BeginGroup(textPos);
		var thisRect : Rect;

		/*if (currentIsChoice())
		{
			for (x=0; x < currentOptionCount(); x++)
			{
				thisRect = Rect(0, (x * 27) + 5, textPos.width, 30);
				if (x == GetSelectionIndex() )
				{
					GUI.DrawTexture(thisRect, highlightBar);
				}
				if (GUI.Button(Rect(thisRect.width - thisRect.height - 2,thisRect.y, thisRect.height, thisRect.height),x + ""))
				{
					OptionSelect(x);
					Speak();
					if (HasEnded())
						parent.BroadcastMessage("OnDialogueEnded",SendMessageOptions.DontRequireReceiver);
				}
				if (GUI.Button(thisRect,currentOptions(x), thisFont) )
				{
					if (x == GetSelectionIndex() )
					{
						Speak();
						if (HasEnded())
							parent.BroadcastMessage("OnDialogueEnded",SendMessageOptions.DontRequireReceiver);
					} else	OptionSelect(x);
				}
			}
			 
		} */
		
		if (currentIsChoice())
		{
			var option_pos = 0;
			
			
			
			for (var x=0; x < currentOptionCount(); x++)
			{
				var c = GUIContent(currentOptions(x));
				var option_height = GUI.skin.button.CalcHeight(c,textPos.width*1.0);
				
				/*** if we have to display one of the history form questions display that first ***/
				if(displayFormItem)
				{
					if(displaySingleAnswer)
					{
						selGridInt = GUI.SelectionGrid (Rect (0, 0, textPos.width, 30 * displayQuestion.length), selGridInt, displayQuestion, 1);
						option_pos = 40 * displayQuestion.length + 5;
					}
					else if(displayMultipleAnswer)
					{
						
						if(displayImage)
						{
							GUI.Label(Rect(0, 0, displayImage.width, displayImage.height), displayImage);
							
							for (var i = 0; i< checkboxAnswers.Length ; i++)
							{
								checkboxAnswers[i] = GUI.Toggle(Rect(0, 39*i, 28, 28), checkboxAnswers[i], "");
								option_pos += 39;
							
							}
							
						}
						else
						{
							for (i = 0; i< checkboxAnswers.Length ; i++)
							{
								//option_pos += 39*i + 5;
								checkboxAnswers[i] = GUI.Toggle(Rect(0, i*25, 18, 18), checkboxAnswers[i], displayQuestion[i]);
								option_pos += 25;
								
							
							}
						}
					}
					//put the 'next" button nown the bottom
				}
					
				thisRect = Rect(0, option_pos, textPos.width, option_height);
				option_pos += option_height + 5;
				
				if (GUI.Button(thisRect,GUIContent(currentOptions(x),"highlight")) )
				{
					var answer = new Array(currentQuestionNum, x);
					//must call the SaveAnswer function from outside the class since co-routines in a class = bad
					
					//parent.BroadcastMessage("SaveAnswer",answer); //BroadcastMessage only accepts ONE parameter
					
					OptionSelect(x); 
					Speak();
					if (HasEnded())
						parent.BroadcastMessage("OnDialogueEnded",SendMessageOptions.DontRequireReceiver);
					
				}
				if(GUI.tooltip && GUI.tooltip == "highlight")
					Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
				
				else
					Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
		
		else
		{
			var text_height = GUI.skin.label.CalcHeight(GUIContent(currentText()),textPos.width);
			thisRect= Rect(0,0,textPos.width, text_height);
			
			
			GUI.Label(thisRect, currentText());
			//Debug.Log(currentText());
			
		}
	GUI.EndGroup();	
}

}

var DialogueHUD	: talkingHeadDialogueStyle_;


function OnGUI()
{
	
	if (!DialogueHUD.HasEnded())
	{
		DialogueHUD.Draw();
	}

}

function StartDialogue(withKeys: crGameKeys)
{
	StartDialogue(withKeys, -3);
}

function StartDialogue(withKeys: crGameKeys, atIndex : int)
{
	if (DialogueHUD.FileName != "")
	{
		DialogueHUD.parent = gameObject;
		DialogueHUD.AssignGameKeys(withKeys);
		DialogueHUD.OpenFile(atIndex);
		DialogueHUD.ActivateDisplay();
	}
	else
	{
		Globals.playMode = ePlaymode.Playing;
		enabled = false;
	}
}

function OnDialogueEnded()
{
	DialogueHUD.ResetDialogue();
	Globals.playMode = ePlaymode.Playing;
	enabled = false;
	//test to see if there are any dialogue files to follow
	if(DialogueHUD.dialogueFiles)
	{
		var i = System.Array.IndexOf(DialogueHUD.dialogueFiles, DialogueHUD.FileName);
		if(i < DialogueHUD.dialogueFiles.Length - 1) //check if we still have another file to open
		{
			var newi = i+1;
			DialogueHUD.FileName = DialogueHUD.dialogueFiles[newi];
			print(DialogueHUD.FileName);
			Globals.playMode = ePlaymode.Dialogue;
			enabled = true;
			StartDialogue(Globals.MainKeys);
			return;
		}
		
	}
	
	DialogueHUD.ResetDialogue();
	Globals.playMode = ePlaymode.Playing;
	enabled = false;
	gameObject.BroadcastMessage("ShowRestart");
	print("restarting");
	
}

/*******everything in Update() has been moved to talkingHeadDialogueActions.js**********/

function Awake()
{
	enabled = false;
	
	if (Application.platform == RuntimePlatform.OSXWebPlayer)
		DialogueHUD.SetPointer("OSX");
	else	
		DialogueHUD.SetPointer("WIN");

}

