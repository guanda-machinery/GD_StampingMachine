using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_MachineConnect
{
    public class GD_StampingMachineConnectHelper : GD_OpcUaHelperClient, IStampingMachineConnect
    {

        public bool Connect(string HostPath, int Port, string DataPath = null)
        {
            bool Result = false;
            var ConnectTask = Task.Run(async () =>
            {
                Result =  await this.OpcuaConnectAsync(HostPath, Port, DataPath);
            });
            ConnectTask.Wait();
            return Result;
        }



        public bool GetMachineStatus(out MachineStatus Status)
        {
            //this.ReadAllReference();
            Status = Enums.MachineStatus.Disconnect;
            return false;
        }


    }
}
