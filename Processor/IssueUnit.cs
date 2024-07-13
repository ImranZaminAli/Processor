using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class IssueUnit
    {
        public void Run(Instruction instruction, Rat rat, ReservationStation reservationStation, Rob rob)
        {
            if (instruction == null)
                return;
            ReservationStationEntry entry = reservationStation.GetFreeStation();
            entry.isFree = false;
            entry.opcode = instruction.Opcode;
            entry.instruction = instruction;
            switch(entry.opcode)
            {
                case "ADD":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate ( int[] inputs) { return inputs[0] + inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry) entry.destination);
                    entry.cycles = 1;
                    break;
                case "SUB":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] - inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 3;
                    break;
                case "MUL":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] * inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 5;
                    break;
                case "ADDI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] + inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "SUBI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] - inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 3;
                    break;
                case "MULI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] * inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 5;
                    break;
                case "AND":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] & inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "OR":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] | inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "NOT":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1 ? 0 : 1; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "EQ":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] == inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
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
                    entry.cycles = 1;
                    break;
                case "ORI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] | inputs[1]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "NOTI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1? 0 : 1; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "EQI":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[2];
                    entry.execution = delegate (int[] inputs) { return inputs[0] == inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
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
                    entry.cycles = 2;
                    break;
                case "GT":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    rat.CheckTags(instruction.Operand[2], ref entry.tags[1], ref entry.values[1]);
                    entry.execution = delegate (int[] inputs) { return inputs[0] > inputs[1] ? 1 : 0; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
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
                    entry.cycles = 2;
                    break;
                case "MOV":
                    entry.destination = rob.Issue(instruction.Operand[0]);
                    rat.CheckTags(instruction.Operand[1], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    rat.Update(instruction.Operand[0], (RobEntry)entry.destination);
                    entry.cycles = 1;
                    break;
                case "JMP":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = instruction.Operand[0];
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return inputs[0]; };
                    entry.cycles = 4;
                    break;
                case "BR":
                    entry.destination = rob.Issue(-1);
                    rat.CheckTags(instruction.Operand[0], ref entry.tags[0], ref entry.values[0]);
                    entry.tags[1] = null;
                    entry.values[1] = instruction.Operand[1];
                    entry.execution = delegate (int[] inputs) { return inputs[0] == 1 ? inputs[1] : -1;};
                    entry.cycles = 5;
                    break;
                case "HLT":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = -1;
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return -1; };
                    entry.cycles = 1;
                    break;
                case "NOP":
                    entry.destination = rob.Issue(-1);
                    entry.tags[0] = null;
                    entry.values[0] = -1;
                    entry.tags[1] = null;
                    entry.values[1] = -1;
                    entry.execution = delegate (int[] inputs) { return -1; };
                    entry.cycles = 1;
                    break;

            }

            entry.destination.opcode = entry.opcode;

            //this.Run(reservationStation, rob);
        }

        
    }
}
