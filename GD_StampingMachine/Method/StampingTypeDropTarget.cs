using GD_StampingMachine.Model;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GD_StampingMachine.Method
{
    public class StampingTypeDropTarget: BaseDropTarget
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)    // 同item時不互換
            {
                //除非是未上鋼印機的預備區字模 否則不容許自己互換
                if (dropInfo.DragInfo.Data is StampingTypeModel SourceData)
                {
                    if (SourceData.StampingTypeNumber > 0 || SourceData.IsNewAddStamping == true)
                    {
                        dropInfo.NotHandled = false;
                        return;
                    }
                }
            }
            else
            {
                if (dropInfo.DragInfo.Data is StampingTypeModel SourceData)
                {
                    if (dropInfo.TargetItem is StampingTypeModel TargetData)
                    {
                        //若源頭是新增字模 且目標是鋼印機的字模 則不可以放上去
                        if (SourceData.IsNewAddStamping == true && TargetData.StampingTypeNumber > 0)
                        {
                            dropInfo.NotHandled = false;
                            return;
                        }

                        //目標是新增字模不可以放上去
                        if (TargetData.IsNewAddStamping)
                        {
                            dropInfo.NotHandled = false;
                            return;
                        }
                    }

                    //目標是垃圾桶
                    if (dropInfo.TargetItem is null && dropInfo.TargetCollection.TryGetList() is null)
                    {
                        if (SourceData.StampingTypeNumber > 0)
                        {
                            dropInfo.NotHandled = false;
                            return;
                        }
                        if (SourceData.IsNewAddStamping)
                        {
                            dropInfo.NotHandled = false;
                            return;
                        }
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

                if (SourceData != null)
                {
                    //目標是刪除按鈕 或是為空
                    if (TargetData is System.Windows.Controls.Button || (TargetData is null && TargetData is null))
                    {
                        DDMethodType = DragDropMethod.None;
                        if ((SourceData as StampingTypeModel).StampingTypeNumber <= 0)
                            if (!(SourceData as StampingTypeModel).IsNewAddStamping)
                                DDMethodType = DragDropMethod.Delete;
                    }
                    //有數值才需要互換No編號

                    var SourceTypeNumber = (SourceData as StampingTypeModel)?.StampingTypeNumber;
                    var TargetTypeNumber = (TargetData as StampingTypeModel)?.StampingTypeNumber;

                    if (TargetData is StampingTypeModel && SourceTypeNumber.HasValue)
                        (TargetData as StampingTypeModel).StampingTypeNumber = SourceTypeNumber.Value;

                    if (SourceData is StampingTypeModel && TargetTypeNumber.HasValue)
                        (SourceData as StampingTypeModel).StampingTypeNumber = TargetTypeNumber.Value;
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
                    //不可以把新建的字模丟掉
                    else if (TargetData is null && targetList is null )
                    {
                        DDMethodType = DragDropMethod.None;
                        return;
                    }
                    else
                    {
                        if (SourceData is ICloneableDragItem cloneableItem)
                        {
                            SourceData = cloneableItem.CloneItem(dropInfo);
                        }
                        else if (SourceData is ICloneable cloneable)
                        {
                            SourceData = cloneable.Clone();
                        }
                        (SourceData as StampingTypeModel).IsNewAddStamping = false;
                        DDMethodType = DragDropMethod.Copy;
                    }
                }

                var SIndex = dropInfo.DragInfo.SourceIndex;
                var TIndex = -1;

                if(targetList is IList)
                    TIndex = targetList.IndexOf(TargetData);



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
            if (targetList != null && sourceList == null)
            {
                try
                {
                    // GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                    if (SourceData is StampingTypeModel)
                        targetList.Insert(dropInfo.UnfilteredInsertIndex, SourceData);
                }
                catch (Exception e)
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
