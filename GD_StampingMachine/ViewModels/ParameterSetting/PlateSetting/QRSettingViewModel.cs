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
using System.Windows;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 繼承
    /// </summary>
    public class QRSettingViewModel : SettingBaseViewModel//, IStampingPlateVM
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SettingViewModelQRViewModel");

        public QRSettingViewModel()
        {
            this.StampPlateSetting = new StampPlateSettingModel();
            this.StampPlateSetting.SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping;
        }
        public QRSettingViewModel(StampPlateSettingModel stampPlateSetting)
        {
            this.StampPlateSetting = stampPlateSetting;
            this.StampPlateSetting.SheetStampingTypeForm = SheetStampingTypeFormEnum.QRSheetStamping;
        }

        /// <summary>
        /// Code設定 字元數量
        /// </summary>
        public int CharactersCount { get => StampPlateSetting.CharactersCount; set { StampPlateSetting.CharactersCount = value; OnPropertyChanged(); } }
        /// <summary>
        /// Code設定 字元型態
        /// </summary>
        public CharactersFormEnum CharactersForm { get => StampPlateSetting.CharactersForm; set { StampPlateSetting.CharactersForm = value; OnPropertyChanged(); } }

        public string ModelSize { get => StampPlateSetting.ModelSize; set { StampPlateSetting.ModelSize = value; OnPropertyChanged(); } }

    }










}
