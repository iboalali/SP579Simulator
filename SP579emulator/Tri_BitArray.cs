using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP579emulator {

    /// <summary>
    /// Big Endian BitArray that is agnostic to the platforms it performs on
    /// </summary>
    public class Tri_BitArray {
        BitArray value;

        /// <summary>
        /// Gets the number of elements in the Array.
        /// </summary>
        public int Length { get { return value.Length; } }

        /// <summary>
        /// Gets the underlying BitArray
        /// </summary>
        public BitArray bitArray { get { return value; } }

        /// <summary>
        /// Initialize a new instance of the Array class that can hold
        /// the specified number of bit values, which are initially set to
        /// the default value true.
        /// </summary>
        /// <param name="length">The number of bit values int the new Array</param>
        public Tri_BitArray( int length ) {
            this.value = new BitArray( length );
            
        }

        /// <summary>
        /// Initialize a new instance of the Array class that can hold
        /// the specified number of bit values, which are initially set to
        /// the specified value.
        /// </summary>
        /// <param name="length">The number of bit values in the new Array</param>
        /// <param name="setAll">The Boolean value to assign to each bit</param>
        public Tri_BitArray( int length, bool setAll ) {
            this.value = new BitArray( length, setAll );

        }

        /// <summary>
        /// Initialize a new instance of the Array class that has
        /// values of an System.Colletions.BitArray
        /// </summary>
        /// <param name="value">The BitArray to be converted to Array</param>
        public Tri_BitArray( BitArray value ) {
            this.value = new BitArray( value.Length );

            // Check the endianess of this computer
            if ( BitConverter.IsLittleEndian ) {
                for ( int i = 0; i < value.Length; i++ ) {
                    this.value.Set( i, value.Get( value.Length - ( i + 1 ) ) );
                }
            } else {
                for ( int i = 0; i < value.Length; i++ ) {
                    this.value.Set( i, value.Get( i ) );
                }
            }

        }

        /// <summary>
        /// Initialize a new instance of the Array class that is 
        /// a deep clone of another Array
        /// form another Array
        /// </summary>
        /// <param name="value">The Array to be deep clone</param>
        public Tri_BitArray( Tri_BitArray value ) {
            // Deep clone (copy)
            this.value = new BitArray( value.Length );
            for ( int i = 0; i < value.Length; i++ ) {
                this.value.Set( i, value.Get( i ) );
            }

        }

        /// <summary>
        /// Gets or Sets the value of the bit at a specific position in the Array
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value of the bit at a specific position in the Array</returns>
        public bool this[int index] {
            get {
                return this.value[( this.Length - 1 ) - index];
            }

            set {
                // Check the endianess of this computer
                if ( BitConverter.IsLittleEndian ) {
                    this.value.Set( this.value.Length - ( index + 1 ), value );
                } else {
                    this.value.Set( index, value );
                }
            }

        }

        /// <summary>
        /// Gets the value of the bit at a specific position in the Array
        /// </summary>
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns></returns>
        public bool Get( int index ) {
            return this.value.Get( ( this.Length - 1 ) - index );

        }

        /// <summary>
        /// Sets the value of the bit at a specific position in the Array
        /// </summary>
        /// <param name="index">The zero-based index of the value to get </param>
        /// <param name="value">The Boolean value to assign to the bit</param>
        public void Set( int index, bool value ) {

            // Check the endianess of this computer
            if ( BitConverter.IsLittleEndian ) {
                this.value.Set( this.value.Length - ( index + 1 ), value );
            } else {
                this.value.Set( index, value );
            }

        }

        /// <summary>
        /// Sets the Array to the specified value. The integer value
        /// will be converted to binary and set to the Array.
        /// </summary>
        /// <param name="value">The value to be set in Decimal</param>
        public void Set( int value ) {

            // Convert the integer to a string of binary
            string s = Convert.ToString( value, 2 ).PadLeft( 8, '0' );
            s = s.Substring( s.Length - 8 ); 
            //// Check if the string is smaller than 8 numbers
            //if ( s.Length < this.value.Length ) {
            //    int d = this.value.Length - s.Length;
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
            //int[] bits = new int[this.value.Length];
            //for ( int i = bits.Length - 1, j = s.Length - 1; i >= 0; i--, j-- ) {
            //    bits[i] = Convert.ToInt16( s[j] ) - 48;
            //}

            // Check the endianess of this computer
            if ( BitConverter.IsLittleEndian ) {

                // Move the binary number to the BitArray
                for ( int i = 0; i < this.value.Length; i++ ) {
                    this.value.Set( i, s[i] == '1' ? true : false );

                }
            } else {
                // Move the binary number to the BitArray
                for ( int i = 0; i < this.value.Length; i++ ) {
                    this.value.Set( ( this.value.Length - ( i + 1 ) ), s[i] == '1' ? true : false );
                }
            }

        }

        /// <summary>
        /// Sets all bit in the Array to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to all bits.</param>
        public void SetAll( bool value ) {
            this.value.SetAll( value );
            
        }

        /// <summary>
        /// Sets the Array to the specified value. The string value
        /// will be converted to binary and set to the Array.
        /// Max = FF, Min = 00.
        /// </summary>
        /// <param name="value">The value to be set in HEX</param>
        public void Set( string value ) {

            // Truncate string that is more than 2 numbers wide
            if ( value.Length > 2 ) {
                value = value.Substring( value.Length - 2 );
                // !Error
                // more than 0xFF
            }

            this.Set( int.Parse( value, System.Globalization.NumberStyles.HexNumber ) );


        }

        /// <summary>
        /// Performs a bitwise AND operation on the elements in the current
        /// Array against the corresponding elements in the specified
        /// Ti_BitArray
        /// </summary>
        /// <param name="value">The Array with which to perform the bitwise AND operation.</param>
        /// <returns>A new Array with the result of the bitwise AND operetion</returns>
        public Tri_BitArray And( Tri_BitArray value ) {

            // Checks which Array is shorter and use its value
            int length = this.value.Length <= value.Length ? this.value.Length : value.Length;
            Tri_BitArray temp = new Tri_BitArray( Length );

            // Go through AND's truth table
            for ( int i = 0; i < Length; i++ ) {
                if ( this.value.Get( i ) && value.Get( i ) ) {
                    temp.bitArray.Set( i, true );
                } else {
                    temp.bitArray.Set( i, false );
                }
            }

            return temp;
        }

        /// <summary>
        /// Performs a bitwise OR operation on the elements in the current
        /// Array against the corresponding elements in the specified
        /// Ti_BitArray
        /// </summary>
        /// <param name="value">The Array with which to perform the bitwise OR operation</param>
        /// <returns>A new Array with the result of the bitwise OR operetion</returns>
        public Tri_BitArray Or( Tri_BitArray value ) {
            // Checks which Array is shorter and use its value
            int length = this.value.Length <= value.Length ? this.value.Length : value.Length;
            Tri_BitArray temp = new Tri_BitArray( Length );

            // Go through OR's truth table
            //for ( int i = 0; i < Length; i++ ) {
            //    if ( this.value.Get( i ) || value.Get( i ) ) {
            //        temp.bitArray.Set( i, true );
            //    } else {
            //        temp.bitArray.Set( i, false );
            //    }
            //}

            return new Tri_BitArray( this.bitArray.Or( value.bitArray ) );
        }

        /// <summary>
        /// Returns a string that represents the bits in the Array
        /// </summary>
        /// <returns>A string that represents the bits in the Array</returns>
        public override string ToString() {
            string s = string.Empty;
            foreach ( var item in value ) {
                s += Convert.ToInt16( item );
            }
            return s;
        }

        /// <summary>
        /// Return the integer that represent the bits in the UpperByte as a decimal number.
        /// </summary>
        /// <returns>An integer that represent the bits in the UpperByte as a decimal number.</returns>
        public int ToIntU() {
            int result = 0;
            for ( int i = 0; i < this.bitArray.Length; i++ ) {
                result += ( this.bitArray.Get( i ) == true ? 1 : 0 ) *
                                ( int ) Math.Pow( 2, this.bitArray.Length - ( i + 1 ) );
            }

            return result;
        }

        /// <summary>
        /// Returns a HEX number as a string that represents the bits in the Array.
        /// </summary>
        /// <returns>A HEX number as a string that represents the bits in the Array</returns>
        public string ToHEX() {
            return ToIntU().ToString( "X" );
        }

        /// <summary>
        /// Inverts all the bit values in the current Tri_BitArray, so
        /// that elements set to true are changed to false, and elements set to false
        /// are changed to true.
        /// </summary>
        /// <returns>A new Tri_BitArray that the NOT operation has been performed on</returns>
        public Tri_BitArray Not() {
            /*
            Tri_BitArray temp = new Tri_BitArray( this.Length );
            for ( int i = 0; i < this.Length; i++ ) {
                temp.Set( i, this.value.Get( i ) == true ? false : true );

            }
             */
            return new Tri_BitArray( this.bitArray.Not() );
        }

    }
}
