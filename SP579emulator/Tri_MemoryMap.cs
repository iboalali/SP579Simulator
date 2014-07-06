using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {
    public partial class Tri_MemoryMap : Form {
        Tri_Byte[] MemoryMap;
        int pos = 0;
        public Tri_MemoryMap( ref Tri_Byte[] MemoryMap ) {
            Icon = global::SP579emulator.Properties.Resources.app_icon;
            this.MemoryMap = MemoryMap;
            this.KeyPreview = true;
            this.KeyDown += Tri_MemoryMap_KeyDown;

            InitializeComponent();

            dataGridView1.ColumnCount = 16;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.RowHeadersWidth = 25;
            
            dataGridView1.Columns[0].Name = "00";
            dataGridView1.Columns[1].Name = "01";
            dataGridView1.Columns[2].Name = "02";
            dataGridView1.Columns[3].Name = "03";
            dataGridView1.Columns[4].Name = "04";
            dataGridView1.Columns[5].Name = "05";
            dataGridView1.Columns[6].Name = "06";
            dataGridView1.Columns[7].Name = "07";
            dataGridView1.Columns[8].Name = "08";
            dataGridView1.Columns[9].Name = "09";
            dataGridView1.Columns[10].Name = "0A";
            dataGridView1.Columns[11].Name = "0B";
            dataGridView1.Columns[12].Name = "0C";
            dataGridView1.Columns[13].Name = "0D";
            dataGridView1.Columns[14].Name = "0E";
            dataGridView1.Columns[15].Name = "0F";
            
            updateCells( Tri_MainWindow.memoryPositon );
            textBox1.Text = Tri_MainWindow.memoryPositon.ToString( "X4" );
            pos = Tri_MainWindow.memoryPositon;
             
        }

        void Tri_MemoryMap_KeyDown( object sender, KeyEventArgs e ) {
            // Hot keys handel
            if ( e.KeyCode == Keys.Down ) {
                pos += 16;
                if ( pos > MemoryMap.Length - 256 ) {
                    pos = MemoryMap.Length - 256;
                }
                e.SuppressKeyPress = true;  // stops bing! also sets handeled which stop event bubbling

            } else if ( e.KeyCode == Keys.PageDown ) {
                pos += 256;
                if ( pos > MemoryMap.Length - 256 ) {
                    pos = MemoryMap.Length - 256;
                }
                e.SuppressKeyPress = true;  // stops bing! also sets handeled which stop event bubbling

            } else if ( e.KeyCode == Keys.Up ) {
                pos -= 16;
                if ( pos < 0 ) {
                    pos = 0;
                }
                e.SuppressKeyPress = true;  // stops bing! also sets handeled which stop event bubbling

            } else if ( e.KeyCode == Keys.PageUp ) {
                pos -= 256;
                if ( pos < 0 ) {
                    pos = 0;
                }
                e.SuppressKeyPress = true;  // stops bing! also sets handeled which stop event bubbling

            }
            updateCells( pos );


        }

        private void btnShowMM_Click( object sender, EventArgs e ) {
            if ( textBox1.Text != string.Empty ) {
                pos = int.Parse( textBox1.Text, System.Globalization.NumberStyles.HexNumber );
                if ( pos > 0xFFFF || pos < 0 ) {
                    System.Media.SystemSounds.Asterisk.Play();
                    return;
                }
                if ( pos > MemoryMap.Length - 256 ) {
                    pos = MemoryMap.Length - 256;
                }
                pos /= 16;
                pos *= 16;
            }

            updateCells( pos );

        }

        private void btnClose_Click( object sender, EventArgs e ) {
            Close();
        }

        /// <summary>
        /// Updates the Cells with the correct information at the specified position
        /// </summary>
        /// <param name="posInMemory"></param>
        private void updateCells( int posInMemory ) {
            Tri_MainWindow.memoryPositon = posInMemory;
            dataGridView1.Rows.Clear();
            string[] num = new string[16];
            //272
            int size = posInMemory + 256;
            for ( int i = posInMemory; i < size; ) {
                for ( int j = 0; j < num.Length; j++, i++ ) {
                    try {
                        num[j] = MemoryMap[i].ToIntU().ToString( "X2" );
                    } catch {

                        return;
                    }


                }

                dataGridView1.Rows.Add( num );
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].HeaderCell.Value = ( i - 1 ).ToString( "X4" )
                                                                                             .Insert( 3, "0" )
                                                                                             .Remove( 4 );
                dataGridView1.AutoScrollOffset = new Point( 0, 200 );


            }

        }

        private void btnUpPage_Click( object sender, EventArgs e ) {
            pos -= 256;
            if ( pos < 0 ) {
                pos = 0;
            }

            updateCells( pos );
        }

        private void btnUp_Click( object sender, EventArgs e ) {
            pos -= 16;
            if ( pos < 0 ) {
                pos = 0;
            }

            updateCells( pos );
        }

        private void btnDownPage_Click( object sender, EventArgs e ) {
            pos += 256;
            if ( pos > MemoryMap.Length - 256 ) {
                pos = MemoryMap.Length - 256;
            }

            updateCells( pos );
        }

        private void btnDown_Click( object sender, EventArgs e ) {
            pos += 16;
            if ( pos > MemoryMap.Length - 256 ) {
                pos = MemoryMap.Length - 256;
            }

            updateCells( pos );
        }


    }
}
