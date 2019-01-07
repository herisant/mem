using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace SIP
{

    public partial class FormLoading : Form
    {
        protected string pathDB = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\myconfig.ini";
        protected string _koneksi = "Server={0};Database={1};User Id={2};Password={3};";
        protected string pathHost = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\HostComSip.exe";
        protected FirewallSip _security = new FirewallSip();

        string host = "localhost";
        string user = "root";
        string password = "";
        string database = "sip_absen";
        string ssl = "none";

        protected bool index_flag { get; set; }
        public FormLoading()
        {
            InitializeComponent();
            index_flag = true;

            pilih();
        }

        private void pilih()
        {
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd = conn.CreateCommand();
            string sqlcount = "SELECT COUNT(*) FROM sekolah";

            conn.Open();

            dbcmd.CommandText = sqlcount;

            Int32 count = Convert.ToInt32(dbcmd.ExecuteScalar()); //proses menghitung jumlah data(count)

            //MessageBox.Show(count.ToString());

            conn.Close();

            //System.Diagnostics.Process.Start(System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\HostComSip.exe");

            if (count==0)
            {
                btnConnect_Click();
            }
            else if (count==1)
            {
                //test();
                updateMesin();
                //btnConnect_Click();
            }


            //tsSIP_Click();
        }

        /*private void tsSIP_Click()
        {
            if (IsProcessOpen("HostComSip") == false)
            {
                Process.Start(pathHost);
            }
            else
            {
                MessageBox.Show("PROGRAM SUDAH RUNNING", "INFO SIP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool IsProcessOpen(string Name)
        {
            string name = Name;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.ToLower().Contains(name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }*/

        protected void progress_Bar()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 1000;

            while (index_flag)
            {
                progressBar1.Value++;
                System.Threading.Thread.Sleep(20);

                if (index_flag == false)
                    break;
            }
        }


        private void cek_konek_database()
        {
            using (SqlConnection sql = new SqlConnection(Koneksi()))
            {
                try
                {
                    bgWorker.RunWorkerAsync();
                    sql.Open();
                    index_flag = false;

                    this.Hide();
                    FormSIP frm = new FormSIP(true);
                    frm.ShowDialog();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Hide();
                    FormSIP frm = new FormSIP(false);
                    frm.ShowDialog();
                }
            }


        }


        private void test()
        {
            string idmesin;
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT sekolah.sekolah_id from sekolah";
            dbcmd_select.CommandText = sqlselect;

            conn.Open();

            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();

                    idmesin = r[0].ToString();

                    string STATUS = "1";
                    string url = "http://localhost/SIP/test-mesin.php?STATUS="+STATUS+ "&IDMESIN="+idmesin+" ";
            
                    try
                    {
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                        myRequest.Method = "GET";
                        WebResponse myResponse = myRequest.GetResponse();
                        StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                        string result = sr.ReadToEnd();
                        //Console.WriteLine(result);
                        result = result.Replace('\n', ' ');
                        sr.Close();
                        myResponse.Close();

                        MessageBox.Show(result);

                        this.Hide();
                        FormSIP frm = new FormSIP(true);
                        frm.ShowDialog();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                r.Close();
            }
            conn.Close();

        }

        private void btnConnect_Click()
        {
            string host = "localhost";
            string user = "root";
            string password = "";
            string database = "sip_absen";
            string ssl = "none";
           // string persis = True;
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
           
            try
            {
                conn.Open();
                //MessageBox.Show("Koneksi berhasil");
                conn.Close();
                this.Hide();
                FormSIP frm = new FormSIP(true);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //MessageBox.Show("Koneksi berhasil");
        }

        protected string Koneksi()
        {
            string server = string.Empty;
            string database = string.Empty;
            string user = string.Empty;
            string password = string.Empty;

            List<string> element = new List<string>();
            var list = new List<string>();
            var fileStream = new FileStream(pathDB, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    element.Add(line);
                }
            }

            server = _security.Decrypt(element[0], true);
            database = _security.Decrypt(element[1], true);
            user = _security.Decrypt(element[2], true);
            password = _security.Decrypt(element[3], true);

            return string.Format(_koneksi, server, database, user, password);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            progress_Bar();
        }


        private async void updateMesin()
        {
            try

            {
                string usernameUrl = "pro6";
                string passwordUrl = "pro6Ci-adminLte";

                string host = "localhost";
                string user = "root";
                string password = "";
                string database = "sip_absen";
                string ssl = "none";

                string idmesin;
                string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand dbcmd_select = conn.CreateCommand();

                string sqlselect = "SELECT sekolah.sekolah_id from sekolah";
                dbcmd_select.CommandText = sqlselect;

                conn.Open();

                var r = dbcmd_select.ExecuteReader();

                if (r.HasRows)
                {
                    r.Read();

                    idmesin = r[0].ToString();

                    string STATUS = "1";

                    using (HttpClient client = new HttpClient())
                    {

                        client.BaseAddress = new Uri("http://104.196.52.215/apps/main/index.php/Service/mesinPostStat");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", usernameUrl, passwordUrl)));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        var yourPostedObject = new object();
                        var values = new Dictionary<string, string>
                                {
                                        { "mesin", idmesin },
                                        { "status", STATUS }
                                };

                        var content = new FormUrlEncodedContent(values);
                        var response = await client.PostAsync("http://104.196.52.215/apps/main/index.php/Service/mesinPostStat", content);

                        var responseString = await response.Content.ReadAsStringAsync();
                        //MessageBox.Show(responseString);

                        this.Hide();
                        FormSIP frm = new FormSIP(true);
                        frm.ShowDialog();
                    }

                    r.Close();
                }
                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void FormLoading_Load(object sender, EventArgs e)
        {

        }
    }

}
