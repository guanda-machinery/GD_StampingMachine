using DevExpress.CodeParser;
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
    /// <summary>
    /// 金屬片設定
    /// </summary>
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
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlateNumberList));
            }
        }


        /// <summary>
        /// 側面字串
        /// </summary>
        public string QR_Special_Text
        {
            get => StampPlateSetting.QR_Special_Text;
            set
            {
                StampPlateSetting.QR_Special_Text = value;
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
                if (_plateNumberList.Count <= ListCount)
                {
                    while (_plateNumberList.Count < ListCount)
                    {
                        var c = _plateNumberList.Count;
                        if (string.IsNullOrEmpty(_plateNumber))
                            _plateNumberList.Add((c + 1).ToString());
                        else
                            _plateNumberList.Add(null);
                    }
                }
                //若格數變少 則刪除
                else
                {
                    while (_plateNumberList.Count > ListCount)
                    {
                        _plateNumberList.RemoveAt(ListCount);
                    }
                }

                if (PlateNumber != null)
                {
                    var plateNumberInput = PlateNumber;
                    //第一排放不下才須尋找
                    if (SpecialSequence == SpecialSequenceEnum.TwoRow)
                    {
                        //找第一排是否有-
                        int MinusIndex;
                        if (PlateNumber.Length > SequenceCount)
                        {
                            //起始搜尋點(從後面開始找)
                            //跑循環去定位minus的位置
                            var SearchIndexStart = PlateNumber.Length - 1;
                            var SearchCount = SequenceCount;
                            do
                            {
                                MinusIndex = PlateNumber.LastIndexOf('-', SearchIndexStart, SearchCount);
                                if (MinusIndex != -1)
                                {
                                    SearchIndexStart = MinusIndex - 1;
                                    SearchCount = MinusIndex - 1;
                                    //將字串切開後 是否會落到第二排 
                                    //會落到第二排且第二排能放得下整串字串的 才移動到第二排
                                    var FirstPlateNumber = _plateNumber.Substring(0, MinusIndex + 1);
                                    var SubPlateNumber = _plateNumber.Substring(MinusIndex + 1);
                                    if (FirstPlateNumber.Length <= SequenceCount && SubPlateNumber.Length <= SequenceCount)
                                    {
                                        //補空白
                                        plateNumberInput = FirstPlateNumber.PadRight(SequenceCount) + SubPlateNumber;
                                    }
                                }
                            } while (MinusIndex != -1);
                        }
                    }
                    for (int i = 0; i < _plateNumberList.Count; i++)
                    {
                        //如果字段不夠長 留白
                        if (i < plateNumberInput.Length)
                        {
                            if (string.IsNullOrWhiteSpace(plateNumberInput[i].ToString()))
                            {
                                _plateNumberList[i] = null;
                            }
                            else
                                _plateNumberList[i] = plateNumberInput[i].ToString();
                        }
                        else
                            _plateNumberList[i] = null;
                    }
                }
                else
                    _plateNumberList.ForEach(p => p = null);




                return _plateNumberList;
            }
            

        }


    }
}
