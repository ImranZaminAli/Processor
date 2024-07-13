using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor
{
    abstract class Store 
    { }
    abstract class Unit : Store, ICloneable
    {
        public int value;
        public bool locked;
        public PipelineRegister lockedBy;

        public Unit(int value)
        {
            this.value = value;
            locked = false;
            lockedBy = null;
        }

        public Unit()
        {
            value = 0;
            locked = false;
            lockedBy = null;
        }

        private void Free()
        {
            locked = false;
            lockedBy = null;
        }

        public void TryFree(PipelineRegister pipelineRegister)
        {
            if (locked && lockedBy == pipelineRegister)
                Free();
        }

        private void Lock(PipelineRegister pipelineRegister)
        {
            locked = true;
            lockedBy = pipelineRegister;
        }

        public void TryLock(PipelineRegister pipelineRegister)
        {
            if (!locked)
                Lock(pipelineRegister);
            else if(lockedBy != pipelineRegister)
                Console.WriteLine("\n\n\nTried locking value when already locked");
        }

        public virtual void Toggle() => locked = !locked;

        //public virtual override Unit Clone() => new Unit();
        public abstract object Clone();

        public override string ToString()
        {
            return value.ToString() + " " + locked.ToString();
        }
    }

    class RegisterUnit : Unit {
        public override object Clone()
        {
            RegisterUnit registerUnit = new RegisterUnit();
            registerUnit.value = value;
            registerUnit.locked = locked;
            return registerUnit;
        }
    }

    class MemoryUnit : Unit { 
    
        public override object Clone()
        {
            MemoryUnit memoryUnit = new MemoryUnit();
            memoryUnit.value = value;
            memoryUnit.locked = locked;
            return memoryUnit;
        }
    }

    class ImmediateUnit : Unit {

        public ImmediateUnit() : base() { }
        public ImmediateUnit(int value) : base(value) { }
        public override void Toggle() => locked = false;

        public override object Clone()
        {
            ImmediateUnit immediateUnit = new ImmediateUnit();
            immediateUnit.value = value;
            immediateUnit.locked = locked;
            return immediateUnit;
        }
    }

    class NullUnit : Unit
    {
        public override object Clone()
        {
            return null;
        }
    }
}
