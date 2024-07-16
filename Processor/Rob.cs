using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Rob
    {
        int tableLength;
        private RobEntry[] table;
        int commitPointer;
        int issuePointer;
        string[] writeOperations = new string[]{
                "ADD", "SUB", "MUL",
                "ADDI", "SUBI", "MULI",
                "AND", "OR", "NOT",
                "ANDI", "ORI", "NOTI",
                "EQ", "EQI",
                "LT", "GT",
                "LDI", "MOV"
                };
        string[] branchOperations = new string[] { "JMP", "BR" };
        public Rob(int length)
        {
            tableLength = length;
            table = new RobEntry[length];
            for(int i = 0; i < length; i++)
            {
                table[i] = new RobEntry();
            }
            commitPointer = 0;
            issuePointer = 0;
        }

        public bool CheckFull()
        {
            bool full = issuePointer == commitPointer && table[issuePointer].opcode != null;
            return full;
        }

        public RobEntry Issue(int destination)
        {
            RobEntry entry = table[issuePointer];
            entry.destination = destination;
            entry.done = false;
            issuePointer = Inc(issuePointer);

            return entry;
        }

        public void Flush(Rat rat, ref bool flushed)
        {
            //while (commitPointer != issuePointer)
            //{
            //    table[commitPointer].Free();
            //    commitPointer = Inc(commitPointer);
            //}
            foreach(var entry in table)
            {
                entry.Free();
            }
            issuePointer = 0;
            commitPointer = 0;
            rat.Flush();
            flushed = true;
        }

        public void Commit(ref int pc, Rat rat, ref bool flushed, ref bool finished, Btb btb)
        {
            
            // not every instruction writes
            var entry = table[commitPointer];

            if (!entry.done)
                return;

            if (writeOperations.Contains(entry.opcode))
                rat.Commit(entry);
            else if (branchOperations.Contains(entry.opcode))
            {
                var btbEntry = btb.Find(entry.pc);
                bool setup = btbEntry.confidence != null;
                if (!setup && entry.value != -1)
                {
                    btbEntry.Setup(entry.value);
                    btb.Commit();
                    pc = entry.value - 1;
                    // flush
                    Flush(rat, ref flushed);
                    return;
                }
                else if(!setup)
                {
                    btbEntry.Setup(entry.value);
                    btb.Commit();
                }
                else
                {
                    int predicted = (int) btbEntry.predicted;
                    if(predicted == entry.value)
                    {
                        btbEntry.IncConfidence();
                    }
                    else
                    {
                        btbEntry.DecConfidence();
                        Flush(rat, ref flushed);
                        pc = predicted == -1 ? btbEntry.branchedPc - 1 : btbEntry.instructionPc + 1;
                        return;
                    }
                }
            }
            else if (entry.opcode == "HLT")
            {
                finished = true;
            }
            //if(entry.done && entry.destination != -1)
            //{
            //    if(writeOperations.Contains(entry.opcode))
            //    {
            //        rat.Commit(entry);
            //    }
            //    else if(branchOperations.Contains(entry.opcode))
            //    {
            //        if(entry.value != -1)
            //        {
            //            // flush
            //            while(commitPointer != issuePointer)
            //            {
            //                table[commitPointer].Free();
            //                commitPointer = Inc(commitPointer);
            //            }
            //            rat.Flush();
            //            flushed = true;
            //            pc = entry.value;
            //        }
            //    }
            //    else if(entry.opcode == "HLT")
            //    {
            //        finished = true;
            //    }
            entry.Free();
            commitPointer = Inc(commitPointer);

        }

        

        private int Inc(int pointer)
        {
            pointer = (pointer + 1) % tableLength;

            return pointer;
        }

    }

    class RobEntry : Store
    {
        public int destination;
        public bool done;
        public int value;

        public string opcode;
        public int pc;

        public RobEntry()
        {
            done = false;
            destination = -1;
            value = -1;
            opcode = null;
            pc = -1;
        }

        public void Free()
        {
            destination = -1;
            value = -1;
            done = false;
            opcode = null;
            pc = -1;
        }

        //public void Commit()
        //{
        //    destination.value = (int)this.value;
        //    Free();
        //}

        public void Write(int value)
        {
            this.value = value;
            done = true;
        }

        //public override object Clone()
        //{
        //    RobEntry robEntry = new RobEntry();
        //    robEntry.destination = this.destination;
        //    robEntry.value = this.value;
        //    robEntry.done = this.done;
        //    return robEntry;
        //}
    }
}
