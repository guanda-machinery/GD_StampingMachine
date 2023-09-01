using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using GD_CommonLibrary;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Newtonsoft.Json;
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
using System.Windows.Markup;

namespace GD_CommonLibrary
{
    public abstract class BaseViewModel : MarkupExtension , INotifyPropertyChanged
    {

        [JsonIgnore]
        public abstract string ViewModelName { get; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

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
