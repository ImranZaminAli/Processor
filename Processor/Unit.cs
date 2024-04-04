using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    abstract class Unit : ICloneable
    {
        public int value;
        public bool inUse;

        public Unit(int value)
        {
            this.value = value;
            inUse = false;
        }

        public Unit()
        {
            value = 0;
            inUse = false;
        }

        public void Free() => inUse = false ;

        public virtual void Toggle() => inUse = !inUse;

        //public virtual override Unit Clone() => new Unit();
        public abstract object Clone();

        public override string ToString()
        {
            return value.ToString() + " " + inUse.ToString();
        }
    }

    class RegisterUnit : Unit {
        public override object Clone()
        {
            RegisterUnit registerUnit = new RegisterUnit();
            registerUnit.value = value;
            registerUnit.inUse = inUse;
            return registerUnit;
        }
    }

    class MemoryUnit : Unit { 
    
        public override object Clone()
        {
            MemoryUnit memoryUnit = new MemoryUnit();
            memoryUnit.value = value;
            memoryUnit.inUse = inUse;
            return memoryUnit;
        }
    }

    class ImmediateUnit : Unit {

        public ImmediateUnit() : base() { }
        public ImmediateUnit(int value) : base(value) { }
        public override void Toggle() => inUse = false;

        public override object Clone()
        {
            ImmediateUnit immediateUnit = new ImmediateUnit();
            immediateUnit.value = value;
            immediateUnit.inUse = inUse;
            return immediateUnit;
        }
    }
}
