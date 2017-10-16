using System;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using  System.Text.RegularExpressions;

namespace ComputerV1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string[] expr;
            

            if (args.Length != 1)
            {
                Console.WriteLine("No expression provided");
            }
            else
            {
                expr = Regex.Split(args[0].Replace(" ", ""), @"(\-)|(\+)|(\=)");
                simplify(expr);
                if (getDegree(expr))
                {
                    Console.WriteLine("Yay");
                }
            }

        }

        public static bool getDegree(string[] expression)
        {
            int tmp = 0;
            int strt = 0;
            int degree = 0;
            
            foreach (var VARIABLE in expression)
            {
                if (VARIABLE.Contains("^"))
                {
                    strt = VARIABLE.IndexOf('^') + 1;
                    if (Int32.TryParse(VARIABLE.Substring(strt), out tmp))
                    {
                        if (tmp > degree)
                            degree = tmp;
                    }
                }
            }
            Console.WriteLine("Polynomial degree: {0}", degree);
            if (degree > 2)
            {
                Console.WriteLine("The polynomial degree is stricly greater than 2, I can't solve.");
                return (false);
            }
            return (true);
        }

        public static string[] simplify(string[] expr)
        {
            Int16 indx = 0;
            ArrayList exprLis = new ArrayList();
            string rhsSign ="+", rhsValue ="";

            foreach (var VARIABLE in expr)
            {
                exprLis.Add(VARIABLE);
            }
            foreach (var VARIABLE in exprLis)
            {
                if (VARIABLE.ToString() != "=")
                    indx++;
                else
                    break;
            }
            if ((exprLis[indx + 1].ToString() == " "))
                exprLis.RemoveAt(indx+1);
            int tmp;
            //there are issues with the list needs looking at.
            int rhs = 1;
            if (!(int.TryParse(exprLis[indx + 1].ToString(), out tmp)))
            {
                if ((exprLis[indx + 2].ToString().Contains("-")) || (exprLis[indx + 2].ToString().Contains("+")))
                {
                    rhsSign = exprLis[indx + 1].ToString();
                    rhs = 2;
                    
                }
                Console.WriteLine(rhs);
                rhsValue = exprLis[indx + rhs + 1].ToString();
                /*if (rhs == 1)
                {
                    exprLis.Add("-");
                    exprLis[indx] = exprLis[indx + 2];
                    exprLis[indx + 1] = rhsValue;
                    exprLis[indx + 2] = "=";
                    exprLis.Add("0");

                }*/
                if (rhs == 2)
                {
                    if (exprLis[indx + 2].ToString() == "-")
                        exprLis[indx + 2] = "+";
                    else
                        exprLis[indx + 2] = "-";

                    exprLis[indx] = exprLis[indx + 2];
                    exprLis[indx + 2] = rhsValue;
                    exprLis[indx + 3] = "=";
                    exprLis.Add("0");
                }
            }
            foreach (var st in exprLis)
            {
                Console.Write(st.ToString());
            }
            Console.Write("\n");
            return (expr);
        }
    }
}