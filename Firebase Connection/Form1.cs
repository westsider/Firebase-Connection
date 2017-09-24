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
using Newtonsoft.Json.Linq;

namespace Firebase_Connection
{

    public partial class Form1 : Form
    {
        public int counter;
        FileSystemWatcher watcher = new FileSystemWatcher();
        public Form1()
        {
            InitializeComponent();

            try
            {
                string dirName = @"C:\Users\MBPtrader\Documents\FireBase";
                CreateFileWatcher(dirName);
                //CreateJsonFromCSV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "File Import Error");
                Console.WriteLine("Error ", ex);
            }
        }

        //MARK: - File Watcher
        public void CreateFileWatcher(string path)
        {

            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            counter = counter + 1;
            Console.WriteLine("File has changed " + counter + " times");
            //MessageBox.Show("File Change");
            
           // System.Threading.Thread.Sleep(2000);
            RemoveDuplicatesJsonFromCSV();
            System.Threading.Thread.Sleep(2000);
            var jsonArray = JsonArrayfromCsv();

            System.Threading.Thread.Sleep(2000);
            postToFireBase(jsonArray: jsonArray);

            watcher.EnableRaisingEvents = true;
        }

        public static void RemoveDuplicatesJsonFromCSV()
        {
            // Not reading csv file
            string path = @"C:\Users\MBPtrader\Documents\FireBase\PriceData.csv";
            //Read the csv file, and then use System.IO.File.ReadAllLines to read the JSON String format for each line 

            System.Threading.Thread.Sleep(2000);

            // remove duplicates
            var sr = new StreamReader(File.OpenRead(path));
            var sw = new StreamWriter(File.OpenWrite(@"C:\Users\MBPtrader\Documents\FireBase\PriceData_Out.csv"));
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

        //MARK: - TODO read edited csv and make json array
        public static JArray JsonArrayfromCsv()
        {
            JArray array = new JArray();

            System.Threading.Thread.Sleep(1000);
            string path = @"C:\Users\MBPtrader\Documents\FireBase\PriceData_Out.csv";

            var fullFile = System.IO.File.ReadAllLines(path);

            foreach (string row in fullFile)
            {
                Console.WriteLine(row);
                string[] words = row.Split(',');

                var json = rowToJson(date: words[0], open: Convert.ToDouble(words[1]), high: Convert.ToDouble(words[2]),
                    low: Convert.ToDouble(words[3]), close: Convert.ToDouble(words[4]));
   
                // next make a json array
                array.Add(json);
            }
            Console.WriteLine(array);
            return array;
        }

        //MARK: -  Convert to JSON
        public static string rowToJson(string date, double open, double high, double low, double close)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Date = date,
                Open = open,
                High = high,
                Low = low,
                Close = close
            });

            return json;
        }

        private void createJsonArray()
        {
            JArray array = new JArray();
            array.Add("Manual text");
        }

        //MARK: -  Upload to Firebase
        public static void postToFireBase(JArray jsonArray)
        {
            // TODO: Convert jArray into something I can post to firebase!

            var json = rowToJson(date: "", open: 100, high: 200, low: 50, close: 150);

            var myFirebase = "https://mtdash01.firebaseio.com/.json";

            var request = WebRequest.CreateHttp(myFirebase);

            request.Method = "POST";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
        }

        private async void theTimer()
        {
            Console.WriteLine("Timer Start");
            await Task.Delay(3000);
            Console.WriteLine("Timer End");
        }
        //MARK: - TODO Consider sorting Date so its consecutive on mutiple loads - remove duplicate times
        //MARK: - TODO Universal filepath to documents
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
