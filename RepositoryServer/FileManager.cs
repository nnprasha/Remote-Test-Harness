/////////////////////////////////////////////////////////////////////
// FileManager.cs - File Manager                                    //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * The main task of this module is to handle ALL the REPOSITORY related queries:
 * ->Retrieves test drivers and code from repository. 
 * -> Save Test Logs To repository
 *
 * Public Interfaces:
 * ===================
 * RetrieveFilesFromRepo()
 * SaveLogsToRepo
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
    class FileManager
    {
        const string pathToRepo = "../../../Repository/";
        public List<String> RetrieveFilesFromRepo(HashSet<String> currentTestDLLsNames)
        {
            List<String> requiredDLLs = new List<String>();

            DirectoryInfo directory = new DirectoryInfo(pathToRepo + "Dlls/");

            FileInfo[] allDLLsInRepo = directory.GetFiles("*.dll");         

            int flag;
            int i = 0;
            while (i < currentTestDLLsNames.Count)
            {
                flag = 0;
                String currentDLL = currentTestDLLsNames.ElementAt(i);
                foreach (FileInfo file in allDLLsInRepo)
                {
                    if (Path.GetFileName(file.Name).Equals(currentDLL))
                    {
                        requiredDLLs.Add(file.FullName);
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Console.WriteLine("\n{0} file not found in the Repository ------> Requirement 3",currentDLL);
                }
                i++;
            }

            return requiredDLLs;
        }

        public void SaveLogsToRepo(List<Logger> testLogs)
        {
            string path = "../../../Repository/TestLogs/" + testLogs[0].author + " " +
                DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "/";

            Directory.CreateDirectory(path);
            foreach (Logger log in testLogs)
            {
                string fileName = log.author + "-" + log.testName + " " + DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                Console.WriteLine("\n\nSaving {0} -----> Requirement 8(Reference: Inside RepositoryServer Project, open FileManager.cs- Line Number 49-62",fileName);
                using (StreamWriter sw = File.CreateText(path + fileName + ".txt"))
                {
                    sw.WriteLine(log.ToString());
                }
            }

        }

        // <----------- Test Stub --------------->
        static void Main()
        {
            FileManager cfm = new FileManager();
            string req = MessageTest.makeTestRequest("Nikhil");
            HashSet<String> str = new HashSet<string>();
            str.Add("td1.dll");
            str.Add("tc1.dll");

            cfm.RetrieveFilesFromRepo(str);
        }
    }
}
