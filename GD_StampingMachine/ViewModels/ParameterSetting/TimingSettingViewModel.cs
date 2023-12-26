using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_StampingMachine.Method;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;
using DevExpress.Mvvm.Native;
using System.Diagnostics;
using System.Windows.Controls;
using GD_CommonLibrary.Extensions;

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

            if (Debugger.IsAttached)
            {
                TimingControlVMCollection.Add(new TimingControlViewModel());
                TimingControlVMCollection.Add(new TimingControlViewModel());
                TimingControlVMCollection.Add(new TimingControlViewModel());

            }
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
                TimingControlVMCollection.Add(TimingControlVMSelectedClone.DeepCloneByJson());
                TimingControlVMSelectedClone = new TimingControlViewModel();
                IsBottomDrawerOpen = false;
            });
        }

        private bool _isNewTimingControl;
        public bool IsNewTimingControl
        { get => _isNewTimingControl; set { _isNewTimingControl = value;OnPropertyChanged(); } }




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
                _timingControlVMSelectedClone = value;OnPropertyChanged();
            }
        }

        private ICommand _timingControlVMCollectionListBox_SelectionChangedlCommand;
        public ICommand TimingControlVMCollectionListBox_SelectionChangedlCommand
        {
            get => _timingControlVMCollectionListBox_SelectionChangedlCommand ??= new RelayCommand<object>(obj =>
            {
                if(obj is SelectionChangedEventArgs e)
                {
                    IsNewTimingControl = false;
                    if (e.AddedItems.Count>0)
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
                _isBottomDrawerOpen= value;
                OnPropertyChanged(); }
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
                if(obj is System.Windows.Controls.Primitives.DragCompletedEventArgs e)
                {
                  if  (e.VerticalChange > 10)
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
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel NewTiming ,true))
                {
                    TimingSetting = NewTiming;
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
                if (await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, TimingSetting, true))
                {

                }
            });
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }
      
    public  class TimingControlViewModel : GD_CommonLibrary.BaseViewModel
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
        public DateTime RestTime { get => timingControlModel.RestTime; set { timingControlModel.RestTime = value; OnPropertyChanged(); } }
        /// <summary>
        /// 開啟時間
        /// </summary>
        public DateTime OpenTime { get => timingControlModel.OpenTime; set { timingControlModel.OpenTime = value; OnPropertyChanged(); } }
        /// <summary>
        /// 已啟用
        /// </summary>
        public bool IsEnable { get=> timingControlModel.IsEnable; set { timingControlModel.IsEnable = value;OnPropertyChanged(); } }



        private ObservableCollection<DayOfWeekWorkViewModel> _dayOfWeekWorkVMObservableCollection;
        public ObservableCollection<DayOfWeekWorkViewModel> DayOfWeekWorkVMObservableCollection
        {
            get
            {
                if(_dayOfWeekWorkVMObservableCollection== null)
                {
                    _dayOfWeekWorkVMObservableCollection = timingControlModel.DayOfWeekWorkCollection.Select(x => new DayOfWeekWorkViewModel(x)).ToObservableCollection();
                }


                return _dayOfWeekWorkVMObservableCollection;
            }
            set
            {
                _dayOfWeekWorkVMObservableCollection = value;
                if (value != null)
                {
                    timingControlModel.DayOfWeekWorkCollection = value.Select(x => x.DayOfWeekWork).ToList();
                }
                OnPropertyChanged(); } 
        }

        private bool _isEdit;
        /// <summary>
        /// 彈出刪除按鈕
        /// </summary>
        public bool IsEdit { get => _isEdit; set { _isEdit = value; OnPropertyChanged(); } }



        private bool _isDeleteButtonTrigger;
        public bool IsDeleteButtonTrigger { get=> _isDeleteButtonTrigger; set { _isDeleteButtonTrigger = value;OnPropertyChanged(); } }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand 
        {
            get => _deleteCommand ??= new AsyncRelayCommand<object>(async obj=> 
            {
                await Task.CompletedTask;

                if(obj is ObservableCollection<GD_StampingMachine.ViewModels.ParameterSetting.TimingControlViewModel> collection)
                {
                    if(collection.Contains(this))
                        collection.Remove(this);
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

                    mouseIsDown = false;

                    if (e.HorizontalChange < 0)
                    {
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
               // this.IsWorkChanged?.Invoke(this, new GD_CommonLibrary.ValueChangedEventArgs<bool>(DayOfWeekWork.IsWork, value));
                DayOfWeekWork.IsWork = value; 
                OnPropertyChanged();
            }
        }


        //public event EventHandler<GD_CommonLibrary.ValueChangedEventArgs<bool>> IsWorkChanged;



    }





}
