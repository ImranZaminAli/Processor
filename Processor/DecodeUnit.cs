using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class DecodeUnit
    {
        bool busy;

        public bool Busy { get => busy; set => busy = value; }

        public DecodeUnit() => busy = false;

        // TODO: LD, ST, 
        public PipelineRegister Run(PipelineRegister pipelineRegister, Unit[] registers, Unit[] memory, Dictionary<int, int> labelMap)
        {
            Instruction instruction = pipelineRegister.Instruction;
            //string[] tokens = instruction.InstructionLine.Split(' ');
            //instruction.Opcode = tokens[0];
            //instruction.Operand = new int[tokens.Length - 1];
            //for (int i = 0; i < tokens.Length - 1; i++)
            //{
            //    instruction.Operand[i] = int.TryParse(tokens[i + 1], out int parsed) ? parsed : tokens[i + 1].GetHashCode();
            //}
            pipelineRegister.operands = new Unit[instruction.Operand.Length];
            var operands = pipelineRegister.operands;
            var instructionOperands = instruction.Operand;
            switch (instruction.Opcode)
            {
                case "ADD":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value + operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "SUB":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value - operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "MUL":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value * operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "ADDI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value + operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "SUBI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value - operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "MULI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value * operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "AND":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value & operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "OR":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value | operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "NOT":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value == 1 ? 0 : 1;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "EQ":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value == operands[2].value ? 1 : 0;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "LT":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value < operands[2].value ? 1 : 0;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "GT":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = registers[instructionOperands[2]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value > operands[2].value ? 1 : 0;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "ANDI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value & operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "ORI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value | operands[2].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "NOTI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = new ImmediateUnit(instructionOperands[1]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value == 1 ? 0 : 1;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "EQI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    operands[2] = new ImmediateUnit(instructionOperands[2]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value == operands[2].value ? 1 : 0;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "LD":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = memory[instructionOperands[1]];
                    pipelineRegister.MemDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "LDI":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = new ImmediateUnit(instructionOperands[1]);
                    pipelineRegister.MemDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "ST":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = memory[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    pipelineRegister.MemDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "MOV":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[instructionOperands[1]];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "MOVIND":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = registers[registers[instructionOperands[1]].value];
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "JMP":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = new ImmediateUnit();
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        pipelineRegister.pc = labelMap[operands[0].value];
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "BR":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = new ImmediateUnit(instructionOperands[1]);
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        if (operands[0].value == 1)
                            pipelineRegister.pc = labelMap[operands[1].value];
                        else
                            pipelineRegister.pc++;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "GOTO":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = new ImmediateUnit(instructionOperands[0]);
                    pipelineRegister.ExecutionDelegate = delegate () 
                    {
                        pipelineRegister.pc = operands[0].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "REF":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = registers[instructionOperands[0]];
                    operands[1] = new ImmediateUnit(instructionOperands[1]);
                    pipelineRegister.MemDelegate = delegate ()
                    {
                        pipelineRegister.SetResult(operands[0].Clone());
                        pipelineRegister.result.value = operands[1].value;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                //case "DREF":
                //    pipelineRegister.opcode = instruction.Opcode;
                //    break;

                case "LABEL":
                    pipelineRegister.opcode = instruction.Opcode;
                    operands[0] = new ImmediateUnit(instructionOperands[0]);
                    pipelineRegister.ExecutionDelegate = delegate () 
                    {
                        if (!labelMap.ContainsKey(operands[0].value))
                            labelMap.Add(operands[0].value, pipelineRegister.pc);
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "HLT":
                    pipelineRegister.opcode = instruction.Opcode;
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        ;
                    };
                    pipelineRegister.cycles = 1;
                    break;

                case "NOP":
                    pipelineRegister.opcode = instruction.Opcode;
                    pipelineRegister.ExecutionDelegate = delegate ()
                    {
                        ;
                    };
                    pipelineRegister.cycles = 1;
                    break;

            }

            return pipelineRegister;
        }
    }
}
