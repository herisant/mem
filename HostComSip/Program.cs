using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Threading;

namespace HostComSip
{
    class Program
    {

        public static bool programActive = true;


        static FirewallSip _security = new FirewallSip();
        static string pathHost = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\WebService\absen.sip";
       // static string pathDB = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\myconfig.ini";
        //static string _koneksi = "Server={0};Database={1};User Id={2};Password={3};";
        static void Main(string[] args)
        {
            //Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " start HostCommSip ..");
            //Console.WriteLine("");

            ConsoleForm cf = new ConsoleForm();
            cf.ShowDialog();

        }
        
        static string ReadURL()
        {
            using (StreamReader sr = new StreamReader(pathHost))
            {
                //Console.WriteLine(_security.Decrypt(sr.ReadToEnd(), true));
                //Console.ReadKey();
                return _security.Decrypt(sr.ReadToEnd(), true);
            }
        }
        
    }
}
