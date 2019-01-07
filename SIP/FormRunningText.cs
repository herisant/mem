using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIP
{
    public partial class FormRunningText : Form
    {
        FormSIP frm;
        string host = "localhost";
        string user = "root";
        string password = "";
        string database = "sip_absen";
        string ssl = "none";
        public FormRunningText(FormSIP frm)
        {
            InitializeComponent();
            this.frm = frm;
            txtRunningText.Text = string.Empty;
            GetRunningText();
            
        }

        private void GetRunningText()
        {
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand dbcmd = conn.CreateCommand();

            string sqlselect = "SELECT * FROM running_text ORDER BY createAt DESC;";
            dbcmd.CommandText = sqlselect;

            var dt = new DataTable("running_text");
            conn.Open();
            var r = dbcmd.ExecuteReader();
            dt.Load(r);

            dgvRT.Rows.Clear();
            dgvRT.Refresh();

            foreach (DataRow baris in dt.Rows)
            {
                dgvRT.Rows.Add(baris["id"], baris["text"], baris["createAt"]);
            }


            conn.Close();

            frm.RefreshRunningText();
        }

        private void bTB_Submit_Click(object sender, EventArgs e)
        {
            string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand dbcmd = conn.CreateCommand();

            string text = txtRunningText.Text;
            DateTime datetimenow = Convert.ToDateTime(DateTime.Now);
            string sekarang = datetimenow.ToString("yyyy-MM-dd HH:mm:ss");

            if (text != string.Empty && text != "" && text != " " && text != null)
            {
                MessageBox.Show(text);

                string sqlinsert = "INSERT INTO running_text (text, createAt) values ('" + text + "','" + sekarang + "');";
                dbcmd.CommandText = sqlinsert;
                try
                {
                    MySqlDataReader myReader = null;
                    conn.Open();
                    myReader = dbcmd.ExecuteReader();
                    conn.Close();

                    //MessageBox.Show("Data text telah tersimpan.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Data text tidak diijinkan kosong.");
            }
            GetRunningText();

            txtRunningText.Text = string.Empty;
        }

        private void dgvRT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Apakah anda yakin menghapus data ini?",
                "Hapus Data",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result1.ToString() == "Yes")
            {
                if (dgvRT.SelectedCells.Count > 0)
                {
                    int index = e.RowIndex;// get the Row Index
                    DataGridViewRow selectedRow = dgvRT.Rows[index];

                    string id = Convert.ToString(selectedRow.Cells["rt_id"].Value);
                    string connStr = "server=" + host + ";user=" + user + ";database=" + database + ";password=" + password + ";SslMode=" + ssl + ";";
                    MySqlConnection conn = new MySqlConnection(connStr);
                    MySqlCommand dbcmd = conn.CreateCommand();
                    conn.Open();
                    string sqldelete = "DELETE FROM running_text WHERE id=" + id + ";";
                    dbcmd.CommandText = sqldelete;
                    dbcmd.ExecuteNonQuery();
                    conn.Close();
                }
                GetRunningText();
            }
        }
    }
}
