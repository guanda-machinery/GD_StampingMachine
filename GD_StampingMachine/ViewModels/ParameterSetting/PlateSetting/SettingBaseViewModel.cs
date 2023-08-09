using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public abstract class SettingBaseViewModel :  ParameterSettingBaseViewModel
    {
        public StampPlateSettingModel StampPlateSetting { get; set; } = new StampPlateSettingModel();

        public SheetStampingTypeFormEnum SheetStampingTypeForm
        {
            get => StampPlateSetting.SheetStampingTypeForm;
            set 
            { 
                StampPlateSetting.SheetStampingTypeForm = value; 
                OnPropertyChanged(); 
            }
        }


        public string NumberSettingMode { get => StampPlateSetting.NumberSettingMode; set { StampPlateSetting.NumberSettingMode = value; OnPropertyChanged(); } }
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount
        {
            get => StampPlateSetting.SequenceCount;
            set
            {
                StampPlateSetting.SequenceCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence
        {
            get => StampPlateSetting.SpecialSequence;
            set
            {
                StampPlateSetting.SpecialSequence = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }
        /// <summary>
        /// 水平對齊
        /// </summary>
        public HorizontalAlignEnum HorizontalAlign { get => StampPlateSetting.HorizontalAlign; set { StampPlateSetting.HorizontalAlign = value; OnPropertyChanged(); } }
        /// <summary>
        /// 垂直對齊
        /// </summary>
        public VerticalAlignEnum VerticalAlign { get => StampPlateSetting.VerticalAlign; set { StampPlateSetting.VerticalAlign = value; OnPropertyChanged(); } }

        public IronPlateMarginStruct IronPlateMargin { get => StampPlateSetting.IronPlateMargin; set { StampPlateSetting.IronPlateMargin = value; OnPropertyChanged(); } }

        [JsonIgnore]
        public ObservableCollection<int> PlateNumberList
        {
            get
            {

                int RowCount = 2;
                _ = SpecialSequence switch
                {
                    SpecialSequenceEnum.OneRow => RowCount = 1,
                    SpecialSequenceEnum.TwoRow => RowCount = 2,
                    _ => RowCount = 2,
                };

                var NumberList = new ObservableCollection<int>();

                var ListCount = SequenceCount * RowCount;
                for (int i = 0; i < ListCount; i++)
                {
                    NumberList.Add(i + 1);
                }
                return NumberList;
            }
        }


    }
}
