/////////////////////////////////////////////////////////////////////
// Loader.cs -   Loader of the Test Harness                        //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 *  This module defines the Loader of the test harness. 
 *  It is the entry point for each child app domain created. 
 *  
 *  Public Interfaces:
 *  ===================
 *  AppDomainLoader():
 *  ->Constructor defined that gets the Test request from Main domain
 *    to this chid domain. 
 *
 *  ExecuteTests():
 *  ->Calls the Test Executor for the test execution. 
 *  
 *  LoaderEntryPoint():
 *  ->Entry point of the Loader. Calls all the above described functions.
 *  
 *  
 *  Public Classes:
 *  ==============
 *  Loader:
 *  ->Defines all the functions described above. 
 *  
 *   Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 * 
 */
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace RemoteTestHarness
{
    public class AppDomainLoader : MarshalByRefObject, ILoader
    {
        private TestRequest tr = null;
        private DirectoryInfo tempDirectory = null;
        private static Mutex mut = new Mutex();

        static object obj = new object();
        //Constructor for AppDomainLoader()
        public AppDomainLoader()
        {           
            tr = (TestRequest)AppDomain.CurrentDomain.GetData("TestRequest");
            tempDirectory = (DirectoryInfo)AppDomain.CurrentDomain.GetData("Directory");
        }

        public List<Logger> ExecuteTests()
        {
            List<Logger> testLogs;

            mut.WaitOne();
                TestExecutor te = new TestExecutor();
                testLogs = te.TestExecute(tempDirectory, tr);
            mut.ReleaseMutex();
            
            return testLogs;            
        }

        //EntryPoint for the AppDomain
        public List<Logger> LoaderEntryPoint()
        {
            string title = "CurrentDomain: " + AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("\n{0} -------> Requirement 4", title);

            "Test Request Processing starts --------> Requirement 4".title('-');
            //Executing Tests
            List<Logger> testLogs = ExecuteTests();
            return testLogs;
        }

        //<--------------------- Test Stub ----------------------------------->
        static void Main()
        {
            AppDomainLoader adl = new AppDomainLoader();
            AppDomain.CurrentDomain.SetData("Directory", "../../../Client/LocalRepository/Dlls/");
            string xmlReq = MessageTest.makeTestRequest("Nikhil");
            AppDomain.CurrentDomain.SetData("TestRequest", xmlReq);
            adl.LoaderEntryPoint();
        }
    }
}
