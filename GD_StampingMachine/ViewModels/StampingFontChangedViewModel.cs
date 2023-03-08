using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
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



        private string _newUnnsedStampingFontString;
        public string NewUnnsedStampingFontString
        {
            get { return _newUnnsedStampingFontString; }
            set 
            {
                _newUnnsedStampingFontString = value; 
                OnPropertyChanged(nameof(NewUnnsedStampingFontString));
              }
        }
        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(NewUnnsedStampingFontString))
                {
                    UnusedStampingTypeVMObservableCollection.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeString = NewUnnsedStampingFontString,
                        StampingTypeUseCount = 0
                    });
                }
            });
        }

        public RelayCommand UnusedStampingFontDelCommand
        {
            get => new RelayCommand(() =>
            {
                if (UnusedStampingFontSelected != null)
                {
                    //下列配合DragDrop會導致介面異常 已停用
                    //UnusedStampingTypeVMObservableCollection.Remove(UnusedStampingFontSelected);




                }

            });
        }



        public override void Drop(IDropInfo dropInfo)
        {
            
            var targetList = dropInfo.TargetCollection.TryGetList();
            var data = dropInfo.DragInfo.Data;
            var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();

            if (targetList != null && sourceList != null)
            {
                if (targetList.Count > 0 && sourceList.Count > 0)
                {
                    var SIndex = dropInfo.DragInfo.SourceIndex;
                    var TIndex = dropInfo.InsertIndex;
                    if(TIndex >= targetList.Count)
                    {
                        TIndex--;
                    }
                    if (TIndex != -1)
                    {
                        //若選擇的目標index會掉出邊界 則-1
                        var TargetOldData = targetList[TIndex];
                        //符合資料結構 可互換值
                        if (dropInfo.Data is StampingTypeModel && TargetOldData is StampingTypeModel)
                        {
                            //取出
                            var SourceTypeNumber = (dropInfo.Data as StampingTypeModel).StampingTypeNumber;
                            var TargetTypeNumber = (TargetOldData as StampingTypeModel).StampingTypeNumber;

                            (dropInfo.Data as StampingTypeModel).StampingTypeNumber = TargetTypeNumber;
                            (TargetOldData as StampingTypeModel).StampingTypeNumber = SourceTypeNumber;

                         //   if (StampingFontSelected != null && UnusedStampingFontSelected != null)



                        }
                        sourceList.RemoveAt(SIndex);
                        sourceList.Insert(SIndex, TargetOldData);
                        targetList.RemoveAt(TIndex);
                        targetList.Insert(TIndex, data);
                    }
                }
                else
                {
                    //移動不互換
                    GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                }
            }

            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView)
            {
                ((ICollectionView)dropInfo.TargetCollection).Refresh();
            }

            var isSameCollection = sourceList.IsSameObservableCollection(targetList);
            if (!isSameCollection)
            {
                if (dropInfo.DragInfo.SourceCollection is ICollectionView)
                {
                    ((ICollectionView)dropInfo.DragInfo.SourceCollection).Refresh();
                }
            }

        }

    }
}
