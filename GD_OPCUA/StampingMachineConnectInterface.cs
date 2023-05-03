using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachineConnect
{
    public interface StampingMachineConnectInterface
    {
        public Task<bool> ConnectAsync(string HostPath ,  int Port , string DataPath =null);

        public void Disconnect();
    }
}
