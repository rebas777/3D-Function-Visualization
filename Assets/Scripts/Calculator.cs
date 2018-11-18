using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator {
	public static float x;
	public float y;

	public static float readNumFromString(string s, ref int index){
		bool end = false;
		string numstring="";
		while (s[index] == ' ') { index++; }
		while (!end && index<s.Length) {
			if (s[index] <= '9'&&s[index] >= '0' || s[index]=='.') {
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


    public static void calculateStack(int i,ref Stack<float> ns, ref Stack<char> cs,bool reserve=false) {
        while (true)
        {
            if (cs.Count == 0) { return; }
            char op = cs.Peek();
            //cs.Pop();
            float num1;
            float num2;
            switch (op)
            {
                case '+':
                    if (i == 2) { return; }
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();
                    ns.Push(num1 + num2);
                    break;
                case '-':
                    if (i == 2) { return; }
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();
                    ns.Push(num2 - num1);
                    break;
                case '*':
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();
                    ns.Push(num1 * num2);
                    break;
                case '/':
                    cs.Pop();
                    num1 = ns.Peek(); ns.Pop();
                    num2 = ns.Peek(); ns.Pop();
                    ns.Push(num2 / num1);
                    break;
                case '(':
                    if (i == 3) { cs.Pop(); }
                    return;
                case 's':
                    if (i == 3)
                    {
                        float num = ns.Peek();
                        num = Mathf.Sin(num);
                        ns.Pop();ns.Push(num);
                        cs.Pop();
                    }
                    return;
                case 'c':
                    if (i == 3)
                    {
                        float num = ns.Peek();
                        num = Mathf.Cos(num);
                        ns.Pop(); ns.Push(num);
                        cs.Pop();
                        //cs.Pop();
                    }
                    return;
                case 't':
                    if (i == 3)
                    {
                        float num = ns.Peek();
                        num = Mathf.Tan(num);
                        ns.Pop(); ns.Push(num);
                        cs.Pop();
                    }
                    return;
                case 'p':

                    if (i == 3 && !reserve)
                    {
                        num1 = ns.Peek(); ns.Pop();
                        num2 = ns.Peek(); ns.Pop();

                        ns.Push(Mathf.Pow(num2,num1));
                        cs.Pop();
                    }
                    return;
                case 'l':

                    if (i == 3 && !reserve)
                    {
                        num1 = ns.Peek(); ns.Pop();
                        num2 = ns.Peek(); ns.Pop();

                        ns.Push(Mathf.Log(num2, num1));
                        cs.Pop();
                    }
                    return;
                default:
                    break;
            }

        }





        while (true) {
			if (cs.Count==0) { return; }
			char op = cs.Peek();
			//cs.Pop();
			float num1 = 0;
			float num2 = 0;
			switch (op)
			{
			case '+':
				if (i == 2) { return; }
				cs.Pop();
				num1 = ns.Peek(); ns.Pop();
				num2 = ns.Peek(); ns.Pop();
				ns.Push(num1 + num2);
				break;
			case '-':
				if (i == 2) { return; }
				cs.Pop();
				num1 = ns.Peek(); ns.Pop();
				num2 = ns.Peek(); ns.Pop();
				ns.Push(num2 - num1);
				break;
			case '*':
				cs.Pop();
				num1 = ns.Peek(); ns.Pop();
				num2 = ns.Peek(); ns.Pop();
				ns.Push(num1 * num2);
				break;
			case '/':
				cs.Pop();
				num1 = ns.Peek(); ns.Pop();
				num2 = ns.Peek(); ns.Pop();
				ns.Push(num2 / num1);
				break;
			case '(':
				if (i == 3) { cs.Pop(); }
				return;
			default:
				break;
			}

		}
	}



	public float calculate(string s, float xin,float yin=0){
        x = xin;
        y = yin;
		Stack<char> operatestack = new Stack<char>();
		Stack<float> numstack = new Stack<float>();
		int i = 0;
		int ssize = s.Length;

        bool negaflag = false;

        while (i < s.Length) {
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
                    if (negaflag)
                        numstack.Push(0-readNumFromString(s, ref i));
                    numstack.Push(readNumFromString(s, ref i));
                    break;

                case 'x':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(-x); negaflag = false; }
                    else { numstack.Push(x); }
                    i++;
                    break;
                case 'y':
                    if (expectOperator(s, i))
                    {
                        calculateStack(2, ref numstack, ref operatestack);
                        operatestack.Push('*');
                    }
                    if (negaflag) { numstack.Push(-y); negaflag = false; }
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
		while (!(operatestack.Count==0)) {
			calculateStack(1, ref numstack, ref operatestack);
		}
		return numstack.Peek();
	}

  
}
