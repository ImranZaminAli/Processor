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
        public Optype optype;
        public ReservationStationEntry input;
        public ExecuteUnit()
        {
            branchInstructions = new string[]{ "GOTO", "JMP", "BR"};
            busy = false;
        }

        public ExecuteUnit(Optype optype)
        {
            this.optype = optype;
        }

        public ReservationStationEntry Run(ReservationStationEntry entry) 
        {
            busy = true;
            entry.cycles--;
            if (entry.cycles == 0)
            {
                // entry.isFree = true;
                entry.result = entry.execution(entry.values);
                busy = false;
                return (ReservationStationEntry) entry.Clone();
            }
            return null;
        }

        public virtual ReservationStationEntry Run() 
        {

            busy = true;
            input.cycles--;
            if (input.cycles == 0)
            {
                // entry.isFree = true;
                input.result = input.execution(input.values);
                busy = false;
                return (ReservationStationEntry) input.Clone();
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

    class LsExecuteUnit : ExecuteUnit
    {

        public Lsq lsq;
        public LsExecuteUnit(Optype optype, Lsq lsq)
        {
            this.optype = optype;
            this.lsq = lsq;
        }

        public override ReservationStationEntry Run()
        {
            busy = true;
            input.cycles--;
            if (input.cycles != 0)
                return null;

            if (lsq.queue[0].value != -1)
                input.result = input.execution(lsq.queue[0].value);
            busy = false;
            lsq.queue.RemoveAt(0);
            return (ReservationStationEntry) input.Clone();
        }
    }
}
