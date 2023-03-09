using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine
{
    public class RelayParameterizedCommand : ICommand
    {
        #region 私有方法

        /// <summary>
        /// 運行的動作
        /// </summary>
        private Action<object> mAction;

        #endregion

        #region  公開事件

        /// <summary>
        /// <see cref ="CanExecute(object)"/>值更改時觸發的事件
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// 建構式
        /// </summary>
        public RelayParameterizedCommand(Action<object> action)
        {
            mAction = action;
        }

        #endregion

        #region 命令方法

        /// <summary>
        /// 中繼命令始終可以執行
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// 執行命令動作
        /// </summary>
        /// <param name="parameter">參數</param>
        public void Execute(object parameter)
        {
            mAction(parameter);
        }

        #endregion
    }
}
