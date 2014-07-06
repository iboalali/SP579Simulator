using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// Stack Pointer Event Arguments containing event data
    /// </summary>
    public class Tri_StackPointerArgs : EventArgs {
        public int intValue;
        public string hexValue;
        public TextBox txtBox;
        public int IntMinValue { get { return 0xFC00; } }
        public int IntMaxValue { get { return 0xFFFF; } }
        public string StringMinValue { get { return "FC00"; } }
        public string StringMaxValue { get { return "FFFF"; } }

        public Tri_StackPointerArgs( TextBox txtBox ) {
            intValue = 0;
            hexValue = "0000";
            this.txtBox = txtBox;
        }

        public Tri_StackPointerArgs( int value, TextBox txtBox ) {
            intValue = value;
            hexValue = value.ToString( "X4" );
            this.txtBox = txtBox;
        }

        public Tri_StackPointerArgs( string value, TextBox txtBox ) {
            intValue = int.Parse( value, System.Globalization.NumberStyles.HexNumber );
            hexValue = value;
            this.txtBox = txtBox;
        }

    
    }

    /// <summary>
    /// Stack Pointer Class
    /// </summary>
    public class Tri_StackPointer {
        private int sp;
        public TextBox txtBox;
        public delegate void StackPointerEventHandler( object sender, Tri_MachineStateArgs e );
        public event StackPointerEventHandler OnUpdate;

        /// <summary>
        /// Gets or Sets the value of the Stack Pointer
        /// </summary>
        public int Value {
            get { 
                return sp; 
            }
            set {
                sp = value;

                if ( OnUpdate != null && txtBox != null ) {
                    OnUpdate( this, new Tri_MachineStateArgs( value, this.txtBox ) );
                }
            }
        }

        
        /// <summary>
        /// Sets the references for the text boxes for the Stack Pointer
        /// </summary>
        /// <param name="txtBox"></param>
        public void SetTextBox( TextBox txtBox ) {
            this.txtBox = txtBox;
        }



    }


}
