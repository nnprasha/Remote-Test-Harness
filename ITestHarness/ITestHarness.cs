/////////////////////////////////////////////////////////////////////
// ITestHarness.cs - Test HArness Interface                        //
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
 *  Defines an interface for the Test Harness and Test Driver ITest Interface. 
 *  
 *   Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteTestHarness
{
    public interface ILoader
    {
        List<Logger> LoaderEntryPoint();
    }
    public interface ITest
    {
        bool test();
    }
}
