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
        public IronPlateDataModel()
        {
            sDataMatrixName1 = string.Empty;
            sDataMatrixName2 = string.Empty;
            sIronPlateName1 = string.Empty;
            sIronPlateName2 = string.Empty;
        }



        private string _sIronPlateName1;
        private string _sIronPlateName2;

        private string _sDataMatrixName1;
        private string _sDataMatrixName2;

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


        /// <summary>
        /// 字串1的內容
        /// </summary>
        public string sIronPlateName1 { get => _sIronPlateName1 ??= string.Empty; set => _sIronPlateName1 = value; }

        /// <summary>
        /// 字串2的內容
        /// </summary>
        public string sIronPlateName2 { get=> _sIronPlateName2 ??= string.Empty; set=> _sIronPlateName2 = value; }
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

        public string sDataMatrixName1 { get => _sDataMatrixName1 ??= string.Empty; set => _sDataMatrixName1 = value; }
        public string sDataMatrixName2 { get => _sDataMatrixName2 ??= string.Empty; set => _sDataMatrixName2 = value; }

    }




}
