using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public static class ImportLog
    {
        public static List<TradeOperation> InputDialog(string filePath, List<Configuration> configurations, double startTime)
        {
            if (filePath == null || "".Equals(filePath))
            {
                return null;
            }
            string path = Environment.ExpandEnvironmentVariables(filePath);
            
            configurations.Where(x => x.Key.Equals("LastSaveGameLoaded")).FirstOrDefault().Value = path;
            
            FileInfo compressedSaveFile = new FileInfo(path);
            return Decompress(compressedSaveFile, startTime);
        }

        private static List<TradeOperation> Decompress(FileInfo fileToDecompress, double startTime)
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(applicationPath).Remove(0, 6);
            
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

            //Load xml
            XDocument xdoc = XDocument.Load(newFileName);
            // <log><entry time="50395.876" category="upkeep"
            // title="Trade Completed"
            // text="Magpie MCY-890 sold 350 Silicon to TEL Silicon Refinery I VUU-215 in Eighteen Billion for 52493 Cr."
            // faction="{20203,601}"
            // money="5249300"/>

            //</root>
            //Run query
            var tradeOperations = (from entry in xdoc.Descendants("log").Descendants("entry")
                where entry.Attribute("title").Value == "Trade Completed"
                    && double.Parse(entry.Attribute("time").Value) > startTime
                        select new TradeOperation(entry.Attribute("text").Value) {
                             Time = double.Parse(entry.Attribute("time").Value),
                             //Faction = entry.Attribute("faction").Value,
                             Money = int.Parse(entry.Attribute("money")?.Value ?? "0")}
                ).ToList();
            
            if (!originalSaveFileUsed)
            {
                Thread newThread = new Thread(Delete);
                newThread.IsBackground = true;
                newThread.Start(new FileInfo(newFileName));
            }

            return tradeOperations;
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
