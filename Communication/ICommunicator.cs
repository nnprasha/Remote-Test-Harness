/////////////////////////////////////////////////////////////////////
// IComminicator.cs -  Communication Interface                      //
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
 *  Defines an interface for the Communication. 
 *  Implemented by CommService Package. 
 *  
 *   Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RemoteTestHarness
{
    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);
       
        // used only locally so not exposed as service method
        Message GetMessage();
    }

    [ServiceContract(Namespace = "RemoteTestHarness")]
    public interface IFileService
    {
        [OperationContract]
        bool OpenFileForWrite(string name);

        [OperationContract]
        bool WriteFileBlock(byte[] block);

        [OperationContract]
        bool CloseFile();
    }


}
