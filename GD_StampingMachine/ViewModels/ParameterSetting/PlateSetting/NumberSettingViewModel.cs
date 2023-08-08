using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_StampingMachine.Interfaces;
using GD_StampingMachine.Model;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class NumberSettingViewModel : SettingBaseViewModel
    {

        public NumberSettingViewModel()
        {
            StampPlateSetting = new();
        }
        public NumberSettingViewModel(StampPlateSettingModel stampPlateSetting)
        {
            StampPlateSetting = stampPlateSetting;
        }


        private StampPlateSettingModel _stampPlateSetting = new StampPlateSettingModel();
        public override StampPlateSettingModel StampPlateSetting
        {
            get => _stampPlateSetting;
            set
            {
                _stampPlateSetting = value; 
                OnPropertyChanged();
            }
        }

    }










}
