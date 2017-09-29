using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Firebase.Database;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json.Linq;

//  inspiration for this firebase app
//  fire sharp unnofficial google lib https://github.com/ziyasal/FireSharp
//  dina cruz blog http://blog.diniscruz.com/2014/03/c-example-of-using-firebase-rest-api.html
//  firbase in C# https://stackoverflow.com/questions/40953382/firebase-in-c-sharp-api-recommendation

namespace Firebase_Connection
{

    public partial class Form1 : Form
    {
        public int counter;
        FileSystemWatcher watcher = new FileSystemWatcher();
        public string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string publicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Firebase";
        public string publicPathOut = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Firebase\PriceData_Out.csv";
        public string update = "No Update Yet";

        public Form1()
        {
            InitializeComponent();

            //MARK: - TODO - rename static path vars
            //MARK: - TODO - last file time to UI
            lastUpdatelabel.Text = update;

            string dirName = Task.Run(async () => { return CheckForDirectory(); }).Result;

            try
            {
                CreateFileWatcher(publicPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "File Watcher Error");
                Console.WriteLine("File Watcher Error ", ex);
            }
        }

        private string CheckForDirectory()
        {
            Console.WriteLine("\nChecking for directory");
            /// check to see if Firebase Dir exists
            bool folderExists = Directory.Exists(systemPath + @"\Firebase");
                Console.WriteLine("path to documents: " + systemPath + " Does Firebase folder exists? " + folderExists);

                /// if not create the directory
                if (!folderExists)
                {
                    Console.WriteLine("creating directory... " + systemPath + @"\Firebase");
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Firebase"));
                }
                else
                {
                    Console.WriteLine("found diretory... " + systemPath + @"\Firebase");
                }

                dirNameLable.Text = systemPath + @"\Firebase";
                Console.WriteLine("Directory check finished");
                return systemPath + @"\Firebase";
        }

        public string deleteFirebase()
        {
            try
            {
                Console.WriteLine("\nStart Delete");
                var myFirebase = "https://mtdash01.firebaseio.com/.json";
                var request = WebRequest.CreateHttp(myFirebase);
                request.Method = "DELETE";
                request.ContentType = "application/json";
                var response = request.GetResponse();
                //System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Finish Delete");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "File Delete Error");
                Console.WriteLine("File Delete Error ", ex);
            }
            return "Firebase Deleted";

        }

        //MARK: - File Watcher
        public void CreateFileWatcher(string path)
        {
            Console.WriteLine("\nCreateing FileWatcher for... " + publicPath);
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string delFirebase = Task.Run(async () => { return deleteFirebase(); }).Result;
            watcher.EnableRaisingEvents = false;
            counter = counter + 1;
            Console.WriteLine("File has changed " + counter + " times");
            Console.WriteLine("Removing duplicates...");
            string removeDup = Task.Run(async () => { return RemoveDuplicatesJsonFromCSV(path: publicPath); }).Result;
    System.Threading.Thread.Sleep(500);
            Console.WriteLine("serialize datatable...");

           // var dataSet = serializeDataTable(path: publicPathOut);
            string dataSet = Task.Run(async () => { return serializeDataTable(path: publicPathOut); }).Result;
            //Console.WriteLine("posting to firebase...");
    System.Threading.Thread.Sleep(500);
            postToFireBase(jsonDataset: dataSet);
            //string postFireB = Task.Run(async () => { return postToFireBase(jsonDataset: dataSet); }).Result;
            watcher.EnableRaisingEvents = true;
        }

        // had file delete error and post to firebase
        public string RemoveDuplicatesJsonFromCSV(string path)
        {
            //System.Threading.Thread.Sleep(2000);
            Console.WriteLine("\nRemoving duplicates");
            try
            {
                var sr = new StreamReader(File.OpenRead(path + @"\PriceData.csv"));
                System.Threading.Thread.Sleep(500);
                var sw = new StreamWriter(File.OpenWrite(path + @"\PriceData_Out.csv"));
                var lines = new HashSet<int>();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int hc = line.GetHashCode();
                    if (lines.Contains(hc))
                        continue;

                    lines.Add(hc);
                    sw.WriteLine(line);
                }
                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "File Read / Write Error\nRemoveDuplicatesJsonFromCSV");
                Console.WriteLine("\nRemove Duplicates Error ", ex);
            }
            Console.WriteLine("Duplicates Removed");
            return "Duplicates removed";
            
        }

        //MARK: -  Upload to Firebase
        public void postToFireBase(string jsonDataset)
        {
            try
            {
                
                Console.WriteLine("\nPosting to firebase");
                var myFirebase = "https://mtdash01.firebaseio.com/.json";
                var request = WebRequest.CreateHttp(myFirebase);
                    Console.WriteLine("request");
                request.Method = "POST";
                    Console.WriteLine("POST");
                request.ContentType = "application/json";
                    Console.WriteLine("application/json");
                var buffer = Encoding.UTF8.GetBytes(jsonDataset);
                    Console.WriteLine("buffer");
                request.ContentLength = buffer.Length;
                    Console.WriteLine("ContentLength");
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                Console.WriteLine("GetRequestStream");
         //System.Threading.Thread.Sleep(10000);
         // this is where the problem is - waiting too long to write data to firebase
                var response = request.GetResponse();
                    Console.WriteLine("response");
                var streamResponse = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                Console.WriteLine(response);
                Console.WriteLine(streamResponse);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "File Read / Write Error\nRemoveDuplicatesJsonFromCSV");
                Console.WriteLine("\nPost to Firebase Error\n", ex);
            }
            
            //return "Post completed";
        }

        public string serializeDataTable(string path)
        {
            Console.WriteLine("\nserializeDataTable");
            DataSet dataSet = new DataSet("dataSet");
            dataSet.Namespace = "NetFrameWork";
            DataTable table = new DataTable();

            DataColumn itemColumn0 = new DataColumn("date");
            table.Columns.Add(itemColumn0);
            
            DataColumn itemColumn1 = new DataColumn("open");
            table.Columns.Add(itemColumn1);

            DataColumn itemColumn2 = new DataColumn("high");
            table.Columns.Add(itemColumn2);

            DataColumn itemColumn3 = new DataColumn("low");
            table.Columns.Add(itemColumn3);

            DataColumn itemColumn4 = new DataColumn("close");
            table.Columns.Add(itemColumn4);

            DataColumn itemColumn5 = new DataColumn("signal");
            table.Columns.Add(itemColumn5);

            dataSet.Tables.Add(table);

            var fullFile = System.IO.File.ReadAllLines(path);

            foreach (string row in fullFile)
            {
                //Console.WriteLine(row);
                string[] words = row.Split(',');

                DataRow newRow = table.NewRow();
                newRow["date"] = words[0];
                newRow["open"] = Convert.ToDouble(words[1]);
                newRow["high"] = Convert.ToDouble(words[2]);
                newRow["low"] = Convert.ToDouble(words[3]);
                newRow["close"] = Convert.ToDouble(words[4]);
                newRow["signal"] = "signal";
                table.Rows.Add(newRow);
            }

            dataSet.AcceptChanges();
            
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }
    }
}

/*
 * https://mtdash01.firebaseio.com/
changed to no auth from:
{
    "rules": {
    ".read": "auth != null",
    ".write": "auth != null"
    }
}
To publish to firebase like this without auth, change your database > rules in firebase to  
{
  "rules": {
    ".read": "auth == null",
    ".write": "auth == null"
    }
}﻿

 To publish without Auth and read in iOS with Auth
{
  "rules": {
            ".read": true,
            ".write": "auth == null"
            }
}
*/
