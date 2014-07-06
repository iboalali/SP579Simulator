using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SP579emulator {
    public partial class Tri_MainWindow : Form {

        /// <summary>
        /// The Memory of the Simulator
        /// </summary>
        Tri_Byte[] MemoryMap;

        /// <summary>
        /// A StreamWriter to write the Log file to the HardDisk
        /// </summary>
        StreamWriter sw_LOG;

        #region Data Registers

        /// <summary>
        /// Data Register X0
        /// </summary>
        Tri_Word X0 = new Tri_Word();
        

        /// <summary>
        /// Data Register X1
        /// </summary>
        Tri_Word X1 = new Tri_Word();

        /// <summary>
        /// Data Register X2
        /// </summary>
        Tri_Word X2 = new Tri_Word();

        /// <summary>
        /// Data Register X3
        /// </summary>
        Tri_Word X3 = new Tri_Word();

        /// <summary>
        /// Data Register X4
        /// </summary>
        Tri_Word X4 = new Tri_Word();

        /// <summary>
        /// Data Register X5
        /// </summary>
        Tri_Word X5 = new Tri_Word();

        /// <summary>
        /// Data Register X6
        /// </summary>
        Tri_Word X6 = new Tri_Word();

        /// <summary>
        /// Data Register X7
        /// </summary>
        Tri_Word X7 = new Tri_Word();

        #endregion

        #region Other Registers
        /// <summary>
        /// Program Counter
        /// </summary>
        Tri_Word PC = new Tri_Word();

        /// <summary>
        /// Stack Pointer
        /// </summary>
        Tri_StackPointer SP = new Tri_StackPointer();

        /// <summary>
        /// Instruction Logic Unit
        /// </summary>
        Tri_ILU ILU = new Tri_ILU();

        /// <summary>
        /// Condition Code Register
        /// </summary>
        Tri_CCR CCR = new Tri_CCR();

        #endregion

        #region Variable
        /// <summary>
        /// Stores the opened file's name
        /// </summary>
        string fileName;

        /// <summary>
        /// Stroes the path of the last opened file
        /// </summary>
        string path;

        /// <summary>
        /// Stores the Application Log
        /// </summary>
        string appLog;

        /// <summary>
        /// Determines if the Records are loaded into the Memory
        /// </summary>
        bool isLoadedIntoMemory;

        /// <summary>
        /// Determines if a file has been opened
        /// </summary>
        bool isFileOpened;

        /// <summary>
        /// Determines if changes was made to the opened file
        /// </summary>
        bool isChangesMade;

        /// <summary>
        /// Determines if the execution started
        /// </summary>
        bool isExecutionStarted;

        /// <summary>
        /// Determines if the step by step execution
        /// </summary>
        bool isStepByStepExecutionStarted;

        /// <summary>
        /// Determines if the opened file has no errors
        /// </summary>
        bool hasNoError;

        /// <summary>
        /// Determines if the Machine State is in Binary numbers
        /// </summary>
        bool isMachineStateBinary;

        /// <summary>
        /// Determines if the Machine State is in Decimal numbers
        /// </summary>
        bool isMachineStateDecimal;

        /// <summary>
        /// Determines if the Machine State is in HEX numbers
        /// </summary>
        bool isMachineStateHex;

        string lastInstructionAddress;
        int numberOfRecord;
        int lineNumber;
        string[] records;
        string logDate;
        public static int memoryPositon;

        #endregion

        public Tri_MainWindow() {
            this.Icon = global::SP579emulator.Properties.Resources.app_icon;

            #region Variable Initialization

            fileName = string.Empty;
            appLog = string.Empty;
            isLoadedIntoMemory = false;
            isFileOpened = false;
            isChangesMade = false;
            isExecutionStarted = false;
            isMachineStateBinary = false;
            isMachineStateDecimal = false;
            isMachineStateHex = true;
            isStepByStepExecutionStarted = false;
            path = string.Empty;
            string lastInstructionAddress = "00";
            lineNumber = 1;
            memoryPositon = 0;
            logDate = DateTime.Now.ToString( "dd.MM.yyyy_HHmmss" );

            #endregion

            #region Memory Map Initialization

            MemoryMap = new Tri_Byte[65536];
            for ( int i = 0; i < MemoryMap.Length; i++ ) {
                MemoryMap[i] = new Tri_Byte();
            }

            #endregion 

            #region Instruction class intitialization
            Tri_Instructions.form = this;
            
            Tri_Instructions.MemoryMap = MemoryMap;


            Tri_Instructions.CPUregisters[0] = this.X0;
            Tri_Instructions.CPUregisters[1] = this.X1;
            Tri_Instructions.CPUregisters[2] = this.X2;
            Tri_Instructions.CPUregisters[3] = this.X3;
            Tri_Instructions.CPUregisters[4] = this.X4;
            Tri_Instructions.CPUregisters[5] = this.X5;
            Tri_Instructions.CPUregisters[6] = this.X6;
            Tri_Instructions.CPUregisters[7] = this.X7;

            Tri_Instructions.PC = this.PC;
            Tri_Instructions.SP = this.SP;
            Tri_Instructions.CCR = this.CCR;


            #endregion
            
            InitializeComponent();

            #region Register Event Handlers

            X0.SetTextBox( txtDataReg1 );
            X1.SetTextBox( txtDataReg2 );
            X2.SetTextBox( txtDataReg3 );
            X3.SetTextBox( txtDataReg4 );
            X4.SetTextBox( txtDataReg5 );
            X5.SetTextBox( txtDataReg6 );
            X6.SetTextBox( txtDataReg7 );
            X7.SetTextBox( txtDataReg8 );
            PC.SetTextBox( txtPC );
            SP.SetTextBox( txtSP );
            CCR.SetTextBox( new TextBox[] { txtCCRz, txtCCRv, txtCCRn, txtCCRc } );

            X0.OnUpdate += X_OnUpdate;
            X1.OnUpdate += X_OnUpdate;
            X2.OnUpdate += X_OnUpdate;
            X3.OnUpdate += X_OnUpdate;
            X4.OnUpdate += X_OnUpdate;
            X5.OnUpdate += X_OnUpdate;
            X6.OnUpdate += X_OnUpdate;
            X7.OnUpdate += X_OnUpdate;
            PC.OnUpdate += X_OnUpdate;
            SP.OnUpdate += SP_OnUpdate;
            CCR.OnUpdate += CCR_OnUpdate;

            #endregion
            X0.Set( 0 );
            X1.Set( 0 );
            X2.Set( 0 );
            X3.Set( 0 );
            X4.Set( 0 );
            X5.Set( 0 );
            X6.Set( 0 );
            X7.Set( 0 );
            PC.Set( 0 );
            this.SP.Value = 0x0000;

        }

        void CCR_OnUpdate( object sender, Tri_CCRArgs e ) {
            ( sender as Tri_CCR ).SetValue( e.value, e.flag );
        }

        void SP_OnUpdate( object sender, Tri_MachineStateArgs e ) {
            updateMachineState( e.txtBox, e.intValue );
        }

        void X_OnUpdate( object sender, Tri_MachineStateArgs e ) {
            updateMachineState( e.txtBox, e.intValue );
        }

        #region Event Handlers

        private void Form1_Load( object sender, EventArgs e ) {
            reorderComponent();
        }

        private void Form1_SizeChanged( object sender, EventArgs e ) {
            reorderComponent();

        }

        private void richTextBox1_KeyPress( object sender, KeyPressEventArgs e ) {
            e.Handled = true;
        }

        private void rtbEditor_TextChanged( object sender, EventArgs e ) {
            if ( isFileOpened ) {
                isChangesMade = true;                
            }
        }

        private void errorsToolStripMenuItem_CheckedChanged( object sender, EventArgs e ) {
            if ( ( sender as ToolStripMenuItem ).Checked ) {
                writeLog( "Button Clicked: View Error Window" );
                rtbError.Visible = true;

            } else {
                writeLog( "Button Clicked: Hide Error Window" );
                rtbError.Visible = false;

            }
            reorderComponent();

        }

        private void assembleyToolStripMenuItem_CheckedChanged( object sender, EventArgs e ) {
            if ( ( sender as ToolStripMenuItem ).Checked ) {
                writeLog( "Button Clicked: View Assembly Window" );
                rtbAssembly.Visible = true;
                lblAssembly.Visible = true;

            } else {
                writeLog( "Button Clicked: Hide Assembly Window" );
                rtbAssembly.Visible = false;
                lblAssembly.Visible = false;

            }
            reorderComponent();

        }

        private void rtbAssembly_KeyPress( object sender, KeyPressEventArgs e ) {
            e.Handled = true;
        }

        protected override void OnFormClosing( FormClosingEventArgs e ) {
            base.OnFormClosing( e );

            if ( e.CloseReason == CloseReason.WindowsShutDown ) {
                if ( fileName != string.Empty ) {
                    saveFile();
                }

                return;
            }

            // Confirm user wants to close
            if ( isChangesMade ) {
                switch ( MessageBox.Show( this, "Are you sure you want to save the file then close?",
                                                "Save and Close", MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Warning ) ) {

                    case DialogResult.Yes:
                        if ( fileName != string.Empty ) {
                            saveFile();
                        } else {
                            saveFileAs();
                        }

                        break;
                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;

                    default:
                        break;
                }
            }
        }

        #region Data Register Event Handlers

        private void txtDataReg_TextChanged( object sender, EventArgs e ) {
            // Select the digit the cursor is at
            ( sender as TextBox ).Select( ( sender as TextBox ).SelectionStart, 1 );
            
        }

        private void txtDataReg_Click( object sender, EventArgs e ) {
            // Select the digit the cursor is at
            ( sender as TextBox ).Select( ( sender as TextBox ).SelectionStart, 1 );
        }

        private void txtDataReg_KeyPress( object sender, KeyPressEventArgs e ) {

            // Overriding the backspace key
            if ( e.KeyChar == ( char ) Keys.Back ) {
                e.Handled = true;
                if ( ( sender as TextBox ).SelectionStart > 0 ) {
                    int s = ( sender as TextBox ).SelectionStart;
                    ( sender as TextBox ).Text = ( sender as TextBox ).Text.Remove( s - 1, 1 );
                    ( sender as TextBox ).Text = ( sender as TextBox ).Text.Insert( s - 1, "0" );
                    ( sender as TextBox ).SelectionStart = s - 1;
                }

                return;
            }

            if ( isMachineStateHex ) {
                char c = char.ToUpper( e.KeyChar );

                // Only allow digits including HEX numbers
                if ( Char.IsDigit( e.KeyChar ) || c == 'A' || c == 'B'
                        || c == 'C' || c == 'D' || c == 'E' || c == 'F' ) {

                    // Overriding the input of the lower case letter and replacing it 
                    // with the upper case letter
                    if ( char.IsLower( e.KeyChar ) ) {
                        e.Handled = true;
                        int s = ( sender as TextBox ).SelectionStart;
                        if ( s >= ( sender as TextBox ).MaxLength ) {
                            System.Media.SystemSounds.Asterisk.Play();
                            return;
                        }
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Remove( s, 1 );
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Insert( s, c.ToString() );
                        ( sender as TextBox ).SelectionStart = s + 1;
                    }

                } else {
                    e.Handled = true;
                }

            } else if ( isMachineStateDecimal ) {
                if ( Char.IsDigit( e.KeyChar ) ) {

                    //// Overriding the input of the lower case letter and replacing it 
                    //// with the upper case letter
                    //if ( char.IsLower( e.KeyChar ) ) {
                        e.Handled = true;
                        int s = ( sender as TextBox ).SelectionStart;

                        if ( s >= ( sender as TextBox ).MaxLength ) {
                            System.Media.SystemSounds.Asterisk.Play();
                            return;
                        }
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Remove( s, 1 );
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Insert( s, e.KeyChar.ToString() );
                        ( sender as TextBox ).SelectionStart = s + 1;
                    //}

                } else {
                    e.Handled = true;
                }
            } else if ( isMachineStateBinary ) {
                if ( e.KeyChar == '1' || e.KeyChar == '0' ) {

                    //// Overriding the input of the lower case letter and replacing it 
                    //// with the upper case letter
                    //if ( char.IsLower( e.KeyChar ) ) {
                        e.Handled = true;
                        int s = ( sender as TextBox ).SelectionStart;
                        if ( s >= ( sender as TextBox ).MaxLength ) {
                            System.Media.SystemSounds.Asterisk.Play();
                            return;
                        }
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Remove( s, 1 );
                        ( sender as TextBox ).Text = ( sender as TextBox ).Text.Insert( s, e.KeyChar.ToString() );
                        ( sender as TextBox ).SelectionStart = s + 1;
                    //}

                } else {
                    e.Handled = true;
                }
            }
            

            // Select the digit the cursor is at (the next digit)
            ( sender as TextBox ).Select( ( sender as TextBox ).SelectionStart, 1 );

        }

        private void txtCCR_KeyPress( object sender, KeyPressEventArgs e ) {
            if ( e.KeyChar == ( char ) Keys.Back ) {
                e.Handled = true;
                ( sender as TextBox ).Text = "0";

            }

            if ( e.KeyChar == '1' || e.KeyChar == '0' ) {
                ( sender as TextBox ).Text = ( sender as TextBox ).Text = e.KeyChar.ToString();

            }

            e.Handled = true;


        }

        #endregion

        #region Tool Strip Menu Click Events

        private void exitToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Exit Application" );
            
            this.Close();
            Environment.Exit( Environment.ExitCode );
        }

        private void memoryToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Memory Map" );
            writeLog( "Opened Memory Map" );
            new Tri_MemoryMap( ref MemoryMap ).Show();
        }

        private void openToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Open File" );
            openFile();
        }

        private void pasteToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Paste:" + Clipboard.GetText() );
            isFileOpened = true;
            rtbEditor.Paste();
        }

        private void loadIntoMemoryToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Load into Memory" );
            load();

        }

        private void checksumCalculatorToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Checksum Calculator" );
            writeLog( "Opened Checksum Calculator" );
            new Tri_CheckSumCalc().Show();
        }

        private void redoToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Redo Change" );
            rtbEditor.Redo();
        }

        private void undoToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Undo Change" );

            rtbEditor.Undo();
        }

        private void executeToolStripMenuItem_Click( object sender, EventArgs e ) {
            if ( tsbExecute.Text == "Execute" ) {
                writeLog( "Button Clicked: Execute" );
                writeLog( "Execution Started" );

                if ( !isStepByStepExecutionStarted ) {
                    reset();
                }

                isExecutionStarted = true;
                //tsbExecute.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                //tsbExecute.Text = "Stop";
                //tsbExecute.ToolTipText = "Stop (Ctrl+F5)";
                //
                //executeToolStripMenuItem.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                //executeToolStripMenuItem.Text = "Stop";
                //executeToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys ) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5 ) ) );

                execute();


            } else {
                writeLog( "Button Clicked: Stop" );
                writeLog( "Execution Stopped" );

                tsbExecute.Image = global::SP579emulator.Properties.Resources.Symbols_Play_16xLG;
                tsbExecute.Text = "Execute";
                tsbExecute.ToolTipText = "Execute (F5)";

                executeToolStripMenuItem.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                executeToolStripMenuItem.Text = "Execute";
                executeToolStripMenuItem.ShortcutKeys = Keys.F5;
                reset();
            }
            
        }

        private void resetToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Reset" );
            reset();
        }

        private void logToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Application Log" );
            writeLog( "Opened Application Log" );
            new Tri_Application_Log( appLog ).Show();
        }

        private void copyToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Copy Selected:" + rtbEditor.SelectedText );
            rtbEditor.Copy();
        }

        private void cutToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Cut Selected:" + rtbEditor.SelectedText );

            rtbEditor.Cut();
        }

        private void deleteToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Delete Selected:" + rtbEditor.SelectedText );
            
            // error
            int l = rtbEditor.SelectionLength;
            if ( l == 0 ) {
                l = 1;
            }
            if ( rtbEditor.Text.Length < l  ) {
                l = rtbEditor.Text.Length;
            } else if ( rtbEditor.Text == "" ) {
                l = 0;
                System.Media.SystemSounds.Asterisk.Play();
            }
            try {
                rtbEditor.Text = rtbEditor.Text.Remove( rtbEditor.SelectionStart, l );

            } catch {
                
                
            }
        }

        private void selectAllToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Select All:" + rtbEditor.SelectedText );
            rtbEditor.SelectAll();
        }

        private void closeToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Close File" );
            isFileOpened = false;
            reset();
            rtbEditor.Clear();
            isFileOpened = false;
        }

        private void restartToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Restart" );

            X0.SetAll( false );
            X1.SetAll( false );
            X2.SetAll( false );
            X3.SetAll( false );
            X4.SetAll( false );
            X5.SetAll( false );
            X6.SetAll( false );
            X7.SetAll( false );
            PC.SetAll( false );
            SP.Value = 0x0000;

            for ( int i = 0; i < records.Length; i++ ) {
                records[i] = string.Empty;
            }

            rtbAssembly.Clear();
            rtbError.Clear();
            rtbEditor.BackColor = System.Drawing.Color.White;

            load();
        }

        private void saveToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Save File" );
            saveFile();
        }

        private void saveAsToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Save File As" );
            saveFileAs();

        }

        private void executeToTheSelectedLineToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Execute To the Selected Line" );
            if ( isExecutionStarted ) {
                reset();
            }

            if ( !isStepByStepExecutionStarted ) {
                tsbExecute.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                tsbExecute.Text = "Stop     ";
                tsbExecute.ToolTipText = "Stop (Ctrl+F5)";

                executeToolStripMenuItem.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                executeToolStripMenuItem.Text = "Stop";
                executeToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys ) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5 ) ) );

                openToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;

                tsbOpenFile.Enabled = false;
                tsbClose.Enabled = false;
                tsbUndo.Enabled = false;
                tsbRedo.Enabled = false;
                tsbSave.Enabled = false;

                isStepByStepExecutionStarted = true;
            }

            if ( !isLoadedIntoMemory ) {
                load();
            }
            int sl = rtbEditor.SelectionStart;
            int lineN = rtbEditor.GetLineFromCharIndex( sl );

            while ( lineNumber <= lineN ) {
                if ( PC.ToIntU() > Convert.ToInt16( lastInstructionAddress, 16 ) ) {
                    System.Media.SystemSounds.Asterisk.Play();
                    break;

                } else {
                    fetchAndDecode( PC.ToIntU() );
                    //txtPC.Text = PC.ToHEX().PadLeft( 4, '0' ); //update PC for next instruction

                    reverseAssemble( ILU ); //method that finds the reverse assembly of the current instruction in the ILU
                }

            }
            
        }

        private void userGuideToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: User Guide" );

            if ( File.Exists( "UserGuide.pdf" ) ) {
                System.Diagnostics.Process.Start( "UserGuide.pdf" );
            } else {
                MessageBox.Show( "User Guide not found, Please contact the developers.",
                            "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void aboutToolStripMenuItem_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: About" );
            new Tri_About().Show();
        }

        private void binaryToolStripMenuItem_Click( object sender, EventArgs e ) {

            if ( isMachineStateDecimal ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {

                            ( item as TextBox ).Text = Convert.ToString( int.Parse( ( item as TextBox ).Text ), 2 ).PadLeft( 16, '0' );
                            ( item as TextBox ).MaxLength = 16;

                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "0000000000000000";
                            //}

                        }
                    }
                }

                binaryToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateBinary = true;
                decimalToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateDecimal = false;

            } else if ( isMachineStateHex ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {

                                ( item as TextBox ).Text = Convert.ToString(
                                    int.Parse( ( item as TextBox ).Text,
                                                System.Globalization.NumberStyles.HexNumber )
                                    , 2 ).PadLeft( 16, '0' );
                            ( item as TextBox ).MaxLength = 16;
                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "0000000000000000";
                            //}
                        }
                    }
                }

                binaryToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateBinary = true;
                hEXToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateHex = false;
            }

        }

        private void decimalToolStripMenuItem_Click( object sender, EventArgs e ) {

            if ( isMachineStateBinary ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {

                            ( item as TextBox ).Text = binaryToInt( ( item as TextBox ).Text ).ToString().PadLeft( 5, '0' ); ;
                            ( item as TextBox ).MaxLength = 5;

                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "00000";
                            //}

                        }
                    }
                }

                decimalToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateDecimal = true;
                binaryToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateBinary = false;

            } else if ( isMachineStateHex ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {


                            ( item as TextBox ).Text = int.Parse( ( item as TextBox ).Text, System.Globalization.NumberStyles.HexNumber ).ToString().PadLeft( 5, '0' );
                            ( item as TextBox ).MaxLength = 5;

                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "00000";
                            //}
                        }
                    }
                }

                decimalToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateDecimal = true;
                hEXToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateHex = false;
            }
        }

        private void hEXToolStripMenuItem_Click( object sender, EventArgs e ) {
            if ( isMachineStateBinary ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {

                            ( item as TextBox ).Text = binaryToInt( ( item as TextBox ).Text ).ToString( "X" ).PadLeft( 4, '0' );
                            ( item as TextBox ).MaxLength = 4;

                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "0000";
                            //}

                        }
                    }
                }

                hEXToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateHex = true;
                binaryToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateBinary = false;

            } else if ( isMachineStateDecimal ) {

                foreach ( var item in this.Controls ) {
                    if ( item is TextBox ) {
                        if ( ( item as TextBox ).Name != "txtCCRc"
                            && ( item as TextBox ).Name != "txtCCRn"
                            && ( item as TextBox ).Name != "txtCCRv"
                            && ( item as TextBox ).Name != "txtCCRz" ) {

                            ( item as TextBox ).Text = int.Parse( ( item as TextBox ).Text ).ToString( "X" ).PadLeft( 4, '0' );
                            ( item as TextBox ).MaxLength = 4;

                            //if ( !isExecutionStarted ) {
                            //    ( item as TextBox ).Text = "0000";
                            //}
                        }
                    }
                }
                hEXToolStripMenuItem.CheckState = CheckState.Checked;
                isMachineStateHex = true;
                decimalToolStripMenuItem.CheckState = CheckState.Unchecked;
                isMachineStateDecimal = false;
            }
        }


        private void tsbExecuteStep_Click( object sender, EventArgs e ) {
            writeLog( "Button Clicked: Execute Step by Step" );
            // fetchAndDecode get the instruction pointed at by PC, and split it to fill the ILU struct object, then return;
            // switch the opcode in the struct then call the appropriate function to execute it.

            if ( isExecutionStarted ) {
                reset();
            }

            if ( !isStepByStepExecutionStarted ) {
                tsbExecute.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                tsbExecute.Text = "Stop     ";
                tsbExecute.ToolTipText = "Stop (Ctrl+F5)";

                executeToolStripMenuItem.Image = global::SP579emulator.Properties.Resources.Symbols_Stop_16xLG;
                executeToolStripMenuItem.Text = "Stop";
                executeToolStripMenuItem.ShortcutKeys = ( ( System.Windows.Forms.Keys ) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5 ) ) );

                openToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;

                tsbOpenFile.Enabled = false;
                tsbClose.Enabled = false;
                tsbUndo.Enabled = false;
                tsbRedo.Enabled = false;
                tsbSave.Enabled = false;

                isStepByStepExecutionStarted = true;
            }

            if ( !isLoadedIntoMemory ) {
                load();
            }

            if ( PC.ToIntU() > Convert.ToInt16( lastInstructionAddress, 16 ) ) {
                System.Media.SystemSounds.Asterisk.Play();


            } else {
                fetchAndDecode( PC.ToIntU() ); 
                //txtPC.Text = PC.ToHEX().PadLeft( 4, '0' ); //update PC for next instruction

                reverseAssemble( ILU ); //method that finds the reverse assembly of the current instruction in the ILU
            }

        }

        #endregion

        #endregion

    }
}