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
                expr = split(args[0]);
                expr = simplify(expr);
                Console.WriteLine("Reduced from: {0}", String.Join("", expr));
                if (getDegree(expr))
                {
                    Console.WriteLine("Yay");
                }
            }

        }

        public static string[] split(string str)
        {
            return (Regex.Split(str.Replace(" ", ""), @"(\-)|(\+)|(\=)"));
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
            
            string[,] Arr = new string[4,2];
            
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
            if ((exprLis[indx + 1].ToString() == ""))
            {
                exprLis.Remove("");
                indx = 0;
                foreach (var VARIABLE in exprLis)
                {
                    if (VARIABLE.ToString() != "=")
                        indx++;
                    else
                        break;
                }
            }
            int tmp;
            //there are issues with the list needs looking at.
            int rhs = 1;
            if (!(int.TryParse(exprLis[indx + 1].ToString(), out tmp)))
            {
                if ((exprLis[indx + 1].ToString().Contains("-")) || (exprLis[indx + 1].ToString().Contains("+")))
                {
                    rhsSign = exprLis[indx + 1].ToString();
                    rhs = 2;
                    
                }
                rhsValue = exprLis[indx + rhs].ToString();
                if (rhs == 1)
                {
                    exprLis.Add("-");
                    exprLis[indx] = exprLis[indx + 2];
                    exprLis[indx + 1] = rhsValue;
                    exprLis[indx + 2] = "=";
                    exprLis.Add("0");

                }
                if (rhs == 2)
                {
                    if (exprLis[indx + 1].ToString() == "-")
                        exprLis[indx + 1] = "+";
                    else
                        exprLis[indx + 1] = "-";

                    exprLis[indx] = exprLis[indx + 1];
                    exprLis[indx + 1] = rhsValue;
                    exprLis[indx + 2] = "=";
                    exprLis.Add("0");
                }
            }
            
            
            for (int i = 0; i < exprLis.Count; i++)
            {
                if (exprLis[i].ToString().Contains("^2"))
                {
                    if ((Arr[0, 1] == null))
                    {
                        if (i == 0)
                        {
                            Arr[0, 0] = "+";
                        }
                        else
                        {
                            Arr[0, 0] = exprLis[i - 1].ToString();
                        }
                        Arr[0, 1] = exprLis[i].ToString();
                    }
                    else
                    {
                        double val1 = 0, val2 = 0;
                        double.TryParse(Arr[2, 1].Substring(0, Arr[0 , 1].IndexOf('^') - 2), out val1);
                        double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2);
                        Console.WriteLine("val1 {0}, val2 {1}", val1, val2);
                        if (Arr[0, 0].Contains("-"))
                        {
                            val1 *= -1;
                        }
                        if (exprLis[i - 1].ToString().Contains("-"))
                        {
                            val2 *= -1;
                        }
                        if (val1 + val2 < 0)
                        {
                            Arr[0, 0] = "-";
                            Arr[0, 1] = ((val1 + val2) * -1).ToString()  + Arr[0,1][Arr[0,1].IndexOf('^') - 1]+ "^2";
                        }
                        else
                        {
                            Arr[0, 1] = (val1 + val2).ToString()  + Arr[0,1][Arr[0,1].IndexOf('^') - 1]+ "^2";
                        }
                    }
                }
                if (exprLis[i].ToString().Contains("^1"))
                    {
                        if ((Arr[1, 1] == null))
                        {
                            if (i == 0)
                            {
                                Arr[1, 0] = "+";
                            }
                            else
                            {
                                Arr[1, 0] = exprLis[i - 1].ToString();
                            }
                            Arr[1, 1] = exprLis[i].ToString();
                        }
                        else
                        {
                            double val1 = 0, val2 = 0;
                            double.TryParse(Arr[2, 1].Substring(0, Arr[1, 1].IndexOf('^') - 2), out val1);
                            double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2);
                            Console.WriteLine("val1 {0}, val2 {1}", val1, val2);
                            if (Arr[1, 0].Contains("-"))
                            {
                                val1 *= -1;
                            }
                            if (exprLis[i - 1].ToString().Contains("-"))
                            {
                                val2 *= -1;
                            }
                            if (val1 + val2 < 0)
                            {
                                Arr[1, 0] = "-";
                                Arr[1, 1] = ((val1 + val2) * -1).ToString()  + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^1";
                            }
                            else
                            {
                                Arr[1, 1] = (val1 + val2).ToString()  + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^1";
                            }
                        }
                    }
                    if (exprLis[i].ToString().Contains("^0"))
                    {
                        if ((Arr[2, 1] == null))
                        {
                            if (i == 0)
                            {
                                Arr[2, 0] = "+";
                            }
                            else
                            {
                                Arr[2, 0] = exprLis[i - 1].ToString();
                            }
                            Arr[2, 1] = exprLis[i].ToString();
                        }
                        else
                        {
                            double val1 = 0, val2 = 0;
                            double.TryParse(Arr[2, 1].Substring(0, Arr[2, 1].IndexOf('^') - 2), out val1);
                            double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2);
                            if (Arr[2, 0].Contains("-"))
                            {
                                val1 *= -1;
                            }
                            if (exprLis[i - 1].ToString().Contains("-"))
                            {
                                val2 *= -1;
                            }
                            if (val1 + val2 < 0)
                            {
                                Arr[2, 0] = "-";
                                Arr[2, 1] = ((val1 + val2) * -1).ToString() + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^0";
                            }
                            else
                            {
                                Arr[2, 1] = (val1 + val2).ToString() + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^0";
                            }
                        }
                    }
                if (exprLis[i].ToString().Contains("="))
                {
                    Arr[3, 0] = "=";
                    Arr[3, 1] = exprLis[i + 1].ToString();
                }
            }
            string[] stmp = new string[4];

            for (int i = 0; i < 4; i++)
            {
                if (Arr[i, 1] != null)
                {
                    stmp[i] = Arr[i, 0] + Arr[i, 1];
                }
                else
                {
                    stmp[i] = "";
                }
            }
            return (split(String.Join("", stmp)));
        }
    }
}