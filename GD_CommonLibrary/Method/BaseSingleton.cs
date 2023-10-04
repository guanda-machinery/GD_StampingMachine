using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary
{
    public abstract class BaseSingleton<T>: IDisposable, INotifyPropertyChanged where T : BaseSingleton<T>  
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

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~BaseSingleton()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /*
             // 多執行緒，lock 使用
        private static readonly object thisLock = new object();

        // 將唯一實例設為 private static
        private static LogDataSingleton instance;


        // 設為 private，外界不能 new
        private LogDataSingleton()
        {

        }
        // 外界只能使用靜態方法取得實例
        public static LogDataSingleton Instance
        {
            //雙重鎖同步
            get
            {
                if (null == instance)
                    lock (thisLock)
                        if (null == instance)
                            instance = new LogDataSingleton();
                return instance;
            }
        }
     
     */








}
