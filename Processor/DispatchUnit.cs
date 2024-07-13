using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class DispatchUnit
    {
        public ReservationStationEntry Run(ReservationStation reservationStations) 
        { 
            ReservationStationEntry entry = reservationStations.Dispatch();
            if (entry != null)
            {
                ReservationStationEntry clone = (ReservationStationEntry)entry.Clone();
                entry.Free();
                return clone;
            }
            return null;
        }
    }
}
