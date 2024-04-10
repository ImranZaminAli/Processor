using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography;

namespace Processor
{
    
    class Instruction
    { 

        private string instructionLine;
        private string opcode;
        private int[] operands;
        public string InstructionLine { get => instructionLine; private set => instructionLine = value; }
        public string Opcode { get => opcode; set => opcode = value; }
        public int[] Operand { get => operands; set => operands = value; }

        public Instruction(string instruction)
        {
            InstructionLine = instruction;
            string[] tokens = instruction.Split(' ');
            opcode = tokens[0];
            operands = new int[tokens.Length - 1];
            for(int i = 0; i < tokens.Length - 1; i++)
            {
                operands[i] = int.TryParse(tokens[i+1], out int parsed) ? parsed : tokens[i+1].GetHashCode();
            }

        }

        public Instruction()
        {
            opcode = "NOP";
            operands = new int[0];
        }

        public void SetOpcode(string opcode) => this.Opcode = opcode;

        public void SetOperand(int[] operand) => this.Operand = operand;

        public override string ToString()
        {
            string returnString = "opcode: " + opcode + "\noperands:";
            foreach (var item in operands)
            {
                //returnString.(" " + item.ToString());
                returnString += " " + item.ToString();
            }

            return returnString;
        }
    }
}
