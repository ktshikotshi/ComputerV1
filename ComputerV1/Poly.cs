﻿using System;
using System.Text.RegularExpressions;

namespace ComputerV1
{
    class Poly
    {
        private double[] expressionValues;
        public Poly(string expression) => ParseExpression(expression);
        private void ParseExpression(string expression)
        {
            expression = expression.Replace(" ", "");
            expression = Regex.Replace(expression, @"(\*)|(\^1)|([xX]\^0)", "");
            var split = expression.Split('=');
            double[] numbs = { 0, 0, 0 };
            for (var i = 0; i < split.Length; i++)
                Reduce(split[i], ref numbs, negate:(i+1==split.Length?true:false));
            expressionValues = numbs;
            Console.WriteLine($"Reduced from : {(numbs[0]!=0? $"{numbs[0]}X^2": "")}" +
                $"{(numbs[0] !=0? $"{(numbs[1] > 0? $" + {numbs[1]}" : $" - {-1 * numbs[1]}X")}":"")}" +
                $"{(numbs[2] != 0 ? $"{(numbs[2] > 0 ? $" + {numbs[2]}" : $" - {-1 * numbs[2]}")}" : "")} = 0");
            for (var i = 2; i < 0; i++)
                if (numbs[i] != 0) { Console.WriteLine($"Degree of Polynomial is {i}"); break; }
        }
        private void Reduce(string expression, ref double[] vals, bool negate)
        {
            var regex = new Regex(@"((\-)?\d+([,.]\d+)?)([xX](\^((\-)?\d+([,.]\d+)?)?)?)?");
            var mathes = regex.Matches(expression.Replace('.',','));
            for (var i = 0; i < mathes.Count; i++)
            {
                var match = mathes[i].Value;
                if (match.Contains("^"))
                {
                    if (double.Parse(match.Substring(match.IndexOf('^') + 1)) != 2)
                    { throw new InvalidEquationException($"Polinomial degree is " +
                        $"{(double.Parse(match.Substring(match.IndexOf('^') + 1)) > 2? "greater than 2": "less than 1")}, kthxbye!"); }
                    else
                        vals[0] += (negate == false ? 1 :-1) * double.Parse(Regex.Match(match, @"^((\-)?\d+([,.]\d+)?)").Value);
                }
                else if (Regex.IsMatch(match, @"^((\-)?\d+([,.]\d+)?)([xX])$"))
                    vals[1] += (negate == false ? 1 : -1) * double.Parse(Regex.Match(match, @"^((\-)?\d+([,.]\d+)?)").Value);
                else if (Regex.IsMatch(match, @"^((\-)?\d+([,.]\d+)?)$"))
                    vals[2] += (negate == false? 1 : -1) * double.Parse(Regex.Match(match, @"^((\-)?\d+([,.]\d+)?)").Value);
            }
        }
        public void Calculate()
        {
            if (expressionValues[0] == 0) { Binomial();  return; }
            double a = expressionValues[0], b = expressionValues[1], c = expressionValues[2];
            double complex = Sqrt(((b * b) - (4 * a * c)) * -1) / (2 * a);
            if (((b * b) - (4 * a * c)) > 0)
                Console.WriteLine($"Solutions on R :\n{((-1*b) - Sqrt((b*b)-(4*a*c))) / (2 * a):0.###}\n{((-1*b) + Sqrt((b * b) - (4 * a * c))) / (2 * a):0.###}");
            else if (((b * b) - (4 * a * c)) < 0)
                Console.WriteLine("Solutions on C :\n{0:0.###} + {1:0.###} * i\n{0:0.###} - {1:0.###} * i", -1 * b/ (2 * a), complex > 0? complex : -1 * complex);
            else
                Console.WriteLine($"Solution on R  : \n{(-1 * b - Sqrt((b * b) - (4 * a * c))) / (2 * a):0.###}");
        }
        private void Binomial()
        {
            if (expressionValues[1] != 0)
                Console.WriteLine($"Solution on R:\n{(-1 * expressionValues[2]) / expressionValues[1]:0.###}");
            else
            {
                if (expressionValues[2] == 0) Console.WriteLine("Solution is undefined.");
                else Console.WriteLine("All real numbers are solution.");
            }
        }
        static double Sqrt(double x)
        {
            if (x <= 0)
                return (x);
            var t = 0.000001;
            while (t * t < x)
                t += 0.000001;
            return double.Parse(t.ToString("0.####"));
        }
    }
}
