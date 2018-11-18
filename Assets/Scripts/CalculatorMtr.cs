using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Threading.Tasks;


public class CalculatorMtr
{

    public int sampleFreqx ;
    public int sampleFreqy;
    public float gap ;
    public float xStart, yStart;
    public List<List<float>> x = new List<List<float>>();
    public List<List<float>> y = new List<List<float>>();
    public List<List<float>> nx = new List<List<float>>();
    public List<List<float>> ny = new List<List<float>>();

    public CalculatorMtr(float xmin, float ymin, float xmax, float ymax, float g)
    {
        xStart = xmin;
        yStart = ymin;
        sampleFreqx = (int)((xmax - xmin) / g);
        sampleFreqy = (int)((ymax - ymin) / g);
        gap = g;
        initialXY();
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
            if ((s[index] <= '9' && s[index] >= '0') || s[index] == ')' || s[index] == 'x' || s[index] == 'y')
            {
                return true;
            }
            else { return false; }

        }
        return false;
    }

    public List<List<float>> ListOpertation(ref List<List<float>> a, ref List<List<float>> b, char op)
    {
        List<List<float>> ret = new List<List<float>>();

        int xMax = a[0].Count > b[0].Count ? a[0].Count : b[0].Count;
        int yMax = a.Count > b.Count ? a.Count : b.Count;

        for (int i = 0; i < yMax; i++)
        {
            int arow, brow;
            if (a.Count < yMax) { arow = 0; }
            else { arow = i; }
            if (b.Count < yMax) { brow = 0; }
            else { brow = i; }
            List<float> tmprow = new List<float>();

            for (int j = 0; j < xMax; j++)
            {
                int acol, bcol;
                if (a[0].Count < xMax) { acol = 0; }
                else { acol = j; }
                if (b[0].Count < xMax) { bcol = 0; }
                else { bcol = j; }
                switch (op)
                {
                    case '+':
                        tmprow.Add(a[arow][acol] + b[brow][bcol]);
                        break;
                    case '-':
                        tmprow.Add(-a[arow][acol] + b[brow][bcol]);
                        break;
                    case '*':
                        tmprow.Add(a[arow][acol] * b[brow][bcol]);
                        break;
                    case '/':
                        tmprow.Add( b[brow][bcol]/ a[arow][acol] );
                        break;
                    case 'p':
                        tmprow.Add(Mathf.Pow(b[brow][bcol], a[arow][acol]));
                        break;
                    case 'l':
                        tmprow.Add(Mathf.Log(b[brow][bcol], a[arow][acol]));
                        break;
                }
            }
            ret.Add(tmprow);
        }
        return ret;

    }

    public void calculateStack(int i, ref Stack<List<List<float>>> ns, ref Stack<char> cs, bool reserve = false)
    {
        while (true)
        {
            if (cs.Count == 0) { return; }
            char op = cs.Peek();
            //cs.Pop();
            List<List<float>> num1;
            List<List<float>> num2;
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
                                ns.Peek()[ii][jj] = Mathf.Sin(ns.Peek()[ii][jj]);
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
                                ns.Peek()[ii][jj] = Mathf.Cos(ns.Peek()[ii][jj]);
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
                                ns.Peek()[ii][jj] = Mathf.Tan(ns.Peek()[ii][jj]);
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
    public void initialXY()
    {
        x.Clear();
        nx.Clear();
        y.Clear();
        ny.Clear();
        
        int i;
        List<float> tmp = new List<float>();
        List<float> tmpp = new List<float>();
        for (i = 0; i <= sampleFreqx; i += 1)
        {
            tmp.Add(xStart + gap * i);
            tmpp.Add(-(xStart + gap * i));
        }
        x.Add(tmp);
        nx.Add(tmpp);


        for (i = 0; i <= sampleFreqy; i += 1)
        {
            List<float> tmp2 = new List<float>();
            List<float> tmpp2 = new List<float>();
            tmp2.Add(yStart + gap * i);
            tmpp2.Add(-yStart - gap * i);
            y.Add(tmp2);
            ny.Add(tmpp2);
        }
    }

    public List<List<float>> calculate(string s, bool twoD = false)
    {
        System.DateTime startTime, endTime;
        System.TimeSpan time;
        startTime = System.DateTime.Now;
        
        Stack<char> operatestack = new Stack<char>();
        Stack<List<List<float>>> numstack = new Stack<List<List<float>>>();
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
                    if (negaflag) { tmp.Add(0 - readNumFromString(s, ref i)); negaflag = false; }
                    else { tmp.Add(readNumFromString(s, ref i)); }
                    tmp2.Add(tmp);
                    numstack.Push(tmp2);
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
        if (numstack.Peek().Count == 1 && twoD==false)
        {
            List<float> tmp = numstack.Peek()[0];
            for (i = 1; i <= sampleFreqy; i += 1)
            {
                numstack.Peek().Add(tmp);
            }
        }
        if (numstack.Peek()[0].Count == 1)
        {

            for (i = 0; i <= sampleFreqy; i += 1)
            {
                float tmp = numstack.Peek()[i][0];
                for (int j = 1; j <= sampleFreqx; j += 1)
                {
                    numstack.Peek()[i].Add(tmp);
                }

            }
        }
        endTime = System.DateTime.Now;
        time = endTime - startTime;
        int runTime = time.Milliseconds;
        Debug.Log("complete time：" + runTime + "ms。");
        return numstack.Peek();
    }

    public float GetPointValue(float x, float z, string inputStr) {
        // GP LAB : YOUR CODE HERE.
        return 0.0f;
    }


    public List<List<float>> calculateAsyn(string s)
    {
        initialXY();
        Stack<char> operatestack = new Stack<char>();
        Stack<List<List<float>>> numstack = new Stack<List<List<float>>>();
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
                    if (negaflag) { tmp.Add(0 - readNumFromString(s, ref i)); negaflag = false; }
                    else { tmp.Add(readNumFromString(s, ref i)); }
                    tmp2.Add(tmp);
                    numstack.Push(tmp2);
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
            calculateStack(1, ref numstack, ref operatestack);
        }
        if (numstack.Peek().Count == 1)
        {
            List<float> tmp = numstack.Peek()[0];
            for (i = 1; i <= sampleFreqy; i += 1)
            {
                numstack.Peek().Add(tmp);
            }
        }
        if (numstack.Peek()[0].Count == 1)
        {

            for (i = 0; i <= sampleFreqy; i += 1)
            {
                float tmp = numstack.Peek()[i][0];
                for (int j = 1; j <= sampleFreqx; j += 1)
                {
                    numstack.Peek()[i].Add(tmp);
                }

            }
        }
        return numstack.Peek();
    }
}
