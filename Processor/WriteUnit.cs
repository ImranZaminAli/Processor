using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class WriteUnit
    {
        private string[] notWriteInstructions;
        public WriteUnit()
        {
            notWriteInstructions = new string[] { "JMP", "BR", "GOTO", "LABEL", "HLT"};
        }
        public PipelineRegister Run(PipelineRegister pipelineRegister, ref int cycles)
        {
            if (!notWriteInstructions.Contains(pipelineRegister.opcode))
                pipelineRegister.operands[0].value = pipelineRegister.result.value;
            cycles += pipelineRegister.cycles;
            return pipelineRegister;
        }
    }
}
