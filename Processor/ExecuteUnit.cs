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

        public PipelineRegister Run(PipelineRegister pipelineRegister, ref bool finished, ref int pc) 
        {
            pipelineRegister.ExecutionDelegate();
            if (pipelineRegister.opcode == "HLT")
            {
                finished = true;
            }
            if (branchInstructions.Contains(pipelineRegister.opcode))
                pc = pipelineRegister.pc;
            return pipelineRegister;
        }
    }
}
