using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostComSip
{
    public partial class ConsoleForm : Form
    {
        //timer upload absensi
        private System.Threading.Timer timerObj = null;
        private DateTime lastUpdateTS = DateTime.MinValue;
        public int refreshInterval { get; set; }
        //end timer

        static bool ubsenMasukBool = false;
        static bool ubsenKeluarBool = false;

        static string host = "localhost";
        static string user = "root";
        static string password = "";
        static string database = "sip_absen";
        static string ssl = "none";

        private static string port = "";
        private static string uni_host = "http://104.196.52.215";
        private static string port_statis = "11960";
        private static string host_port = "/api/absen/port";

        private System.Windows.Forms.ContextMenu contextMenu1;
        public ConsoleForm()
        {
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            InitializeComponent();

            var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uni_host + ":" + port_statis + "" + host_port);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
            string status = string.Empty;
            using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                status = result;
            }
            if (status != string.Empty)
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(status);
                status = o["status"].ToString();

                if (status == "00")
                {
                    port = o["port"].ToString();
                }
                else
                {
                    port = port_statis;
                }
            }
            else
            {
                port = port_statis;
            }

            RefreshData();

            Hide();
            notifyIcon1.Visible = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.programActive = false;
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
            else
            {
                contextMenuStrip1.Close();
            }
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Close();
        }

        private void ConsoleForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }
        public void RefreshData()
        {
            try
            {
                this.timerObj = new System.Threading.Timer(LoadData, null, Timeout.Infinite, Timeout.Infinite);
                this.timerObj.Change(0, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //Log error     
            }
        }
        private void LoadData(object state)
        {
            try
            {
                TimeSpan elapsed = DateTime.Now - this.lastUpdateTS;
                if (elapsed.TotalMinutes >= this.refreshInterval)
                {
                    // Load value from database     
                    RunUploadAbsensi();

                    this.lastUpdateTS = DateTime.Now;
                }

                // 6 seconds interval to call the method again..  
                this.timerObj.Change(1000, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                // log exception    
            }
        }

        static void RunUploadAbsensi()
        {
            if (ubsenMasukBool == false)
            {
                //UploadAbsensiMasuk();
                //MessageBox.Show("absen masuk");
            }
            if (ubsenKeluarBool == false)
            {
                //UploadAbsensiKeluar();
                //MessageBox.Show("absen keluar");
            }

            //System.Threading.Thread.Sleep(1000);
        }

        static async void UploadAbsensiMasuk()
        {
            ubsenMasukBool = true;

            string usernameUrl = "pro6";
            string passwordUrl = "pro6Ci-adminLte";

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd = conn.CreateCommand();
            try

            {
                int i = 1;
                string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu, absen.absen_id, absen.absen_tanggal, absen.absen_masuk, absen.absen_keluar, absen.absen_tanggal";
                string sqldata = "FROM absen LEFT JOIN siswa ON siswa.siswa_nomor_kartu = absen.absen_nomor_kartu where absen.upload_masuk='0';";
                dbcmd.CommandText = sqlselect + " " + sqldata;

                var dt = new DataTable("absen");
                conn.Open();
                var r = dbcmd.ExecuteReader();

                dt.Load(r);

                foreach (DataRow baris in dt.Rows)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://104.196.52.215:" + port + "/api/absen/insert");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", usernameUrl, passwordUrl)));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        DateTime dateValuess = Convert.ToDateTime(baris["absen_tanggal"]);
                        string tanggalabsen = dateValuess.ToString("yyyy-MM-dd");

                        var yourPostedObject = new object();
                        var values = new Dictionary<string, string>
                        {
                             { "nomorMesin", id_sekolah() },
                             { "nomorKartu", baris["siswa_nomor_kartu"].ToString() },
                             { "waktuPresensi", tanggalabsen+" "+baris["absen_masuk"].ToString() }

                        };

                        string json = JsonConvert.SerializeObject(values, Formatting.Indented);

                        //Console.WriteLine(json);

                        string postData = JsonConvert.SerializeObject(values.ToString());

                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = client.PostAsync("http://104.196.52.215:" + port + "/api/absen/insert", content).Result;

                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString());
                        Console.WriteLine("");

                        //Console.WriteLine(id_sekolah()+"/"+ baris["siswa_nomor_kartu"].ToString()+"/"+ tanggalabsen + " " + baris["absen_masuk"].ToString());

                        //Console.WriteLine(postData);

                        Dictionary<string, Object> Items = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseString);
                        foreach (KeyValuePair<string, Object> itm in Items)
                        {
                            //Console.WriteLine(tanggalabsen + " " + baris["absen_masuk"].ToString());
                            if (itm.Key == "sukses")
                            {

                                if (itm.Value.ToString() == "1")
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " sukses");
                                    Console.WriteLine("Masuk");

                                    Update_status_absen_masuk(baris["absen_id"].ToString());
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " gagal");
                                    Console.WriteLine("");
                                }
                            }
                        }

                    }
                }

                r.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Absen Masuk " + ex.Message);
            }


            ubsenMasukBool = false;


        }

        static async void UploadAbsensiMasukOri()
        {
            ubsenMasukBool = true;

            string usernameUrl = "pro6";
            string passwordUrl = "pro6Ci-adminLte";

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd = conn.CreateCommand();
            try

            {

                int i = 1;
                string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu, absen.absen_id, absen.absen_tanggal, absen.absen_masuk, absen.absen_keluar, absen.absen_tanggal";
                string sqldata = "FROM absen LEFT JOIN siswa ON siswa.siswa_nomor_kartu = absen.absen_nomor_kartu where absen.upload_masuk='0';";
                dbcmd.CommandText = sqlselect + " " + sqldata;

                var dt = new DataTable("absen");
                conn.Open();
                var r = dbcmd.ExecuteReader();

                dt.Load(r);

                foreach (DataRow baris in dt.Rows)
                {

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://104.196.52.215:" + port + "/api/absen/insert");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", usernameUrl, passwordUrl)));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        DateTime dateValuess = Convert.ToDateTime(baris["absen_tanggal"]);
                        string tanggalabsen = dateValuess.ToString("yyyy-MM-dd");

                        var yourPostedObject = new object();
                        var values = new Dictionary<string, string>
                        {
                             { "nomorMesin", id_sekolah() },
                             { "nomorKartu", baris["siswa_nomor_kartu"].ToString() },
                             { "waktuPresensi", tanggalabsen+" "+baris["absen_masuk"].ToString() }

                        };

                        string postData = JsonConvert.SerializeObject(values.ToString());

                        var content = new FormUrlEncodedContent(values);
                        var response = await client.PostAsync("http://104.196.52.215:" + port + "/api/absen/insert", content);

                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString());
                        Console.WriteLine("");

                        //Console.WriteLine(id_sekolah()+"/"+ baris["siswa_nomor_kartu"].ToString()+"/"+ tanggalabsen + " " + baris["absen_masuk"].ToString());

                        Console.WriteLine(postData);

                        Dictionary<string, Object> Items = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseString);
                        foreach (KeyValuePair<string, Object> itm in Items)
                        {
                            //Console.WriteLine(tanggalabsen + " " + baris["absen_masuk"].ToString());
                            if (itm.Key == "sukses")
                            {

                                if (itm.Value.ToString() == "1")
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " sukses");
                                    Console.WriteLine("");

                                    Update_status_absen_masuk(baris["absen_id"].ToString());
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " gagal");
                                    Console.WriteLine("");
                                }
                            }
                        }



                    }
                }

                r.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Absen Masuk " + ex.Message);
            }


            ubsenMasukBool = false;


        }

        static async void UploadAbsensiKeluar()
        {
            ubsenKeluarBool = true;
            try

            {
                string usernameUrl = "pro6";
                string passwordUrl = "pro6Ci-adminLte";

                string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
                MySqlConnection conn = new MySqlConnection(connStr);

                int i = 1;
                MySqlCommand dbcmd = conn.CreateCommand();
                string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu, absen.absen_id, absen.absen_tanggal, absen.absen_masuk, absen.absen_keluar, absen.absen_tanggal";
                string sqldata = "FROM absen LEFT JOIN siswa ON siswa.siswa_nomor_kartu = absen.absen_nomor_kartu where upload_masuk='1' and upload_keluar='0' and absen_keluar IS NOT NULL ;";
                dbcmd.CommandText = sqlselect + " " + sqldata;

                var dt = new DataTable("absen");
                conn.Open();
                var r = dbcmd.ExecuteReader();

                dt.Load(r);


                foreach (DataRow baris in dt.Rows)
                {

                    using (HttpClient client = new HttpClient())
                    {

                        client.BaseAddress = new Uri("http://104.196.52.215:" + port + "/api/absen/insert");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", usernameUrl, passwordUrl)));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        DateTime dateValuess = Convert.ToDateTime(baris["absen_tanggal"]);
                        string tanggalabsen = dateValuess.ToString("yyyy-MM-dd");

                        var yourPostedObject = new object();
                        var values = new Dictionary<string, string>
                        {
                             { "nomorMesin", id_sekolah() },
                             { "nomorKartu", baris["siswa_nomor_kartu"].ToString() },
                             { "waktuPresensi", tanggalabsen+" "+baris["absen_keluar"].ToString() }
                        };

                        string json = JsonConvert.SerializeObject(values, Formatting.Indented);

                        //Console.WriteLine(json);

                        string postData = JsonConvert.SerializeObject(values.ToString());

                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = client.PostAsync("http://104.196.52.215:" + port + "/api/absen/insert", content).Result;

                        var responseString = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString());
                        Console.WriteLine("");

                        Dictionary<string, Object> Items = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseString);
                        foreach (KeyValuePair<string, Object> itm in Items)
                        {
                            if (itm.Key == "sukses")
                            {
                                if (itm.Value.ToString() == "1")
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " sukses");
                                    Console.WriteLine("Keluar");

                                    Update_status_absen_keluar(baris["absen_id"].ToString());
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Upload data siswa to server : " + baris["absen_id"].ToString() + " gagal");
                                    Console.WriteLine("");
                                }
                            }
                        }
                    }

                }
                r.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Absen Keluar " + ex.Message);
            }
            ubsenKeluarBool = false;

        }
        static string id_sekolah()
        {

            string id = string.Empty;

            MySqlDataReader reader = null;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_count = conn.CreateCommand();
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlcount = "SELECT COUNT(*) FROM sekolah;";
            dbcmd_count.CommandText = sqlcount;

            string sqlselect = "SELECT sekolah_id, nama_sekolah, alamat FROM sekolah";
            dbcmd_select.CommandText = sqlselect;

            conn.Open();

            dbcmd_count.CommandText = sqlcount;
            Int32 count = Convert.ToInt32(dbcmd_count.ExecuteScalar()); //proses menghitung jumlah data(count)

            reader = dbcmd_select.ExecuteReader();

            if (count > 0)
            {
                while (reader.Read())
                {

                    id = reader.GetString("sekolah_id");

                }
            }
            else
            {
                id = "0";
            }
            reader.Close();
            conn.Close();


            return id;
        }
        static void Update_status_absen_masuk(string absenId)
        {

            string id = string.Empty;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd_update = conn.CreateCommand();
            //MessageBox.Show("man");
            string sqlup = "update absen SET absen_status = 'SUKSES', upload_masuk ='1' where absen_id ='" + absenId + "';";
            dbcmd_update.CommandText = sqlup;
            try
            {
                //conn.Close();
                conn.Open();
                dbcmd_update.ExecuteReader();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static void Update_status_absen_keluar(string absenId)
        {

            string id = string.Empty;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd_update = conn.CreateCommand();
            //MessageBox.Show("man");
            string sqlup = "update absen SET absen_status = 'SUKSES', upload_keluar ='1' where absen_id ='" + absenId + "';";
            dbcmd_update.CommandText = sqlup;
            try
            {
                conn.Open();
                dbcmd_update.ExecuteReader();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
