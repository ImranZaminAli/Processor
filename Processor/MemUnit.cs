using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class MemUnit
    {
        public PipelineRegister Run(PipelineRegister pipelineRegister) 
        {
            if (pipelineRegister.opType == OpType.MEM)
                pipelineRegister.MemAccessDelegate();
            return pipelineRegister;
        }

    }
}
