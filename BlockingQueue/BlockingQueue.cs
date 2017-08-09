/////////////////////////////////////////////////////////////////////////////
//  BlockingQueue.cs - demonstrate threads communicating via Queue         //
//  ver 4.0                                                                //
//  Language:     C#, VS 2003                                              //
//  Platform:     Dell Dimension 8100, Windows 2000 Pro, SP2               //
//  Application:  CSE681 - Software Modeling & Analysis                    //
//                 Project 4                                               //
// Source:      Dr. Jim Fawcett, Syracuse Universisty,                     //
//              jfawcett@twcny.rr.com, (315) 443-3948                      //
// Author:      Nikhil Prashar, Syracuse University,                       //
//              nnprasha@syr.edu, (914) 733-8184                           //
////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module demonstrates communication between two threads using a 
 *   blocking queue.  If the queue is empty when the reader attempts to deQ
 *   an item then the reader will block until the writing thread enQs an item.
 *   Thus waiting is efficient.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.
 * 
 *   Public Interface
 *   ----------------
 *   BlockingQueue<string> bQ = new BlockingQueue<string>();
 *   bQ.enQ(msg);
 *   string msg = bQ.deQ();
 * 
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs
 *   - Compiler command: csc BlockingQueue.cs Program.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 21 November 2016
 *     - first release
 * 
 */

//
using System;
using System.Collections;
using System.Threading;

namespace RemoteTestHarness
{
    public class BlockingQueue<T>
    {
        private Queue blockingQ;
        object locker_ = new object();

        //----< constructor >--------------------------------------------

        public BlockingQueue()
        {
            blockingQ = new Queue();
        }
        //----< enqueue a string >---------------------------------------

        public void enQ(T msg)
        {
            lock (locker_)  // uses Monitor
            {
                blockingQ.Enqueue(msg);
                Monitor.Pulse(locker_);
            }
        }
        //----< dequeue a T >---------------------------------------
        //
        // Note that the entire deQ operation occurs inside lock.
        // You need a Monitor (or condition variable) to do this.

        public T deQ()
        {
            T msg = default(T);
            lock (locker_)
            {
                while (this.size() == 0)
                {
                    Monitor.Wait(locker_);
                }
                msg = (T)blockingQ.Dequeue();
                return msg;
            }
        }
        //
        //----< return number of elements in queue >---------------------

        public int size()
        {
            int count;
            lock (locker_) { count = blockingQ.Count; }
            return count;
        }
        //----< purge elements from queue >------------------------------

        public void clear()
        {
            lock (locker_) { blockingQ.Clear(); }
        }

        //<------------------ Test Stub ----------------->
        static void Main()
        {
            BlockingQueue<String> bq = new BlockingQueue<string>();
            bq.enQ("Hello");
            Console.WriteLine(bq.deQ());
        }
    }
}
