﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionControl : MonoBehaviour
{
    private string function;
    private Dictionary<double, double> inputDict = new Dictionary<double, double>();

    // get output if you replace "x" with your input
    public double output(double a)
    {
        if (inputDict.ContainsKey(a))
        {
            return inputDict[a];
        }
        else
        {
            string functionDupe = function.Replace("x", Convert.ToString(a));
            functionDupe = functionDupe.Replace("X", Convert.ToString(a));
            double output = eval(functionDupe);
            inputDict.Add(a, output);
            return output;
        }
    }

    // evaluate expression
    private double eval(string input)
    {
        string[] array = input.Split(' ');
        Stack<double> eval = new Stack<double>();

        for (int i = 0; i < array.Length; i++)
        {
            string current = array[i];
            if ("+-*/^".Contains(current))
            {
                double t1;
                double t2;
                switch (current)
                {
                    case "+":
                        eval.Push(eval.Pop() + eval.Pop());
                        break;
                    case "-":
                        t1 = eval.Pop();
                        t2 = eval.Pop();
                        eval.Push(t2 - t1);
                        break;
                    case "*":
                        eval.Push(eval.Pop() * eval.Pop());
                        break;
                    case "/":
                        t1 = eval.Pop();
                        t2 = eval.Pop();
                        eval.Push(t2 / t1);
                        break;
                    case "^":
                        t1 = eval.Pop();
                        t2 = eval.Pop();
                        eval.Push(Mathf.Pow((float)(t2), (float)(t1)));
                        break;
                }
            }
            else
            {
                eval.Push(Convert.ToDouble(current));
            }
        }
        return eval.Pop();
    }

    // convert an infix equation to a post fix equation
    private string postFix(string str)
    {
        string a = str; //copy of input
        a = '(' + str + ')'; //surround in parentheses for later
        char[] input = a.ToCharArray();
        string output = "";
        Stack<char> stack = new Stack<char>();
        for (int i = 0; i < input.Length; i++)
        {
            char current = input[i];
            if (current == '(')
            {
                stack.Push(current);
            }
            else if (current == ')')
            {
                char popped = ' ';
                while (popped != '(')
                {
                    if (popped == '+' || popped == '-' || popped == '*' || popped == '/' || popped == '^')
                    {
                        output = output + " " + popped;
                    }
                    popped = stack.Pop();
                }
            }
            else if (current == '+' || current == '-' || current == '*' || current == '/' || current == '^')
            {
                stack.Push(current);
                output = output + " ";
            }
            else
            {
                output = output + current;
            }
        }
        return output;
    }

    // set the current function
    public void SetFunction(string function1)
    {
        function = postFix(function1);
        inputDict.Clear();
    }
}