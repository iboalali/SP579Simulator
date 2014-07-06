using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// Displays the log file in a text box
    /// </summary>
    public partial class Tri_Application_Log : Form {

        public Tri_Application_Log( string appLog ) {
            this.Icon = global::SP579emulator.Properties.Resources.app_icon;
            InitializeComponent();

            richTextBox1.Text = appLog;
        }

        private void button1_Click( object sender, EventArgs e ) {
            this.Close();
        }
    }
}
