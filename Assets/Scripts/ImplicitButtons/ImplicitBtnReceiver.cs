using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class ImplicitBtnReceiver : InteractionReceiver {

    private InputField inputField;
    private GameObject implicitMenu;
    private Manager manager;
    private GameObject inputPanel;
    public CalculatorWrapper cw;

    // Use this for initialization
    void Start () {
        inputField = GameObject.Find("InputField").GetComponent<HoloToolkit.UI.Keyboard.KeyboardInputField>();
        implicitMenu = GameObject.Find("ImplicitButtons");
        manager = GameObject.Find("manager").GetComponent<Manager>();
        inputPanel = GameObject.Find("KeyboardCanvas");
        cw = GameObject.Find("manager").GetComponent<CalculatorWrapper>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void InputClicked(GameObject obj, InputClickedEventData eventData) {
        switch(obj.name) {
            case "HolographicButton(3D)": {
                    Debug.Log("start draw 3d implicit function");
                    implicitMenu.SetActive(false);
                    inputPanel.SetActive(false);

                    cw.calculate(inputField.text);
                    break;
                }
            case "HolographicButton(2D)": {
                    Debug.Log("start draw 2d implicit function");
                    implicitMenu.SetActive(false);
                    inputPanel.SetActive(false);

                    cw.calculatevec(inputField.text);
                    break;
                }
        }
    }

}
