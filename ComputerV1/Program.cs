using System;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using  System.Text.RegularExpressions;

namespace ComputerV1
{
    internal class Program
    {
        private static int dgree = 0;
        private static bool dgreeStatus;
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No expression provided");
            }
            else
            {
                var expr = Split(args[0]);
                dgreeStatus = getDegree(expr);
                expr = simplify(expr);
                Console.WriteLine("Polynomial degree: {0}", dgree);
                if (dgreeStatus)
                {
                    Console.WriteLine("Reduced from: {0}", String.Join("", expr));
                    if (dgree == -1)
                    {
                        Console.WriteLine("Expression is not in the correct format.");
                    }
                    else if (dgree == 2)
                        QuadraticEq(expr);
                    else if (dgree == 1)
                        BinomialSolve(expr);
                    else
                        Console.WriteLine("The polynomial degree is stricly less than 1, I can't solve.");
                }
                else if (dgree > 2)
                {
                    Console.WriteLine("The polynomial degree is stricly greater than 2, I can't solve.");
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
                        if (tmp > degree && getval != 0)
                        {
                            degree = tmp;
                        }
                    }
                }
            }
            dgree = degree;
            if (degree > 2)
            {
                return (false);
            }
            return (true);
        }

        public static string[] simplify(string[] expr)
        {
            Int16 indx = 0;
            ArrayList exprLis = new ArrayList();
            
            string[,] Arr = new string[4,2];
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
            if (indx == exprLis.Count)
            {
                dgree = -1;
                return expr;
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
            //make sure the a sign on the first term.
            if (!(exprLis[0].ToString().Contains("-")) && !(exprLis[0].ToString().Contains("+")))
                exprLis.Insert(0, "+");
            //put everything in the natural form
            if (exprLis[indx + 1].ToString() != "0")
            {
                if (!(exprLis[indx + 1].ToString().Contains("-")) && !(exprLis[indx + 1].ToString().Contains("+")))
                    exprLis.Insert(indx + 1, "+");
                for (int i = indx + 1; i < exprLis.Count; i++)
                {
                    if (exprLis[i].ToString().Contains("-"))
                    {
                        exprLis[i] = "+";
                    }
                    else if (exprLis[i].ToString().Contains("+"))
                    {
                        exprLis[i] = "-";
                    }
                }
                exprLis.Remove("=");
                exprLis.Add("=");
                exprLis.Add("0");
            }
            //find duplications of like terms
            for (int i = 0; i < exprLis.Count; i++)
            {
                
                if (exprLis[i].ToString().Contains("^2"))
                {
                    if ((Arr[0, 1] == null) && (Arr[0, 0] == null))
                    {
                        Arr[0, 0] = exprLis[i - 1].ToString();
                        Arr[0, 1] = exprLis[i].ToString();
                    }
                    else
                    {
                        double val1 = 0, val2 = 0;
                        if (!(double.TryParse(Arr[0, 1].Substring(0, Arr[0, 1].IndexOf('^') - 2), out val1)))
                        {
                            Console.WriteLine("format for term {0} is not correct, coefficient will default to 0", Arr[0, 1]);
                        }
                        if (!double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2))
                        {
                            Console.WriteLine("format for term {0} is not correct, coefficient will default to 0", exprLis[i].ToString());
                        }
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
                            Arr[0, 1] = ((val1 + val2) * -1).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));
                        }
                        else
                        {
                            Arr[0, 1] = (val1 + val2).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));
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
                            if (!(double.TryParse(Arr[1, 1].Substring(0, Arr[1, 1].IndexOf('^') - 2), out val1)))
                            {
                                Console.WriteLine("format for term {0} is not correct, coefficient will default to 0", Arr[1, 1]);
                            }
                            if (!double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2))
                            {
                                Console.WriteLine("format for term {0} is not correct, coefficient will default to 0",exprLis[i].ToString());
                            }
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
                                Arr[1, 1] = ((val1 + val2) * -1).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));
                            }
                            else
                            {
                            Arr[1, 1] = (val1 + val2).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));
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
                        if (!(double.TryParse(Arr[2, 1].Substring(0, Arr[2, 1].IndexOf('^') - 2), out val1)))
                        {
                            Console.WriteLine("format for term {0} is not correct, coefficient will default to 0", Arr[2, 1]);
                        }
                        if (!double.TryParse(exprLis[i].ToString().Substring(0, exprLis[i].ToString().IndexOf('^') - 2), out val2))
                        {
                            Console.WriteLine("format for term {0} is not correct, coefficient will default to 0", exprLis[i].ToString());
                        }
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
                                Arr[2, 1] = ((val1 + val2) * -1).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));

                        }
                        else
                            {
                            Arr[2, 1] = (val1 + val2).ToString() + "*" + exprLis[i].ToString().Substring(exprLis[i].ToString().IndexOf('^') - 1, exprLis[i].ToString().IndexOf('^'));
                            }
                    }
                }
                 if (exprLis[i].ToString().Contains("="))
                {
                    Arr[3, 0] = "=";
                    Arr[3, 1] = exprLis[i + 1].ToString();
                }
            }
            if (dgree < 2)
            {
                Arr[0, 0] = "+";
                Arr[0, 1] = "0*X^2";
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

        public  static void QuadraticEq(string[] expr)
        {
            double a = 0, b = 0, b2 = 0, b3 = 0, c = 0, ac4 = 0, a2 = 0, sqRoot = 0, x1 = 0, x2 = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i].Contains("^2"))
                {
                    Double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
                    if (expr[i - 1].Contains("-"))
                    {
                        a *= -1;
                    }
                    Console.WriteLine("a = {0}", a);
                }
                if (expr[i].Contains("^1"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out b);
                    if (expr[i - 1].Contains("-"))
                    {
                        b *= -1;
                    }
                    Console.WriteLine("b = {0}", b);
                }
                if (expr[i].Contains("^0"))
                {
                    double.TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out c);
                    if (expr[i - 1].Contains("-"))
                    {
                        c *= -1;
                    }
                    Console.WriteLine("c = {0}", c);
                } 
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
                Console.WriteLine("----------\nDiscriminant is strictly positive, the two solutions are:\n{0:N2}\n{1:N2}", x1, x2);
            }
            else
            {
                sqRoot = FindSquareRoot_BS((b3 - (ac4)) * -1);
                x1 = sqRoot/a2; //(b2 + sqRoot) / a2;
                x2 = sqRoot/a2; //(b2 - sqRoot) / a2;
                Console.WriteLine("----------\nDiscriminant is strictly negative, the two solutions are:\n{2:N2} + {0:N2}i\n{2:N2} - {1:N2}i", x1, x2, b2/a2);
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
                Console.WriteLine("----------\nthe solution is:\n{0:N2}", x);
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