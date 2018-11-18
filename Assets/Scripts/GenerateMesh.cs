using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh: MonoBehaviour {

    public enum ShadeMode {
        wireframe,
        normal,
        alpha,
        holo,
        tone
    }

    public float upperbound = 10.0f;
    public float lowerbound = -10.0f;
    //public bool isWireframe = false;
    public GameObject upSurf;
    public GameObject downSurf;
    public GameObject tdLineMesh;
    public Material material_wireframe;
    public Material material_normal;
    public Material material_alpha;
    public Material material_holo;
    public Material material_tone;
    public ShadeMode sm = ShadeMode.wireframe;
    public float maxY = 0;
    public float minY = 0;
    public float avgY = 0;
    public float Line_width = 1f;


    private float lastFrameUpperbound;
    private float lastFrameLowerbound;
    private bool lastFrameIsWireframe;
    private ShadeMode lastFrameSm;
    private Vector3[][] inputMatrix;
    private Vector3[] inputArray;
    private bool virgin = true;
    private MeshRenderer mr_up;
    private MeshRenderer mr_dw;
    private float MAX = 2000;
    private float MIN = -2000;

    MeshFilter mf;
    Mesh mesh;
    Vector3[] vertices;
    Vector3[][] matrix;
    int[] triangles;
    Vector2[] uvs;
    Vector2[] uvs1;

    // Use this for initialization
    void Start() {
        lastFrameLowerbound = upperbound;
        lastFrameLowerbound = lowerbound;
        //lastFrameIsWireframe = isWireframe;
        lastFrameSm = sm;
        mr_up = upSurf.GetComponent<MeshRenderer>();
        mr_dw = downSurf.GetComponent<MeshRenderer>();

        //Vector3[][] inputM =
        //    new Vector3[3][] {
        //        new Vector3[4] {new Vector3(0,0.5f,0),new Vector3(0,1.2f,1),new Vector3(0,-0.3f,2),new Vector3(0,-1.2f,3)},
        //        new Vector3[4] {new Vector3(1,-0.2f,0),new Vector3(1,1.3f,1),new Vector3(1,0.3f,2),new Vector3(1,-0.8f,3)},
        //        new Vector3[4] {new Vector3(2,-0.3f,0),new Vector3(2,0.8f,1),new Vector3(2,0.25f,2),new Vector3(2, -0.7f, 3)}
        //    };

        //Draw3dMesh(inputM);
    }

    

    // Update is called once per frame
    void Update() {
        if(virgin)
            return;
        if(/*isWireframe != lastFrameIsWireframe ||*/
            sm != lastFrameSm||
            upperbound != lastFrameUpperbound ||
            lowerbound != lastFrameLowerbound) {
            //lastFrameIsWireframe = isWireframe;
            lastFrameSm = sm;
            lastFrameLowerbound = lowerbound;
            lastFrameUpperbound = upperbound;
            Draw3dMesh();
        }
    }

    public void SetShadeMode(ShadeMode smIn) {
        this.sm = smIn;
    }

    public void AnalyzeValue() {

        if(inputMatrix.Length <= 0 || inputMatrix[0].Length <= 0) {
            Debug.Log("GenerateMesh: AnalyzeValue : bad inputMatrix");
        }
        maxY = MIN;
        minY = MAX;
        float sum = 0;
        int rowNum = inputMatrix.Length;
        int colNum = inputMatrix[0].Length;
        for(int i = 0; i < rowNum; i++) {
            Vector3[] tmpRow = inputMatrix[i];
            for(int j = 0; j < colNum; j++) {
                Vector3 cur = tmpRow[j];
                sum += cur.y;
                if(cur.y < minY) {
                    minY = cur.y;
                }
                if(cur.y > maxY) {
                    maxY = cur.y;
                }
            }
        }
        avgY = sum / (inputMatrix.Length + inputMatrix[0].Length);
    }

    public void Draw3dMesh(Vector3[][] inputM) {
        virgin = false;
        this.inputMatrix = inputM;
        if(sm != ShadeMode.wireframe) {
            Material curMaterial = null;

            // set shader accordingly.
            switch(sm) {
                case ShadeMode.alpha: {
                        curMaterial = material_alpha;
                        break;
                    }
                case ShadeMode.tone: {
                        curMaterial = material_tone;
                        break;
                    }
                case ShadeMode.normal: {
                        curMaterial = material_normal;
                        break;
                    }
                case ShadeMode.holo: {
                        curMaterial = material_holo;
                        break;
                    }
            }

            mr_up.material = curMaterial;
            mf = upSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            setUpMesh(true);
            mr_dw.material = curMaterial;
            mf = downSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            setUpMesh(false);
        }
        else {
            mr_up.material = material_wireframe;
            mf = upSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            drawWireframe(true);
            mr_dw.material = material_wireframe;
            mf = downSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            drawWireframe(false);
        }
    }

    public void Draw3dMesh() {
        if(sm != ShadeMode.wireframe) {
            Material curMaterial = null;

            // set shader accordingly.
            switch(sm) {
                case ShadeMode.alpha: {
                        curMaterial = material_alpha;
                        break;
                    }
                case ShadeMode.tone: {
                        curMaterial = material_tone;
                        break;
                    }
                case ShadeMode.normal: {
                        curMaterial = material_normal;
                        break;
                    }
                case ShadeMode.holo: {
                        curMaterial = material_holo;
                        break;
                    }
            }

            mr_up.material = curMaterial;
            mf = upSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            setUpMesh(true);
            mr_dw.material = curMaterial;
            mf = downSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            setUpMesh(false);
        }
        else {
            mr_up.material = material_wireframe;
            mf = upSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            drawWireframe(true);
            mr_dw.material = material_wireframe;
            mf = downSurf.GetComponent<MeshFilter>();
            mesh = mf.mesh;
            drawWireframe(false);
        }
    }

    public void setVertices(Vector3[] inputVerts) {
        vertices = inputVerts;
    }

    public void setTriangles(int[] inputTriangles) {
        triangles = inputTriangles;
    }

    public void setUV(Vector2[] inputUV) {
        uvs = inputUV;
    }

    public void transToBackface() {
        if((triangles.Length) % 3 != 0) {
            Debug.Log("error when transform to backface, bad triangle list\n ");
            return;
        }
        for(int i = 0; i < triangles.Length; i += 3) {
            int tmp = triangles[i + 1];
            triangles[i + 1] = triangles[i + 2];
            triangles[i + 2] = tmp;
        }
    }

    // may be useful when only one attributes are changed and want to redraw the mesh.
    public void setUpMesh() {
        // Set up mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        //upSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
        //downSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // should be used when given only an input matrix of verts position without triangle information.
    // need to calculate the triangle information and uv information first.
    // 假设传入的 matrix 在 x 和 z 的维度上是等距的
    public void setUpMesh(bool isUpSurf) {
        if(inputMatrix.Length == 0 || inputMatrix[0].Length == 0) {
            Debug.Log("Error: bad input matrix.\n");
            return;
        }
        int uVal, vVal;

        int foo = 0;
        int rowNum = inputMatrix.Length;
        int colNum = inputMatrix[0].Length;
        int[,] index = new int[rowNum, colNum];
        int[,] valid = new int[rowNum, colNum];
        //Debug.Log(rowNum + "-" + colNum);
        vertices = new Vector3[rowNum * colNum];
        uvs = new Vector2[rowNum * colNum];
        uvs1 = new Vector2[rowNum * colNum];
        triangles = new int[(rowNum - 1) * (colNum - 1) * 6];
        // set up vertices and uvs
        for(int i = 0; i < rowNum; i++) {
            //Debug.Log("i = " + i);
            uVal = i / (rowNum - 1);
            for(int j = 0; j < colNum; j++) {
                //Debug.Log("j = " + j);
                vVal = j / (colNum - 1);
                index[i, j] = foo;
                Vector3 tempPos = inputMatrix[i][j];
                if(tempPos.y >= upperbound) {
                    tempPos.y = upperbound;
                    valid[i, j] = 0;
                }
                else if(tempPos.y <= lowerbound) {
                    tempPos.y = lowerbound;
                    valid[i, j] = 0;
                }
                else {
                    valid[i, j] = 1;
                }
                vertices[foo] = tempPos;
                uvs[foo] = new Vector2(uVal, vVal);
                //uvs1[foo] = new Vector2(-1.0f, -1.0f);
                foo++;
            }
        }


        // set up triangles

        int bar = 0;
        for(int i = 0; i < rowNum - 1; i++) {
            for(int j = 0; j < colNum - 1; j++) {
                // any quad with 2 or more invalid points will be discard.
                if((valid[i, j] + valid[i, j + 1] + valid[i + 1, j] + valid[i + 1, j + 1]) <= 2)
                    continue;
                triangles[bar] = index[i, j];
                triangles[bar + 1] = index[i, j + 1];
                triangles[bar + 2] = index[i + 1, j + 1];
                triangles[bar + 3] = index[i, j];
                triangles[bar + 4] = index[i + 1, j + 1];
                triangles[bar + 5] = index[i + 1, j];
                bar += 6;
            }
        }

        if(!isUpSurf)
            transToBackface();

        // Set up mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.uv2 = uvs1;
        mesh.RecalculateNormals();
        if(isUpSurf) {
            upSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        else {
            downSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

    }

    public void clearMesh() {
        mf = upSurf.GetComponent<MeshFilter>();
        mesh = mf.mesh;
        mesh.Clear();
        mf = downSurf.GetComponent<MeshFilter>();
        mesh = mf.mesh;
        mesh.Clear();
    }

    public void Draw2dMesh(Vector3[] inputA) {
        this.inputArray = inputA;
        setUpMesh2D();
    }

    // inputArray should contain some vertex position with 0 z value.
    public void setUpMesh2D() {
        if(inputArray.Length < 2) {
            Debug.Log("Error: bad input array.\n");
            return;
        }
        vertices = new Vector3[(inputArray.Length - 1) * 4];
        uvs = new Vector2[(inputArray.Length - 1) * 4];
        triangles = new int[(inputArray.Length - 1) * 6];
        for(int i = 0; i < inputArray.Length - 1; i++) {
            twoPoint2Strip(inputArray[i], inputArray[i + 1], i);
        }

        // set up mesh
        mf = tdLineMesh.GetComponent<MeshFilter>();
        mesh = mf.mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        Debug.Log("2D mesh painted");

    }

    // to vertex position with 0 z value.
    private void twoPoint2Strip(Vector3 p1, Vector3 p2, int idx) {
        Vector3 v0, v1, v2, v3;
        v2 = p1;
        v3 = p2;
        v0 = v2 + new Vector3(0, Line_width, 0);
        v1 = v3 + new Vector3(0, Line_width, 0);
        vertices[4 * idx] = v0;
        vertices[4 * idx + 1] = v1;
        vertices[4 * idx + 2] = v2;
        vertices[4 * idx + 3] = v3;
        uvs[4 * idx] = new Vector2(0, 0);
        uvs[4 * idx+1] = new Vector2(0, 1);
        uvs[4 * idx+2] = new Vector2(1, 0);
        uvs[4 * idx+3] = new Vector2(1, 1);
        triangles[6 * idx + 0] = 4 * idx + 0;
        triangles[6 * idx + 1] = 4 * idx + 1;
        triangles[6 * idx + 2] = 4 * idx + 2;
        triangles[6 * idx + 3] = 4 * idx + 1;
        triangles[6 * idx + 4] = 4 * idx + 3;
        triangles[6 * idx + 5] = 4 * idx + 2;
    }

    public void SetInputArray(Vector3[] inputA) {
        this.inputArray = inputA;
        
    }

    public void drawWireframe(bool isUpSurf) {
        Debug.Log("Start draw wirefram.\n");
        if(inputMatrix.Length == 0 || inputMatrix[0].Length == 0) {
            Debug.Log("Error: bad input matrix.\n");
            return;
        }
        int rowNum = inputMatrix.Length;
        int colNum = inputMatrix[0].Length;
        Debug.Log("rowNum : " + rowNum + "   colNum : " + colNum);

        vertices = new Vector3[((rowNum - 1) * (colNum - 1)) * 2 * 3];
        uvs = new Vector2[((rowNum - 1) * (colNum - 1)) * 2 * 3];
        uvs1 = new Vector2[((rowNum - 1) * (colNum - 1)) * 2 * 3];
        triangles = new int[((rowNum - 1) * (colNum - 1)) * 2 * 3];


        int foo = 0;
        Vector3 topleft, topright, bottomleft, bottomright;
        for(int i = 0; i < rowNum - 1; i++) {
            for(int j = 0; j < colNum - 1; j++) {
                int valid_point1 = 0;
                topleft = inputMatrix[i][j];
                topright = inputMatrix[i][j + 1];
                bottomleft = inputMatrix[i + 1][j];
                bottomright = inputMatrix[i + 1][j + 1];
                Vector4 tmp = new Vector2(0.0f, 0.0f);
                // the first triangle in the quad.
                // tl -> tr -> bl
                vertices[foo] = topleft;
                if(topleft.y > upperbound || topleft.y < lowerbound)
                    valid_point1 += 1;
                tmp.x = getDistance(topleft, topright, bottomleft);
                uvs[foo] = tmp;
                vertices[foo + 1] = topright;
                if(topright.y > upperbound || topright.y < lowerbound)
                    valid_point1 += 1;
                tmp.y = getDistance(topright, topleft, bottomleft);
                uvs[foo + 1] = tmp;
                vertices[foo + 2] = bottomleft;
                if(bottomleft.y > upperbound || bottomleft.y < lowerbound)
                    valid_point1 += 1;
                tmp.z = getDistance(bottomleft, topleft, topright);
                uvs1[foo + 2] = tmp;
                if(valid_point1 == 0) {
                    // setup triangles
                    triangles[foo] = foo;
                    triangles[foo + 1] = foo + 1;
                    triangles[foo + 2] = foo + 2;
                }

                valid_point1 = 0;

                // the second triangle in the quad.
                // bl -> tr -> br
                vertices[foo + 3] = bottomleft;
                if(bottomleft.y > upperbound || bottomleft.y < lowerbound)
                    valid_point1 += 1;
                tmp.x = getDistance(bottomleft, topright, bottomright);
                uvs[foo + 3] = tmp;
                vertices[foo + 4] = topright;
                if(topright.y > upperbound || topright.y < lowerbound)
                    valid_point1 += 1;
                tmp.y = getDistance(topright, bottomright, bottomleft);
                uvs[foo + 4] = tmp;
                vertices[foo + 5] = bottomright;
                if(bottomright.y > upperbound || bottomright.y < lowerbound)
                    valid_point1 += 1;
                tmp.z = getDistance(bottomright, topright, bottomleft);
                uvs1[foo + 5] = tmp;
                if(valid_point1 == 0) {
                    // setup triangles
                    triangles[foo + 3] = foo + 3;
                    triangles[foo + 4] = foo + 4;
                    triangles[foo + 5] = foo + 5;
                }
                foo += 6;
            }
        }

        if(!isUpSurf)
            transToBackface();

        // Set up mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.uv2 = uvs1;
        mesh.RecalculateNormals();
        if(isUpSurf) {
            upSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        else {
            downSurf.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        //Debug.Log("can still pluck yew.");

    }

    //public void setWireframe(bool wireframeOn) {
    //    this.isWireframe = wireframeOn;
    //}

    private float getDistance(Vector3 top, Vector3 bottom1, Vector3 bottom2) {
        Vector3 a = bottom1 - bottom2;
        Vector3 b = top - bottom2;
        Vector3 c = Vector3.Cross(a, b);
        float Distance = Mathf.Sqrt((Vector3.Dot(c, c)) / (Vector3.Dot(a, a)));
        return Distance;
    }
}
