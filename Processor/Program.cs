using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Instruction[] instructions = Parser.Parse(args[0]);
            
            Processor processor = new Processor(instructions);
            processor.Run();
            Console.ReadLine();
        }
    }
}
