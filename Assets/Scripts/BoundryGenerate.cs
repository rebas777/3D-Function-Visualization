using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryGenerate : MonoBehaviour {

    private MeshFilter mf;
    private Mesh mesh;

	// Use this for initialization
	void Start () {
        mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
