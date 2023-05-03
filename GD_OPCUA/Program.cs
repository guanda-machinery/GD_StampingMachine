using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace GD_StampingMachineConnect
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                new Program().Test();
            });

            //opc.tcp://127.0.0.1:62541/SharpNodeSettings/OpcUaServer
            OpcUaHelper.Forms.FormBrowseServer formBrowseServer = new(ServerUrl);
            formBrowseServer.ShowDialog();
        

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }





        public static string ServerUrl
        {
            get 
            {  
                var BaseUrl = new Uri($"opc.tcp://{HostString}:{Port}");
                var CombineUrl = new Uri(BaseUrl, ServerDataPath); 
                return CombineUrl.ToString(); 
            }
    }
         
        readonly static string HostString = "127.0.0.1";
        readonly static int Port = 62541;
        readonly static string ServerDataPath = "SharpNodeSettings/OpcUaServer";

        private async void Test()
        {
                StampingMachineConnectInterface Opcua = new GD_StampingMachineConnectHelperClient();
                if (await Opcua.ConnectAsync(HostString, Port, ServerDataPath))
                {
                //Opcua.WriteNode();
                //Opcua.ReadNode_TEST();
               // Opcua.ReadReference_Test();
                //Opcua.ReadAllReference();
                Opcua.Disconnect();
                }
            }


    }
}
