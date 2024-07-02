using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class WriteUnit
    {
        private string[] notWriteInstructions;
        public WriteUnit()
        {
            notWriteInstructions = new string[] { "JMP", "BR", "GOTO", "LABEL", "HLT", "NOP"};
        }
        public PipelineRegister Run(PipelineRegister pipelineRegister, ref int cycles, ref bool finished)
        {
            if (!notWriteInstructions.Contains(pipelineRegister.opcode))
            {
                pipelineRegister.operands[0].value = pipelineRegister.result.value;
                pipelineRegister.operands[0].TryFree(pipelineRegister);
            }
            cycles += pipelineRegister.cycles;

            if (pipelineRegister.opcode == "HLT")
                finished = true;
            return pipelineRegister;
        }
    }
}
