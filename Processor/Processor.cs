using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Processor
    {
        private Instruction[] instructions;
        private int pc;
        private int cycles;
        private bool finished;
        private Unit[] registers;
        private Unit[] memory;
        private Dictionary<int, int> labelMap; // label to line
        private PipelineRegister[] pipelineRegisters;
        private FetchUnit fetchUnit;
        private DecodeUnit decodeUnit;
        private ExecuteUnit executeUnit;
        private MemUnit memUnit;
        private WriteUnit writeUnit;

        public Processor(Instruction[] instructions)
        {
            this.instructions = instructions;
            pc = 0;
            finished = false;
            registers = new Unit[32];
            memory = new Unit[32];
            for (int i = 0; i < 32; i++)
            {
                registers[i] = new RegisterUnit();
                memory[i] = new MemoryUnit();
            }
            labelMap = new Dictionary<int, int>();
            pipelineRegisters = new PipelineRegister[5];
            for (int i = 0;i < pipelineRegisters.Length ;i++)
                pipelineRegisters[i] = new PipelineRegister();
            fetchUnit = new FetchUnit(instructions);
            decodeUnit = new DecodeUnit();
            executeUnit = new ExecuteUnit();
            memUnit = new MemUnit();
            writeUnit = new WriteUnit();
        }

        private bool transferPipelineReg(ref PipelineRegister oldReg, PipelineRegister newReg){
            if (!newReg.stall)
            {
                oldReg = newReg;
                
                return true;
            }
            return false;
        }

        private void transferPipeline() {
            if (!transferPipelineReg(ref pipelineRegisters[4], pipelineRegisters[3]))
                return;
            if (!transferPipelineReg(ref pipelineRegisters[3], pipelineRegisters[2]))
                return;

            if (!transferPipelineReg(ref pipelineRegisters[2], pipelineRegisters[1]))
                return;

            if (!transferPipelineReg(ref pipelineRegisters[1], pipelineRegisters[0]))
                return;
            else
                pipelineRegisters[0] = new PipelineRegister();
        }

        public void Run()
        {
            while (!finished) 
            {
                // Instruction instruction;
                if (!pipelineRegisters[0].Empty || !pipelineRegisters[0].stall)
                {
                    pc = fetchUnit.Run(pc, pipelineRegisters[0]);
                }

                if (!pipelineRegisters[1].Empty || !pipelineRegisters[1].stall)
                {
                    pipelineRegisters[1] = decodeUnit.Run(pipelineRegisters[1], registers, memory, labelMap);
                }

                if (!pipelineRegisters[2].Empty || !pipelineRegisters[2].stall)
                {
                    pipelineRegisters[2] = executeUnit.Run(pipelineRegisters[2], ref finished, ref pc);
                }

                if (!pipelineRegisters[3].Empty || !pipelineRegisters[3].stall)
                {
                    pipelineRegisters[3] = memUnit.Run(pipelineRegisters[3]);
                }
                
                if (!pipelineRegisters[4].Empty || !pipelineRegisters[4].stall)
                {
                    pipelineRegisters[4] = writeUnit.Run(pipelineRegisters[4], ref cycles);
                }


                transferPipeline();


                // not pipelined
                //pipelineRegisters[0] = new PipelineRegister();
                //pc = fetchUnit.Run(pc, pipelineRegisters[0]);
                //pipelineRegisters[0] = decodeUnit.Run(pipelineRegisters[0], registers, memory, labelMap);
                //pipelineRegisters[0] = executeUnit.Run(pipelineRegisters[0], ref finished, ref pc);
                //pipelineRegisters[0] = memUnit.Run(pipelineRegisters[0]);
                //pipelineRegisters[0] = writeUnit.Run(pipelineRegisters[0], ref cycles);

            }

            Console.WriteLine("{0} {1} {2}, {3}", registers[0].value, registers[1].value, registers[2].value, registers[3].value);
        }
    }
}
