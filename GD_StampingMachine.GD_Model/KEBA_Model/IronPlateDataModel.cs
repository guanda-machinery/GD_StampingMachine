using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class IronPlateDataModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int iIronPlateID { get; set; }
        /// <summary>
        /// 字串1的x座標
        /// </summary>
        public float rXAxisPos1 { get; set; }
        /// <summary>
        /// 字串1的y座標
        /// </summary>
        public float rYAxisPos1 { get; set; }

        /// <summary>
        /// 字串2的x座標
        /// </summary>
        public float rXAxisPos2 { get; set; }
        /// <summary>
        /// 字串2的y座標
        /// </summary>
        public float rYAxisPos2 { get; set; }


        private string _sIronPlateName1;
        /// <summary>
        /// 字串1的內容
        /// </summary>
        public string sIronPlateName1 { get => _sIronPlateName1 ??= string.Empty; set => _sIronPlateName1 = value; }

        private string _sIronPlateName2;
        /// <summary>
        /// 字串2的內容
        /// </summary>
        public string sIronPlateName2 { get=> _sIronPlateName2 ??= string.Empty; set=> _sIronPlateName2 = value; }
        /// <summary>
        /// 字串3的x座標
        /// </summary>
        //public float rXAxisPos3 { get; set; }
        /// <summary>
        /// 字串3的y座標
        /// </summary>
        //public float rYAxisPos3 { get; set; }
        /// <summary>
        /// 字串3的內容
        /// </summary>
        //public string sIronPlateName3 { get; set; }
        /// <summary>
        /// QR Code的字串
        /// </summary>
        //public string sQRCodeName1 { get; set; }

        /// <summary>
        ///  QR Code的x座標
        /// </summary>
        //public float rQRcodeXAxisPos { get; set; }
        /// <summary>
        /// QR Code前字串
        /// </summary>
        //public string sQRCodeName2 { get; set; }
        /// <summary>
        /// 分料盒
        /// </summary>
        public int iStackingID { get; set; }
        /// <summary>
        /// QRCode完成
        /// </summary>
        //public bool bQRCodeFinish { get; set; }
        /// <summary>
        /// 刻碼完成
        /// </summary>
        public bool bEngravingFinish { get; set; }
        /// <summary>
        /// QRCode完成?
        /// </summary>
        public bool bDataMatrixFinish { get; set; }


    }
}
