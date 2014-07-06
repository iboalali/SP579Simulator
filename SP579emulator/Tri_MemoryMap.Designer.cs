namespace SP579emulator {
    partial class Tri_MemoryMap {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lblFrom = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnShowMM = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnUpPage = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnDownPage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(12, 9);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 20);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "From";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(48, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 2;
            // 
            // btnShowMM
            // 
            this.btnShowMM.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnShowMM.Location = new System.Drawing.Point(154, 4);
            this.btnShowMM.Name = "btnShowMM";
            this.btnShowMM.Size = new System.Drawing.Size(75, 23);
            this.btnShowMM.TabIndex = 5;
            this.btnShowMM.Text = "Show";
            this.btnShowMM.UseVisualStyleBackColor = true;
            this.btnShowMM.Click += new System.EventHandler(this.btnShowMM_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(740, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.Size = new System.Drawing.Size(800, 376);
            this.dataGridView1.TabIndex = 8;
            // 
            // btnUpPage
            // 
            this.btnUpPage.Image = global::SP579emulator.Properties.Resources.arrow_Up_32x47LG;
            this.btnUpPage.Location = new System.Drawing.Point(823, 131);
            this.btnUpPage.Name = "btnUpPage";
            this.btnUpPage.Size = new System.Drawing.Size(32, 47);
            this.btnUpPage.TabIndex = 9;
            this.btnUpPage.UseVisualStyleBackColor = true;
            this.btnUpPage.Click += new System.EventHandler(this.btnUpPage_Click);
            // 
            // btnUp
            // 
            this.btnUp.Image = global::SP579emulator.Properties.Resources.arrow_Up_32xLG;
            this.btnUp.Location = new System.Drawing.Point(823, 184);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(32, 32);
            this.btnUp.TabIndex = 10;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Image = global::SP579emulator.Properties.Resources.arrow_Down_32xLG;
            this.btnDown.Location = new System.Drawing.Point(823, 224);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(32, 32);
            this.btnDown.TabIndex = 11;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnDownPage
            // 
            this.btnDownPage.Image = global::SP579emulator.Properties.Resources.arrow_Down_32x47LG;
            this.btnDownPage.Location = new System.Drawing.Point(823, 262);
            this.btnDownPage.Name = "btnDownPage";
            this.btnDownPage.Size = new System.Drawing.Size(32, 47);
            this.btnDownPage.TabIndex = 12;
            this.btnDownPage.UseVisualStyleBackColor = true;
            this.btnDownPage.Click += new System.EventHandler(this.btnDownPage_Click);
            // 
            // Tri_MemoryMap
            // 
            this.AcceptButton = this.btnShowMM;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(867, 421);
            this.Controls.Add(this.btnDownPage);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnUpPage);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnShowMM);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblFrom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Tri_MemoryMap";
            this.Text = "Memory Map";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnShowMM;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnUpPage;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnDownPage;
    }
}