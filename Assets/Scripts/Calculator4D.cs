using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator4D
{

    public int sampleFreqx;
    public int sampleFreqy;
    public int sampleFreqz;
    public float gap;
    public float xStart, yStart, zStart;
    public List<List<List<float>>> x = new List<List<List<float>>>();
    public List<List<List<float>>> y = new List<List<List<float>>>();
    public List<List<List<float>>> z = new List<List<List<float>>>();
    public List<List<List<float>>> nx = new List<List<List<float>>>();
    public List<List<List<float>>> ny = new List<List<List<float>>>();
    public List<List<List<float>>> nz = new List<List<List<float>>>();
    public List<List<List<float>>> zerol = new List<List<List<float>>>();

    public Calculator4D(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax, float g)
    {
        xStart = xmin;
        yStart = ymin;
        zStart = zmin;
        sampleFreqx = Mathf.RoundToInt((xmax - xmin) / g);
        sampleFreqy = Mathf.RoundToInt((ymax - ymin) / g);
        sampleFreqz = Mathf.RoundToInt((zmax - zmin) / g);
       
        gap = g;
        initialXYZ();
    }
    public Calculator4D()
    {
        ;
    }
    public void init(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax, float g)
    {
        xStart = xmin;
        yStart = ymin;
        zStart = zmin;
        sampleFreqx = Mathf.RoundToInt((xmax - xmin) / g);
        sampleFreqy = Mathf.RoundToInt((ymax - ymin) / g);
        sampleFreqz = Mathf.RoundToInt((zmax - zmin) / g);
        Debug.Log("samplez:" + sampleFreqz);
        gap = g;
        initialXYZ();
    }

    public string calcutext = "";

    public float readNumFromString(string s, ref int index)
    {
        bool end = false;
        string numstring = "";
        while (s[index] == ' ') { index++; }
        while (!end && index < s.Length)
        {
            if (s[index] <= '9' && s[index] >= '0' || s[index] == '.')
            {
                numstring += s[index];
                index++;
            }
            else
                break;
        }
        return float.Parse(numstring);
    }

    public bool expectOperator(string s, int index)
    {
        index--;
        while (index >= 0)
        {
            if (s[index] == ' ') { index--; continue; }
            if ((s[index] <= '9' && s[index] >= '0') || s[index] == ')' || s[index] == 'x' || s[index] == 'y' || s[index] == 'z')
            {
                return true;
            }
            else { return false; }

        }
        return false;
    }

    public List<List<List<float>>> ListOpertation(ref List<List<List<float>>> a, ref List<List<List<float>>> b, char op)
    {
        List<List<List<float>>> ret = new List<List<List<float>>>();

        int xMax = a[0][0].Count > b[0][0].Count ? a[0][0].Count : b[0][0].Count;
        int yMax = a[0].Count > b[0].Count ? a[0].Count : b[0].Count;
        int zMax = a.Count > b.Count ? a.Count : b.Count;

       
        for (int i = 0; i < zMax; i++)
        {
            int arow, brow;
            if (a.Count < zMax) { arow = 0; }
            else { arow = i; }
            if (b.Count < zMax) { brow = 0; }
            else { brow = i; }
            List<List<float>> tmprow = new List<List<float>>();

            for (int j = 0; j < yMax; j++)
            {
                int acol, bcol;
                if (a[0].Count < yMax) { acol = 0; }
                else { acol = j; }
                if (b[0].Count < yMax) { bcol = 0; }
                else { bcol = j; }
                List<float> tmpcol = new List<float>();
                for (int k = 0; k < xMax; k++) {
                    int az, bz;
                    if (a[0][0].Count < xMax) { az = 0; }
                    else { az = k; }
                    if (b[0][0].Count < xMax) { bz = 0; }
                    else { bz = k; }
                    switch (op)
                    {
                        case '+':
                            tmpcol.Add(a[arow][acol][az] + b[brow][bcol][bz]);
                            break;
                        case '-':
                            tmpcol.Add(-a[arow][acol][az] + b[brow][bcol][bz]);
                            break;
                        case '*':
                            tmpcol.Add(a[arow][acol][az] * b[brow][bcol][bz]);
                            break;
                        case '/':
                            tmpcol.Add(a[arow][acol][az] / b[brow][bcol][bz]);
                            break;
                        case 'p':
                            tmpcol.Add(Mathf.Pow(b[brow][bcol][bz], a[arow][acol][az]));
                            break;
                        case 'l':
                            tmpcol.Add(Mathf.Log(b[brow][bcol][bz], a[arow][acol][az]));
                            break;
                    }
                }
                tmprow.Add(tmpcol);
                
            }
            ret.Add(tmprow);
        }
        return ret;

    }

    public void calculateStack(int i, ref Stack<List<List<List<float>>>> ns, ref Stack<char> cs, bool reserve = false)
    {
        while (true)
        {
            if (cs.Count == 0) { return; }
            char op = cs.Peek();
            //cs.Pop();
            List<List<List<float>>> num1;
            List<List<List<float>>> num2;
            switch (op)
            {
                case '+':
                case '-':
                    if (i == 2) { return; }
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();

                    ns.Push(ListOpertation(ref num1, ref num2, op));
                    break;
                case '*':
                case '/':
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();

                    ns.Push(ListOpertation(ref num1, ref num2, op));
                    break;
                case '(':
                    if (i == 3) { cs.Pop(); }
                    return;
                case 's':
                    if (i == 3)
                    {
                        for (int ii = 0; ii < ns.Peek().Count; ii++)
                        {
                            for (int jj = 0; jj < ns.Peek()[ii].Count; jj++)
                            {
                                for(int kk = 0; kk < ns.Peek()[ii][jj].Count; kk++) {
                                    ns.Peek()[ii][jj][kk] = Mathf.Sin(ns.Peek()[ii][jj][kk]);
                                }
                                
                            }
                        }

                        cs.Pop();
                    }
                    return;
                case 'c':
                    if (i == 3)
                    {
                        for (int ii = 0; ii < ns.Peek().Count; ii++)
                        {
                            for (int jj = 0; jj < ns.Peek()[ii].Count; jj++)
                            {
                                for (int kk = 0; kk < ns.Peek()[ii][jj].Count; kk++)
                                {
                                    ns.Peek()[ii][jj][kk] = Mathf.Cos(ns.Peek()[ii][jj][kk]);
                                }
                            }
                        }

                        cs.Pop();
                    }
                    return;
                case 't':
                    if (i == 3)
                    {
                        for (int ii = 0; ii < ns.Peek().Count; ii++)
                        {
                            for (int jj = 0; jj < ns.Peek()[ii].Count; jj++)
                            {
                                for (int kk = 0; kk < ns.Peek()[ii][jj].Count; kk++)
                                {
                                    ns.Peek()[ii][jj][kk] = Mathf.Tan(ns.Peek()[ii][jj][kk]);
                                }
                            }
                        }

                        cs.Pop();
                    }
                    return;
                case 'p':

                    if (i == 3 && !reserve)
                    {
                        num1 = ns.Peek(); ns.Pop();
                        num2 = ns.Peek(); ns.Pop();

                        ns.Push(ListOpertation(ref num1, ref num2, op));
                        cs.Pop();
                    }
                    return;
                case 'l':

                    if (i == 3 && !reserve)
                    {
                        num1 = ns.Peek(); ns.Pop();
                        num2 = ns.Peek(); ns.Pop();

                        ns.Push(ListOpertation(ref num1, ref num2, op));
                        cs.Pop();
                    }
                    return;
                default:
                    break;
            }

        }


    }
    public void initialXYZ()
    {
        x.Clear();
        nx.Clear();
        y.Clear();
        ny.Clear();
        z.Clear();
        nz.Clear();

        int i;
        List<List<float>> tmpx=new List<List<float>>(), tmpnx=new List<List<float>>();
        List<float> tmp = new List<float>();
        List<float> tmpp = new List<float>();
        for (i = 0; i <= sampleFreqx; i += 1)
        {
            tmp.Add(xStart + gap * i);
            tmpp.Add(-(xStart + gap * i));
        }
        tmpx.Add(tmp);
        tmpnx.Add(tmpp);
        x.Add(tmpx);
        nx.Add(tmpnx);

        List<List<float>> tmpy = new List<List<float>>(), tmpny = new List<List<float>>();
        for (i = 0; i <= sampleFreqy; i += 1)
        {
            List<float> tmp2 = new List<float>();
            List<float> tmpp2 = new List<float>();
            tmp2.Add(yStart + gap * i);
            tmpp2.Add(-yStart - gap * i);
            tmpy.Add(tmp2);
            tmpny.Add(tmpp2);
        }
        y.Add(tmpy);ny.Add(tmpny);

        
        for (i = 0; i <= sampleFreqz; i += 1)
        {
            List<List<float>> tmpz = new List<List<float>>(), tmpnz = new List<List<float>>();
            List<float> tmp2 = new List<float>();
            List<float> tmpp2 = new List<float>();
            tmp2.Add(zStart + gap * i);
            tmpp2.Add(-zStart - gap * i);
            tmpz.Add(tmp2);
            tmpnz.Add(tmpp2);
            z.Add(tmpz);
            nz.Add(tmpnz);
        }

        for (i = 0; i <= sampleFreqz; i += 1) {
            List<List<float>> tmp1 = new List<List<float>>();
            for (int j = 0; j <= sampleFreqy; j++) {
                List<float> tmp2 = new List<float>();
                for (int k = 0; k <= sampleFreqx; k++) {
                    tmp2.Add(0);
                }
                tmp1.Add(tmp2);
            }
            zerol.Add(tmp1);
        }






    }

    public List<List<List<float>>> calculate(string s, bool twoD = false)
    {
        System.DateTime startTime, endTime;
        System.TimeSpan time;
        startTime = System.DateTime.Now;

        Stack<char> operatestack = new Stack<char>();
        Stack<List<List<List<float>>>> numstack = new Stack<List<List<List<float>>>>();
        int i = 0;
        int ssize = s.Length;
        bool negaflag = false;

        while (i < s.Length)
        {
            Debug.Log("parsing " + s[i]);
            switch (s[i])
            {
                case '+':
                    if (!expectOperator(s, i)) { i++; break; }
                    calculateStack(1, ref numstack, ref operatestack);
                    operatestack.Push('+');
                    i++;
                    break;
                case '-':
                    if (!expectOperator(s, i)) { i++; negaflag = !negaflag; break; }
                    calculateStack(1, ref numstack, ref operatestack);
                    operatestack.Push('-');
                    i++;
                    break;
                case '*':
                    calculateStack(2, ref numstack, ref operatestack);
                    operatestack.Push('*');
                    i++;
                    break;
                case '/':
                    calculateStack(2, ref numstack, ref operatestack);
                    operatestack.Push('/');
                    i++;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':

                    List<float> tmp = new List<float>();
                    List<List<float>> tmp2 = new List<List<float>>();
                    List<List<List<float>>> tmp3 = new List<List<List<float>>>();
                    if (negaflag) { tmp.Add(0 - readNumFromString(s, ref i)); negaflag = false; }
                    else { tmp.Add(readNumFromString(s, ref i)); }
                    tmp2.Add(tmp);
                    tmp3.Add(tmp2);
                    numstack.Push(tmp3);
                    break;
                case 'x':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(nx); negaflag = false; }
                    else { numstack.Push(x); }
                    i++;
                    break;
                case 'y':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(ny); negaflag = false; }
                    else { numstack.Push(y); }
                    i++;
                    break;
                case 'z':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(ny); negaflag = false; }
                    else { numstack.Push(z); }
                    i++;
                    break;
                case '(':
                    operatestack.Push('(');
                    i++;
                    break;
                case ')':
                    calculateStack(3, ref numstack, ref operatestack);
                    i++;
                    break;
                case 's':
                    if (s[i + 1] == 'i' && s[i + 2] == 'n' && s[i + 3] == '(')
                    {
                        operatestack.Push('s');
                        i += 4;
                    }

                    break;
                case 'c':
                    if (s[i + 1] == 'o' && s[i + 2] == 's' && s[i + 3] == '(')
                    {
                        operatestack.Push('c');
                        i += 4;
                    }

                    break;
                case 't':
                    if (s[i + 1] == 'a' && s[i + 2] == 'n' && s[i + 3] == '(')
                    {
                        operatestack.Push('t');
                        i += 4;
                    }

                    break;
                case 'p':
                    if (s[i + 1] == 'o' && s[i + 2] == 'w' && s[i + 3] == '(')
                    {
                        operatestack.Push('p');
                        i += 4;
                    }

                    break;
                case 'l':
                    if (s[i + 1] == 'o' && s[i + 2] == 'g' && s[i + 3] == '(')
                    {
                        operatestack.Push('l');
                        i += 4;
                    }

                    break;
                case ',':
                    calculateStack(3, ref numstack, ref operatestack, true);
                    i++;
                    break;
                default:
                    i++;
                    break;
            }
        }
        while (!(operatestack.Count == 0))
        {
            //Debug.Log("here");
            calculateStack(1, ref numstack, ref operatestack);
        }
        //if (numstack.Peek()[0][0].Count == 1) {
        //    float tmp = numstack.Peek()[0][0][0];
        //    for (int j = 1; j <= sampleFreqx; j += 1)
        //    {
        //        numstack.Peek()[0][0].Add(tmp);
        //    }
        //}
        //if (numstack.Peek()[0].Count == 1)
        //{
        //    List<float> tmp = numstack.Peek()[0][0];
        //    for (int j = 1; j <= sampleFreqy; j += 1)
        //    {
        //        numstack.Peek()[0].Add(tmp);
        //    }
        //}
        List<List<List<float>>> tmpl = numstack.Peek();
        
        

        endTime = System.DateTime.Now;
        time = endTime - startTime;
        int runTime = time.Milliseconds;
        Debug.Log("complete time：" + runTime + "ms");
        return ListOpertation(ref tmpl, ref zerol, '+');
    }

    public string calcuStr;
    public bool GL2D = false;

    public List<List<List<float>>> calcuRet = new List<List<List<float>>>();
    public bool calcuDone = false;

    public void calculate()
    {
        System.DateTime startTime, endTime;
        System.TimeSpan time;
        startTime = System.DateTime.Now;
        string s = calcuStr;
        calcuRet.Clear();
        Stack<char> operatestack = new Stack<char>();
        Stack<List<List<List<float>>>> numstack = new Stack<List<List<List<float>>>>();
        int i = 0;
        int ssize = s.Length;
        bool negaflag = false;

        while (i < s.Length)
        {
            Debug.Log("parsing " + s[i]);
            switch (s[i])
            {
                case '+':
                    if (!expectOperator(s, i)) { i++; break; }
                    calculateStack(1, ref numstack, ref operatestack);
                    operatestack.Push('+');
                    i++;
                    break;
                case '-':
                    if (!expectOperator(s, i)) { i++; negaflag = !negaflag; break; }
                    calculateStack(1, ref numstack, ref operatestack);
                    operatestack.Push('-');
                    i++;
                    break;
                case '*':
                    calculateStack(2, ref numstack, ref operatestack);
                    operatestack.Push('*');
                    i++;
                    break;
                case '/':
                    calculateStack(2, ref numstack, ref operatestack);
                    operatestack.Push('/');
                    i++;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':

                    List<float> tmp = new List<float>();
                    List<List<float>> tmp2 = new List<List<float>>();
                    List<List<List<float>>> tmp3 = new List<List<List<float>>>();
                    if (negaflag) { tmp.Add(0 - readNumFromString(s, ref i)); negaflag = false; }
                    else { tmp.Add(readNumFromString(s, ref i)); }
                    tmp2.Add(tmp);
                    tmp3.Add(tmp2);
                    numstack.Push(tmp3);
                    break;
                case 'x':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(nx); negaflag = false; }
                    else { numstack.Push(x); }
                    i++;
                    break;
                case 'y':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(ny); negaflag = false; }
                    else { numstack.Push(y); }
                    i++;
                    break;
                case 'z':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(ny); negaflag = false; }
                    else { numstack.Push(z); }
                    i++;
                    break;
                case '(':
                    operatestack.Push('(');
                    i++;
                    break;
                case ')':
                    calculateStack(3, ref numstack, ref operatestack);
                    i++;
                    break;
                case 's':
                    if (s[i + 1] == 'i' && s[i + 2] == 'n' && s[i + 3] == '(')
                    {
                        operatestack.Push('s');
                        i += 4;
                    }

                    break;
                case 'c':
                    if (s[i + 1] == 'o' && s[i + 2] == 's' && s[i + 3] == '(')
                    {
                        operatestack.Push('c');
                        i += 4;
                    }

                    break;
                case 't':
                    if (s[i + 1] == 'a' && s[i + 2] == 'n' && s[i + 3] == '(')
                    {
                        operatestack.Push('t');
                        i += 4;
                    }

                    break;
                case 'p':
                    if (s[i + 1] == 'o' && s[i + 2] == 'w' && s[i + 3] == '(')
                    {
                        operatestack.Push('p');
                        i += 4;
                    }

                    break;
                case 'l':
                    if (s[i + 1] == 'o' && s[i + 2] == 'g' && s[i + 3] == '(')
                    {
                        operatestack.Push('l');
                        i += 4;
                    }

                    break;
                case ',':
                    calculateStack(3, ref numstack, ref operatestack, true);
                    i++;
                    break;
                default:
                    i++;
                    break;
            }
        }
        while (!(operatestack.Count == 0))
        {
            //Debug.Log("here");
            calculateStack(1, ref numstack, ref operatestack);
        }
     
        List<List<List<float>>> tmpl = numstack.Peek();



        endTime = System.DateTime.Now;
        time = endTime - startTime;
        int runTime = time.Milliseconds;
        calcuRet= ListOpertation(ref tmpl, ref zerol, '+');
        Debug.Log("complete time：" + runTime + "ms");
        calcuDone = true;
        return;
    }

    public List<List<List<float>>> getCalcuRet()
    {
        if (calcuDone == true)
        {
            calcuDone = false;
            //while (calcuRet==null)
            //Debug.Log("worker"+calcuRet[0].Count);
            return calcuRet;
        }
        return null;
    }



}
