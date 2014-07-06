using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SP579emulator {
    partial class Tri_MainWindow {

        private void updateMachineState ( TextBox txtBox, object value ) {
            if ( isMachineStateBinary ) {
                // if the machine state is binary and the value is decimal
                if ( value is int ) {
                    txtBox.Text = Convert.ToString( ( int ) value, 2 ).PadLeft( 16, '0' );

                } else if ( value is string ) {
                    // if the machine state is binary and the value is HEX
                    if ( ( ( string ) value ).Length == 4 ) {
                        txtBox.Text = Convert.ToString(
                                    int.Parse( ( string ) value,
                                                System.Globalization.NumberStyles.HexNumber ), 2 )
                                                .PadLeft( 16, '0' );
                        // if the machine state is binary and the value is binary
                    } else {
                        txtBox.Text = ( string ) value;
                    }
                    // if the machine state is binary and the value is none of them
                } else {
                    txtBox.Text = "1111111111111111";
                }

            } else if ( isMachineStateDecimal ) {
                // if the machine state is decimal and the value is decimal
                if ( value is int ) {
                    txtBox.Text = ( ( int ) value ).ToString().PadLeft( 5, '0' );

                } else if ( value is string ) {
                    // if the machine state is decimal and the value is HEX
                    if ( ( ( string ) value ).Length == 4 ) {
                        txtBox.Text = int.Parse( ( string ) value,
                                                System.Globalization.NumberStyles.HexNumber )
                                                .ToString().PadLeft( 5, '0' );

                        // if the machine state is decimal and the value is binary
                    } else {
                        txtBox.Text = binaryToInt( ( string ) value ).ToString().PadLeft( 5, '0' );

                    }
                    // if the machine state is decimal and the value is none of them
                } else {
                    txtBox.Text = "99999";
                }


            } else if ( isMachineStateHex ) {

                // if the machine state is hex and the value is decimal
                if ( value is int ) {
                    txtBox.Text = ( ( int ) value ).ToString( "X" ).PadLeft( 4, '0' );
                    txtBox.Text = txtBox.Text.Substring( txtBox.Text.Length - 4 );


                } else if ( value is string ) {
                    // if the machine state is hex and the value is HEX
                    if ( ( ( string ) value ).Length == 4 ) {
                        txtBox.Text = ( ( string ) value ).PadLeft( 4, '0' );

                        // if the machine state is hex and the value is binary
                    } else {
                        txtBox.Text = binaryToInt( ( string ) value ).ToString( "X" ).PadLeft( 4, '0' );

                    }
                    // if the machine state is decimal and the value is none of them
                } else {
                    txtBox.Text = "FFFF";
                }



            }
        }

        private void execute () {
            // Read from the rtbEditor (Rich Text Box) and convert it to the
            // binary format (UpperByte) and store into the memory of the
            // simulator (Memory Map).

            if ( !isLoadedIntoMemory ) {
                if ( !load() )
                    return;
            }
            /*
            if ( !hasNoError ) {
                reset();
                return;
            }
            */
            // Go into an infinite loop, and only come out if there is
            // no record left to fetch or the HLT() has been called.

            // while( !Instruction.finished /* from the HLT() function */
            //              && there are still records to read ) {
            //     ...
            //     ...
            //
            // }

            // Call the fetch function, to read the record from the memory
            // of the simulator (MemoryMap).

            // nextRecord = MemoryMap[PC.ToInt()];

            // Call the decode function, to decode the fetched record and
            // decide what instruction it is and call the responsible
            // function

            // Execute happens in the responsible function for an instruction.
            while ( !Tri_Instructions.finished
                && PC.ToIntU() <= Convert.ToInt16( lastInstructionAddress, 16 ) ) {

                fetchAndDecode( PC.ToIntU() );
                reverseAssemble( ILU ); //method that finds the reverse assembly of the current instruction in the ILU


            }


        }

        private void openFile () {

            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr;

            ofd.Filter = "S Record|*.S|Text File|*.txt|Any File|*.*";
            ofd.Multiselect = false;
            ofd.DereferenceLinks = true;
            ofd.Title = "Open Record File";
            if ( path == string.Empty ) {
                ofd.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
            } else {
                ofd.InitialDirectory = path;
            }

            dr = ofd.ShowDialog();
            StreamReader sr;


            if ( dr == DialogResult.OK ) {
                fileName = ofd.FileName;
                path = fileName.Substring( 0, fileName.LastIndexOf( '\\' ) );
                sr = new StreamReader( fileName );
                reset();
                rtbEditor.Text = sr.ReadToEnd();
                isFileOpened = true;
                writeLog( "openFile(): File Opened: " + fileName );
                //load();
                sr.Close();


            }
        }

        private void reset () {
            writeLog( "reset(): Resetting All Parameters" );

            // Reset the application except for the editor
            // - Set the memory map to it's default value
            // - Delete error list
            // - Remove the status image from the tool bar

            for ( int i = 0; i < MemoryMap.Length; i++ ) {
                MemoryMap[i].SetAll( true );
            }

            X0.Set( 0 );
            X1.Set( 0 );
            X2.Set( 0 );
            X3.Set( 0 );
            X4.Set( 0 );
            X5.Set( 0 );
            X6.Set( 0 );
            X7.Set( 0 );
            PC.Set( 0 );
            SP.Value = 0x0000;
            CCR.Clear( CCRflags.Carry );
            CCR.Clear( CCRflags.Negative );
            CCR.Clear( CCRflags.Overflow );
            CCR.Clear( CCRflags.Zero );


            rtbError.Clear();
            rtbAssembly.Clear();
            tslStatus.Image = global::SP579emulator.Properties.Resources.no_image_16xLG;
            executeToolStripMenuItem.Text = "Execute";
            executeToolStripMenuItem.Image = global::SP579emulator.Properties.Resources.Symbols_Play_16xLG;
            tsbExecute.Text = "Execute";
            tsbExecute.Image = global::SP579emulator.Properties.Resources.Symbols_Play_16xLG;
            tsbExecute.ToolTipText = "Execute (F5)";
            executeToolStripMenuItem.ShortcutKeys = Keys.F5;

            tsbOpenFile.Enabled = true;
            tsbClose.Enabled = true;
            tsbSave.Enabled = true;
            tsbUndo.Enabled = true;
            tsbRedo.Enabled = true;

            tsbExecute.Enabled = true;
            tsbExecuteStep.Enabled = true;
            tsbExecuteToPosition.Enabled = true;
            tsbRestart.Enabled = true;

            openToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            redoToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;

            stepByStepExecutionToolStripMenuItem.Enabled = true;
            restartToolStripMenuItem.Enabled = true;
            executeToTheSelectedLineToolStripMenuItem.Enabled = true;
            executeToolStripMenuItem.Enabled = true;

            records = rtbEditor.Text.Split( new char[] { '\n', '\r' }, StringSplitOptions.None );
            if ( rtbEditor.Text.Contains( "   " ) ) {
                rtbEditor.Clear();
                for ( int i = 0; i < records.Length - 1; i++ ) {
                    records[i] = records[i].Remove( 0, 5 );
                    rtbEditor.Text += records[i] + Environment.NewLine;
                }
            }

            rtbEditor.SelectAll();
            rtbEditor.SelectionBackColor = System.Drawing.Color.White;
            rtbEditor.DeselectAll();

            if ( records != null ) {
                for ( int i = 0; i < records.Length; i++ ) {
                    records[i] = string.Empty;
                }
            }

            isLoadedIntoMemory = false;
            Tri_Instructions.finished = false;

            if ( isStepByStepExecutionStarted ) {
                isStepByStepExecutionStarted = false;
            }

            if ( isExecutionStarted ) {
                isExecutionStarted = false;
            }

        }

        private bool load () {
            writeLog( "load(): Checking for errors in the Record while loading it into memory" );
            //reset();

            // false if an error was found in the current record
            bool recordHasNoError = true;

            hasNoError = true;

            // True is duplicate record type-1 found
            bool isType1Found = false;

            bool isORGfound = false;

            // stores the address of the last checked record
            int lastAddress = 0;

            int lastNumberOfBytes = 0;

            if ( !isFileOpened ) {
                errorMessage( 13, 0 );
                hasNoError = false;
                return false;
            }

            if ( rtbEditor.Text == string.Empty ) {
                errorMessage( 12, 0 );
                hasNoError = false;
                return false;
            }


            records = rtbEditor.Text.Split( new char[] { '\n', '\r' }, StringSplitOptions.None );
            int recordLength = records.Length;

            if ( rtbEditor.Text.Contains( "   " ) ) {
                recordLength = records.Length - 1;

                for ( int i = 0; i < recordLength; i++ ) {
                    try {
                        records[i] = records[i].Remove( 0, 5 );

                    } catch {

                        continue;
                    }

                }
            }

            string[] rp;

            numberOfRecord = recordLength;

            if ( records[0][0] != '1' ) {
                errorMessage( 4, 1 );
                hasNoError = false;
            }

            for ( int i = 0; i < recordLength; i++ ) {
                recordHasNoError = true;

                rp = records[i].Split( new char[] { '*', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries );

                #region Record Type 1
                //if ( rp == null ) {
                //    continue;
                //}

                try {
                    if ( rp[0] == "1" ) {

                        if ( isType1Found ) {
                            errorMessage( 9, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        if ( rp.Length != 4 ) {
                            errorMessage( 7, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        // Check the byte count
                        int bc = int.Parse( rp[1], System.Globalization.NumberStyles.HexNumber );
                        if ( bc != byteCount( rp ) ) {
                            errorMessage( 2, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        if ( !isType1Found ) {
                            int addr = int.Parse( rp[2], System.Globalization.NumberStyles.HexNumber );

                            if ( addr < 0 || addr > 0xFFFF ) {
                                errorMessage( 5, i + 1 );
                                recordHasNoError = false;
                                hasNoError = false;

                            } else if ( addr > 0xFBFF ) {
                                errorMessage( 6, i + 1 );
                                recordHasNoError = false;
                                hasNoError = false;
                            }

                            PC.Set( addr );
                            //updateMachineState( txtPC, PC.ToString() );
                            //Console.WriteLine( PC.ToHEX() );

                            isType1Found = true;

                        }
                    }

                #endregion
                    #region Record Type 2
                        else if ( rp[0] == "2" ) {

                        if ( rp.Length != 5 ) {
                            errorMessage( 8, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        // Check the byte count
                        int bc = int.Parse( rp[1], System.Globalization.NumberStyles.HexNumber );
                        if ( bc != byteCount( rp ) ) {
                            errorMessage( 2, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        // Convert string Hex to integer
                        int address = int.Parse( ( rp[2] ), System.Globalization.NumberStyles.HexNumber );

                        // storing the line number for later use in the error messages
                        if ( PC.ToIntU() == address ) {
                            lineNumber = i + 1;
                            isORGfound = true;
                        }

                        if ( address <= lastAddress ) {
                            errorMessage( 10, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        if ( address < lastAddress + lastNumberOfBytes ) {
                            warningMessage( 1, i + 1 );

                        }

                        lastNumberOfBytes = rp[3].Length / 2;


                        if ( address < 0 || address > 0xFFFF ) {
                            errorMessage( 5, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;

                        } else if ( address > 0xFBFF ) {
                            errorMessage( 6, i + 1 );
                            recordHasNoError = false;
                            hasNoError = false;
                        }

                        if ( recordHasNoError ) {
                            lastAddress = address;
                            for ( int j = 0; j < rp[3].Length; j += 2 ) {
                                MemoryMap[address++].Set( rp[3].Substring( j, 2 ) );

                            }
                        }

                    }
                    #endregion
                    #region Unknown Record
                    else {
                        errorMessage( 1, ( i + 1 ) );
                        recordHasNoError = false;
                        hasNoError = false;
                    }
                    #endregion

                } catch ( IndexOutOfRangeException ) {
                    recordLength--;
                    continue;
                }




                //if ( recordHasNoError ) {
                //
                //    if ( rp[rp.Length - 1] != calculateCheckSum( rp ) ) {
                //        errorMessage( 3, i + 1 );
                //        recordHasNoError = false;
                //        hasNoError = false;
                //    }
                //}

                lastInstructionAddress = rp[2]; //hold the last instruction address after the loop ends

            }

            if ( !isORGfound ) {
                hasNoError = false;
            }

            int mmm = PC.ToIntU();
            if ( lastAddress < mmm ) {
                errorMessage( 11, 0 );
                recordHasNoError = false;
                hasNoError = false;
            }

            if ( hasNoError ) {
                tslStatus.Image = global::SP579emulator.Properties.Resources.StatusAnnotations_Complete_and_ok_16xLG;
                tsbExecute.Enabled = true;
                tsbExecuteStep.Enabled = true;
                tsbExecuteToPosition.Enabled = true;
                tsbRestart.Enabled = true;

                executeToolStripMenuItem.Enabled = true;
                executeToTheSelectedLineToolStripMenuItem.Enabled = true;
                stepByStepExecutionToolStripMenuItem.Enabled = true;
                restartToolStripMenuItem.Enabled = true;

                isLoadedIntoMemory = true;
                rtbEditor.Clear();

                for ( int i = 0; i < recordLength; i++ ) {
                    rtbEditor.Text += ( i + 1 ).ToString( "D2" ) + "   " + records[i] + Environment.NewLine;
                }

                rtbAssembly.Text += "ORG\t\t$" + PC.ToHEX();
                rtbAssembly.Text += Environment.NewLine;

                // for debug purposes.-.-.-.-.-.-
                //MemoryMap[65315].Set( -4 );
                //MemoryMap[65316].Set( 200 );
                //MemoryMap[62438].Set( 92 );
                //MemoryMap[62439].Set( 157 );
                //MemoryMap[62440].Set( 175 );
                //MemoryMap[62441].Set( 65 );
                //MemoryMap[62442].Set( 67 );
                //MemoryMap[62443].Set( 6 );
                //X4.Set( 8 );
                //X7.Set( 2 );
                //-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-

                return true;
            }
            return false;

        }

        public void errorMessage ( int errno, int lineNumber ) {
            // diable the execute buttons
            tslStatus.Image = global::SP579emulator.Properties.Resources.StatusAnnotations_Critical_16xLG;
            tsbExecute.Enabled = false;
            tsbExecuteStep.Enabled = false;
            tsbExecuteToPosition.Enabled = false;

            executeToolStripMenuItem.Enabled = false;
            executeToTheSelectedLineToolStripMenuItem.Enabled = false;
            stepByStepExecutionToolStripMenuItem.Enabled = false;

            if ( lineNumber > 0 ) {
                rtbEditor.Select( rtbEditor.GetFirstCharIndexFromLine( lineNumber - 1 ), rtbEditor.Lines[lineNumber - 1].Length );
                rtbEditor.SelectionBackColor = System.Drawing.Color.PaleVioletRed;
                rtbEditor.DeselectAll();
            }
            

            string errorText = string.Empty;

            switch ( errno ) {
                case 1: errorText = "Wrong Record Type";
                    break;

                case 2: errorText = "Byte Count Mismatch";
                    break;

                case 3: errorText = "Invalid Checksum";
                    break;

                case 4: errorText = "First Record Type Not 1";
                    break;

                case 5: errorText = "Address Out of Range";
                    break;

                case 6: errorText = "Address Within Stack Range";
                    break;

                case 7: errorText = "Record Type-1 Invalid Format";
                    break;

                case 8: errorText = "Record Type-2 Invalid Format";
                    break;

                case 9: errorText = "Duplicate Record Type-1 Found";
                    break;

                case 10: errorText = "Wrong Address Sequence";
                    break;

                case 11: errorText = "No Instruction Found at the Start Address";
                    break;

                case 12: errorText = "No Records in the File";
                    break;

                case 13: errorText = "No File has Been Opened";
                    break;

                case 14: errorText = "Wrong Addressing Mode";
                    break;

                case 15: errorText = "Wrong Operation Size";
                    break;

                case 16: errorText = "Divide by Zero";
                    break;

                case 17: errorText = "Stack is Full";
                    break;

                case 18: errorText = "Stack is Empty";
                    break;

                case 19: errorText = "Stack is Outside of it's Range between 0xFC00 and 0xFFFF";
                    break;

                case 20: errorText = "Index Out of Range";
                    break;

                default: errorText = "Unknown Error";
                    break;
            }

            rtbError.Text += string.Format( "ERROR #{2}: {3} (Line:{0}){1}",
                    lineNumber, Environment.NewLine, errno.ToString( "D2" ), errorText );
            writeLog( "errorMessage(): Error found: "
                        + string.Format( "ERROR #{1}: {2} (Line:{0})", lineNumber,
                                errno.ToString( "D2" ), errorText ) );

        }

        private void warningMessage ( int warno, int lineNumber ) {
            string warningText = string.Empty;

            switch ( warno ) {
                case 1: warningText = "The Instruction Overrides Parts of the Previous Instruction";
                    break;
                default: warningText = "Unkown Warning";
                    break;
            }

            rtbError.Text += string.Format( "Warning #{2}: {3} (Line:{0}){1}",
                    lineNumber, Environment.NewLine, warno.ToString( "D2" ), warningText );
            writeLog( "warningMessage(): Warning found: "
                        + string.Format( "Warning #{1}: {2} (Line:{0})", lineNumber,
                                warno.ToString( "D2" ), warningText ) );

        }

        private int byteCount ( string[] record ) {
            int count = 0;

            for ( int i = 1; i < record.Length - 1; i++ ) {
                count += record[i].Length / 2;
            }

            return count;

        }

        private string calculateCheckSum ( string[] record ) {

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
            hex = result.ToString( "X4" );
            string wrapAround = string.Empty;
            while ( hex.Length > 2 ) {
                wrapAround = hex.Substring( 0, hex.Length - 2 );
                hex = hex.Substring( wrapAround.Length );

                result = int.Parse( hex, System.Globalization.NumberStyles.HexNumber );
                result += int.Parse( wrapAround, System.Globalization.NumberStyles.HexNumber );
                hex = result.ToString( "X4" );
            }

            // One's compliment
            //hex = result.ToString( "X2" );
            //
            //result = int.Parse( hex[0].ToString(), System.Globalization.NumberStyles.HexNumber );
            //result = 15 - result;
            //
            //hex = result.ToString( "X" ) + hex[1];
            //
            //result = int.Parse( hex[1].ToString(), System.Globalization.NumberStyles.HexNumber );
            //result = 15 - result;
            //
            //hex = hex[0] + result.ToString( "X4" );

            return ( 255 - result ).ToString( "X2" );

        }

        private void writeLog ( string log ) {


            sw_LOG = new StreamWriter( "LogDump_" + logDate + ".log", true );
            string temp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                    + "  " + log + "." + Environment.NewLine;

            sw_LOG.Write( temp );
            appLog += temp;

            sw_LOG.Close();

        }

        private void reorderComponent () {
            if ( errorsToolStripMenuItem.Checked ) {
                rtbEditor.Height = this.Height - 285;
                rtbEditor.Top = 160;

                rtbAssembly.Height = rtbEditor.Height;
                rtbAssembly.Top = rtbEditor.Top;

                rtbError.Left = 15;
                rtbError.Width = rtbEditor.Width + rtbAssembly.Width + 5;
                rtbError.Top = this.Height - 120;

            } else {
                rtbEditor.Height = this.Height - 214;
                rtbEditor.Top = 160;

                rtbAssembly.Height = rtbEditor.Height;
                rtbAssembly.Top = rtbEditor.Top;

            }

            if ( assembleyToolStripMenuItem.Checked ) {
                rtbEditor.Width = this.Width / 2;
                rtbEditor.Left = 15;

                rtbAssembly.Width = ( this.Width / 2 ) - 49;
                rtbAssembly.Left = rtbEditor.Width + 20;

                lblAssembly.Left = rtbAssembly.Left;

                rtbError.Left = 15;
                rtbError.Width = rtbEditor.Width + rtbAssembly.Width + 5;
                rtbError.Top = this.Height - 120;

            } else {
                rtbEditor.Left = 15;
                rtbEditor.Width = this.Width - 45;

                rtbError.Left = rtbEditor.Left;
                rtbError.Width = rtbEditor.Width;
                rtbError.Top = this.Height - 120;

            }

        }

        private void saveFileAs () {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "S Record|*.S|Text File|*.txt|Any File|*.*";
            sfd.Title = "Save Record As";
            sfd.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

            switch ( sfd.ShowDialog() ) {
                case DialogResult.OK:
                    StreamWriter sww = new StreamWriter( sfd.FileName );

                    records = rtbEditor.Text.Split( new char[] { '\n', '\r' }, StringSplitOptions.None );
                    if ( rtbEditor.Text.Contains( "   " ) ) {
                        for ( int i = 0; i < records.Length - 1; i++ ) {
                            records[i] = records[i].Remove( 0, 5 );
                        }
                    }

                    for ( int i = 0; i < records.Length - 1; i++ ) {
                        sww.WriteLine( records[i] );
                    }

                    sww.Close();
                    break;
                default:
                    break;
            }
        }

        private void saveFile () {
            if ( fileName == "" ) {
                saveFileAs();
                return;
            }
            StreamWriter sww = new StreamWriter( fileName );
            records = rtbEditor.Text.Split( new char[] { '\n', '\r' }, StringSplitOptions.None );
            if ( rtbEditor.Text.Contains( "   " ) ) {
                for ( int i = 0; i < records.Length - 1; i++ ) {
                    records[i] = records[i].Remove( 0, 5 );
                }
            }

            for ( int i = 0; i < records.Length - 1; i++ ) {
                sww.WriteLine( records[i] );
            }
            sww.Close();
        }

        private int binaryToInt ( string value ) {
            int result = 0;
            for ( int i = 0; i < value.Length; i++ ) {
                result += ( value[i] == '1' ? 1 : 0 ) * ( int ) Math.Pow( 2, value.Length - ( i + 1 ) );
            }

            return result;
        }


        /// <summary>
        /// The method that fetches the next instruction and decodes it to fill the ILU
        /// </summary>
        /// <param name="startLocation"> where from memory to get a byte</param>
        private void fetchAndDecode ( int startLocation ) {
            writeLog( "Fetch at location: " + startLocation.ToString( "X4" ) );

            //this method returns the opcode of the current instruction and updates the PC depending on the current instruction size
            string instruction = MemoryMap[startLocation].ToString() + MemoryMap[startLocation + 1].ToString();
            writeLog( "Instruction: " + instruction );

            int opcode = Convert.ToInt16( instruction.Substring( 0, 5 ), 2 );
            writeLog( "Operation Code: " + Convert.ToString( opcode, 2 ) );

            int operationSize = Convert.ToInt16( instruction.Substring( 5, 1 ), 2 );
            writeLog( "Operation Size: " + ( ( operationSize == 0 ) ? "Byte" : "Word" ) );
            /* operationSize values:
             * 0 --> Byte
             * 1 --> Word
             */

            int addressingMode = Convert.ToInt16( instruction.Substring( 6, 2 ), 2 );
            switch ( addressingMode ) {
                case 0:
                    writeLog( "Addressing Mode: Register Indirect with displacement addressing" );
                    break;
                case 1:
                    writeLog( "Addressing Mode: Register Addressing" );
                    break;
                case 2:
                    writeLog( "Addressing Mode: Immediate (Literal) Addressing" );
                    break;
                case 3:
                    writeLog( "Addressing Mode: Absolute Addressing (m=11)" );
                    break;

                default:
                    break;
            }

            /* addressingMode values:
             * 0 --> Register Indirect with displacement
             * 1 --> Register Addressing
             * 2 --> Immediate
             * 3 --> Absolute
             
             modes 0,2,3 need two more bytes from memory
             */
            int sourceReg = Convert.ToInt16( instruction.Substring( 8, 3 ), 2 );
            writeLog( "Source Register: X" + sourceReg );
            int destinationReg = Convert.ToInt16( instruction.Substring( 11, 3 ), 2 );
            writeLog( "Destination Register: X" + destinationReg );
            //the remaining (14,15) is a 00 padding, if we need to fetch two more bytes the starting index to get the address will be 16

            int address = 0;
            int addToPC = 2; //the default value to add to pc is 2, 4 if the instruction uses one of the modes 0,2,3 for addressing

            switch ( opcode ) {
                //these cases test for instructions that take two more bytes from memory
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 17:
                case 18:
                case 20:
                    if ( addressingMode != 1 ) {
                        writeLog( "Get two more byte for the instruction" );
                        instruction += MemoryMap[startLocation + 2].ToString() + MemoryMap[startLocation + 3].ToString();
                        writeLog( "New Instruction is: " + instruction );
                        address = Convert.ToInt16( instruction.Substring( 16, 16 ), 2 );
                        writeLog( "Address is: " + address.ToString( "X4" ) );
                        addToPC += 2;
                    }
                    break;

                //test for branch instructions for special treatment depending on the value of r, instructions may be 16 or 32
                //r is in the same position of operationSize 
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    if ( operationSize == 0 ) {
                        string qwe = instruction.Substring( 6, 10 ).PadLeft( 16, instruction[6] );
                        address = Convert.ToInt16( qwe , 2 );
                    } else {
                        instruction += MemoryMap[startLocation + 2].ToString() + MemoryMap[startLocation + 3].ToString();
                        address = Convert.ToInt16( instruction.Substring( 16, 16 ), 2 );
                        addToPC += 2;
                    }
                    break;

                default:
                    break;
            }

            // Immediate Addressing doesn't use negative numbers.
            // the negative numbers are actually big positive numbers.
            if ( addressingMode == 3 && address < 0 && opcode < 21 && opcode > 26 ) {
                if ( operationSize == 0 ) {
                    address = ( int ) Tri_Byte.MAXVALUE + address;
                } else if ( operationSize == 1 ) {
                    address = ( int ) Tri_Word.MAXVALUE + address;
                }
            }

            ILU.opCode = opcode;
            ILU.addressingMode = addressingMode;
            ILU.operationSize = operationSize;
            ILU.sourceReg = sourceReg;
            ILU.destinationReg = destinationReg;
            ILU.addressOrImm16OrOffset = address;
            // store the line number of the record
            // then increment the number 
            ILU.lineNumber = lineNumber++;

            PC.Set( PC.ToIntU() + addToPC );

            //call the correct function to execute the instruction:
            switch ( opcode ) {
                case 0:
                    //call LD Function
                    Tri_Instructions.LD( ILU );
                    break;
                case 1:
                    //call STR function
                    Tri_Instructions.STR( ILU );
                    break;
                case 2:
                    //call LSP
                    Tri_Instructions.LSP( ILU );
                    break;
                case 3:
                    //call EXG
                    Tri_Instructions.EXG( ILU );
                    break;
                case 4:
                    //call ADD
                    Tri_Instructions.ADD( ILU );
                    break;
                case 5:
                    //call SUB
                    Tri_Instructions.SUB( ILU );
                    break;
                case 6:
                    //call MULS
                    Tri_Instructions.MULS( ILU );
                    break;
                case 7:
                    //call DIVU
                    Tri_Instructions.DIVU( ILU );
                    break;
                case 8:
                    //call MIN
                    Tri_Instructions.MIN( ILU );
                    break;
                case 9:
                    //call AND
                    Tri_Instructions.AND( ILU );
                    break;
                case 10:
                    //call NOT
                    Tri_Instructions.NOT( ILU );
                    break;
                case 11:
                    //call OR
                    Tri_Instructions.OR( ILU );
                    break;
                case 12:
                    //call ASHR 
                    Tri_Instructions.ASHR( ILU );
                    break;
                case 13:
                    //call LSHL
                    Tri_Instructions.LSHL( ILU );
                    break;
                case 14:
                    //call RCR
                    Tri_Instructions.RCR( ILU );
                    break;
                case 15:
                    //call PUSH
                    Tri_Instructions.PUSH( ILU );
                    break;
                case 16:
                    //call POP
                    Tri_Instructions.POP( ILU );
                    break;
                case 17:
                    //call CMP
                    break;
                case 18:
                    //call BTST
                    Tri_Instructions.BTST( ILU );
                    break;
                case 19:
                    //call BSS
                    Tri_Instructions.BSS( ILU );
                    break;
                case 20:
                    //call BCLR
                    Tri_Instructions.BCLR( ILU );
                    break;
                case 21:
                    //call BRA
                    Tri_Instructions.BRA( ILU );
                    break;
                case 22:
                    //call BEQ
                    Tri_Instructions.BEQ( ILU );
                    break;
                case 23:
                    //call BNE
                    Tri_Instructions.BNE( ILU );
                    break;
                case 24:
                    //call BCS
                    Tri_Instructions.BCS( ILU );
                    break;
                case 25:
                    //call BLT
                    Tri_Instructions.BLT( ILU );
                    break;
                case 26:
                    //call BSUB
                    Tri_Instructions.BSUB( ILU );
                    break;
                case 27:
                    //call RSUB
                    Tri_Instructions.RSUB( ILU );
                    break;
                case 28:
                    //call HLT
                    Tri_Instructions.HLT();
                    break;
                case 29:
                    //call TRP
                    break;
                case 30:
                    //call RTRP
                    break;




            }

        }

        /// <summary>
        /// Converts an Object Code to the Original Instruction
        /// </summary>
        /// <param name="currentInstruction"></param>
        private void reverseAssemble ( Tri_ILU currentInstruction ) {
            string sourceRegister, destinationRegister;
            string h;
            
            //prepare registers to be used later
            #region Register names preparation

            switch ( currentInstruction.sourceReg ) {
                case 0:
                    sourceRegister = "X0";
                    break;
                case 1:
                    sourceRegister = "X1";
                    break;
                case 2:
                    sourceRegister = "X2";
                    break;
                case 3:
                    sourceRegister = "X3";
                    break;
                case 4:
                    sourceRegister = "X4";
                    break;
                case 5:
                    sourceRegister = "X5";
                    break;
                case 6:
                    sourceRegister = "X6";
                    break;
                case 7:
                    sourceRegister = "X7";
                    break;
                default:
                    sourceRegister = "NULL";
                    break;
            }

            switch ( currentInstruction.destinationReg ) {
                case 0:
                    destinationRegister = "X0";
                    break;
                case 1:
                    destinationRegister = "X1";
                    break;
                case 2:
                    destinationRegister = "X2";
                    break;
                case 3:
                    destinationRegister = "X3";
                    break;
                case 4:
                    destinationRegister = "X4";
                    break;
                case 5:
                    destinationRegister = "X5";
                    break;
                case 6:
                    destinationRegister = "X6";
                    break;
                case 7:
                    destinationRegister = "X7";
                    break;
                default:
                    destinationRegister = "NULL";
                    break;
            }
            #endregion


            #region Reverse Instructions
            switch ( currentInstruction.opCode ) {
                case 0:
                    //LD
                    rtbAssembly.Text += "LD";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type the destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source operand
                    switch ( currentInstruction.addressingMode ) {
                        case 0:
                            rtbAssembly.Text += currentInstruction.addressOrImm16OrOffset + "[" + sourceRegister + "]";
                            break;

                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;
                    }
                    break;

                case 1:
                    //call STR function
                    rtbAssembly.Text += "STR";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type the destination register
                    switch ( currentInstruction.addressingMode ) {
                        case 0:
                            rtbAssembly.Text += currentInstruction.addressOrImm16OrOffset + "[" + destinationRegister + "], ";
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 ) + ", ";
                            break;

                        default:
                            break;
                    }

                    //type the source register
                    rtbAssembly.Text += sourceRegister;

                    break;
                case 2:
                    //call LSP
                    rtbAssembly.Text += "LSP.W\t\t";

                    //type source info
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;
                case 3:
                    //call EXG
                    rtbAssembly.Text += "EXG";

                    //type info about operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type info about the destination
                    switch ( currentInstruction.addressingMode ) {
                        case 0:
                            rtbAssembly.Text += currentInstruction.addressOrImm16OrOffset + "[" + destinationRegister + "], ";
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 ) + ", ";
                            break;

                        default:
                            break;
                    }

                    //type the source register
                    rtbAssembly.Text += sourceRegister;

                    break;
                case 4:
                    //call ADD
                    rtbAssembly.Text += "ADD";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 5:
                    //call SUB
                    rtbAssembly.Text += "SUB";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 6:
                    //call MULS
                    rtbAssembly.Text += "MULS";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 7:
                    //call DIVU
                    rtbAssembly.Text += "DIVU";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 8:
                    //call MIN
                    rtbAssembly.Text += "MIN";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 9:
                    //call AND
                    rtbAssembly.Text += "AND";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 10:
                    //call NOT
                    rtbAssembly.Text += "NOT";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 11:
                    //call OR
                    rtbAssembly.Text += "OR";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 12:
                    //call ASHR 
                    rtbAssembly.Text += "ASHR";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 13:
                    //call LSHL
                    rtbAssembly.Text += "LSHL";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 14:
                    //call RCR
                    rtbAssembly.Text += "RCR";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 15:
                    //call PUSH
                    rtbAssembly.Text += "PUSH";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type the source register
                    rtbAssembly.Text += sourceRegister;

                    break;

                case 16:
                    //call POP
                    rtbAssembly.Text += "POP";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type the destination register
                    rtbAssembly.Text += destinationRegister;

                    break;

                case 17:
                    //call CMP
                    rtbAssembly.Text += "CMP";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type the destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 0:
                            rtbAssembly.Text += currentInstruction.addressOrImm16OrOffset + "[" + sourceRegister + "]";
                            break;

                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        case 3:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );
                            break;
                    }

                    break;

                case 18:
                    //call BTST
                    rtbAssembly.Text += "BTST";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 19:
                    //call BSS
                    rtbAssembly.Text += "BSS";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;
                    }


                    break;
                case 20:
                    //call BCLR
                    rtbAssembly.Text += "BCLR";

                    //type operation size
                    if ( currentInstruction.operationSize == 0 )
                        rtbAssembly.Text += ".B\t";
                    else if ( currentInstruction.operationSize == 1 )
                        rtbAssembly.Text += ".W\t";

                    //type destination register
                    rtbAssembly.Text += destinationRegister + ", ";

                    //type info about the source register
                    switch ( currentInstruction.addressingMode ) {
                        case 1:
                            rtbAssembly.Text += sourceRegister;
                            break;

                        case 2:
                            h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                            rtbAssembly.Text += "#$" + h.Substring( h.Length - 4 );
                            break;

                        default:
                            break;
                    }

                    break;

                case 21:
                    //call BRA
                    rtbAssembly.Text += "BRA\t\t";

                    if ( currentInstruction.operationSize == 0 ) {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );

                    } else {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 22:
                    //call BEQ
                    rtbAssembly.Text += "BEQ\t\t";

                    if ( currentInstruction.operationSize == 0 ) {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );

                    } else {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 23:
                    //call BNE
                    rtbAssembly.Text += "BNE\t\t";

                    if ( currentInstruction.operationSize == 0 ) {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );

                    } else {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 24:
                    //call BCS
                    rtbAssembly.Text += "BCS\t\t";

                    if ( currentInstruction.operationSize == 0 ) {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );

                    } else {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 25:
                    //call BLT
                    rtbAssembly.Text += "BLT\t\t";

                    if ( currentInstruction.operationSize == 0 ) {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "$" + h.Substring( h.Length - 4 );

                    } else {
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 26:
                    //call BSUB
                    rtbAssembly.Text += "BSUB\t";

                    if ( currentInstruction.operationSize == 1 ) {
                        rtbAssembly.Text += "&$";
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    } else {
                        rtbAssembly.Text += "$";
                        h = currentInstruction.addressOrImm16OrOffset.ToString( "X4" );
                        rtbAssembly.Text += "&$" + h.Substring( h.Length - 4 );
                    }

                    break;

                case 27:
                    //call RSUB
                    rtbAssembly.Text += "RSUB";
                    break;

                case 28:
                    //call HLT
                    rtbAssembly.Text += "HLT";
                    break;

                case 29:
                    //call TRP
                    rtbAssembly.Text += "TRP";
                    break;
                case 30:
                    //call RTRP
                    rtbAssembly.Text += "RTRP";
                    break;

            }

            #endregion

            rtbAssembly.Text += Environment.NewLine;

        }







    }
}
