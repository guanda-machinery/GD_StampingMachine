using DevExpress.Mvvm.Native;
using DevExpress.Xpf.CodeView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class SyncObservableCollection<T> : ObservableCollection<T>
    {
        // SynchronizationContext _SynchronizationContext = SynchronizationContext.Current;
        public void SyncAdd(T item)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                base.Add(item);
            });
        }
        public void SyncRemove(T item)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                base.Remove(item);
            });
        }
        public void SyncRemoveItem(int index)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                base.RemoveItem(index);
            });
        }

        public void SyncRemoveRange(int index , int count)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (count != 0)
                {
                    List<T> list = new List<T>(count);
                    for (int i = index; i < count; i++)
                    {
                        list.Add(Items[i]);
                    }

                    Items.RemoveRange(index, count);
                    //OnCollectionChangedCore(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, index));
                }
            });
        }

    }
}
