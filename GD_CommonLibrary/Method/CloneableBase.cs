using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine
{
    public abstract class CloneableBase
    {
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
