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



namespace Firebase_Connection
{

    public partial class Form1 : Form
    {
        public int counter;
        FileSystemWatcher watcher = new FileSystemWatcher();
        public Form1()
        {
            InitializeComponent();
            // CreateJsonFromCSV();
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
            //[ ] if file  changed:
            //[ ]   convert file to json
            //[ ]   send to firebase
            //[ ]   delete prior fire base file? decide on program flow
        }

        
        //   File watcher magic
        public void CreateFileWatcher(string path)
        {
            
            watcher.Path = path;
            watcher.NotifyFilter =  NotifyFilters.LastWrite;
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
            CreateJsonFromCSV();
            System.Threading.Thread.Sleep(2000);
            watcher.EnableRaisingEvents = true;
        }

        /*
         * https://mtdash01.firebaseio.com/

        {
            "rules": {
            ".read": "auth != null",
            ".write": "auth != null"
            }
        }
        */

        public void authFireBase()
        {
           
        }



        public static void CreateJsonFromCSV()
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

            //MARK: - TODO Consider sorting Date so its consecutive on mutiple loads - remove duplicate times

            //MARK: - TODO Convert to JSON

            //MARK: - TODO Upload to Firebase

            //MARK: - TODO Universal filepath to documents

        }

        private async void theTimer()
        {
            Console.WriteLine("Timer Start");
            await Task.Delay(3000);
            Console.WriteLine("Timer End");

        }
    }
}
