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
        public readonly PlateFontModel PlateFont = new PlateFontModel();

        public override string ViewModelName => nameof(PlateFontViewModel);
        public bool IsUsed
        {
            get => PlateFont.IsUsed; 
            set 
            {
                PlateFont.IsUsed = value;
                    OnPropertyChanged();
                
            }
        }

        public string FontString
        {
            get => PlateFont.FontString; set { PlateFont.FontString = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 是否可變更IsUsed
        /// </summary>
        public bool IsUsedEditedable
        {
            get => PlateFont.IsUsedEditedable; set { PlateFont.IsUsedEditedable = value; OnPropertyChanged(); }
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

        [JsonIgnore]
        /// <summary>
        /// 遇到- 換行的功能
        /// </summary>
        public bool DashAutoWrapping
        {
            get => !PlateNumber.Contains(" ");
           /* get => StampPlateSetting.DashAutoWrapping;
            set
            {
                StampPlateSetting.DashAutoWrapping = value;
                OnPropertyChanged();
                ChangePlateNumberList();
            }*/
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
                ChangePlateNumberList();
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
                ChangePlateNumberList();
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
                ChangePlateNumberList();
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





        private void ChangePlateNumberList()
        {
            if (_plateNumberList == null)
            {
                _plateNumberList = new ObservableCollection<PlateFontViewModel>();
                foreach (var setting in StampPlateSetting.StampableList)
                {
                    _plateNumberList.Add(new PlateFontViewModel() { IsUsed = setting.IsUsed, FontString = null });
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
                _plateNumberList.ForEach(p => p.IsUsedEditedable = false);

                var plateNumberInput = PlateNumber;
                //第一排放不下才須尋找
                if (SpecialSequence == SpecialSequenceEnum.TwoRow && PlateNumber.Contains("-") && DashAutoWrapping)
                {
                    var firstPlateNumber = _plateNumberList.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);
                    var secondPlateNumber = _plateNumberList.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);

                    //找第一排是否有-
                    if (PlateNumber.Length > firstPlateNumber.Count)
                    {
                        int MinusIndex;
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
                                    //可換行
                                    plateNumberInput = FirstPlateNumber.PadRight(firstPlateNumber.Count) + SubPlateNumber;
                                }
                            }
                        } while (MinusIndex != -1);
                    }
                }



                int num = 0;
                for (int i = 0; i < _plateNumberList.Count; i++)
                {
                    //可使用的位置才能刻字
                    if (_plateNumberList[i].IsUsed)
                    {
                        if (num < plateNumberInput.Length)
                        {
                            if (string.IsNullOrWhiteSpace(plateNumberInput[num].ToString()))
                            {
                                _plateNumberList[i].FontString = null;
                            }
                            else
                                _plateNumberList[i].FontString = plateNumberInput[num].ToString();
                            num++;
                        }
                        else
                            _plateNumberList[i].FontString = null;
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
                    p.IsUsedEditedable = true;
                    j++;
                });
            }

            StampPlateSetting.StampableList = new List<PlateFontModel>();
            _plateNumberList.ForEach(p =>
            {
                StampPlateSetting.StampableList.Add(p.PlateFont);
            });

            OnPropertyChanged(nameof(PlateNumberList));
        }

        private ObservableCollection<PlateFontViewModel> _plateNumberList;
        [JsonIgnore]
        public ObservableCollection<PlateFontViewModel> PlateNumberList
        {
            get
            {
                if (_plateNumberList == null)
                    ChangePlateNumberList();
                return _plateNumberList;
            }
            set
            {
                _plateNumberList = value;
                OnPropertyChanged();
            }
        }








    }
}
