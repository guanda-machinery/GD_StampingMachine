using GD_CommonLibrary;
using GD_StampingMachine.GD_Model;
using Newtonsoft.Json;
using System;

namespace GD_StampingMachine.ViewModels
{
    public class IO_InfoViewModel : BaseViewModel
    {
        public override string ViewModelName => nameof(IO_InfoViewModel);

        private IO_InfoModel IO_Info = new();

        public IO_InfoViewModel()
        {
            IO_Info = new();
        }
        public IO_InfoViewModel(IO_InfoModel io_info)
        {
            IO_Info = io_info;
        }
        /// <summary>
        /// 功能或狀態說明
        /// </summary>
        public string Info { get => IO_Info.Info; set { IO_Info.Info = value; OnPropertyChanged(); } }

        /// <summary>
        /// OPCUA 對應ID
        /// </summary>
        public string NodeID { get => IO_Info.NodeID; set { IO_Info.NodeID = value; OnPropertyChanged(); } }

        /// <summary>
        /// 端子
        /// </summary>
        public string BondingCableTerminal { get => IO_Info.BondingCableTerminal; set { IO_Info.BondingCableTerminal = value; OnPropertyChanged(); } }

        /// <summary>
        /// KEBA 定義
        /// </summary>
        public string KEBA_Definition { get => IO_Info.KEBA_Definition; set { IO_Info.KEBA_Definition = value; OnPropertyChanged(); } }


        /// <summary>
        /// i/o類別
        /// </summary>
        public GD_Enum.ioSensorType SensorType { get => IO_Info.SensorType; set { IO_Info.SensorType = value; OnPropertyChanged(); } }
        /// <summary>
        /// 資料格式
        /// </summary>
        public Type ValueType { get => IO_Info.ValueType; set { IO_Info.ValueType = value; OnPropertyChanged(); } }

        [JsonIgnore]
        public object IO_Value { get => IO_Info.IO_Value; set { IO_Info.IO_Value = value; OnPropertyChanged(); } }

    }
}
