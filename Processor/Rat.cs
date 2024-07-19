using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Rat
    {
        public RobEntry[] rat;
        public int ratLength;
        public Unit[] registers;
        public Unit[] memory;
        public int[] instructionCounts;

        public Rat(Unit[] registers, Unit[] memory)
        {
            this.registers = registers;
            this.memory = memory;
            ratLength = registers.Length;
            rat = new RobEntry[ratLength];
            instructionCounts = new int[ratLength];
            for(int i = 0; i < ratLength; i++)
            {
                rat[i] = null;
                instructionCounts[i] = -1;
            }
        }

        public void Update(int index, int instructionCount, RobEntry robEntry)
        {
            if (instructionCount > instructionCounts[index])
            {
                rat[index] = robEntry;
                instructionCounts[index] = instructionCount;
            }
        }

        public void Commit(RobEntry entry)
        {

            int index = entry.destination;
            registers[index].value = entry.value;
            if (rat[index] == entry)
            {
                rat[index] = null;
                instructionCounts[index] = -1;
            }
            //for(int i = 0; i < ratLength; i++)
            //{
            //    if(rat[i] == entry)
            //    {
            //        rat[i] = null;
            //        registers[i].value = (int) entry.value;
            //    }
            //}
        }

        public void CommitMem(RobEntry entry)
        {
            int index = entry.destination;
            memory[index].value = entry.value;
            if (rat[index] == entry)
                rat[index] = null;
        }

        //public void Flush() => rat.ToList().ForEach(x => x.Free());

        public void Flush()
        {
            for(int i = 0; i < ratLength; i++)
            {
                if (rat[i] != null)
                    rat[i] = null;
            }
        }

        public void CheckTags(int operand, ref RobEntry store, ref int value)
        {
            if(rat[operand] == null)
            {
                value = registers[operand].value;
                store = null;
            }
            else if (rat[operand].done)
            {
                value = rat[operand].value;
                store = null;
            }
            else
            {
                value = -1;
                store = rat[operand];
            }
        }
    }
}
