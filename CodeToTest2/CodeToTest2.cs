/////////////////////////////////////////////////////////////////////
// CodeToTest2.cs -   Test Code                                   //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                         //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett @twcny.rr.com, (315) 443-3948             //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
*   Testing code 2. Test Driver 2 calls and executes this testing code. 
*   
*    Subtraction():
*   ->Function that is tested. 
*   
*   Public Class:
*   ============
*   CodeToTest2
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToTest2
{
    public class CodeToTest2
    {
        public bool Subtraction(int a, int b, int diff)
        {
            if (b - a == diff)
                return true;
            return false;
        }
        //<----------------Test Stub For Test Code------------------->
        static void Main(string[] args)
        {
            CodeToTest2 ctt = new CodeToTest2();
            Console.WriteLine(ctt.Subtraction(2, 1, 1));
            Console.Write("\n\n");
        }
    }
}
