using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Processor
    {
        const int robLength = 6;
        const int reservationStationLength = 4;
        const int btbLength = 2;
        const bool twoBitBtb = true;
        const int numAlu = 2;
        const int numLoadStoreUnit = 1;
        const int numBranchUnit = 1;
        const int width = 2;

        private int pc;
        private int cycles;
        private bool finished;
        private Unit[] registers;
        private Unit[] memory;
        private Rat rat;
        private Dictionary<int, int> labelMap; // label to line
        private PipelineRegister[] pipelineRegisters;
        private ReservationStation reservationStation;
        private Rob rob;
        private FetchUnit fetchUnit;
        private IssueUnit issueUnit;
        private DispatchUnit dispatchUnit;
        private ExecuteUnit executeUnit;
        private ExecuteUnit[] alus;
        private ExecuteUnit[] loadStoreUnit;

        private ExecuteUnit[] branchUnit;
        private MemUnit memUnit;
        private WriteUnit writeUnit;
        private Btb btb;
        private Lsq lsq;

        private Queue<ReservationStationEntry> executeFinishedQueue = new Queue<ReservationStationEntry>();
        private Queue<Instruction> instructionQueue = new Queue<Instruction>();
        
        // old

        //private FetchUnit fetchUnit;
        private DecodeUnit decodeUnit = new DecodeUnit();
        //private ExecuteUnit executeUnit;
        //private MemUnit memUnit;
        //private WriteUnit writeUnit;

        public Processor(Instruction[] instructions, bool old)
        {
            pc = 0;
            finished = false;
            registers = new Unit[32];
            memory = new Unit[32];
            //rat = new RobEntry[32];
            for (int i = 0; i < 32; i++)
            {
                registers[i] = new RegisterUnit();
                memory[i] = new MemoryUnit();
                //rat[i] = null;
            }
            labelMap = new Dictionary<int, int>();
            pipelineRegisters = new PipelineRegister[5];
            for (int i = 0; i < pipelineRegisters.Length; i++)
                pipelineRegisters[i] = new PipelineRegister();
            fetchUnit = new FetchUnit(instructions);
            decodeUnit = new DecodeUnit();
            executeUnit = new ExecuteUnit();
            memUnit = new MemUnit();
            writeUnit = new WriteUnit();
            btb = new Btb(btbLength, twoBitBtb);
        }

        public Processor(Instruction[] instructions)
        {
            pc = 0;
            finished = false;
            registers = new Unit[32];
            memory = new Unit[32];
            for (int i = 0; i < 32; i++)
            {
                registers[i] = new RegisterUnit();
                memory[i] = new MemoryUnit();
            }
            rat = new Rat(registers, memory);
            labelMap = new Dictionary<int, int>();
            rob = new Rob(robLength);
            reservationStation = new ReservationStation(reservationStationLength);
            fetchUnit = new FetchUnit(instructions);
            issueUnit = new IssueUnit();
            dispatchUnit = new DispatchUnit();
            executeUnit = new ExecuteUnit();
            memUnit = new MemUnit();
            writeUnit = new WriteUnit();
            lsq = new Lsq(memory, registers);
            btb = new Btb(btbLength, twoBitBtb);
            alus = new ExecuteUnit[numAlu];
            for (int i = 0; i < numAlu; i++)
                alus[i] = new ExecuteUnit(Optype.Alu);
            loadStoreUnit = new ExecuteUnit[numLoadStoreUnit];
            for (int i = 0; i < numLoadStoreUnit; i++)
                loadStoreUnit[i] = new LsExecuteUnit(Optype.LoadStore, lsq);
            branchUnit = new ExecuteUnit[numBranchUnit];
            for (int i = 0; i < numBranchUnit; i++)
                branchUnit[i] = new ExecuteUnit(Optype.Branch);
            // fetch, decode/issue, dispatch, execute + broadcast, write, commit


        }

        public void Execute(ExecuteUnit[] units)
        {
            foreach(var unit in units)
            {
                ReservationStationEntry output = null;
                if (unit.input != null)
                    output = unit.Run();
                if (output != null)
                    executeFinishedQueue.Enqueue(output);
            }
        }

        public void Dispatch(ExecuteUnit[] units, ref int dispatchCounter)
        {
            foreach(var unit in units)
            {
                if (dispatchCounter == width)
                    return;

                if (unit.busy)
                    continue;

                var input = dispatchUnit.Run(reservationStation, unit.optype);
                unit.input = input;
                if (input != null)
                    dispatchCounter++;
            }
        }

        public void Run()
        {
            bool flushed = false;
            ExecuteUnit[][] executeUnits = new ExecuteUnit[][] { alus, loadStoreUnit, branchUnit };
            int cycles = 0;
            while (!finished)
            {
                // commit
                for (int i = 0; i < width; i++)
                    rob.Commit(ref pc, rat, ref flushed, ref finished, btb);

                if (flushed)
                {
                    instructionQueue.Clear();
                    executeFinishedQueue.Clear();
                    foreach(var units in executeUnits)
                    {
                        foreach(var unit in units)
                        {
                            unit.input = null;
                            unit.busy = false;
                        }
                    }
                    reservationStation = new ReservationStation(reservationStationLength);
                    flushed = false;
                }

                // broadcast
                int broadcastCounter = 0;
                while (broadcastCounter < width)
                {
                    if (executeFinishedQueue.Count > 0)
                    {
                        ReservationStationEntry entry = executeFinishedQueue.Dequeue();
                        reservationStation.Broadcast(entry, lsq);
                        entry.destination.value = entry.result;
                        entry.destination.done = true;
                    }
                    broadcastCounter++;
                }

                // execute
                foreach (var units in executeUnits)
                    Execute(units);

                // dispatch
                int dispatchCounter = 0;
                foreach (var units in executeUnits)
                    Dispatch(units, ref dispatchCounter);

                // issue
                for(int i = 0; i < width; i++)
                {
                    if (!reservationStation.CheckFull() && !rob.CheckFull() && instructionQueue.Count > 0)
                        issueUnit.Run(instructionQueue.Dequeue(), rat, reservationStation, rob, ref pc, btb, lsq, instructionQueue);
                }

                // fetch
                for (int i = 0; i < width; i++)
                {
                    var instruction = fetchUnit.Run(ref pc, btb);
                    if(instruction != null)
                        instructionQueue.Enqueue(instruction);
                }

                cycles++;
            }
            Console.WriteLine("cycles: {0}", cycles);
            Console.WriteLine("{0} {1} {2}, {3} {4}", registers[0].value, registers[1].value, registers[2].value, registers[3].value, registers[4].value);

        }

        //public void Run()
        //{
        //    Instruction issueInput = null;
        //    ReservationStationEntry dispatchOutput = null;
        //    //ReservationStationEntry dispatchInput = null;
        //    ReservationStationEntry executeInput = null;
        //    ReservationStationEntry executeOutput = null;
        //    ReservationStationEntry broadcastInput = null;
        //    bool flushed = false;
        //    while (!finished)
        //    {
        //        /* 

        //            fetch:
        //            see if there is a free reservation station and if there is a free rob entry
        //            if there is get the next instruction, else do nothing

        //         */
        //        Instruction fetchOutput = null;
        //        for(int i = 0; i < width; i++)
        //        {                
        //            if (!reservationStation.CheckFull() && !rob.CheckFull())
        //            {
        //                fetchOutput = fetchUnit.Run(ref pc, btb);
        //                issueUnit.Run(fetchOutput, rat, reservationStation, rob, pc, btb);
        //            }
        //        }
        //        //else
        //        //    issueUnit.Run(reservationStation, rob);

        //        /* 

        //            dispatch:
        //            check if there is a reservation station that has an instruction ready. 
        //            if so dispatch the instruction and free the reservation station.
        //            else do nothing

        //        */



        //        /*

        //            execute:
        //            execute the instruction over however many cycles it takes

        //         */

        //        //if (executeUnit.busy == false)
        //        //{
        //        //    executeInput = dispatchUnit.Run(reservationStation); 
        //        //    if(executeInput != null)
        //        //        executeOutput = executeUnit.Run(executeInput);
        //        //    if (executeOutput != null)
        //        //        executeInput = null;
        //        //}


        //        // if (executeUnit.busy == false)
        //        //     executeInput = dispatchUnit.Run(reservationStation);
        //        // if (executeInput != null)
        //        //     executeOutput = executeUnit.Run(executeInput);
        //        // if (executeOutput != null)
        //        //     executeInput = null;


        //        foreach (ExecuteUnit unit in alus)
        //        {
        //            ReservationStationEntry input = null;
        //            ReservationStationEntry output = null;
        //            if(!unit.busy)
        //            {
        //                input = dispatchUnit.Run(reservationStation, unit.optype);
        //                unit.input = input;
        //            }
        //            if(unit.input != null)
        //                output = unit.Run();
        //            if(output != null)
        //                executeFinishedQueue.Enqueue(output);
        //        }

        //        foreach (ExecuteUnit unit in loadStoreUnit)
        //        {
        //            ReservationStationEntry input = null;
        //            ReservationStationEntry output = null;
        //            if (!unit.busy)
        //            {
        //                input = dispatchUnit.Run(reservationStation, unit.optype);
        //                unit.input = input;
        //            }
        //            if (unit.input != null)
        //                output = unit.Run();
        //            if (output != null)
        //                executeFinishedQueue.Enqueue(output);
        //        }

        //        foreach (ExecuteUnit unit in branchUnit)
        //        {
        //            ReservationStationEntry input = null;
        //            ReservationStationEntry output = null;
        //            if (!unit.busy)
        //            {
        //                input = dispatchUnit.Run(reservationStation, unit.optype);
        //                unit.input = input;
        //            }
        //            if (unit.input != null)
        //                output = unit.Run();
        //            if (output != null)
        //                executeFinishedQueue.Enqueue(output);
        //        }



        //        /*

        //            write back:
        //            broadcast execution result to the reservation stations and rob

        //         */

        //        // if (broadcastInput != null)
        //        // {
        //        //     reservationStation.Broadcast(broadcastInput);
        //        //     broadcastInput.destination.value = broadcastInput.result;
        //        //     broadcastInput.destination.done = true;
        //        //     broadcastInput = null;
        //        //     //rob.Broadcast(broadcastInput);
        //        // }

        //        int counter = 0;

        //        while(counter < width)
        //        {
        //            if (executeFinishedQueue.Count > 0)
        //            {
        //                ReservationStationEntry entry = executeFinishedQueue.Dequeue();
        //                reservationStation.Broadcast(entry);
        //                entry.destination.value = entry.result;
        //                entry.destination.done = true;
        //            }
        //            counter++;
        //        }

        //        /*

        //            commit:
        //            if the commit pointer is at a done instruction update the register file
        //            if required also update the rat
        //            flush mispredictions

        //        */
        //        for (int i = 0; i < width; i++)
        //            rob.Commit(ref pc, rat, ref flushed, ref finished, btb);

        //        if(flushed){
        //            issueInput = null;
        //            executeInput = null;
        //            broadcastInput = null;
        //            dispatchOutput = null;
        //            executeOutput = null;
        //            executeUnit.busy = false;
        //            flushed = false;
        //            executeFinishedQueue.Clear();
        //        }

        //        if(executeOutput != null)
        //        {
        //            broadcastInput = executeOutput;
        //            executeOutput = null;
        //        }


        //    }
        //    Console.WriteLine("{0} {1} {2}, {3}", registers[0].value, registers[1].value, registers[2].value, registers[3].value);
        //}

        private void CheckFlushed() 
        {
            if (pipelineRegisters[2].Flushed)
            {
                pipelineRegisters[0] = new PipelineRegister();
                pipelineRegisters[1] = new PipelineRegister();
            }
        }

        private void AdvancePipeline()
        {
            for(int i = pipelineRegisters.Length - 1; i > 0; i--)
            {
                if (pipelineRegisters[i-1].Stalled || pipelineRegisters[i-1].Busy)
                    return;
                pipelineRegisters[i] = pipelineRegisters[i - 1];
                pipelineRegisters[i - 1] = new PipelineRegister();
            }
            pipelineRegisters[0] = new PipelineRegister();
        }


        // old

        public void Run(bool old)
        {
            while (!finished)
            {

                if (pipelineRegisters[0].Empty)
                {
                    pc = fetchUnit.Run(pc, pipelineRegisters[0]);
                }

                if (!pipelineRegisters[1].Empty)
                {
                    pipelineRegisters[1] = decodeUnit.Run(pipelineRegisters[1], registers, memory, labelMap);
                }

                if (!pipelineRegisters[2].Empty && !pipelineRegisters[2].Stalled)
                {
                    pipelineRegisters[2] = executeUnit.Run(pipelineRegisters[2], ref pc);
                }

                if (!pipelineRegisters[3].Empty && !pipelineRegisters[3].Stalled)
                {
                    pipelineRegisters[3] = memUnit.Run(pipelineRegisters[3]);
                }

                if (!pipelineRegisters[4].Empty && !pipelineRegisters[4].Stalled)
                {
                    pipelineRegisters[4] = writeUnit.Run(pipelineRegisters[4], ref finished);
                }

                CheckFlushed();
                AdvancePipeline();

                // not pipelined
                //pipelineRegisters[0] = new PipelineRegister();
                //pc = fetchUnit.Run(pc, pipelineRegisters[0]);
                //pipelineRegisters[0] = decodeUnit.Run(pipelineRegisters[0], registers, memory, labelMap);
                //pipelineRegisters[0] = executeUnit.Run(pipelineRegisters[0], ref finished, ref pc);
                //pipelineRegisters[0] = memUnit.Run(pipelineRegisters[0]);
                //pipelineRegisters[0] = writeUnit.Run(pipelineRegisters[0], ref cycles);
                cycles++;
            }
            Console.WriteLine("Cycles: {0}", cycles);
            Console.WriteLine("{0} {1} {2}, {3}", registers[0].value, registers[1].value, registers[2].value, registers[3].value);
        }
    }
}
