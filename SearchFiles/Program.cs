using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SearchFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1) Create 500000 samples files. It will take longer time approximately 60 mins & above
            //Files_Processor.CreateSampleFiles();

            // 2) Search the string in sample files
            Files_Processor.Start();
        }
    }
    public class File_Processor
    {        
        private string m_path;
        private string m_string;
        Thread m_Thread;
        public Boolean IsAlive()
        {
            return ((m_Thread != null) && m_Thread.IsAlive);
        }

        public Boolean FinishedLoading()
        {
            return ((m_Thread == null) || m_Thread.Join(10));
        }

        public File_Processor()
        {           
            Console.WriteLine("Enter the valid File Path to search files");
            m_path = Console.ReadLine();
            Console.WriteLine("Type the string to be searched");
            m_string = Console.ReadLine();
            m_Thread = new Thread(new ThreadStart(Load));
            m_Thread.Name = "Background File Processor";
            m_Thread.IsBackground = true;
        }

        public void Start()
        {
            if (m_Thread != null)
                m_Thread.Start();
        }

        public void Stop()
        {
            if ((m_Thread != null) && m_Thread.IsAlive)
                m_Thread.Abort();
        }

        private void Load()
        {
            Regex match = new Regex(m_string);            
            List<string> filenames = new List<string>(Directory.GetFiles(m_path));
            int count = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (string filename in filenames)
            {                
                string fileContent = File.ReadAllText(filename);
                if (match.IsMatch(fileContent))
                    count++;
            }
            sw.Stop();
            
            Console.WriteLine("Search string [" + m_string + "] appears in " + count.ToString() + " place/s");
            Console.WriteLine("Time taken in Milli Seconds: " + sw.ElapsedMilliseconds);                       
            //Files_Processor.Stop();            
            Console.ReadLine(); 
        }
    }

    public static class Files_Processor
    {
        private static List<File_Processor> m_FileProcessors;
        private static string path_m=string.Empty;        
        public static void CreateSampleFiles()
        {
            Console.WriteLine("Enter the valid File Path");
            path_m = Console.ReadLine();
            if (path_m.Length > 0)
            {
                if (path_m.Substring(path_m.Length - 1, 1) != "\\")
                    path_m += "\\";
                for (int i = 1; i <= 500000; i++)
                {                    
                    string path = path_m + i + ".txt";
                    if (!File.Exists(path))
                    {
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            sw.WriteLine("please");
                            sw.WriteLine("search");
                            sw.WriteLine("string");
                        }
                    }
                }
                Console.WriteLine("Sample files created");
            }
           
        }
        public static void Start()
        {
            m_FileProcessors = new List<File_Processor>();
            InstanciateFileProcessor();
            while (!FinishedLoading())
                Application.DoEvents();
        }

        public static void Stop()
        {
            foreach (File_Processor processor in m_FileProcessors)
                processor.Stop();

            m_FileProcessors.Clear();
            m_FileProcessors = null;
        }

        private static Boolean FinishedLoading()
        {
            foreach (File_Processor processor in m_FileProcessors)
            {
                if (processor.IsAlive() && !processor.FinishedLoading())
                    return false;
            }

            return true;
        }

        private static void InstanciateFileProcessor()
        {
            File_Processor processor = new File_Processor();
            processor.Start();
            m_FileProcessors.Add(processor);
        }
    }

}
