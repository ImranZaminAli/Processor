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
        public delegate void Mem();
        public Unit result;
        public int executionCycles;
        public int pc;
        private bool stalled;
        private bool busy;
        public Execution ExecutionDelegate { get; set; }

        public Mem MemDelegate { get; set; }

        public bool Empty { get => empty; set => empty = value; }

        public bool Stalled { get => stalled; set => stalled = value; }

        public Instruction Instruction { get => instruction; set => instruction = value; }
        public bool Busy { get => busy; set => busy = value; }

        public void SetResult(object result) => this.result = (Unit) result;

        public PipelineRegister()
        {
            empty = true;
            operands = new Unit[0];
            stalled = false;
            busy = false;
            executionCycles = 0;
            ExecutionDelegate = delegate () { ; };
            MemDelegate = delegate () { ; };
        }
        public PipelineRegister(Instruction instruction)
        {
            this.instruction = instruction;
            empty = false;
            stalled = false;
            executionCycles = 0;
            busy = false;
            ExecutionDelegate = delegate () {; };
            MemDelegate = delegate () { ; };
        }

        public override string ToString()
        {
            string operandStr = "";
            foreach(var operand in operands) { operandStr += operand.ToString() + " "; }
            return String.Format("instruction: {0}\nempty: {1}, opcode: {2}\noperands: {3}\n result: {4}\ncycles: {5}", 
                instruction.ToString(), empty, opcode, operandStr, result == null? "" : result.ToString(), executionCycles);
        }
    }
}
