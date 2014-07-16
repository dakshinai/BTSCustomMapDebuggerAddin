using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BTSCustomMapDebugEngine
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                if (args.Length == 5)
                {
                    string input = args[0];
                    string stylesheet = args[1];
                    string extension_object = args[2];
                    string save_location = args[3];
                    string save_fileName = args[4];

                    Thread.Sleep(5000);
                    if (!Debugger.IsAttached)
                        Debugger.Launch();

                    string result = DebugHelper.TransformXslt(input, stylesheet, extension_object);

                    Directory.CreateDirectory(save_location);
                    File.WriteAllText(save_location + "\\" + save_fileName, result);
                    Console.WriteLine("The mapped output has been saved to: "+save_location+"\\"+save_fileName);

                }

            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    Console.WriteLine("Error occured while processing map " + e.InnerException.Message + "; " + e.Message);
                else
                {
                    Console.WriteLine("Error occured while processing map " + e.Message);

                }

            }

        }
    }
}
