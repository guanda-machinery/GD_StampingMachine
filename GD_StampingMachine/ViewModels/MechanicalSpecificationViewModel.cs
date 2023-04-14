using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class MechanicalSpecificationViewModel : ViewModelBase
    {
        public MechanicalSpecificationViewModel(MechanicalSpecificationModel _mechanicalSpecification)
        {
            this.MechanicalSpecification = _mechanicalSpecification;
        }


        public MechanicalSpecificationModel MechanicalSpecification { get; set; } = new ();

    }
}
