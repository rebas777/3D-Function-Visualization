using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class InputBtnReceiver : InteractionReceiver {

    private InputField inputField;
    private GameObject inputMenu;
    private Manager manager;
    private GameObject inputPanel;


	// Use this for initialization
	void Start () {
        inputField = GameObject.Find("InputField").GetComponent<HoloToolkit.UI.Keyboard.KeyboardInputField>();
        inputMenu = GameObject.Find("InputButtons");
        manager = GameObject.Find("manager").GetComponent<Manager>();
        inputPanel = GameObject.Find("KeyboardCanvas");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void InputClicked(GameObject obj, InputClickedEventData eventData) {
        switch(obj.name) {
            case "HolographicButton(3D)": {
                    string inputStr = inputField.text;
                    if(inputStr != null) {
                        // call draw function
                        manager.GetComponent<Manager>().calculateInput(inputStr);
                    }
                    inputMenu.SetActive(false);
                    inputPanel.SetActive(false);
                    break;
                }
            case "HolographicButton(2D)": {
                    string inputStr = inputField.text;
                    if(inputStr != null) {
                        // call draw function
                        manager.GetComponent<Manager>().calculate2dInput(inputStr);
                    }
                    inputMenu.SetActive(false);
                    inputPanel.SetActive(false);
                    break;
                }
            case "HolographicButton(Implicit)": {
                    inputMenu.SetActive(false);
                    inputPanel.SetActive(false);
                    break;
                }
        }
    }
}
