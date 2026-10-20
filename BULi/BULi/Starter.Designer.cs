namespace BULi
{
    partial class Starter
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
            this.components = new System.ComponentModel.Container();
            this.tmrOpen = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrOpen
            // 
            this.tmrOpen.Tick += new System.EventHandler(this.tmrOpen_Tick);
            // 
            // Starter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.BackgroundImage = global::BULi.Properties.Resources.tempstarter;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(982, 563);
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Name = "Starter";
            this.Text = "`";
            this.Load += new System.EventHandler(this.Starter_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrOpen;
    }
}