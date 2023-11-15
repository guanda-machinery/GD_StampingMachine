using OpcUaHelper.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GD_MachineConnect
{
    public class OpcuaTest
    {
        OpcUaHelper.Forms.FormBrowseServer formBrowseServer;
        public async Task<DialogResult> OpenFormBrowseServerAsync(IPAddress hostIP, int Port = 62541, string ServerDataPath = "SharpNodeSettings/OpcUaServer")
        {
            try
            {
                if(hostIP == null)
                    formBrowseServer = new FormBrowseServer();
                else
                {
                    var BaseUrl = new Uri($"opc.tcp://{hostIP.ToString()}:{Port}");
                    var CombineUrl = new Uri(BaseUrl, ServerDataPath);
                    var ServerUrl = CombineUrl.ToString();
                    formBrowseServer = new FormBrowseServer(ServerUrl);
                }

                var tcs = new TaskCompletionSource<DialogResult>();
                formBrowseServer.FormClosed += (sender, e) => tcs.SetResult(DialogResult.Abort);
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    formBrowseServer.Show();
                });
                return await tcs.Task;
            }
            catch
            {

            }

            return DialogResult.Abort;
         }


        public  DialogResult OpenFormBrowseServer(IPAddress hostIP, int Port = 62541, string ServerDataPath = "SharpNodeSettings/OpcUaServer")
        {
            try
            {
                if (hostIP == null)
                    formBrowseServer = new FormBrowseServer();
                else
                {
                    var BaseUrl = new Uri($"opc.tcp://{hostIP.ToString()}:{Port}");
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
