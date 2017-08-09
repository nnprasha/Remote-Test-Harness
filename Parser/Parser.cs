/////////////////////////////////////////////////////////////////////
// Parser.cs -  Test Request Parser                                //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,             //
//              jfawcett @twcny.rr.com, (315) 443-3948             //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 *  This module defines the Test Request Parser.
 *
 * Public Interfaces:
 * ===================
 * Parser():
 * ->Constructed defined that initialises an XDocument type and a List of type <Test>. 
 * 
 * parse(String):
 * ->Parses the input string and adds the test cases info to the List<TestRequest> and returns it. 
 * 
 * Public classes:
 * ==============
 * Parser:
 * ->Defines the parse function, main and the constructor. 
 * 
 *    Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
namespace RemoteTestHarness
{

    public class Parser
    {
        private XDocument doc_ { get; set; }
        private TestRequest tr = null;
        static object obj = new object();
        // ------ Default Constructor --------------
        public Parser()
        {
            doc_ = new XDocument();
            tr = new TestRequest();
        }

        //----------- Parses the xml string -----------
        public TestRequest parse(String xml)
        {
            
                try
                {
                    //parses the xml string.                
                    doc_ = XDocument.Parse(xml);
                    if (doc_ != null)
                    {
                        string author = doc_.Descendants("author").First().Value;
                        tr.author = author;

                        // Console.WriteLine("\n--> Inside Parser. Current Domain : {0}", AppDomain.CurrentDomain.FriendlyName);
                        XElement[] xtests = doc_.Descendants("TestElement").ToArray();
                        int numTests = xtests.Count();
                        foreach (var xtest in xtests)
                        {
                            TestElement test = new TestElement();
                            test.testName = xtest.Element("testName").Value;
                            test.testDriver = xtest.Element("testDriver").Value;
                            XElement xtestCodes = xtest.Element("testCodes");
                            IEnumerable<XElement> xLibraries = xtestCodes.Elements("string");
                            foreach (var lib in xLibraries)
                            {
                                test.testCodes.Add(lib.Value);
                            }
                            tr.tests.Add(test);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n\n-->##### Exception caught in Parser: {0}", ex.Message);
                    tr = null;   //Passes null to the test list (List<Test>) if any error encountered while parsing. 
                    Console.WriteLine("\n\n-->Assigned NULL to test case List");
                    Console.WriteLine("\n\n-->*** XML FILE needs to be fixed!!");
                }
            
            
           // Console.WriteLine("\n--> Exiting Parser & returning <Test List>. Current Domain : {0}", AppDomain.CurrentDomain.FriendlyName);
            return tr;
        }

        //<----------------------------------Test Stub for Parser------------------------------------>
       static void Main()
        {
            String xmlReq=MessageTest.makeTestRequest("nikhil");
            Parser p = new Parser();
            TestRequest tr=p.parse(xmlReq);
            Console.WriteLine(tr.ToString());
        }
    }
}
