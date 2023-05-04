using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_MachineConnect.Machine.Interfaces
{
    public interface IStampingMachineConnect
    {
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        bool Connect(string HostPath ,  int Port , string DataPath =null);
        /// <summary>
        /// 離線
        /// </summary>
        void Disconnect();
        //在這邊寫取得機台參數等功能
        bool GetMachineStatus(out Enums.MachineStatus Status);


    }
}
