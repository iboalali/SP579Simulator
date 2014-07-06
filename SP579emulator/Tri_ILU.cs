using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SP579emulator {
    public struct Tri_ILU {


        public int opCode;

        /// <summary>
        /// operationSize values: 
        /// 0 --> Byte 
        /// 1 --> Word 
        /// </summary>
        public int operationSize;

        /// <summary>
        /// 0 --> Register Indirect with displacement 
        /// 1 --> Register Addressing 
        /// 2 --> Immediate 
        /// 3 --> Absolute 
        /// 
        /// modes 0,2,3 need two more bytes from memory 
        /// </summary>
        public int addressingMode;


        public int sourceReg;
        public int destinationReg;
        //the remaining (14,15) is a 00 padding, if we need to fetch two more bytes 
        //the starting index to get the address will be 16 

        public int addressOrImm16OrOffset;

        public int lineNumber;

    }

}
