
using DevExpress.Mvvm.Native;
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
        public virtual int SequenceCount
        {
            get => StampPlateSetting.SequenceCount;
            set
            {
                StampPlateSetting.SequenceCount = value;
                OnPropertyChanged();
                (PlateNumberList1,PlateNumberList2) = ChangePlateNumberList(PlateNumber);
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
                (PlateNumberList1,PlateNumberList2) = ChangePlateNumberList(PlateNumber);
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

        private IronPlateMarginViewModel _ironPlateMarginVM;
        public IronPlateMarginViewModel IronPlateMarginVM
        { 
            get=> _ironPlateMarginVM ??= new (StampPlateSetting.IronPlateMargin);
            set
            {
                _ironPlateMarginVM = value;
                StampPlateSetting.IronPlateMargin = _ironPlateMarginVM._ironPlateMargin;
                OnPropertyChanged();
            }
        } 
        // public IronPlateMarginStruct IronPlateMargin     
        public class IronPlateMarginViewModel : BaseViewModel
        {
            public override string ViewModelName => nameof(IronPlateMarginViewModel);

           public IronPlateMarginViewModel(IronPlateMarginStruct ironPlateMargin)
            {
                _ironPlateMargin = ironPlateMargin;
            }

            internal IronPlateMarginStruct _ironPlateMargin= new();
            public double A_Margin 
            {
                get => _ironPlateMargin.A_Margin; 
                set 
                {
                    _ironPlateMargin.A_Margin = value; 
                    OnPropertyChanged(); 
                }
            }
            public double B_Margin
            {
                get => _ironPlateMargin.B_Margin;
                set
                {
                    _ironPlateMargin.B_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double C_Margin
            {
                get => _ironPlateMargin.C_Margin;
                set
                {
                    _ironPlateMargin.C_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double D_Margin
            {
                get => _ironPlateMargin.D_Margin;
                set
                {
                    _ironPlateMargin.D_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double E_Margin
            {
                get => _ironPlateMargin.E_Margin;
                set
                {
                    _ironPlateMargin.E_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double F_Margin
            {
                get => _ironPlateMargin.F_Margin;
                set
                {
                    _ironPlateMargin.F_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double G_Margin
            {
                get => _ironPlateMargin.G_Margin;
                set
                {
                    _ironPlateMargin.G_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double H_Margin
            {
                get => _ironPlateMargin.H_Margin;
                set
                {
                    _ironPlateMargin.H_Margin = value;
                    OnPropertyChanged();
                }
            }
            public double I_Margin
            {
                get => _ironPlateMargin.I_Margin;
                set
                {
                    _ironPlateMargin.I_Margin = value;
                    OnPropertyChanged();
                }
            }

        }








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
                (PlateNumberList1, PlateNumberList2) = ChangePlateNumberList(PlateNumber);

            }
        }



        /// <summary>
        /// QR字串(可留白)
        /// </summary>
        public string QrCodeContent
        {
            get => StampPlateSetting.QrCodeContent;
            set
            {

                StampPlateSetting.QrCodeContent = value;
                OnPropertyChanged();
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





        protected (ObservableCollection<PlateFontViewModel>, ObservableCollection<PlateFontViewModel>) ChangePlateNumberList(string pNumber )
        {
            ObservableCollection<PlateFontViewModel> pNumberList1 = new();
            ObservableCollection<PlateFontViewModel> pNumberList2 = new();

            //先讀舊檔
            foreach (var setting in StampPlateSetting.StampableList.Item1)
            {
                pNumberList1.Add(new PlateFontViewModel() { IsUsed = setting.IsUsed, FontString = null });
            }

            foreach (var setting in StampPlateSetting.StampableList.Item2)
            {
                pNumberList2.Add(new PlateFontViewModel() { IsUsed = setting.IsUsed, FontString = null });
            }

            for (int i = 0; i < SequenceCount; i++)
                pNumberList1.Add(new PlateFontViewModel() { IsUsed =true});

            pNumberList1 = pNumberList1.Take(SequenceCount).ToObservableCollection();
            //pNumberList1.RemoveRange(SequenceCount, pNumberList1.Count- SequenceCount);

            switch (SpecialSequence)
            {
                case (SpecialSequenceEnum.OneRow):
                    pNumberList2 = new ObservableCollection<PlateFontViewModel>();
                    break;
                case (SpecialSequenceEnum.TwoRow):
                    for (int i = 0; i < SequenceCount; i++)
                        pNumberList2.Add(new PlateFontViewModel() { IsUsed = true});
                    pNumberList2 = pNumberList2.Take(SequenceCount).ToObservableCollection();
                   // pNumberList2.RemoveRange(SequenceCount, pNumberList2.Count - SequenceCount);
                    break;
                    default: 
                    throw new ArgumentException();
            }

            if (pNumber != null)
            {
                pNumberList1.ForEach(p => p.IsUsedEditedable = false);
                pNumberList2.ForEach(p => p.IsUsedEditedable = false);

                var plateNumberInput = pNumber;
                //第一排放不下才須尋找
                if (SpecialSequence == SpecialSequenceEnum.TwoRow && pNumber.Contains("-") && DashAutoWrapping)
                {
                    var firstPlateNumber = pNumberList1.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);
                   // var secondPlateNumber = pNumberList.ToList().GetRange(0, SequenceCount).FindAll(x => x.IsUsed);

                    //找第一排是否有-
                    if (pNumber.Length > firstPlateNumber.Count)
                    {
                        int MinusIndex;
                        //起始搜尋點(從後面開始找)
                        //跑循環去定位minus的位置
                        var SearchIndexStart = pNumber.Length - 1;
                        var SearchCount = firstPlateNumber.Count;
                        do
                        {
                            MinusIndex = pNumber.LastIndexOf('-', SearchIndexStart, SearchCount);
                            if (MinusIndex != -1)
                            {
                                SearchIndexStart = MinusIndex - 1;
                                SearchCount = MinusIndex - 1;
                                //將字串切開後 是否會落到第二排 
                                //會落到第二排且第二排能放得下整串字串的 才移動到第二排
                                var FirstPlateNumber = pNumber.Substring(0, MinusIndex + 1);
                                var SubPlateNumber = pNumber.Substring(MinusIndex + 1);
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
                for (int i = 0; i < pNumberList1.Count; i++)
                {
                    //可使用的位置才能刻字
                    if (pNumberList1[i].IsUsed)
                    {
                        if (num < plateNumberInput.Length)
                        {
                            if (string.IsNullOrWhiteSpace(plateNumberInput[num].ToString()))
                            {
                                pNumberList1[i].FontString = null;
                            }
                            else
                                pNumberList1[i].FontString = plateNumberInput[num].ToString();
                            num++;
                        }
                        else
                            pNumberList1[i].FontString = null;
                    }
                    else
                        pNumberList1[i].FontString = null;
                }

                //int num2 = 0;
                for (int i = 0; i < pNumberList2.Count; i++)
                {
                    //可使用的位置才能刻字
                    if (pNumberList2[i].IsUsed)
                    {
                        if (num < plateNumberInput.Length)
                        {
                            if (string.IsNullOrWhiteSpace(plateNumberInput[num].ToString()))
                            {
                                pNumberList2[i].FontString = null;
                            }
                            else
                                pNumberList2[i].FontString = plateNumberInput[num].ToString();
                            num++;
                        }
                        else
                            pNumberList2[i].FontString = null;
                    }
                    else
                        pNumberList2[i].FontString = null;
                }
            }
            else
            {
                int j = 1;
                pNumberList1.ForEach(p =>
                {
                    p.FontString = j.ToString();
                    p.IsUsedEditedable = true;
                    j++;
                }); 

                pNumberList2.ForEach(p =>
                {
                    p.FontString = j.ToString();
                    p.IsUsedEditedable = true;
                    j++;
                });
            }

            StampPlateSetting.StampableList = (new List<PlateFontModel>(), new List<PlateFontModel>());
            pNumberList1.ForEach(p =>
            {
                StampPlateSetting.StampableList.Item1.Add(p.PlateFont);
            }); 

            pNumberList2.ForEach(p =>
            {
                StampPlateSetting.StampableList.Item2.Add(p.PlateFont);
            });


            return (pNumberList1 ,pNumberList2);
        }

        private ObservableCollection<PlateFontViewModel> _plateNumberList1;
        private ObservableCollection<PlateFontViewModel> _plateNumberList2;
        [JsonIgnore]
        public ObservableCollection<PlateFontViewModel> PlateNumberList1
        {
            get
            {
                if (_plateNumberList1 == null)
                    (_plateNumberList1, _plateNumberList2) = ChangePlateNumberList(PlateNumber);
                return _plateNumberList1;
            }
            set
            {
                _plateNumberList1 = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public ObservableCollection<PlateFontViewModel> PlateNumberList2
        {
            get
            {
                if (_plateNumberList2 == null)
                    (_plateNumberList1,_plateNumberList2) = ChangePlateNumberList(PlateNumber);
                return _plateNumberList2;
            }
            set
            {
                _plateNumberList2 = value;
                OnPropertyChanged();
            }
        }








    }
}
