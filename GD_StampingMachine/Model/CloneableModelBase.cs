using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Model
{
    public class CloneableModelBase : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
