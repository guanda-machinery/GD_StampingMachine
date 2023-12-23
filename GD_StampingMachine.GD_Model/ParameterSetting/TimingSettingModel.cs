using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.GD_Model
{
    /// <summary>
    /// 計時設定
    /// </summary>
    public class TimingSettingModel
    {
        /// <summary>
        /// 設定名稱
        /// </summary>
        public string SettingName { get; set; }
        /// <summary>
        /// 休息時間
        /// </summary>
        public DateTime RestTime { get; set; }
        /// <summary>
        /// 開啟時間
        /// </summary>
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// 重複次數
        /// </summary>
        public int RepeatCount { get; set; }


        private List<TimingControlModel> timingControlCollection;
        public List<TimingControlModel> TimingControlCollection { get => timingControlCollection ??= new(); set => timingControlCollection = value; }
    }
    public class TimingControlModel
    {
        /// <summary>
        /// 休息時間
        /// </summary>
        public DateTime RestTime { get; set; }
        /// <summary>
        /// 開啟時間
        /// </summary>
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// 已啟用
        /// </summary>
        public bool IsEnable { get; set; }


    }


}
