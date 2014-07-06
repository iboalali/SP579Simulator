using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// CCR Event Arguments containing event data
    /// </summary>
    public class Tri_CCRArgs : EventArgs {
        public bool value;
        public TextBox txtBox;
        public CCRflags flag;

        public Tri_CCRArgs( TextBox txtBox, CCRflags flag ) {
            value = false;
            this.txtBox = txtBox;
            this.flag = flag;
        }

        public Tri_CCRArgs( bool value, TextBox txtBox, CCRflags flag ) {
            this.value = value;
            this.txtBox = txtBox;
            this.flag = flag;
        }


    }

    /// <summary>
    /// CCR flags
    /// </summary>
    public enum CCRflags{
        Zero = 0,
        Overflow = 1,
        Negative = 2,
        Carry = 3

    }

    /// <summary>
    /// Condition Code Register Class
    /// </summary>
    public class Tri_CCR {
        bool[] ccr;
        TextBox[] txtCCR;

        public delegate void CCREventHandler( object sender, Tri_CCRArgs e );
        public event CCREventHandler OnUpdate;

        public Tri_CCR() {
            ccr = new bool[4];
            for ( int i = 0; i < ccr.Length; i++ ) {
                ccr[i] = false;
            }
            txtCCR = new TextBox[4];

        }

        /// <summary>
        /// Sets the value of a specified Condition Code
        /// </summary>
        /// <param name="value">The value to be set</param>
        /// <param name="flag">The Condition Code to set the value to</param>
        public void SetValue( bool value, CCRflags flag ) {
            this.ccr[( int ) flag] = value;
            if ( value ) {
                this.txtCCR[( int ) flag].Text = "1";
            } else {
                this.txtCCR[( int ) flag].Text = "0";
            }
            //if ( OnUpdate != null && txtCCR != null ) {
            //    if ( txtCCR[( int ) flag] != null ) {
            //        OnUpdate( this, new Tri_CCRArgs( value, this.txtCCR[( int ) flag], flag ) );
            //    }
            //}
        }

        /// <summary>
        /// Set a Condition Code to One
        /// </summary>
        /// <param name="flag">Condition Code</param>
        public void Set( CCRflags flag ) {
            ccr[( int ) flag] = true;
            if ( OnUpdate != null && txtCCR != null ) {
                if ( txtCCR[( int ) flag] != null ) {
                    OnUpdate( this, new Tri_CCRArgs( true, this.txtCCR[( int ) flag], flag ) );
                }
            }

        }

        /// <summary>
        /// Clear a Condition Code to Zero
        /// </summary>
        /// <param name="flag">Condition Code</param>
        public void Clear( CCRflags flag ) {
            ccr[( int ) flag] = false;
            if ( OnUpdate != null && txtCCR != null ) {
                if ( txtCCR[( int ) flag] != null ) {
                    OnUpdate( this, new Tri_CCRArgs( false, this.txtCCR[( int ) flag], flag ) );
                }
            }

        }

        /// <summary>
        /// Gets the value of the Condition Code 
        /// </summary>
        /// <param name="flag">Condition Code</param>
        /// <returns></returns>
        public bool Get( CCRflags flag ) {
            return ccr[( int ) flag];
        }

        /// <summary>
        /// Sets the references for the text boxes for the Condition Code
        /// </summary>
        /// <param name="txtBox"></param>
        public void SetTextBox( TextBox[] txtBox ) {
            if ( txtBox.Length < 4 ) {
                return;
            }
            for ( int i = 0; i < ccr.Length; i++ ) {
                txtCCR[i] = txtBox[i];
            }

        }

    }
}
