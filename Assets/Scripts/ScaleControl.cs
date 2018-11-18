using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour {

    public GameObject playerFinger;
    public GameObject cubeRange;

    private bool onTouch;
    private bool needRefresh;

	// Use this for initialization
	void Start () {
        onTouch = false;
        needRefresh = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(needRefresh) {
            // do refresh




            needRefresh = false;
        }
        Vector3 playerPos = playerFinger.transform.position;
        if(onTouch && playerPos.x > 0 && playerPos.y > 0 && playerPos.z < 0) {
            this.transform.position = playerPos;
            cubeRange.transform.localScale = new Vector3(2 * playerPos.x, 2 * playerPos.y, -2 * playerPos.z);
        }
	}

    void OnTriggerEnter(Collider collider) {
        Debug.Log("touched !!!!");
        if(collider.tag == "Player") {
            onTouch = true;
            
        }
    }

    void OnTriggerExit(Collider collider) {
        if(collider.tag == "Player") {
            onTouch = false;
            needRefresh = true;
        }
    }

    void OnTriggerStay(Collider collider) {
        if(collider.tag == "Player") {
            onTouch = true;
        }
    }
}
