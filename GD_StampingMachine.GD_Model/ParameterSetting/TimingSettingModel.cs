using System;
using System.Collections.Generic;
using System.Linq;

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
        public TimingControlModel()
        {

        }
        public bool IsEquals(TimingControlModel obj)
        {
            return 
                this.RestTime == obj.RestTime &&
                this.OpenTime == obj.OpenTime &&
                this.IsEnable == obj.IsEnable &&
                DayOfWeekWorkCollection.Count == obj.DayOfWeekWorkCollection.Count &&
                DayOfWeekWorkCollection.Zip(obj.DayOfWeekWorkCollection, (a, b) =>
                a.DayOfWeek == b.DayOfWeek && a.IsWork == b.IsWork).All(x => x);
        }




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

        public List<DayOfWeekWorkModel> DayOfWeekWorkCollection { get; set; }


    }

    public class DayOfWeekWorkModel
    {
        public DayOfWeekWorkModel()
        {

        }
        public DayOfWeekWorkModel(DayOfWeek dayOfWeek, bool isWork)
        {
            DayOfWeek = dayOfWeek;
            IsWork = isWork;
        }

        public DayOfWeek DayOfWeek { get; set; }
        public bool IsWork { get; set; }
    }


}
