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
        public bool busy;
        public ExecuteUnit()
        {
            branchInstructions = new string[]{ "GOTO", "JMP", "BR"};
            busy = false;
        }

        public ReservationStationEntry Run(ReservationStationEntry entry) 
        {
            busy = true;
            entry.cycles--;
            if (entry.cycles == 0)
            {
                // entry.isFree = true;
                entry.result = entry.execution(entry.values);
                if(entry.result == -1)
                    Console.WriteLine("here");
                busy = false;
                return (ReservationStationEntry) entry.Clone();
            }
            return null;
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
