using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class FetchUnit
    {
        private Instruction[] instructions;
        private bool busy;

        public FetchUnit(Instruction[] instructions) 
        { 
            this.instructions = instructions;
            busy = false;
        }

        public bool Busy { get => busy; set => busy = value; }

        public int Run(int pc, PipelineRegister pipelineRegister)
        {
            if (!busy)
            {
                try
                {
                    pipelineRegister.Instruction = instructions[pc];
                    pipelineRegister.pc = pc;
                    pipelineRegister.Empty = false;
                    return pc + 1;
                }
                catch(IndexOutOfRangeException)
                {
                    pipelineRegister.Instruction = new Instruction();
                    pipelineRegister.pc = pc;
                    pipelineRegister.Empty = true;
                    return pc;
                }
            }
            return pc;
        }
    }
}
