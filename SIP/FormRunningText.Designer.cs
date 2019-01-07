namespace SIP
{
    partial class FormRunningText
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRunningText));
            this.txtRunningText = new System.Windows.Forms.TextBox();
            this.dgvRT = new System.Windows.Forms.DataGridView();
            this.rt_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rt_text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rt_createAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.bTB_Submit = new Bunifu.Framework.UI.BunifuThinButton2();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRT)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRunningText
            // 
            this.txtRunningText.Location = new System.Drawing.Point(100, 11);
            this.txtRunningText.Name = "txtRunningText";
            this.txtRunningText.Size = new System.Drawing.Size(556, 20);
            this.txtRunningText.TabIndex = 8;
            // 
            // dgvRT
            // 
            this.dgvRT.AllowUserToAddRows = false;
            this.dgvRT.AllowUserToDeleteRows = false;
            this.dgvRT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRT.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rt_id,
            this.rt_text,
            this.rt_createAt});
            this.dgvRT.Location = new System.Drawing.Point(12, 88);
            this.dgvRT.Name = "dgvRT";
            this.dgvRT.ReadOnly = true;
            this.dgvRT.Size = new System.Drawing.Size(644, 220);
            this.dgvRT.TabIndex = 9;
            this.dgvRT.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRT_CellDoubleClick);
            // 
            // rt_id
            // 
            this.rt_id.HeaderText = "ID";
            this.rt_id.Name = "rt_id";
            this.rt_id.ReadOnly = true;
            this.rt_id.Visible = false;
            // 
            // rt_text
            // 
            this.rt_text.HeaderText = "Text";
            this.rt_text.Name = "rt_text";
            this.rt_text.ReadOnly = true;
            this.rt_text.Width = 500;
            // 
            // rt_createAt
            // 
            this.rt_createAt.HeaderText = "Tanggal";
            this.rt_createAt.Name = "rt_createAt";
            this.rt_createAt.ReadOnly = true;
            this.rt_createAt.Width = 120;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Running Text";
            // 
            // bTB_Submit
            // 
            this.bTB_Submit.ActiveBorderThickness = 1;
            this.bTB_Submit.ActiveCornerRadius = 20;
            this.bTB_Submit.ActiveFillColor = System.Drawing.Color.SeaGreen;
            this.bTB_Submit.ActiveForecolor = System.Drawing.Color.White;
            this.bTB_Submit.ActiveLineColor = System.Drawing.Color.SeaGreen;
            this.bTB_Submit.BackColor = System.Drawing.SystemColors.Control;
            this.bTB_Submit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bTB_Submit.BackgroundImage")));
            this.bTB_Submit.ButtonText = "Submit";
            this.bTB_Submit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bTB_Submit.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bTB_Submit.ForeColor = System.Drawing.Color.SeaGreen;
            this.bTB_Submit.IdleBorderThickness = 1;
            this.bTB_Submit.IdleCornerRadius = 20;
            this.bTB_Submit.IdleFillColor = System.Drawing.Color.White;
            this.bTB_Submit.IdleForecolor = System.Drawing.Color.SeaGreen;
            this.bTB_Submit.IdleLineColor = System.Drawing.Color.SeaGreen;
            this.bTB_Submit.Location = new System.Drawing.Point(100, 39);
            this.bTB_Submit.Margin = new System.Windows.Forms.Padding(5);
            this.bTB_Submit.Name = "bTB_Submit";
            this.bTB_Submit.Size = new System.Drawing.Size(118, 41);
            this.bTB_Submit.TabIndex = 10;
            this.bTB_Submit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bTB_Submit.Click += new System.EventHandler(this.bTB_Submit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(477, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "* Klik ganda untuk hapus data";
            // 
            // FormRunningText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 319);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bTB_Submit);
            this.Controls.Add(this.txtRunningText);
            this.Controls.Add(this.dgvRT);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRunningText";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RunningText";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuThinButton2 bTB_Submit;
        private System.Windows.Forms.TextBox txtRunningText;
        private System.Windows.Forms.DataGridView dgvRT;
        private System.Windows.Forms.DataGridViewTextBoxColumn rt_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn rt_text;
        private System.Windows.Forms.DataGridViewTextBoxColumn rt_createAt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}