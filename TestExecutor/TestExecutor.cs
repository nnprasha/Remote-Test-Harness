/////////////////////////////////////////////////////////////////////
// TestExecutor.cs -  Tet Execution Package                        //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett@twcny.rr.com, (315) 443-3948              //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 *  This module is responsible for the test execution.   
 *  ->The Loader calls this package and passes the directory information 
 *  about the DLLs and a List of Test objects. 
 *  ->Loads the DLLs and stored the test drivers in a List. 
 *  ->Runs and executes tests and calls the getLog() function to display the logs. 
 * 
 * Public Interface:
 * ===================
 * TestExecutor()
 * ->Default Constructor that assigns logger reference to ILog interface object. 
 * 
 * TestExecute(DirectoryInfo, List<Test>):
 * ->Retrieves the DLL directory info and a List of test cases from the Loader and starts test execution. 
 * 
 * LoadTests(DirectoryInfo):
 * ->Retrieves DLL info from the Directory and stores the test drivers(derived from ITest) in a list of test drivers(List<TestData>). 
 * 
 * Struct TestData:
 * ->Structure that defines a Test Driver. Includes the name of the test driver and defines an ITest type for the test driver. 
 * 
 *  
 * Public classes:
 * ==============
 * TestExecutor:
 * ->Defines all the above mentioned functions. 
 * 
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Xml.Linq;

namespace RemoteTestHarness
{
    public class TestExecutor
    {
        private List<TestData> testDriver = new List<TestData>();
        private List<TestElement> testElements;
        private TestRequest testRequest { get; set; }
        private List<Logger> testLogs = new List<Logger>();

        public struct TestData
        {
            public string Name;
            public ITest testDriver;
        }

            
        public TestExecutor()
        {
           // iLog = new Logger.Logger();
        }
        //----< load test dlls to invoke >-------------------------------
        bool LoadTests(DirectoryInfo d)
        {
            try
            {
                string[] TestCases = Directory.GetFiles(d.FullName, "*.dll");
                Console.WriteLine("\nRetrieving files from temporary folder that was made to hold the Test Drivers and Test Code");
                foreach (string file in TestCases)  //for each dll encountered in the path 
                {
                    Console.WriteLine("\nloading from path specified above: {0}", Path.GetFileName(file));

                    Assembly assem = Assembly.LoadFrom(file);
                    Type[] types = assem.GetExportedTypes();

                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                        {
                            Console.WriteLine("\t---->Derives from ITest() Interface(Reference: TestExecutor.cs line number 93 and ITestHarness.cs Line number 16 --------> Requirement 5");
                            ITest tdr = (ITest)Activator.CreateInstance(t);    // create instance of test driver

                            // save type name and reference to created type on managed heap
                            TestData td = new TestData();
                            td.Name = t.Name + ".dll";
                            td.testDriver = tdr;
                            testDriver.Add(td);
                        }
                    }
                }
                Console.Write("\n");
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}\n\n", ex.Message);
                return false;
            }
            return testDriver.Count > 0;   // if we have items in list then Load succeeded
        }

        //----< run all the tests on list made in LoadTests >------------
        void run()
        {
            foreach (TestData td in testDriver)  // Test execution for each test driver
            {
                Logger log = new Logger();
                try
                {
                    Console.WriteLine("\n-->**** Testing {0} ****", td.Name);
                    if (td.testDriver.test() == true)
                    {
                        foreach (TestElement t in testElements)
                        {
                            if (t.testDriver == td.Name)
                            {
                                Console.WriteLine("\n\t-->Test Passed and Logs Generated");
                                log.generateLog(t, "PASS", testRequest.author);
                                Console.WriteLine("\n\t-->Logs: {0}", log.ToString());
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (TestElement t in testElements)
                        {
                            if (t.testDriver == td.Name)
                            {
                                Console.WriteLine("\n\t-->Test Failed and Logs Generated");
                                log.generateLog(t,"FAIL", testRequest.author);
                                Console.WriteLine("\n\t-->Logs: {0}", log.ToString());
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n\n\t\t-->**Exception thrown by test driver. Caught in Test Executor: {0}", ex.Message);  //If an exception is thrown, it means the Test has failed. So, generating logs specfying the exception and Test status as failed
                    foreach (TestElement t in testElements)
                    {
                        if (t.testDriver == td.Name)
                        {
                            log.generateLog(t, "FAIL due to Exception: " + ex.Message,testRequest.author);                            
                            break;
                        }
                    }
                }
                testLogs.Add(log);
            }
        }

        //------- Entry Point of the Test Executor Package------
        public List<Logger> TestExecute(DirectoryInfo directoryInfo, TestRequest tr)
        {
            testRequest = tr;
            testElements = testRequest.tests;

            if (LoadTests(directoryInfo))
                run();
            else
                Console.Write("\n  couldn't load tests");

            Console.Write("\n\n");
           return testLogs;
        }


        //<-----------------------------------Test Stub for Test Executor--------------------------------->
        static void Main()
        {
            string path = "../../../Client/LocalRepository/Dlls/";
            DirectoryInfo d = new DirectoryInfo(Path.GetFullPath(path));
            Parser p = new Parser();

            TestRequest tr = p.parse(MessageTest.makeTestRequest("Nikhil"));

            TestExecutor tx = new TestExecutor();
            tx.TestExecute(d, tr);

        }
    }
}
