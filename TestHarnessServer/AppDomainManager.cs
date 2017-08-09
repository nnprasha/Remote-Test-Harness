/////////////////////////////////////////////////////////////////////
// AppDomainManager.cs - App Domain Manager                        //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This module defines the functionality of an AppDomainManager:
 * 
 * Public Interface:
 * ===================
 * createChildDomain
 * ->Creates child domains for every test request in the Queue and assings that test request to the child domain. 
 * 
 * Public Classes:
 * ==============
 * TestHarness: 
 * ->Defines the above mentioned functions. 
 * 
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 */
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;


namespace RemoteTestHarness
{
    public class AppDomainManager
    {

        public List<Logger> CreateChildDomains(DirectoryInfo directoryInfo,
            TestRequest tr)
        {

            //Creating a new child app domain
            AppDomain childDomain = AppDomain.CreateDomain("ChildAppDomain", null);

            Console.WriteLine("\nChild AppDomain Created for the dequeued test request ---------> Requirement 4");

            childDomain.SetData("TestRequest", tr);    //Setting the data for child domain, where the data
            childDomain.SetData("Directory", directoryInfo);                //is the first test request from the queue. 
            Object ob = childDomain.CreateInstanceAndUnwrap("AppDomainLoader", "RemoteTestHarness.AppDomainLoader");
            ILoader iLoader = (ILoader)ob;
            List<Logger> testLogs = iLoader.LoaderEntryPoint(); //Calling loader, the entry point for the child domain

            
            Console.WriteLine("\nUnloading Child Domain and Back to domain: {0} ---------> Requirement 7(Reference: AppDomainManager.cs Line Number 66", AppDomain.CurrentDomain.FriendlyName);
            
            AppDomain.Unload(childDomain);

            if (directoryInfo != null)
            {   //If the temporary folder was created to store the current test Drivers and test code.
                Console.WriteLine("\nDeleting temporary folder that was created.");
                Directory.Delete(directoryInfo.FullName, true);
                Console.WriteLine("\nTemporary Folder Deleted");
            }
            return testLogs;
        }

        //<---------------------Test Stub for AppDomainManager---------------------->
        
        static void Main()
        {
            string path = "../../../TempDirectory/";
            DirectoryInfo d = Directory.CreateDirectory(Path.GetFullPath(path));
            Parser p = new Parser();

            TestRequest tr = p.parse(MessageTest.makeTestRequest("Nikhil"));
            AppDomainManager apm = new AppDomainManager();
            apm.CreateChildDomains(d, tr);

        }
    }
}
