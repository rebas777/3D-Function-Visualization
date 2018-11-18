using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ButtonController : MonoBehaviour {
    public string mystring;
    TextMesh inputtext;
	// Use this for initialization
	void Start () {
        GameObject go = GameObject.FindWithTag("InputText");
        inputtext = go.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        if (mystring == "backspace")
        {
            int length = inputtext.text.Length;
            if (length == 0) { return; }
            if (length - 2 >= 0 && inputtext.text[length - 2] <= 'z' && inputtext.text[length - 2] >= 'a' && inputtext.text[length - 1] == '(')
            {
                inputtext.text = inputtext.text.Substring(0, length - 4);
            }
            else
            {
                inputtext.text = inputtext.text.Substring(0, length - 1);
            }
        }
        else if (mystring == "ac") {
            inputtext.text = "";
        }
        else
        {
            inputtext.text += mystring;
        }
    }
}
