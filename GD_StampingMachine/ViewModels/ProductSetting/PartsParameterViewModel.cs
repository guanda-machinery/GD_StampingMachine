using CommunityToolkit.Mvvm.Input;
using DevExpress.DataAccess.Json;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpo.DB;
using DevExpress.XtraRichEdit.Layout;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model.ProductionSetting;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ParameterSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using static Opc.Ua.NodeState;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// ABC參數加工型
    /// </summary>
    public class PartsParameterViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_PartsParameterViewModel");

        public PartsParameterViewModel()
        {
            PartsParameter = new();
            //SettingBaseVM.StampPlateSetting = PartsParameter.StampingPlate;
        }
        public PartsParameterViewModel(PartsParameterModel PParameter)
        {
            PartsParameter = PParameter;
            //SettingBaseVM.StampPlateSetting = PartsParameter.StampingPlate;
        }


        //private PartsParameterModel? _partsParameter;
        public readonly PartsParameterModel PartsParameter;


        public event EventHandler? StateChanged;


        /// <summary>
        /// 加工進程
        /// </summary>
        public float FinishProgress
        {
            get => PartsParameter.Processing;
            set
            {
                PartsParameter.Processing = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        ///加工分配專案名
        /// </summary>
        public string? DistributeName
        {
            get => PartsParameter.DistributeName;
            set
            {
                if(PartsParameter.DistributeName != value)
                {
                    PartsParameter.DistributeName = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// 製品設定專案名
        /// </summary>
        public string ProductProjectName
        {
            get => PartsParameter.ProductProjectName;
            set
            {
                if (PartsParameter.ProductProjectName != value)
                {
                    PartsParameter.ProductProjectName = value;
                    OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// 加工識別id
        /// </summary>
        public int ID
        {
            get => PartsParameter.ID;
            set
            {
                PartsParameter.ID = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// QR陣列已完成
        /// </summary>
        public bool DataMatrixIsFinish
        {
            get => PartsParameter.DataMatrixIsFinish;
            set
            {
                if (PartsParameter.DataMatrixIsFinish != value)
                {
                    PartsParameter.DataMatrixIsFinish = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// 鋼印已完成
        /// </summary>
        public bool EngravingIsFinish
        {
            get => PartsParameter.EngravingIsFinish;
            set
            {
                if (PartsParameter.EngravingIsFinish != value)
                {
                    PartsParameter.EngravingIsFinish = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// 已剪斷
        /// </summary>
        public bool ShearingIsFinish
        {
            get => PartsParameter.ShearingIsFinish;
            set
            {
                if(PartsParameter.ShearingIsFinish != value)
                {
                    PartsParameter.ShearingIsFinish = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// 已完成
        /// </summary>
        public bool IsFinish
        {
            get => PartsParameter.IsFinish;
            set
            {
                if (PartsParameter.IsFinish != value)
                {
                    PartsParameter.IsFinish = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public bool IsTransported
        {
            get => PartsParameter.IsTransported;
            set
            {
                if (PartsParameter.IsTransported != value)
                {
                    PartsParameter.IsTransported = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }






        /// <summary>
        /// 已送進機台內
        /// </summary>
        public bool IsSended
        {
            get => PartsParameter.SendMachineCommand.IsSended;
            set
            {
                if (PartsParameter.SendMachineCommand.IsSended != value)
                {
                    PartsParameter.SendMachineCommand.IsSended = value;
                    OnPropertyChanged();
                    StateChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 加工順序編號
        /// </summary>
        public int WorkIndex
        {
            get => PartsParameter.SendMachineCommand.WorkIndex;
            set
            {
                if (PartsParameter.SendMachineCommand.WorkIndex != value)
                {
                    PartsParameter.SendMachineCommand.WorkIndex = value;
                    OnPropertyChanged();

                    IsScheduled = value != -1;
                }
            }
        }




        private bool? _isScheduled;
        public bool IsScheduled
        {
            get => _isScheduled ??= WorkIndex!=-1;
            private set
            {
                if (_isScheduled != value)
                {
                    _isScheduled = value;
                    OnPropertyChanged();
                }
            }
        }




        public string ParameterA
        {
            get => PartsParameter.IronPlateString;
            set
            {
                if (PartsParameter.IronPlateString != value || SettingBaseVM.PlateNumber != value)
                {
                    PartsParameter.IronPlateString = value;
                    SettingBaseVM.PlateNumber = value;
                    OnPropertyChanged();
                }
            }
        }


        /*public string ParameterB
        {
            get => PartsParameter.ParamB;
            set
            {
                PartsParameter.ParamB = value;
                OnPropertyChanged();
            }
        }*/
        public string ParameterC
        {
            get => PartsParameter.QrCodeContent;
            set
            {
                if (PartsParameter.QrCodeContent != value || SettingBaseVM.QrCodeContent != value)
                {
                    PartsParameter.QrCodeContent = value;
                    SettingBaseVM.QrCodeContent = value;
                    OnPropertyChanged(nameof(ParameterC));
                }
            }
        }

        /*public string IronPlateString
        { 
            get=> PartsParameter.IronPlateString; 
            set
            { 
                PartsParameter.IronPlateString = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }*/

        /*public string QrCodeContent
        {
            get => PartsParameter.ParamC;
            set
            {
                PartsParameter.ParamC = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingBaseVM));
            }
        }*/

        /// <summary>
        /// 側邊字串(橫著打)
        /// </summary>
        public string QR_Special_Text
        {
            get => PartsParameter.QR_Special_Text;
            set
            {
                if (PartsParameter.QR_Special_Text != value || SettingBaseVM.QR_Special_Text != value)
                {
                    PartsParameter.QR_Special_Text = value;
                    SettingBaseVM.QR_Special_Text = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// (加工)盒子編號
        /// </summary>
        public int? BoxIndex
        {
            get => PartsParameter.BoxIndex;
            set
            {
                if (
                PartsParameter.BoxIndex != value)
                {
                    PartsParameter.BoxIndex = value;
                    OnPropertyChanged();
                }
            }
        }






        private SettingBaseViewModel? _settingBaseVM;//= new NumberSettingViewModel();
        /// <summary>
        /// 金屬牌樣式
        /// </summary>
        public SettingBaseViewModel SettingBaseVM
        {
            get
            {
                if (PartsParameter.StampingPlate.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                {
                    _settingBaseVM ??= new QRSettingViewModel(PartsParameter.StampingPlate);
                }
                else
                {
                    _settingBaseVM ??= new NumberSettingViewModel(PartsParameter.StampingPlate);
                }

                _settingBaseVM.PlateNumber = this.ParameterA;
                _settingBaseVM.QR_Special_Text = this.QR_Special_Text;
                _settingBaseVM.QrCodeContent = this.ParameterC;
                return _settingBaseVM;
            }
            set
            {
                _settingBaseVM = value;
                if (_settingBaseVM != null)
                {
                    PartsParameter.StampingPlate = _settingBaseVM.StampPlateSetting;

                    _settingBaseVM.PlateNumber = this.ParameterA;
                    _settingBaseVM.QR_Special_Text = this.QR_Special_Text;
                    _settingBaseVM.QrCodeContent = this.ParameterC;
                }
                OnPropertyChanged();
            }
        }



        public bool _editPartDraggableIsPopup;
        /// <summary>
        /// 編輯視窗
        /// </summary>
        public bool EditPartDraggableIsPopup
        {
            get => _editPartDraggableIsPopup;
            set
            {
                if (_editPartDraggableIsPopup != value)
                {
                    _editPartDraggableIsPopup = value;
                    OnPropertyChanged();
                }
            }
        }


        [JsonIgnore]
        public RelayCommand ProjectEditCommand
        {
            get => new(() =>
            {
                EditPartDraggableIsPopup = true;
            });
        }




        // private SendMachineCommandViewModel? _sendMachineCommandVM;

        //  [JsonIgnore]
        /* public SendMachineCommandViewModel SendMachineCommandVM
         {
             get => _sendMachineCommandVM ??= new SendMachineCommandViewModel(PartsParameter.SendMachineCommand);
             set { _sendMachineCommandVM = value;OnPropertyChanged();}
         }*/
    }






    /// <summary>
    /// ABC參數加工表
    /// </summary>
    public class PartsParameterViewModelObservableCollection : ObservableCollection<PartsParameterViewModel>
    {

        public PartsParameterViewModelObservableCollection() : base()
        {



        }



        public PartsParameterViewModelObservableCollection(List<PartsParameterViewModel> list) : base(list)
        {
            foreach (var item in list)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            CalcNotifyCollectionChange();
        }

        public PartsParameterViewModelObservableCollection(IEnumerable<PartsParameterViewModel> collection):base(collection) 
        {

            if (collection == null)
            {
                return;
            }
            foreach (var item in collection)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }

            CalcNotifyCollectionChange();
        }

        public void AddRange(IEnumerable<PartsParameterViewModel> items)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                (this.Items as List<PartsParameterViewModel>)?.AddRange(items);
                var NotifyE = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items as List<PartsParameterViewModel>);
                this.OnCollectionChanged(NotifyE);
            });
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is PartsParameterViewModel item)
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldItems in e.OldItems)
                {
                    if (oldItems is PartsParameterViewModel item)
                    {
                        item.PropertyChanged += Item_PropertyChanged;

                    }
                }
            }

            CalcNotifyCollectionChange();
            //CalcSeparateBoxValue();

            base.OnCollectionChanged(e);
        }
        protected override void InsertItem(int index, PartsParameterViewModel item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            Application.Current.Dispatcher.Invoke(() =>
            {
                base.InsertItem(index, item);
            });
            CalcNotifyCollectionChange();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CalcNotifyCollectionChange();
        }

        private void CalcNotifyCollectionChange()
        {
            ItemFinish = this.Select(p => p.IsFinish);
            FinishProgress = this.Any() ? this.Average(p => p.FinishProgress) : 0;
            UnFinishedCount = this.Any() ? this.Count(p => !p.IsFinish) : 0;
            NotAssignedProductProjectCount = this.Any() ? this.Count(p => string.IsNullOrEmpty(p.DistributeName) && !p.IsFinish) : 0;
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= Item_PropertyChanged;

            Application.Current.Dispatcher.Invoke(() =>
            {
                base.RemoveItem(index);
            });
            CalcNotifyCollectionChange();
        }




        private IEnumerable<bool>? _itemFinish;
        /// <summary>
        /// 進度條(平均值)
        /// </summary>
        public IEnumerable<bool> ItemFinish
        {
            get => _itemFinish ??= new List<bool>();
            set
            {
                _itemFinish = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemFinish)));
            }
        }

        private float _finishProgress;
        /// <summary>
        /// 進度條(平均值)
        /// </summary>
        public float FinishProgress
        {
            get => _finishProgress;
            set
            {
                _finishProgress = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(FinishProgress)));
            }
        }

        private int _unFinishedCount;
        /// <summary>
        /// 未完成的總和
        /// </summary>
        public int UnFinishedCount
        {
            get => _unFinishedCount;
            private set
            {
                _unFinishedCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(UnFinishedCount)));
            }
        }


        private int _notAssignedProductProjectCount;
        /// <summary>
        /// 未排版的資料
        /// </summary>
        public int NotAssignedProductProjectCount
        {
            get => _notAssignedProductProjectCount;
            private set
            {
                _notAssignedProductProjectCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(NotAssignedProductProjectCount)));
            }
        }



    }






}



