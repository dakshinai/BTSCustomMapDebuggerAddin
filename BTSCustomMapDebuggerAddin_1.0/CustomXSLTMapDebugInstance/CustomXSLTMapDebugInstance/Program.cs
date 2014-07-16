using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CustomXSLTMapDebugInstance
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Map Debugging has started..");

            string input = @"..\..\Source.xml";
            string stylesheet = @"..\..\Map.xslt";
            string extension_object = @"..\..\ExtentionObject.xml";

            string result = DebugHelper.TransformXslt(input, stylesheet, extension_object);

            File.WriteAllText(@"..\..\Destination.xml", result);

            Console.WriteLine("Map Debugging has ended");

        }
    }
}
