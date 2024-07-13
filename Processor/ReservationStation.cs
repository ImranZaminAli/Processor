using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{

    class ReservationStation
    {
        public ReservationStationEntry[] table;
        public int tableLength;

        public ReservationStation(int length)
        {
            tableLength = length;
            table = new ReservationStationEntry[length];

            for(int i = 0; i < length; i++)
            {
                table[i] = new ReservationStationEntry();
            }
        }

        public ReservationStationEntry GetFreeStation() => table.ToList().Find(x => x.isFree);

        public bool CheckFull() => table.All(x => !x.isFree);

        public ReservationStationEntry Dispatch()
        {
            foreach (var entry in table)
            {
                if (entry.CheckReady())
                {
                    return entry;
                }
            }
            return null;
        }

        public void Broadcast(ReservationStationEntry entry)
        {
            foreach (var station in table)
            {
                for (int i = 0; i < station.tags.Length; i++)
                {
                    if (station.tags[i] == entry.destination)
                    {
                        station.tags[i] = null;
                        station.values[i] = entry.result;
                    }
                }
            }
        }


    }
    class ReservationStationEntry : ICloneable
    {
        public string opcode;
        public RobEntry destination;
        public RobEntry[] tags;
        public int[] values;
        public bool isFree;
        public Execution execution;
        public Mem mem;
        public Instruction instruction;

        public int result;

        public int cycles;
        public ReservationStationEntry()
        {
            isFree = true;
            tags = new RobEntry[2];
            values = new int[2];
            for(int i = 0; i < tags.Length; i++)
            {
                tags[i] = null;
                values[i] = -1;
            }
            execution = delegate (int[] operands) { return -1; };
            mem = delegate (int[] operands) { return -1; };
            cycles = 0;
            result = -1;
            instruction = null;
        }

        public bool CheckReady()
        {
            return (!isFree) && tags.All(x => x == null);
        }

        public void Free()
        {
            opcode = null;
            destination = null;
            tags = new RobEntry[2];
            values = new int[2];
            isFree = true;
            execution = delegate (int[] operands) { return -1; };
            mem = delegate (int[] operands) { return -1; };
            cycles = 0;
            result = -1;
            instruction = null;
        }

        public object Clone()
        {
            ReservationStationEntry clone = new ReservationStationEntry();
            clone.opcode = opcode;
            clone.destination = destination;
            clone.tags = tags;
            clone.values = values;
            clone.isFree = isFree;
            clone.execution = execution;
            clone.mem = mem;
            clone.cycles = cycles;
            clone.result = result;
            clone.instruction = instruction;
            return clone;
        }
    }
}
