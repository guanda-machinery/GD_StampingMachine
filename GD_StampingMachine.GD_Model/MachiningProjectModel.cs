namespace GD_StampingMachine.GD_Model
{
    public class MachiningProjectModel
    {
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 製作數量
        /// </summary>
        public double WorkPieceCurrent { get; set; }

        /// <summary>
        /// 目標製作數量
        /// </summary>
        public double WorkPieceTarget { get; set; }

    }
}
