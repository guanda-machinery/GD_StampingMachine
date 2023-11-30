using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_CommonLibrary
{
    public abstract class BaseSingleton<T>: IAsyncDisposable, INotifyPropertyChanged where T : BaseSingleton<T>  
    {

        private static readonly Lazy<T> Lazy = new(() => (Activator.CreateInstance(typeof(T), true) as T)!);
        private static readonly object thisLock = new();
        private static T _instance;
        private bool disposedValue;

        public static T Instance 
        {
            //雙重鎖同步
            get
            {
                if (_instance == null)
                {
                    lock (thisLock)
                    {
                        if (_instance == null)
                        {
                            try
                            {
                                _instance = Lazy.Value;
                                _instance.Init();
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
                        }
                    }
                }
                return _instance;
            }
        }





        /// <summary>
        /// 如果有需要初始化 複寫這裡！
        /// </summary>
        protected virtual void Init()  
        {

        }


        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        /*protected virtual void Dispose(bool disposing)
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
        }*/

        protected virtual async ValueTask DisposeAsyncCoreAsync()
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
            await Task.Delay(1);
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~BaseSingleton()
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
