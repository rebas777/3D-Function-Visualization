using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testCalculator : MonoBehaviour {
    public Transform pointModel;
    public CalculatorMtr calculator;
    public CalculatorMtr calculatory;
    

    public Calculator calcubase;

    public Calculator4D calculatort;
    public ParticleSystem system;
    public ParticleSystem linesystem;
    public InputField inputfield;
    public TextMesh tm;

    public CalculatorWrapper cw;

    ParticleSystem.Particle[] m_Particles;
    ParticleIllustration pi;
    ParticleIllustration pi2;
    public Vector3 hide = new Vector3(100, 100, 100);
    // Use this for initialization

    void Start () {
        calculator = new CalculatorMtr(-5,-5,5,5,0.05f);
        calculatory = new CalculatorMtr(-5, -5, 5, 5, 0.01f);
        calculatort = new Calculator4D(-5, -5,-5,5, 5, 5, 0.05f);
        cw.init(-5, -5,-5,5, 5, 5, 0.05f);

        pi = new ParticleIllustration(system);
        pi2 = new ParticleIllustration(linesystem,Color.black);
        GameObject go = GameObject.FindWithTag("InputText");
        tm = go.GetComponent<TextMesh>();

        calcubase = new Calculator();
        Debug.Log(calcubase.calculate("sin(xy)", 1.655f, -3.439f));
        Debug.Log(Mathf.Sin(1.655f * (-3.439f)));


    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.W)) {
            List<List<float>> ret = calculator.calculate("16/(xy)");
            pi.initialParticle();
            pi2.initialParticle();
            float hotz=0f;
            float maxz = 10;
            int linegap = 20;
            int countx = 0, county = 0;
            for (int i = 0; i <= calculator.sampleFreqy; i += 1)
            {
                county = (county + 1) % linegap;
                for (int j = 0; j <= calculator.sampleFreqx; j += 1)
                {
                    countx = (countx + 1) % linegap;
                    float nowx = calculator.xStart + calculator.gap * j;
                    float nowy = calculator.yStart + calculator.gap * i;
                    float mingap = calculator.gap;
                    float nowz = ret[i][j];
                    if ((/*(hotz > maxz||float.IsNaN(hotz)) && */nowz > maxz) || (/*(hotz < -maxz || float.IsNaN(hotz)) &&*/ nowz < -maxz)) { hotz = float.NaN; continue; }
                    if (float.IsNaN(nowz)) { hotz = float.NaN; continue; }
                    //Debug.Log(Mathf.Abs(hotz - nowz)-mingap);
                    if (j > 0 && Mathf.Abs(hotz - nowz) >= mingap && !float.IsNaN(hotz))
                    {
                        //
                        float deltax = calculator.gap * mingap / (Mathf.Abs(hotz - nowz));
                        float deltaz = (nowz - hotz) * mingap / (Mathf.Abs(hotz - nowz));
                        int count = 1;
                        for (float k = mingap; k < Mathf.Abs(hotz - nowz); k += mingap)
                        {
                            //Debug.Log("herer" + count);


                            Vector3 posit = new Vector3(nowx - calculator.gap + count * deltax, hotz + count * deltaz, nowy);
                            if (nowz > maxz || nowz < -maxz) { break; }

                            if (county == linegap - 1 )
                            {
                                pi2.drawParticle(posit);
                            }
                            else
                            {
                                pi.drawParticle(posit);
                            }
                            count++;
                            //if (count >= 10) { break; }
                        }

                    }
                    Vector3 posi = new Vector3((nowx), nowz, (nowy));
                    if (county == linegap - 1 || countx == linegap - 1)
                    {
                        pi2.drawParticle(posi);
                    }
                    else
                    {
                        pi.drawParticle(posi);
                    }
                    
                    hotz = nowz;
                    
                }
            }
            pi.endDraw();
            pi2.endDraw();

            Debug.Log("calcuFinished!");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            List<List<float>> ret = calculator.calculate(tm.text);
            float maxz = 10;
            for (int i = 0; i <= calculator.sampleFreqy; i += 1)
            {
                for (int j = 0; j <= calculator.sampleFreqx; j += 1)
                {
                    if (float.IsNaN(ret[i][j])) { continue; }
                    
                    Vector3 posi = new Vector3((calculator.xStart + calculator.gap * i), ret[i][j], (calculator.yStart + calculator.gap * j));
                    Instantiate(pointModel, posi, new Quaternion(0, 0, 0, 0));
                }
            }
            Debug.Log("calcuFinished!");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            cw.calculate("z*z+2xyz-16");

           
        }
        List<List<List<float>>> cwret=new List<List<List<float>>>();

        if ((cwret=cw.getCalcuResult())!=null) {
            Debug.Log("count1"+cwret.Count+" "+ cw.sampleFreqz);
            Debug.Log("count2" + cwret[0].Count + " " + cw.sampleFreqy);
            pi.initialParticle();

            for (int i = 2; i <= cw.sampleFreqz - 2; i += 1)
            {
                for (int j = 2; j <= cw.sampleFreqy - 2; j += 1)
                {
                    for (int k = 2; k < cw.sampleFreqx - 2; k++)
                    {
                        float devi = Mathf.Abs(cwret[i][j][k]);
                        if (float.IsNaN(cwret[i][j][k])) { continue; }

                        if (devi <= cw.gap * 4)
                        {

                            if (devi <= Mathf.Abs(cwret[i][j][k - 1 >= 0 ? (k - 1) : 0]) && devi <= Mathf.Abs(cwret[i][j][k + 1 < cw.sampleFreqx ? (k + 1) : cw.sampleFreqx - 1]))
                            {
                                Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                pi.drawParticle(posi);
                            }
                            else if (devi <= Mathf.Abs(cwret[i][j - 1 >= 0 ? (j - 1) : 0][k]) && devi <= Mathf.Abs(cwret[i][j + 1 < cw.sampleFreqy ? (j + 1) : cw.sampleFreqy - 1][k]))
                            {
                                Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                pi.drawParticle(posi);
                            }
                            else if (devi <= Mathf.Abs(cwret[i - 1 >= 0 ? (i - 1) : 0][j][k]) && devi <= Mathf.Abs(cwret[i + 1 < cw.sampleFreqz ? (i + 1) : cw.sampleFreqz - 1][j][k]))
                            {
                                Vector3 posi = new Vector3((cw.xMin + cw.gap * i), (cw.zMin + cw.gap * k), (cw.yMin + cw.gap * j));
                                pi.drawParticle(posi);
                            }
                        }
                    }

                    //Debug.Log(ret[i][j]);



                }

            }
            //var mainModule = system.main;
            // mainModule.maxParticles = countp;
            pi.endDraw();
            Debug.Log("calcuFinished!");
        }


        if (Input.GetKeyDown(KeyCode.Y))
        {
            List<List<float>> ret = calculatory.calculate("pow(x,3)+pow(y,3)-6xy");

            pi.initialParticle();
            
            for (int i = 0; i <= calculatory.sampleFreqy; i += 1)
            {
                for (int j = 0; j <= calculatory.sampleFreqx; j += 1)
                {
                    if (float.IsNaN(ret[i][j])) { continue; }
                    float devi = Mathf.Abs(ret[i][j]);
                   
                    if (devi <= calculatory.gap*25) {

                        if (devi <= Mathf.Abs(ret[i][j - 1>=0?(j-1):0]) && devi <= Mathf.Abs(ret[i][j + 1 < calculatory.sampleFreqx ? (j + 1) : calculatory.sampleFreqx-1])){
                            Vector3 posi = new Vector3((calculatory.xStart + calculatory.gap * i), 0, (calculatory.yStart + calculatory.gap * j));
                            pi.drawParticle(posi);
                        }
                        else if (devi <= Mathf.Abs(ret[i- 1 >= 0 ? (i - 1) : 0][j]) && devi <= Mathf.Abs(ret[i + 1 < calculatory.sampleFreqy ? (i + 1) : calculatory.sampleFreqy - 1][j]))
                        {
                            Vector3 posi = new Vector3((calculatory.xStart + calculatory.gap * i), 0, (calculatory.yStart + calculatory.gap * j));
                            pi.drawParticle(posi);
                        }


                    }
                }
               
            }

            pi.endDraw();

            //float start = 0 - calculator.gap * calculator.sampleFreqx / 2;
            //for (int i = 0; i < ret.Count; i++) {
            //    Debug.Log(ret[i]);
            //}
            Debug.Log("calcuFinished!");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<List<List<float>>> ret = calculatort.calculate("z*z+2xyz-16");

            pi.initialParticle();

            for (int i = 2; i <= calculatort.sampleFreqz-2; i += 1)
            {
                for (int j = 2; j <= calculatort.sampleFreqy-2; j += 1)
                {
                    for (int k = 2; k < calculatort.sampleFreqx-2; k++) {
                        float devi = Mathf.Abs(ret[i][j][k]);
                        if (float.IsNaN(ret[i][j][k])) { continue; }

                        if (devi <= calculatort.gap*4)
                        {

                            if (devi <= Mathf.Abs(ret[i][j][k - 1 >= 0 ? (k - 1) : 0]) && devi <= Mathf.Abs(ret[i][j][k + 1 < calculatort.sampleFreqx ? (k + 1) : calculatort.sampleFreqx - 1]))
                            {
                                Vector3 posi = new Vector3((calculatort.xStart + calculatort.gap * i), (calculatort.zStart + calculatort.gap * k), (calculatort.yStart + calculatort.gap * j));
                                pi.drawParticle(posi);
                            }
                            else if (devi <= Mathf.Abs(ret[i][j - 1 >= 0 ? (j - 1) : 0][k]) && devi <= Mathf.Abs(ret[i][j + 1 < calculatort.sampleFreqy ? (j + 1) : calculatort.sampleFreqy - 1][k]))
                            {
                                Vector3 posi = new Vector3((calculatort.xStart + calculatort.gap * i), (calculatort.zStart + calculatort.gap * k), (calculatort.yStart + calculatort.gap * j));
                                pi.drawParticle(posi);
                            }
                            else if (devi <= Mathf.Abs(ret[i - 1 >= 0 ? (i - 1) : 0][j][k]) && devi <= Mathf.Abs(ret[i + 1 < calculatort.sampleFreqz ? (i + 1) : calculatort.sampleFreqz - 1][j][k]))
                            {
                                Vector3 posi = new Vector3((calculatort.xStart + calculatort.gap * i), (calculatort.zStart + calculatort.gap * k), (calculatort.yStart + calculatort.gap * j));
                                pi.drawParticle(posi);
                            }
                        }
                    }
                   
                    //Debug.Log(ret[i][j]);
                    
                    

                }
                
            }
            //var mainModule = system.main;
            // mainModule.maxParticles = countp;
            pi.endDraw();
            

            
            Debug.Log("calcuFinished!");
        }
       
    }
}
