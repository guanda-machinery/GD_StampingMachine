using GD_StampingMachine.GD_Model;

namespace GD_StampingMachine.ViewModels
{
    public class MachanicalSpecificationViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_MachanicalSpecificationViewModel");
        public MachanicalSpecificationViewModel(MachanicalSpecificationModel _machanicalSpecification)
        {
            this.MachanicalSpecification = _machanicalSpecification;
        }


        public MachanicalSpecificationModel MachanicalSpecification { get; set; } = new();

    }
}
