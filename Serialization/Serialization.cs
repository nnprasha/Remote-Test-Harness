/////////////////////////////////////////////////////////////////////
// Serialization.cs                                                //
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
 * Perform opeartions such as converting a request to xml and retrieving it from the xml format.
 * 
 * Pulic Interfaces:
 * ===================
 * ToXML()
 * FromXML()
 * 
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RemoteTestHarness
{
    public static class Serialization
    {
        //---- < serialize object to XML > --------------------------------

        static public string ToXml(this object obj)
        {
            // suppress namespace attribute in opening tag

            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            var sb = new StringBuilder();
            try
            {
                var serializer = new XmlSerializer(obj.GetType());
                using (StringWriter writer = new StringWriter(sb))
                {
                    serializer.Serialize(writer, obj, nmsp);
                }
            }
            catch (Exception ex)
            {
                Console.Write("\n  exception thrown:");
                Console.Write("\n  {0}", ex.Message);
            }
            return sb.ToString();
        }

        //----< deserialize XML to object >------------------------------
        static public T FromXml<T>(this string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(xml));
            }
            catch (Exception ex)
            {
                Console.Write("\n  deserialization failed\n  {0}", ex.Message);
                return default(T);
            }
        }
        // <----------- Test Stub ---------->
        static void Main()
        {
            string xml="Hi. This is a test message".ToXml();
            Console.WriteLine(xml);

            string backToMsg = xml.FromXml<String>();
            Console.WriteLine(backToMsg);
        }
    }

    public static class extMethods
    {
        public static void showMsg(this Message msg)
        {
            Console.Write("\n  formatted message:");
            string[] lines = msg.ToString().Split(new char[] { ',' });
            foreach (string line in lines)
            {
                Console.Write("\n    {0}", line.Trim());
            }
            Console.WriteLine();
        }
        public static string showThis(this object msg)
        {
            string showStr = "\n  formatted message:";
            string[] lines = msg.ToString().Split('\n');
            foreach (string line in lines)
                showStr += "\n    " + line.Trim();
            showStr += "\n";
            return showStr;
        }
        public static string shift(this string str, int n = 2)
        {
            string insertString = new string(' ', n);
            string[] lines = str.Split('\n');
            for (int i = 0; i < lines.Count(); ++i)
            {
                lines[i] = insertString + lines[i];
            }
            string temp = "";
            foreach (string line in lines)
                temp += line + "\n";
            return temp;
        }
        public static string formatXml(this string xml, int n = 2)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString().shift(n);
        }
    }

    public static class Utilities
    {
        public static void title(this string aString, char underline = '-')
        {
            Console.Write("\n  {0}", aString);
            Console.Write("\n {0}", new string(underline, aString.Length + 2));
        }
    }    
}
