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

namespace Firebase_Connection
{

    public partial class Form1 : Form
    {
        public int counter;
        FileSystemWatcher watcher = new FileSystemWatcher();
        public string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string publicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Firebase";
        public string update = "No Update Yet";

        public Form1()
        {
            InitializeComponent();

            var dirName = checkForDirectory();

            //MARK: - TODO - dirName to UI
            //MARK: - TODO - last file time to UI
            //lastUpdatelabel.Text = update;

            try
            {
                //string dirName = @"C:\Users\MBPtrader\Documents\FireBase";
                

                CreateFileWatcher(publicPath);
                //CreateJsonFromCSV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "File Import Error");
                Console.WriteLine("Error ", ex);
            }
        }

        public void SetTextboxTextSafe(string result)
        {
            lastUpdatelabel.Text = result.ToString();
        }

        public string checkForDirectory()
        {

            /// check to see if Firebase Dir exists
            bool folderExists = Directory.Exists(systemPath + @"\Firebase");
            Console.WriteLine("\npath to documents: " + systemPath + " Does Firebase folder exists? " + folderExists);

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

            return systemPath + @"\Firebase";
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
            watcher.EnableRaisingEvents = false;
            counter = counter + 1;
            Console.WriteLine("File has changed " + counter + " times");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Removing duplicates...");
            RemoveDuplicatesJsonFromCSV(path: publicPath);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("serialize datatable...");
            var dataSet = serializeDataTable();
            Console.WriteLine("posting to firebase...");
            postToFireBase(jsonDataset: dataSet);
            watcher.EnableRaisingEvents = true;
        }

        public static void RemoveDuplicatesJsonFromCSV(string path)
        {
            // Not reading csv file
            //string path = @"C:\Users\MBPtrader\Documents\FireBase\PriceData.csv";
            //Read the csv file, and then use System.IO.File.ReadAllLines to read the JSON String format for each line 

            System.Threading.Thread.Sleep(2000);

            // remove duplicates
            var sr = new StreamReader(File.OpenRead(path + @"\PriceData.csv"));
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

        //MARK: -  Upload to Firebase
        public static void postToFireBase(string jsonDataset)
        {
            var myFirebase = "https://mtdash01.firebaseio.com/.json";
            var request = WebRequest.CreateHttp(myFirebase);
            request.Method = "POST";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(jsonDataset);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            var streamResponse = (new StreamReader(response.GetResponseStream())).ReadToEnd();
            Console.WriteLine(response);
            Console.WriteLine(streamResponse);
        }

        private async void theTimer()
        {
            Console.WriteLine("Timer Start");
            await Task.Delay(3000);
            Console.WriteLine("Timer End");
        }
        //MARK: - TODO Consider sorting Date so its consecutive on mutiple loads - remove duplicate times
        //MARK: - TODO Universal filepath to documents

        public static string serializeDataTable()
        {
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

            // get csv
   string path = @"C:\Users\MBPtrader\Documents\FireBase\PriceData_Out.csv";

            var fullFile = System.IO.File.ReadAllLines(path);

            foreach (string row in fullFile)
            {
                Console.WriteLine(row);
                string[] words = row.Split(',');

                DataRow newRow = table.NewRow();
                newRow["date"] = words[0];
                newRow["open"] = Convert.ToDouble(words[1]);
                newRow["high"] = Convert.ToDouble(words[2]);
                newRow["low"] = Convert.ToDouble(words[3]);
                newRow["close"] = Convert.ToDouble(words[4]);
                newRow["signal"] = "signal";
                table.Rows.Add(newRow);

                //SetTextboxTextSafe(result: words[0]);
            }

            dataSet.AcceptChanges();
            
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            
            Console.WriteLine(json);
            
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
*/
