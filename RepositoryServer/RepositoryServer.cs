/////////////////////////////////////////////////////////////////////
// RepositoryServer.cs - Repository Server that handles            // 
//                        the Repository                           //
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
 * This Module defines the Repository Server. 
 * 
 * Pulic Interfaces:
 * ===================
 * MakeMessage()
 * SendFiles()
 * CheckIfAllDllsPresent()
 * 
 * Public Classes: 
 * ==============
 * RepositoryServer:
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
using System.ServiceModel;
using System.IO;

namespace RemoteTestHarness{
    
    class RepositoryServer
    {
        public Comm<RepositoryServer> comm { get; set; } = new Comm<RepositoryServer>();
      
        static Object obj = new Object();
       
        private Thread rcvThread = null;

        //Creating channel to transfer Files
        static IFileService CreateChannelForFileTransfer(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<IFileService> factory = new ChannelFactory<IFileService>(binding, address);
            return factory.CreateChannel();
        }
        public RepositoryServer()
        {
           comm.rcvr.CreateRecvChannel("http://localhost:8080/RepositoryServer");
           rcvThread = comm.rcvr.start(rcvThreadProc);            
        }

     public bool SendFile(IFileService service, string file)
    {
      long blockSize = 512;
      try
      {        
        string filename = Path.GetFileName(file);
        
        service.OpenFileForWrite(filename);
        FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
        int bytesRead = 0;
        while (true)
        {
          long remainder = (int)(fs.Length - fs.Position);
          if (remainder == 0)
            break;
          long size = Math.Min(blockSize, remainder);
          byte[] block = new byte[size]; //Sending File using Chunks
          bytesRead = fs.Read(block, 0, block.Length);
          service.WriteFileBlock(block);
        }
        fs.Close();
        service.CloseFile();
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n  can't open {0} for writing - {1}", file, ex.Message);
        return false;
      }
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
                msg.time = DateTime.Now;

            //Console.Write("\n  {0} received message:", comm.name);
                msg.showMsg();
                Action<Message> act = null;

                if (msg.from == "http://localhost:8080/TestHarnessServer")
                    act = (message) => TestHarnessMessageProcessing(message);
                else 
                    act = (message) => ClientMessageProcessing(message);
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

        void ClientMessageProcessing(Message msg)
        {
             if (msg.type == "RetrieveAllLogs")
            {   //Retrieving all logs from Repository
                string path = "../../../Repository/TestLogs/";
                string[] logs = Directory.GetFiles(@path, "*", SearchOption.AllDirectories);

                if(logs.Count() > 0)
                {
                    //Sending all files 
                    int count = 0;
                    IFileService fs = null;
                    while (true)
                    {
                        try
                        {
                            fs = CreateChannelForFileTransfer(msg.from); //Creating Channel to transfer Files
                            break;
                        }
                        catch
                        {
                            Console.Write("\n  connection to Test Harness service failed {0} times - trying again", ++count);
                            Thread.Sleep(500);
                            continue;
                        }
                    }
                    Console.WriteLine("\nRepository is sending back the following files in chunks: -----> Requirement number 9(Reference: RepositoryServer.cs Line number 37-66,135. CommService.cs Line Number 70-114");
                    foreach (string file in logs)
                    {
                        string filename = Path.GetFileName(file);
                        Console.WriteLine("--->sending file {0}", filename);

                        if (!SendFile(fs, file))
                            Console.Write("\n  could not send file");
                    }
                }
               
                Message msg2 = makeMessage("Repository", msg.to, msg.from, "RepositoryNotificationForLogs");
                if (logs.Count() > 0)
                    msg2.body = logs.ToXml();
                else
                    msg2.body = "No Logs Present in Repo";

                comm.sndr.PostMessage(msg2);
            }
        }
        public String CheckIfAllDllsPresent(HashSet<String> a,List<String> b)
        {
            int flag = 0;
            String retStr = "";
            foreach(string file in a)
            {
                flag = 0;
                foreach(string file2 in b)
                {
                    if(file == Path.GetFileName(file2))
                    {
                        flag = 1;
                        break;
                    }                    
                }
                if (flag == 0)
                {
                    retStr += file + " ";
                }
            }
            return retStr;        
        }
        void TestHarnessMessageProcessing(Message msg)
        {
            if(msg.type == "RequestForTestDLLs")
            {
                string filesnotfound = "";
                HashSet<String> currentTestDlls = msg.body.FromXml<HashSet<String>>();
                FileManager fm = new FileManager();
                List<String> requiredTestDllsFiles=fm.RetrieveFilesFromRepo(currentTestDlls);
                if (currentTestDlls.Count != requiredTestDllsFiles.Count)
                {                   
                    filesnotfound = "Dlls not found in Repo: " + CheckIfAllDllsPresent(currentTestDlls, requiredTestDllsFiles);
                }
                int count = 0;
                IFileService fs = null;
                while (true)
                {
                    try
                    {
                        fs = CreateChannelForFileTransfer(msg.from); //Creating Channel to transfer Files
                        break;
                    }
                    catch
                    {
                        Console.Write("\n  connection to Test Harness service failed {0} times - trying again", ++count);
                        Thread.Sleep(500);
                        continue;
                    }
                }
                Console.WriteLine("\nRepository is sending back the following files in chunks: ------> Requirement 6 (Reference: Repository.cs Line Number 200-107) ");
                foreach (string file in requiredTestDllsFiles)
                {
                    string filename = Path.GetFileName(file);
                    Console.Write("\n  sending file {0}", filename);

                    if (!SendFile(fs, file))
                        Console.Write("\n  could not send file");
                }         
                Message msg2 = makeMessage("Repository", msg.to, msg.from, "NotificationForDllsSent");
                msg2.body = filesnotfound;
                comm.sndr.PostMessage(msg2);
            }
            else if(msg.type == "SaveTestLogs")
            {
                Console.WriteLine("\n Saving Test Logs received from Test Harness to the Repository: ----------> Requirement number 7(Reference: RepositoryServer.cs Line Number 89,150,190)");
                List<Logger> testLogs = msg.body.FromXml<List<Logger>>();
                FileManager fm = new FileManager();
                fm.SaveLogsToRepo(testLogs);
            }           
        }
        
        static void Main()
        {
            Console.WriteLine("\t\t\tRepository Server");
            Console.WriteLine("=======================================================================\n\n");

            RepositoryServer repo = new RepositoryServer();
        }

       
    }
}
