using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class DispatchUnit
    {

        public ReservationStationEntry Run(ReservationStation reservationStations, Optype optype){
            ReservationStationEntry entry = reservationStations.Dispatch(optype);
            if (entry != null)
            {
                ReservationStationEntry clone = (ReservationStationEntry)entry.Clone();
                entry.Free();
                return clone;
            }
            return null;
        }
        public void Run(ReservationStation[] reservationStations, ExecuteUnit[] alus, ExecuteUnit[] loadStore, ExecuteUnit[] branchUnit, int width)
        {
            //List<ReservationStationEntry> entries = reservationStations.Dispatch();
            //List<List<ReservationStationEntry>> freeEntries = reservationStations.ToList().ForEach(x => x.Dispatch())
            List<List<ReservationStationEntry>> freeEntries = new List<List<ReservationStationEntry>>();
            foreach(var rs in reservationStations)
            {
                freeEntries.Add(rs.Dispatch());
            }
            //if (entries == null)
            //    return;

            if (freeEntries.All(x => x == null))
                return;

            List < ReservationStationEntry > entries = new List<ReservationStationEntry>();

            // get first two reservation station entries that have the lowest instruction count
            for(int i = 0; i < 2; i++)
            {
                int min = int.MaxValue;
                ReservationStationEntry minEntry = null;
                List<ReservationStationEntry> allEntries = new List<ReservationStationEntry>();

                // remove nulls from freeEntries
                freeEntries.RemoveAll(x => x == null);

                foreach (var idk in freeEntries) {
                    idk.ForEach(x => allEntries.Add(x));
                }
                foreach (var entry in allEntries)
                {
                    if (entry == null)
                        continue;

                    if (entry.instructionCount < min)
                    {
                        min = entry.instructionCount;
                        minEntry = entry;
                    }
                }

                if (minEntry != null)
                {
                    entries.Add(minEntry);
                    freeEntries.First(x => x.Contains(minEntry)).Remove(minEntry);
                }
            }

            int dispatchCounter = 0;
            foreach (var entry in entries)
            {
                if (dispatchCounter == width)
                    break;

                ExecuteUnit[] searchThrough;
                switch (entry.optype)
                {
                    case Optype.Alu:
                        searchThrough = alus;
                        break;
                    case Optype.LoadStore:
                        searchThrough = loadStore;
                        break;
                    case Optype.Branch:
                        searchThrough = branchUnit;
                        break;
                    default:
                        searchThrough = new ExecuteUnit[0];
                        break;
                }

                foreach (var unit in searchThrough)
                {
                    if (unit.busy)
                        continue;

                    unit.input = (ReservationStationEntry)entry.Clone();
                    unit.busy = true;
                    entry.Free();
                    dispatchCounter++;
                    break;
                }
            }
        }

    }
}

