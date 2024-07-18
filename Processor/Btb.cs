using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    class Btb
    {
        public List<BtbEntry> buffer;
        public int length;
        bool twoBit;
        int maxConfidence;
        public Btb(int length, bool twoBit)
        {
            this.length = length;
            buffer = new List<BtbEntry>();
            this.twoBit = twoBit;
            maxConfidence = twoBit ? 4 : 1;
        }

        public bool Contains(int pc) => buffer.Any(x => x.instructionPc == pc);

        public BtbEntry Find(int pc) => buffer.Find(x => x.instructionPc == pc);

        public BtbEntry Add(int pc, int branchedPc)
        {
            if(!Contains(pc))
            {
                //if (buffer.Count == length)
                //    buffer.RemoveAt(0);
                var newEntry = new BtbEntry(pc, branchedPc, twoBit ? 4 : 2);
                buffer.Add(newEntry);
                return newEntry;
            }
            return null;
        }

        public int Predict(int pc)
        {
            var entry = Find(pc);
            return entry.Predict();
        }

        public void Commit()
        {
            if(buffer.Count > length)
            {
                buffer.RemoveAt(0);
            }
            if(buffer.Count > length)
                Console.WriteLine("Error occured btb is still too long");
        }

        public bool CheckPrediction(int instructionPc, int branchedPc)
        {
            var entry = Find(instructionPc);
            return entry.branchedPc == branchedPc;
        }
    }

    class BtbEntry
    {
        public int maxConfidence;
        public int? confidence;
        public int? predicted;
        public int branchedPc;
        public int instructionPc;
        public bool setup;

        public BtbEntry(int pc, int branchedPc, int maxConfidence)
        {
            this.instructionPc = pc;
            this.branchedPc = branchedPc;
            this.maxConfidence = maxConfidence;
            setup = false;
        }

        private double GetMidpoint()
        {
            return (1 + (double)maxConfidence) / 2; ;
        }

        public int Predict()
        {
            double midPoint = GetMidpoint();
            
            predicted = confidence < midPoint ? -1 : branchedPc;
            return (int) predicted;
        }

        public void Setup(int predicted)
        {
            this.predicted = predicted;
            double conf = GetMidpoint();
            conf = predicted == -1 ? conf - 0.5 : conf + 0.5;
            this.confidence = ((int)conf);
        }

        public void IncConfidence()
        {
            confidence++;
            if (confidence > maxConfidence)
                confidence = maxConfidence;
        }

        public void DecConfidence()
        {
            confidence--;
            if (confidence < 1)
                confidence = 1;
        }
    }
}
