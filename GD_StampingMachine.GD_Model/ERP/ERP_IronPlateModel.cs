using System;
using System.Collections.Generic;

namespace GD_StampingMachine.GD_Model
{
    public class ERP_IronPlateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OkNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Kg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool QrCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? NewDataTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifyDataTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PartPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeatNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> QrCodeContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> TrainNumber { get; set; }
    }
}
