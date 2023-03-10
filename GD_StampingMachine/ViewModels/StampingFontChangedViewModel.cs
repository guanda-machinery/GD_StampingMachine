using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using JetBrains.Annotations;
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

namespace GD_StampingMachine.ViewModels
{
    public class StampingFontChangedViewModel : ViewModelBase
    {


        private ObservableCollection<StampingTypeModel> _stampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeModel> StampingTypeVMObservableCollection
        {
            get
            {
                if (_stampingTypeVMObservableCollection == null)
                    _stampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>();
                return _stampingTypeVMObservableCollection;
            }
            set
            {
                _stampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(StampingTypeVMObservableCollection));
            }
        }


        private ObservableCollection<StampingTypeModel> _unusedStampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeModel> UnusedStampingTypeVMObservableCollection
        {
            get
            {
                if (_unusedStampingTypeVMObservableCollection == null)
                    _unusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>();
                return _unusedStampingTypeVMObservableCollection;
            }
            set
            {
                _unusedStampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(UnusedStampingTypeVMObservableCollection));
            }
        }
        /// <summary>
        /// 鋼印機上的字模
        /// </summary>
        public StampingTypeModel StampingFontSelected { get; set; }
        /// <summary>
        /// 被新建出來還沒放上去的字模/被換下來的字模
        /// </summary>
        public StampingTypeModel UnusedStampingFontSelected { get; set; }

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

            StampingTypeVMObservableCollection[ST_index] = new StampingTypeModel()
            {
                StampingTypeNumber = FontStringNumber,
                StampingTypeString = UnusedFontString,
                StampingTypeUseCount = UnusedFontStringUseCount
            };

            UnusedStampingTypeVMObservableCollection[UST_index] = new StampingTypeModel()
            {
                StampingTypeNumber = UnusedFontStringNumber,
                StampingTypeString = FontString,
                StampingTypeUseCount = FontStringUseCount
            };
        }

        private ObservableCollection<StampingTypeModel> _newUnusedStampingFont;
        /// <summary>
        /// 新增字模
        /// </summary>
        public ObservableCollection<StampingTypeModel> NewUnusedStampingFont
        {
            get
            {
                if (_newUnusedStampingFont == null)
                {
                    _newUnusedStampingFont = new ObservableCollection<StampingTypeModel>();
                }

                if (_newUnusedStampingFont.Count == 0)
                {
                    _newUnusedStampingFont.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeUseCount = 0,
                        StampingTypeString = null,
                        IsNewAddStamping = true,
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

        //private string _newUnnsedStampingFontString;
        /*public string NewUnnsedStampingFontString
        {
            get
            {
                return  NewUnusedStampingFont.FirstOrDefault().StampingTypeString;
            }
            set 
            {
                NewUnusedStampingFont.FirstOrDefault().StampingTypeString = value; 
                OnPropertyChanged(nameof(NewUnnsedStampingFontString));
              }
        }*/
        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                var FirstFont = NewUnusedStampingFont.FirstOrDefault().Clone() as StampingTypeModel;
                FirstFont.IsNewAddStamping = false;
                UnusedStampingTypeVMObservableCollection.Add(FirstFont);
                /*
                if (!string.IsNullOrEmpty(NewUnnsedStampingFontString))
                {
                    UnusedStampingTypeVMObservableCollection.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeString = NewUnnsedStampingFontString,
                        StampingTypeUseCount = 0
                    });
                }*/
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

        public StampingTypeDropTarget DragStampingTypeDropTarget { get; set; } = new StampingTypeDropTarget();

    }
}
