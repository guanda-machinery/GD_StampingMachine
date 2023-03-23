using DevExpress.DataAccess.Json;
using DevExpress.Office.Forms;
using DevExpress.Utils.StructuredStorage.Internal;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// GridcontrolVM
    /// </summary>
    public class PartsParameterViewModel:ViewModelBase
    {
        public PartsParameterViewModel()
        {

        }
        public PartsParameterViewModel(PartsParameterModel PParameter)
        {
            PartsParameter = PParameter;
        }
        public readonly PartsParameterModel PartsParameter = new PartsParameterModel();
        public string ParameterA
        {
            get => PartsParameter.Parametert_A;
            set
            {
                PartsParameter.Parametert_A = value;
                OnPropertyChanged(nameof(ParameterA));
            }
        }
        public string ParameterB
        {
            get => PartsParameter.Parametert_B;
            set
            {
                PartsParameter.Parametert_B = value;
                OnPropertyChanged(nameof(ParameterB));
            }
        }
        public string ParameterC
        {
            get => PartsParameter.Parametert_C;
            set
            {
                PartsParameter.Parametert_C = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }
        public NumberSettingModelBase NumberSetting
        {
            get =>PartsParameter.NumberSetting;
            set 
            {
                PartsParameter.NumberSetting = value;
                OnPropertyChanged(nameof(NumberSetting));
            }
        }






    }
}
