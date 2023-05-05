using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class AxisSettingViewModel : ParameterSettingBaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_AxisSettingViewModel");
        
        public AxisSettingViewModel(AxisSettingModel AxisSettingModel)
        {
            AxisSetting = AxisSettingModel;
        }
        public AxisSettingModel AxisSetting { get; } = new();

        public double XAxisSpeed 
        {
            get => AxisSetting.XAxisSpeed;
            set{ AxisSetting.XAxisSpeed = value; OnPropertyChanged(); }
        }
        public double YAxisSpeed
        {
            get => AxisSetting.YAxisSpeed;
            set{ AxisSetting.YAxisSpeed = value; OnPropertyChanged(); }
        }
        public double FontDepth
        {
            get => AxisSetting.FontDepth;
            set{ AxisSetting.FontDepth = value; OnPropertyChanged(); }
        }
        public double RouletteSpeed
        {
            get => AxisSetting.RouletteSpeed;
            set{ AxisSetting.RouletteSpeed = value; OnPropertyChanged(); }
        }


        public double ZAxisPressure
        {
            get => AxisSetting.ZAxisPressure;
            set{ AxisSetting.ZAxisPressure = value; OnPropertyChanged(); }
        }
        public double ZAxisOrigin
        {
            get => AxisSetting.ZAxisOrigin;
            set{ AxisSetting.ZAxisOrigin = value; OnPropertyChanged(); }
        }
        public double ZAxisPreparationPoint
        {
            get => AxisSetting.ZAxisPreparationPoint;
            set{ AxisSetting.ZAxisPreparationPoint = value; OnPropertyChanged(); }
        }

        public double FeedDistance
        {
            get => AxisSetting.FeedDistance;
            set { AxisSetting.FeedDistance = value; OnPropertyChanged(); }
        }




        public override ICommand RecoverSettingCommand => throw new NotImplementedException();

        public override ICommand SaveSettingCommand => throw new NotImplementedException();

        public override ICommand LoadSettingCommand => throw new NotImplementedException();

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }
}
