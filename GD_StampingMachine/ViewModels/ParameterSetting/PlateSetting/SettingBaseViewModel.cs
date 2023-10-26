using DevExpress.Utils.Extensions;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
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
    /// 字模
    /// </summary>
    public class PlateFontViewModel : BaseViewModel
    {
        public override string ViewModelName => nameof(PlateFontViewModel);

        private bool _isUsed = true;
        public bool IsUsed
        {
            get => _isUsed; set { _isUsed = value; OnPropertyChanged(); }
        }

        private string _fontString;
        public string FontString
        {
            get => _fontString; set { _fontString = value; OnPropertyChanged(); }
        }

    }

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

        private ObservableCollection<PlateFontViewModel> _plateNumberList;
        [JsonIgnore]
        public ObservableCollection<PlateFontViewModel> PlateNumberList
        {
            get
            {
                if(_plateNumberList == null)
                {
                    _plateNumberList = new ObservableCollection<PlateFontViewModel>();
                    foreach(var use in StampPlateSetting.StampableList)
                    {
                        _plateNumberList.Add(new PlateFontViewModel() { IsUsed = use, FontString = null });
                    }

                }

                int RowCount;
                _ = SpecialSequence switch
                {
                    SpecialSequenceEnum.OneRow => RowCount = 1,
                    SpecialSequenceEnum.TwoRow => RowCount = 2,
                    _ => RowCount = 0,
                };

                var ListCount = SequenceCount * RowCount;
                //若格數不夠 則擴充
                if (_plateNumberList.Count < ListCount)
                {
                    while (_plateNumberList.Count < ListCount)
                    {
                        _plateNumberList.Add(new PlateFontViewModel 
                        { 
                            IsUsed = true, 
                            FontString = null 
                        });
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
                        var firstPlateNumber = _plateNumberList.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);
                        var secondPlateNumber = _plateNumberList.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);

                        //找第一排是否有-
                        int MinusIndex;
                        if (PlateNumber.Length > firstPlateNumber.Count)
                        {
                            //起始搜尋點(從後面開始找)
                            //跑循環去定位minus的位置
                            var SearchIndexStart = PlateNumber.Length - 1;
                            var SearchCount = firstPlateNumber.Count;
                            do
                            {
                                MinusIndex = PlateNumber.LastIndexOf('-', SearchIndexStart, SearchCount);
                                if (MinusIndex != -1)
                                {
                                    SearchIndexStart = MinusIndex - 1;
                                    SearchCount = MinusIndex - 1;
                                    //將字串切開後 是否會落到第二排 
                                    //會落到第二排且第二排能放得下整串字串的 才移動到第二排
                                    var FirstPlateNumber = PlateNumber.Substring(0, MinusIndex + 1);
                                    var SubPlateNumber = PlateNumber.Substring(MinusIndex + 1);
                                    if (FirstPlateNumber.Length <= firstPlateNumber.Count && SubPlateNumber.Length <= firstPlateNumber.Count)
                                    {
                                        //補空白
                                        int i = 0;
                                        var plateInput = string.Empty;
                                        foreach (var fontPlate in _plateNumberList)
                                        {
                                            if (fontPlate.IsUsed)
                                            {
                                                plateInput += PlateNumber[i];
                                                i++;
                                            }
                                            else
                                            {
                                                plateInput += " ";
                                            }
                                        }
                                        plateNumberInput = plateInput;


                                        //補空白
                                        // plateNumberInput = FirstPlateNumber.PadRight(firstLineCount) + SubPlateNumber;
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
                                _plateNumberList[i].FontString = null;
                            }
                            else
                                _plateNumberList[i].FontString = plateNumberInput[i].ToString();
                        }
                        else
                            _plateNumberList[i].FontString = null;
                    }
                }
                else
                {
                    int j = 1;
                    _plateNumberList.ForEach(p =>
                    {
                            p.FontString = j.ToString();

                    });
                }

                StampPlateSetting.StampableList = _plateNumberList.ToList().Select(x => x.IsUsed).ToList();

                return _plateNumberList;
            }
        }








    }
}
