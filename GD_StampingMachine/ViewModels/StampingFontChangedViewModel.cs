using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GD_CommonLibrary;

namespace GD_StampingMachine.ViewModels
{
    public class StampingFontChangedViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingFontChangedViewModel");

        private ObservableCollection<StampingTypeViewModel> _stampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeViewModel> StampingTypeVMObservableCollection
        {
            get
            {
                if (_stampingTypeVMObservableCollection == null)
                    _stampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>();
                return _stampingTypeVMObservableCollection;
            }
            set
            {
                _stampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(StampingTypeVMObservableCollection));
            }
        }


        private ObservableCollection<StampingTypeViewModel> _unusedStampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeViewModel> UnusedStampingTypeVMObservableCollection
        {
            get
            {
                if (_unusedStampingTypeVMObservableCollection == null)
                    _unusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>();
                return _unusedStampingTypeVMObservableCollection;
            }
            set
            {
                _unusedStampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(UnusedStampingTypeVMObservableCollection));
            }
        }


        private StampingTypeViewModel _stampingFontSelected;
        /// <summary>
        /// 鋼印機上的字模
        /// </summary>
        public StampingTypeViewModel StampingFontSelected
        {
            get
            {
                return _stampingFontSelected;
            }
            set
            {
                _stampingFontSelected = value;
                OnPropertyChanged(nameof(StampingFontSelected));
            }
        }
        /// <summary>
        /// 被新建出來還沒放上去的字模/被換下來的字模
        /// </summary>
        public StampingTypeViewModel UnusedStampingFontSelected { get; set; }

        public RelayCommand StampingFontReplaceCommand
        {
            get => new RelayCommand(() =>
            {
                if (StampingFontSelected != null && UnusedStampingFontSelected != null)
                {
                    StampingTypeModelExchanged();
                }
            });
        }

        private void StampingTypeModelExchanged()
        {
            var FontString = StampingFontSelected.StampingTypeString;
            var FontStringNumber = StampingFontSelected.StampingTypeNumber;
            var FontStringUseCount = StampingFontSelected.StampingTypeUseCount;

            var UnusedFontString = UnusedStampingFontSelected.StampingTypeString;
            var UnusedFontStringNumber = UnusedStampingFontSelected.StampingTypeNumber;
            var UnusedFontStringUseCount = UnusedStampingFontSelected.StampingTypeUseCount;

            var ST_index = StampingTypeVMObservableCollection.FindIndex(x => x == StampingFontSelected);
            var UST_index = UnusedStampingTypeVMObservableCollection.FindIndex(x => x == UnusedStampingFontSelected);

            StampingTypeVMObservableCollection[ST_index] = new StampingTypeViewModel()
            {
                StampingTypeNumber = FontStringNumber,
                StampingTypeString = UnusedFontString,
                StampingTypeUseCount = UnusedFontStringUseCount
            };  

            UnusedStampingTypeVMObservableCollection[UST_index] = new StampingTypeViewModel()
            {
                StampingTypeNumber = UnusedFontStringNumber,
                StampingTypeString = FontString,
                StampingTypeUseCount = FontStringUseCount
            };
        }

        private ObservableCollection<StampingTypeViewModel> _newUnusedStampingFont;
        /// <summary>
        /// 新增字模
        /// </summary>
        public ObservableCollection<StampingTypeViewModel> NewUnusedStampingFont
        {
            get
            {
                if (_newUnusedStampingFont == null)
                {
                    _newUnusedStampingFont = new ObservableCollection<StampingTypeViewModel>();
                }

                if (_newUnusedStampingFont.Count == 0)
                {
                    _newUnusedStampingFont.Add(new StampingTypeViewModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeUseCount = 0,
                        StampingTypeString = null,
                        IsNewAddStamping = true,
                        //LogDataObservableCollection = this.LogDataObservableCollection
                    });
                };

                return _newUnusedStampingFont;
            }
            set
            {
                _newUnusedStampingFont = value;
                OnPropertyChanged(nameof(NewUnusedStampingFont));
            }
        }


        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                var FirstFont = NewUnusedStampingFont.FirstOrDefault().DeepCloneByJson();
                FirstFont.IsNewAddStamping = false;
                UnusedStampingTypeVMObservableCollection.Add(FirstFont);
            });
        }

        public RelayCommand UnusedStampingFontDelCommand
        {
            get => new RelayCommand(() =>
            {
                if (UnusedStampingFontSelected != null)
                {
                    //下列配合DragDrop會導致介面異常 已停用
                    UnusedStampingTypeVMObservableCollection.Remove(UnusedStampingFontSelected);




                }

            });
        }



        private double _stampingFontTurntable_RorateAngle;
        public double StampingFontTurntable_RorateAngle
        {
            get => _stampingFontTurntable_RorateAngle;
            set
            {
                _stampingFontTurntable_RorateAngle = value;
                OnPropertyChanged(nameof(StampingFontTurntable_RorateAngle));
            }
        }

        private StampingTypeModelMartixViewModel _stampingTypeModelMartix= new();

        public StampingTypeModelMartixViewModel StampingTypeModelMartix
        {
            get => _stampingTypeModelMartix; 
            set
            {
                _stampingTypeModelMartix = value;
                OnPropertyChanged(nameof(StampingTypeModelMartix));
            }
        }


        private readonly object balanceLock = new object();
        private bool StampingTypeModel_ReadyStamping_IsRotating = false;
     


        private SweepDirection? _direction;

        public SweepDirection? Direction
        {
            get=> _direction; 
            set 
            { 
                _direction = value;
                OnPropertyChanged(); 
            }
        }

        private StampingTypeViewModel _stampingTypeModel_readyStamping;
        public StampingTypeViewModel StampingTypeModel_ReadyStamping
        {
            get
            {
                if (_stampingTypeModel_readyStamping == null)
                {
                    if (_stampingTypeVMObservableCollection.Count != 0)
                    {
                        _stampingTypeModel_readyStamping = _stampingTypeVMObservableCollection.FirstOrDefault();
                    }
                }
                var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                if (FIndex != -1)
                {
                    StampingTypeModelMartix = new ();
                    //離原點差距的角度-以逆時針計算

                    double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;
                    
                    int ClockDirection = 1;
                    if (Direction == null)
                    {
                        int ReverseInt = 1;
                        if(Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle)>180)
                        {
                            ReverseInt = -1;
                        }

                        if (TargetAngle - StampingFontTurntable_RorateAngle>0)
                            ClockDirection = 1* ReverseInt;
                        else
                            ClockDirection = -1* ReverseInt;
                    }
                    if (Direction == SweepDirection.Clockwise)
                    {
                        ClockDirection = -1;
                    }
                    if (Direction == SweepDirection.Counterclockwise)
                    {
                        ClockDirection = 1;
                    }

                    //延遲旋轉
                    Task.Run(() =>
                    {
                        if (StampingTypeModel_ReadyStamping_IsRotating)
                        {
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }
                        try
                        {
                            StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                            StampingTypeModelMartix.BottomStampingTypeModel = StampingTypeVMObservableCollection[FIndex];
                            double RotateGap = StampingTypeVMObservableCollection.Count / 4;



                            var LeftIndex = FIndex + RotateGap * 1;
                            while (LeftIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                LeftIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.LeftStampingTypeModel = StampingTypeVMObservableCollection[(int)LeftIndex];

                            var TopIndex = FIndex + RotateGap * 2;
                            while (TopIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                TopIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.TopStampingTypeModel = StampingTypeVMObservableCollection[(int)TopIndex];

                            var RightIndex = FIndex + RotateGap * 3;
                            while (RightIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                RightIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.RightStampingTypeModel = StampingTypeVMObservableCollection[(int)RightIndex];


                        }
                        catch (Exception ex)
                        {

                        }

                        //先取得目前角度->方向逆時針
                        lock (balanceLock)
                        {
                            //var CurrentAngle = StampingFontTurntable_RorateAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = true;

                            //StampingFontTurntable_RorateAngle = TargetAngle;
                            while (StampingTypeModel_ReadyStamping_IsRotating)
                            {
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }


                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2 ||
                                    Math.Abs(TargetAngle+360 - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }
                                StampingFontTurntable_RorateAngle += ClockDirection* 0.5;
                                System.Threading.Thread.Sleep(2);
                            }
                           StampingFontTurntable_RorateAngle = TargetAngle;
                           

                            StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                            StampingTypeVMObservableCollection[FIndex].StampingIsUsing = true;
                            StampingTypeModel_ReadyStamping_IsRotating = false;

                            //找出上下左右四個格子的方塊
                            //下方



                        }

                    });
                }

                return _stampingTypeModel_readyStamping;
            }
            set
            {
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged(nameof(StampingTypeModel_ReadyStamping));
            }
        }



        /*動起來很好看 所以保留在這裡*/
        /*private bool StampingTypeModel_ReadyStamping_IsRotating = false;
        private StampingTypeModel _stampingTypeModel_readyStamping;
        public StampingTypeModel StampingTypeModel_ReadyStamping
        {
            get => _stampingTypeModel_readyStamping;
            set
            {
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged(nameof(StampingTypeModel_ReadyStamping));
                var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                if (FIndex != -1)
                {
                    //離原點差距的角度-以逆時針計算
                    double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;

                    var AngleGap = Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle);
                    if (TargetAngle > StampingFontTurntable_RorateAngle)
                    {
                        AngleGap += 360;
                    }
                    int SleepTime = 0;
                    if (AngleGap != 0)
                    {
                        SleepTime = Convert.ToInt32(360 / AngleGap);
                        if (SleepTime >= 5)
                            SleepTime = 5;
                    }

                    //延遲旋轉
                    Task.Run(() =>
                    {
                        if (StampingTypeModel_ReadyStamping_IsRotating)
                        {
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                        //先取得目前角度->方向逆時針
                        lock (balanceLock)
                        {
                            var CurrentAngle = StampingFontTurntable_RorateAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = true;
                            while (StampingTypeModel_ReadyStamping_IsRotating)
                            {
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                //以0.1度進行逆時針旋轉  超過360度需要處理?

                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }
                                StampingFontTurntable_RorateAngle -= 0.5;
                                System.Threading.Thread.Sleep(SleepTime);
                            }
                            StampingFontTurntable_RorateAngle = TargetAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                    });
                }
            }
        }*/






        /// <summary>
        /// DropDarg
        /// </summary>
        public StampingTypeDropTarget DragStampingTypeDropTarget { get; set; } = new StampingTypeDropTarget();

    }

    public class StampingTypeModelMartixViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingFontChangedViewModel");
        private StampingTypeViewModel _bottomStampingTypeModel;
        private StampingTypeViewModel _rightStampingTypeModel;
        private StampingTypeViewModel _topStampingTypeModel;
        private StampingTypeViewModel _leftStampingTypeModel;

        public StampingTypeViewModel BottomStampingTypeModel
        {
            get=> _bottomStampingTypeModel;
            set
            {
                _bottomStampingTypeModel = value;
                OnPropertyChanged(nameof(BottomStampingTypeModel));
            }
        }
        public StampingTypeViewModel RightStampingTypeModel
        {
            get => _rightStampingTypeModel;
            set
            {
                _rightStampingTypeModel = value;
                OnPropertyChanged(nameof(RightStampingTypeModel));
            }
        }
        public StampingTypeViewModel TopStampingTypeModel
        {
            get => _topStampingTypeModel;
            set
            {
                _topStampingTypeModel = value;
                OnPropertyChanged(nameof(TopStampingTypeModel));
            }
        }
        public StampingTypeViewModel LeftStampingTypeModel
        {
            get => _leftStampingTypeModel; set
            {
                _leftStampingTypeModel = value;
                OnPropertyChanged(nameof(LeftStampingTypeModel));
            }
        }
    }



}
