using System;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using  System.Text.RegularExpressions;

namespace ComputerV1
{
    internal class Program
    {
        private static int dgree = 0;
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No expression provided");
            }
            else
            {
                var expr = Split(args[0]);
                expr = simplify(expr);
                Console.WriteLine("Reduced from: {0}", String.Join("", expr));
                if (getDegree(expr))
                {
                    if (dgree == 2)
                        QuadraticEq(expr);
                    else
                    {
                        BinomialSolve(expr);
                    }
                }
            }

        }

        public static string[] Split(string str)
        {
            string s = str.Replace(".", ",");
            return (Regex.Split(s.Replace(" ", ""), @"(\-)|(\+)|(\=)"));
        }
        public static bool getDegree(string[] expression)
        {
            int tmp = 0;
            int strt = 0;
            int degree = 0;
            Double getval = 0;
            foreach (var VARIABLE in expression)
            {
                if (VARIABLE.Contains("^"))
                {
                    strt = VARIABLE.IndexOf('^') + 1;
                    if (Int32.TryParse(VARIABLE.Substring(strt), out tmp))
                    {
                        Double.TryParse(VARIABLE.Substring(0, VARIABLE.IndexOf('*')), out getval); 

                        if (tmp > degree && getval > 0)
                        {
                            degree = tmp;
                        }
                    }
                }
            }
            dgree = degree;
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
                if (exprLis[i].ToString().Contains("^2") || dgree < 2)
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
                    else if (dgree < 2)
                    {
                        Arr[0, 0] = "+";
                        Arr[0, 1] = "0*X^2";
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
                            Arr[0, 1] = ((val1 + val2) * -1).ToString() + "*" + Arr[0,1][Arr[0,1].IndexOf('^') - 1]+ "^2";
                        }
                        else
                        {
                            Arr[0, 1] = (val1 + val2).ToString() + "*" + Arr[0,1][Arr[0,1].IndexOf('^') - 1]+ "^2";
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
                                Arr[1, 1] = ((val1 + val2) * -1).ToString() + "*" + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^1";
                            }
                            else
                            {
                                Arr[1, 1] = (val1 + val2).ToString() + "*" + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^1";
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
                                Arr[2, 1] = ((val1 + val2) * -1).ToString() + "*" + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^0";
                            }
                            else
                            {
                                Arr[2, 1] = (val1 + val2).ToString() + "*" + Arr[0, 1][Arr[0, 1].IndexOf('^') - 1] + "^0";
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
            return (Split(String.Join("", stmp)));
        }
        //attempt to solve the epression using quadratic equation///should not solve is the square root is negative
        public  static void QuadraticEq(string[] expr)
        {
            double a = 0, b = 0, b2 = 0, b3 = 0, c = 0, ac4 = 0, a2 = 0, sqRoot = 0, x1 = 0, x2 = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i].Contains("^2"))
                {
                    float tmp;
                    Double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
                    Console.WriteLine("a = {0}",a);
                    if (expr[i - 1].Contains("-"))
                    {
                        a *= -1;
                    }
                }
                if (expr[i].Contains("^1"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out b);
                    Console.WriteLine("b = {0}", b);
                    if (expr[i - 1].Contains("-"))
                    {
                        b *= -1;
                    }
                }
                if (expr[i].Contains("^0"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out c);
                    Console.WriteLine("c = {0}",c);
                    if (expr[i - 1].Contains("-"))
                    {
                        c *= -1;
                    }
                }
            }
            foreach (var str in expr)
            {
                
                              
            }
            b2 = b * -1;
            b3 = b * b;
            ac4 = 4 * (a) * (c);
            a2 = 2 * (a);

            if (!(b3 - (ac4) < 0))
            {
                sqRoot = FindSquareRoot_BS(b3 - (ac4));
                x1 = (b2 + sqRoot) / a2;
                x2 = (b2 - sqRoot) / a2;
                Console.WriteLine("----------\nDiscriminant is strictly positive, the two solutions are:\n{0}\n{1}", x1, x2);
            }
            else
            {
                Console.WriteLine("Cannot Solve.");
            }
            
        }

        public static void BinomialSolve(string[] expr)
        {
            double a = 0, b = 0, x = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i].Contains("^1"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
                    Console.WriteLine("a = {0}", a);
                    if (expr[i - 1].Contains("-"))
                    {
                        a *= -1;
                    }
                }
                if (expr[i].Contains("^0"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out b);
                    Console.WriteLine("b = {0}", b);
                    if (expr[i - 1].Contains("-"))
                    {
                        b *= -1;
                    }
                }
            }
            if (b != 0)
            {
                x = a / (b * -1);
                Console.WriteLine("----------\nthe solution is:\n{0}", x);
            }
            else
            {
                Console.WriteLine("Solution is undefined.");
            }
            
        }
        public static float FindSquareRoot_BS(double number)  
        {  
            float precision = 0.0001f;  
            float min = 0;  
            float max = Convert.ToSingle(number);  
            float result = 0;  
            while (max-min > precision)  
            {  
                result = (min + max) / 2;  
                if ((result*result) >= number)  
                {  
                    max = result;  
                }  
                else  
                {  
                    min = result;  
                }  
            }  
            return result;  
        } 
    }
}