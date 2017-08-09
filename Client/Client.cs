/////////////////////////////////////////////////////////////////////
// Client.cs - Client of the test harness                          //
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
 * This Module defines Client1 of a test harness. 
 * -> Sends test request to test harness
 * -> Querys the Repository
 * Pulic Interfaces:
 * ===================
 * ->Client()
 * ->MakeMessage()
 * ->wait()
 * ->sendFile()
 * ->SendingDllsToRepository()
 * Public Classes: 
 * ==============
 * Client:
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

namespace RemoteTestHarness
{
    class Client
    {
        public Comm<Client> comm { get; set; } = new Comm<Client>();

        public string endPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8080,"Client");
        
        private Thread rcvThread = null;

        static bool resultArrived = false;
        //----< initialize receiver >------------------------------------

        public Client()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }
        //----< join receive thread >------------------------------------

        public void wait()
        {
            rcvThread.Join();
        }
        //----< construct a basic message >------------------------------

        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string type)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.type = type;
            return msg;
        }
        //----< use private service method to receive a message >--------

        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                msg.showMsg();
                if(msg.type == "TestResult")
                    resultArrived = true;
                if(msg.type == "RepositoryNotificationForLogs")
                {
                    String body = "Logs Sent: \n";
                    if (msg.body.FromXml<string[]>() != null)
                    {
                        int count = msg.body.FromXml<string[]>().Count();
                        if (count > 0)
                            Console.WriteLine("Logs can be found at: WPFClient1//TestLogs//");
                        foreach (string a in msg.body.FromXml<string[]>())
                        {
                            body += System.IO.Path.GetFileName(System.IO.Path.GetFullPath(a)) + "\n";
                        }
                        Console.WriteLine("\n{0}", body);
                    }
                }
                if (msg.body == "quit")
                    break;
            }
        }

        //----< run client demo >----------------------------------------
        static IFileService CreateChannelForFileTransfer(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<IFileService> factory = new ChannelFactory<IFileService>(binding, address);
            return factory.CreateChannel();
        }

        public bool SendFile(IFileService service, string file)
        {
            long blockSize = 512;
            try
            {
                string filename = System.IO.Path.GetFileName(file);

                service.OpenFileForWrite(filename);
                FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
                int bytesRead = 0;
                while (true)
                {
                    long remainder = (int)(fs.Length - fs.Position);
                    if (remainder == 0)
                        break;
                    long size = Math.Min(blockSize, remainder);
                    byte[] block = new byte[size];
                    bytesRead = fs.Read(block, 0, block.Length);
                    service.WriteFileBlock(block);
                }
                fs.Close();
                service.CloseFile();
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n  can't open {0} for writing - {1}", file, ex.Message);
                return false;
            }
        }
        public void SendingDllsToRepository(Message msg)
        {
            Parser p = new Parser();

            TestRequest tr = p.parse(msg.body);
            ClientFileManager cfm = new ClientFileManager();
            List<string> dlls = cfm.RetrieveFilesFromRepo(tr);

            int count = 0;
            IFileService fs = null;
            while (true)
            {
                try
                {
                    fs = CreateChannelForFileTransfer("http://localhost:8080/RepositoryServer");
                    break;
                }
                catch 
                {
                    Console.Write("\n  connection to Test Harness service failed {0} times - trying again", ++count);
                    Thread.Sleep(500);
                    continue;
                }
            }

            Console.WriteLine("Sending the Following dlls to the Repository ------> Requirement 6(Reference: Client.cs line number 68-92,141)");
            foreach (string file in dlls)
            {
                string filename = System.IO.Path.GetFileName(file);
                Console.Write("\n\t  sending file {0} from Client's Local Repository to Main Repository", filename);

                if (!SendFile(fs, file))
                    Console.Write("\n  could not send file");
            }
        }
        public void QueryTheRepoForAllLogs()
        {
            string author = "Nikhil Prashar";
            string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8080, "RepositoryServer");
            Message msg = makeMessage(author, endPoint, remoteEndPoint, "RetrieveAllLogs");
            msg.body = "Retrieve all logs";
            Console.WriteLine("\nClient sending message to Repository to Retrieve All The Logs: {0} ------> Requirement Number 9(Reference: Client.cs line number 138-144)", msg.ToString());
            comm.sndr.PostMessage(msg);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("\t\t\tClient1");
            Console.WriteLine("=======================================================================\n\n");

            Client client = new Client();
            string author1 = "Nikhil Prashar";
            string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost",8080,"TestHarnessServer");
            Message msg = client.makeMessage(author1,client.endPoint,remoteEndPoint, "TestRequest");
            msg.body = MessageTest.makeTestRequest(author1);
            Console.WriteLine();
            "Demonstrating Requirement Number 2".title('-');
            Console.WriteLine("\n\tMessage Created at Client to be sent to the Test Harness\n{0}", msg.ToString());
    
            client.SendingDllsToRepository(msg); //Sending Dlls to the repo

            resultArrived = false;
            client.comm.sndr.PostMessage(msg);  //Sending test request message to the Test Harness

            Console.WriteLine("\n\tTest Request Sent To the Test Harness Server -------> Requirement 6(Reference: Client.cs Line Number 146");
            Console.WriteLine("\n\tReference: Client Project, Line number 110-148");
            Console.WriteLine("\n-----------------------------------------------\n");

            while(true)
            {
                if(resultArrived)
                {
                    Thread.Sleep(2000);
                    client.QueryTheRepoForAllLogs(); //Quering the repo for all logs
                    break;
                }
            }
            
            Console.ReadKey();
            msg.to = client.endPoint;
            msg.body = "quit";
            client.comm.sndr.PostMessage(msg);
            client.wait();
            Console.Write("\n\n");
        }
        
    }

}
