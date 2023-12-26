namespace GD_StampingMachine.GD_Model
{
    /// <summary>
    /// 鋼印文字
    /// </summary>
    public class StampingTypeModel
    {
        /// <summary>
        /// 鋼印文字
        /// </summary>
        public string StampingTypeString { get; set; }

        /// <summary>
        /// No編號
        /// </summary>
        public int StampingTypeNumber { get; set; }
        /// <summary>
        /// 使用次數
        /// </summary>
        public int StampingTypeUseCount { get; set; }

        public bool IsNewAddStamping { get; set; }
    }
}
