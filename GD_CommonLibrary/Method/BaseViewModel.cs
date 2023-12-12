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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GD_CommonLibrary
{
    public abstract class BaseViewModel : MarkupExtension , INotifyPropertyChanged, IDisposable //, IAsyncDisposable
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private bool disposedValue;

        [JsonIgnore]
        public abstract string ViewModelName { get; }

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~BaseViewModel()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

   /*     public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore().ConfigureAwait(false);
            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await Task.CompletedTask;
        }*/

        public bool IsDebuggerAttached => Debugger.IsAttached; 


    }
}
