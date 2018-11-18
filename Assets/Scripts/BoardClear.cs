using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClear : MonoBehaviour {

    public GameObject manager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider) {
        manager.GetComponent<Manager>().clearMesh();
    }
}
