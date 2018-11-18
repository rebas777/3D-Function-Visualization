using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class CaptureBtnReceiver : InteractionReceiver {

    private InputField inputField;
    private GameObject captureMenu;
    private Manager manager;
    private GameObject inputPanel;

    // Use this for initialization
    void Start () {
        inputField = GameObject.Find("InputField").GetComponent<HoloToolkit.UI.Keyboard.KeyboardInputField>();
        captureMenu = GameObject.Find("CaptureButtons");
        manager = GameObject.Find("manager").GetComponent<Manager>();
        inputPanel = GameObject.Find("KeyboardCanvas");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void InputClicked(GameObject obj, InputClickedEventData eventData) {
        switch(obj.name) {
            case "HolographicButton(capture)": {
                    Debug.Log("photo taken");


                    captureMenu.SetActive(false);
                    inputPanel.SetActive(false);
                    break;
                }
            
        }
    }

}
