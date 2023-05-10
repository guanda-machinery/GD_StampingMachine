using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;

namespace GD_MachineConnectTest
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string HostString = "127.0.0.1";
            int Port = 62541;
            string ServerDataPath = "SharpNodeSettings/OpcUaServer";


            var BaseUrl = new Uri($"opc.tcp://{HostString}:{Port}");
            var CombineUrl = new Uri(BaseUrl, ServerDataPath);
            var ServerUrl = CombineUrl.ToString();


            Task.Run(async () =>
            {
                try
                {
                    var Opcua = new GD_OpcUaHelperClient();
                    if (await Opcua.OpcuaConnectAsync(HostString, Port, ServerDataPath))
                    {
                        //Opcua.WriteNode();
                        //Opcua.ReadNode_TEST();
                        // Opcua.ReadReference_Test();
                        Opcua.ReadAllReference();
                        Opcua.Disconnect();
                    }

                    IStampingMachineConnect SMachine = new GD_Stamping_Opcua();
                    if (SMachine.Connect(HostString, Port, ServerDataPath))
                    {

                        SMachine.GetMachineStatus(out var status);
                        SMachine.Disconnect();
                    }
                }
                catch (Exception ex)
                {

                }



            });

            //opc.tcp://127.0.0.1:62541/SharpNodeSettings/OpcUaServer
            OpcUaHelper.Forms.FormBrowseServer formBrowseServer = new OpcUaHelper.Forms.FormBrowseServer(ServerUrl);
            formBrowseServer.ShowDialog();



            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
