namespace BULi
{
    partial class display
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
            this.lblYearTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlhawakupdate = new System.Windows.Forms.Panel();
            this.btnDeleteSI = new System.Windows.Forms.Button();
            this.btnAddSI = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnProfile = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnView = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnUpdateSI = new System.Windows.Forms.Button();
            this.btnSort = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnlhawakupdate.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblYearTitle
            // 
            this.lblYearTitle.AutoSize = true;
            this.lblYearTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYearTitle.Location = new System.Drawing.Point(30, 40);
            this.lblYearTitle.Name = "lblYearTitle";
            this.lblYearTitle.Size = new System.Drawing.Size(190, 25);
            this.lblYearTitle.TabIndex = 0;
            this.lblYearTitle.Text = "year title placeholder";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.pnlhawakupdate);
            this.panel1.Controls.Add(this.btnDeleteSI);
            this.panel1.Controls.Add(this.btnAddSI);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 563);
            this.panel1.TabIndex = 1;
            // 
            // pnlhawakupdate
            // 
            this.pnlhawakupdate.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlhawakupdate.Controls.Add(this.label3);
            this.pnlhawakupdate.Controls.Add(this.label2);
            this.pnlhawakupdate.Controls.Add(this.label1);
            this.pnlhawakupdate.Controls.Add(this.textBox4);
            this.pnlhawakupdate.Controls.Add(this.textBox3);
            this.pnlhawakupdate.Controls.Add(this.textBox2);
            this.pnlhawakupdate.Controls.Add(this.btnUpdateSI);
            this.pnlhawakupdate.Location = new System.Drawing.Point(25, 224);
            this.pnlhawakupdate.Name = "pnlhawakupdate";
            this.pnlhawakupdate.Size = new System.Drawing.Size(239, 312);
            this.pnlhawakupdate.TabIndex = 4;
            // 
            // btnDeleteSI
            // 
            this.btnDeleteSI.Location = new System.Drawing.Point(60, 141);
            this.btnDeleteSI.Name = "btnDeleteSI";
            this.btnDeleteSI.Size = new System.Drawing.Size(172, 51);
            this.btnDeleteSI.TabIndex = 5;
            this.btnDeleteSI.Text = "Delete";
            this.btnDeleteSI.UseVisualStyleBackColor = true;
            // 
            // btnAddSI
            // 
            this.btnAddSI.Location = new System.Drawing.Point(60, 75);
            this.btnAddSI.Name = "btnAddSI";
            this.btnAddSI.Size = new System.Drawing.Size(172, 51);
            this.btnAddSI.TabIndex = 4;
            this.btnAddSI.Text = "Add";
            this.btnAddSI.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.btnProfile);
            this.panel2.Controls.Add(this.lblYearTitle);
            this.panel2.Location = new System.Drawing.Point(292, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(690, 95);
            this.panel2.TabIndex = 2;
            // 
            // btnProfile
            // 
            this.btnProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfile.BackColor = System.Drawing.Color.Transparent;
            this.btnProfile.BackgroundImage = global::BULi.Properties.Resources.image_removebg_preview__1_;
            this.btnProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnProfile.Location = new System.Drawing.Point(628, 21);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(50, 49);
            this.btnProfile.TabIndex = 1;
            this.btnProfile.UseVisualStyleBackColor = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(327, 190);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(606, 307);
            this.dataGridView1.TabIndex = 3;
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(789, 503);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(181, 48);
            this.btnView.TabIndex = 6;
            this.btnView.Text = "View Student Information";
            this.btnView.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Location = new System.Drawing.Point(327, 106);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(518, 63);
            this.panel3.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 14);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(449, 32);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Enter Keme";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(470, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(46, 32);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "search 2";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnUpdateSI
            // 
            this.btnUpdateSI.Location = new System.Drawing.Point(35, 232);
            this.btnUpdateSI.Name = "btnUpdateSI";
            this.btnUpdateSI.Size = new System.Drawing.Size(172, 51);
            this.btnUpdateSI.TabIndex = 6;
            this.btnUpdateSI.Text = "Update";
            this.btnUpdateSI.UseVisualStyleBackColor = true;
            // 
            // btnSort
            // 
            this.btnSort.BackColor = System.Drawing.Color.Maroon;
            this.btnSort.Location = new System.Drawing.Point(878, 120);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(55, 43);
            this.btnSort.TabIndex = 8;
            this.btnSort.Text = "button1";
            this.btnSort.UseVisualStyleBackColor = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(23, 52);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(197, 22);
            this.textBox2.TabIndex = 9;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(23, 113);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(197, 22);
            this.textBox3.TabIndex = 10;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(23, 169);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(197, 22);
            this.textBox4.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "label3";
            // 
            // display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 563);
            this.Controls.Add(this.btnSort);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "display";
            this.Text = "display";
            this.panel1.ResumeLayout(false);
            this.pnlhawakupdate.ResumeLayout(false);
            this.pnlhawakupdate.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblYearTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnDeleteSI;
        private System.Windows.Forms.Button btnAddSI;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel pnlhawakupdate;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnUpdateSI;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
    }
}