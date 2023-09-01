using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary
{
    public abstract class BaseSingleton<T> where T : BaseSingleton<T>
    {




        private static readonly Lazy<T> Lazy = new(() => (Activator.CreateInstance(typeof(T), true) as T)!);
        private static readonly object thisLock = new();
        private static T _instance; 
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
                            _instance = Lazy.Value;
                            _instance.Init();
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
