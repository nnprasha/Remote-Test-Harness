/////////////////////////////////////////////////////////////////////
// Mainwindow.cs - GUI Cleint 1                                    //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 4                                          //
// Author:      Nikhil Prashar, Syracuse University,               //
//              nnprasha@syr.edu, (914) 733-8184                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This module defines the GUI for a client. 
 * 
 * Public Interface:
 * ===================
 * WPFClient1()
 * wait()
 * MakeMessage()
 * SendFile()
 * 
 * Public Classes:
 * ==============
 * WPFClient1: 
 * ->Defines the above mentioned functions. 
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.IO;

namespace RemoteTestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFClient1 : Window
    {        

        public Comm<WPFClient1> comm { get; set; } = new Comm<WPFClient1>();

        public string endPoint { get; } = Comm<WPFClient1>.makeEndPoint("http://localhost", 8080, "WPFClient1");

        private Thread rcvThread = null;
        delegate void NewMessage(string msg);
        event NewMessage RecvNewMessage;
        event NewMessage SentNewMessage;

        public WPFClient1()
        {
            InitializeComponent();
            RecvNewMessage += new NewMessage(RecvNewMessageHandler);
            SentNewMessage += new NewMessage(SentNewMessageHandler);

            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }

        void RecvNewMessageHandler(string msg)
        {
            listBox2.Items.Insert(0, msg);
        }

        void SentNewMessageHandler(string msg)
        {
            listBox1.Items.Insert(0, msg);
        }
        //----< initialize receiver >------------------------------------

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
                string body = "Logs Received: \n";
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                if (msg.from == "http://localhost:8080/TestHarnessServer")
                    msg.showMsg();
                else
                {
                    msg.showMsg();
                    if (msg.body.FromXml<string[]>() != null)
                    {   
                        int count = msg.body.FromXml<string[]>().Count();
                        if (count > 0)
                            Console.WriteLine("Logs can be found at: WPFClient1//TestLogs//");
                        foreach (string a in msg.body.FromXml<string[]>())
                        {                            
                            body += System.IO.Path.GetFileName(System.IO.Path.GetFullPath(a)) + "\n";
                        }
                        msg.body = body;
                    }                
                }
                                    
                // call window functions on UI thread
                this.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  RecvNewMessage,
                  "FROM: "+msg.from+"\n"+msg.body);

                if (msg.body == "quit")
                    break;
            }
        }

        //----< run client demo >----------------------------------------
        private void RetrieveLogsButton_Click(object sender, RoutedEventArgs e)
        {
            string author = "Nikhil Prashar";
            string remoteEndPoint = Comm<WPFClient1>.makeEndPoint("http://localhost", 8080, "RepositoryServer");
            Message msg = makeMessage(author, endPoint, remoteEndPoint, "RetrieveAllLogs");
            msg.body = "Retrieve all logs";
            Console.WriteLine("\nClient sending message to Repository: {0}", msg.ToString());
            comm.sndr.PostMessage(msg);
        }

        public bool SendFile(IFileService service, string file)
        {
            long blockSize = 512;
            try
            {
                Console.WriteLine("-----" + service);
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

        static IFileService CreateChannelForFileTransfer(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<IFileService> factory = new ChannelFactory<IFileService>(binding, address);
            return factory.CreateChannel();
        }

        //----< send message to connected listener >---------------------
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string author1 = "Nikhil Prashar";
            string remoteEndPoint = Comm<WPFClient1>.makeEndPoint("http://localhost", 8080, "TestHarnessServer");
            Message msg = makeMessage(author1, endPoint, remoteEndPoint, "TestRequest");
            msg.body = MessageTest.makeTestRequest(author1);
            Parser p = new Parser();

            TestRequest tr=p.parse(msg.body);
            ClientFileManager cfm = new ClientFileManager();
            List<string> dlls=cfm.RetrieveFilesFromRepo(tr);

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

            foreach (string file in dlls)
            {
                string filename = System.IO.Path.GetFileName(file);
                Console.Write("\n  sending file {0}", filename);

                if (!SendFile(fs, file))
                    Console.Write("\n  could not send file");
            }
          
            
            Console.WriteLine("\nClient sending message: {0}", msg.ToString());
            comm.sndr.PostMessage(msg);

            this.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  SentNewMessage,
                  msg.body);
        }
    }
}
