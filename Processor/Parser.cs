using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Processor
{
    static class Parser
    {
        public static Instruction[] Parse(string filename)
        {
            // check if file exists
            //Console.WriteLine(Directory.GetFiles(Directory.GetCurrentDirectory()));
            //foreach(string file in Directory.GetFiles())
            //{
            //    Console.WriteLine(file);
            //}
            //Console.Read();
            if (!File.Exists(@filename))
            {
                throw new FileNotFoundException(@filename);
            }
            Instruction[] instructions;
            using (StreamReader reader = new StreamReader(@filename)) {
                string[] lines = reader.ReadToEnd().Split('\n', '\r');
                lines = lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                instructions = new Instruction[lines.Length];
                //string line = reader.ReadLine();
                for(int i  = 0; i < lines.Length; i++)
                {
                    Instruction instruction = new Instruction(lines[i]);
                    instructions[i] = instruction;
                }

            }

            return instructions;
        }
    }
}
