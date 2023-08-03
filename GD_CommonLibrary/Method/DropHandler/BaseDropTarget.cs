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
using System.Windows.Markup;

namespace GD_CommonLibrary.Method
{
    /// <summary>
    /// https://qiita.com/mkuwan/items/35d91e7c2f9edfe9884a#%E3%81%AF%E3%81%98%E3%82%81%E3%81%AB
    /// </summary>
    public class BaseDropTarget : MarkupExtension,IDropTarget
    {

        public virtual void DragEnter(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragEnter(dropInfo);
            // nothing here
        }

        public virtual void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public virtual void DragLeave(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragLeave(dropInfo);
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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
          return this;
        }
    }




}
