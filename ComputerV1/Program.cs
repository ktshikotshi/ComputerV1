using System;
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
        const string RgxPar = @"^(\d+\,)?(\d+\.)?\d+\*[a-z]\^\d$";

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
                expr = Reduce(expr);
                GetDegree(expr);
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
                        default:
                            Console.WriteLine("The polynomial degree is stricly less than 1, I can't solve.");
                            break;
                    }
                }
                else if (_dgree > 2)
                    Console.WriteLine("The polynomial degree is stricly greater than 2, I can't solve.");
                else
                {
                    Console.WriteLine("Expression is invalid.");                   
                }
            }
            else
                Console.WriteLine("Arguments are not valid.");

        }

        public static string[] Split(string str)
        {
            var s = str;//.Replace(".", ",");
            return (Regex.Split(s.Replace(" ", ""), @"(\-)|(\+)|(\=)"));
        }

        public static string[] ManageNaturalForm(string[] expr)
        {
            char ch = 'X';
            int elements = 0;
            ArrayList exprLis = new ArrayList();

            foreach (var str in expr)
            {
                exprLis.Add(str);
                elements++;
            }
            for (var i = 0; i < exprLis.Count; i++)
            {
                if (exprLis[i].ToString().Contains("*"))
                    ch = exprLis[i].ToString()[exprLis[i].ToString().IndexOf('*') + 1];
                if ((exprLis[i].ToString().Contains("*") && !(exprLis[i].ToString().Contains("^"))))
                    exprLis[i] = exprLis[i] + "^1";
                double digit = 0;
                if (TryParse(exprLis[i].ToString(), out digit))
                {
                    exprLis[i] = exprLis[i].ToString() + "*" + _termChar + "^0";
                }
                else if ((exprLis[i].ToString().Length == 1) || (exprLis[i].ToString().Length == 3 && exprLis[i].ToString().Contains("^2"))
                    || exprLis[i].ToString().Contains("*"))
                {
                    ch = exprLis[i].ToString()[0];
                    if ((exprLis[i].ToString().Length == 1 && IsLetter(ch)))
                        exprLis[i] = ch + "^1";
                    
                    if (IsLetter(ch) && (ch == exprLis[i].ToString()[exprLis[i].ToString().IndexOf('*') + 1]))
                    {
                        exprLis.Insert(i, "1" + (exprLis[i].ToString().Contains("*") == true ? "" : "*" ) + exprLis[i].ToString());
                        exprLis.RemoveAt(i + 1);
                    }
                    
                }   
            }
            var newExpr = new string[elements];
            int count = 0;
            foreach (var el in exprLis)
            {
                newExpr[count] = el.ToString();
                count++;
            }
            return (newExpr);
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

        public static ArrayList NormalForm(ArrayList exprLis)
        {
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
                Console.WriteLine("Added missing rhs(*= 0)");
                exprLis.Add("=");
                exprLis.Add("0");
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
            if (exprLis[indx + 1].ToString() == "0") return exprLis;
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
            return exprLis;
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
        
        public static string[] Reduce(string[] expr)
        {
            var exprLis = new ArrayList();
            var natural = new string[4,2];
            
            foreach (var str in expr)
            {
                exprLis.Add(str);
            }
            //put all the terms on the left side of the equation.
            exprLis = NormalForm(exprLis);
            //stop exacution if  there is no equal sign on the input
            if (_dgreeStatus == false)
                return expr;
            
            //const string rgxPar  = @"^(\d+\,)?\d+\*[a-zA-Z]?\^\d$";
            //put the equation in the form  + a * x^2 + b * x^1 + c * x^0 = 0 
            for (var i = 0; i < exprLis.Count; i++)
            {
                
                    //get the term character.
                    if (exprLis[i].ToString().Contains("*"))
                        _termChar = IsLetter(exprLis[i].ToString()[exprLis[i].ToString().IndexOf('*') + 1])
                            ? exprLis[i].ToString()[exprLis[i].ToString().IndexOf('*') + 1]
                            : 'X';
                    if (exprLis[i].ToString().Contains("^2"))
                    {
                        if (!(Regex.IsMatch(exprLis[i].ToString(), RgxPar, RegexOptions.IgnoreCase)))
                        {
                            Console.WriteLine("format for term {0} is not correct, please fix it and try again.", exprLis[i].ToString());
                            _dgreeStatus = false;
                            return expr;
                        }
                        if ((natural[0, 1] == null) && (natural[0, 0] == null))
                        {
                            natural[0, 0] = exprLis[i - 1].ToString();
                            natural[0, 1] = exprLis[i].ToString();
                        }
                        else
                            MergeDuplicates(ref natural, exprLis, i, 0);
                    }
                    if (exprLis[i].ToString().Contains("^1"))
                    {
                        if (!(Regex.IsMatch(exprLis[i].ToString(), RgxPar, RegexOptions.IgnoreCase)))
                        {
                            Console.WriteLine("format for term {0} is not correct, please fix it and try again.", exprLis[i].ToString());
                            _dgreeStatus = false;
                            return expr;
                        }
                        if ((natural[1, 1] == null))
                        {
                            if (i == 0)
                                natural[1, 0] = "+";
                            else
                                natural[1, 0] = exprLis[i - 1].ToString();
                            natural[1, 1] = exprLis[i].ToString();
                        }
                        else
                            MergeDuplicates(ref natural, exprLis, i, 1);
                    }
                    if (exprLis[i].ToString().Contains("^0"))
                    {
                        if (!(Regex.IsMatch(exprLis[i].ToString(), RgxPar, RegexOptions.IgnoreCase)))
                        {
                            Console.WriteLine("format for term {0} is not correct, please fix it and try again.", exprLis[i].ToString());
                            _dgreeStatus = false;
                            return expr;
                        }
                        if ((natural[2, 1] == null))
                        {
                            if (i == 0)
                                natural[2, 0] = "+";
                            else
                                natural[2, 0] = exprLis[i - 1].ToString();
                            natural[2, 1] = exprLis[i].ToString();
                        }
                        else
                            MergeDuplicates(ref natural, exprLis, i, 2);
                    }
                    if (!exprLis[i].ToString().Contains("=")) continue;
                    natural[3, 0] = "=";
                    natural[3, 1] = exprLis[i + 1].ToString();
                
            }
            //add a second degree term with a coefficiant of 0 to the equation, if degree is less than 2.
            if (_dgree < 2)
            {
                natural[0, 0] = "+";
                natural[0, 1] = "0*" + _termChar + "^2";
            }
            //add first degree a term with coefficient of 0, if missing
            if (natural[1, 1] == null)
            {
                natural[1, 0] = "+";
                natural[1, 1] = "0*" + _termChar + "^1";
            }
            //add the constant term with the value of 0, if it is missing
            if (natural[2, 1] == null)
            {
                natural[2, 0] = "+";
                natural[2, 1] = "0*" + _termChar + "^0";
            }
            //join the equation back into a single string before spliting it to the proper form again.
            var stmp = new string[4];

            for (var i = 0; i < 4; i++)
            {
                if (natural[i, 1] != null)
                {
                    stmp[i] = natural[i, 0] + natural[i, 1];
                }
                else
                {
                    stmp[i] = "";
                }
            }
            //return the equation in the natural form.
            return (Split(string.Join("", stmp)));
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
                Console.WriteLine("----------\nDiscriminant is strictly positive, the two solutions are:\n{0:0.###}\n{1:0.###}", x1, x2);
            }
            else if (b3 - ac4 < 0)
            {
                sqRoot = Sqrt((b3 - (ac4)) * -1);
                Console.WriteLine("{3} = ({0:0.###} ± {1:0.###} * i ) / {2:0.###}", b2, sqRoot, a2, _termChar);
                Console.WriteLine("{3} = ({0:0.###} / {2:0.###}) ± ({1:0.###} / {2:0.###}) * i", b2, sqRoot, a2, _termChar);
                x1 = sqRoot/a2; 
                x2 = sqRoot/a2;
                Console.WriteLine("----------\nDiscriminant is strictly negative, the two solutions are:\n{2:0.###} + {0:0.###}i\n{2:0.###} - {1:0.###}i", x1, x2, b2/a2);
            }
            else
            {
                sqRoot = Sqrt(b3 - (ac4));
                Console.WriteLine("{3} = ({0:0.###} ± {1:0.###}) / {2:0.###}", b2, sqRoot, a2, _termChar);
                x1 = (b2 + sqRoot) / a2;
                Console.WriteLine("----------\nDiscriminant is null, the solution is:\n{0:0.###}", x1);
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
                var x = b/ a;
                Console.WriteLine("----------\nthe solution is:\n{0:0.###}", x);
            }
            else
                Console.WriteLine("Solution is undefined.");
            
        }
        
        //square root function.
        public static float Sqrt(double number)
        {
            const float precision = 0.0001f;
            float min = 0, result = 0;
            var max = Convert.ToSingle(number);
            
            while (max - min > precision)
            {
                result = (min + max) / 2;
                if ((result * result) >= number)
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