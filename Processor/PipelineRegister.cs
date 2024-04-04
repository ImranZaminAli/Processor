using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class PipelineRegister
    {
        private Instruction instruction;
        private bool empty;
        public string opcode;
        public Unit[] operands;
        public delegate void Execution();
        public Unit result;
        public int cycles;
        public int pc;
        public bool stall;
        public bool flush;
        public Execution ExecutionDelegate { get; set; }

        public bool Empty { get => empty; set => empty = value; }
        public Instruction Instruction { get => instruction; set => instruction = value; }

        public void SetResult(object result) => this.result = (Unit) result;

        public PipelineRegister()
        {
            empty = true;
            stall = false;
            flush = false;
            operands = new Unit[0];
        }
        public PipelineRegister(Instruction instruction)
        {
            this.instruction = instruction;
            empty = false;
            stall = false;
            flush = false;
        }

        public override string ToString()
        {
            string operandStr = "";
            foreach(var operand in operands) { operandStr += operand.ToString() + " "; }
            return String.Format("instruction: {0}\nempty: {1}, opcode: {2}\noperands: {3}\n result: {4}\ncycles: {5}", 
                instruction.ToString(), empty, opcode, operandStr, result == null? "" : result.ToString(), cycles);
        }
    }
}
