using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using GD_StampingMachine.Model;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GD_StampingMachine.ViewModels
{



    public class ViewModelBase : INotifyPropertyChanged
    { 
        /// <summary>
        /// 訊息紀錄
        /// </summary>
       // public SyncObservableCollection<OperatingLogViewModel> LogDataObservableCollection { get; set; } = new SyncObservableCollection<OperatingLogViewModel>();

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        /// <summary>
        /// 屬性變更事件
        /// </summary>
        /// <param name="propertyName">屬性名稱</param>
      /*  public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

    }
}
