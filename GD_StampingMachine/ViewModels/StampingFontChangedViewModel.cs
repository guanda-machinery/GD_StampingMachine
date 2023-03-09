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

        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)    // 同item時不互換
            {
                bool IsPreFont = false;
                //除非是未上鋼印機的預備區字模 否則不容許自己互換
                if (dropInfo.DragInfo.Data is StampingTypeModel SourceData)
                {
                    if (SourceData.StampingTypeNumber != 0 || SourceData.IsNewAddStamping == true)
                    {
                        dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
                        return;
                    }
                }
            }
            else
            {
                if (dropInfo.DragInfo.Data is StampingTypeModel SourceData && dropInfo.TargetItem is StampingTypeModel TargetData )
                {
                    //若源頭是新增字模 且目標是鋼印機的字模 則不可以放上去
                    if (SourceData.IsNewAddStamping == true && TargetData.StampingTypeNumber > 0)
                    {
                        dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
                        return;
                    }

                    //目標是新增字模不可以放上去
                    if (TargetData.IsNewAddStamping)
                    {
                        dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
                        return;
                    }
                }



            }



            dropInfo.DropTargetAdorner = typeof(DropTargetHighlightAdorner);
            dropInfo.Effects = DragDropEffects.Move;
        }

        private enum DragDropMethod
        {
            None,
            /// <summary>
            /// 移動
            /// </summary>
            Move,
            /// <summary>
            /// 複製
            /// </summary>
            Copy,
            /// <summary>
            /// 交換
            /// </summary>
            Exchange,
            /// <summary>
            /// 刪除
            /// </summary>
            Delete,
        }


        public override void Drop(IDropInfo dropInfo)
        {
            var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
            var targetList = dropInfo.TargetCollection.TryGetList();
            var SourceData = dropInfo.Data;
            var TargetData = dropInfo.TargetItem;


            if (SourceData == null)
            {
                //沒資料不須處理
                return;
            }

            if (targetList != null && sourceList != null)
            {
                var DDMethodType = DragDropMethod.Exchange;
                //符合資料結構 可進行互換值
                if (dropInfo.Data is StampingTypeModel)//&& dropInfo.TargetItem is StampingTypeModel)
                {
                    //新字模區不能放東西進來
                    if ((TargetData as StampingTypeModel)?.IsNewAddStamping == true)
                    {
                        DDMethodType = DragDropMethod.None;
                        return;
                    }

                    //如果來源是新字模 將模式從互換更改為增加
                    if ((SourceData as StampingTypeModel)?.IsNewAddStamping == true)
                    {
                        //不可以把新建的字模直接增加到鋼印機上
                        if ((TargetData as StampingTypeModel)?.StampingTypeNumber != 0 && (TargetData as StampingTypeModel)?.StampingTypeNumber != null)
                        {
                            DDMethodType = DragDropMethod.None;
                            return;
                        }
                        else
                        {
                            SourceData = (SourceData as StampingTypeModel).Clone();
                            (SourceData as StampingTypeModel).IsNewAddStamping = false;
                            DDMethodType = DragDropMethod.Copy;
                        }
                    }

                    if (SourceData != null && TargetData != null)
                    {
                        //有數值才需要互換No編號
                        var SourceTypeNumber = (SourceData as StampingTypeModel).StampingTypeNumber;
                        var TargetTypeNumber = (TargetData as StampingTypeModel).StampingTypeNumber;
                        (TargetData as StampingTypeModel).StampingTypeNumber = SourceTypeNumber;
                        (SourceData as StampingTypeModel).StampingTypeNumber = TargetTypeNumber;
                    }
                }

                var SIndex = dropInfo.DragInfo.SourceIndex;
                var TIndex = targetList.IndexOf(TargetData);
                switch (DDMethodType)
                {
                    case DragDropMethod.Move:
                        sourceList.RemoveAt(SIndex);
                        targetList.Insert(dropInfo.UnfilteredInsertIndex, SourceData);
                        break;
                    case DragDropMethod.Copy:

                        if (SourceData is ICloneableDragItem cloneableItem)
                        {
                            SourceData = cloneableItem.CloneItem(dropInfo);
                        }
                        else if (SourceData is ICloneable cloneable)
                        {
                            SourceData = cloneable.Clone();
                        }
                        targetList.Insert(dropInfo.UnfilteredInsertIndex, SourceData);
                        break;
                    case DragDropMethod.Delete:
                        sourceList.RemoveAt(SIndex);
                        break;
                    case DragDropMethod.Exchange:
                        if (SIndex != -1 && TIndex != -1)
                        {
                            sourceList.RemoveAt(SIndex);
                            sourceList.Insert(SIndex, TargetData);
                            targetList.RemoveAt(TIndex);
                            targetList.Insert(TIndex, SourceData);
                        }
                        break;


                    default:
                        break;
                }
            }



            else
            {
                //移動不互換
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

            //建立新物件
            if(targetList !=null && sourceList == null)
            {
                try
                {
                   // GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                    if (SourceData is StampingTypeModel)
                        targetList.Insert(dropInfo.UnfilteredInsertIndex, SourceData);
                }
                catch(Exception e) 
                {

                }
            }


            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView)
            {
                ((ICollectionView)dropInfo.TargetCollection).Refresh();
            }

            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)    
            {
                if (dropInfo.DragInfo.SourceCollection is ICollectionView)
                {
                    ((ICollectionView)dropInfo.DragInfo.SourceCollection).Refresh();
                }
            }

        }

    }
}
