using System;
using System.Collections.Generic;
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
            foreach (var VARIABLE in expr)
            {
                if (VARIABLE != "=")
                    indx++;
                else
                    break;
            }
            Console.WriteLine(indx);
            return (expr);
        }
    }
}