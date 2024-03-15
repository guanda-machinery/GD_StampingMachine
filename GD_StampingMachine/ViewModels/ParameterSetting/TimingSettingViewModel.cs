using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using GD_CommonControlLibrary;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ParameterSetting;
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
      readonly  StampingMachineJsonHelper JsonHM = new();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TimingSettingViewModel");

        public TimingSettingViewModel(TimingSettingModel timingSetting)
        {
            this.TimingSetting = timingSetting;
            TimingControlVMCollection = new ObservableCollection<TimingControlViewModel>(timingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));
            TimingControlVMCollectionCloned = TimingControlVMCollection.DeepCloneByJson();
        }

        //private TimingSettingModel? _timingSetting;
        [JsonIgnore]
        public TimingSettingModel TimingSetting { get; private set; }


        private ICommand?_pageUnloadedCommand;
        public ICommand PageUnloadedCommand
        {
            get => _pageUnloadedCommand ??= new AsyncRelayCommand<object>(async obj =>
            {
                //詢問是否要儲存
                //比較兩者
                bool isSame = TimingControlVMCollection.Count == TimingControlVMCollectionCloned.Count &&
                               TimingControlVMCollection
                                   .Zip(TimingControlVMCollectionCloned, (a, b) => a.TimingControl.IsEquals(b.TimingControl))
                                   .All(x => x);

                if (!isSame)
                {
                    if (await MethodWinUIMessageBox.AskSaveChangeOrNotAsync())
                    {
                        await SaveSettingAsync(false);
                    }
                    else
                    {
                        TimingControlVMCollectionCloned = TimingControlVMCollection.DeepCloneByJson();
                    }
                }
            });
        }


        private ICommand?_addNewTimingControlCommand;
        public ICommand AddNewTimingControlCommand
        {
            get => _addNewTimingControlCommand ??= new RelayCommand(() =>
            {
                TimingControlVMSelectedClone = new TimingControlViewModel();
                TimeSettingPopupIsOpen = true;
                IsNewTimingControl = true;
            });
        }
        private ICommand?_saveNewTimingControlCommand;
        public ICommand SaveNewTimingControlCommand
        {
            get => _saveNewTimingControlCommand ??= new RelayCommand(() =>
            {
                var newTC = TimingControlVMSelectedClone.DeepCloneByJson();
                newTC.IsEdit = EditButtonIsChecked;
                TimingControlVMCollectionCloned.Add(newTC);

                OnPropertyChanged(nameof(TimingControlVMCollectionCloned));
                TimingControlVMSelectedClone = new TimingControlViewModel();
                TimeSettingPopupIsOpen = false;
            });
        }

        private bool _isNewTimingControl;
        public bool IsNewTimingControl
        { get => _isNewTimingControl; set { _isNewTimingControl = value; OnPropertyChanged(); } }



        private ObservableCollection<TimingControlViewModel>? _timingControlVMCollection;
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
                    TimingSetting.TimingControlCollection = _timingControlVMCollection.Select(x => x.TimingControl).ToList();
                }
                return _timingControlVMCollection;
            }
            set
            {
                _timingControlVMCollection = value;
                TimingSetting.TimingControlCollection = value.Select(x => x.TimingControl).ToList();
                OnPropertyChanged();
            }
        }


        private ObservableCollection<TimingControlViewModel>? _timingControlVMCollectionCloned;
        public ObservableCollection<TimingControlViewModel> TimingControlVMCollectionCloned
        {
            get => _timingControlVMCollectionCloned??= new();
            set
            {
                _timingControlVMCollectionCloned = value; 
                OnPropertyChanged();
            }
        }







        private TimingControlViewModel? _timingControlVMSelected;
        public TimingControlViewModel TimingControlVMSelected
        {
            get => _timingControlVMSelected ??= new();
            set
            {
                _timingControlVMSelected = value;
                OnPropertyChanged();
            }
        }

        private TimingControlViewModel? _timingControlVMSelectedClone;
        public TimingControlViewModel? TimingControlVMSelectedClone
        {
            get => _timingControlVMSelectedClone;
            set
            {
                _timingControlVMSelectedClone = value; OnPropertyChanged();
            }
        }

        private bool IsMouseInListBox;

        private ICommand?_timingControlVMCollectionListBox_MouseEnterCommand;
        public ICommand TimingControlVMCollectionListBox_MouseEnterCommand
        {
            get => _timingControlVMCollectionListBox_MouseEnterCommand ??= new RelayCommand<object>(obj =>
            {
                IsMouseInListBox = true;
            });
        }
        private ICommand?_timingControlVMCollectionListBox_MouseLeaveCommand;
        public ICommand TimingControlVMCollectionListBox_MouseLeaveCommand
        {
            get => _timingControlVMCollectionListBox_MouseLeaveCommand ??= new RelayCommand<object>(obj =>
            {
                IsMouseInListBox = false;
            });
        }



        private ICommand?_timingControlVMCollectionListBox_MouseUpCommand;
        public ICommand TimingControlVMCollectionListBox_MouseUpCommand
        {
            get => _timingControlVMCollectionListBox_MouseUpCommand ??= new RelayCommand<object>(obj =>
            {
               TimingControlVMSelectedClone = TimingControlVMSelected.DeepCloneByJson();
                if (obj is System.Windows.Input.MouseButtonEventArgs e)
                {
                    IsNewTimingControl = false;
                    TimeSettingPopupIsOpen = true;
                }
            }, obj=>IsMouseInListBox);
        }

        /*
          private ICommand?_timingControlVMCollectionListBox_SelectionChangedCommand;
           public ICommand TimingControlVMCollectionListBox_SelectionChangedCommand
           {
               get => _timingControlVMCollectionListBox_SelectionChangedCommand ??= new RelayCommand<object>(obj =>
               {
                   if (obj is SelectionChangedEventArgs e)
                   {
                       IsNewTimingControl = false;
                       if (e.AddedItems.Count > 0)
                       {
                           //TimeSettingPopupIsOpen = true;
                       }
                       else
                       {
                           TimeSettingPopupIsOpen = false;
                       }
                   }
               });
           }*/

        private ICommand?_finishTimingControlChangedCommand;
        public ICommand FinishTimingControlChangedCommand
        {
            get => _finishTimingControlChangedCommand ??= new RelayCommand<object>(obj =>
            {
                var index = TimingControlVMCollectionCloned.IndexOf(TimingControlVMSelected);
                if (index != -1)
                    TimingControlVMCollectionCloned[index] = TimingControlVMSelectedClone;
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


        private ICommand?_editTimingControlCommand;
        public ICommand EditTimingControlCommand
        {
            get => _editTimingControlCommand ??= new RelayCommand(() =>
            {
                TimingControlVMCollectionCloned.ForEach(x => x.IsEdit = true);


            });
        }


        private ICommand?_finishEditTimingControlCommand;
        public ICommand FinishEditTimingControlCommand
        {
            get => _finishEditTimingControlCommand ??= new RelayCommand(() =>
            {
                TimingControlVMCollectionCloned.ForEach(x =>
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
                var EnumList = new ObservableCollection<int>
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7
                };


                return EnumList;
            }
        }


        private bool _timeSettingPopupIsOpen;
        public bool TimeSettingPopupIsOpen
        {
            get => _timeSettingPopupIsOpen; set
            {
                _timeSettingPopupIsOpen = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 往下拉
        /// </summary>
        private ICommand?_onThumbMouseUpCommand;
        public ICommand OnThumb_DragCompletedCommand
        {
            get => _onThumbMouseUpCommand ??= new RelayCommand<object>(obj =>
            {/*
                if (IsThumbDragDeltaDown)
                        TimeSettingPopupIsOpen = false;*/
                if (obj is System.Windows.Controls.Primitives.DragCompletedEventArgs e)
                {
                    if (e.VerticalChange > 10)
                    {
                        TimeSettingPopupIsOpen = false;
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
                    TimingControlVMCollectionCloned = TimingControlVMCollection.DeepCloneByJson();
                    OnPropertyChanged(nameof(TimingControlVMCollection));
                    OnPropertyChanged(nameof(TimingControlVMCollectionCloned));
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
               await SaveSettingAsync(true);
            });
        }
        private async Task SaveSettingAsync(bool showMessageBox)
        {
            TimingControlVMCollection = TimingControlVMCollectionCloned.DeepCloneByJson();
            TimingSetting.TimingControlCollection = TimingControlVMCollectionCloned.Select(x => x.TimingControl).ToList();
            if (await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, TimingSetting, showMessageBox))
            {

            }
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }

    public class TimingControlViewModel : GD_CommonControlLibrary.BaseViewModel
    {
        public override string ViewModelName => "TimingControlViewModel";
        public TimingControlViewModel()
        {
            TimingControl = new();
        }
        public TimingControlViewModel(TimingControlModel timingControl)
        {
            TimingControl = timingControl;
        }

        [JsonIgnore]
        public readonly TimingControlModel TimingControl;
        /// <summary>
        /// 休息時間
        /// </summary>
        public DateTime RestTime
        {
            get
            {
                return new DateTime(TimingControl.RestTime.TimeOfDay.Ticks);
            }
            set 
            {
                TimingControl.RestTime =value; 
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
                return new DateTime(TimingControl.OpenTime.TimeOfDay.Ticks);
            }
            set 
            { 
                TimingControl.OpenTime = value; OnPropertyChanged();
            } 
        }
        /// <summary>
        /// 已啟用
        /// </summary>
        public bool IsEnable { get => TimingControl.IsEnable; set { TimingControl.IsEnable = value; OnPropertyChanged(); } }



        private bool _isDayOfWeekWorkVMObservableCollectionDropDownListShow;
        /// <summary>
        /// 顯示選單
        /// </summary>
        public bool IsDayOfWeekWorkVMObservableCollectionDropDownListShow
        {
            get => _isDayOfWeekWorkVMObservableCollectionDropDownListShow; set { _isDayOfWeekWorkVMObservableCollectionDropDownListShow = value; OnPropertyChanged(); }
        }


        private ObservableCollection<DayOfWeekWorkViewModel>? _dayOfWeekWorkVMObservableCollection;
        public ObservableCollection<DayOfWeekWorkViewModel> DayOfWeekWorkVMObservableCollection
        {
            get
            {
                if (_dayOfWeekWorkVMObservableCollection == null)
                {
                    if (TimingControl.DayOfWeekWorkCollection == null)
                    {
                        TimingControl.DayOfWeekWorkCollection =new List<DayOfWeekWorkModel>(); for (int i = 0; i < 7; i++)
                        {
                            TimingControl.DayOfWeekWorkCollection.Add(new DayOfWeekWorkModel((DayOfWeek)i, false));
                        }
                    }

                    _dayOfWeekWorkVMObservableCollection = TimingControl.DayOfWeekWorkCollection.Select(x => new DayOfWeekWorkViewModel(x)).ToObservableCollection();
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
                    TimingControl.DayOfWeekWorkCollection = value.Select(x => x.DayOfWeekWork).ToList();

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

        private ICommand?_deleteCommand;
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


        private ICommand?_onThumbDragDeltaCommand;
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
                }


            });
        }
        private bool mouseIsDown = false;

        private ICommand?_onThumbMouseDownCommand;
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
        private ICommand?_onThumbMouseUpCommand;
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
        /// 兩個時間點在同一天
        /// </summary>
        public bool SameDay=>
            this.OpenTime >= this.RestTime;
        

        /// <summary>
        /// 確認某個時間段是否介於開始與休息之間
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool DateTimeIsBetween(DateTime dateTime)
        {
            DateTime RTime = this.RestTime;
            //flag = true代表在同一天 =false代表跨日
            bool flag = SameDay;
            DateTime OTime = flag ? this.OpenTime : this.OpenTime.AddDays(1);

            var dayValue = (int)(flag ? dateTime.DayOfWeek : dateTime.DayOfWeek - 1);
            var day = (DayOfWeek)((dayValue + 7) % 7);

            if (this.DayOfWeekWorkVMObservableCollection.Any(x => x.IsWork && x.DayOfWeek == day))
            {
                var time = new DateTime(dateTime.TimeOfDay.Ticks);
                if (!flag)
                    time = time.AddDays(1);

                if (RTime < time && time < OTime)
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


        public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<bool>>? IsWorkChanged;

        protected virtual void OnIsWorkChanged(GD_CommonLibrary.ValueChangedEventArgs<bool> e)
        {
            IsWorkChanged?.Invoke(this, e);
        }


    }





}
