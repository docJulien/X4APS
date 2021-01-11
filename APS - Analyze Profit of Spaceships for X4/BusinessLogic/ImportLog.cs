using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml;
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

            FileStream decompressedSaveFile = new FileStream(newFileName, FileMode.Open);

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





            XmlReader xmlSave = XmlReader.Create(decompressedSaveFile);
            while (xmlSave.Read())
            {
                if (xmlSave.NodeType == XmlNodeType.Element)
                {
                    if (xmlSave.Name == "entry" || xmlSave.Name == "entry")
                    {
                        if (xmlSave.HasAttributes)
                        {
                            writeLogEntry(xmlSave, p);
                        }
                    }
                    
                }
            }
            
            foreach (TradeOperation tradeOp in tradeOperations)
            {
                p.GlobalTradeOperations.Add(tradeOp);
            }
            //todo use db and remove this
            using (StreamWriter file = File.CreateText(directory + @"\X4LogAnalyzerTempXML.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, p.GlobalTradeOperations);
                file.Close();
            }

            decompressedSaveFile.Close();
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

        private static void writeLogEntry(XmlReader logEntry, Process p)
        {
            logEntry.MoveToNextAttribute();
            //List<TradeOperation> tradeOperations = new List<TradeOperation>();
            
            bool isATradeOperation = false;
            if ("time".Equals(logEntry.Name))
            {
                if (p.GlobalTradeOperations.Find(x => x.Time == double.Parse(logEntry.Value)) != null)
                {
                    //This item already exists in the list
                    return;
                }
                TradeOperation currentTradeOperation = new TradeOperation(double.Parse(logEntry.Value));
                //Console.WriteLine(string.Format("\t{0} : {1}", logEntry.Name, logEntry.Value));
                while (logEntry.MoveToNextAttribute())
                { //todo serialize to object may be much faster than checking strings here....
                    //Console.WriteLine(string.Format("\t{0} : {1}", logEntry.Name, logEntry.Value));
                    if ("title".Equals(logEntry.Name) && p.Configurations.Where(x => x.Key.Equals("TradeCompletedTranslation")).FirstOrDefault().Value.Equals(logEntry.Value))
                    {
                        isATradeOperation = true;

                    }
                    if (isATradeOperation && "text".Equals(logEntry.Name))
                    {
                        currentTradeOperation = new TradeOperation(logEntry, p);
                    }
                    if (isATradeOperation && "faction".Equals(logEntry.Name))
                    {
                        currentTradeOperation.Faction = logEntry.Value;
                        //Console.WriteLine(string.Format("\t{0} : {1}", "Faction", logEntry.Value));
                    }
                    if (isATradeOperation && "time".Equals(logEntry.Name))
                    {
                        //Console.WriteLine(string.Format("\t{0} : {1}", "Time", logEntry.Value));
                        currentTradeOperation = new TradeOperation(float.Parse(logEntry.Value));
                        tradeOperations.Add(currentTradeOperation);
                    }
                    if (isATradeOperation && "money".Equals(logEntry.Name))
                    {
                        int money = int.Parse(logEntry.Value);
                        money = (money / 100);
                        currentTradeOperation.Money = money;
                        //Console.WriteLine(string.Format("\t{0} : {1}", "money", money));
                    }
                }
                if (isATradeOperation)
                {
                    
                    tradeOperations.Add(currentTradeOperation);
                    //currentTradeOperation.OurShip.AddTradeOperation(currentTradeOperation);
                    //currentTradeOperation.SoldTo.AddTradeOperation(currentTradeOperation);
                    //currentTradeOperation.PartialSumByShip = currentTradeOperation.OurShip.GetListOfTradeOperations().Sum(x => x.Money);

                }
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
