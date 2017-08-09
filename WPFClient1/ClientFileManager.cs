/////////////////////////////////////////////////////////////////////
// ClientFileManager.cs - File Manager for client                  //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * The main task of this module is to handle ALL the REPOSITORY related queries:
 * ->Retrieves test drivers and code from Local repository. 

 * Public Interfaces:
 * ===================
 * RetrieveFilesFromRepo()
 * 
 * Public Classes:
 * ==============
 * FileManager:
 * ->Defines all the functions described above. 
 * 
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 *
 */
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteTestHarness
{
    class ClientFileManager
    {
           public List<String> RetrieveFilesFromRepo(TestRequest tr)
        {
            string pathToRepo = "../../../WPFClient1/LocalRepository/";
            List<String> requiredDLLs = new List<String>();
            
                DirectoryInfo directory = new DirectoryInfo(pathToRepo + "Dlls/");

                FileInfo[] allDLLsInRepo = directory.GetFiles("*.dll");


                HashSet<String> currentTestDLLsNames = new HashSet<String>();//Names of the current Test DLLs


                //Retrieving all DLL Names from Test Request
                foreach (TestElement t in tr.tests)
                {
                    currentTestDLLsNames.Add(t.testDriver);
                    foreach (string lib in t.testCodes)
                    {
                        currentTestDLLsNames.Add(lib);
                    }
                }

                int flag;
                int i = 0;
                while (i < currentTestDLLsNames.Count)
                {
                    flag = 0;
                    String currentDLL = currentTestDLLsNames.ElementAt(i);
                    foreach (FileInfo file in allDLLsInRepo)
                    {
                        if (Path.GetFileName(file.FullName).Equals(currentDLL))
                        {
                            requiredDLLs.Add(file.FullName);
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                    Console.WriteLine("\n{0} file not found in the Local Repository ", currentDLL);
                }
                    i++;
                }         
            
            return requiredDLLs;
        }
        
    }
}
