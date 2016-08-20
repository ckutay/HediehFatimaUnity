using UnityEngine;
using System.Collections;

public class ChangeLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<TextMesh>().fontSize = 20;
        GetComponent<TextMesh>().color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0))
            Application.LoadLevel ("Kinship"); 
        if(Input.GetMouseButtonDown(1))
            Application.LoadLevel ("Kinship"); 
        if(Input.GetMouseButtonDown(2))
            Application.LoadLevel ("Kinship"); 
	}
}
