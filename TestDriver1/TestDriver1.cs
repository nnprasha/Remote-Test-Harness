﻿/////////////////////////////////////////////////////////////////////
// TestDriver1.cs -   Test Driver                                  //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Prototype                               //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett @twcny.rr.com, (315) 443-3948             //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
*   Test driver needs to know the types and their interfaces
*   used by the code it will test.  It doesn't need to know
*   anything about the test harness.
*   
*   Function Operations:
*   ====================
*   TestDriver1():
*   ->Default constructor that creates a reference to the test code. 
*   
*   test():
*   ->Function where the test executes. 
*   
*   create():
*   ->Creating an ITest reference to test driver 1. 
*   
*   Public Classes:
*   ==============
*   TestDriver1
*   
*   Public Interfaces:
*   =================
*   ITest:
*   ->bool test()
*   
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RemoteTestHarness
{
    using CodeToTest1;

    public class TestDriver1 : ITest
    {
        private CodeToTest1 code;  // will be compiled into separate DLL

        //----< Testdriver constructor >---------------------------------
        /*
        *  For production code the test driver may need the tested code
        *  to provide a creational function.
        */
        public TestDriver1()
        {
            code = new CodeToTest1();
        }
        //----< factory function >---------------------------------------
        /*
        *   This can't be used by any code that doesn't know the name
        *   of this class.  That means the TestHarness will need to
        *   use reflection - ugh!
        *
        *   The language gives us this problem because it won't
        *   allow a static method in an interface or abstract class.
        */
        public static ITest create()
        {
            return new TestDriver1();
        }
        //----< test method is where all the testing gets done >---------

        public bool test()
        {
            if (code.addition(1, 2, 3) == true)
                return true;

            return false;
        }


        //<-------------------Test Stub for Test Driver--------------->

        static void Main(string[] args)
        {
            Console.Write("\n  Local test:\n");

            ITest test = TestDriver1.create();

            if (test.test() == true)
                Console.Write("\n  test passed");
            else
                Console.Write("\n  test failed");
            Console.Write("\n\n");
        }
    }
}
