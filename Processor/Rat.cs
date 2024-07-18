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

        public Rat(Unit[] registers, Unit[] memory)
        {
            this.registers = registers;
            this.memory = memory;
            ratLength = registers.Length;
            rat = new RobEntry[ratLength];
            for(int i = 0; i < ratLength; i++)
            {
                rat[i] = null;
            }
        }

        public void Update(int index, RobEntry robEntry)
        {
            rat[index] = robEntry;
        }

        public void Commit(RobEntry entry)
        {

            int index = entry.destination;
            registers[index].value = entry.value;
            if (rat[index] == entry)
                rat[index] = null;
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
