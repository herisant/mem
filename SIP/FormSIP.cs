using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UsbHidKeyboard;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Net;

namespace SIP
{
    public partial class FormSIP : Form
    {
        protected string pathDB = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\myconfig.ini";
        protected string pathSQL = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\Installer\SQLEXPRWT_x86_ENU.exe";
        protected string pathPDF = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\Panduan Instal SQL\PANDUAN INSTAL SQL SERVER.pdf";
        protected string pathSIP = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\Panduan SIP\CARA PENGGUNAAN APLIKASI SIP.pdf";
        protected string pathHost = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\HostComSip.exe";
        protected string DatabaseName = "sip_absen";
        protected FirewallSip _security = new FirewallSip();
        //running text
        private int xpos = 0, ypos = 0;
        public string mode = "Right-to-Left";
        #region VARIABLE
        RawInput _rawInput = null;
        const bool CaptureOnlyInForeground = false;
        string keyData1 = "", keyData2 = "", keyData3 = "", keyData4 = "", keyData5 = "", keyData6 = "";
        string keySource1 = "", keySource2 = "", keySource3 = "", keySource4 = "", keySource5 = "", keySource6 = "";

        public string device_reader_1 { get; set; }
        public string device_reader_2 { get; set; }
        public string device_reader_3 { get; set; }
        public string device_reader_4 { get; set; }
        public string device_reader_5 { get; set; }
        public string device_reader_6 { get; set; }
        #endregion
        string _koneksi = "Server={0};Database={1};User Id={2};Password={3};";

        string host = "localhost";
        string user = "root";
        string password = "";
        string database = "sip_absen";
        string ssl = "none";

        protected bool validasi { get; set; }
        public FormSIP(bool msg)
        {
            InitializeComponent();
            downloadFile();

            validasi = msg;

            //running text
            RunRunningText();

            if (IsProcessOpen("HostComSip") == false)
            {
                Process.Start(pathHost);
            }
        }
        private void LoggerUSB()
        {
            if (_rawInput == null)
            {
                _rawInput = new RawInput(Handle, CaptureOnlyInForeground);
                _rawInput.AddMessageFilter();
                //_rawInput.KeyPressed -= OnKeyPressed;
                _rawInput.KeyPressed += OnKeyPressed;
                Win32.DeviceAudit();
            }
        }
        void UnLoggerUSB()
        {
            if (_rawInput != null)
            {
                _rawInput.KeyPressed -= OnKeyPressed;
                _rawInput.RemoveMessageFilter();
                _rawInput.ReleaseHandle();
                _rawInput.DestroyHandle();
                _rawInput = null;
            }
        }
        public void downloadFile()
        {
            var pathDevice = Application.StartupPath + "\\Settings\\";
            string[] filePaths = Directory.GetFiles(pathDevice);

            foreach (string line in filePaths)
            {
                using (StreamReader sr = new StreamReader(line))
                {
                    if (line.EndsWith("Reader1.txt"))
                    {
                        device_reader_1 = sr.ReadToEnd().Replace("\r\n", "");
                    }
                    if (line.EndsWith("Reader2.txt"))
                    {
                        device_reader_2 = sr.ReadToEnd().Replace("\r\n", "");
                    }
                    if (line.EndsWith("Reader3.txt"))
                    {
                        device_reader_3 = sr.ReadToEnd().Replace("\r\n", "");
                    }
                    if (line.EndsWith("Reader4.txt"))
                    {
                        device_reader_4 = sr.ReadToEnd().Replace("\r\n", "");
                    }
                    //if (line.EndsWith("Reader5.txt"))
                    //{
                    //    device_reader_5 = sr.ReadToEnd().Replace("\r\n", "");
                    //}
                    //if (line.EndsWith("Reader6.txt"))
                    //{
                    //    device_reader_6 = sr.ReadToEnd().Replace("\r\n", "");
                    //}
                }
            }
        }
        private void FormSIP_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.ExitThread();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelJAM.Text = DateTime.Now.ToString("dd MMMM yyyy - HH:mm:ss");
            string timeRestart = DateTime.Now.ToString("HH:mm:ss");
            if (timeRestart == "00:00:00" && timeRestart == "01:00:00" && timeRestart == "02:00:00")
            {
                // Shut down the current app instance
                Application.Exit();
                // Restart the app
                System.Diagnostics.Process.Start(Application.ExecutablePath);
            }
        }
        private void btnMinimaze_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }
        string handleTemporary { get; set; }
        private void OnKeyPressed(object sender, RawInputEventArg e)
        {
            if (e.KeyPressEvent.DeviceName == "Global Keyboard")
            {
                return;
            }

            string deviceName = e.KeyPressEvent.DeviceName;

            string[] div = deviceName.Split('#');
            if (div.Length >= 2)
            {
                string[] hid = div[1].Split('&');
                if (hid.Length >= 2)
                {
                    string deviceHandle = e.KeyPressEvent.DeviceHandle.ToString();

                    //MessageBox.Show(e.KeyPressEvent.Source);
                    if (e.KeyPressEvent.KeyPressState == "MAKE")
                    {
                        //MessageBox.Show(e.KeyPressEvent.Source);
                        if (e.KeyPressEvent.VKeyName.ToString() != "13")
                        {

                            if (keySource1 == "" || keySource1 == e.KeyPressEvent.Source ){

                                if (keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source && keySource6 != e.KeyPressEvent.Source)
                                {
                                    keySource1 = e.KeyPressEvent.Source;
                                }

                            } else if (keySource2 == "" || keySource2 == e.KeyPressEvent.Source)
                            {
                                if (keySource1 != e.KeyPressEvent.Source)
                                {

                                    if (keySource1 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source && keySource6 != e.KeyPressEvent.Source)
                                    {
                                        keySource2 = e.KeyPressEvent.Source;
                                    }

                                }
                            } else if (keySource3 == "" || keySource3 == e.KeyPressEvent.Source)
                            {
                                if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source)
                                {

                                    if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source && keySource6 != e.KeyPressEvent.Source)
                                    {
                                        keySource3 = e.KeyPressEvent.Source;
                                    }
                                }

                            } else if (keySource4 == "" || keySource4 == e.KeyPressEvent.Source)
                            {
                                if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source)
                                {

                                    if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source && keySource6 != e.KeyPressEvent.Source)
                                    {
                                        keySource4 = e.KeyPressEvent.Source;
                                    }
                                }
                            }
                            //else if (keySource5 == "" || keySource5 == e.KeyPressEvent.Source)
                            //{
                            //    if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source)
                            //    {

                            //        if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource6 != e.KeyPressEvent.Source)
                            //        {
                            //            keySource5 = e.KeyPressEvent.Source;
                            //        }
                            //    }
                            //} else if (keySource6 == "" || keySource6 == e.KeyPressEvent.Source)
                            //{
                            //    if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source)
                            //    {

                            //        if (keySource1 != e.KeyPressEvent.Source && keySource2 != e.KeyPressEvent.Source && keySource3 != e.KeyPressEvent.Source && keySource4 != e.KeyPressEvent.Source && keySource5 != e.KeyPressEvent.Source)
                            //        {
                            //            keySource5 = e.KeyPressEvent.Source;
                            //        }
                            //    }
                            //}

                            if (e.KeyPressEvent.Source== keySource1)
                            {
                                keyData1 += e.KeyPressEvent.VKeyName;
                            }
                            else if (e.KeyPressEvent.Source == keySource2)
                            {
                                keyData2 += e.KeyPressEvent.VKeyName;
                            }
                            else if (e.KeyPressEvent.Source == keySource3)
                            {
                                keyData3 += e.KeyPressEvent.VKeyName;
                            }
                            else if (e.KeyPressEvent.Source == keySource4)
                            {
                                keyData4 += e.KeyPressEvent.VKeyName;
                            }
                            //else if (e.KeyPressEvent.Source == keySource5)
                            //{
                            //    keyData5 += e.KeyPressEvent.VKeyName;
                            //}
                            //else if (e.KeyPressEvent.Source == keySource6)
                            //{
                            //    keyData6 += e.KeyPressEvent.VKeyName;
                            //}
                        }
                        

                    } else {
                        if (e.KeyPressEvent.VKeyName.ToString() == "13")
                        {

                            //MessageBox.Show(keySource1+"--"+keySource2 + "--" + keySource3 + "--" + keySource4 + "--" + keySource5 + "--" + keySource6);

                            if (e.KeyPressEvent.Source == keySource1) {
                                if (keyData1.Length > 0)
                                {
                                    clokck_in_1(1, keyData1, "Mesin Absen 1");
                                    keyData1 = string.Empty;
                                    keySource1 = string.Empty;
                                }
                            }
                            else if (e.KeyPressEvent.Source == keySource2)
                            {
                                if (keyData2.Length > 0)
                                {
                                    clokck_in_2(2, keyData2, "Mesin Absen 2");
                                    keyData2 = string.Empty;
                                    keySource2 = string.Empty;
                                }
                            }
                            else if (e.KeyPressEvent.Source == keySource3)
                            {
                                if (keyData3.Length > 0)
                                {
                                    clokck_in_3(3, keyData3, "Mesin Absen 3");
                                    keyData3 = string.Empty;
                                    keySource3 = string.Empty;
                                }
                            }
                            else if (e.KeyPressEvent.Source == keySource4)
                            {
                                if (keyData4.Length > 0)
                                {
                                    clokck_in_4(4, keyData4, "Mesin Absen 4");
                                    keyData4 = string.Empty;
                                    keySource4 = string.Empty;
                                }
                            }
                            //else if (e.KeyPressEvent.Source == keySource5)
                            //{
                            //    if (keyData5.Length > 0)
                            //    {
                            //        clokck_in_5(5, keyData5, "Mesin Absen 5");
                            //        keyData5 = string.Empty;
                            //        keySource5 = string.Empty;
                            //    }
                            //}
                            //else if (e.KeyPressEvent.Source == keySource6)
                            //{
                            //    if (keyData6.Length > 0)
                            //    {
                            //        clokck_in_6(6, keyData6, "Mesin Absen 6");
                            //        keyData6 = string.Empty;
                            //        keySource6 = string.Empty;
                            //    }
                            //}


                        }
                    }
                }
            }
        }
        private void CekData(string text)
        {
            AbsensiDataContext dc = new SIP.AbsensiDataContext(Koneksi());

            if (dc.siswas.Where(x => x.siswa_nomor_kartu == text).Count() > 0)
            {
                ClockIn3.Text = "SUKSES";
            }

            dc.Dispose();

        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            //FrmSettings frm = new SIP.FrmSettings(this);
            //frm.ShowDialog();
        }
        private void clokck_in_1(int index, string keyData, string noMesin)
        {

            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select    = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData+"'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();
            
            if (r.HasRows)
            {
                r.Read();

                gbr1.Image = Properties.Resources.YES;
                ClockIn1.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                Name1.Text = "Name : " + r[0];
                //MessageBox.Show(Name1.Text);
                
                        ClockIn1.Refresh();
                        Name1.Refresh();
                        gbr1.Refresh();

                Stopwatch stopwatch1 = Stopwatch.StartNew();
                while (true)
                {
                    //some other processing to do possible
                    if (stopwatch1.ElapsedMilliseconds >= 1000)
                    {
                        gbr1.Image = Properties.Resources.NO;
                        ClockIn1.Text = "Clock In";
                        Name1.Text = "Name";
                        break;
                    }
                }

                r.Close();
            }
            conn.Close();
                // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void clokck_in_2(int index, string keyData, string noMesin)
        {
            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData + "'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();

                gbr2.Image = Properties.Resources.YES;
                    ClockIn2.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                    Name2.Text = "Name : " + r[0];
                //MessageBox.Show(Name1.Text);
                ClockIn2.Refresh(); Name2.Refresh(); gbr2.Refresh();

                Stopwatch stopwatch2 = Stopwatch.StartNew();
                while (true)
                {
                    //some other processing to do possible
                    if (stopwatch2.ElapsedMilliseconds >= 500)
                    {

                        gbr2.Image = Properties.Resources.NO;
                        ClockIn2.Text = "Clock In";
                        Name2.Text = "Name";
                       break;
                    }
                }
     

                r.Close();
            }
            conn.Close();
            // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void clokck_in_3(int index, string keyData, string noMesin)
        {
            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData + "'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();
                gbr3.Image = Properties.Resources.YES;
                    ClockIn3.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                    Name3.Text = "Name : " + r[0];
                //MessageBox.Show(Name1.Text);

                Task.Delay(1000).Wait();
                ClockIn3.Refresh(); Name3.Refresh(); gbr3.Refresh();

                    gbr3.Image = Properties.Resources.NO;
                    ClockIn3.Text = "Clock In";
                    Name3.Text = "Name";

                r.Close();
            }
            conn.Close();
            // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void clokck_in_4(int index, string keyData, string noMesin)
        {
            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData + "'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();
                gbr4.Image = Properties.Resources.YES;
                    ClockIn4.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                    Name4.Text = "Name : " + r[0];
                //MessageBox.Show(Name1.Text);

                Task.Delay(1000).Wait();
                ClockIn4.Refresh(); Name4.Refresh(); gbr4.Refresh();

                    gbr4.Image = Properties.Resources.NO;
                    ClockIn4.Text = "Clock In";
                    Name4.Text = "Name";

                r.Close();
            }
            conn.Close();
            // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void clokck_in_5(int index, string keyData, string noMesin)
        {
            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData + "'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();
                //gbr5.Image = Properties.Resources.YES;
                //    ClockIn5.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                //    Name5.Text = "Name : " + r[0];
                ////MessageBox.Show(Name1.Text);

                //Task.Delay(1000).Wait();
                //ClockIn5.Refresh(); Name5.Refresh(); gbr5.Refresh();

                //    gbr5.Image = Properties.Resources.NO;
                //    ClockIn5.Text = "Clock In";
                //    Name5.Text = "Name";

                r.Close();
            }
            conn.Close();
            // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void clokck_in_6(int index, string keyData, string noMesin)
        {
            DateTime dtime = DateTime.Now;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_select = conn.CreateCommand();

            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu";
            string sqldata = "FROM siswa where siswa_nomor_kartu='" + keyData + "'; ";
            dbcmd_select.CommandText = sqlselect + " " + sqldata;

            conn.Open();


            var r = dbcmd_select.ExecuteReader();

            if (r.HasRows)
            {
                r.Read();
                //gbr6.Image = Properties.Resources.YES;
                //    ClockIn6.Text = "Clock In : " + DateTime.Now.ToString("HH:mm");
                //    Name6.Text = "Name : " + r[0];
                ////MessageBox.Show(Name1.Text);

                //Task.Delay(1000).Wait();
                //ClockIn6.Refresh(); Name6.Refresh(); gbr6.Refresh();

                //    gbr6.Image = Properties.Resources.NO;
                //    ClockIn6.Text = "Clock In";
                //    Name6.Text = "Name";

                r.Close();
            }
            conn.Close();
            // SIMPAN
            SIMPAN(keyData, noMesin, dtime);
        }
        private void SIMPAN(string keyData, string noMesin, DateTime dt)
        {
            bool msg = false;
            TimeSpan ts = new TimeSpan(0, 15, 0);

            string absenid = GetRandomNumber(1, 100000).ToString();
            string absensekolahid = id_sekolah();
            string absenkartuid = keyData;
            TimeSpan clock = dt.TimeOfDay;

            MySqlDataReader reader = null;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd_count = conn.CreateCommand();
            MySqlCommand dbcmd_select = conn.CreateCommand();
            MySqlCommand dbcmd_insert = conn.CreateCommand();
            //MySqlCommand dbcmd_update = conn.CreateCommand();

         
            DateTime dateValuess = DateTime.Now;
            string tanggalabsen = dateValuess.ToString("yyyy-MM-dd");

            DateTime dateValue = DateTime.Now;
            string insertat = dateValue.ToString("yyyy-MM-dd HH:mm:ss");

            DateTime dateValues = DateTime.Now;
            string jammasuk = dateValues.ToString("HH:mm:ss");

            string ceksiswa = "SELECT COUNT(*) FROM siswa where siswa_nomor_kartu='" + keyData + "'";

            string sqlcount = "SELECT COUNT(*) FROM absen where absen_nomor_kartu='" + keyData + "' and absen_tanggal='"+ tanggalabsen +"' ";
            dbcmd_count.CommandText = sqlcount;

            string sqlselect = "SELECT sekolah_id, nama_sekolah, alamat FROM sekolah";
            dbcmd_select.CommandText = sqlselect;

            //tambahan hari
            int date_day = (int)System.DateTime.Now.DayOfWeek;
            string hari = date_day.ToString();
            switch (hari)
            {
                case "1":
                    hari = "2";
                    break;
                case "2":
                    hari = "3";
                    break;
                case "3":
                    hari = "4";
                    break;
                case "4":
                    hari = "5";
                    break;
                case "5":
                    hari = "6";
                    break;
                case "6":
                    hari = "7";
                    break;
                default:
                    hari = "1";
                    break;
            }
            string ab_mesin = noMesin.Substring(noMesin.Length - 1, 1);

            string cek_jammulai = "SELECT * FROM waktu_absen WHERE hari='" + hari + "'";
            dbcmd_select.CommandText = cek_jammulai;

            conn.Open();

            var r = dbcmd_select.ExecuteReader();

            //conn.Close();

            if (r.HasRows)
            {
                r.Read();
                
                //MessageBox.Show("ww");

                DateTime mulai_masuk = Convert.ToDateTime(r[1]);
                DateTime mulai_selesai = Convert.ToDateTime(r[2]);
                DateTime pulang_masuk = Convert.ToDateTime(r[3]);
                DateTime pulang_selesai = Convert.ToDateTime(r[4]);
                DateTime get_masuk = Convert.ToDateTime(DateTime.Now);
                DateTime get_pulang_masuk = Convert.ToDateTime(DateTime.Now);
                DateTime get_masuk_selesai = Convert.ToDateTime(DateTime.Now);

                if (get_masuk.TimeOfDay.Ticks < mulai_masuk.TimeOfDay.Ticks)
                {
                    //MessageBox.Show("Belum waktunya absen bro");
                    switch (ab_mesin)
                    {
                        case "1":
                            gbr1.Image = Properties.Resources.TRY;
                            ClockIn1.Text = "Clock In";
                            Name1.Text = "Belum waktunya absen ";
                            break;
                        case "2":
                            gbr2.Image = Properties.Resources.TRY;
                            ClockIn2.Text = "Clock In";
                            Name1.Text = "Belum waktunya absen ";
                            break;
                        case "3":
                            gbr3.Image = Properties.Resources.TRY;
                            ClockIn3.Text = "Clock In";
                            Name1.Text = "Belum waktunya absen ";
                            break;
                        default:
                            gbr4.Image = Properties.Resources.TRY;
                            ClockIn4.Text = "Clock In";
                            Name1.Text = "Belum waktunya absen ";
                            break;
                    }
                }
                else
                {
                    //MessageBox.Show("test");

                    if (get_masuk.TimeOfDay.Ticks > mulai_selesai.TimeOfDay.Ticks && get_pulang_masuk.TimeOfDay.Ticks < pulang_masuk.TimeOfDay.Ticks)
                    {
                        //MessageBox.Show("Maaf, anda sudah terlambat absen");
                        switch (ab_mesin)
                        {
                            case "1":
                                gbr1.Image = Properties.Resources.TRY;
                                ClockIn1.Text = "Clock In";
                                Name1.Text = "Terlambat absen masuk";
                                break;
                            case "2":
                                gbr2.Image = Properties.Resources.TRY;
                                ClockIn2.Text = "Clock In";
                                Name2.Text = "Terlambat absen masuk";
                                break;
                            case "3":
                                gbr3.Image = Properties.Resources.TRY;
                                ClockIn3.Text = "Clock In";
                                Name3.Text = "Terlambat absen masuk";
                                break;
                            default:
                                gbr4.Image = Properties.Resources.TRY;
                                ClockIn4.Text = "Clock In";
                                Name4.Text = "Terlambat absen masuk";
                                break;
                        }
                    }
                    else
                    {
                        conn.Close();
                        conn.Open();
                        dbcmd_count.CommandText = sqlcount;
                        Int32 count = Convert.ToInt32(dbcmd_count.ExecuteScalar()); //proses menghitung jumlah data(count)

                        //reader = dbcmd_select.ExecuteReader();

                        conn.Close();
                        if (count == 0)
                        {
                            if (get_masuk.TimeOfDay.Ticks > mulai_masuk.TimeOfDay.Ticks && get_masuk.TimeOfDay.Ticks < mulai_selesai.TimeOfDay.Ticks)
                            {
                                conn.Open();
                                dbcmd_count.CommandText = ceksiswa;
                                Int32 countsiswa = Convert.ToInt32(dbcmd_count.ExecuteScalar()); //proses menghitung jumlah data(count)
                                conn.Close();

                                if (countsiswa==0)
                                {
                                    switch (ab_mesin)
                                    {
                                        case "1":
                                            gbr1.Image = Properties.Resources.TRY;
                                            ClockIn1.Text = "Clock In";
                                            Name1.Text = "Siswa tidak ditemukan";
                                            break;
                                        case "2":
                                            gbr2.Image = Properties.Resources.TRY;
                                            ClockIn2.Text = "Clock In";
                                            Name2.Text = "Siswa tidak ditemukan";
                                            break;
                                        case "3":
                                            gbr3.Image = Properties.Resources.TRY;
                                            ClockIn3.Text = "Clock In";
                                            Name3.Text = "Siswa tidak ditemukan";
                                            break;
                                        default:
                                            gbr4.Image = Properties.Resources.TRY;
                                            ClockIn4.Text = "Clock In";
                                            Name4.Text = "Siswa tidak ditemukan";
                                            break;
                                    }
                                    //MessageBox.Show("Maaf, data siswa tidak ditemukan!");
                                }
                                else
                                {
                                    string sqlcekabsen = "SELECT absen_masuk FROM absen WHERE absen_nomor_kartu='" + absenkartuid + "' AND absen_sekolah_id='" + absensekolahid + "' AND absen_tanggal='" + tanggalabsen + "'";
                                    dbcmd_select.CommandText = sqlcekabsen;
                                    conn.Open();
                                    MySqlDataReader rdr = dbcmd_select.ExecuteReader();
                                    string absen_masuk = "";
                                    while (rdr.Read())
                                    {
                                        absen_masuk = rdr.GetValue(0).ToString();
                                    }
                                    conn.Close();
                                    if (absen_masuk != null || absen_masuk != "")
                                    {
                                        string sqlinsert = "insert into absen(absen_nomor_kartu, absen_sekolah_id, absen_status, absen_device, absen_tanggal, InsertAt, absen_masuk ) " +
                                        "values('" + absenkartuid + "','" + absensekolahid + "','" + "PENDING" + "','" + noMesin + "','" + tanggalabsen + "','" + insertat + "','" + jammasuk + "');";
                                        dbcmd_insert.CommandText = sqlinsert;

                                        try
                                        {
                                            MySqlDataReader MyReader1 = null;
                                            conn.Open();
                                            MyReader1 = dbcmd_insert.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                                            conn.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                        msg = true;
                                    }
                                    else
                                    {
                                        switch (ab_mesin)
                                        {
                                            case "1":
                                                gbr1.Image = Properties.Resources.TRY;
                                                ClockIn1.Text = "Clock In";
                                                Name1.Text = "Siswa sudah melakukan absen masuk";
                                                break;
                                            case "2":
                                                gbr2.Image = Properties.Resources.TRY;
                                                ClockIn2.Text = "Clock In";
                                                Name2.Text = "Siswa sudah melakukan absen masuk";
                                                break;
                                            case "3":
                                                gbr3.Image = Properties.Resources.TRY;
                                                ClockIn3.Text = "Clock In";
                                                Name3.Text = "Siswa sudah melakukan absen masuk";
                                                break;
                                            default:
                                                gbr4.Image = Properties.Resources.TRY;
                                                ClockIn4.Text = "Clock In";
                                                Name4.Text = "Siswa sudah melakukan absen masuk";
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                switch (ab_mesin)
                                {
                                    case "1":
                                        gbr1.Image = Properties.Resources.TRY;
                                        ClockIn1.Text = "Clock In";
                                        Name1.Text = "Terlambat absen masuk";
                                        break;
                                    case "2":
                                        gbr2.Image = Properties.Resources.TRY;
                                        ClockIn2.Text = "Clock In";
                                        Name2.Text = "Terlambat absen masuk";
                                        break;
                                    case "3":
                                        gbr3.Image = Properties.Resources.TRY;
                                        ClockIn3.Text = "Clock In";
                                        Name3.Text = "Terlambat absen masuk";
                                        break;
                                    default:
                                        gbr4.Image = Properties.Resources.TRY;
                                        ClockIn4.Text = "Clock In";
                                        Name4.Text = "Terlambat absen masuk";
                                        break;
                                }
                            }



                        }
                        else
                        {

                           // MessageBox.Show("update");


                            if (get_pulang_masuk.TimeOfDay.Ticks < pulang_masuk.TimeOfDay.Ticks)
                            {
                                //MessageBox.Show("Maaf, anda sudah melakukan absen");
                                switch (ab_mesin)
                                {
                                    case "1":
                                        gbr1.Image = Properties.Resources.TRY;
                                        ClockIn1.Text = "Clock In";
                                        Name1.Text = "Belum waktunya absen pulang";
                                        break;
                                    case "2":
                                        gbr2.Image = Properties.Resources.TRY;
                                        ClockIn2.Text = "Clock In";
                                        Name2.Text = "Belum waktunya absen pulang";
                                        break;
                                    case "3":
                                        gbr3.Image = Properties.Resources.TRY;
                                        ClockIn3.Text = "Clock In";
                                        Name3.Text = "Belum waktunya absen pulang";
                                        break;
                                    default:
                                        gbr4.Image = Properties.Resources.TRY;
                                        ClockIn4.Text = "Clock In";
                                        Name4.Text = "Belum waktunya absen pulang";
                                        break;
                                }
                            }
                            else if(get_pulang_masuk.TimeOfDay.Ticks > pulang_selesai.TimeOfDay.Ticks)
                            {
                                //MessageBox.Show("Maaf, anda sudah terlambat untuk absen pulang");
                                switch (ab_mesin)
                                {
                                    case "1":
                                        gbr1.Image = Properties.Resources.TRY;
                                        ClockIn1.Text = "Clock In";
                                        Name1.Text = "Terlambat absen pulang";
                                        break;
                                    case "2":
                                        gbr2.Image = Properties.Resources.TRY;
                                        ClockIn2.Text = "Clock In";
                                        Name2.Text = "Terlambat absen pulang";
                                        break;
                                    case "3":
                                        gbr3.Image = Properties.Resources.TRY;
                                        ClockIn3.Text = "Clock In";
                                        Name3.Text = "Terlambat absen pulang";
                                        break;
                                    default:
                                        gbr4.Image = Properties.Resources.TRY;
                                        ClockIn4.Text = "Clock In";
                                        Name4.Text = "Terlambat absen pulang";
                                        break;
                                }
                            }
                            else
                            {

                                MySqlCommand dbcmd_update = conn.CreateCommand();
                                //MessageBox.Show("man");
                                //string sqlcekabsen = "SELECT absen_keluar FROM absen WHERE absen_nomor_kartu='" + absenkartuid + "' AND absen_sekolah_id='" + absensekolahid + "' AND absen_tanggal='" + tanggalabsen + "';";
                                //dbcmd_update.CommandText = sqlcekabsen;
                                //conn.Open();
                                //MySqlDataReader rdr = dbcmd_update.ExecuteReader();
                                //string absen_keluar = "";
                                //while (rdr.Read())
                                //{
                                //    absen_keluar = rdr.GetValue(0).ToString();
                                //}
                                //conn.Close();
                                //if (absen_keluar == null || absen_keluar == "")
                                //{   
                                    //MessageBox.Show("man");
                                    string sqlup = "update absen SET absen_keluar = '" + jammasuk + "', upload_keluar ='0' where absen_nomor_kartu ='" + absenkartuid + "' and absen_tanggal = '" + tanggalabsen + "';";
                                    dbcmd_update.CommandText = sqlup;
                                    try 
                                    {
                                        MySqlDataReader MyReader2 = null;

                                        //conn.Close();
                                        conn.Open();
                                        MyReader2 = dbcmd_update.ExecuteReader(); 
                                        conn.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        //MessageBox.Show(ex.Message);
                                    }
                                //}
                                //else
                                //{
                                //    gbr1.Image = Properties.Resources.NO;
                                //    ClockIn1.Text = "Clock In";
                                //    Name1.Text = "Sudah absen pulang";
                                //}
                                /*
                                }
                                else
                                {
                                    MessageBox.Show("Not Found");
                                }
                                */
                            }

                            
                        }


                    }
                }


                r.Close();
            }

            conn.Close();

            timerUpdate();
        }
        protected bool isActiveCard(string keyData)
        {
            AbsensiDataContext dc = new SIP.AbsensiDataContext(Koneksi());
            dc.CommandTimeout = 0;

            bool msg = false;
            if (dc.siswas.Where(x => x.siswa_nomor_kartu == keyData).Count() > 0)
            {
                msg = true;
            }
            dc.Dispose();
            return msg;
        }
        private static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
        protected string id_sekolah()
        {
            string id = string.Empty;

            MySqlDataReader reader = null;

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            /*int i = 1;*/
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
        private void FormSIP_Load(object sender, EventArgs e)
        {
            try
            {
                if (validasi == true)
                {
                    //timerRefresh.Enabled = true;
                    timerUpdate();
                }
                LoggerUSB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tsDevice_Click(object sender, EventArgs e)
        {
            FormSetAbsen frm = new SIP.FormSetAbsen(this);
            frm.ShowDialog();
            //MessageBox.Show("testtt");
        }
        private void tsDownload_Click(object sender, EventArgs e)
        {
            FormUpload frm = new FormUpload();
            //FormDownload frm = new SIP.FormDownload();
            frm.ShowDialog();
        }
        private void tsSekolah_Click(object sender, EventArgs e)
        {
            UnLoggerUSB();
            FormSekolah frm = new SIP.FormSekolah(this);
            frm.ShowDialog();
        }
        public void Load_USB()
        {
            try
            {
                LoggerUSB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void tsInstall_Click(object sender, EventArgs e)
        {
            string SQL = string.Empty;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        SQL = Environment.MachineName + @"\" + instanceName;
                    }
                }
            }

            if (string.IsNullOrEmpty(SQL))
            {
                Process.Start(pathSQL);
            }
            else
            {
                MessageBox.Show("SQL SUDAH TERINSTAL", "INFO SIP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool CheckDatabaseExists(SqlConnection tmpConn)
        {
            string sqlCreateDBQuery;
            bool result = false;

            try
            {
                tmpConn = new SqlConnection("server=(local)\\SQLEXPRESS;Trusted_Connection=yes");

                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", DatabaseName);

                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();

                        object resultObj = sqlCmd.ExecuteScalar();

                        int databaseID = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }

                        tmpConn.Close();

                        result = (databaseID > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return true;
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

        private void tsHelpSQL_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(pathPDF);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tsSettings_Click(object sender, EventArgs e)
        {
            UnLoggerUSB();
            FormSettings frm = new SIP.FormSettings(this);
            frm.ShowDialog();
        }
        private void tsCreate_Click_1(object sender, EventArgs e)
        {
            FormCreate frm = new SIP.FormCreate();
            frm.ShowDialog();
        }
        private void tsWebservice_Click(object sender, EventArgs e)
        {
            UnLoggerUSB();
            FormWebService frm = new SIP.FormWebService(this);
            frm.ShowDialog();
        }
        private void tsSIP_Click(object sender, EventArgs e)
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
        }
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            DateTime dateValuessa = DateTime.Now;
            string tanggalabs = dateValuessa.ToString("yyyy-MM-dd");

            // string persis = True;
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd = conn.CreateCommand();
            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu, absen.absen_masuk, absen.absen_keluar, absen.absen_tanggal";
            string sqldata   = "FROM absen LEFT JOIN siswa ON siswa.siswa_nomor_kartu = absen.absen_nomor_kartu where absen_tanggal='"+ tanggalabs +"' ;";
            dbcmd.CommandText = sqlselect +" "+ sqldata;

            var dt = new DataTable("absen");
            conn.Open();
            var r = dbcmd.ExecuteReader();
            
            dt.Load(r);

            dgv.Rows.Clear();
            dgv.Refresh();

            foreach (DataRow baris in dt.Rows)
            {
                dgv.Rows.Add(i, baris["siswa_nama"], baris["absen_masuk"], baris["absen_keluar"]);
                i++;
            }
            conn.Close();
        }

        private void timerUpdate()
        {
            DateTime dateValuessa = DateTime.Now;
            string tanggalabs = dateValuessa.ToString("yyyy-MM-dd");

            // string persis = True;
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            int i = 1;
            MySqlCommand dbcmd = conn.CreateCommand();
            string sqlselect = "SELECT siswa.siswa_nama, siswa.siswa_nomor_kartu, absen.absen_masuk, absen.absen_keluar, absen.absen_tanggal";
            string sqldata = "FROM absen LEFT JOIN siswa ON siswa.siswa_nomor_kartu = absen.absen_nomor_kartu where absen_tanggal='" + tanggalabs + "' ;";
            dbcmd.CommandText = sqlselect + " " + sqldata;

            var dt = new DataTable("absen");
            conn.Open();
            var r = dbcmd.ExecuteReader();

            dt.Load(r);

            dgv.Rows.Clear();
            dgv.Refresh();

            foreach (DataRow baris in dt.Rows)
            {
                dgv.Rows.Add(i, baris["siswa_nama"], baris["absen_masuk"], baris["absen_keluar"]);
                i++;
            }
            conn.Close();
        }

        private void runningTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnLoggerUSB();
            FormRunningText frm = new SIP.FormRunningText(this);
            frm.ShowDialog();
        }

        private void RunRunningText()
        {
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand dbcmd = conn.CreateCommand();

            string sqlselect = "SELECT * FROM running_text ;";
            dbcmd.CommandText = sqlselect;

            var dt = new DataTable("running_text");
            conn.Open();
            var r = dbcmd.ExecuteReader();
            dt.Load(r);
            string brs;
            bCL_RunningText.Text = "";
            foreach (DataRow baris in dt.Rows)
            {
                brs = baris["text"].ToString();
                bCL_RunningText.Text = bCL_RunningText.Text + " | " + brs + "";
            }
            xpos = bCL_RunningText.Location.X;
            ypos = bCL_RunningText.Location.Y;
            mode = "Right-to-Left";
            timerRunningText.Start();
        }

        private void timerRunningText_Tick(object sender, EventArgs e)
        {
            //if (xpos == 0)

            //{

            //    this.bCL_RunningText.Location = new System.Drawing.Point(this.Width, ypos);

            //    xpos = this.Width;

            //}

            //else

            //{

            //    this.bCL_RunningText.Location = new System.Drawing.Point(xpos, ypos);

            //    xpos -= 2;

            //}
            this.bCL_RunningText.Location = new Point(bCL_RunningText.Location.X - 3, bCL_RunningText.Location.Y);
            if (bCL_RunningText.Location.X < (0 - bCL_RunningText.Width))
            {
                bCL_RunningText.Location = new Point(this.Width, bCL_RunningText.Location.Y);
            }
        }

        public void RefreshRunningText()
        {
            RunRunningText();
        }



    }
}
