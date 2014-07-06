using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SP579emulator {

    /// <summary>
    /// Represents an 8-bit number
    /// </summary>
    public class Tri_Byte {
        private Tri_BitArray value;

        public Tri_BitArray Array { get { return value; } }
        public int Length { get { return 8; } }
        public static double MAXVALUE { get { return Math.Pow( 2, 8 ); } }


        /// <summary>
        /// Gets or Sets the value of the bit at a specific position in the Tri_Byte
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value of the bit at a specific position in the Tri_Byte</returns>
        public bool this[int index] {
            get {
                return this.value[index];
            }
            set {
                this.value.Set( index, value );
            }

        }

        

        /// <summary>
        /// Initialize a new instance of the Tri_Byte class that can hold
        /// 8-bits, which are initially set to the default value true.
        /// </summary>
        public Tri_Byte() {
            this.value = new Tri_BitArray( 8, true );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Byte class that can hold
        /// 8-bit, which are initially set to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to each bit</param>
        public Tri_Byte( bool value ) {
            this.value = new Tri_BitArray( 8, value );

        }

        /// <summary>
        /// Initialize a new instance of the Tri_Byte class that is 
        /// a deep clone of another Tri_Byte
        /// </summary>
        /// <param name="value">The Tri_Byte to be deep clone</param>
        public Tri_Byte( Tri_Byte value ) {
            this.value = new Tri_BitArray( 8 );

            for ( int i = 0; i < this.value.Length; i++ )
                this.Set( i, value.Get( i ) );

        }

        /// <summary>
        /// Sets the Array to the specified value. The string value
        /// will be converted to binary and set to the Array.
        /// </summary>
        /// <param name="value">A HEX number</param>
        public Tri_Byte( string value ) {
            this.value = new Tri_BitArray( 8 );
            this.value.Set( value );
        }

        /// <summary>
        /// Sets the Array to the specified value. The integer value
        /// will be converted to binary and set to the Array.
        /// </summary>
        /// <param name="value">An interger number</param>
        public Tri_Byte( int value ) {
            this.value = new Tri_BitArray( 8 );
            this.value.Set( value );
        }

        /// <summary>
        /// Initialize a new instance of the Tri_Byte class that has
        /// values of an Tri_BitArray
        /// </summary>
        /// <param name="value">The Array to be converted to Tri_Byte</param>
        public Tri_Byte( Tri_BitArray value ) {
            this.value = new Tri_BitArray( value );

        }

        /// <summary>
        /// Gets the value of the bit at a specific position in the Tri_Byte
        /// </summary>
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns></returns>
        public bool Get( int index ) {
            return this.value.Get( 7 - index );

        }

        /// <summary>
        /// Sets the value of the bit at a specific position in the Tri_Byte.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get </param>
        /// <param name="value">The Boolean value to assign to the bit</param>
        public void Set( int index, bool value ) {
            this.value.Set( index, value );

        }

        /// <summary>
        /// Sets the Tri_Byte to the specified value. The integer value
        /// will be converted to binary and set to the Tri_Byte.
        /// Unsiged integer: max = 255, min = 0.
        /// Signed integer: max = 128, min = -128.
        /// </summary>
        /// <param name="value">The value to be set in Decimal.</param>
        public void Set( int value ) {
            this.Array.Set( value );
        }

        /// <summary>
        /// Sets the Tri_Byte to the specified value. The integer value
        /// will be converted to binary and set to the Tri_Byte.
        /// Unsiged integer: max = 255, min = 0.
        /// Signed integer: max = 128, min = -128.
        /// </summary>
        /// <param name="Value">The value to be set in Decimal.</param>
        public void Set( Tri_Byte Value ) {
            //this.value = new Tri_BitArray( 8 );

            for ( int i = 0; i < this.value.Length; i++ )
                this.Set( this.value.Length - ( i + 1 ), Value.Get( i ) );
        }

        /// <summary>
        /// Sets the Tri_Byte to the specified value. The string value
        /// will be converted to binary and set to the Tri_Byte
        /// </summary>
        /// <param name="value">The value to be set in HEX</param>
        public void Set( string value ) {
            this.Array.Set( value );
        }

        /// <summary>
        /// Sets all bit in the Tri_Byte to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to all bits.</param>
        public void SetAll( bool value ) {
            this.value.SetAll( value );

        }

        /// <summary>
        /// Performs a bitwise AND operation on the elements in the current
        /// Tri_Byte against the corresponding elements in the specified
        /// Ti_Byte
        /// </summary>
        /// <param name="value">The Tri_Byte with which to perform the bitwise AND operation.</param>
        /// <returns>A new Tri_Byte with the result of the bitwise AND operetion</returns>
        public Tri_Byte And( Tri_Byte value ) {
            return new Tri_Byte( this.value.And( value.Array ) );

        }

        /// <summary>
        /// Performs a bitwise OR operation on the elements in the current
        /// Tri_Byte against the corresponding elements in the specified
        /// Ti_Byte
        /// </summary>
        /// <param name="value">The Tri_Byte with which to perform the bitwise OR operation.</param>
        /// <returns>A new Tri_Byte with the result of the bitwise OR operetion</returns>
        public Tri_Byte Or( Tri_Byte value ) {
            return new Tri_Byte( this.value.Or( value.Array ) );
        }

        /// <summary>
        /// Return the integer that represent the bits in the Tri_Byte as a decimal number.
        /// </summary>
        /// <returns>An integer that represent the bits in the Tri_Byte as a decimal number.</returns>
        public int ToIntU() {
            return this.Array.ToIntU();

        }

        public int ToIntS () {
            int result = 0;
            for ( int i = 0; i < this.Length; i++ ) {
                result += ( this.Get( i ) == true ? 1 : 0 ) * ( int ) Math.Pow( 2, this.Length - ( i + 1 ) );
            }

            if ( this.Get( 0 ) ) {
                result = ( int ) Math.Pow( 2, this.Length ) - result - 1;
                result *= -1;
            }

            return result;

        }

        /// <summary>
        /// Returns a string that represents the bits in the Array
        /// </summary>
        /// <returns>A string that represents the bits in the Array</returns>
        public override string ToString() {
            return Array.ToString().PadLeft( 8, '0' );

        }

        /// <summary>
        /// Returns a HEX number as a string that represents the bits in the Array.
        /// </summary>
        /// <returns>A HEX number as a string that represents the bits in the Array</returns>
        public string ToHEX() {
            return this.Array.ToHEX().PadLeft( 2, '0' );
        }

        /// <summary>
        /// Inverts all the bit values in the current Tri_Byte, so
        /// that elements set to true are changed to false, and elements set to false
        /// are changed to true.
        /// </summary>
        /// <returns>A new Tri_Byte that the NOT operation has been performed on</returns>
        public Tri_Byte Not() {
            return new Tri_Byte( Array.Not() );
        
        }

        /// <summary>
        /// Checks if all bits are Zero
        /// </summary>
        /// <returns>Return true if all bits are Zero, false otherwise</returns>
        public bool isZero() {
            for ( int i = 0; i < this.Length; i++ ) {
                if ( this.Get(i) ) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the current Tri_Byte contains a negative number
        /// </summary>
        /// <returns>Returns true is the current Tri_Byte contains a negative number</returns>
        public bool isNegative() {
            int a = this.ToIntS();
            return a < 0;
        }


    }

}