/////////////////////////////////////////////////////////////////////
// TestHarnessServer.cs - Test Harness runs on this S              //
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
 * Test Harness runs on this Server.
 * 
 * Pulic Interfaces:
 * ===================
 * TestHarnessServer()
 * MakeRequest()
 * wait()
 * 
 * Public Classes: 
 * ==============
 * TestHarnessServer:
 * ->Defines all the functions described above
 *
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 */
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.ServiceModel;

namespace RemoteTestHarness
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    class TestHarnessServer
    {
        public Comm<TestHarnessServer> comm { get; set; } = new Comm<TestHarnessServer>();
        
        static Object obj = new Object();
        public static string tempDirectoryPath = "../../../";
        static int recvmsgcount = 0;
        static bool checkFiles = false;
        static string errorFromRepo = "";
        string repoAddress = "http://localhost:8080/RepositoryServer";
        
        public string endPoint { get; } = Comm<TestHarnessServer>.makeEndPoint("http://localhost", 8080,
            "TestHarnessServer");

        private Thread rcvThread = null;

        public TestHarnessServer()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);            
        }

        public void wait()
        {
            rcvThread.Join();
        }
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string type)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.type = type;
            return msg;
        }

        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                recvmsgcount++;
                msg.time = DateTime.Now;
                Console.WriteLine("\nMessage no {0} Received: ",recvmsgcount);
                msg.showMsg();
                if (msg.type == "NotificationForDllsSent")
                {
                    checkFiles = true;
                    errorFromRepo = msg.body;
                    continue;
                }
                Console.WriteLine("\nTest Request Dequeued for Message no. {0}--------> Requirement 4 (Reference: TestHarnessServer.cs Line Number 56",recvmsgcount);
                Console.WriteLine("\n");
                Action<Message> act = (message) => TestRequestProcessing(message);
                if (msg.body == "quit")
                    break;
                ThreadStart ts = () =>
                {
                    act.Invoke(msg);
                };
                Thread t = new Thread(ts);
                t.Start();
            }
        }  

        public List<object> RequestingRepoForDlls(Message msg,TestRequest tr)
        {
            HashSet<String> currentDlls = new HashSet<String>();
            Console.WriteLine("\nTest Harness Requesting the Repository for the following DLLs: ------> Requirement 6(Reference: TestHarnessServer.cs Line Number 95-103)");
            foreach (TestElement te in tr.tests)
            {
                currentDlls.Add(te.testDriver);
                Console.WriteLine("--> {0}", te.testDriver);
                foreach (string lib in te.testCodes)
                {
                    Console.WriteLine("--> {0}", lib);
                    currentDlls.Add(lib);
                }
            }
            Message reqToRepo = makeMessage("TestHarness", endPoint, repoAddress, "RequestForTestDLLs");
            //Message body containing XML Representation of the DLLs to be retrieved from the Repository
            reqToRepo.body = currentDlls.ToXml();

            tempDirectoryPath += msg.author + " " +
            DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            DirectoryInfo d = Directory.CreateDirectory(@tempDirectoryPath);
            //Sending Request to Repository to retrieve DLLs
            comm.sndr.PostMessage(reqToRepo);
            List<object> ob = new List<object>();
            ob.Add(d);
            ob.Add(currentDlls.Count);
            return ob;
        }

        public void TestResultReplyToClient(Message msg,List<Logger> testLogs)
        {
            Message replyMsgToClient = makeMessage("Server", msg.to, msg.from, "TestResult");
            Console.WriteLine("\nTest Harness is Sending results back to the Client using Async Channel -----> Requirement 6,7 (Reference: TestHarnessServer.cs line number 117-126)");
            foreach (Logger log in testLogs)
            {
                replyMsgToClient.body += log.testName + ":" + log.TestResult + "\n";
            }
            Console.WriteLine("--> {0}", replyMsgToClient.body);
            comm.sndr.PostMessage(replyMsgToClient); //Sending Results back to Client
            Thread.Sleep(100);
            Console.WriteLine("\n Sending Test Logs to the Repository in xml format: -----> REquirement 7(Reference: TestHarnessServer.cs Line Number 131");
            Message msgToRepo = makeMessage("Server", endPoint, repoAddress, "SaveTestLogs");
            msgToRepo.body = testLogs.ToXml();
            Console.WriteLine("\n{0}", msgToRepo.body);
            comm.sndr.PostMessage(msgToRepo); //Sending Logs to Repository
        }

        public void TestRequestProcessing(Message msg)
        {
            lock(obj)
            {
                Console.WriteLine("\nChild Thread Created for Test Request (Message number {0}) ------->Requirement 4(Reference: TestHarnessServer.cs Line number 61, 66 and 75)", recvmsgcount);
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Parser parser = new Parser(); 
                TestRequest tr = parser.parse(msg.body);
                Console.WriteLine("\nTest Request Parsed: \n");
                Console.WriteLine("{0}", tr.ToString());
                if (tr != null)
                {
                    List<object> ob=RequestingRepoForDlls(msg, tr);
                    DirectoryInfo d = (DirectoryInfo)ob.ElementAt(0);
                    int filecount = (int)ob.ElementAt(1);
                    Thread.Sleep(1000);
                    while (true)
                    {
                        if (checkFiles)
                            break;
                    }
                    if (d.GetFiles().Count() != filecount)
                    {
                        Message errorMsgToClient = makeMessage("TestHarnessServer", msg.to, msg.from, "DLLsNotFound");
                        Console.WriteLine("Sending Error Message back to Client about the DLLs that were no found ------> Requirement 3(Reference: TestHarnessServer.cs Line Number 118-121");
                        errorMsgToClient.body = errorFromRepo;
                        comm.sndr.PostMessage(errorMsgToClient);
                    }
                    if (d != null)
                    {   //Creating Child AppDomains
                        AppDomainManager adm = new AppDomainManager();
                        List<Logger> testLogs = adm.CreateChildDomains(d, tr);
                        TestResultReplyToClient(msg,testLogs);
                    }
                }
            }           
        }       
        static void Main(string[] args)
        {
            Console.WriteLine("\t\t\tTest Harness Server");
            Console.WriteLine("=======================================================================\n\n");

            TestHarnessServer server = new TestHarnessServer();        
                       
            //-------------------------------------------------
            Console.Write("\n  press key to exit: ");
            Console.ReadKey();
            
            server.wait();
            Console.Write("\n\n");
        }

     
    }
}
