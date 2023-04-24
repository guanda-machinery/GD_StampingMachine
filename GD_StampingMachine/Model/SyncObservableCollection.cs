using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Model
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
    }
}
