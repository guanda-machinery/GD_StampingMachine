using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class SendMachineCommandViewModel :BaseViewModel
    {
        public SendMachineCommandViewModel()
        {

        }

        public SendMachineCommandViewModel(SendMachineCommandModel sendMachineCommand)
        {
            SendMachineCommand = sendMachineCommand;
        }

        public readonly SendMachineCommandModel SendMachineCommand = new();


        /// <summary>
        /// 
        /// </summary>
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus
        {
            get=> SendMachineCommand.SteelBeltStampingStatus; 
            set 
            {
                SendMachineCommand.SteelBeltStampingStatus = value;
                OnPropertyChanged(); 
            } 
        }

        /// <summary>
        /// 工作移動到下一個工站的相對距離(由計算得出 輪到他加工時他需移動的距離)
        /// </summary>
        public double RelativeMoveDistance
        {
            get => SendMachineCommand.RelativeMoveDistance; 
            set 
            {
                SendMachineCommand.RelativeMoveDistance = value; 
                OnPropertyChanged(); 
            }
        }

        /// <summary>
        /// 工作需要移動的絕對距離(目前位置離加工位置多遠)
        /// </summary>
        public double AbsoluteMoveDistance 
        { 
            get => SendMachineCommand.AbsoluteMoveDistance; set
            {
                SendMachineCommand.AbsoluteMoveDistance = value; 
                OnPropertyChanged();
            } 
        }


        private GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel _settingBaseVM;
        /// <summary>
        /// 鐵牌
        /// </summary>
        public GD_StampingMachine.ViewModels.ParameterSetting.SettingBaseViewModel SettingBaseVM
        {
            get=>_settingBaseVM;             
            set
            {
                _settingBaseVM = value;
                if (value != null)
                    SendMachineCommand.StampPlateSetting = value.StampPlateSetting;
                OnPropertyChanged();
            }
        }

        public double StampWidth
        {
            get => SendMachineCommand.StampWidth;
            set
            {
                SendMachineCommand.StampWidth = value;
                OnPropertyChanged();
            }
        }












    }
}
