using System;
using System.IO;
using System.Configuration;
using System.Diagnostics;


namespace BradyETRM
{
    internal class Program
    {
        static void Main()
        {
            //Fetching the keys(folder paths) from the app.config file
            string inputFolder = ConfigurationManager.AppSettings["InputFolder"];
            string outputFolder = ConfigurationManager.AppSettings["OutputFolder"];
            string referenceDataPath = ConfigurationManager.AppSettings["ReferenceDataPath"];

            //Creating a watcher to monitor the files creation
            FileSystemWatcher watcher = new FileSystemWatcher(inputFolder, "*.xml");
            watcher.Created += (s, e) =>
            {
                IFileProcessor processor = new XmlFileProcessor(new DataProcessor(referenceDataPath), new XmlGenerator());
                processor.ProcessFile(e.FullPath, outputFolder);
            };
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Monitoring input folder. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
