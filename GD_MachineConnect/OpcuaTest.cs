using OpcUaHelper.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GD_MachineConnect
{
    public class OpcuaTest
    {
        OpcUaHelper.Forms.FormBrowseServer formBrowseServer;
        public DialogResult OpenFormBrowseServer(string HostString = "127.0.0.1", int Port = 62541, string ServerDataPath = "SharpNodeSettings/OpcUaServer")
        {
            try
            {
                if(string.IsNullOrEmpty( HostString ))
                    formBrowseServer = new FormBrowseServer();
                else
                {
                    var BaseUrl = new Uri($"opc.tcp://{HostString}:{Port}");
                    var CombineUrl = new Uri(BaseUrl, ServerDataPath);
                    var ServerUrl = CombineUrl.ToString();
                    formBrowseServer = new FormBrowseServer(ServerUrl);
                }

                return formBrowseServer.ShowDialog();
            }
            catch
            {

            }
            return DialogResult.Abort;
         }


    }
}
