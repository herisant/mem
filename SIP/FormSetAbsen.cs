using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIP
{
    public partial class FormSetAbsen : Form
    {
        FormSIP frm;
        string _koneksi = "Server={0};Database={1};User Id={2};Password={3};";
        protected string pathDB = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)) + @"\myconfig.ini";
        protected FirewallSip _security = new FirewallSip();

        string host = "localhost";
        string user = "root";
        string password = "";
        string database = "sip_absen";
        string ssl = "none";

        public FormSetAbsen(FormSIP frm)
        {
            InitializeComponent();
            this.frm = frm;

            GetJamAbsensi();
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
        private void btnSimpanIdentitas_Click(object sender, EventArgs e)
        {

        }
        private void FormSekolah_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.frm.Load_USB();
        }

        private void txtKode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNama_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSimpanSenin_Click(object sender, EventArgs e)
        {
            string day = "2";
            string mm = dtp_MM_Senin.Text;
            string sm = dtp_SM_Senin.Text;
            string mp = dtp_MP_Senin.Text;
            string sp = dtp_SP_Senin.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanSelasa_Click(object sender, EventArgs e)
        {
            string day = "3";
            string mm = dtp_MM_Selasa.Text;
            string sm = dtp_SM_Selasa.Text;
            string mp = dtp_MP_Selasa.Text;
            string sp = dtp_SP_Selasa.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanRabu_Click(object sender, EventArgs e)
        {
            string day = "4";
            string mm = dtp_MM_Rabu.Text;
            string sm = dtp_SM_Rabu.Text;
            string mp = dtp_MP_Rabu.Text;
            string sp = dtp_SP_Rabu.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanKamis_Click(object sender, EventArgs e)
        {
            string day = "5";
            string mm = dtp_MM_Kamis.Text;
            string sm = dtp_SM_Kamis.Text;
            string mp = dtp_MP_Kamis.Text;
            string sp = dtp_SP_Kamis.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanJumat_Click(object sender, EventArgs e)
        {
            string day = "6";
            string mm = dtp_MM_Jumat.Text;
            string sm = dtp_SM_Jumat.Text;
            string mp = dtp_MP_Jumat.Text;
            string sp = dtp_SP_Jumat.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanSabtu_Click(object sender, EventArgs e)
        {
            string day = "7";
            string mm = dtp_MM_Sabtu.Text;
            string sm = dtp_SM_Sabtu.Text;
            string mp = dtp_MP_Sabtu.Text;
            string sp = dtp_SP_Sabtu.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void bTB_SimpanMinggu_Click(object sender, EventArgs e)
        {
            string day = "1";
            string mm = dtp_MM_Minggu.Text;
            string sm = dtp_SM_Minggu.Text;
            string mp = dtp_MP_Minggu.Text;
            string sp = dtp_SP_Minggu.Text;
            simpanJamAbsensi(day, mm, sm, mp, sp);
        }

        private void simpanJamAbsensi(string hari, string mm, string sm, string mp, string sp)
        {

            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand dbcmd = conn.CreateCommand();
            string sqlcount = "SELECT COUNT(*) FROM waktu_absen WHERE hari='" + hari + "'";

            conn.Open();

            dbcmd.CommandText = sqlcount;

            Int32 count = Convert.ToInt32(dbcmd.ExecuteScalar()); //proses menghitung jumlah data(count)

            conn.Close();

            if (count == 0)
            {
                string sqlinsert = "insert into waktu_absen(mulai_masuk, mulai_selesai, pulang_masuk, pulang_selesai, hari) " +
                    "values('" + mm + "','" + sm + "','" + mp + "','" + sp + "','" + hari + "');";
                dbcmd.CommandText = sqlinsert;

                try
                {

                    MySqlDataReader MyReader2;
                    conn.Open();
                    MyReader2 = dbcmd.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                    MessageBox.Show("Settingan Absen Berhasil Disimpan.");

                    conn.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (count == 1)
            {
                string sqlupdate = "UPDATE waktu_absen SET mulai_masuk='" + mm + "',mulai_selesai='" + sm + "',pulang_masuk='" + mp + "',pulang_selesai='" + sp + "' WHERE hari='" + hari + "'";
                dbcmd.CommandText = sqlupdate;
                MySqlDataReader MyReader2;
                conn.Open();
                MyReader2 = dbcmd.ExecuteReader();
                //dbcmd.ExecuteNonQuery();
                MessageBox.Show("Data Setingan Absen telah diperbarui.");
                conn.Close();
                //MessageBox.Show("Data Setingan Absen telah disetting. Silahkan kontak admin jika ingin merubah data setingan");
            }

            GetJamAbsensi();
        }

        private void GetJamAbsensi()
        {
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand dbcmd = conn.CreateCommand();
            string findSenin = "SELECT * FROM waktu_absen WHERE hari='2'";
            string findSelasa = "SELECT * FROM waktu_absen WHERE hari='3'";
            string findRabu = "SELECT * FROM waktu_absen WHERE hari='4'";
            string findKamis = "SELECT * FROM waktu_absen WHERE hari='5'";
            string findJumat = "SELECT * FROM waktu_absen WHERE hari='6'";
            string findSabtu = "SELECT * FROM waktu_absen WHERE hari='7'";
            string findMinggu = "SELECT * FROM waktu_absen WHERE hari='1'";

            dbcmd.CommandText = findSenin;
            conn.Open();
            var r_Senin = dbcmd.ExecuteReader();
            if (r_Senin.HasRows)
            {
                r_Senin.Read();

                dtp_MM_Senin.Text = Convert.ToString(r_Senin[1]);
                dtp_SM_Senin.Text = Convert.ToString(r_Senin[2]);
                dtp_MP_Senin.Text = Convert.ToString(r_Senin[3]);
                dtp_SP_Senin.Text = Convert.ToString(r_Senin[4]);

            }
            conn.Close();
            dbcmd.CommandText = findSelasa;
            conn.Open();
            var r_Selasa = dbcmd.ExecuteReader();
            if (r_Selasa.HasRows)
            {
                r_Selasa.Read();

                dtp_MM_Selasa.Text = Convert.ToString(r_Selasa[1]);
                dtp_SM_Selasa.Text = Convert.ToString(r_Selasa[2]);
                dtp_MP_Selasa.Text = Convert.ToString(r_Selasa[3]);
                dtp_SP_Selasa.Text = Convert.ToString(r_Selasa[4]);

            }
            conn.Close();
            dbcmd.CommandText = findRabu;
            conn.Open();
            var r_Rabu = dbcmd.ExecuteReader();
            if (r_Rabu.HasRows)
            {
                r_Rabu.Read();

                dtp_MM_Rabu.Text = Convert.ToString(r_Rabu[1]);
                dtp_SM_Rabu.Text = Convert.ToString(r_Rabu[2]);
                dtp_MP_Rabu.Text = Convert.ToString(r_Rabu[3]);
                dtp_SP_Rabu.Text = Convert.ToString(r_Rabu[4]);

            }
            conn.Close();
            dbcmd.CommandText = findKamis;
            conn.Open();
            var r_Kamis = dbcmd.ExecuteReader();
            if (r_Kamis.HasRows)
            {
                r_Kamis.Read();

                dtp_MM_Kamis.Text = Convert.ToString(r_Kamis[1]);
                dtp_SM_Kamis.Text = Convert.ToString(r_Kamis[2]);
                dtp_MP_Kamis.Text = Convert.ToString(r_Kamis[3]);
                dtp_SP_Kamis.Text = Convert.ToString(r_Kamis[4]);

            }
            conn.Close();
            dbcmd.CommandText = findJumat;
            conn.Open();
            var r_Jumat = dbcmd.ExecuteReader();
            if (r_Jumat.HasRows)
            {
                r_Jumat.Read();

                dtp_MM_Jumat.Text = Convert.ToString(r_Jumat[1]);
                dtp_SM_Jumat.Text = Convert.ToString(r_Jumat[2]);
                dtp_MP_Jumat.Text = Convert.ToString(r_Jumat[3]);
                dtp_SP_Jumat.Text = Convert.ToString(r_Jumat[4]);

            }
            conn.Close();
            dbcmd.CommandText = findSabtu;
            conn.Open();
            var r_Sabtu = dbcmd.ExecuteReader();
            if (r_Sabtu.HasRows)
            {
                r_Sabtu.Read();

                dtp_MM_Sabtu.Text = Convert.ToString(r_Sabtu[1]);
                dtp_SM_Sabtu.Text = Convert.ToString(r_Sabtu[2]);
                dtp_MP_Sabtu.Text = Convert.ToString(r_Sabtu[3]);
                dtp_SP_Sabtu.Text = Convert.ToString(r_Sabtu[4]);

            }
            conn.Close();
            dbcmd.CommandText = findMinggu;
            conn.Open();
            var r_Minggu = dbcmd.ExecuteReader();
            if (r_Minggu.HasRows)
            {
                r_Minggu.Read();

                dtp_MM_Minggu.Text = Convert.ToString(r_Minggu[1]);
                dtp_SM_Minggu.Text = Convert.ToString(r_Minggu[2]);
                dtp_MP_Minggu.Text = Convert.ToString(r_Minggu[3]);
                dtp_SP_Minggu.Text = Convert.ToString(r_Minggu[4]);

            }
            conn.Close();

        }
    }
}
