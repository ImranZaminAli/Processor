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

        public ReservationStationEntry Dispatch(Optype optype)
        {
            List<ReservationStationEntry> ready = new List<ReservationStationEntry>();
            foreach (var entry in table)
            {
                if (entry.CheckReady() && entry.optype == optype)
                {
                    ready.Add(entry);
                }
            }
            if(ready.Count == 0)
                return null;

            //Random random = new Random();
            //int index = random.Next(0, ready.Count());
            //return ready[index];
            return ready.OrderBy(x => x.instructionCount).ToArray()[0];
        }

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

        public void Broadcast(ReservationStationEntry entry, Lsq lsq, Rat rat)
        {
            foreach (var station in table)
            {
                for (int i = 0; i < station.tags.Length; i++)
                {
                    if (station.tags[i] == entry.destination)
                    {
                        if (!(station.opcode == "MOVINDB" && station.destination.destination == -1))
                        {
                            station.tags[i] = null;
                            station.values[i] = entry.result;
                            if (station.opcode == "MOVIND" && station.tags[1] != null)
                            {
                                station.tags[1] = null;
                                rat.CheckTags(station.values[0], ref station.tags[0], ref station.values[0]);
                            }
                        }
                        else
                        {
                            station.destination.destination = entry.result;
                            station.tags[1] = null;
                            station.values[1] = -1;
                            rat.Update(entry.result, entry.instructionCount, station.destination);
                        }
                    }
                    
                }
            }

            

            if (entry.optype != Optype.LoadStore)
                return;

            foreach(var lsEntry in lsq.queue)
            {
                if (entry.destination == lsEntry.memAddress)
                {
                    lsEntry.value = entry.result;
                    lsEntry.done = true;
                }
                if (entry.destination == lsEntry.regAddress)
                {
                    lsEntry.value = entry.result;
                    lsEntry.done = true;
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
        //public Mem mem;
        public Instruction instruction;
        public int pc;
        public Optype optype;

        public int result;
        public int instructionCount;
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
            //mem = delegate (int[] operands) { return -1; };
            cycles = 0;
            result = -1;
            instruction = null;
            pc = -1;
            optype = Optype.Null;
            instructionCount = -1;
        }

        public bool CheckReady()
        {
            if (opcode == "MOVINDB" && destination.destination == -1)
                return false;
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
            //mem = delegate (int[] operands) { return -1; };
            cycles = 0;
            result = -1;
            instruction = null;
            pc = -1;
            optype = Optype.Null;
            instructionCount = -1;
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
            //clone.mem = mem;
            clone.cycles = cycles;
            clone.result = result;
            clone.instruction = instruction;
            clone.pc = pc;
            clone.optype = optype;
            clone.instructionCount = instructionCount;
            return clone;
        }
    }
}
