
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{

    /// <summary>
    /// 金屬片設定
    /// </summary>
    public abstract class SettingBaseViewModel : ParameterSettingBaseViewModel
    {

        public SettingBaseViewModel()
        {
            StampPlateSetting = new();
        }

        public SettingBaseViewModel(StampPlateSettingModel stampPlateSetting)
        {
            StampPlateSetting = stampPlateSetting;

            PlateNumberList1 = stampPlateSetting.StampableList1.Select(x => new PlateFontViewModel(x)).ToObservableCollection();
            PlateNumberList2 = stampPlateSetting.StampableList2.Select(x => new PlateFontViewModel(x)).ToObservableCollection();
        }

        [JsonIgnore]
        public StampPlateSettingModel StampPlateSetting;

        public SheetStampingTypeFormEnum SheetStampingTypeForm 
        { 
            get => StampPlateSetting.SheetStampingTypeForm;
            set
            {
                StampPlateSetting.SheetStampingTypeForm = value;
                OnPropertyChanged(); 
            } 
        }


        //[JsonIgnore]
        /// <summary>
        /// 遇到- 換行的功能
        /// </summary>
        public bool DashAutoWrapping
        {
            get => StampPlateSetting.DashAutoWrapping;
            set
            {
                StampPlateSetting.DashAutoWrapping = value;
                OnPropertyChanged();
                ChangePlateNumberList(PlateNumber);
            }
        }

        public string NumberSettingMode
        {
            get => StampPlateSetting.NumberSettingMode; 
            set 
            {
                StampPlateSetting.NumberSettingMode = value; 
                OnPropertyChanged(); 
            } 
        }


        /// <summary>
        /// 單排數量最大值
        /// </summary>
        protected abstract int SequenceCountMax { get; }

        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount
        {
            get => StampPlateSetting.SequenceCount = Math.Min(StampPlateSetting.SequenceCount, SequenceCountMax);
            set
            {
                StampPlateSetting.SequenceCount = Math.Min(value, SequenceCountMax);
                OnPropertyChanged();
                ChangePlateNumberList(PlateNumber);
                ChangeHorizontalStampingMarginPosVM();
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
                ChangePlateNumberList(PlateNumber);
                ChangeVerticalStampingMarginPosVM();
            }
        }
        /// <summary>
        /// 水平對齊
        /// </summary>
        public HorizontalAlignEnum HorizontalAlign
        {
            get => StampPlateSetting.HorizontalAlign;
            set
            {
                StampPlateSetting.HorizontalAlign = value;
                OnPropertyChanged();
                ChangeHorizontalStampingMarginPosVM();

            }
        }

        /// <summary>
        /// 垂直對齊
        /// </summary>
        public VerticalAlignEnum VerticalAlign
        {
            get => StampPlateSetting.VerticalAlign; set
            {
                StampPlateSetting.VerticalAlign = value;
                OnPropertyChanged();
                ChangeVerticalStampingMarginPosVM();
            }
        }




        private void ChangeVerticalStampingMarginPosVM()
        {
            if (SpecialSequence == SpecialSequenceEnum.OneRow)
            {
                switch (StampPlateSetting.VerticalAlign)
                {
                    case VerticalAlignEnum.Top:
                        StampingMarginPosVM.rXAxisPos1 = 10;
                        StampingMarginPosVM.rXAxisPos2 = 25;
                        break;
                    case VerticalAlignEnum.Center:
                        StampingMarginPosVM.rXAxisPos1 = 10 + MachineConst.FontVerticalInterval / 2; ;
                        StampingMarginPosVM.rXAxisPos2 = 25 + MachineConst.FontVerticalInterval / 2;
                        break;
                    case VerticalAlignEnum.Bottom:
                        StampingMarginPosVM.rXAxisPos1 = 10 + MachineConst.FontVerticalInterval;
                        StampingMarginPosVM.rXAxisPos2 = 25 + MachineConst.FontVerticalInterval;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                StampingMarginPosVM.rXAxisPos1 = 10;
                StampingMarginPosVM.rXAxisPos2 = 25;
            }
        }

        private void ChangeHorizontalStampingMarginPosVM()
        {
            var diff = this.SequenceCountMax - SequenceCount;
            switch (StampPlateSetting.HorizontalAlign)
            {
                case HorizontalAlignEnum.Left:
                    StampingMarginPosVM.rYAxisPos1 = 14;
                    StampingMarginPosVM.rYAxisPos2 = 14;
                    break;
                case HorizontalAlignEnum.Center:
                    StampingMarginPosVM.rYAxisPos1 = 14 + (diff * MachineConst.FontHorizonInterval) / 2;// +MachineConst.FontWidth;
                    StampingMarginPosVM.rYAxisPos2 = 14 + (diff * MachineConst.FontHorizonInterval) / 2;
                    break;
                case HorizontalAlignEnum.Right:
                    StampingMarginPosVM.rYAxisPos1 = 14 + diff * MachineConst.FontHorizonInterval;
                    StampingMarginPosVM.rYAxisPos2 = 14 + diff * MachineConst.FontHorizonInterval;
                    break;
                default:
                    break;
            }
        }



        private IronPlateMarginViewModel _ironPlateMarginVM;
        public IronPlateMarginViewModel IronPlateMarginVM
        {
            get => _ironPlateMarginVM ??= new(StampPlateSetting.IronPlateMargin);
            set
            {
                _ironPlateMarginVM = value;
                StampPlateSetting.IronPlateMargin = _ironPlateMarginVM._ironPlateMargin;
                OnPropertyChanged();
            }
        }


        private StampingMarginPosViewModel _stampingMarginPosVM;
        public StampingMarginPosViewModel StampingMarginPosVM
        {
            get => _stampingMarginPosVM ??= new(StampPlateSetting.StampingMarginPos);
            set
            {
                _stampingMarginPosVM = value;
                StampPlateSetting.StampingMarginPos = _stampingMarginPosVM._stampingMarginPos;
                OnPropertyChanged();
            }
        }


        //private string _plateNumber;
        /// <summary>
        /// 板子上的字 可留白
        /// </summary>
        public string PlateNumber
        {
            get => StampPlateSetting.PlateNumber;
            set
            {
                StampPlateSetting.PlateNumber = value;
                ChangePlateNumberList(PlateNumber);
                OnPropertyChanged();
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

        public abstract Visibility QRCodeVisibility { get; }



        private ICommand _changePlateNumberListCommand;
        public ICommand ChangePlateNumberListCommand
        {
            get => _changePlateNumberListCommand ??= new AsyncRelayCommand(async () =>
            {
                await Task.CompletedTask;
                ChangePlateNumberList(PlateNumber);
            });
        }

        private void ChangePlateNumberList(string pNumber)
        {
            List<PlateFontModel> pNumberList1 = new();
            List<PlateFontModel> pNumberList2 = new();
            //先讀舊檔
            /*foreach (var setting in StampPlateSetting.StampableList.Item1)
             {
                 pNumberList1.Add(new PlateFontViewModel(setting) 
                 {
                     FontString = null });
             }

             foreach (var setting in StampPlateSetting.StampableList.Item2)
             {
                 pNumberList2.Add(new PlateFontViewModel(setting)
                 {
                     FontString = null 
                 });
             }*/

            //新增的
            /* for (int i = pNumberList1.Count; i < SequenceCount; i++)
                 pNumberList1.Add(new PlateFontViewModel() { FontIndex = (ushort)i, IsUsed = true }); ;*/

            for (int i = 0; i < SequenceCount; i++)
            {
                PlateFontModel plateFont;
                if (i < StampPlateSetting.StampableList1.Count)
                    plateFont = StampPlateSetting.StampableList1[i];
                else
                    plateFont = new PlateFontModel()
                    {
                        FontIndex = (ushort)i,
                        IsUsed = true
                    };
                pNumberList1.Add(plateFont);
            }

            //pNumberList1 = pNumberList1.Take(SequenceCount).ToObservableCollection();
            //pNumberList1.RemoveRange(SequenceCount, pNumberList1.Count- SequenceCount);

            switch (SpecialSequence)
            {
                case (SpecialSequenceEnum.OneRow):
                    pNumberList2 = new List<PlateFontModel>();
                    break;
                case (SpecialSequenceEnum.TwoRow):
                    for (int i = 0; i < SequenceCount; i++)
                    {
                        PlateFontModel plateFont;
                        if (i < StampPlateSetting.StampableList2.Count)
                            plateFont = StampPlateSetting.StampableList2[i];
                        else
                            plateFont = new PlateFontModel()
                            {
                                FontIndex = (ushort)i,
                                IsUsed = true
                            };
                        pNumberList2.Add(plateFont);
                    }




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
                            MinusIndex = pNumber.LastIndexOf('-', SearchIndexStart, SearchCount+1);
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


            StampPlateSetting.StampableList1 = pNumberList1;
            StampPlateSetting.StampableList2 = pNumberList2;

            PlateNumberList1 = pNumberList1.Select(p => new PlateFontViewModel(p)).ToObservableCollection();
            PlateNumberList2 = pNumberList2.Select(p => new PlateFontViewModel(p)).ToObservableCollection();
        }

        private ObservableCollection<PlateFontViewModel> _plateNumberList1;
        private ObservableCollection<PlateFontViewModel> _plateNumberList2;


        public ObservableCollection<PlateFontViewModel> PlateNumberList1
        {
            get
            {
                if (_plateNumberList1 == null)
                {
                    _plateNumberList1 = StampPlateSetting.StampableList1.Select(x => new PlateFontViewModel(x)).ToObservableCollection();
                }
                return _plateNumberList1;
            }
            set
            {
                _plateNumberList1 = value;
                StampPlateSetting.StampableList1 = value.Select(p => p.PlateFont).ToList();
                OnPropertyChanged();
            }
        }


        public ObservableCollection<PlateFontViewModel> PlateNumberList2
        {
            get
            {
                if (_plateNumberList2 == null)
                {
                    _plateNumberList2 = StampPlateSetting.StampableList2.Select(x => new PlateFontViewModel(x)).ToObservableCollection();
                }

                return _plateNumberList2;
            }
            set
            {
                _plateNumberList2 = value;
                StampPlateSetting.StampableList2 = value.Select(p => p.PlateFont).ToList();
                OnPropertyChanged();
            }
        }



    }

    /// <summary>
    /// 鐵片資料
    /// </summary>
    public class IronPlateMarginViewModel : BaseViewModel
    {
        public override string ViewModelName => nameof(IronPlateMarginViewModel);
        public IronPlateMarginViewModel()
        {
            _ironPlateMargin = new(); ;
        }
        public IronPlateMarginViewModel(IronPlateMarginStruct ironPlateMargin)
        {
            _ironPlateMargin = ironPlateMargin;
        }

        internal IronPlateMarginStruct _ironPlateMargin = new();
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

    /// <summary>
    /// 鐵片打點基準
    /// </summary>
    public class StampingMarginPosViewModel : BaseViewModel
    {
        public override string ViewModelName => "StampingMarginPosViewModel";

        public StampingMarginPosViewModel()
        {
            _stampingMarginPos = new();
        }
        public StampingMarginPosViewModel(StampingMarginPosModel stampingMarginPos)
        {
            _stampingMarginPos = stampingMarginPos;
        }

        internal readonly StampingMarginPosModel _stampingMarginPos;
        public float rXAxisPos1 { get => _stampingMarginPos.rXAxisPos1; set { _stampingMarginPos.rXAxisPos1 = value; OnPropertyChanged(); } }
        public float rYAxisPos1 { get => _stampingMarginPos.rYAxisPos1; set { _stampingMarginPos.rYAxisPos1 = value; OnPropertyChanged(); } }
        public float rXAxisPos2 { get => _stampingMarginPos.rXAxisPos2; set { _stampingMarginPos.rXAxisPos2 = value; OnPropertyChanged(); } }
        public float rYAxisPos2 { get => _stampingMarginPos.rYAxisPos2; set { _stampingMarginPos.rYAxisPos2 = value; OnPropertyChanged(); } }
    }



}
