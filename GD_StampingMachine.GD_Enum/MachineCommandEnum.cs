using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    public enum MachineCommandEnum
    {
        /// <summary>
        /// 無狀態
        /// </summary>
        None = 0,
        /// <summary>
        /// 運輸
        /// </summary>
        Transport,
        /// <summary>
        /// QRCODE
        /// </summary>
        QR,
        /// <summary>
        /// 字體鋼印
        /// </summary>
        Font,
        /// <summary>
        /// 切斷
        /// </summary>
        Cut

    }




}
