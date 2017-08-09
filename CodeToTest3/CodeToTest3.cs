/////////////////////////////////////////////////////////////////////
// CodeToTest3.cs -     Test Code                                  //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett @twcny.rr.com, (315) 443-3948             //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
*   Testing code 3. Test Driver 3 calls and executes this testing code. 
*   
*    division():
*   ->Function that is tested. 
*   
*   Public Class:
*   ============
*   CodeToTest3
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToTest3
{
    public class CodeToTest3
    {
        public bool division(int a, int b, int div)
        {
            int c = 0;
            div = a / c;    //Should throw divide by 0 error
            if (a / b == div)
                return true;
            return false;
        }

        //<-------------------Test Stub For Test Code------------------>
        static void Main(string[] args)
        {
            CodeToTest3 ctt = new CodeToTest3();
            Console.WriteLine(ctt.division(1, 2, 1 / 2));
            Console.Write("\n\n");
        }
    }
}
