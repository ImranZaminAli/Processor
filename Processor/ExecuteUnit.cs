using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class ExecuteUnit
    {
        private string[] branchInstructions;

        public ExecuteUnit()
        {
            branchInstructions = new string[]{ "GOTO", "JMP", "BR"};
        }

        public PipelineRegister Run(PipelineRegister pipelineRegister, ref int pc) 
        {
            pipelineRegister.executionCycles--;
            if (pipelineRegister.executionCycles == 0)
            {
                pipelineRegister.Busy = false;
                pipelineRegister.ExecutionDelegate();
                if (branchInstructions.Contains(pipelineRegister.opcode))
                    pc = pipelineRegister.pc;
            }
            else
                pipelineRegister.Busy = true;
            return pipelineRegister;
        }
    }
}
