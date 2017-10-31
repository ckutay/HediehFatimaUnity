var salsa3D : CrazyMinnow.SALSA.Salsa3D;
private var caffeine_audio = new Array();
private var dlg : talkingHeadDialogueStyle;
private var next_speech_bubble = false;
private var next_choice = false;
private var playing = true;

var progressfg			: Texture2D;
var progressbg			: Texture2D;
private var cur_progress	: int;
private var next_progress	: int;
private var progress_sections	:int;

var thisFont				: GUIStyle;

private var questions : String[];
private var treatments : String[];
private var stoolchart : Texture2D;
private var stoolAnswers :boolean[];
private var treatmentAnswers :boolean[];
private var atstart = true;

private var anim : Animator;
private var shake= false;


function Awake()
{
	dlg = GetComponent(talkingHeadDialogueStyle);
		//get all the audio dialogue files
	/*caffeine_audio= Resources.LoadAll("audio_files/caffeine", AudioClip);
	for(var c : AudioClip in caffeine_audio)
		print(c + ", "+c.name);*/
	//InitialiseQuestions();
	
	//anim = GetComponent("Animator");
	
		
		
}




function SpeakTo ()
{
	if (dlg)
	{
		
		Globals.playMode = ePlaymode.Dialogue;
		dlg.enabled = true;
		dlg.StartDialogue(Globals.MainKeys);
		
	}
	

}

function Salsa_OnTalkStatusChanged(status : CrazyMinnow.SALSA.SalsaStatus) {
		/*Debug.Log("Salsa_OnTalkStatusChanged:" +
			" instance(" + status.instance.GetType() + ")," +
			" talkerName(" + status.talkerName + ")," +
			((status.isTalking) ? "started" : "finished") + " saying " + status.clipName);*/
			if(next_speech_bubble)
			{
				if(!status.isTalking)//once the clip is finished show next speech bubble
				{
					dlg.DialogueHUD.Speak();

				}
			}
	}

/*function SaveAnswer(answer)
{
	var questionID = answer[0];
	var answerID = answer[1];
	Debug.Log("saving answer: "+questionID+": "+answerID);
	var form = new WWWForm();
 	form.AddField("questionID", questionID);
 	form.AddField("answerID", answerID);
	
 	var save_answer = new WWW("https://comp.mq.edu.au/vworlds/DrEvie/eADVICE_functions.php",form);

	yield save_answer;
	

}	*/

function ProcessKeys(params : String)
{
	var Args	: Array = params.Split(" "[0]);
	dlg.DialogueHUD.displayFormItem = false;
	dlg.DialogueHUD.displaySingleAnswer = false;
	dlg.DialogueHUD.displayMultipleAnswer = false;
	switch(Args[0])
	{
		case "play_audio":
			
			var index = Args[2];
			var t : String = index.ToString();
			
			dlg.DialogueHUD.currentQuestionNum = Args[1]+"_"+t;
			
			var line = Resources.Load("audio_files/"+Args[1]+"/"+Args[1]+t, AudioClip);
			if(line)
			{
				salsa3D.SetAudioClip(line);
				salsa3D.Play();
				playing = true;
				
			}
			else
				playing = false;
			
			/*var i = parseInt(t);
			
			if(caffeine_audio.length >= i)
			{
				i = i-1;
				salsa3D.SetAudioClip(caffeine_audio[i]);
				salsa3D.Play();
			}*/
				
		
		break;
		case "display_next":
			
			if(Args[1] == "audio") //make the audio show the next speech bubble when it stops playing
			{
				next_speech_bubble = true;
				next_choice = false;
				dlg.DialogueHUD.displaychoice = false;
			}
			else if(Args[1] == "choice"){
				next_speech_bubble = true;
				dlg.DialogueHUD.displaychoice = true;
			}
			else {
				next_speech_bubble = false;
				dlg.DialogueHUD.displaychoice = false;
			}
		break;
		case "numchoices":
			next_progress = cur_progress = 0;
			var num = Args[1];
			var tmp : String = num.ToString();
			progress_sections = parseInt(tmp);
		break;
		case "choicenum":
			num = Args[1];
			tmp  = num.ToString();
			next_progress = parseInt(tmp);
			cur_progress = next_progress - 1; 
		break;
		/*case "playanim":
			if(Args[1] == "shake")
			{
				anim.SetBool("shake", true);
				anim.SetBool("idle",false);
				anim.SetBool("nod",false);
				//anim.SetBool("idle",true);
				
			}
			if(Args[1] == "idle")
			{
				anim.SetBool("idle", true);
				anim.SetBool("shake",false);
				anim.SetBool("nod",false);
				
			}
		break;*/
		case "showquestion":			
			var tmp1 = Args[1];
			var qnum : String = tmp1.ToString();
			var q = parseInt(qnum);
			var questionstring : String = questions[q-1];
			var questionArray = questionstring.Split("~"[0]);
			dlg.DialogueHUD.displayFormItem = true;
			dlg.DialogueHUD.displaySingleAnswer = true;
			dlg.DialogueHUD.displayQuestion = questionArray;
		break;
		case "showcheckbox":
			dlg.DialogueHUD.displayFormItem = true;
			dlg.DialogueHUD.displayMultipleAnswer = true;
			if(Args[1] == "chart")
			{
				dlg.DialogueHUD.displayImage = Resources.Load("stoolchart", Texture2D);
				dlg.DialogueHUD.checkboxAnswers = [false,false,false,false,false,false,false];
			}
			else if(Args[1] == "treatment")
			{	
				dlg.DialogueHUD.displayQuestion = treatments;
				dlg.DialogueHUD.checkboxAnswers = treatmentAnswers;
				dlg.DialogueHUD.displayImage = null;
			}
				
		break;
	}
}

function OnGUI()
{
		/** draw progress bar **
	var progressbgRect : Rect = Rect(Screen.width - 255, 5, 250, 15);
	GUI.DrawTexture(progressbgRect, progressbg);
	GUI.Label(Rect(Screen.width - 310, 5, 50, 30), "Progress", thisFont);
	//Debug.Log(progress	+" "+ progress_sections);
	if(next_progress && progress_sections)
	{
		//Debug.Log(progress	+" "+ progress_sections);
		if(dlg.DialogueHUD.whoIsSpeakingID() == 0)
		{
			cur_progress = next_progress; //don't want to update the progress if the user is still choosing an answer
			//print(cur_progress +" "+ next_progress);
		}
		if(dlg.DialogueHUD.HasEnded())
		{
			cur_progress = progress_sections = next_progress = 1;
		}
		var progressRect : Rect = Rect(Screen.width - 255, 5, 250*cur_progress/progress_sections, 15); 
		GUI.DrawTexture(progressRect, progressfg);
	}
	/***end progress bar ***/
	
}

/******this would usually be in EvieDialogueStyle, but we want to check if the audio is playing, so it's easier to just have it here****/
function Update()
{
	if (!dlg.DialogueHUD.HasEnded())
	{
		atstart = false;
		if (dlg.DialogueHUD.currentIsChoice())
		{
			if (Input.GetKeyDown("return"))
			{
				dlg.DialogueHUD.Speak();
				if (dlg.DialogueHUD.HasEnded())
					dlg.OnDialogueEnded();
			}
			if (Input.GetKeyDown("up"))
			{
				dlg.DialogueHUD.OptionPrevious();
			}
			if (Input.GetKeyDown("down"))
			{
				dlg.DialogueHUD.OptionNext();
			}
		} 
		
		else
		{
			if (Input.GetKeyDown("return") || Input.GetMouseButtonDown(0))
			{
				if(!playing)
				{	
					dlg.DialogueHUD.Speak();
				
					if (dlg.DialogueHUD.HasEnded())
						dlg.OnDialogueEnded();
				}
			}
		}
	}
	else if(!atstart)
	{
		salsa3D.Stop();
		dlg.OnDialogueEnded();
	}
	 /*var stateInfo : AnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
	 if(stateInfo.IsName("Shake"))
	 {
	 
				anim.SetBool("shake",false);
				anim.SetBool("nod",false);
				anim.SetBool("idle",true);
	}*/
}
