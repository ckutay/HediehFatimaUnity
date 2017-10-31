using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_write_DB : MonoBehaviour {

	// Use this for initialization
	void Start () {

          
        StartCoroutine(SaveToDB());


    
}
    IEnumerator SaveToDB()
    {
        string url = "http://comp.mq.edu.au/vworlds/save_stress_data.php";
        WWWForm form = new WWWForm();
        form.AddField("question", 1); //question_number should be an integer (e.g. 1 for Q1, 2 for Q2 etc)
        form.AddField("answer", "please work"); //answer_value should be a string e.g. "They're not good. I just can't seem to get anything done."
        form.AddField("username", "test-sarah"); /*username_value should be a string. If you don't want to record a user ID, just put an empty string here: ""*/

        WWW w = new WWW(url, form);

        yield return w;

         print(w.text);
        Debug.LogWarning(url);
        Debug.LogWarning(w.text);
        Debug.LogWarning("Hedieh");

    }
    // Update is called once per frame
    void Update () {
		
	}
}
