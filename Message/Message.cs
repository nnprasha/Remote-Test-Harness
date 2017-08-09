/////////////////////////////////////////////////////////////////////
// Message.cs -  Message Class                                      //
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
 * This module defines the Message Class that is used by the test harness. 
 *
 * Public Interfaces:
 * ===================
 * Mesage()
 * FromString()
 * ToString()
 * copy()
 * 
 * public classes:
 * ==============
 * Message:
 * ->Defines the functions described above. 
 * 
 *  Maintence History:
 * ==================
 * ver 1.0 : 21 November 2016
 *   - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteTestHarness
{
    [Serializable]
    public class Message
    {
        public string type { get; set; } = "default";
        public string to { get; set; }
        public string from { get; set; }
        public string author { get; set; } = "";
        public DateTime time { get; set; } = DateTime.Now;
        public string body { get; set; } = "none";

        public List<string> messageTypes { get; set; } = new List<string>();

        public Message()
        {
            messageTypes.Add("TestRequest");
            body = "";
        }
        public Message(string bodyStr)
        {
            messageTypes.Add("TestRequest");
            body = bodyStr;
        }
        public Message fromString(string msgStr)
        {
            Message msg = new Message();
            try
            {
                string[] parts = msgStr.Split(',');
                for (int i = 0; i < parts.Count(); ++i)
                    parts[i] = parts[i].Trim();

                msg.type = parts[0].Substring(6);
                msg.to = parts[1].Substring(4);
                msg.from = parts[2].Substring(6);
                msg.author = parts[3].Substring(8);
                msg.time = DateTime.Parse(parts[4].Substring(6));
                msg.body = parts[5].Substring(6);
            }
            catch
            {
                Console.Write("\n  string parsing failed in Message.fromString(string)");
                return null;
            }
            //XDocument doc = XDocument.Parse(body);
            return msg;
        }
        public override string ToString()
        {
            string temp = "\n\ttype: " + type;
            temp += "\n\tto: " + to;
            temp += "\n\tfrom: " + from;
            if (author != "")
                temp += "\n\tauthor: " + author;
            temp += "\n\ttime: " + time;
            temp += "\n\tbody:\n" + body;
            return temp;
        }
        public Message copy()
        {
            Message temp = new Message();
            temp.type = type;
            temp.to = to;
            temp.from = from;
            temp.author = author;
            temp.time = DateTime.Now;
            temp.body = body;
            return temp;
        }

        //<----------- Test Stub ----------------->
        static void Main()
        {
            Message m = new Message();
            m.type = "test";
            m.author = "nikhil";
            m.to = "abc";
            m.from = "def";
            m.body = "Hiii.";
            Console.WriteLine(m.ToString());
        }
    }
}
