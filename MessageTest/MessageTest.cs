// Message.cs -  Message Class                                      //
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
 * This module defines the Test Request and Test Elements that is used by the test harness. 
 *
 * Public Interfaces:
 * ===================
 * MakeTestRequest()
 * ToString()
 * 
 * public classes:
 * ==============
 * TestElement
 * TestRequest
 * MessageTest
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
    public class TestElement
    {
        public string testName { get; set; }
        public string testDriver { get; set; }
        public List<string> testCodes { get; set; } = new List<string>();

        public TestElement() { }
        public TestElement(string name)
        {
            testName = name;
        }
        public void addDriver(string name)
        {
            testDriver = name;
        }
        public void addCode(string name)
        {
            testCodes.Add(name);
        }
        public override string ToString()
        {
            string te = "\ntestName:\t" + testName;
            te += "\ntestDriver:\t" + testDriver;
            foreach (string code in testCodes)
            {
                te += "\ntestCode:\t" + code;
            }
            return te += "\n";
        }
    }
    [Serializable]
    public class TestRequest
    {
        public TestRequest() { }
        public string author { get; set; }
        public List<TestElement> tests { get; set; } = new List<TestElement>();

        public override string ToString()
        {
            string tr = "\nAuthor:\t" + author + "\n";
            foreach (TestElement te in tests)
            {
                tr += te.ToString();
            }
            return tr;
        }
    }

    public class MessageTest
    {
        public static string makeTestRequest(string author)
        {
            TestElement te1 = new TestElement("test1");
            te1.addDriver("TestDriver1.dll");
            te1.addCode("CodeToTest1.dll");
            TestElement te2 = new TestElement("test2");
            te2.addDriver("TestDriver2.dll");
            te2.addCode("CodeToTest2.dll");
            TestElement te3 = new TestElement("test3");
            te3.addDriver("TestDriver4.dll");
            te3.addCode("CodeToTest4.dll");

            TestRequest tr = new TestRequest();
            tr.author = author;
            tr.tests.Add(te1);
            tr.tests.Add(te2);
            tr.tests.Add(te3);
            return tr.ToXml();
        }

        //------Test Stub----------
        static void Main()
        {
            String xmlReq=MessageTest.makeTestRequest("nikhil");
            Console.WriteLine(xmlReq);
        }
    }
}
