/////////////////////////////////////////////////////////////////////
// CodeToTest1.cs -   Test Code                                   //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett @twcny.rr.com, (315) 443-3948             //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
*   Testing code 1. Test Driver 1 calls and executes this testing code. 
*   
*   addition():
*   ->Function that is tested. 
*   
*   Public Class:
*   ============
*   CodeToTest1
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToTest1
{
    public class CodeToTest1
    {
        public bool addition(int a, int b, int sum)
        {
            if (a + b == sum)
                return true;
            return false;
        }

        //<-------------------Test Stub For Test Code------------------>
        static void Main(string[] args)
        {
            CodeToTest1 ctt = new CodeToTest1();
            Console.WriteLine(ctt.addition(1, 2, 3));
            Console.Write("\n\n");
        }
    }
}
