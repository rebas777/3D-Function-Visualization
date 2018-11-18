using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;

public class ShaderBtnReceiver : InteractionReceiver {

    private GenerateMesh gm;
    private GameObject shaderMenu;

	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GeneratedMesh").GetComponent<GenerateMesh>();
        shaderMenu = GameObject.Find("ShaderButtons");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void FocusEnter(GameObject obj, PointerSpecificEventData eventData) {
        //Debug.Log(obj.name + " : FocusEnter");
    }

    protected override void FocusExit(GameObject obj, PointerSpecificEventData eventData) {
        //Debug.Log(obj.name + " : FocusExit");
    }

    protected override void InputDown(GameObject obj, InputEventData eventData) {
        //Debug.Log(obj.name + " : InputDown");
        switch(obj.name) {
            case "HolographicButton(wireframe)": {
                    gm.SetShadeMode(GenerateMesh.ShadeMode.wireframe);
                    break;
                }
            case "HolographicButton(standard)": {
                    gm.SetShadeMode(GenerateMesh.ShadeMode.normal);
                    break;
                }
            case "HolographicButton(tone_based)": {
                    gm.SetShadeMode(GenerateMesh.ShadeMode.tone);
                    break;
                }
            case "HolographicButton(alpha)": {
                    gm.SetShadeMode(GenerateMesh.ShadeMode.alpha);
                    break;
                }
            case "HolographicButton(holographic)": {
                    gm.SetShadeMode(GenerateMesh.ShadeMode.holo);
                    break;
                }
            case "HolographicButton(hide)": {
                    shaderMenu.SetActive(false);
                    break;
                }
        }
    }

    protected override void InputUp(GameObject obj, InputEventData eventData) {
        //Debug.Log(obj.name + " : InputUp");
    }
}
