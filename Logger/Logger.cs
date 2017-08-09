/////////////////////////////////////////////////////////////////////
// Logger.cs -  Logger                                             //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This module defines the logger that is used in the test harness. 
 *
 * Public Interfaces:
 * ===================
 * generateLog()
 * ToString()
 * 
 * public classes:
 * ==============
 * Logger:
 * ->Defines the functions described above. 
 * 
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteTestHarness
{
    [Serializable]
    public class Logger
    {
        public string TestResult { get; set; }
        public DateTime timeStamp { get; set; }
        public string testName { get; set; }
        public string author { get; set; }
        public string testDriver { get; set; }
        public List<string> testCode = new List<string>();
        public string testCodes { get; set; }

        public string testEnv = Environment.OSVersion.ToString();
        public string runtimeVersion = Environment.Version.ToString();

        public override string ToString()
        {
            string testLog = "\t\tName of Test Case: " + testName
                                           + "\n\t\tAuthor Name: " + author
                                           + "\n\t\tTest Driver: " + testDriver
                                           + "\n\t\tCode To Test: " + testCodes
                                           + "\n\t\tResult: " + TestResult
                                           + "\n\t\tTimeStamp: " + timeStamp
                                           + "\n\t\t-----------------Other Details-------------------\n"
                                           + "\n\t\tTest Environment: " + testEnv
                                           + "\n\t\tRuntime Versions: " + runtimeVersion;

            return testLog;
        }

        public void generateLog(TestElement t,string result,string author)
        {
            this.TestResult = result;
            this.author = author;
            this.testDriver = t.testDriver;
            this.testName = t.testName;
            this.timeStamp = DateTime.Now;
            foreach(string lib in t.testCodes)
            {
                this.testCode.Add(lib);
                this.testCodes += "  " + lib;
            }
        }

        // <------------- Test Stub ---------------->
        static void Main()
        {
            string xmlReq = MessageTest.makeTestRequest("nikhil Prashar");
            Parser p = new Parser();
            TestRequest tr = p.parse(xmlReq);
            Logger lg = new Logger();
            lg.generateLog(tr.tests.ElementAt(0), "PASS", tr.author);
            Console.WriteLine(lg.ToString());
        }
    }
}
