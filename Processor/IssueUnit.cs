using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class IssueUnit
    {
        public void Run(Instruction instruction, Rat rat, ReservationStation reservationStation, Rob rob, ref int pc, Btb btb, Lsq lsq, Queue<Instruction> instructionQueue)
        {
            if (instruction == null)
                return;
            ReservationStationEntry entry = reservationStation.GetFreeStation();
            entry.isFree = false;
            entry.opcode = instruction.Opcode;
            entry.instruction = instruction;
            entry.pc = instruction.pc;
            switch(entry.opcode)
            {
                case "ADD":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate ( int[] inputs) { return inputs[0] + inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry) entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "SUB":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] - inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 3;
                    break;
                case "MUL":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] * inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 5;
                    break;
                case "ADDI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] + inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "SUBI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] - inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 3;
                    break;
                case "MULI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] * inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 5;
                    break;
                case "AND":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] & inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "OR":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] | inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "NOT":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1 ? 0 : 1; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "EQ":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] == inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                // case "NEQ":
                //     entry.destination = rob.Issue(instruction.Operand[0]);
                //     rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                //     rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                //     entry.execution = delegate (int[] inputs) { return inputs[0] != inputs[1] ? 1 : 0; };
                //     rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                //     break;
                case "ANDI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] & inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "ORI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] | inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "NOTI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1? 0 : 1; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "EQI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] == inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                // case "NEQI":
                //     entry.destination = rob.Issue(instruction.Operand[0]);
                //     rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                //     entry.tags[1] = null;
                //     entry.values[1] = instruction.Operand[2];
                //     entry.execution = delegate (int[] inputs) { return inputs[0] != inputs[1] ? 1 : 0; };
                //     rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                //     break;
                case "LT":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] < inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 2;
                    break;
                case "GT":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] > inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 2;
                    break;
                case "LDI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    entry.tags[0] = null;
                    entry.values[0] = instruction.Operand[1];
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 2;
                    break;
                case "MOV":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "JMP":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = instruction.Operand[0];
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    if (!btb.Contains(instruction.pc))
                    {
                        var btbEntry = btb.Add(instruction.pc, instruction.Operand[0]);
                        btbEntry.Setup((instruction.pc > instruction.Operand[0]) ? instruction.Operand[0] : instruction.pc);
                        int predicted = btb.Predict(instruction.pc);
                        pc = predicted != -1 ? btbEntry.branchedPc - 1 : btbEntry.instructionPc + 1;
                        if (btbEntry.predicted != -1)
                            instructionQueue.Clear();
                    }
                    entry.optype = Optype.Branch;
                    entry.cycles = 4;
                    break;
                case "BR":
                    entry.destination = rob.Issue(-1);
                    rat.CheckTags(instruction.Operand[0], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[1];
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1 ? inputs[1] : -1;};
                    if (!btb.Contains(instruction.pc))
                    {
                        var btbEntry = btb.Add(instruction.pc, instruction.Operand[1]);
                        btbEntry.Setup((instruction.pc > instruction.Operand[0]) ? instruction.Operand[0] : instruction.pc);
                        int predicted = btb.Predict(instruction.pc);
                        pc = predicted != -1? btbEntry.branchedPc - 1 : btbEntry.instructionPc + 1;
                        if (btbEntry.predicted != -1)
                            instructionQueue.Clear();
                    }
                    entry.optype = Optype.Branch;
                    entry.cycles = 5;
                    break;
                case "LD":
                    // load memory into register
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    lsq.Add(true, instruction.Operand[1], instruction.Operand[0]);
                    entry.tags[0] = null;
                    entry.tags[1] = null;
                    entry.values[0] = -1;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    entry.optype = Optype.LoadStore;
                    entry.cycles = 2;
                    break;
                case "ST":
                    entry.destination = rob.Issue(-1);
                    lsq.Add(false, instruction.Operand[0], instruction.Operand[1]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    entry.optype = Optype.LoadStore;
                    entry.cycles = 2;
                    break;
                case "HLT":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = -1;
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return -1; };
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;
                case "NOP":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = -1;
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return -1; };
                    entry.optype = Optype.Alu;
                    entry.cycles = 1;
                    break;

            }

            entry.destination.opcode = entry.opcode;
            entry.destination.pc = instruction.pc;
            //this.Run(reservationStation, rob);
        }

        
    }
}
