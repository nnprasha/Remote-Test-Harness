/////////////////////////////////////////////////////////////////////
// CommService.cs - Communication Module between endpoints          //
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
 * This module defines the communicatio between the different endpoints.
 * Implements the ICommunicator interface and IFileService Interface.
 * 
 * Pulic Interfaces:
 * ===================
 * -> Receiver()
 * ->Client()
 * ->CreateRecvChannel()
 * ->CreateSndrChannel()
 * ->PostMessage()
 * ->GetMessage()
 * 
 * Public Classes: 
 * ==============
 * Receiver<T>
 * Sender
 * Comm<T>
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
using System.Reflection;

namespace RemoteTestHarness
{
    public class Receiver<T> : ICommunicator, IFileService
    {
        static BlockingQueue<Message> rcvBlockingQ = null;
        ServiceHost service = null;
        string tempDirectoryPath = "";
        string fileSpec = "";
        FileStream fs = null;
        public string name { get; set; }

        public Receiver()
        {
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message>();
        }

        public Thread start(ThreadStart rcvThreadProc)
        {
            Thread rcvThread = new Thread(rcvThreadProc);
            rcvThread.Start();
            return rcvThread;
        }

        public void Close()
        {
            service.Close();
        }

        //  Create ServiceHost for Communication service
        public void CreateRecvChannel(string address)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver<T>), baseAddress);
            service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
            service.AddServiceEndpoint(typeof(IFileService), binding, baseAddress);
            service.Open();
            Console.Write("\n  Service is open listening on {0}", address);
        }

        // Implement service method to receive messages from other Peers
        public void PostMessage(Message msg)
        {
            //Console.Write("\n  service enQing message: \"{0}\"", msg.body);
            rcvBlockingQ.enQ(msg);
        }

        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.

        public Message GetMessage()
        {
            Message msg = rcvBlockingQ.deQ();
            //Console.Write("\n  {0} dequeuing message from {1}", name, msg.from);
            return msg;
        }

        public bool OpenFileForWrite(string name)
        {
            Type t = typeof(T);
            if(t.Name == "TestHarnessServer")
            {
                FieldInfo f = t.GetField("tempDirectoryPath", BindingFlags.Public | BindingFlags.Static);
                tempDirectoryPath = f.GetValue(null).ToString();

                if (!Directory.Exists(tempDirectoryPath))
                    Directory.CreateDirectory(tempDirectoryPath);

                fileSpec = tempDirectoryPath + "\\" + name;
            }
            else if(t.Name == "RepositoryServer")
            {
                fileSpec = "../../../Repository/DLLs" + "\\" + name;               
            }
            else
            {
                fileSpec = "../../../"+t.Name+"/LocalRepository/TestLogs" + "\\" + name;
            }

            try
            {
                fs = File.Open(fileSpec, FileMode.Create, FileAccess.Write);
               // Console.Write("\n  {0} opened", fileSpec);
                return true;
            }
            catch
            {
                Console.Write("\n  {0} failed to open", fileSpec);
                return false;
            }
        }

        public bool WriteFileBlock(byte[] block)
        {
            try
            {
               // Console.Write("\n  writing block with {0} bytes", block.Length);
                fs.Write(block, 0, block.Length);
                fs.Flush();
                return true;
            }
            catch { return false; }
        }

        public bool CloseFile()
        {
            try
            {
                fs.Close();
               // Console.Write("\n  {0} closed", fileSpec);
                return true;
            }
            catch { return false; }

        }
    }

    public class Sender
    {
        public string name { get; set; }

        ICommunicator channel;
        string lastError = "";
        BlockingQueue<Message> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;
        string currEndpoint = "";

        //----< processing for send thread >-----------------------------

        void ThreadProc()
        {
            tryCount = 0;
            while (true)
            {
                Message msg = sndBlockingQ.deQ();
                if (msg.to != currEndpoint)
                {
                    currEndpoint = msg.to;
                    CreateSendChannel(currEndpoint);
                }
                while (true)
                {
                    try
                    {
                        channel.PostMessage(msg);
                        Console.Write("\n  posted message from {0} to {1}", name, msg.to);
                        tryCount = 0;
                        break;
                    }
                    catch 
                    {
                        Console.Write("\n  connection failed");
                        if (++tryCount < MaxCount)
                            Thread.Sleep(100);
                        else
                        {
                            Console.Write("\n  {0}", "can't connect\n");
                            currEndpoint = "";
                            tryCount = 0;
                            break;
                        }
                    }
                }
                if (msg.body == "quit")
                    break;
            }
        }

        //----< initialize Sender >--------------------------------------

        public Sender()
        {
            sndBlockingQ = new BlockingQueue<Message>();
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        //----< Create proxy to another Peer's Communicator >------------

        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, address);
            channel = factory.CreateChannel();
            Console.Write("\n  service proxy created for {0}", address);
        }

        //----< posts message to another Peer's queue >------------------
        /*
         *  This is a non-service method that passes message to
         *  send thread for posting to service.
         */
        public void PostMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public string GetLastError()
        {
            string temp = lastError;
            lastError = "";
            return temp;
        }

        //----< closes the send channel >--------------------------------

        public void Close()
        {
            ChannelFactory<ICommunicator> temp = (ChannelFactory<ICommunicator>)channel;
            temp.Close();
        }       
    }

    ///////////////////////////////////////////////////////////////////
    // Comm class simply aggregates a Sender and a Receiver
    //
    public class Comm<T>
    {
        public string name { get; set; } = typeof(T).Name;

        public Receiver<T> rcvr { get; set; } = new Receiver<T>();

        public Sender sndr { get; set; } = new Sender();

        public Comm()
        {
            rcvr.name = name;
            sndr.name = name;
        }
        public static string makeEndPoint(string url, int port, string destn)
        {
            string endPoint = url + ":" + port.ToString() + "/" + destn;
            return endPoint;
        }
        //----< this thrdProc() used only for testing, below >-----------

        public void thrdProc()
        {
            while (true)
            {
                Message msg = rcvr.GetMessage();
                msg.showMsg();
                if (msg.body == "quit")
                {
                    break;
                }
            }
        }

        //<-------------- Test Stub ------------------>
        static void Main()
        {
            
        }
    }
    
}
