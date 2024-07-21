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
        public void Run(ReservationStation reservationStations, ExecuteUnit[] alus, ExecuteUnit[] loadStore, ExecuteUnit[] branchUnit, int width)
        {
            List<ReservationStationEntry> entries = reservationStations.Dispatch();
            if (entries == null)
                return;

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

