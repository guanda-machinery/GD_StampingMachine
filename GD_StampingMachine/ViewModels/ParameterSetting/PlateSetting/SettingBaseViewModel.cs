using DevExpress.Mvvm.Native;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public abstract class SettingBaseViewModel :  ParameterSettingBaseViewModel
    {
        private StampPlateSettingModel _stampPlateSetting;
        public StampPlateSettingModel StampPlateSetting 
        { 
            get=> _stampPlateSetting ??= new StampPlateSettingModel(); 
            protected set => _stampPlateSetting=value; 
        }

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




        private string _plateNumber;
        /// <summary>
        /// 板子上的字 可留白
        /// </summary>
        public string PlateNumber
        {
            get => _plateNumber;
            set
            {
                _plateNumber = value;
               if (_plateNumber != null)
               {
                    //找是否有-
                    for (int i = 0; i < PlateNumberList.Count; i++)
                    {
                        //如果字段不夠長 留白
                        if (i < _plateNumber.Length)
                            PlateNumberList[i] = _plateNumber[i].ToString();
                        else
                            PlateNumberList[i] = null;
                    }
                }



                OnPropertyChanged();
            }
        } 


        private ObservableCollection<string> _plateNumberList;
        [JsonIgnore]
        public ObservableCollection<string> PlateNumberList
        {
            get
            {
                 _plateNumberList ??= new ObservableCollection<string>();
                
                int RowCount;
                _ = SpecialSequence switch
                {
                    SpecialSequenceEnum.OneRow => RowCount = 1,
                    SpecialSequenceEnum.TwoRow => RowCount = 2,
                    _ => RowCount = 0,
                };

                var ListCount = SequenceCount * RowCount;
                //若格數不夠 則擴充
                while(_plateNumberList.Count< ListCount)
                {
                    var c = _plateNumberList.Count;
                    if (string.IsNullOrEmpty(_plateNumber))
                        _plateNumberList.Add((c + 1).ToString());
                    else
                        _plateNumberList.Add(null);
                }

                return _plateNumberList;
            }
            

        }


    }
}
