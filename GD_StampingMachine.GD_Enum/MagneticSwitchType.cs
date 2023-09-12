using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    public enum StampingCylinderType
    {
        /// <summary>
        /// 雙導桿缸(可動端)
        /// </summary>
        GuideRod_Move,
        /// <summary>
        /// 雙導桿缸(固定端)
        /// </summary>
        GuideRod_Fixed,
        /// <summary>
        /// QR壓座組
        /// </summary>
        QRStamping,
        /// <summary>
        /// 鋼印壓座組
        /// </summary>
        StampingSeat,
        /// <summary>
        /// 阻擋缸
        /// </summary>
        BlockingCylinder,
        /// <summary>
        /// Z軸油壓缸
        /// </summary>
        HydraulicEngraving,
        /// <summary>
        /// 裁切
        /// </summary>
        HydraulicCutting,
    }
}
