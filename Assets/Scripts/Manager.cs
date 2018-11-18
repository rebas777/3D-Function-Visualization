using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject GeneratedMesh;
    //public GameObject Portal;
    public float upperbound = 10.0f;
    public float lowerbound = -10.0f;
    public GameObject cursor;

    private GenerateMesh meshRenderer;
    private Vector3[][] inputM;
    private TextMesh tm;
    private CalculatorMtr calculator;
    private Calculator calcubase;
    private GameObject planeMin;
    private GameObject planeMax;
    private GameObject planeAvg;
    private bool observeMode = false;
    private Text textX;
    private Text textY;
    private Text textZ;
    private string curFomularStr;

    private int meshSize = 5;
    public float scaleSize;


    // Use this for initialization
    void Start () {

        //float tmp = Mathf.Sin(2.203f*-4.986f);
        //Debug.Log(tmp);

        GameObject go = GameObject.FindWithTag("InputText");
        tm = go.GetComponent<TextMesh>();

        planeMin = GameObject.Find("planeMin");
        planeMax = GameObject.Find("planeMax");
        planeAvg = GameObject.Find("planeAvg");
        planeMin.SetActive(false);
        planeMax.SetActive(false);
        planeAvg.SetActive(false);


        textX = GameObject.Find("TextX").GetComponent<Text>();
        textY = GameObject.Find("TextY").GetComponent<Text>();
        textZ = GameObject.Find("TextZ").GetComponent<Text>();

        calculator = new CalculatorMtr(-meshSize, -meshSize, meshSize, meshSize, 0.1f);
        calcubase = new Calculator();
        gameObject.GetComponent<CalculatorWrapper>().init(-meshSize, -meshSize, -meshSize, meshSize, meshSize, meshSize, 0.1f);

        //inputM = new Vector3[101][];
        //for(int i = 0; i < 101; i++) {
        //    inputM[i] = new Vector3[101];
        //}
        //float tiny = 0.5f;
        //for(int i = 0; i < 101; i++) {
        //    for(int j = 0; j < 101; j++) {
        //        float tmp_x = i * tiny;
        //        float tmp_z = j * tiny;
        //        Vector3 tmp = new Vector3(tmp_x, calculate(tmp_x, tmp_z), tmp_z);
        //        inputM[i][j] = tmp;
        //    }
        //}

        meshRenderer = GeneratedMesh.GetComponent<GenerateMesh>();
        //calculateInput();
    }

    // Only used for testing.
    float calculate(float x, float z) {
        return Mathf.Sin(x + z) * 2;
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.Q)) {
            //calculateInput(tm.text);
            calculateInput("sin(x-y)");
        }
        if(Input.GetKeyDown(KeyCode.M)) {
            calculate2dInput("5*sin(x)");
        }
        if(observeMode && cursorInRange()) {
            calculateAndDisplayValue();
        }


        //particlesys = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
    }

    private bool cursorInRange() {
        if(Mathf.Abs(cursor.transform.position.x - GeneratedMesh.transform.position.x) / GeneratedMesh.transform.localScale.x >= meshSize
            || Mathf.Abs(cursor.transform.position.z - GeneratedMesh.transform.position.z) / GeneratedMesh.transform.localScale.x >= meshSize) {
            return false;
        }
        return true;
    }

    private void calculateAndDisplayValue() {
        //Debug.Log("Move your tone around more");
        float xPos = (cursor.transform.position.x - GeneratedMesh.transform.position.x) / GeneratedMesh.transform.localScale.x;
        float zPos = (cursor.transform.position.z - GeneratedMesh.transform.position.z) / GeneratedMesh.transform.localScale.x;
        float yPos = 23.33f;
        if(curFomularStr != null) {
            yPos = calcubase.calculate(curFomularStr, xPos, zPos);
        }
        textX.text = xPos.ToString();
        textY.text = yPos.ToString();
        textZ.text = zPos.ToString();
    }

    public void ChangeObserveMode() {
        observeMode = observeMode ? false : true;
    }

    public void calculateInput(string inputText) {
        curFomularStr = inputText;
        List<List<float>> ret = calculator.calculate(inputText);

        List<Vector3[]> matrix = new List<Vector3[]>();
        

        for(int i = 0; i <= calculator.sampleFreqy; i += 1) {
            List<Vector3> tmp = new List<Vector3>();
            for(int j = 0; j <= calculator.sampleFreqx; j += 1) {
                Vector3 posi;
                if(float.IsNaN(ret[i][j])) {
                    posi = new Vector3((calculator.xStart + calculator.gap * j), upperbound + 1, (calculator.yStart + calculator.gap * i));
                }
                else {

                    posi = new Vector3((calculator.xStart + calculator.gap * j), ret[i][j], (calculator.yStart + calculator.gap * i));
                    //if(posi.y != Mathf.Sin(posi.x * posi.z)) {
                    //    Debug.Log(posi.x + " " + posi.y + " " + posi.z + " " + Mathf.Sin(posi.x * posi.z));
                    //}
                    
                }
                tmp.Add(posi);
                
            }
            
            Vector3[] tmpv = tmp.ToArray();
            matrix.Add(tmpv);
            
        }
        Vector3[][] mat = matrix.ToArray();


        // Draw the mesh
        meshRenderer.Draw3dMesh(mat);
        //Portal.GetComponent<ParticleSystem>().Stop();
        hideAnalysisValues();
    }

    public void calculate2dInput(string inputText) {
        List<List<float>>rett=  calculator.calculate(inputText, true);
        //Vector3[] matrix = new Vector3[];
        List<float> ret = rett[0];

        List<Vector3> tmp = new List<Vector3>();

        for(int j = 0; j <= calculator.sampleFreqx; j += 1) {
            Vector3 posi;
            if(float.IsNaN(ret[j])) {
                posi = new Vector3((calculator.xStart + calculator.gap * j), upperbound + 1, 0);
            }
            else {

                posi = new Vector3((calculator.xStart + calculator.gap * j), ret[j], 0);


            }
            tmp.Add(posi);

        }


        //    Vector3 v0,v1,v2,v3,v4;
        //v0 = new Vector3 ( -1, 0, 0 );
        //v1 = new Vector3(0, 1, 0);
        //v2 = new Vector3(1, 0, 0);
        //v3 = new Vector3(2, -1, 0);
        //v4 = new Vector3(3, -2, 0);
        Vector3[] inputArray = tmp.ToArray();
        meshRenderer.Draw2dMesh(inputArray);
    }

    public void clearMesh() {
        //Debug.Log("Clear Mesh !!");
        meshRenderer.clearMesh();
        //Portal.GetComponent<ParticleSystem>().Play();
    }

    public void showAnalysisValues() {
        GeneratedMesh.GetComponent<GenerateMesh>().AnalyzeValue();
        float min = GeneratedMesh.GetComponent<GenerateMesh>().minY;
        float max = GeneratedMesh.GetComponent<GenerateMesh>().maxY;
        float avg = GeneratedMesh.GetComponent<GenerateMesh>().avgY;
        Vector3 oldMinPos = planeMin.transform.position;
        oldMinPos.y = min*GeneratedMesh.transform.localScale.y;
        planeMin.transform.position = oldMinPos;
        Vector3 oldMaxPos = planeMax.transform.position;
        oldMaxPos.y = max * GeneratedMesh.transform.localScale.y;
        planeMax.transform.position = oldMaxPos;
        Vector3 oldAvgPos = planeAvg.transform.position;
        oldAvgPos.y = avg * GeneratedMesh.transform.localScale.y;
        planeAvg.transform.position = oldAvgPos;
        planeMin.SetActive(true);
        planeMax.SetActive(true);
        planeAvg.SetActive(true);
    }

    public void hideAnalysisValues() {
        planeMin.SetActive(false);
        planeMax.SetActive(false);
        planeAvg.SetActive(false);
    }
}
