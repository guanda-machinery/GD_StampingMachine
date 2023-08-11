using DevExpress.Xpf.Scheduling.Themes;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>
    {
        protected override void Init()
        {

        }


        private bool _opcuaTestIsOpen= false;
        public bool OpcuaTestIsOpen { get => _opcuaTestIsOpen; }

        internal async Task TestConnect()
        {
            await Task.Run(() =>
            {
                if (_opcuaTestIsOpen == false)
                {
                    try
                    {
                        _opcuaTestIsOpen = true;
                        var _opcuaTest = new OpcuaTest();
                        _opcuaTest.TestOpen();
                    }
                    catch(Exception ex)
                    {

                    }
                    finally
                    {
                        _opcuaTestIsOpen = false;
                    }
                }
            });
        }


        public string HostString = "127.0.0.1";
        public int Port = 62541;
        public string ServerDataPath = "SharpNodeSettings/OpcUaServer";

        private GD_OpcUaHelperClient gd_OpcUaHelperClient = new();
            //啟用掃描
        public async Task<bool> TestConnectToOpcua()
        {
            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(HostString, Port, ServerDataPath))
            {
                gd_OpcUaHelperClient.ReadAllReference();
                gd_OpcUaHelperClient.Disconnect();
                return true;
            }
            return false;
        }

        private bool ContinueScanning = true;
        public bool IsScaning { get; set; } = false;
        public void ScanOpcua()
        {
            if (!IsScaning)
            {
                IsScaning = true;
                Task.Run(async () =>
                {
                    do
                    {
                        try
                        {
                            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(HostString, Port, ServerDataPath))
                            {
                                var a = gd_OpcUaHelperClient.ReadAllReference();
                                gd_OpcUaHelperClient.Disconnect();
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        await Task.Delay(100);

                    } while (ContinueScanning);

                });
                IsScaning = false;
            }
            return;
        }


        public async void StopScan()
        {
            if (!IsScaning)
                return;

            await Task.Run(async () => 
            {
                ContinueScanning = false;
               //等待掃描解除
                while(IsScaning)
                {
                    await Task.Delay(100);
                }

                //回復狀態
                ContinueScanning = true;
            });
            return;
        }




        public async void WriteOpcua()
        {

            if (await gd_OpcUaHelperClient.OpcuaConnectAsync(HostString, Port, ServerDataPath))
            {
                var a = gd_OpcUaHelperClient.ReadAllReference();
                gd_OpcUaHelperClient.Disconnect();
            }
            return;
        }







    }
}
