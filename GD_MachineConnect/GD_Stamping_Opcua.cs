using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_MachineConnect
{
    public class GD_Stamping_Opcua : IStampingMachineConnect
    {
        private GD_OpcUaHelperClient GD_OpcUaClient = new();

        public bool Connect(string HostPath, int Port, string DataPath)
        {
            bool Result = false;
            var ConnectTask = Task.Run(async () =>
            {
                Result = await GD_OpcUaClient.OpcuaConnectAsync(HostPath, Port, DataPath);
            });
            ConnectTask.Wait();
            return Result;
        }
        public void Disconnect()
        {
            GD_OpcUaClient.Disconnect();
        }

        public bool GetAxisSetting(out AxisSettingModel AxisSetting)
        {
            throw new NotImplementedException();
        }

        public bool GetEngineerSetting(out EngineerSettingModel EngineerSetting)
        {
            throw new NotImplementedException();
        }

        public bool GetInputOutput(out InputOutputModel InputOutput)
        {
            throw new NotImplementedException();
        }

        public bool GetMachanicalSpecification(out MachanicalSpecificationModel MachanicalSpecification)
        {
            throw new NotImplementedException();
        }

        public bool GetMachineStatus(out MachineStatus Status)
        {
            Status = Enums.MachineStatus.Disconnect;
            return false;
        }

        public bool GetSeparateBoxNumber(int Index, out SeparateBoxModel SeparateBox)
        {
            throw new NotImplementedException();
        }

        public bool GetSeparateSetting(out SeparateSettingModel SeparateSetting)
        {
            throw new NotImplementedException();
        }

        public bool GetSingleStampingType(int Index, out StampingTypeModel StampingType)
        {
            throw new NotImplementedException();
        }

        public bool GetStampingTypeList(out ObservableCollection<StampingTypeModel> StampingTypeList)
        {
            throw new NotImplementedException();
        }

        public bool GetTimingSetting(out TimingSettingModel TimingSetting)
        {
            throw new NotImplementedException();
        }

        public bool SetAxisSetting(AxisSettingModel AxisSetting)
        {
            throw new NotImplementedException();
        }

        public bool SetEngineerSetting(EngineerSettingModel EngineerSetting)
        {
            throw new NotImplementedException();
        }

        public bool SetInputOutput(InputOutputModel InputOutput)
        {
            throw new NotImplementedException();
        }

        public bool SetMachanicalSpecification(MachanicalSpecificationModel MachanicalSpecification)
        {
            throw new NotImplementedException();
        }

        public bool SetSeparateBoxNumber(SeparateBoxModel SeparateBox)
        {
            throw new NotImplementedException();
        }

        public bool SetSeparateSetting(SeparateSettingModel SeparateSetting)
        {
            throw new NotImplementedException();
        }

        public bool SetSingleStampingTypeList(int Index, StampingTypeModel StampingType)
        {
            throw new NotImplementedException();
        }

        public bool SetStampingTypeList(ObservableCollection<StampingTypeModel> StampingTypeList)
        {
            throw new NotImplementedException();
        }

        public bool SetTimingSetting(TimingSettingModel TimingSetting)
        {
            throw new NotImplementedException();
        }
    }
}
