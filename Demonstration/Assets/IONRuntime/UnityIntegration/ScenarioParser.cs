/*
 * Henrique Campos made this (07-2011)
 * 
 * Parses scenarios' xml file from FAtiMA and generates the characters' info
 * from a certain scenario.
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FAtiMA.RemoteAgent;
using System.Xml;
using System.Xml.Schema;
using System;

public class ScenarioParser : XmlParser {

	// Instance
	public static readonly ScenarioParser Instance = new ScenarioParser();
		
	// Scenario info class - stores agents
	public class Scenario {
		private string _name;
		private int _port;
		
		private Dictionary<string, Character> _characters;
		private List<string> _charactersNames;
		
		internal Scenario(string name, int port){
			_name = name;
			_port = port;
			_characters = new Dictionary<string, Character>();
			_charactersNames = new List<string>();
		}
		
		public string Name { get { return _name; } }
		public int Port { get { return _port; } }
		
		public Character GetCharacter(string name){
			return _characters[name];
		}
		
		internal void AddCharacter(Character character){
			_charactersNames.Add(character.Name);
			_characters.Add(character.Name, character);
		}
	}
	
	// Character info class
	public class Character {
		private string _name;
		private int _port;
		private string _role;
		private string _sex;
		private Dictionary<string, string> _properties;
		private List<string> _propNames;
		
		public string Name { get { return _name; } }
		public int Port { get { return _port; } }
		public string Role { get { return _role; } }
		public string Sex { get { return _sex; } }
		public List<string> Properties { get { return _propNames; } }
	
		internal Character(string name, int port, string role, string sex){
			_name = name;
			_port = port;
			_role = role;
			_sex = sex;
			_propNames = new List<string>();
			_properties = new Dictionary<string, string>();
		}
		
		public string GetPropertyValue(string key){
			return _properties[key];
		}
		
		internal void AddProperty(string key, string val){
			_properties.Add(key, val);
			_propNames.Add(key);
		}
		
		public override string ToString(){
			string str = "Agent name='" + Name + "' role='" + Role + "' sex='" + Sex + "' port='" + Port + "' properties[";
			foreach(string p in Properties){
				str += " '" + p + "'='" + GetPropertyValue(p) + "'";
			}
			str += " ]";
			return str;
		}
	}
	
	private Dictionary<string, Dictionary<string, Scenario>> _files;
	
	private ScenarioParser(){
		_files = new Dictionary<string, Dictionary<string, Scenario>>();
	}
	
	public Scenario GetScenario(string filepath, string scenario){
		if(!_files.ContainsKey(filepath)){
			Dictionary<string, Scenario> scenarios = (Dictionary<string, Scenario>) ParseFile(filepath);
			_files.Add(filepath, scenarios);
		}
		
		if(!(_files[filepath]).ContainsKey(scenario)){
				//TODO : error throw
				return null;
		}
		
		return (_files[filepath])[scenario];
	}
	
	#region implemented abstract members of XmlParser
	protected override void XmlErrorsHandler(object sender, ValidationEventArgs args)
	{
	    // TO DO: deal with xml errors
	    Console.WriteLine("Validation error: " + args.Message);
	}
	
	protected override object ParseElements (XmlDocument xml){
		Dictionary<string, Scenario> scenarios = new Dictionary<string, Scenario>();
		
		// Find the scenario node matching the scenario we want ..
		foreach (XmlNode node in xml.DocumentElement.ChildNodes)
        {
			if(node.Name.CompareTo("Scenario") == 0){
				Scenario scenario = ParseScenario(node);
				scenarios.Add(scenario.Name, scenario);
			}
        }
						
		return scenarios;
	}
	
	protected override void ParseElements (XmlDocument xml, object elements){
		elements = ParseElements(xml);
	}
	#endregion
	
	private Scenario ParseScenario(XmlNode scenarioNode){
		string name = scenarioNode.Attributes["name"].Value;
		int port = 0; //TODO
		Scenario scenario = new Scenario(name, port);
		
		// Find the agent nodes ..
		foreach(XmlNode node in scenarioNode.ChildNodes){
			if(node.Name.CompareTo("Agent") == 0){
				Character character = ParseAgent(node);
				scenario.AddCharacter(character);
			}
		}
		
		return scenario;
	}
	
	private Character ParseAgent(XmlNode agentNode){
		string name = agentNode.Attributes["name"].Value;
		string role = agentNode.Attributes["role"].Value;
		int port = Int32.Parse(agentNode.Attributes["port"].Value);
		string sex = agentNode.Attributes["sex"].Value;
		
		Character character = new Character(name, port, role, sex);
		
		/*
		// Get the Properties ..
		XmlNode properties = agentNode["Properties"];
		foreach(XmlNode property in properties){
			if(property.Name.CompareTo("Property") == 0){
				string pName = property.Attributes["name"].Value;
				string pValue = property.Attributes["value"].Value;
				character.AddProperty(pName, pValue);
			}
		}
		
		Debug.Log("@@ " + character.ToString());
		*/
		return character;
	}
}
