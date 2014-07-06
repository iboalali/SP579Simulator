using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// Calculates the checksum of a record
    /// </summary>
    public partial class Tri_CheckSumCalc : Form {
        public Tri_CheckSumCalc() {
            Icon = global::SP579emulator.Properties.Resources.app_icon;

            InitializeComponent();
        }

        private void btnCalc_Click( object sender, EventArgs e ) {
            txtResult.Text = calculateCheckSum( txtRecord.Text );
            txtRecord.Text = string.Empty;
        }

        /// <summary>
        /// Calculates the checksum of a record
        /// </summary>
        /// <param name="rec">An Hex value in a string that is two charecters wide</param>
        /// <returns></returns>
        private string calculateCheckSum( string rec ) {

            string[] record = rec.Split( new char[] { '*', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Adding the number together
            int result = int.Parse( record[1], System.Globalization.NumberStyles.HexNumber );
            result += int.Parse( record[2], System.Globalization.NumberStyles.HexNumber );
            string hex;

            if ( record[0] == "2" ) {
                for ( int i = 0; i < record[3].Length; i += 2 ) {
                    hex = record[3].Substring( i, 2 );
                    result += int.Parse( hex, System.Globalization.NumberStyles.HexNumber );

                }
            }

            // Wrap around
            hex = result.ToString( "X" );
            string wrapAround = string.Empty;
            while ( hex.Length > 2 ) {
                wrapAround = hex.Substring( 0, hex.Length - 2 );
                hex = hex.Substring( wrapAround.Length );

                result = int.Parse( hex, System.Globalization.NumberStyles.HexNumber );
                result += int.Parse( wrapAround, System.Globalization.NumberStyles.HexNumber );
                hex = result.ToString( "X" );
            }

            // One's compliment
            hex = result.ToString( "X" );

            result = int.Parse( hex[0].ToString(), System.Globalization.NumberStyles.HexNumber );
            result = 15 - result;

            hex = result.ToString("X") + hex[1];

            result = int.Parse( hex[1].ToString(), System.Globalization.NumberStyles.HexNumber );
            result = 15 - result;

            hex = hex[0] + result.ToString("X");

            return hex;

        }

        private void txtResult_KeyPress( object sender, KeyPressEventArgs e ) {
            e.Handled = true;
        }

        private void button1_Click( object sender, EventArgs e ) {
            this.Close();
        }
    }
}
