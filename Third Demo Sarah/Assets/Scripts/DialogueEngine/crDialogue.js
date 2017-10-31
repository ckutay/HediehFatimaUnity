/*
Copyright Jacco Jansen 2009
This code and the file format described herein may not be used
without the express consent of me, Jacco Andries Jansen.
See http://www.ninjaportal.com 
*/

/*
DESCRIPTION
================================
This is the base class for the dialogue system.

- The dialogue system handles the selection of dialogue turns to display and
- creates the illusion of AI in objects and characters.
- It can be used to display single lines of text for objects being viewed,
- for displaying error messages when trying to interact with objects,
- to instigate an action in response to interacting with an object,
- displaying dialogue for non playing characters and also
- to display complex dialgoue.
- Due to the ability to instigate actions, it can also be used to do audio dialogues
- or voice overs.
- The possibilities are endless...

The engine was designed to be powerful, yet easy to use. To use this engine, you must
subclass it to an object used to display your dialogue according to your requirements.
Once subclassed, entire dialogues can be displayed using only as little as the OpenFile
method to initialize the engine and the Speak function to handle everything else.

Additional methods for advanced dialogues are included also...

*/

/*
MAIN FUNCTIONS
===============
OpenFile() : boolean
OpenFile(id: int) : boolean
OpenFile(filename: String) : boolean
OpenFile(filename: String, id: int) : boolean
Speak(id: int)
Speak() : int
HasEnded()

EndDialogue()
ResetDialogue()
destroy()

AssignGameKeys(name: crGameKeys)

EXTRA FUNCTIONS
=============
fileIsLoaded() : boolean

currentIsChoice() : boolean
OptionNext()
OptionPrevious()
OptionSelect(val)
GetSelectionIndex() : int
currentOptions(x: int) : String
currentOptionCount() : int

currentText() : String
GetFormattedText();
GetUnformattedText() : String[]
GetCurrentLine()
IDOfNextLine() : int
 
whoIsSpeakingName() : String
whoIsSpeakingID() : int
whoIsSpeakingAvatar() : Texture2D

DEBUG FUNCTIONS
===============
ScanForId(id: int) : int
dumpLine(id: int) : String
currentRequirements() : Array
currentRedirects() : Array
currentKeys() : Array
 
*/

import System.IO; 

enum ePlaymode
{
	MainMenu			= 0,
	InGameMenu			= 1,
	Playing				= 2,
	NoKeyPress			= 3,
	Dialogue			= 4,
	PlayerSelect		= 5,
	Loading				= 6,
	GameOver			= 7
}

static var gkNone 		: int = -1;
static var gkNotFound	: int = -2;
static var gkContinue	: int = -3;
static var gkFalse		: int = 0;
static var gkTrue		: int = 1;

//contains the parsed details read from one dialogue turn
class crDialogueLine 
{
	var id			: int = -1;		// each dialogue turn is assigned a number
	var who			: int = -1;		// index to actor speaking this turn
	var isChoice	: boolean;		// is this plain text or a choice that needs making
	var requirements =new Array();	// keys required for this text to display
	var next 		= new Array();	// which turn to display after this one
	var keys 		= new Array();	// keys to modify if this turn is displayed
	var line 		= new Array();	// the actual dialogue to display this turn
}

//requirements class
class crDialogueReq
{
	var kind	: String;			// what kind of key is this: + or - (must have? or must not have?)
	var name	: String;			// key name to test against
	var value	: int;				// value key must meet
	var redir	: int;				// turn to redirect to if test fails
}

//basic key class
class crDialogueKey
{
	var kind	: String;			// key type (- or + or other)
	var name	: String;			// the name of the key
	var value	: String;			// and it's value
}

//the actor class contains info about the actors participating in the dialogue
class crActor extends Object
{
	var player : String;			// the name of the actor
	var avatar : Texture2D;			// the image to use during his/her dialogue turns
	
	function getAvatar() : Texture2D
	{
		return avatar;
	}
	
	function getPlayer() : String
	{
		return player;
	}
	
	function crActor(name: String, image: String)
	{
		player = name;
		//avatar = Resources.Load(image, Texture2D);
	}
}

// the main dialogue class
// since only one dialogue can take place at a time, make most of the variables static
// to save on variable memory when instantiated multiple instances of the dialogue engine
class _crDialogue extends Object
{

static var FileIsLoaded				: boolean = false;			// test if there was a problem opening the specified filename

private var formattedText 			: String;					// if current line is text only, this will contain linesToShow as 1 string
private var linesToShow				: String[];					// from currentLine, starting at startAtIncrement, up to maxLinesToShow
	
static var maxLinesToShow 			: int = 20;					// only show this many lines of text at a time even if there are more this turn
static var lastIdCreated 			: int = -1;					// when automatically incrementing id numbers during file read
static var lastActorToSpeak 		: int = -1;					// when splitting large dialogue turns
static var startAtIncrement 		: int = -1;					// if continuing a dialogue turn, skip the previous text
static var mustProcessReq 			: boolean = false;			// has this turn's requirements been processed yet?

static var lines					= new Array();				// all imported dialogue lines, broken into it's parsed base elements
static var actors					= new Array();				// list of all actors participating in conversation

static var currentlySpeakingLine	= -1;						// numeric index to what is being spoken
static var selection 				: int = -1;					// when selecting options, this is the currently selected one
static var currentLine	 			: crDialogueLine = null;	// the line being spoken, complete with access to all RAW data

static var avatarImages				: Texture2D[];	

//gameKeysInUse is left exposed to allow easy access to change mid dialogue. Use with caution...
var gameKeysInUse		 			: crGameKeys = null;		// the crGameKeys object to modify

// These variables can either be set in the inspector or set through scripts
var parent							: GameObject;				// object to receive messages / instructions from the dialogue
var FileName						: String;					// file that contains the dialogue

//initialize the class
//since nothing can be done until a file is loaded,
//all that needs to be done is for the static array to be created
//according to the specified value for maxLinesToShow
function _crDialogue()
{
	linesToShow = new String[maxLinesToShow];
}

//getter function for FileeIsLoaded. Not needed but here if you want it.
function fileIsLoaded() : boolean
{
	return FileIsLoaded;
}

//return all lines of text as separate array entries
//only return the text to display this turn
function GetUnformattedText() : String[]
{
	return linesToShow;
}

//return the text to show this turn as a single string with "\n" embedded
//as an error detection method, if this turn is a list of choices, return nothing 
function GetFormattedText() : String
{
	if ( ! currentIsChoice() )
		 return formattedText;
	else return "";
}

//if current turn is a list of options, what option is currently selected?
function GetSelectionIndex() : int
{
	return selection;
}

//return the complete record of this dialogue turn
//useful if RAW access to current line is needed
function GetCurrentLine() : crDialogueLine
{
	return currentLine;
}

//make sure the system knows nothing is selected. Delete anything that is...
function ResetDialogue()
{
	EndDialogue();
	FileIsLoaded	= false;
	destroy();
}

//used during the parsing of a file
//add this dialogue turn to the array of turns
function addLine(id: int, multiChoice: boolean)
{
	//if another turn was processed before this one indicate which actor spoke last. If this is the first
	//turn processed, initialize the actor vars. The purpose of this is to enable dialogue developers
	//to skip the [who] tag at every entry. If this turn is spoken by the same actor as the line before, wether as
	//part of the previous dialogue or not, the [who] tag is optional
	if (lines.length > 0)
	{
		lastActorToSpeak	= currentLine.who;
		lastIdCreated		= currentLine.id;
	} else
	{
		//the first line must be initialized with an actor. if none is selected, instead of giving an error,
		//hard code the first line of dialogue to the first actor.
		lastActorToSpeak = 0;
		lastIdCreated = -1;
	}
	
	//error avoidance. Since each turn has to have a unique id, make sure this is the case...
	if (id == lastIdCreated) id = lastIdCreated + 1;

	//now create the dialogue turn object and give it the processed values
	currentLine = new crDialogueLine();
	currentLine.who = lastActorToSpeak;
	currentLine.id = id;
	currentLine.isChoice = multiChoice;
	
	//NEXT points to where dialogue should brach to. since options branch to various points in the dialogue
	//NEXT needs to be an array. If this turn is plain text then the NEXT array will always have a length of 1
	currentLine.next.Add(id + 1);

	lines.Add(currentLine);
}

//turns have positive numbers as ID tags so -1 is hard coded as the QUIT redirection
//this function tests if the dialogue has ended by testing if the engine is trying to
//read the turn with id -1
function HasEnded() : boolean
{
	return (currentlySpeakingLine == crDialogue.gkNone);
}

//for plain text turns, where must the dialogue skip to next?
function IDOfNextLine() : int
{
	return currentLine.next[0];
}
	
//get the avatar image for the actor speaking this turn	
function whoIsSpeakingAvatar() : Texture2D
{
	var o : crActor = actors[currentLine.who];
	return o.getAvatar();
}

//get the avatar of the actor at a given index
function getAvatar(index)
{
	var o : crActor = actors[index];
	return o.getAvatar();
}

//get the numeric index for the actor speaking this turn	
function whoIsSpeakingID() : int
{
	return currentLine.who;
}

//get the avatar name for the actor speaking this turn
//index is an int field but due to engine restrictions and extended use of this function
//this should not be coded. This function is used with a value of 0 to get the name of
//the currently speaking actor or with a value > 0 to get the name of an actor speaking
//a turn at an indexed position. This function was modified in this way for the sake of
//the dialogue dump function to enable dialogue verification tests
function whoIsSpeakingName(index) : String
{
	var o : crActor;
	if (index)
	{
		var l : crDialogueLine = lines[index];
		 o = actors[l.who];
	}
	else o = actors[currentLine.who];
	
	return o.getPlayer();
}

function whoIsSpeakingName() : String
{
	whoIsSpeakingName(0);
}

//used to link external game keys to the dialogue. with this function, the inventory
//of a specific character or of the party or the global crGameKeys or any other set of 
//crGameKeys can be linked in. Thsi is optional for basic dialogue but required for any
//dialogue that makes use of the [keys] or [require] tags
function AssignGameKeys(name : crGameKeys)
{
	print("assigning game keys");
	gameKeysInUse = name;
}

//hard code the dialogue to point to a turn with id -1
function EndDialogue()
{	
	currentlySpeakingLine = crDialogue.gkNone;
}

//get the specified dialogue turn or return the first line if the turn was not found.
//with the dialogue's redirection system this will have the effect of ending the conversation
//and restarting again. Although this will mean the last conversation was abruptly ended,
//the redirection system will make it look like the actor has started a new line of conversation
//instead of getting an error or the dialogue simply stopping
function find(id : int) : crDialogueLine
{	
	for (var tmp : crDialogueLine in lines)
		if (tmp.id == id)
			return tmp;
			
	return lines[0];
}

//make your choice from the dialogue options
function OptionSelect(val : int)
{
	//in case the user hits the select button when viewing plain text
	if (currentLine.line.length <= val)
		return false;
	
	//else choose this option
	selection = val;
}

//when selecting a choice from multiple options, select the previous one
function OptionPrevious()
{
	if (selection > 0) selection--;
}

//when selecting a choice from multiple options, select the next one
function OptionNext()
{
	if (selection < (currentLine.line.length - 1))
		selection++;
}

//for debugging purposes
function ScanForId(id: int) : int
{
	var result = 0;
	for (var x : crDialogueLine in lines)
	{
		if (x.id == id)
			return result;
		result++;
	}
	
	return crDialogue.gkNone;
}

//format the current text for disply on screen.
//only maxLinesToShow lines will be displayed at any one time, but a turn might consist of
//any number of lines of text. linesToShow contain a subset of the lines for the current turn
//so this function will return only the text that actually needs to be displayed
function currentText() : String
{
	var result : String;
	for (var s:String in linesToShow)
	{
		if(s == "$")
			result += "\n\n";
		else if(s)
			result += s + "\n";
	}	
		
	return result;	
}

function GetConcatenatedText(separator: String) : String
{
	var result : String;
	for (var s:String in linesToShow)
		result += s + " ";
		
	return result;	
}

//a function to procedurally return the text associated with an option.
//this simplifies positioning and formatting the text by color etc as it allows you to access
//the text of each individual option when you require it to do with it whatever you want to...
function currentOptions(x: int) : String
{
	if (x < maxLinesToShow)
		 return linesToShow[x];
	else return linesToShow[0];
}

//only really useful for debuggin purposes. This is used with the building of the UDE Editor
function currentRequirements() : Array
{
	return currentLine.requirements;
}

//only really useful for debuggin purposes. This is used with the building of the UDE Editor
function currentRedirects() : Array
{
	return currentLine.next;
}

//only really useful for debuggin purposes. This is used with the building of the UDE Editor
function currentKeys() : Array
{
	return currentLine.keys;
}

//test if this turn is a choice
function currentIsChoice()
{
	return currentLine.isChoice;
}

//how many options are there in this turn's specification? Use when currentIsChoice() = true
function currentOptionCount() : int
{
	return currentLine.line.length;
}

//destroy the parsed data from the last read file to free up the memory and prepare for the next
function destroy()
{
	lines.Clear();
	actors.Clear();	
}

// MAIN FUNCTION 1 ///////////////////////////////////////
// OPEN FILE /////////////////////////////////////////////
// call open file to load the file that contains the dialogue
// the file will be parsed and stored as objects in an ordered array.

function OpenFile() : boolean
{
	return OpenFile(FileName);
}

function OpenFile(index) : boolean
{
	return OpenFile(FileName, index);
}

function OpenFile(filename: String) : boolean
{
	return OpenFile(filename, crDialogue.gkContinue);
}


function OpenFile(fn: String, startAt : int) : boolean
{
	var FileResource	: TextAsset;
	var linesArray		: Array;

    var lastActorToSpeak : int = 0;		// All dialogue spoken by same person unless otherwise specified
    var inputMode : int = 0;			// Text/ option/ other etc etc etc...
   	var txt : String[];							// helper var. 
    		    		
    FileIsLoaded = false;
    		    		
    //load the entire file into memory
	FileResource = Resources.Load(fn, TextAsset);
	if (!FileResource)
	{
		Debug.Log("Could not load file: " + fn);
		EndDialogue();
		return;
	}
	
	//now split the file into separate lines for parsing
	linesArray = FileResource.text.Split("\n"[0]);

    // only maxLinesToShow lines will be displayed onscreen at any one time, but the dialogue may contain
    // any amount of lines per turn. This will create space to store the ACTUAL portion of text to display
   	linesToShow = new String[maxLinesToShow];

	   	// Read lines from the file until the end of the file is reached.
	for(var line : String in linesArray)
      	{
      		// in case the user likes to indent his files or leave trailing blanks. A tab character at the end
      		// would indicate a new field and if it is empty it could confuse/ crash the engine. So compensate...
      		// also, empty lines and lines starting with // are ignored
      		line = line.Trim();
      		
	      	if ((line == "") || (line.IndexOf("//") == 0) )
	   		{
      			continue;
	      	}
	      		
	      	//in case the user wants the dialogue numbers to increment automatically
	      	//they need only write "[LINE]" in stead of "[LINE] 3" so ensure parsing format is intact
	      	if (line == "[line]" || line == "[choice]" )
	   			line += "\t" + (lastIdCreated + 1);
	   		
			// ensure tag formatting - [TAG][TAB} ...  except the [actors].
			if ( line[0]=="[" &&  line.IndexOf("]\t") == -1 && line.ToLower().IndexOf("[actors]") == -1 )
			{
				Debug.Log("Syntax Error on line #" + lastIdCreated.ToString() + "\n" + line);
				continue;
			}

	   		//next, split the line into TAG and VALUE
	   		var splitLine = line.Split("\t"[0]);
	   		
	   		//only if there is a TAG should it be made lowercase for testing purposes. Leave text alone
	   		if (splitLine.length > 1);
      		splitLine[0] = splitLine[0].ToLower();

			// for readability sake only. Use firstField instead of splitLine[0]
	      	var firstField = splitLine[0].Trim();

			//could have used a SWITCH statement but... nahh... this is just as readable....
	      	if (firstField == "[actors]")
	      	{
	      		inputMode = 0;
	   		} else
	   		if (firstField == "[line]")
      		{
				inputMode = 1;
				// create a new line, set it as NOT an option list and make it point to the next ID to be created
				addLine(parseInt(splitLine[1]), false);
      		} else
       		if (firstField == "[choice]")
	      	{
				inputMode = 2;
				// create a new line, set is an an option
				addLine(parseInt(splitLine[1]), true);
	      	} else
	      	if (firstField == "[who]")
	   		{
	   			// a line must exist before a [WHO] can be assigned
	   			// all dialogue will be spoken by the last character to speak until a new actor is selected
				if (currentLine)
				{
					if (splitLine[1] == "")
						 currentLine.who = lastActorToSpeak;
					else currentLine.who = parseInt(splitLine[1]);

					lastActorToSpeak = currentLine.who;
				}				
	      	} else
	      	if (firstField == "[next]")
	   		{
	   			// a line must exist before a [NEXT] can be assigned
				if (currentLine)
				{
					//when a new line is created, NEXT is automatically set to point to the next ID so
					//when an explicit [NEXT] is found, simply overwrite the auto generated value
					currentLine.next[0] = parseInt(splitLine[1]);
				}
	      	} else
	      	if (firstField == "[require]")
	   		{
	   			// a line must exist before a [REQUIRE] can be assigned
				if (currentLine)
				{
					for (var x = 1; x < splitLine.length; x++)
					{
						txt = splitLine[x].Split(" "[0]);
					    if ( txt.length < 4 || txt.length % 4)
						{
							Debug.Log("Syntax Error for [REQUIRE] on line #" + lastIdCreated.ToString() + "\n" + splitLine[x]);
						} else
						{
							var tmpReq = new crDialogueReq();
							tmpReq.kind  = txt[0];				// mustHave or mustNotHave
							tmpReq.name  = txt[1];				// of What
							tmpReq.value = parseInt(txt[2]);	// how much?
							tmpReq.redir = parseInt(txt[3]);	// where to redirect to if test fails
							currentLine.requirements.Add(tmpReq);
						}
					}							 
				}
	   		} else
	   		if (firstField == "[keys]")
	      	{
	   			// a line must exist before a [KEYS] can be assigned
				if (currentLine)
				{
					for(x = 1; x < splitLine.length; x++)
					{
						txt = splitLine[x].Split(" "[0]);
					    if ( txt.length < 3 || txt.length % 3)
						{
							Debug.Log("Syntax Error for [KEY] on line #" + lastIdCreated.ToString() + "\n" + splitLine[x]);
						} else
						{							
							var tmpKey = new crDialogueKey();
							tmpKey.kind  = txt[0];		// gameKey or gameAction?
							tmpKey.name  = txt[1];		// name of key or action to take?
							tmpKey.value = txt[2];		// key value or optional action option
							currentLine.keys.Add(tmpKey);
						}
					}
				}
	      	} else
	      	{
	      		// if a line of text is read without a TAG, assume it is one of the lines of text to display
				switch (inputMode)
					{
						// mode 0 is where actors are defined for participation in the dialogue
						case 0:
						//splitLine[0] was converted to lower case. Get capitals again...
							var capText : String[] = line.Split("\t"[0]);
							var newActor = new crActor(capText[0].Trim(), splitLine[1]);
							actors.Add(newActor);
							break;

						// mode 1 is for normal text lines
						case 1:
							if (currentLine)
							{
								currentLine.line.Add( line );
							}
							break;

						// mode 2 is when dealing with OPTIONS lines
						case 2:
							if (currentLine)
							{
								// split the line into REDIRECT_TAG and TEXT_TO_SHOW
								currentLine.line.Add( splitLine[1] );
								
								// since I automatically created a NEXT field when I created the line, there is
								// already 1 value here and needs to be replaced. after adding the text to the
								// list, and before adding the redirect, if the number of lines and the number of
								// redirects match, it means that this is the first line I am adding so instead of
								// adding the current REDIRECT_TAG, instead overwrite the current one.
								// This test will obviously only return true for the first line of text in an OPTION
								if (currentLine.line.length == currentLine.next.length)
									 currentLine.next[0] = parseInt(splitLine[0]);
								else currentLine.next.Add( parseInt(splitLine[0]) );
							}
							break;
					}
	      	}
        }
		
     if (lines.length == 0)
     {
        print("No dialogue was defined for this dialogue...");
       	return false;
     }

     if (actors.length == 0)
     {
        print("No actors were defined for participation in this dialogue...");
       	return false;
     }

	FileIsLoaded = true;
	Speak(startAt);
    return true;
} //open file
		

// MAIN FUNCTION 2 ///////////////////////////////////////
// SPEAK /////////////////////////////////////////////////
// Speak does all the grunt work. It selects the dialogue turns to display,
// it tests for redirection, automatically redirects if need be, modifies game keys
// if required and sends messages to the parent object if instructed to do so by
// the dialogue file. Once the file is loaded, programmers need only call SPEAK
// until the dialogue has ended and it will do the rest.
// It selects subsets of dialogue text from large turns and continues on to the next
// turn when required.

function Speak() : int
{
	return Speak(crDialogue.gkContinue);
}

function Speak(id : int) : int
{
	var tempLine : crDialogueLine;
	var tempKey : crDialogueKey;
	var tempReq : crDialogueReq;
try {		
		//if we are instructed to quit the conversation
		if (id == -1)
		{
			EndDialogue();
			return;
		}

		//if no line number is provided, try and determine the line
		if (id == crDialogue.gkContinue)
		{
			//first see if we were reading something the previous turn...
			if (currentlySpeakingLine > -1)
			{
				id = currentlySpeakingLine;
			}
			//if not, then assign to first ID
			else
			{
				tempLine = lines[0];
				id = tempLine.id;
			}
		
			//if we are currently speaking the same line as before
			if (id == currentlySpeakingLine)
			{
				startAtIncrement++;
				if (currentLine.line.length > (startAtIncrement * maxLinesToShow))
				{//continue to show this ID
					var continueFrom = id;
					mustProcessReq = false;
				} else
				{
					if (currentLine.isChoice)
						 continueFrom = currentLine.next[selection];
					else continueFrom = currentLine.next[0];
					
					startAtIncrement = 0;
					mustProcessReq = true;
				}
			} else
			//if not, find the new one
			{
				mustProcessReq = true;
				startAtIncrement = 0;
		
				// first determine wether we were speaking at all...
				if (currentlySpeakingLine == -1)
				{
					var o : crDialogueLine = lines[0];
					continueFrom = o.id;
				} else
				//and if we were, check what comes next...
				{
					if (currentLine.isChoice)
					{
						 continueFrom = currentLine.next[selection];
					} else
					{
						continueFrom = currentLine.next[0];
					}
				}
			} // find the new ID
		} // if no id was provided
		else
		{ // if ID was given to us, we HAVE to use it
			// if it is the same as the last ID
			if (id == currentlySpeakingLine)
			{
				//if there is nothing more to read here... This is an error... Quit!!!
				if (currentLine.line.length > (startAtIncrement * maxLinesToShow))
				{
					startAtIncrement++;
					mustProcessReq = false;
				} else
				{
					print("Opted to quit rather than speak line " + id);
					EndDialogue();
					return -1;
				}
			} else
			//if it is a new ID
			{
				startAtIncrement = 0;
				mustProcessReq = true;
			}
			continueFrom = id;
		}
	
		//see if we are being redirected to an end of conversation
		if (continueFrom == -1)
		{
			EndDialogue();
			return crDialogue.gkNone;
		}

		//now that we know what we want, let's go get it...
		if (continueFrom != currentlySpeakingLine)
		{
			selection = 0;
		
			currentLine = find(continueFrom);
			currentlySpeakingLine = currentLine.id;
			startAtIncrement = 0;
		}

		//first process the requirements for this id
		if (gameKeysInUse && mustProcessReq && (currentLine.requirements.length > 0))
		{
			for(var x= 0; x < currentLine.requirements.length; x++)
			{
				tempReq = currentLine.requirements[x];
				switch(tempReq.kind)
				{
					case "+":
						if (gameKeysInUse.doesNotHave(tempReq.name, tempReq.value))
						{
							Speak(tempReq.redir);
							return crDialogue.gkFalse;
							
						} 
						break;
					case "-":
						if (gameKeysInUse.doesHave(tempReq.name, tempReq.value))
						{
							Speak(tempReq.redir);
							return crDialogue.gkFalse;
						} 
						break;
				}
			}
		}

		//now modify the keys	
		if ( gameKeysInUse != null && mustProcessReq && (currentLine.keys.length > 0))
		{
			for(x= 0; x < currentLine.keys.length; x++)
			{
				tempKey = currentLine.keys[x];
				switch(tempKey.kind)
				{
					case "+":
							gameKeysInUse.add(tempKey.name, parseInt(tempKey.value));
							break;
					case "-":
							gameKeysInUse.subtract(tempKey.name, parseInt(tempKey.value));
							break;
					default :
							var tempString : String = tempKey.kind +" " +tempKey.name + " " + tempKey.value;
							parent.BroadcastMessage("ProcessKeys", tempString, SendMessageOptions.DontRequireReceiver);
				}
			}
		}


		continueFrom = maxLinesToShow * startAtIncrement;

		for (x=0; x < maxLinesToShow; x++)
		{
			if (continueFrom < currentLine.line.length)
				 linesToShow[x] = currentLine.line[continueFrom];
			else linesToShow[x] = "";
			continueFrom++;
		}
	
		if (!currentLine.isChoice)
			formattedText = currentText();
	
		return crDialogue.gkTrue;
	}
	
	catch(e)
	{
		Debug.Log("There was an error when requesting ID: " + id + " continueFrom set to:" + continueFrom);
	}
}

}

