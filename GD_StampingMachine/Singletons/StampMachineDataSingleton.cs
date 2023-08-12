using DevExpress.Xpf.Scheduling.Themes;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.Singletons
{
    public class StampMachineDataSingleton : GD_CommonLibrary.BaseSingleton<StampMachineDataSingleton>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }


        protected override void Init()
        {

        }


        private bool _opcuaTestIsOpen= false;
        public bool OpcuaTestIsOpen { get => _opcuaTestIsOpen; private set { _opcuaTestIsOpen = value; OnPropertyChanged(); } }
     
        
        OpcuaTest _opcuaTest = new OpcuaTest();
        internal async Task TestConnect()
        {
            await Task.Run(() =>
            {
                if (OpcuaTestIsOpen == false)
                {
                    try
                    {
                        OpcuaTestIsOpen = true;
                        _opcuaTest.TestOpen();
                    }
                    catch(Exception ex)
                    {

                    }
                    finally
                    {
                        OpcuaTestIsOpen = false;
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
        private bool _isScaning = false;
        public bool IsScaning { get=> _isScaning; set { _isScaning = value; OnPropertyChanged(); } }
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

                    IsScaning = false;
                });
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
