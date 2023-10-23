using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    /// <summary>
    /// 加工種類
    /// </summary>
    public enum SteelBeltStampingStatusEnum
    {
        /// <summary>
        /// 無
        /// </summary>
        None,
        /// <summary>
        /// QR雕刻
        /// </summary>
        QRCarving,
        /// <summary>
        /// 沖壓字模
        /// </summary>
        Stamping,
        /// <summary>
        /// 鋼帶切割
        /// </summary>
        Shearing
    }
}
