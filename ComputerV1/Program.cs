﻿using System;
using System.Collections;
using  System.Text.RegularExpressions;
using static System.Char;
using static System.Double;
using static System.Int32;

namespace ComputerV1
{
    internal class Program
    {
        private static int _dgree = 0;
        private static bool _dgreeStatus;
        private static char _termChar = 'X';
        const string RgxPar = @"^(\d+\,)?\d+\*[a-z]\^\d$";

        public static void Main(string[] args)
        {
            if (!(args.Length >= 2))
            {
                string[] expr;
                if (args.Length < 1)
                {
                    //get input from console if arguments are empty or not valid.
                    Console.WriteLine("Please enter an equation :");
                    expr = Split(Console.ReadLine()?.Replace("\"", ""));
                }
                else
                    expr = Split(args[0]);
                //check to see if the equation is in the natural form
                expr = ManageNaturalForm(expr);
                _dgreeStatus = GetDegree(expr);
                expr = ReduceRgx(expr);
                if (_dgree > 2)
                {
                    Console.WriteLine("The polynomial degree is stricly greater than 2, I can't solve.");
                    return ;
                }
                if (_dgreeStatus)
                {
                        if (_dgree > -1)
                        {
                            Console.WriteLine("Reduced from: {0}", string.Join(" ", expr));
                            Console.WriteLine("Polynomial degree: {0}", _dgree);
                        }
                        switch (_dgree)
                    {
                        case -1:
                            Console.WriteLine("Expression is not in the correct format.");
                            break;
                        case 2:
                            QuadraticEq(expr);
                            break;
                        case 1:
                            BinomialSolve(expr);
                            break;
                        case 0:
                            monomialtypes(expr);
                            break;
                        default:
                            Console.WriteLine("Please review your input and try again");
                            break ;
                    }
                }
                else
                {
                    Console.WriteLine("Equation is invalid.");                   
                }
            }
            else
                Console.WriteLine("Arguments are not valid.");
        }

        public static string[] Split(string str)
        {
            var s = str.Replace(".", ",");
            return (Regex.Split(s.Replace(" ", ""), @"(\-)|(\+)|(\=)"));
        }

        public static string[] ManageNaturalForm(string[] expr)
        {
            const string pow1 = @"^(\d+)?(\*)?[A-Za-z](\^[1])?$";
            const string pow2 = @"^(\d+)?(\*)?[A-Za-z]\^[2]$";
            const string pow0 = @"^(\d+)((\*)?[A-Za-z]\^[0])?$";
            var ch = 'X';
            
            for (var i = 0; i < expr.Length; i++)
            {
                if (Regex.IsMatch(expr[i], pow2, RegexOptions.IgnoreCase))
                    expr[i] = (Regex.IsMatch(expr[i], @"^(\d+\,)?\d+", RegexOptions.IgnoreCase)? 
                        Regex.Match(expr[i], @"^(\d+\,)?\d+", RegexOptions.IgnoreCase).ToString(): "1")  + "*"+ ch +"^2";
                else if (Regex.IsMatch(expr[i], pow1, RegexOptions.IgnoreCase))
                    expr[i] = (Regex.IsMatch(expr[i], @"^(\d+\,)?\d+", RegexOptions.IgnoreCase)? 
                        Regex.Match(expr[i], @"^(\d+\,)?\d+", RegexOptions.IgnoreCase).ToString(): "1" ) + "*"+ ch +"^1";
                else if (Regex.IsMatch(expr[i], pow0, RegexOptions.IgnoreCase))
                    expr[i] = Regex.Match(expr[i], @"^(\d+\,)?\d+", RegexOptions.IgnoreCase) + "*"+ ch +"^0";
            }
            return expr;
        }

        public static bool GetDegree(string[] expression)
        {
            var degree = 0;
            foreach (var str in expression)
            {
                if (!str.Contains("^")) continue;
                var strt = str.IndexOf('^') + 1;
                var tmp = 0;
                if (!TryParse(str.Substring(strt), out tmp)) continue;
                double getval = 0;
                if (str.Contains("*"))
                    TryParse(str.Substring(0, str.IndexOf('*')), out getval);
                else
                    return (false);
                if (tmp > degree && getval != 0)
                {
                    degree = tmp;
                }
            }
            _dgree = degree;
            return degree <= 2;
        }
        public static string[] NormRgx(string[] expr)
        {
            var exprLis = new ArrayList();
            var natural = new string[4, 2];

            foreach (var str in expr)
            {
                exprLis.Add(str);
            }
            short indx = 0;

            foreach (var str in exprLis)
            {
                if (str.ToString() != "=")
                    indx++;
                else
                    break;
            }

            if (indx == exprLis.Count)
            {
                _dgreeStatus = false;
                return expr;
            }
            if ((exprLis[indx + 1].ToString() == ""))
                {
                    exprLis.Remove("");
                    indx = 0;
                    foreach (var str in exprLis)
                    {
                        if (str.ToString() != "=")
                            indx++;
                        else
                            break;
                    }
                }
            
            //make sure the a sign on the first term.
            if (!(exprLis[0].ToString().Contains("-")) && !(exprLis[0].ToString().Contains("+")))
                exprLis.Insert(0, "+");
            //put everything in the natural form
            if (!(exprLis[indx + 1].ToString().Contains("-")) && !(exprLis[indx + 1].ToString().Contains("+")))
                exprLis.Insert(indx + 1, "+");
            for (var i = indx + 1; i < exprLis.Count; i++)
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

            return ((string[])exprLis.ToArray(typeof(string)));
        }

        public static void MergeDuplicates(ref string[,] arr, ArrayList exprLis, int i, int arrLoc)
        {
            double val1 = 0;
            double val2 = 0;
            int v1Loc = arr[arrLoc, 1].IndexOf('^');
            int v2Loc = exprLis[i].ToString().IndexOf('^');

            //check if the format of each term matches the format required for calculation
            if ((!TryParse(arr[arrLoc, 1].Substring(0, v1Loc >= 2 && Regex.IsMatch(arr[arrLoc, 1].ToString(),RgxPar, RegexOptions.IgnoreCase) ? v1Loc - 2 : 0), out val1) ||
                !TryParse(exprLis[i].ToString().Substring(0, v2Loc >= 2 && Regex.IsMatch(arr[arrLoc, 1].ToString(), RgxPar, RegexOptions.IgnoreCase) ? v2Loc - 2 : 0), out val2)))
            {
                //stop execution, if the term is the wrong format
                Console.WriteLine("format for term {0} is not correct, please fix it and try again.", exprLis[i].ToString());
                _dgreeStatus = false;
                return ;
            }
            if (arr[arrLoc, 0].Contains("-"))
            {
                val1 *= -1;
            }
            if (exprLis[i - 1].ToString().Contains("-"))
            {
                val2 *= -1;
            }
            if (val1 + val2 < 0)
            {
                arr[arrLoc, 0] = "-";
                arr[arrLoc, 1] = ((val1 + val2) * -1).ToString() + Regex.Match(arr[arrLoc, 1].ToString(), @"\*[a-z]\^\d$", RegexOptions.IgnoreCase);
            }
            else
            {
                arr[arrLoc, 1] = (val1 + val2).ToString() + Regex.Match(arr[arrLoc, 1].ToString(), @"\*[a-z]\^\d$", RegexOptions.IgnoreCase);
            }
        }
        public static double getVal(string s,string s1)
        {
            double var = 0;
            var tmp = Convert.ToDouble( s1 + Regex.Match(s, @"^(\d+\,)?\d+", RegexOptions.IgnoreCase));
            var = tmp;
            return var;
        }
        public static string[] ReduceRgx(string[] expr)
        {
            var natural = new string[8];
            double[] coeff = { 0, 0, 0};
            int eq = 0;
            expr = NormRgx(expr);
            if (_dgreeStatus == false)
                return expr;
            for (var i = 0; i < expr.Length; i++)
            {
                if (expr[i] == "=")
                    eq = i;
                if (expr[i].Contains("^")) {
                    if (Regex.IsMatch(expr[i], RgxPar, RegexOptions.IgnoreCase))
                    {
                        _termChar = expr[i][expr[i].IndexOf('*') + 1];
                        if (expr[i].Contains("^0"))
                            coeff[2] += getVal(expr[i], i > 0 ? (expr[i - 1] == "-" ? "-" : "+") : "+");
                        else if (expr[i].Contains("^1")) {
                            coeff[1] += getVal(expr[i], i > 0 ? (expr[i - 1] == "-" ? "-" : "+") : "+"); }
                        else if (expr[i].Contains("^2"))
                            coeff[0] += getVal(expr[i], i > 0 ? (expr[i - 1] == "-" ? "-" : "+") : "+");
                    }
                    else
                    {
                        _dgreeStatus = false;
                        Console.WriteLine("the term {0} is not valid", expr[i]);
                        return (natural);
                    }
                }
            }
            natural[0] = coeff[0] >= 0 ? "+" : "-";
            natural[1] = (coeff[0] > 0?coeff[0]: coeff[0] * -1) + "*" + _termChar + "^2";
            natural[2] = coeff[1] >= 0 ? "+" : "-";
            natural[3] = (coeff[1] > 0 ? coeff[1] : coeff[1] * -1) + "*" + _termChar + "^1";
            natural[4] = coeff[2] >= 0 ? "+" : "-";
            natural[5] = (coeff[2] > 0 ? coeff[2] : coeff[2] * -1) + "*" + _termChar + "^0";
            natural[6] = "=";
            natural[7] = "0";
            return (natural);
        }
        
        //use the quadratic equation for equations with a degree of 2.
        public  static void QuadraticEq(string[] expr)
        {
            double a = 0, b = 0, b2 = 0, b3 = 0, c = 0, ac4 = 0, a2 = 0, sqRoot = 0, x1 = 0, x2 = 0;
            for (var i = 0; i < expr.Length; i++)
            {
                if (expr[i].Contains("^2"))
                {
                    TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
                    if (expr[i - 1].Contains("-"))
                        a *= -1;
                }
                if (expr[i].Contains("^1"))
                {
                    TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out b);
                    if (expr[i - 1].Contains("-"))
                        b *= -1;
                }
                if (!expr[i].Contains("^0")) continue;
                TryParse(expr[i].Substring(0, expr[i].IndexOf('*') ), out c);
                if (expr[i - 1].Contains("-"))
                    c *= -1;
            }
            if (a == 0)
            {
                BinomialSolve(expr);
                return;
            }
            Console.WriteLine("----------");
            Console.WriteLine("a = {0:0.###}, b = {1:0.###}, c = {2:0.###}", a, b ,c);
            Console.WriteLine("----------");
            b2 = b * -1;
            b3 = b * b;
            ac4 = 4 * (a) * (c);
            a2 = 2 * (a);
            Console.WriteLine("{3} = (-({1:0.###}) ± √({1:0.###}^2 - 4({0:0.###})({2:0.###}))) / 2({0:0.###})", a, b, c, _termChar);
            Console.WriteLine("{4} = ( {0:0.###} ± √({1:0.###} - {2:0.###})) / {3:0.###}", b2, b3, ac4 >= 0 ? ac4 : ac4 * -1, a2, _termChar);
            Console.WriteLine("{3} = ( {0:0.###} ± √({1:0.###})) / {2:0.###}", b2, b3 - ac4, a2, _termChar);
            if (b3 - ac4 > 0)
            {
                sqRoot = Sqrt(b3 - (ac4));
                Console.WriteLine("{3} = ({0:0.###} ± {1:0.###}) / {2:0.###}", b2, sqRoot, a2, _termChar);
                x1 = (b2 + sqRoot) / a2;
                x2 = (b2 - sqRoot) / a2;
                Console.WriteLine("----------\nDiscriminant is strictly positive, the two solutions are:\n{0}\n{1}", FractionView((b2 + sqRoot), a2), FractionView((b2 - sqRoot), a2));
            }
            else if (b3 - ac4 < 0)
            {
                sqRoot = Sqrt((b3 - (ac4)) * -1);
                Console.WriteLine("{3} = ({0:0.###} ± {1:0.###} * i ) / {2:0.###}", b2, sqRoot, a2, _termChar);
                Console.WriteLine("{3} = ({0:0.###} / {2:0.###}) ± ({1:0.###} / {2:0.###}) * i", b2, sqRoot, a2, _termChar);
                x1 = sqRoot/a2; 
                x2 = sqRoot/a2;
                Console.WriteLine("----------\nDiscriminant is strictly negative, the two solutions are:\n{0} + {1} * i\n{0} - {1} * i", FractionView(b2,a2), FractionView(sqRoot, a2));
            }
            else
            {
                sqRoot = Sqrt(b3 - (ac4));
                Console.WriteLine("{3} = ({0:0.###} ± {1:0.###}) / {2:0.###}", b2, sqRoot, a2, _termChar);
                x1 = (b2 + sqRoot) / a2;
                Console.WriteLine("----------\nDiscriminant is null, the solution is:\n{0}", FractionView((b2+sqRoot), a2));
            }
            
        }
    
        //use this for equations with a degree of 1
        public static void BinomialSolve(string[] expr)
        {
            double a = 0, b = 0;
            for (var i = 0; i < expr.Length; i++)
            {
                if (expr[i].Contains("^1"))
                {
                    TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
                    if (expr[i - 1].Contains("-"))
                    {
                        a *= -1;
                    }
                }
                if (!expr[i].Contains("^0")) continue;
                TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out b);
                if (expr[i - 1].Contains("-"))
                {
                    b *= -1;
                }
            }
            Console.WriteLine("----------");
            Console.WriteLine("a = {0:0.###}, b = {1:0.###}", a, b);
            Console.WriteLine("{0:0.###}*{2}^1 + ({1:0.###}) = 0", a, b, _termChar);
            b *= -1;
            Console.WriteLine("{0:0.###}*{2}^1 = {1:0.###}", a, b, _termChar);
            Console.WriteLine("----------");
            if (a != 0)
            {
                Console.WriteLine("({1:0.###} / {1:0.###}) * {2} = {0:0.###} / {1:0.###}", b, a, _termChar);
                //var x = b/ a;
                Console.WriteLine("----------\nthe solution is:\n{0:0.###}", FractionView(b, a));
                //Console.WriteLine(FractionView(12, 18));
            }
            else
                Console.WriteLine("Solution is undefined.");
            
        }
        public static void monomialtypes(string[] expr)
        {
            double a = 0;
            for (var i = 0; i < expr.Length; i++)
            {
                if (!expr[i].Contains("^0")) continue;
                TryParse(expr[i].Substring(0, expr[i].IndexOf('*')), out a);
            }
            if (a == 0)
                Console.WriteLine("All real numbers are a solution");
            else
                Console.WriteLine("The monomial has no solutuins");
        }

        //find lowest common denominator
        public static string FractionView(double a,  double b)
        {
                for (var i = b; i > 0; i--)
                {
                    if (b % i == 0 && a % i == 0)
                    {
                        a /= i; b /= i;
                    }
                }
                if (a % 1 == 0 && b % 1 == 0)
            {
                if (a % b == 0) {return (a / b).ToString("0.###");}
                else if ((a  >= 0 && b >= 0) || (a < 0 && b < 0)) {return (a >= 0 ? a : a * -1) + "/" + (b >= 0 ? b : b * -1);}
                else {return "-" + (a >= 0 ? a : a * -1) + "/" + (b >= 0 ? b : b * -1);}
                }
            return (a / b).ToString("0.###");
        }
        //accurate to 4 decimal points....really slow with big numbers
        static double Sqrt(double x)
        {
            if (x <= 0)
                return (x);
            double t = 0.000001;
            while (t * t < x)
            {
                t += 0.000001;
            }
            return Convert.ToDouble(t.ToString("0.####"));
        }
    }
}