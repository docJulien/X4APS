using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using BusinessLogic.Models;
using Newtonsoft.Json;

namespace BusinessLogic
{
    public static class ImportLog
    {
        private static List<TradeOperation> tradeOperations = new List<TradeOperation>();
        
        public static void InputDialog(string filePath)
        {
            if (filePath == null || "".Equals(filePath))
            {
                return;
            }
            string path = Environment.ExpandEnvironmentVariables(filePath);
            
            var p = new Process();
            p.Configurations.Where(x => x.Key.Equals("LastSaveGameLoaded")).FirstOrDefault().Value = path;

            p.SaveConfigurations();

            //todo logbuilder interface Console.WriteLine(@path);
            FileInfo compressedSaveFile = new FileInfo(path);
            Decompress(compressedSaveFile, p);
        }

        private static void Decompress(FileInfo fileToDecompress, Process p)
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(applicationPath).Remove(0, 6);

            p.DeserializeWares();

            var originalSaveFileUsed = fileToDecompress.Extension.ToLower().EndsWith("xml");
            if (originalSaveFileUsed)
                newFileName = fileToDecompress.FullName;
            else
            {
                newFileName = directory + @"\X4LogAnalyzerTempXML.xml";
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    NewMethod(fileToDecompress, newFileName, originalFileStream);

                }
            }

            //todo performance todo #8
            //try like this:
            //XmlDocument doc = new XmlDocument();
            //doc.Load("booksort.xml");

            //XmlNodeList nodeList;
            //XmlNode root = doc.DocumentElement;

            //nodeList = root.SelectNodes("descendant::book[author/last-name='Austen']");

            ////Change the price on the books.
            //foreach (XmlNode book in nodeList)
            //{
            //    book.LastChild.InnerText = "15.95";
            //}

            //Console.WriteLine("Display the modified XML document....");
            //doc.Save(Console.Out);


            //Load xml
            XDocument xdoc = XDocument.Load(newFileName);
            // <log><entry time="50395.876" category="upkeep"
            // title="Trade Completed"
            // text="Magpie MCY-890 sold 350 Silicon to TEL Silicon Refinery I VUU-215 in Eighteen Billion for 52493 Cr."
            // faction="{20203,601}"
            // money="5249300"/>

            //</root>
            //Run query
            double startFrom = 0;
            p.GlobalTradeOperations = (from entry in xdoc.Descendants("log").Descendants("entry")
                where entry.Attribute("title").Value == "Trade Completed"
                    && double.Parse(entry.Attribute("time").Value) > startFrom
                         select new TradeOperation(entry.Attribute("text").Value) {
                             Time = double.Parse(entry.Attribute("time").Value),
                             //Faction = entry.Attribute("faction").Value,
                             Money = int.Parse(entry.Attribute("money")?.Value ?? "0")}
                ).ToList();
            
            //todo use db and remove this
            using (StreamWriter file = File.CreateText(directory + @"\X4LogAnalyzerTempXML.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, p.GlobalTradeOperations);
                file.Close();
            }

            //todo cleanup decompressedSaveFile.Close();
            if (!originalSaveFileUsed)
            {
                Thread newThread = new Thread(Delete);
                newThread.IsBackground = true;
                newThread.Start(new FileInfo(newFileName));
            }
        }

        private static void NewMethod(FileInfo fileToDecompress, string newFileName, FileStream originalFileStream)
        {
            using (FileStream decompressedFileStream = File.Create(@newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                    Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    decompressionStream.Close();
                };

                decompressedFileStream.Flush();
                decompressedFileStream.Close();
            }
        }

        private static void Delete(object file)
        {
            FileInfo fileInfo = (FileInfo)file;
            if (fileInfo.Exists)
            {
                int Attempt = 0;
                bool ShouldStop = false;
                while (!ShouldStop)
                {
                    if (CanDelete(fileInfo))
                    {
                        fileInfo.Delete();
                        ShouldStop = true;
                    }
                    else if (Attempt >= 3)
                    {
                        ShouldStop = true;
                    }
                    else
                    {
                        // wait one sec
                        System.Threading.Thread.Sleep(1000);
                    }

                    Attempt++;
                }
            }
        }

        private static bool CanDelete(FileInfo file)
        {
            try
            {
                //Just opening the file as open/create
                using (FileStream fs = new FileStream(file.FullName, FileMode.OpenOrCreate))
                {
                    //If required we can check for read/write by using fs.CanRead or fs.CanWrite
                    fs.Close();
                    return true;
                }
                //return false;
            }
            catch (IOException ex)
            {
                //check if message is for a File IO
                string __message = ex.Message.ToString();
                if (__message.Contains("The process cannot access the file"))
                    return false;
                else
                    throw;
            }
        }

    }
}
