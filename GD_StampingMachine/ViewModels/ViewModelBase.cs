using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GD_StampingMachine.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDropTarget
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// 屬性變更事件
        /// </summary>
        /// <param name="propertyName">屬性名稱</param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public virtual void DragEnter(IDropInfo dropInfo)
        {
            // nothing here
        }

        public virtual void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
            {
                dropInfo.NotHandled = dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource;
            }
        }

        public virtual void DragLeave(IDropInfo dropInfo)
        {

        }

        public virtual void Drop(IDropInfo dropInfo)
        {
            // The default drop handler don't know how to set an item's group. You need to explicitly set the group on the dropped item like this.
            //沒拿東西
            if (dropInfo?.DragInfo == null)
            {
                return;
            }
            //drop預設
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            return;
            var insertIndex = dropInfo.UnfilteredInsertIndex;

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                if (itemsControl.Items is IEditableCollectionView editableItems)
                {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0)
                    {
                        ++insertIndex;
                    }
                    else if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count)
                    {
                        --insertIndex;
                    }
                }
            }

            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool isSameCollection = false;
            var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
            if (sourceList != null)
            {
                isSameCollection = sourceList.IsSameObservableCollection(destinationList);
                if (!isSameCollection)
                {
                    foreach (var o in data)
                    {
                        var index = sourceList.IndexOf(o);
                        if (index != -1)
                        {
                            sourceList.RemoveAt(index);

                            // If source is destination too fix the insertion index
                            if (destinationList != null && ReferenceEquals(sourceList, destinationList) && index < insertIndex)
                            {
                                --insertIndex;
                            }
                        }
                    }
                }
            }
            if (destinationList != null)
            {
                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(System.Windows.DragDropEffects.Copy) || dropInfo.Effects.HasFlag(System.Windows.DragDropEffects.Link);

                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        if (o is ICloneableDragItem cloneableItem)
                        {
                            obj2Insert = cloneableItem.CloneItem(dropInfo);
                        }
                        else if (o is ICloneable cloneable)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    if (!cloneData && isSameCollection)
                    {
                        var index = destinationList.IndexOf(o);
                        if (index != -1)
                        {
                            if (insertIndex > index)
                            {
                                insertIndex--;
                            }

                            Move(destinationList, index, insertIndex++);
                        }
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }

                    if (obj2Insert is IDragItemSource dragItemSource)
                    {
                        dragItemSource.ItemDropped(dropInfo);
                    }
                }
            }

            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView)
            {
                ((ICollectionView)dropInfo.TargetCollection).Refresh();
            }
        }


        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable enumerable and not string)
            {
                return enumerable;
            }

            return Enumerable.Repeat(data, 1);
        }

        protected static void Move(IList list, int sourceIndex, int destinationIndex)
        {
            if (!list.IsObservableCollection())
            {
                throw new ArgumentException("ObservableCollection<T> was expected", nameof(list));
            }

            if (sourceIndex != destinationIndex)
            {
                var method = list.GetType().GetMethod("Move", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                _ = method?.Invoke(list, new object[] { sourceIndex, destinationIndex });
            }
        }



    }
}
