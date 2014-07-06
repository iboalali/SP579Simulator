using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// Register Event Arguments containing event data
    /// </summary>
    public class Tri_MachineStateArgs : EventArgs {

        public int intValue;
        public string hexValue;
        public TextBox txtBox;

        public Tri_MachineStateArgs( TextBox txtBox ) {
            intValue = 99999;
            hexValue = "FFFF";
            this.txtBox = txtBox;
        }

        public Tri_MachineStateArgs( int value, TextBox txtBox ) {
            intValue = value;
            hexValue = value.ToString( "X4" );
            hexValue = hexValue.Substring( hexValue.Length - 4 );
            this.txtBox = txtBox;
        }

        public Tri_MachineStateArgs( string value, TextBox txtBox ) {
            intValue = int.Parse( value, System.Globalization.NumberStyles.HexNumber );
            hexValue = value;
            this.txtBox = txtBox;
        }

        public Tri_MachineStateArgs( bool value, TextBox txtBox ) {
            if ( value ) {
                intValue = 99999;
                hexValue = "FFFF";
            } else {
                intValue = 0;
                hexValue = "0000";
            }

            this.txtBox = txtBox;
        }
    }

    /// <summary>
    /// Represents an 16-bit number in two separate 8-bit number
    /// </summary>
    public class Tri_Word {

        /// <summary>
        /// The Upper Byte of the current word.
        /// </summary>
        public Tri_Byte UpperByte { get; set; }

        /// <summary>
        /// The Lower Byte of the current word.
        /// </summary>
        public Tri_Byte LowerByte { get; set; }

        public delegate void MachineStateEventHandler( object sender, Tri_MachineStateArgs e );
        public event MachineStateEventHandler OnUpdate;

        /// <summary>
        /// The corresponding Text Box in the interface 
        /// </summary>
        public TextBox txtBox;

        /// <summary>
        /// Returns the Lenght of the Word (16 bits)
        /// </summary>
        public int Length { get { return 16; } }

        public static double MAXVALUE { get { return Math.Pow( 2, 16 ); } }


        #region Constructors

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that can hold
        /// 16-bits, which are initially set to the default value true.
        /// </summary>
        public Tri_Word() {
            this.UpperByte = new Tri_Byte();
            this.LowerByte = new Tri_Byte();

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that can hold
        /// 16-bit, which are initially set to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to each bit</param>
        public Tri_Word( bool value ) {
            this.UpperByte = new Tri_Byte( value );
            this.LowerByte = new Tri_Byte( value );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that can hold
        /// 16-bit, which are initially set to the specified value.
        /// Unsiged integer: max = 65535, min = 0.
        /// Signed integer: max = 32767, min = -32768.
        /// </summary>
        /// <param name="value">The value to be set in Decimal</param>
        public Tri_Word( int value ) {
            this.UpperByte = new Tri_Byte();
            this.LowerByte = new Tri_Byte();
            this.Set( value );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that can hold
        /// 16-bit, which are initially set to the specified value.
        /// Max = FFFF, Min = 0000.
        /// </summary>
        /// <param name="value">The value to be set in Hex</param>
        public Tri_Word( string value ) {
            this.UpperByte = new Tri_Byte();
            this.LowerByte = new Tri_Byte();
            this.Set( value );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that can hold
        /// 16-bits, which are specified by two Tri_Byte instances.
        /// </summary>
        /// <param name="UpperByte">The upper byte value that will be cloned to the upper byte from Tri_Word</param>
        /// <param name="lowerByte">The lower byte value that will be cloned to the lower byte from Tri_Word</param>
        public Tri_Word( Tri_Byte UpperByte, Tri_Byte lowerByte ) {
            this.UpperByte = new Tri_Byte( UpperByte.ToIntU() );
            this.LowerByte = new Tri_Byte( lowerByte.ToIntU() );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that is 
        /// a deep clone of another Tri_Word
        /// </summary>
        /// <param name="value">The Tri_Byte to be deep clone</param>
        public Tri_Word( Tri_Word value ) {
            this.UpperByte = new Tri_Byte( value.UpperByte );
            this.LowerByte = new Tri_Byte( value.LowerByte );

        }

        #endregion

        #region Set Methods

        /// <summary>
        /// Gets or Sets the value of the bit at a specific position in the Tri_Word
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value of the bit at a specific position in the Tri_Word</returns>
        public bool this[int index] {
            get {
                if ( index < 8 ) {
                    return this.LowerByte.Get( index );
                } else if ( index < this.Length ) {
                    return this.UpperByte.Get( ( index - 8 ) );
                }

                // !Error
                // index out of range
                return false;
            }
            set {

                if ( index < 8 ) {
                    this.LowerByte.Set( index, value );
                } else if ( index < this.Length ) {
                    this.UpperByte.Set( ( index - 8 ), value );
                } else {
                    // !Error
                    // index out of range
                }

                if ( OnUpdate != null && txtBox != null ) {
                    OnUpdate( this, new Tri_MachineStateArgs( this.ToIntU(), this.txtBox ) );
                }

            }
        }

        /// <summary>
        /// Sets the value of the bit at a specific position in the Tri_Word with
        /// a specified value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set( int index, bool value ) {
            if ( index < 8 ) {
                this.LowerByte.Set( index, value );
            } else if ( index < this.Length ) {
                this.UpperByte.Set( ( index - 8 ), value );
            }

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( this.ToIntU(), this.txtBox ) );
            }

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that is 
        /// a deep clone of another Tri_Word
        /// </summary>
        /// <param name="value">The Tri_Word to be deep clone</param>
        public void Set( Tri_Word value ) {
            this.UpperByte.Set( value.UpperByte );
            this.LowerByte.Set( value.LowerByte );

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( this.ToIntU(), this.txtBox ) );
            }
        }

        /// <summary>
        /// Initialize a new instance of the Tri_Word class that is 
        /// a deep clone of an UpperByte and a LowerByte
        /// </summary>
        /// <param name="UpperByte">The UpperByte to be deep clone</param>
        /// <param name="LowerByte">The LowerByte to be deep clone</param>
        public void Set( Tri_Byte UpperByte, Tri_Byte LowerByte ) {
            this.UpperByte.Set( UpperByte );
            this.LowerByte.Set( LowerByte );

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( this.ToIntU(), this.txtBox ) );
            }
        }

        /// <summary>
        /// Sets the Tri_Word to the specified value. The integer value
        /// will be converted to binary and set to the Tri_Word.
        /// Unsigned integer: max = 65535, min = 0.
        /// Signed integer: max = 32767, min = -32768.
        /// </summary>
        /// <param name="value">The value to be set in Decimal</param>
        public void Set( int value ) {

            // Convert the integer to a string of binary
            string s = Convert.ToString( value, 2 );
            s = s.PadLeft( 16, '0' );
            s = s.Substring( s.Length - 16 );

            //// Check if the string is smaller than 8 numbers
            //if ( s.Length < this.Length ) {
            //    int d = this.Length - s.Length;
            //
            //    // Add zeros to the most significants bits.
            //    // negative numbers will not be affected, because 
            //    // they won't even get through the if statement above.
            //    // They are always 32 bits
            //    for ( int i = 0; i < d; i++ ) {
            //        s = s.Insert( 0, "0" );
            //    }
            //}

            // Convert the string of binarys to an array of integers
            //int[] bits = new int[this.Length];
            //for ( int i = bits.Length - 1, j = s.Length - 1; i >= 0; i--, j-- ) {
            //    bits[i] = Convert.ToInt16( s[j] ) - 48;
            //}


            // Move the binary number to the BitArray
            for ( int i = 0; i < this.Length; i++ ) {

                if ( i < 8 ) {
                    this.UpperByte.Array.Set( this.UpperByte.Length - ( i + 1 ), s[i] == '1' ? true : false );
                } else {
                    this.LowerByte.Array.Set( this.LowerByte.Length - ( i - this.LowerByte.Length + 1 ), s[i] == '1' ? true : false );
                }
            }

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( value, this.txtBox ) );
            }

        }

        /// <summary>
        /// Sets the Tri_Word to the specified value. The string value
        /// will be converted to binary and set to the Tri_Word.
        /// Max = FFFF, Min = 0000.
        /// </summary>
        /// <param name="value">The value to be set in HEX</param>
        public void Set( string value ) {

            // Truncate string that is more than 4 numbers wide
            if ( value.Length > 4 ) {
                value = value.Substring( value.Length - 4 );
                // !Error
                // more than 0xFFFF
            }


            this.Set( int.Parse( value, System.Globalization.NumberStyles.HexNumber ) );

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( value, this.txtBox ) );
            }
        }

        /// <summary>
        /// Sets the references for the text boxes for the Condition Code
        /// </summary>
        /// <param name="txtBox"></param>
        public void SetTextBox( TextBox txtBox ) {
            this.txtBox = txtBox;

        }

        /// <summary>
        /// Sets all bit in the Tri_Word to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to all bits.</param>
        public void SetAll( bool value ) {
            this.UpperByte.SetAll( value );
            this.LowerByte.SetAll( value );

            if ( OnUpdate != null && txtBox != null ) {
                OnUpdate( this, new Tri_MachineStateArgs( value, this.txtBox ) );
            }
        }

        #endregion

        /// <summary>
        /// Gets the value of the bit at a specific position in the Tri_Word
        /// </summary>
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns></returns>
        public bool Get( int index ) {
            if ( index < 8 ) {
                return this.LowerByte.Get( index );
            } else if ( index < this.Length ) {
                return this.UpperByte.Get( ( index - 8 ) );
            }

            return false;
        }



        /// <summary>
        /// Returns a string that represents the bits in the Tri_Word as Binaray
        /// </summary>
        /// <returns>A string that represents the bits in the Tri_Word as Binary</returns>
        public override string ToString() {
            return ( UpperByte.ToString() + LowerByte.ToString() ).PadLeft( 16, '0' );

        }

        /// <summary>
        /// Returns an integer that represents the bits in the Tri_Word
        /// </summary>
        /// <returns>An integer that represents the bits in the Tri_Word</returns>
        public int ToIntU() {
            int i = Convert.ToInt32( this.ToString(), 2 );
            return i;
/*
            int result = 0;
            for ( int i = 0; i < this.Length; i++ ) {

                if ( i < this.UpperByte.Length ) {
                    result += ( this.UpperByte.Get( i ) == true ? 1 : 0 ) *
                                ( int ) Math.Pow( 2, this.Length - ( i + 1 ) );

                } else {
                    result += ( this.LowerByte.Get( ( i - this.UpperByte.Length ) ) == true ? 1 : 0 ) *
                                ( int ) Math.Pow( 2, ( this.Length - ( i + 1 ) ) );

                }

            }

            return result;
            */
        }

        public int ToIntS () {
            return Convert.ToInt16( this.ToString(), 2 );

        }

        /// <summary>
        /// Returns a string that represents the bits in the Tri_Word as HEX
        /// </summary>
        /// <returns>A string that represents the bits in the Tri_Word as HEX</returns>
        public string ToHEX() {
            return this.ToIntU().ToString( "X4" );
        }

        #region Logical Operations

        /// <summary>
        /// Performs a bitwise AND operation on the elements in the current
        /// Tri_Word against the corresponding elements in the specified
        /// Tri_Word
        /// </summary>
        /// <param name="value">The Tri_Word with which to perform the bitwise AND operation.</param>
        /// <returns>A new Tri_Word with the result of the bitwise AND operetion</returns>
        public Tri_Word And( Tri_Word value ) {
            return new Tri_Word(
                this.UpperByte.And( value.UpperByte ),
                this.LowerByte.And( value.LowerByte )
                );
        }

        /// <summary>
        /// Performs a bitwise OR operation on the elements in the current
        /// Tri_Word against the corresponding elements in the specified
        /// Tri_Word
        /// </summary>
        /// <param name="value">The Tri_Word with which to perform the bitwise OR operation.</param>
        /// <returns>A new Tri_Word with the result of the bitwise OR operetion</returns>
        public Tri_Word Or( Tri_Word value ) {
            return new Tri_Word(
                this.UpperByte.Or( value.UpperByte ),
                this.LowerByte.Or( value.LowerByte )
                );
        }

        /// <summary>
        /// Inverts all the bit values in the current Tri_Word, so
        /// that elements set to true are changed to false, and elements set to false
        /// are changed to true.
        /// </summary>
        /// <returns>A new Tri_Word that the NOT operation has been performed on</returns>
        public Tri_Word Not() {
            return new Tri_Word( UpperByte.Not(), LowerByte.Not() );
        
        }

        #endregion

        /// <summary>
        /// Checks if all bits are Zero
        /// </summary>
        /// <returns>Return true if all bits are Zero, false otherwise</returns>
        public bool isZero() {
            return this.UpperByte.isZero() && this.LowerByte.isZero();

        }

        /// <summary>
        /// Checks if the current Tri_Word contains a negative number
        /// </summary>
        /// <returns>Returns true is the current Tri_Word contains a negative number</returns>
        public bool isNegative() {
            return this.ToIntS() < 0;

        }

    }

}
