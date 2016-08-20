using UnityEngine;
using System.Collections;
using ION.Core.Extensions;

public class CharacterInteraction: MonoBehaviour
{
	GameObject User;
	TextMesh tm;
	private string _name;
	string _speech;
	
	void Start ()
	{
			User = GameObject.Find ("User");
		tm=User.GetComponentInChildren<TextMesh>();
        if (this.transform.parent)
		_name = this.transform.parent.name;
        else _name = "";
	}
	
	void Update ()
	{
	}
	
	// Mouse Up gives actions to the character
	void OnMouseOver ()
	{
		
		// Get User GameObject
	
		
		if (Input.GetMouseButton (0)) {
          

			// Get IONAction script
			Attack attack = User.GetComponent<Attack> ();
		
			// Create Action's Parameters
			ActionParameters args = new ActionParameters ();
		
			args.ActionType = attack.Action.Name;
			args.Subject = "User";
		
			args.Target = _name;
			// Start Action with the previous Parameters
			attack.Action.Start (args);
          
			_speech="User  attacks "+_name;
		}
		if (Input.GetMouseButton (2)) {
           

			// Get IONAction script
			UserSpeech say = User.GetComponent<UserSpeech> ();
		
			// Create Action's Parameters
			ActionParameters args = new ActionParameters ();
		
			args.ActionType = say.Action.Name;
			args.Subject = "User";
			args.Target = _name;
	
			string totem = "school";
			IONStringProperty[] props = User.GetComponents<IONStringProperty> ();
			foreach (IONStringProperty prop in props) {
				if (prop.name == "totem")
					totem = prop.propertyValue;
			}
			args.Parameters.Add (totem);
		
			// Start Action with the previous Parameters
			say.Action.Start (args);
			_speech="User talks to "+_name+" as "+totem;
		}
		if (Input.GetMouseButton (1)) {


			// Get IONAction script
			RunAway runaway = User.GetComponent<RunAway> ();
		
			// Create Action's Parameters
			ActionParameters args = new ActionParameters ();
				
			args.ActionType = runaway.Action.Name;
			args.Subject = "User";
			args.Target = _name;
			args.AddParameter("clinic");
		
			// Start Action with the previous Parameters
			runaway.Action.Start (args);
			_speech="User runs away";
		}
		
		if (tm!=null){
			
			tm.text=_speech;
		}
	}
}
