using System;

namespace Processor
{
    public delegate int Execution(params int[] operands);
    public delegate int Mem(params int[] operands);
}