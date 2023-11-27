using DevExpress.CodeParser;
using DevExpress.XtraRichEdit.Utils;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary
{
    public class BaseSingletonAsync<T> : System.IAsyncDisposable, INotifyPropertyChanged where T : BaseSingletonAsync<T>, new()
    {
        private static readonly AsyncLazy<T> _instance =
            new(CreateAndLoadDataAsync, null);

        private static async Task<T> CreateAndLoadDataAsync()
        {
            var ret = new T();
            await ret.InitAsync();
            return ret;
        }

        public static AsyncLazy<T> Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 初始化複寫
        /// </summary>
        /// <returns></returns>
        protected virtual async Task InitAsync()
        {
            await Task.CompletedTask;
        }

        /*private BaseSingletonAsync()
        {

        }*/

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        
        private bool disposedValue;
        protected virtual async ValueTask DisposeAsyncCoreAsync()
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
            await Task.Delay(1);
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~BaseSingletonAsync()
        {

        }

        /*  public void Dispose()
          {
              // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
              Dispose(disposing: true);
              GC.SuppressFinalize(this);
          }*/

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCoreAsync();
            //Dispose(false);
            GC.SuppressFinalize(this);
        }



    }
}
