using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Lsq
    {
        public List<LsqEntry> queue;
        public Unit[] memory;
        public Unit[] registers;
        public Lsq(Unit[] memory, Unit[] registers)
        {
            queue = new List<LsqEntry>();
            this.memory = memory;
            this.registers = registers;
        }

        public void Add(bool load, int memAddress, int regAddress)
        {
            var entry = new LsqEntry(load, memory[memAddress], registers[regAddress]);
            queue.Add(entry);
        }

    }

    class LsqEntry : ReservationStationEntry
    {
        public bool load;
        public Store memAddress;
        public Store regAddress;
        public int value;
        public bool done;

        public LsqEntry(bool load, Unit memAddress, Unit regAddress)
        {
            this.load = load;
            this.memAddress = memAddress;
            this.regAddress = regAddress;
            value = -1;
            done = false;
        }

    }
}
