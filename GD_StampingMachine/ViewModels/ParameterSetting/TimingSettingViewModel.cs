using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 計時設定
    /// </summary>
    public class TimingSettingViewModel : ParameterSettingBaseViewModel
    {
        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TimingSettingViewModel");


        public TimingSettingViewModel(TimingSettingModel timingSetting)
        {
            this.TimingSetting = timingSetting;

            TimingControlVMCollection = new ObservableCollection<TimingControlViewModel>(timingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));
        }

        //private TimingSettingModel _timingSetting;
        [JsonIgnore]
        public TimingSettingModel TimingSetting { get; private set; }



        private ICommand _addNewTimingControlCommand;
        public ICommand AddNewTimingControlCommand
        {
            get => _addNewTimingControlCommand ??= new RelayCommand(() =>
            {
                //TimingControlVMCollection.Add(new TimingControlViewModel());
                TimingControlVMSelectedClone = new TimingControlViewModel();
                IsBottomDrawerOpen = true;
                IsNewTimingControl = true;
            });
        }
        private ICommand _saveNewTimingControlCommand;
        public ICommand SaveNewTimingControlCommand
        {
            get => _saveNewTimingControlCommand ??= new RelayCommand(() =>
            {
                var newTC = TimingControlVMSelectedClone.DeepCloneByJson();
                newTC.IsEdit = EditButtonIsChecked;
                TimingControlVMCollection.Add(newTC);

                
                OnPropertyChanged(nameof(TimingControlVMCollection));

                TimingControlVMSelectedClone = new TimingControlViewModel();
                IsBottomDrawerOpen = false;
            });
        }

        private bool _isNewTimingControl;
        public bool IsNewTimingControl
        { get => _isNewTimingControl; set { _isNewTimingControl = value; OnPropertyChanged(); } }




        //private List<TimingControlModel> _timingControlCollection = new();

        private ObservableCollection<TimingControlViewModel> _timingControlVMCollection;
        public ObservableCollection<TimingControlViewModel> TimingControlVMCollection
        {
            get
            {
                if (_timingControlVMCollection == null)
                {
                    _timingControlVMCollection = new ObservableCollection<TimingControlViewModel>(TimingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));
                }
                else
                {
                    TimingSetting.TimingControlCollection = _timingControlVMCollection.Select(x => x.timingControlModel).ToList();
                }
                return _timingControlVMCollection;
            }
            set
            {
                _timingControlVMCollection = value;
                TimingSetting.TimingControlCollection = value.Select(x => x.timingControlModel).ToList();
                OnPropertyChanged();
            }
        }

        private TimingControlViewModel _timingControlVMSelected;
        public TimingControlViewModel TimingControlVMSelected
        {
            get => _timingControlVMSelected ??= new();
            set
            {
                _timingControlVMSelected = value;
                if (value != null)
                    TimingControlVMSelectedClone = value.DeepCloneByJson();
                OnPropertyChanged();
            }
        }

        private TimingControlViewModel _timingControlVMSelectedClone;
        public TimingControlViewModel TimingControlVMSelectedClone
        {
            get => _timingControlVMSelectedClone;
            set
            {
                _timingControlVMSelectedClone = value; OnPropertyChanged();
            }
        }

        private ICommand _timingControlVMCollectionListBox_SelectionChangedlCommand;
        public ICommand TimingControlVMCollectionListBox_SelectionChangedlCommand
        {
            get => _timingControlVMCollectionListBox_SelectionChangedlCommand ??= new RelayCommand<object>(obj =>
            {
                if (obj is SelectionChangedEventArgs e)
                {
                    IsNewTimingControl = false;
                    if (e.AddedItems.Count > 0)
                    {
                        IsBottomDrawerOpen = true;
                    }
                    else
                    {
                        IsBottomDrawerOpen = false;
                    }


                }
            });
        }

        private ICommand _finsihTimingControlChangedlCommand;
        public ICommand FinsihTimingControlChangedlCommand
        {
            get => _finsihTimingControlChangedlCommand ??= new RelayCommand<object>(obj =>
            {
                var index = TimingControlVMCollection.IndexOf(TimingControlVMSelected);
                if (index != -1)
                    TimingControlVMCollection[index] = TimingControlVMSelectedClone;



            });
        }


        private bool _editButtonIsChecked;
        public bool EditButtonIsChecked
        {
            get => _editButtonIsChecked;
            set
            {
                _editButtonIsChecked = value;OnPropertyChanged();
            }
        }


        private ICommand _editTimingControlCommand;
        public ICommand EditTimingControlCommand
        {
            get => _editTimingControlCommand ??= new RelayCommand(() =>
            {
                TimingControlVMCollection.ForEach(x => x.IsEdit = true);


            });
        }


        private ICommand _finishEditTimingControlCommand;
        public ICommand FinishEditTimingControlCommand
        {
            get => _finishEditTimingControlCommand ??= new RelayCommand(() =>
            {
                TimingControlVMCollection.ForEach(x =>
                {
                    x.IsEdit = false;
                    x.IsDeleteButtonTrigger = false;
                });

            });
        }








        public ObservableCollection<int> CharactersCountCollection
        {
            get
            {
                var EnumList = new ObservableCollection<int>();
                EnumList.Add(1);
                EnumList.Add(2);
                EnumList.Add(3);
                EnumList.Add(4);
                EnumList.Add(5);
                EnumList.Add(6);
                EnumList.Add(7);


                return EnumList;
            }
        }


        private bool _isBottomDrawerOpen;
        public bool IsBottomDrawerOpen
        {
            get => _isBottomDrawerOpen; set
            {
                _isBottomDrawerOpen = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 往下拉
        /// </summary>
        private ICommand _onThumbMouseUpCommand;
        public ICommand OnThumb_DragCompletedCommand
        {
            get => _onThumbMouseUpCommand ??= new RelayCommand<object>(obj =>
            {/*
                if (IsThumbDragDeltaDown)
                        IsBottomDrawerOpen = false;*/
                if (obj is System.Windows.Controls.Primitives.DragCompletedEventArgs e)
                {
                    if (e.VerticalChange > 10)
                    {
                        IsBottomDrawerOpen = false;
                    }
                }
            });
        }




        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel NewTiming, true))
                {
                    TimingSetting = NewTiming;
                    TimingControlVMCollection = new ObservableCollection<TimingControlViewModel>(TimingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));
                    OnPropertyChanged(nameof(TimingControlVMCollection));
                }
            });
        }

        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                TimingSetting = new TimingSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                TimingSetting.TimingControlCollection = TimingControlVMCollection.Select(x => x.timingControlModel).ToList();
                if (await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, TimingSetting, true))
                {

                }
            });
        }

        public override ICommand DeleteSettingCommand => null;
    }

    public class TimingControlViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => "TimingControlViewModel";
        public TimingControlViewModel()
        {
            timingControlModel = new();
        }
        public TimingControlViewModel(TimingControlModel timingControl)
        {
            timingControlModel = timingControl;
        }

        [JsonIgnore]
        public readonly TimingControlModel timingControlModel;
        /// <summary>
        /// 休息時間
        /// </summary>
        public DateTime RestTime
        {
            get
            {
                var ts = timingControlModel.RestTime - timingControlModel.RestTime.Date;
                return new DateTime(ts.Ticks);
            }
            set 
            {
                timingControlModel.RestTime =value; 
                OnPropertyChanged(); 
            }
        }
        /// <summary>
        /// 開啟時間
        /// </summary>
        public DateTime OpenTime
        {
            get
            {
                var ts = timingControlModel.OpenTime - timingControlModel.OpenTime.Date;
                return new DateTime(ts.Ticks);
            }
            set 
            { 
                timingControlModel.OpenTime = value; OnPropertyChanged();
            } 
        }
        /// <summary>
        /// 已啟用
        /// </summary>
        public bool IsEnable { get => timingControlModel.IsEnable; set { timingControlModel.IsEnable = value; OnPropertyChanged(); } }



        private bool _dayOfWeekWorkVMObservableCollectionDropDownListlsShow;
        /// <summary>
        /// 顯示選單
        /// </summary>
        public bool DayOfWeekWorkVMObservableCollectionDropDownListlsShow
        {
            get => _dayOfWeekWorkVMObservableCollectionDropDownListlsShow; set { _dayOfWeekWorkVMObservableCollectionDropDownListlsShow = value; OnPropertyChanged(); }
        }


        private ObservableCollection<DayOfWeekWorkViewModel> _dayOfWeekWorkVMObservableCollection;
        public ObservableCollection<DayOfWeekWorkViewModel> DayOfWeekWorkVMObservableCollection
        {
            get
            {
                if (_dayOfWeekWorkVMObservableCollection == null)
                {
                    if (timingControlModel.DayOfWeekWorkCollection == null)
                    {
                        timingControlModel.DayOfWeekWorkCollection =new List<DayOfWeekWorkModel>(); for (int i = 0; i < 7; i++)
                        {
                            timingControlModel.DayOfWeekWorkCollection.Add(new DayOfWeekWorkModel((DayOfWeek)i, false));
                        }
                    }

                    _dayOfWeekWorkVMObservableCollection = timingControlModel.DayOfWeekWorkCollection.Select(x => new DayOfWeekWorkViewModel(x)).ToObservableCollection();
                    foreach (var obj in _dayOfWeekWorkVMObservableCollection)
                    {
                        obj.IsWorkChanged += (sender, e) =>
                        {
                            OnPropertyChanged(nameof(DayOfWeekWorkVMObservableCollection));
                        };
                    }
                }


                return _dayOfWeekWorkVMObservableCollection;
            }
            set
            {
                _dayOfWeekWorkVMObservableCollection = value;
                if (value != null)
                {
                    timingControlModel.DayOfWeekWorkCollection = value.Select(x => x.DayOfWeekWork).ToList();

                    foreach (var obj in value)
                    {
                        obj.IsWorkChanged += (sender, e) =>
                        {
                            OnPropertyChanged(nameof(DayOfWeekWorkVMObservableCollection));
                        };
                    }


                }
                OnPropertyChanged();
            }
        }



        private bool _isEdit;
        /// <summary>
        /// 彈出刪除按鈕
        /// </summary>
        public bool IsEdit { get => _isEdit; set { _isEdit = value; OnPropertyChanged(); } }



        private bool _isDeleteButtonTrigger;
        public bool IsDeleteButtonTrigger { get => _isDeleteButtonTrigger; set { _isDeleteButtonTrigger = value; OnPropertyChanged(); } }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get => _deleteCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                await Task.CompletedTask;

                if (obj is ItemsControl items)
                {
                    if (items.ItemsSource is ObservableCollection<GD_StampingMachine.ViewModels.ParameterSetting.TimingControlViewModel> collection)
                    {
                        if (collection.Contains(this))
                            collection.Remove(this);
                    }
                }

            });
        }


        private ICommand _onThumbDragDeltaCommand;
        public ICommand OnThumbDragDeltaCommand
        {
            get => _onThumbDragDeltaCommand ??= new RelayCommand<object>(obj =>
            {
                if (obj is System.Windows.Controls.Primitives.DragDeltaEventArgs e)
                {


                    if (e.HorizontalChange < 2)
                    {
                        mouseIsDown = false;
                        IsDeleteButtonTrigger = true;
                    }
                    else
                        IsDeleteButtonTrigger = false;
                    /* if (e.LeftButton == MouseButtonState.Pressed)
                     {
                         Thumb thumb = (Thumb)sender;
                         thumb.CaptureMouse();
                     }*/
                }


            });
        }
        private bool mouseIsDown = false;

        private ICommand _onThumbMouseDownCommand;
        public ICommand OnThumbMouseDownCommand
        {
            get => _onThumbMouseDownCommand ??= new RelayCommand<object>(obj =>
            {

                mouseIsDown = true;

                /*
                Thumb thumb = (Thumb)sender;
                double newX = Canvas.GetLeft(thumb) + e.HorizontalChange;
                double newY = Canvas.GetTop(thumb) + e.VerticalChange;

                Canvas.SetLeft(thumb, newX);
                Canvas.SetTop(thumb, newY);*/
            });
        }
        private ICommand _onThumbMouseUpCommand;
        public ICommand OnThumbMouseUpCommand
        {
            get => _onThumbMouseUpCommand ??= new RelayCommand<object>(obj =>
            {
                if (!IsEdit)
                {

                    if (mouseIsDown)
                    {
                        if (IsDeleteButtonTrigger)
                        {
                            IsDeleteButtonTrigger = false;
                        }
                        else
                        {
                            if (obj is ListBox listBox)
                            {
                                listBox.SelectedIndex = -1;
                                listBox.SelectedItem = this;
                            }
                        }
                    }
                }
                /*
                Thumb thumb = (Thumb)sender;
                double newX = Canvas.GetLeft(thumb) + e.HorizontalChange;
                double newY = Canvas.GetTop(thumb) + e.VerticalChange;

                Canvas.SetLeft(thumb, newX);
                Canvas.SetTop(thumb, newY);*/
            });
        }

        /// <summary>
        /// 確認某個時間段是否介於開始與休息之間
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool DateTimeIsBetween(DateTime dateTime)
        {
            DateTime RTime = this.RestTime;
            DateTime OTime;
            if(this.OpenTime >= this.RestTime)
            {
                OTime = this.OpenTime;
            }
            else
            {
                OTime = this.OpenTime.AddDays(1);
            }

            if (this.DayOfWeekWorkVMObservableCollection.Any(x=>x.IsWork && x.DayOfWeek == dateTime.DayOfWeek))
            {
                var time = new DateTime( dateTime.TimeOfDay.Ticks);
                if(RTime < time && time< OTime)
                {
                    return true;
                }
            }
            return false;
        }





    }


    public class DayOfWeekWorkViewModel : BaseViewModel
    {
        public override string ViewModelName => "DayOfWeekWorkViewModel";

        public DayOfWeekWorkViewModel()
        {
            DayOfWeekWork = new();
        }
        public DayOfWeekWorkViewModel(DayOfWeekWorkModel dayOfWeekWork)
        {
            DayOfWeekWork = dayOfWeekWork;
        }

        [JsonIgnore]
        public readonly DayOfWeekWorkModel DayOfWeekWork;

        public DayOfWeek DayOfWeek { get => DayOfWeekWork.DayOfWeek; set { DayOfWeekWork.DayOfWeek = value; OnPropertyChanged(); } }
        public bool IsWork
        {
            get => DayOfWeekWork.IsWork;
            set
            {
                bool oldValue = DayOfWeekWork.IsWork;

                if (oldValue != value)
                {
                    DayOfWeekWork.IsWork = value;
                    OnPropertyChanged();
                    OnIsWorkChanged(new GD_CommonLibrary.ValueChangedEventArgs<bool>(oldValue, value));
                }

            }
        }


        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<bool>> IsWorkChanged;

        protected virtual void OnIsWorkChanged(GD_CommonLibrary.ValueChangedEventArgs<bool> e)
        {
            IsWorkChanged?.Invoke(this, e);
        }


    }





}
