using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class CalculatorWrapper : MonoBehaviour {
    public float xMin, xMax, yMin, yMax, zMin, zMax;
    public float gap;

    public int sampleFreqx, sampleFreqy, sampleFreqz;

    public int threadNum = 2;
    private int threadDoneNum = 0;

    private bool calculatorMtrDone = false;
    private bool calculatorVecDone = false;

    private List<Calculator4D> calculator4ds=new List<Calculator4D>();
    private CalculatorMtr cmtr;
    public List<List<List<float>>> result = new List<List<List<float>>>();
    public List<List<float>> resultvec = new List<List<float>>();
    private List<List<List<List<float>>>> calcuresults = new List<List<List<List<float>>>>();

    //private float divide(float min, float max, float gap, int slices, int index) {
    //    if (index == slices){ return max; }
    //    float ret=0;
        
    //    return ret;
    //}

    public void init(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax, float g)
    {
        for(int i = 0; i < threadNum; i++) {
            calculator4ds.Add(new Calculator4D());
            calcuresults.Add(new List<List<List<float>>>());
        }
        xMin = xmin;
        yMin = ymin;
        xMax = xmax;
        yMax = ymax;
        zMin = zmin;
        zMax = zmax;
        gap = g;
        sampleFreqx = Mathf.RoundToInt((xmax - xmin) / g);
        sampleFreqy = Mathf.RoundToInt((ymax - ymin) / g);
        sampleFreqz = Mathf.RoundToInt((zmax - zmin) / g);

        float xlgap = (xmax - xmin) / calculator4ds.Count;
        float ylgap = (ymax - ymin) / calculator4ds.Count;
        float zlgap = (zmax - zmin) / calculator4ds.Count;
        int gapnum = Mathf.RoundToInt(zlgap / gap);
        zlgap = Mathf.RoundToInt((float)gapnum) * gap;

        for (int i = 0; i < calculator4ds.Count; i++)
        {
            if (i == calculator4ds.Count - 1)
            {
                calculator4ds[i].init(xMin, yMin, zMin + i * zlgap, xMax, yMax, zMax, gap);
            }
            else
            {
                calculator4ds[i].init(xMin, yMin, zMin + i * zlgap, xMax, yMax , zMin + (i+1) * zlgap-gap, gap);
                
            }
            
        }
        cmtr = new CalculatorMtr(xMin, yMin, xMax, yMax, gap/10);
    }
    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        waitForWorker();
	}

    public void waitForWorker() {
        for (int i = 0; i < threadNum; i++)
        {
            List<List<List<float>>> tmp=new List<List<List<float>>>();
            if ((tmp = calculator4ds[i].getCalcuRet()) != null)
            {
                calcuresults[i] = tmp;
                Debug.Log("thread:" + i);
                threadDoneNum++;
            }
        }
        if (threadDoneNum == threadNum)
        {
            threadDoneNum = 0;
            result.Clear();
            for (int i = 0; i < threadNum; i++)
            {
                Debug.Log(calcuresults[i].Count);
                Debug.Log("y"+calcuresults[i][0].Count);
                Debug.Log("x"+calcuresults[i][0][0].Count);
                //Debug.Log(calcuresults[i][0][0][0].Count);
              
                    result.AddRange(calcuresults[i]);
                
            }
            calculatorMtrDone = true;
        }
    }

    //public void calculate(string s) {
    //    System.Threading.ThreadPool.SetMaxThreads(threadNum+2, threadNum + 2);
    //    for (int i = 0; i < threadNum; i++) {
    //        calculator4ds[i].calcuStr = s;

    //        //启动工作者线程
    //        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(calculator4ds[i].calculate));
    //        //Thread _thread = new Thread(calculator4ds[i].calculate);
    //        //_thread.Start();
    //    }
    //}

    public void calculate(string s) {
         //ThreadPool.SetMaxThreads(6, 6);
        Debug.Log("here");
        for(int i = 0; i < threadNum; i++) {
            calculator4ds[i].calcuStr = s;

            //MyDelegate mydelegate = new MyDelegate(calculator4ds[i].calculate);
            //  System.IAsyncResult result = mydelegate.BeginInvoke("", callback, "");

            //异步执行完成
            //string resultstr = mydelegate.EndInvoke(result);

            //启动工作者线程
            //ThreadPool.QueueUserWorkItem(new WaitCallback(calculator4ds[i].calculate));
            calculator4ds[i].calculate();
            //Thread _thread = new Thread(calculator4ds[i].calculate);
            //_thread.Start();
        }
    }





    public List<List<List<float>>> getCalcuResult() {
        if (calculatorMtrDone == true) {
            calculatorMtrDone = false;
            return result;
        }
        return null;
    }

    public void calculatevec(string s) {
        resultvec = cmtr.calculate(s);
        calculatorVecDone = true;
    }

    public List<List<float>> getCalcuResultvec() {
        if(calculatorVecDone == true) {
            calculatorVecDone = false;
            return resultvec;
        }
        return null;
    }


}
