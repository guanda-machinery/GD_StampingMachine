using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Model;
using Opc.Ua;
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

        public bool Connect(string HostPath, int Port, string DataPath , string UserName = null, string Password = null)
        {
            bool Result = false;
            Task.Run(async () =>
            {
                if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    GD_OpcUaClient.UserIdentity = new UserIdentity(UserName, Password);

                Result = await GD_OpcUaClient.OpcuaConnectAsync(HostPath, Port, DataPath);
            }).Wait();
            return Result;
        }
        public void Disconnect()
        {
            GD_OpcUaClient.Disconnect();
        }

        public bool FeedingPositionBwd()
        {
            //GD_OpcUaClient.WriteNode( , true)
            throw new NotImplementedException();
        }

        public bool FeedingPositionFwd()
        {

            throw new NotImplementedException();
        }

        public bool FeedingPositionReturnToStandby()
        {

            throw new NotImplementedException();
        }

        public bool GetAxisSetting(out AxisSettingModel AxisSetting)
        {

            throw new NotImplementedException();
        }
        public bool GetFeedingPosition(out float Position)
        {
            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Feeding1.sv_rFeedingPosition , out Position);
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

        public bool SetFeedingPosition(out float Position)
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



        #region 節點
        /// <summary>
        /// 節點對應字串
        /// </summary>
        private class StampingOpcUANode
        {
            static readonly string NodeHeader = "ns=4;s=APPL";
            /// <summary>
            /// 節點變數
            /// </summary>
            enum NodeVariable
            {
                /// <summary>
                /// 進料X軸馬達
                /// </summary>
                Feeding1,
                /// <summary>
                /// 雙導桿缸-移動
                /// </summary>
                GuideRodsFixture1,
                /// <summary>
                /// 雙導桿缸-固定
                /// </summary>
                GuideRodsFixture2,
                /// <summary>
                /// QR壓座組
                /// </summary>
                Fixture1,
                /// <summary>
                /// 鋼印壓座組
                /// </summary>
                Fixture2,
                /// <summary>
                /// 阻擋缸
                /// </summary>
                BlockingClips1,
                /// <summary>
                /// 鋼印轉盤進料
                /// </summary>
                EngravingFeeding1,
                /// <summary>
                /// 鋼印轉盤進料
                /// </summary>
                Engraving1,

                /// <summary>
                /// 鋼印轉盤旋轉
                /// </summary>
                EngravingRotate1,
                /// <summary>
                /// 系統
                /// </summary>
                system,
                /// <summary>
                /// 油壓單元
                /// </summary>
                Motor1,
                /// <summary>
                /// 裁切
                /// </summary>
                Cutting1,
                /// <summary>
                /// 分料
                /// </summary>
                Stacking1

            }

            /// <summary>
            /// 按鈕
            /// </summary>
            enum BButton
            {
                sv_bButtonFwd,
                sv_bButtonBwd,
                sv_bButtonUp,
                sv_bButtonDown,
                sv_bButtonOpen,
                sv_bButtonClose,
                sv_bButtonCW,
                sv_bButtonCCW,
                sv_bButtonMotor,

            }

            /// <summary>
            /// 裁切
            /// </summary>
            enum BCutting
            {
                sv_bCuttingOpen,
                sv_bCuttingClosed,
            }

            /// <summary>
            /// 伺服控制
            /// </summary>
            enum SServoMove
            {
                sv_rFeedingPosition,
                sv_rServoMovePos,
                sv_rServoStandbyPos
            }


            enum REngraving
            {
                /// <summary>
                /// Y軸馬達目前位置
                /// </summary>
                sv_rEngravingFeedingPosition,
                /// <summary>
                /// 鋼印目前Z軸位置
                /// </summary>
                sv_rEngravingPosition
            }



            /// <summary>
            /// 一般磁簧
            /// </summary>
            enum SMagneticSwitch
            {


                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                bFixtureUp,
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                bFixtureDown
            }
            /// <summary>
            /// 導桿缸磁簧
            /// </summary>
            enum SMagneticSwitch_GuideRods
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                sv_bGuideRodsFixtureUp,

                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bGuideRodsFixtureDown,
            }

            /// <summary>
            /// 阻擋缸磁簧
            /// </summary>
            enum SMagneticSwitch_BlockingClips
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                sv_bBlockingClipsUp,

                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bBlockingClipsDown,
            }

            /// <summary>
            /// 站號
            /// </summary>
            enum TargetStation
            {
                /// <summary>
                /// 目前站號
                /// </summary>
                sv_iThisStation,
                /// <summary>
                /// 目標站號
                /// </summary>
                sv_iTargetAStation
            }

            enum HMI
            {
                sv_HMIIronPlateName 
            }



            /// <summary>
            /// 進料X軸馬達
            /// </summary>
            public class Feeding1
            {
                /// <summary>
                /// 馬達目前位置
                /// </summary>
                public static string sv_rFeedingPosition => $"{NodeHeader}.{NodeVariable.Feeding1}.{SServoMove.sv_rFeedingPosition}";
                /// <summary>
                /// 馬達位置移動命令
                /// </summary>
                public static string sv_rServoMovePos => $"{NodeHeader}.{NodeVariable.Feeding1}.{SServoMove.sv_rServoMovePos}";
                /// <summary>
                /// 回歸基準點命令
                /// </summary>
                public static string sv_rServoStandbyPos => $"{NodeHeader}.{NodeVariable.Feeding1}.{SServoMove.sv_rServoStandbyPos}";
                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonBwd}";
            }

            /// <summary>
            /// 雙導桿缸-移動
            /// </summary>
            public class GuideRodsFixture1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureDown}";          
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{BButton.sv_bButtonUp}";        
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture1}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// 雙導桿缸-固定
            /// </summary>
            public class GuideRodsFixture2
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{SMagneticSwitch_GuideRods.sv_bGuideRodsFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.GuideRodsFixture2}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// QR壓座組
            /// </summary>
            public class Fixture1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.bFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.Fixture1}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.Fixture1}.{BButton.sv_bButtonDown}";
            }

            /// <summary>
            /// 鋼印壓座組
            /// </summary>
            public class Fixture2
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bGuideRodsFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bGuideRodsFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.bFixtureDown}";
                /// <summary>
                ///手動氣壓缸升命令
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.Fixture2}.{BButton.sv_bButtonUp}";
                /// <summary>
                ///手動氣壓缸降命令
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.Fixture2}.{BButton.sv_bButtonDown}";
            }
        
            /// <summary>
            /// 阻擋缸
            /// </summary>
            public class BlockingClips1
            {
                /// <summary>
                /// 磁簧訊號上限 
                /// </summary>
                public static string sv_bBlockingClipsUp => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{SMagneticSwitch_BlockingClips.sv_bBlockingClipsUp}";
                /// <summary>
                /// 磁簧訊號下限 
                /// </summary>
                public static string sv_bBlockingClipsDown => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{SMagneticSwitch_BlockingClips.sv_bBlockingClipsDown}";

                /// <summary>
                /// 手動氣壓缸升命令 
                /// </summary>
                public static string sv_bButtonUp => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{BButton.sv_bButtonUp}";
                /// <summary>
                /// 手動氣壓缸降命令 
                /// </summary>
                public static string sv_bButtonDown => $"{NodeHeader}.{NodeVariable.BlockingClips1}.{BButton.sv_bButtonDown}";

            }

            /// <summary>
            /// 鋼印進料
            /// </summary>
            public class EngravingFeeding1
            {
                /// <summary>
                /// Y軸馬達目前位置
                /// </summary>
                public static string sv_rEngravingFeedingPosition => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{REngraving.sv_rEngravingFeedingPosition}";
                /// <summary>
                /// 回歸基準點命令
                /// </summary>
                public static string rServoStandbyPos => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{SServoMove.sv_rServoStandbyPos}";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                ///  手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonBwd}";
            }

            /// <summary>
            /// 鋼印
            /// </summary>
            public class Engraving1
            {
                /// <summary>
                /// 鋼印目前Z軸位置 
                /// </summary>
                public static string sv_rEngravingPosition => $"{NodeHeader}.{NodeVariable.Engraving1}.{REngraving.sv_rEngravingPosition}";

                /// <summary>
                /// 手動油壓缸升命令 
                /// </summary>
                public static string sv_bButtonClose => $"{NodeHeader}.{NodeVariable.Engraving1}.{BButton.sv_bButtonClose}";
                /// <summary>
                /// 手動油壓缸降命令 
                /// </summary>
                public static string sv_bButtonOpen => $"{NodeHeader}.{NodeVariable.Engraving1}.{BButton.sv_bButtonOpen}";

            }


            /// <summary>
            /// 鋼印旋轉
            /// </summary>
            public class EngravingRotate1
            {
                /// <summary>
                /// 目前旋轉角度/或選定的字元 (目前站號)
                /// </summary>
                public static string sv_rEngravingPosition => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{TargetStation.sv_iThisStation}";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCW}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCCW}";



            }

            /// <summary>
            /// 系統
            /// </summary>
            public class system
            {
                /// <summary>
                /// 鋼印目前選定的字元 - 變更鋼印目前選定的字元命令
                /// </summary>
                public static string sv_iTargetAStation => $"{NodeHeader}.{NodeVariable.system}.{TargetStation.sv_iTargetAStation}";

                /// <summary>
                /// 進行自動加工時需傳入資料
                /// </summary>
                public static string sv_HMIIronPlateName => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}";
            }


            /// <summary>
            /// 分料組
            /// </summary>
            public class Stacking1
            {
                /// <summary>
                /// 分料組當前位置
                /// </summary>
                public static string sv_iThisStation => $"{NodeHeader}.{NodeVariable.Stacking1}.{TargetStation.sv_iThisStation}";
                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.Stacking1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.Stacking1}.{BButton.sv_bButtonBwd}";
            }


            /// <summary>
            /// 油壓單元
            /// </summary>
            public class Motor1
            {
                /// <summary>
                /// 馬達啟動
                /// </summary>
                public static string sv_bButtonMotor => $"{NodeHeader}.{NodeVariable.Motor1}.{BButton.sv_bButtonMotor}";

            }

            /// <summary>
            /// 裁切
            /// </summary>
            public class Cutting1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bCuttingOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingOpen}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bCuttingClosed => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingClosed}";
                /// <summary>
                /// 手動油壓缸升命令
                /// </summary>
                public static string sv_bButtonOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonOpen}";
                /// <summary>
                /// 手動油壓缸降命令
                /// </summary>
                public static string sv_bButtonClose => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonClose}";
            }  









        }


        #endregion







    }
}
