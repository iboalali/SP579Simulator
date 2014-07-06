using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {

    /// <summary>
    /// A Static class for all instruction that have direct access to the Registers and Memory
    /// </summary>
    public class Tri_Instructions {

        public static Tri_Byte[] MemoryMap;
        public static Tri_Word[] CPUregisters = new Tri_Word[8];
        public static Tri_Word PC;
        public static Tri_StackPointer SP;
        public static Tri_CCR CCR;

        public static Tri_MainWindow form;

        public static bool finished = false;

        #region Branch Instruction

        public static void BRA ( Tri_ILU nameX ) {
            int destination;
            switch ( nameX.operationSize ) {
                case 0:
                    //Program counter relative mode
                    //PC = PC + offset

                    //destination = PC.ToIntU() + ( int ) ( Math.Pow( 2, 10 ) - nameX.addressOrImm16OrOffset );

                    destination = PC.ToIntU() - nameX.addressOrImm16OrOffset;
                    if ( ( destination < 0 ) || ( destination > 0xFFFF ) ) {    
                        //ERROR
                        form.errorMessage( 5, nameX.lineNumber ); // Address out of range
                        MessageBox.Show( "Address Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;

                    } else if ( destination > 0xFBFF ) {
                        //ERROR
                        form.errorMessage( 6, nameX.lineNumber ); // Address Within Stack Range
                        MessageBox.Show( "Address Within Stack Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;

                    } else
                        PC.Set( destination );

                    break;
                case 1:
                    //Direct addressing mode
                    //PC = address
                    if ( ( nameX.addressOrImm16OrOffset < 0 ) || ( nameX.addressOrImm16OrOffset > 0xFFFF ) ) {
                        //ERROR
                        form.errorMessage( 5, nameX.lineNumber ); // Address out of range
                        MessageBox.Show( "Address Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    } else if ( nameX.addressOrImm16OrOffset > 0xFBFF ) {
                        //ERROR
                        form.errorMessage( 6, nameX.lineNumber ); // Address Within Stack Range
                        MessageBox.Show( "Address Within Stack Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    } else
                        PC.Set( nameX.addressOrImm16OrOffset );
                    break;

            }
        }

        public static void BEQ ( Tri_ILU nameX ) {
            if ( CCR.Get( CCRflags.Zero ) )
                Tri_Instructions.BRA( nameX );
        }

        public static void BNE ( Tri_ILU nameX ) {
            if ( !CCR.Get( CCRflags.Zero ) )
                Tri_Instructions.BRA( nameX );
        }

        public static void BCS ( Tri_ILU nameX ) {
            if ( CCR.Get( CCRflags.Carry ) )
                Tri_Instructions.BRA( nameX );
        }
        
        public static void BLT ( Tri_ILU nameX ) {
            if ( ( CCR.Get( CCRflags.Overflow ) ) ^ ( CCR.Get( CCRflags.Negative ) ) )
                Tri_Instructions.BRA( nameX );
        }

        #endregion

        #region Miscellaneous

        public static void HLT () {
            finished = true;
            MessageBox.Show( "HLT Instruction", "Reached an HLT instruction. Execution has been stopped",
                MessageBoxButtons.OK, MessageBoxIcon.Stop );
        }

        #endregion

        #region Data Movement Instruction

        public static void LD ( Tri_ILU nameX ) {
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            int rs = CPUregisters[nameX.sourceReg].LowerByte.ToIntU() + nameX.addressOrImm16OrOffset;
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte, MemoryMap[rs] );

                            break;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.sourceReg].LowerByte );

                            break;

                        // Immediate Addressing
                        case 2:
                            //if ( nameX.addressOrImm16OrOffset < 0 ) {
                            //    nameX.addressOrImm16OrOffset = ( int ) Tri_Byte.MAXVALUE + nameX.addressOrImm16OrOffset;
                            //}

                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( nameX.addressOrImm16OrOffset ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte,
                                MemoryMap[nameX.sourceReg] );

                            break;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:

                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            int rs = CPUregisters[nameX.sourceReg].ToIntU() + nameX.addressOrImm16OrOffset;
                            CPUregisters[nameX.destinationReg].Set( MemoryMap[rs], MemoryMap[rs + 1] );

                            break;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.sourceReg] );

                            break;

                        // Immediate Addressing
                        case 2:
                            //if ( nameX.addressOrImm16OrOffset < 0 ) {
                            //    nameX.addressOrImm16OrOffset = ( int ) Tri_Word.MAXVALUE + nameX.addressOrImm16OrOffset;
                            //}
                            CPUregisters[nameX.destinationReg].Set( nameX.addressOrImm16OrOffset );

                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set(
                                MemoryMap[nameX.addressOrImm16OrOffset], MemoryMap[nameX.addressOrImm16OrOffset + 1] );

                            break;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }
        }

        public static void STR ( Tri_ILU nameX ) {
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            MemoryMap[CPUregisters[nameX.destinationReg].ToIntU() + nameX.addressOrImm16OrOffset]
                            .Set( CPUregisters[nameX.sourceReg].LowerByte );

                            break;

                        case 1:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            MemoryMap[nameX.addressOrImm16OrOffset].Set( CPUregisters[nameX.sourceReg].LowerByte );

                            break;

                        default:
                            break;
                    }

                    break;

                // Size: Word
                case 1:
                    int addr;
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            addr = CPUregisters[nameX.destinationReg].ToIntU() + nameX.addressOrImm16OrOffset;
                            MemoryMap[addr].Set( CPUregisters[nameX.sourceReg].UpperByte );
                            MemoryMap[addr + 1].Set( CPUregisters[nameX.sourceReg].LowerByte );
                            break;

                        case 1:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            MemoryMap[nameX.addressOrImm16OrOffset].Set( CPUregisters[nameX.sourceReg].UpperByte );
                            MemoryMap[nameX.addressOrImm16OrOffset + 1].Set( CPUregisters[nameX.sourceReg].LowerByte );

                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }

        public static void LSP ( Tri_ILU nameX ) {
            switch ( nameX.operationSize ) {
                case 0:
                    // ERROR
                    form.errorMessage( 15, nameX.lineNumber ); // Wrong Operation Size
                    MessageBox.Show( "Wrong Operation Size", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    finished = true;
                    return;
                case 1:
                    switch ( nameX.addressingMode ) {

                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            int addr = CPUregisters[nameX.sourceReg].ToIntU();
                            if ( addr < 0xFC00 || addr > 0xFFFF ) {
                                form.errorMessage( 19, nameX.lineNumber ); // Outside Stack Range
                                MessageBox.Show( "Stack is Outside of it's Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            SP.Value = addr;

                            break;

                        // Immediate Addressing
                        case 2:
                            if ( nameX.addressOrImm16OrOffset < 0 )
                                nameX.addressOrImm16OrOffset = ( int ) Tri_Word.MAXVALUE + nameX.addressOrImm16OrOffset;

                            if ( nameX.addressOrImm16OrOffset < 0xFC00 || nameX.addressOrImm16OrOffset > 0xFFFF ) {
                                form.errorMessage( 19, nameX.lineNumber ); // Outside Stack Range
                                MessageBox.Show( "Stack is Outside of it's Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            SP.Value = nameX.addressOrImm16OrOffset;

                            break;

                        // Absolute Addressing
                        case 3:
                            int w = new Tri_Word( MemoryMap[nameX.addressOrImm16OrOffset],
                                MemoryMap[nameX.addressOrImm16OrOffset + 1] ).ToIntU();
                            SP.Value = w;
                            break;

                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }


        }

        public static void EXG ( Tri_ILU nameX ) {
            int temp;
            int addr;

            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            addr = CPUregisters[nameX.destinationReg].ToIntU() + nameX.addressOrImm16OrOffset;
                            temp = CPUregisters[nameX.sourceReg].ToIntU();

                            CPUregisters[nameX.sourceReg].Set(
                                CPUregisters[nameX.sourceReg].UpperByte, MemoryMap[addr] );
                            MemoryMap[addr].Set( temp );

                            break;

                        case 1:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            temp = CPUregisters[nameX.sourceReg].ToIntU();
                            CPUregisters[nameX.sourceReg].Set(
                                CPUregisters[nameX.sourceReg].UpperByte, MemoryMap[nameX.addressOrImm16OrOffset] );

                            MemoryMap[nameX.addressOrImm16OrOffset].Set( temp );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    Tri_Word t;
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            addr = CPUregisters[nameX.destinationReg].ToIntU() + nameX.addressOrImm16OrOffset;
                            temp = CPUregisters[nameX.sourceReg].ToIntU();

                            CPUregisters[nameX.sourceReg].Set( MemoryMap[addr], MemoryMap[addr + 1] );
                            t = new Tri_Word( temp );
                            MemoryMap[addr].Set( t.UpperByte );
                            MemoryMap[addr + 1].Set( t.LowerByte );

                            break;

                        case 1:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            temp = CPUregisters[nameX.sourceReg].ToIntU();
                            CPUregisters[nameX.sourceReg].Set( MemoryMap[nameX.addressOrImm16OrOffset], MemoryMap[nameX.addressOrImm16OrOffset + 1] );

                            t = new Tri_Word( temp );
                            MemoryMap[nameX.addressOrImm16OrOffset].Set( t.UpperByte );
                            MemoryMap[nameX.addressOrImm16OrOffset + 1].Set( t.LowerByte );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;

            }

        }

        #endregion

        #region Logical Instruction

        public static void AND ( Tri_ILU nameX ) {
            Tri_Word temp;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.destinationReg].LowerByte.And( CPUregisters[nameX.sourceReg].LowerByte ) );
                            break;

                        // Immediate Addressing
                        case 2:
                            //if ( nameX.addressOrImm16OrOffset < 0 )
                            //    nameX.addressOrImm16OrOffset = ( int ) Tri_Word.MAXVALUE + nameX.addressOrImm16OrOffset;

                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                               CPUregisters[nameX.destinationReg].LowerByte.And( temp.LowerByte ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.destinationReg].LowerByte.And( MemoryMap[nameX.addressOrImm16OrOffset] ) );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].And(
                                CPUregisters[nameX.sourceReg] ) );


                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].And(
                                temp ) );


                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte.And(
                                MemoryMap[nameX.addressOrImm16OrOffset] ),
                                CPUregisters[nameX.destinationReg].LowerByte.And(
                                MemoryMap[nameX.addressOrImm16OrOffset + 1] )
                                );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }





        }

        public static void OR ( Tri_ILU nameX ) {
            Tri_Word temp;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;


                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.destinationReg].LowerByte.Or( CPUregisters[nameX.sourceReg].LowerByte ) );

                            break;

                        // Immediate Addressing
                        case 2:
                            //if ( nameX.addressOrImm16OrOffset < 0 )
                            //    nameX.addressOrImm16OrOffset = ( int ) Tri_Word.MAXVALUE + nameX.addressOrImm16OrOffset;

                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                               CPUregisters[nameX.destinationReg].LowerByte.Or( temp.LowerByte ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.destinationReg].LowerByte.Or( MemoryMap[nameX.addressOrImm16OrOffset] ) );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].Or(
                                CPUregisters[nameX.sourceReg] ) );


                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].Or(
                                temp ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte.Or(
                                MemoryMap[nameX.addressOrImm16OrOffset] ),
                                CPUregisters[nameX.destinationReg].LowerByte.Or(
                                MemoryMap[nameX.addressOrImm16OrOffset + 1] )
                                );
                            break;

                        default:
                            break;

                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }


        }

        public static void NOT ( Tri_ILU nameX ) {
            Tri_Word temp;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                CPUregisters[nameX.sourceReg].LowerByte.Not() );

                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                temp.LowerByte.Not() );


                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                MemoryMap[nameX.addressOrImm16OrOffset].Not() );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.sourceReg].Not() );

                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set( temp.Not() );


                            break;

                        // Absolute Addressing
                        case 3:
                            CPUregisters[nameX.destinationReg].Set(
                                MemoryMap[nameX.addressOrImm16OrOffset].Not(),
                                MemoryMap[nameX.addressOrImm16OrOffset + 1].Not() );
                            break;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Stack

        public static void PUSH ( Tri_ILU nameX ) {
            if ( nameX.addressingMode != 1 ) {
                // ERROR
                form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                finished = true;
                return;
            }

            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    if ( SP.Value <= 0xFC00 ) {
                        form.errorMessage( 17, nameX.lineNumber );
                        MessageBox.Show( "Stack is Full. Not Allowed to write outside of the Stack Range",
                            "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    }
                    SP.Value--;
                    MemoryMap[SP.Value].Set( CPUregisters[nameX.sourceReg].LowerByte );

                    break;
                // Size: Word
                case 1:
                    if ( SP.Value <= 0xFC01 ) {
                        form.errorMessage( 17, nameX.lineNumber );
                        MessageBox.Show( "Stack is Full. Not Allowed to write outside of the Stack Range",
                            "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    }

                    SP.Value--;
                    MemoryMap[SP.Value].Set( CPUregisters[nameX.sourceReg].LowerByte );
                    SP.Value--;
                    MemoryMap[SP.Value].Set( CPUregisters[nameX.sourceReg].UpperByte );

                    break;

                default: break;
            }


        }

        public static void POP ( Tri_ILU nameX ) {
            if ( nameX.addressingMode != 1 ) {
                //Error
                form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                finished = true;
                return;
            }
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    if ( SP.Value >= 0xFFFF ) {
                        form.errorMessage( 18, nameX.lineNumber );
                        MessageBox.Show( "Stack is Empty. Cannot Access outside of the Stack Range",
                            "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    }

                    CPUregisters[nameX.destinationReg].Set(
                        CPUregisters[nameX.destinationReg].UpperByte, MemoryMap[SP.Value] );
                    SP.Value++;

                    updateCCR_Byte( nameX );


                    break;
                // Size: Word
                case 1:
                    if ( SP.Value >= 0xFFFE ) {
                        form.errorMessage( 18, nameX.lineNumber );
                        MessageBox.Show( "Stack is Empty. Cannot Access outside of the Stack Range",
                            "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        finished = true;
                        return;
                    }


                    CPUregisters[nameX.destinationReg].Set( MemoryMap[SP.Value], MemoryMap[SP.Value + 1] );
                    SP.Value += 2;

                    updateCCR_Word( nameX );

                    break;

                default: break;
            }

        }

        #endregion

        #region Arithmetic Instruction

        public static void ADD ( Tri_ILU nameX ) {
            Tri_Word temp;
            bool b7 = true, b6 = true;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            b7 = CPUregisters[nameX.destinationReg].LowerByte[7];
                            b6 = CPUregisters[nameX.destinationReg].LowerByte[6];

                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte(
                                    CPUregisters[nameX.destinationReg].LowerByte.ToIntU()
                                    + CPUregisters[nameX.sourceReg].LowerByte.ToIntU()
                                    )
                                );
                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            b7 = CPUregisters[nameX.destinationReg].LowerByte[7];
                            b6 = CPUregisters[nameX.destinationReg].LowerByte[6];

                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                               new Tri_Byte( CPUregisters[nameX.destinationReg].LowerByte.ToIntU() + temp.LowerByte.ToIntU() )
                                );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    // CCR Overflow
                    // Check if the last two bit are different 
                    if ( ( b7 && !b6 ) || ( !b7 && b6 ) ) {
                        CCR.Set( CCRflags.Overflow );
                    } else {
                        CCR.Clear( CCRflags.Overflow );
                    }

                    if ( CPUregisters[nameX.destinationReg].LowerByte.ToIntU() > Tri_Byte.MAXVALUE ) {
                        CCR.Set( CCRflags.Carry );
                    } else {
                        CCR.Clear( CCRflags.Carry );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            b7 = CPUregisters[nameX.destinationReg][15];
                            b6 = CPUregisters[nameX.destinationReg][14];

                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].ToIntS() + CPUregisters[nameX.sourceReg].ToIntS()
                                );

                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            b7 = CPUregisters[nameX.destinationReg][15];
                            b6 = CPUregisters[nameX.destinationReg][14];

                            CPUregisters[nameX.destinationReg].Set(
                               CPUregisters[nameX.destinationReg].ToIntS() + temp.ToIntS()
                                );


                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    updateCCR_Word( nameX );

                    // CCR Overflow
                    // Check if the last two bit are different 
                    if ( ( b7 && !b6 ) || ( !b7 && b6 ) ) {
                        CCR.Set( CCRflags.Overflow );
                    } else {
                        CCR.Clear( CCRflags.Overflow );
                    }

                    if ( CPUregisters[nameX.destinationReg].ToIntU() > Tri_Word.MAXVALUE ) {
                        CCR.Set( CCRflags.Carry );
                    } else {
                        CCR.Clear( CCRflags.Carry );
                    }

                    break;

                default:
                    break;
            }





        }

        public static void SUB ( Tri_ILU nameX ) {
            Tri_Word temp;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;


                        // Register Addressing
                        case 1:
                            CPUregisters[nameX.destinationReg].Set(
                                CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte(
                                 CPUregisters[nameX.destinationReg].LowerByte.ToIntS()
                                 - CPUregisters[nameX.sourceReg].LowerByte.ToIntS()
                                 )
                            );
                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            int t = CPUregisters[nameX.destinationReg].LowerByte.ToIntS();
                            int q = temp.LowerByte.ToIntS();
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( t - q )
                                );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;


                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    int d = 0, s = 0;
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            d = CPUregisters[nameX.destinationReg].ToIntS();
                            s = CPUregisters[nameX.sourceReg].ToIntS();
                            CPUregisters[nameX.destinationReg].Set( d - s );
                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            CPUregisters[nameX.destinationReg].Set(
                               CPUregisters[nameX.destinationReg].ToIntS() - temp.ToIntS()
                                );


                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;

                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }


        }

        public static void MULS ( Tri_ILU nameX ) {
            Tri_Word temp;
            int d;
            int s;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            d = CPUregisters[nameX.destinationReg].LowerByte.ToIntS();
                            s = CPUregisters[nameX.sourceReg].LowerByte.ToIntS();

                            //if ( d > 0x7F ) {
                            //    d = 256 - d;
                            //}
                            //if ( s > 0x7F ) {
                            //    s = 256 - s;
                            //}

                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( s * d ) );

                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            d = CPUregisters[nameX.destinationReg].LowerByte.ToIntS();
                            s = temp.LowerByte.ToIntS();

                            //if ( d > 0x7F ) {
                            //    d = 256 - d;
                            //}
                            //if ( s > 0x7F ) {
                            //    s = 256 - s;
                            //}

                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( s * d ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;


                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    //ERROR
                    form.errorMessage( 15, nameX.lineNumber ); // Wrong Operation Size
                    MessageBox.Show( "Wrong Operation Size", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    finished = true;
                    return;

                default:
                    break;

            }
        }

        public static void DIVU ( Tri_ILU nameX ) {
            int result;
            int remainder;
            Tri_Word temp;

            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            try {
                                int d = CPUregisters[nameX.destinationReg].LowerByte.ToIntU();
                                int r = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                                result = d / r;
                                remainder = d % r;
                                CPUregisters[nameX.destinationReg].Set( new Tri_Byte( result ), new Tri_Byte( remainder ) );

                            } catch ( DivideByZeroException ) {
                                //error
                                form.errorMessage( 16, nameX.lineNumber ); // Divide by Zero
                                MessageBox.Show( "Divide by Zero Exception", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            break;

                        // Immediate Addressing
                        case 2:

                            try {

                                temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                                result = CPUregisters[nameX.destinationReg].LowerByte.ToIntU() / temp.LowerByte.ToIntU();
                                remainder = CPUregisters[nameX.destinationReg].LowerByte.ToIntU() % temp.LowerByte.ToIntU();
                                CPUregisters[nameX.destinationReg].Set( new Tri_Byte( result ), new Tri_Byte( remainder ) );

                            } catch ( DivideByZeroException ) {
                                //error
                                form.errorMessage( 16, nameX.lineNumber ); // Divide by Zero
                                MessageBox.Show( "Divide by Zero Exception", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;


                        default:
                            break;
                    }

                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].UpperByte.isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].UpperByte.isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            try {
                                result = CPUregisters[nameX.destinationReg].ToIntU() / CPUregisters[nameX.sourceReg].ToIntU();
                                CPUregisters[nameX.destinationReg].Set( result );

                            } catch ( DivideByZeroException ) {
                                //error
                                form.errorMessage( 16, nameX.lineNumber ); // Divide by Zero
                                MessageBox.Show( "Divide by Zero Exception", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            break;

                        // Immediate Addressing
                        case 2:
                            try {
                                temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                                result = CPUregisters[nameX.destinationReg].ToIntU() / temp.ToIntU();
                                CPUregisters[nameX.destinationReg].Set( result );

                            } catch ( DivideByZeroException ) {
                                //error
                                form.errorMessage( 16, nameX.lineNumber ); // Divide by Zero
                                MessageBox.Show( "Divide by Zero Exception", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }


                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;

                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }


        }

        public static void MIN ( Tri_ILU nameX ) {
            Tri_Word temp;
            int s;
            int d;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            s = CPUregisters[nameX.sourceReg].LowerByte.ToIntS();
                            d = CPUregisters[nameX.destinationReg].LowerByte.ToIntS();
                            if ( s > d ) {
                                CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                    new Tri_Byte( d ) );
                            } else
                                CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                    new Tri_Byte( s ) );
                            break;

                        // Immediate Addressing
                        case 2:

                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            s = temp.LowerByte.ToIntS();
                            d = CPUregisters[nameX.destinationReg].LowerByte.ToIntS();

                            if ( s > d ) {
                                CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                    new Tri_Byte( d ) );
                            } else
                                CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                    new Tri_Byte( s ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            s = CPUregisters[nameX.sourceReg].ToIntS();
                            d = CPUregisters[nameX.destinationReg].ToIntS();
                            if ( s > d ) {
                                CPUregisters[nameX.destinationReg].Set( d );
                            } else
                                CPUregisters[nameX.destinationReg].Set( s );

                            break;

                        // Immediate Addressing
                        case 2:
                            temp = new Tri_Word( nameX.addressOrImm16OrOffset );
                            s = temp.ToIntS();
                            d = CPUregisters[nameX.destinationReg].ToIntS();

                            if ( s > d ) {
                                CPUregisters[nameX.destinationReg].Set( d );
                            } else
                                CPUregisters[nameX.destinationReg].Set( s );


                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );
                    
                    break;

                default:
                    break;
            }





        }

        #endregion

        #region Shift Instruction

        public static void ASHR ( Tri_ILU nameX ) {
            char msb;
            string bits = string.Empty;
            int sh = 0;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            bits = CPUregisters[nameX.destinationReg].LowerByte.ToString();
                            msb = bits[0];
                            if ( sh > 8 ) {
                                sh = 8;
                            }
                            CCR.SetValue( bits[bits.Length - sh] == '1' ? true : false, CCRflags.Carry );
                            bits = bits.Substring( 0, bits.Length - sh );
                            bits = bits.PadLeft( 8, msb );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( bits ) );


                            break;

                        // Immediate Addressing
                        case 2:
                            sh = nameX.addressOrImm16OrOffset;
                            bits = CPUregisters[nameX.destinationReg].LowerByte.ToString();
                            msb = bits[0];
                            if ( sh > 8 ) {
                                sh = 8;
                            }
                            CCR.SetValue( bits[bits.Length - sh] == '1' ? true : false, CCRflags.Carry );
                            bits = bits.Substring( 0, bits.Length - sh );
                            bits = bits.PadLeft( 8, msb );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( bits ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            msb = bits[0];
                            if ( sh > 16 ) {
                                sh = 16;
                            }
                            CCR.SetValue( bits[bits.Length - sh] == '1' ? true : false,
                                CCRflags.Carry );
                            bits = bits.Substring( 0, bits.Length - sh );
                            bits = bits.PadLeft( 16, msb );
                            CPUregisters[nameX.destinationReg].Set( bits );


                            break;

                        // Immediate Addressing
                        case 2:
                            sh = nameX.addressOrImm16OrOffset;
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            msb = bits[0];
                            if ( sh > 16 ) {
                                sh = 16;
                            }
                            CCR.SetValue( bits[bits.Length - sh] == '1' ? true : false,
                                CCRflags.Carry );
                            bits = bits.Substring( 0, bits.Length - sh );
                            bits = bits.PadLeft( 16, msb );
                            CPUregisters[nameX.destinationReg].Set( bits );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                default:
                    break;
            }



        }

        public static void LSHL ( Tri_ILU nameX ) {
            string bits = string.Empty;
            int sh = 0;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            bits = CPUregisters[nameX.destinationReg].LowerByte.ToString();
                            CCR.SetValue( ( bits[0] == '1' ? true : false ), CCRflags.Carry );
                            if ( sh > 8 ) {
                                sh = 8;
                            }
                            bits = bits.Substring( sh );
                            bits = bits.PadRight( 8, '0' );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( bits ) );
                            break;

                        // Immediate Addressing
                        case 2:
                            sh = nameX.addressOrImm16OrOffset;
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            CCR.SetValue( ( bits[0] == '1' ? true : false ), CCRflags.Carry );
                            if ( sh > 8 ) {
                                sh = 8;
                            }
                            bits = bits.Substring( sh );
                            bits = bits.PadRight( 8, '0' );
                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( bits ) );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            CCR.SetValue( ( bits[0] == '1' ? true : false ), CCRflags.Carry );
                            if ( sh > 16 ) {
                                sh = 16;
                            }
                            bits = bits.Substring( sh );
                            bits = bits.PadRight( 16, '0' );
                            CPUregisters[nameX.destinationReg].Set( bits );

                            break;

                        // Immediate Addressing
                        case 2:
                            sh = nameX.addressOrImm16OrOffset;
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            CCR.SetValue( ( bits[0] == '1' ? true : false ), CCRflags.Carry );
                            if ( sh > 16 ) {
                                sh = 16;
                            }
                            bits = bits.Substring( sh );
                            bits = bits.PadRight( 16, '0' );
                            CPUregisters[nameX.destinationReg].Set( bits );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                default:
                    break;
            }



        }

        public static void RCR ( Tri_ILU nameX ) {
            string bits = string.Empty;
            string temp;
            int sh = 0;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            bits = CPUregisters[nameX.destinationReg].LowerByte.ToString();
                            CCR.SetValue( ( bits[8 - sh] == '1' ? true : false ),
                                CCRflags.Carry );
                            sh %= 8;
                            temp = bits.Substring( 8 - sh );
                            bits = bits.Substring( 0, 8 - sh );
                            bits = bits.Insert( 0, temp );

                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( bits ) );

                            break;

                        // Immediate Addressing
                        case 2:
                            sh = nameX.addressOrImm16OrOffset;
                            bits = CPUregisters[nameX.destinationReg].LowerByte.ToString();
                            CCR.SetValue( ( bits[8 - sh] == '1' ? true : false ), CCRflags.Carry );
                            sh %= 8;
                            temp = bits.Substring( 8 - sh );
                            bits = bits.Substring( 0, 8 - sh );
                            bits = bits.Insert( 0, temp );

                            CPUregisters[nameX.destinationReg].Set( CPUregisters[nameX.destinationReg].UpperByte,
                                new Tri_Byte( bits ) );
                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            // Store the number of rotation
                            sh = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            // Store the value of the destination register as binary
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            // If the rotation value is more then the maximum bit number
                            // bring it back to the range
                            sh %= 16;
                            // Set the Carry flag accordigly
                            CCR.SetValue( ( bits[18 - sh] == '1' ? true : false ), CCRflags.Carry );
                            // Store the bits that will be rotated
                            temp = bits.Substring( 16 - sh );
                            // Remove the bits that will be rotated
                            bits = bits.Substring( 0, 16 - sh );
                            // Insert the removed bits back at the beginning
                            bits = bits.Insert( 0, temp );
                            // store it back to the destination register
                            CPUregisters[nameX.destinationReg].Set( bits );
                            break;

                        // Immediate Addressing
                        case 2:
                            // Store the number of rotation
                            sh = nameX.addressOrImm16OrOffset;
                            // Store the value of the destination register as binary
                            bits = CPUregisters[nameX.destinationReg].ToString();
                            // If the rotation value is more then the maximum bit number
                            // bring it back to the range
                            sh %= 16;
                            // Set the Carry flag accordigly
                            CCR.SetValue( ( bits[18 - sh] == '1' ? true : false ), CCRflags.Carry );
                            // Store the bits that will be rotated
                            temp = bits.Substring( 16 - sh );
                            // Remove the bits that will be rotated
                            bits = bits.Substring( 0, 16 - sh );
                            // Insert the removed bits back at the beginning
                            bits = bits.Insert( 0, temp );
                            // store it back to the destination register
                            CPUregisters[nameX.destinationReg].Set( bits );

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    // CCR Negative
                    if ( CPUregisters[nameX.destinationReg].isNegative() ) {
                        CCR.Set( CCRflags.Negative );
                    } else {
                        CCR.Clear( CCRflags.Negative );
                    }

                    break;

                default:
                    break;
            }



        }

        #endregion

        #region Bit Manipulation


        public static void BTST ( Tri_ILU nameX ) {
            int index;
            bool b = true;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:

                            index = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            if ( index < 8 ) {
                                b = CPUregisters[nameX.destinationReg].LowerByte[index - 1];
                                if ( b ) {
                                    CCR.Clear( CCRflags.Zero );
                                } else {
                                    CCR.Set( CCRflags.Zero );
                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            break;

                        // Immediate Addressing
                        case 2:
                            if ( nameX.addressOrImm16OrOffset < 8 ) {
                                b = CPUregisters[nameX.destinationReg].LowerByte[nameX.addressOrImm16OrOffset - 1];
                                if ( b ) {
                                    CCR.Clear( CCRflags.Zero );
                                } else {
                                    CCR.Set( CCRflags.Zero );
                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:

                            index = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();
                            if ( index < 16 ) {
                                b = CPUregisters[nameX.destinationReg][index];
                                if ( b ) {
                                    CCR.Clear( CCRflags.Zero );
                                } else {
                                    CCR.Set( CCRflags.Zero );
                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            break;

                        // Immediate Addressing
                        case 2:
                            if ( nameX.addressOrImm16OrOffset < 16 ) {
                                b = CPUregisters[nameX.destinationReg][nameX.addressOrImm16OrOffset];
                                if ( b ) {
                                    CCR.Clear( CCRflags.Zero );
                                } else {
                                    CCR.Set( CCRflags.Zero );
                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }


                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }


                    break;

                default:
                    break;
            }

        }

        public static void BSS ( Tri_ILU nameX ) {
            int counter = 0;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            for ( int index = 0; index < 8; index++ ) {
                                if ( CPUregisters[nameX.sourceReg][index] )
                                    counter++;
                            }
                            CPUregisters[nameX.destinationReg].Set( 
                                CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( counter ) );

                            break;

                        // Immediate Addressing
                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].LowerByte.isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            for ( int index = 0; index < 15; index++ ) {
                                if ( CPUregisters[nameX.sourceReg][index] )
                                    counter++;
                            }
                            CPUregisters[nameX.destinationReg].Set( 
                                CPUregisters[nameX.destinationReg].UpperByte, new Tri_Byte( counter ) );

                            break;

                        // Immediate Addressing
                        case 2:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    // CCR Zero
                    if ( CPUregisters[nameX.destinationReg].isZero() ) {
                        CCR.Set( CCRflags.Zero );
                    } else {
                        CCR.Clear( CCRflags.Zero );
                    }

                    break;

                default:
                    break;
            }

        }

        public static void BCLR ( Tri_ILU nameX ) {
            Tri_Word temp;
            int range;
            int index;
            bool b = true;
            switch ( nameX.operationSize ) {
                // Size: Byte
                case 0:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:

                            range = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();

                            if ( range <= 8 ) {
                                for ( index = 0; index < range; index++ ) {
                                    CPUregisters[nameX.destinationReg][index] = false;

                                }

                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            break;

                        // Immediate Addressing
                        case 2:
                            if ( nameX.addressOrImm16OrOffset <= 8 ) {
                                for ( index = 0; index < nameX.addressOrImm16OrOffset; index++ ) {
                                    CPUregisters[nameX.destinationReg][index] = false;

                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }

                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    updateCCR_Byte( nameX );


                    break;

                // Size: Word
                case 1:
                    switch ( nameX.addressingMode ) {
                        // Register Indirect with displacement
                        case 0:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        // Register Addressing
                        case 1:
                            range = CPUregisters[nameX.sourceReg].LowerByte.ToIntU();

                            if ( range <= 16 ) {
                                for ( index = 0; index < range; index++ ) {
                                    CPUregisters[nameX.destinationReg][index] = false;

                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            break;

                        // Immediate Addressing
                        case 2:
                            if ( nameX.addressOrImm16OrOffset <= 16 ) {
                                for ( index = 0; index < nameX.addressOrImm16OrOffset; index++ ) {
                                    CPUregisters[nameX.destinationReg][index] = false;

                                }
                            } else {
                                // Error
                                form.errorMessage( 20, nameX.lineNumber ); // Index Out of Range
                                MessageBox.Show( "Index Out of Range", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                finished = true;
                                return;
                            }
                            break;

                        // Absolute Addressing
                        case 3:
                            //ERROR
                            form.errorMessage( 14, nameX.lineNumber ); // Wrong Addressing Mode
                            MessageBox.Show( "Wrong Addressing Mode", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
                            finished = true;
                            return;

                        default:
                            break;
                    }

                    updateCCR_Word( nameX );

                    break;

                default:
                    break;
            }

        }


        #endregion

        #region Subroutine

        public static void BSUB ( Tri_ILU nameX ) {

            SP.Value--;
            MemoryMap[SP.Value].Set( PC.LowerByte );
            SP.Value--;
            MemoryMap[SP.Value].Set( PC.UpperByte );

            PC.Set( nameX.addressOrImm16OrOffset );

        }

        public static void RSUB ( Tri_ILU nameX ) {

            PC.Set( MemoryMap[SP.Value], MemoryMap[SP.Value + 1] );
            SP.Value += 2;


        }

        #endregion

        private static void updateCCR_Byte ( Tri_ILU nameX ) {
            // CCR Zero
            if ( CPUregisters[nameX.destinationReg].LowerByte.isZero() ) {
                CCR.Set( CCRflags.Zero );
            } else {
                CCR.Clear( CCRflags.Zero );
            }

            // CCR Negative
            if ( CPUregisters[nameX.destinationReg].LowerByte.isNegative() ) {
                CCR.Set( CCRflags.Negative );
            } else {
                CCR.Clear( CCRflags.Negative );
            }
        }

        private static void updateCCR_Word ( Tri_ILU nameX ) {
            // CCR Zero
            if ( CPUregisters[nameX.destinationReg].isZero() ) {
                CCR.Set( CCRflags.Zero );
            } else {
                CCR.Clear( CCRflags.Zero );
            }

            // CCR Negative
            if ( CPUregisters[nameX.destinationReg].isNegative() ) {
                CCR.Set( CCRflags.Negative );
            } else {
                CCR.Clear( CCRflags.Negative );
            }
        }

    }

}
