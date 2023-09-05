using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    /// <summary>
    /// i/o類別
    /// </summary>
    public enum ioSensorType
    {
        None,
        /// <summary>
        /// 數位輸入
        /// </summary>
        DI,
        /// <summary>
        /// 數位輸出
        /// </summary>
        DO,
        /// <summary>
        /// 類比輸入
        /// </summary>
        AI,
        /// <summary>
        /// 類比輸出
        /// </summary>
        AO
    }
}
