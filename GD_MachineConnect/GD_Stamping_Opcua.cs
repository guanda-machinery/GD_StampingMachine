using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;

using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using System.Net.Sockets;
using static GD_MachineConnect.GD_Stamping_Opcua.StampingOpcUANode;
using Opc.Ua;


namespace GD_MachineConnect
{
    public class GD_Stamping_Opcua : IAsyncDisposable
    {
        /// <summary>
        /// 實作長連接
        /// </summary>
        private GD_OpcUaFxClient GD_OpcUaClient;

        public const int ConntectMillisecondsTimeout = 1000;
        public const int MoveTimeout = 5000;

        private bool disposedValue;


        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                await GD_OpcUaClient.DisposeAsync();
                disposedValue = true;
            }

        }


        public GD_Stamping_Opcua()
        {
            GD_OpcUaClient = new();
        }

        public GD_Stamping_Opcua(string HostIP, int Port, string DataPath, string UserName, string Password)
        {
            Opc.UaFx.Client.OpcClientIdentity UserIdentity = new (UserName, Password);
            GD_OpcUaClient = new GD_OpcUaFxClient(HostIP, Port, DataPath, UserIdentity);
        }

        public bool IsConnected { get => GD_OpcUaClient?.IsConnected == true; }
        /// <summary>
        /// 建立連線
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncConnect()
        {
           return await GD_OpcUaClient.AsyncConnect();
        }

        public Exception ConnectException { get => GD_OpcUaClient.ConnectException; }


        /*public void Disconnect()
        {
            GD_OpcUaClient.Disconnect();
        }*/


        public async Task DisconnectAsync()
        {
            await  GD_OpcUaClient?.DisconnectAsync();
        }










        public async Task<bool> FeedingPositionBwd(bool Active)
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, false);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, Active);
        }

        public async Task<bool> FeedingPositionFwd(bool Active)
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, false);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, Active);
        }

        public async Task<bool> FeedingPositionReturnToStandbyPosition()
        {
            await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bUseHomeing, true);
            await Task.Delay(500);
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_bUseHomeing, false);
        }

        /* public async Task<bool> SetReset()
         {
             await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.system.sv_bResetTotEnergy, true);
             await Task.Delay(500);
             return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.system.sv_bResetTotEnergy, false);
         }*/



        /// <summary>
        /// 訂閱機台模式
        /// </summary>
        /// <param name="action"></param>
        public Task SubscribeOperationMode(Action<int> action)
        {
           return GD_OpcUaClient.SubscribeNodeDataChange<int>(StampingOpcUANode.system.sv_OperationMode, action);
        }





        /// <summary>
        /// 取得機台狀態
        /// </summary>
        public async Task<(bool, OperationModeEnum)> GetOperationMode()
        {
            var ret = await GD_OpcUaClient.AsyncReadNode<int>(StampingOpcUANode.system.sv_OperationMode);
            return (ret.Item1, (OperationModeEnum)ret.Item2);
        }
        /// <summary>
        /// 設定機台狀態
        /// </summary>
        public async Task<bool> SetOperationMode(OperationModeEnum operationMode)
        {
            bool ret = false;
            if (GD_OpcUaClient.IsConnected)
            {
                switch (operationMode)
                    {
                        case OperationModeEnum.Setup:
                            ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonSetup, true);
                            break;
                        case OperationModeEnum.Manual:
                            ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonManual, true);
                            break;
                        case OperationModeEnum.HalfAutomatic:
                            ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, true);
                            break;
                        case OperationModeEnum.FullAutomatic:
                            ret = await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, true);
                            break;
                        default:
                            break;
                    }
                await Task.Delay(100);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonManual, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonSetup, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonHalfAuto, false);
                await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonFullAuto, false);
            }
            return ret;
        }

        /// <summary>
        /// 取得X軸是否在原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,bool)> GetServoHome()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Feeding1.di_ServoHome);
        }



        public async Task<(bool, float)> GetFeedingPosition()
        {
            return await GD_OpcUaClient.AsyncReadNode<float> (StampingOpcUANode.Feeding1.sv_rFeedingPosition) ;
        }


        public async Task<(bool, EngineerSettingModel)> GetEngineerSetting( )
        {
            throw new NotImplementedException();
        }



        public async Task<(bool, InputOutputModel)> GetInputOutput()
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, MachanicalSpecificationModel)> GetMachanicalSpecification()
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, MachineStatus)> GetMachineStatus()
        {
            var Status = Enums.MachineStatus.Disconnect;

           // sv_iActState

           // return GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Feeding1.sv_rFeedingPosition, out Position);
            return (false,Status);
        }

        public async Task<bool> Reset()
        {
             await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm , true);
             await Task.Delay(100);
            return await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonAlarmConfirm ,false);
        }

        public async Task<bool> CycleStart()
        {
            await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, true);
            await Task.Delay(100);
            return await GD_OpcUaClient.AsyncWriteNode<bool>(StampingOpcUANode.OperationMode1.sv_bButtonCycleStart, false);
        }




        public async Task<(bool, int)> GetSeparateBoxNumber()
        {
            return await GD_OpcUaClient.AsyncReadNode<int>(StampingOpcUANode.Stacking1.sv_iThisStation);
        }

        public async Task<(bool, SeparateSettingModel)> GetSeparateSetting()
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, StampingTypeModel)> GetSingleStampingType(int Index)
        {
            throw new NotImplementedException();
        }


        public async Task<(bool, TimingSettingModel)> GetTimingSetting()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetAxisSetting(AxisSettingModel AxisSetting)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetEngineerSetting(EngineerSettingModel EngineerSetting)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetFeedingPosition(float Position)
        {
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_rServoMovePos, Position);
        }

        public async Task<bool> SetInputOutput(InputOutputModel InputOutput)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetMachanicalSpecification(MachanicalSpecificationModel MachanicalSpecification)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetSeparateBoxNumber(int boxIndex)
        {
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Stacking1.sv_iThisStation, boxIndex);
        }

        public async Task<bool> SetSeparateSetting(SeparateSettingModel SeparateSetting)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetSingleStampingTypeList(int Index, StampingTypeModel StampingType)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> SetTimingSetting(TimingSettingModel TimingSetting)
        {
            throw new NotImplementedException();
        }



        public async Task<bool> Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction , bool IsTrigger)
        {
            bool ret;
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, IsTrigger);
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    break;
                case StampingCylinderType.GuideRod_Fixed:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, IsTrigger);
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    break;
                case StampingCylinderType.QRStamping:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, IsTrigger);
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    break;
                case StampingCylinderType.StampingSeat:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, IsTrigger);
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    break;
                case StampingCylinderType.BlockingCylinder:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonUp, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonDown, IsTrigger);
                            break;
                        default:
                            var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, false);
                            var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, false);
                            ret = await O_ret && await C_ret;
                            break;
                    }
                    break;
                case StampingCylinderType.HydraulicEngraving:

                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, IsTrigger);
                            break;
                        default:
                            var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                            var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                            ret = await O_ret && await C_ret;
                            break;
                    }

                    break;
                case StampingCylinderType.HydraulicCutting:

                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, IsTrigger);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, IsTrigger);
                            break;
                        default:
                            var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, false);
                            var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, false);
                            ret = await O_ret && await C_ret;
                            break;
                    }

                    break;
                default: throw new NotImplementedException();
            }
            return ret;
        }



        /// <summary>
        ///  雙導桿缸(可動端)在上方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        ///  雙導桿缸(可動端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Move_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureDown);
        }
          /// <summary>
          /// 雙導桿缸(固定端)在上方
          /// </summary>
          /// <param name="stampingCylinder"></param>
          /// <param name="direction"></param>
          /// <returns></returns>
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureUp);
        }
        /// <summary>
        /// 雙導桿缸(固定端)在下方
        /// </summary>
        /// <param name="stampingCylinder"></param>
        /// <param name="direction"></param>
        /// <returns></returns> 
        public async Task<(bool, bool)> GetGuideRod_Fixed_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureDown);
        }

        /// <summary>
        /// QR壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureUp);
        }

        /// <summary>
        /// QR壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetQRStamping_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureDown);
        }
        /// <summary>
        /// 鋼印壓座組在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureUp);

        }
        /// <summary>
        /// 鋼印壓座組在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetStampingSeat_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureDown);
        }

        /// <summary>
        /// 阻擋缸在上方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isUp()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp);
        }
        /// <summary>
        /// 阻擋缸在下方
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetBlockingCylinder_Position_isDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown);
        }

        /// <summary>
        /// 原點位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_Origin()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
        }

        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StandbyPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
        }
        public async Task<(bool, bool)> GetHydraulicEngraving_Position_StopDown()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
        }

        /// <summary>
        /// 原點
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_Origin()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingOrigin);
        }
        /// <summary>
        /// 待命位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_StandbyPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingStandbyPoint);
        }
        /// <summary>
        /// 切割位置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicCutting_Position_CutPoint()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.di_CuttingCutPoint);
        }


        public async Task<bool> SetHydraulicPumpMotor(bool Active)
        {
            var pumptask = await GetHydraulicPumpMotor();
            if (pumptask.Item1)
            {
                if (pumptask.Item2 == Active)
                    return true;
                await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, true);
                await Task.Delay(1000);
                return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, false);
            }
            else
                return false;
        }

        /// <summary>
        /// 油壓單元
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, bool)> GetHydraulicPumpMotor()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Motor1.sv_bMotorStarted);
        }


        /// <summary>
        /// 訂閱油壓單元
        /// </summary>
        /// <param name="action"></param>
        public Task SubscribeHydraulicPumpMotor(Action<bool> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange(StampingOpcUANode.Motor1.sv_bMotorStarted, action);
        }




        /// <summary>
        /// 取得鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<(bool, bool)> GetRequestDatabit()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.system.sv_bRequestDatabit}");
        }
        /// <summary>
        /// 設定鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        public async Task<bool> SetRequestDatabit(bool databit)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_bRequestDatabit}", databit);
        }

        /// <summary>
        /// 取得預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        /*public async Task<(bool,string)> GetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType)
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}");
        }*/

        /// <summary>
        /// 設定預計要打的鋼片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
     /*   public async Task<bool> SetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}", StringLine);
        }*/


        /// <summary>
        /// 取得下一個要加工的鋼印
        /// </summary>

        public async Task<(bool,IronPlateDataModel )> GetHMIIronPlate()
        {
            var rootNode = StampingOpcUANode.system.sv_HMIIronPlateName.NodeName;
            return await this.GetIronPlate(rootNode);
        }
        /// <summary>
        /// 設定下一個鋼印
        /// </summary>
        public async Task<bool> SetHMIIronPlate(IronPlateDataModel ironPlateData)
        {

           // bool ret = false;
            // var wNodeTrees = IronPlateDataTreeNode(StampingOpcUANode.system.sv_HMIIronPlateName.NodeName, ironPlateData);
            var rootNode = StampingOpcUANode.system.sv_HMIIronPlateName.NodeName;
          var wNodeTrees=  new Dictionary<string, object>
            {
                [rootNode + "." + "iIronPlateID"] = ironPlateData.iIronPlateID,
                [rootNode + "." + "rXAxisPos1"] = ironPlateData.rXAxisPos1,
                [rootNode + "." + "rYAxisPos1"] = ironPlateData.rYAxisPos1,
                [rootNode + "." + "rXAxisPos2"] = ironPlateData.rXAxisPos2,
                [rootNode + "." + "rYAxisPos2"] = ironPlateData.rYAxisPos2,
                [rootNode + "." + "sIronPlateName1"] = ironPlateData.sIronPlateName1,
                [rootNode + "." + "sIronPlateName2"] = ironPlateData.sIronPlateName2,
                [rootNode + "." + "iStackingID"] = ironPlateData.iStackingID,
                [rootNode + "." + "bEngravingFinish"] = ironPlateData.bEngravingFinish,
                [rootNode + "." + "bDataMatrixFinish"] = ironPlateData.bDataMatrixFinish,
                [rootNode + "." + "sDataMatrixName1"] = ironPlateData.sDataMatrixName1,
                [rootNode + "." + "sDataMatrixName2"] = ironPlateData.sDataMatrixName2

        };
            var ret = await GD_OpcUaClient.AsyncWriteNodes(wNodeTrees);
            //設定完後須更新hmi
            return !ret.Contains(false);
        }

        [Obsolete]
        public async Task<bool> SetDataMatrixMode(bool IsUse)
        {
            bool ret = false;
            //ns=4;s=APPL.system.sv_DataMatrixMode
            var rootNode = StampingOpcUANode.system.sv_DataMatrixMode;
            ret = await GD_OpcUaClient.AsyncWriteNode(rootNode, Convert.ToInt32(IsUse));
            //設定完後須更新hmi
            return ret;
        }

        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<(bool result, List<IronPlateDataModel> ironPlateCollection)> GetIronPlateDataCollection()
        {
            List<IronPlateDataModel> ironPlateDataList = new();
            if (GD_OpcUaClient.IsConnected)
            {
                try
                {
                    for (int i = 1; i <= 25; i++)
                    {
                        var node = $"{StampingOpcUANode.system.sv_IronPlateData}[{i}]";
                        if ((await GetIronPlate(node)).Item1)
                        {
                            ironPlateDataList.Add((await GetIronPlate(node)).Item2);
                        }
                        else
                        {
                            return (false, ironPlateDataList);
                        }
                    }
                    return (true, ironPlateDataList);
                }
                catch (Exception)
                {

                }
            }
            return (false, ironPlateDataList);
        }



        private async Task<(bool, IronPlateDataModel)> GetIronPlate(string rootNode)
        {
            var getTask_iIronPlateID = await GD_OpcUaClient.AsyncReadNode<int>(rootNode + "." + "iIronPlateID");
            var getTask_rXAxisPos1 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rXAxisPos1");
            var getTask_rYAxisPos1 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rYAxisPos1");
            var getTask_rXAxisPos2 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rXAxisPos2");
            var getTask_rYAxisPos2 = await GD_OpcUaClient.AsyncReadNode<float>(rootNode + "." + "rYAxisPos2");
            var getTask_sIronPlateName1 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sIronPlateName1");
            var getTask_sIronPlateName2 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sIronPlateName2");
            var getTask_iStackingID = await GD_OpcUaClient.AsyncReadNode<int>(rootNode + "." + "iStackingID");
            var getTask_bEngravingFinish = await GD_OpcUaClient.AsyncReadNode<bool>(rootNode + "." + "bEngravingFinish");
            var getTask_bDataMatrixFinish = await GD_OpcUaClient.AsyncReadNode<bool>(rootNode + "." + "bDataMatrixFinish");
            var getTask_sDataMatrixName1 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sDataMatrixName1");
            var getTask_sDataMatrixName2 = await GD_OpcUaClient.AsyncReadNode<string>(rootNode + "." + "sDataMatrixName2");

            var ret = ((getTask_iIronPlateID).Item1 &&
                (getTask_rXAxisPos1).Item1 &&
                (getTask_rYAxisPos1).Item1 &&
                (getTask_sIronPlateName1).Item1 &&
                (getTask_rXAxisPos2).Item1 &&
                (getTask_rYAxisPos2).Item1 &&
                (getTask_sIronPlateName2).Item1 &&
                (getTask_iStackingID).Item1 &&
                (getTask_bEngravingFinish).Item1 &&
                (getTask_bDataMatrixFinish).Item1 &&
                (getTask_sDataMatrixName1).Item1 &&
                (getTask_sDataMatrixName2).Item1
                );

            var newIronPlateData = new IronPlateDataModel()
            {    //ID
                iIronPlateID = (getTask_iIronPlateID).Item2,
                // 字串1的x座標
                rXAxisPos1 = (getTask_rXAxisPos1).Item2,
                // 字串1的y座標
                rYAxisPos1 = (getTask_rYAxisPos1).Item2,
                // 字串1的內容
                sIronPlateName1 = (getTask_sIronPlateName1).Item2,
                // 字串2的x座標
                rXAxisPos2 = (getTask_rXAxisPos2).Item2,
                // 字串2的y座標
                rYAxisPos2 = (getTask_rYAxisPos2).Item2,

                /// 字串2的內容
                sIronPlateName2 = (getTask_sIronPlateName2).Item2,
                /// 字串3的x座標
                //rXAxisPos3 = (await getTask_rXAxisPos3).Item2,
                /// 字串3的y座標
                //rYAxisPos3 = (await getTask_rYAxisPos3).Item2,
                /// 字串3的內容
                //sIronPlateName3  =(await getTask_sIronPlateName3).Item2,
                // QR Code的字串
                //sQRCodeName1=  (await getTask_sQRCodeName1).Item2,

                //  QR Code的x座標
                //rQRcodeXAxisPos  =(await getTask_rQRcodeXAxisPos).Item2,
                // QR Code前字串
                //sQRCodeName2 = (await getTask_sQRCodeName2).Item2,
                // 分料盒
                iStackingID = (getTask_iStackingID).Item2,
                // QRCode完成
                //bQRCodeFinish  =(await getTask_bQRCodeFinish).Item2,
                // 刻碼完成 
                bEngravingFinish = (getTask_bEngravingFinish).Item2,
                bDataMatrixFinish = (getTask_bDataMatrixFinish).Item2,
                sDataMatrixName1 = (getTask_sDataMatrixName1).Item2,
                sDataMatrixName2 = (getTask_sDataMatrixName2).Item2,

            };

            return (ret, newIronPlateData);
        }






        /// <summary>
        /// 設定鐵片群資訊
        /// </summary>
        /// <param name="ironPlateDataList"></param>
        /// <returns></returns>
        public async Task<bool> SetIronPlateDataCollection(List<IronPlateDataModel> ironPlateDataList)
        {
     
            int ExistedDataCollectionCount = 1000;
            List<IronPlateDataModel> write_ironPlateDataList;
            //取得舊有的鐵片群資訊
            var getIronPlateDataCollectionTuple = await GetIronPlateDataCollection();
            if (getIronPlateDataCollectionTuple.result)
            {
                write_ironPlateDataList = getIronPlateDataCollectionTuple.ironPlateCollection;
                ExistedDataCollectionCount = write_ironPlateDataList.Count;
                //超出範圍的不寫
            }
            else
            {
                write_ironPlateDataList = Enumerable.Repeat(new IronPlateDataModel()
                {
                    sIronPlateName1 = string.Empty,
                    sIronPlateName2 = string.Empty,
                }, ExistedDataCollectionCount).ToList();
            }

            for (int i = 0 ; i< write_ironPlateDataList.Count; i++)
            {
                if (i < ironPlateDataList.Count)
                    write_ironPlateDataList[i] = ironPlateDataList[i];
                else
                {
                    write_ironPlateDataList[i].sIronPlateName1 = string.Empty;
                    write_ironPlateDataList[i].sIronPlateName2 = string.Empty;
                    write_ironPlateDataList[i].sDataMatrixName1 = string.Empty;
                    write_ironPlateDataList[i].sDataMatrixName2 = string.Empty;
                }
            }



            List<(string,bool)> WritedList = new();
            for (int i = 0; i < write_ironPlateDataList.Count; i++)
            {
                IronPlateDataModel ironPlateData = write_ironPlateDataList[i];

                string node = $"{StampingOpcUANode.system.sv_IronPlateData}[{i + 1}]";
                var wNodeTreeNodes = IronPlateDataTreeNode(node, ironPlateData);

                var WriteBoolean = false;
                for (int j = 0; j < 5; j++)
                {
                    var WriteBooleanList = await GD_OpcUaClient.AsyncWriteNodes(wNodeTreeNodes);
                    WriteBoolean = !WriteBooleanList.Contains(false);
                    if (WriteBoolean)
                        break;
                }
                WritedList.Add((node, WriteBoolean));
            }
            return WritedList.Exists(x=>x.Item2 ==true);
           // return true;
          //  return await GD_OpcUaClient.AsyncWriteNodes(wNodeTrees);
        }


        private Dictionary<string, object> IronPlateDataTreeNode(string node, IronPlateDataModel ironPlateData)
        {
            return new Dictionary<string, object>
            {
                [node + "." + "iIronPlateID"] = ironPlateData.iIronPlateID,
                [node + "." + "rXAxisPos1"] = ironPlateData.rXAxisPos1,
                [node + "." + "rYAxisPos1"] = ironPlateData.rYAxisPos1,
                [node + "." + "rXAxisPos2"] = ironPlateData.rXAxisPos2,
                [node + "." + "rYAxisPos2"] = ironPlateData.rYAxisPos2,
                [node + "." + "sIronPlateName1"] = ironPlateData.sIronPlateName1,
                [node + "." + "sIronPlateName2"] = ironPlateData.sIronPlateName2,
                [node + "." + "iStackingID"] = ironPlateData.iStackingID,
                [node + "." + "bEngravingFinish"] = ironPlateData.bEngravingFinish,
                [node + "." + "bDataMatrixFinish"] = ironPlateData.bDataMatrixFinish
            };
        }

















        public async Task<(bool, float)> GetEngravingYAxisPosition()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rEngravingFeedingPosition}");
        }

        public async Task<(bool, float)> GetEngravingZAxisPosition()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Engraving1.sv_rEngravingPosition}");            
        }


       /* public async Task<bool> GetEngravingZAxisHydraulicUp(out bool IsActived)
        {
       return await GD_OpcUaClient.AsyncReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", out IsActived);
        }

        public async Task<bool> GetEngravingZAxisHydraulicDown(out bool IsActived)
        {
       return await GD_OpcUaClient.AsyncReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", out IsActived);
        }
        public async Task<bool> SetEngravingZAxisHydraulicUp(bool Actived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", Actived);
        }

        public async Task<bool> SetEngravingZAxisHydraulicDown(bool Actived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", Actived);
        }*/





















        public async Task<bool> SetEngravingYAxisToStandbyPos()
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rServoStandbyPos}", true);
        }
        

        public async Task<(bool, bool)> GetEngravingYAxisBwd()
        {
       return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}");
        }
        public async Task<(bool, bool)> GetEngravingYAxisFwd()
        {
       return await GD_OpcUaClient.AsyncReadNode<bool>($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}");
        }

        public async Task<bool> SetEngravingYAxisBwd(bool Active)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}", Active);
        }
        public async Task<bool> SetEngravingYAxisFwd(bool Active)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}", Active);
        }




        public async Task<(bool,int)> GetEngravingRotateStation()
        { 
            int Station = -1;
            var ret = await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.EngravingRotate1.sv_iThisStation}");

            if (ret.Item1)
            {
                //※須注意 目前使用的函式回傳index時是1為起始 所以需-1才能符合其他位置
                var StationIndex = ret.Item2;
                Station = StationIndex - 1;
            }

            return (ret.Item1 , Station);
        }

        /// <summary>
        /// 總站數
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, int)> GetEngravingTotalSlots()
        {
            return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.EngravingRotate1.sv_iTotalSlots}");
        }
        



        public async Task<(bool, StampingTypeModel)> GetEngravingRotateStationChar()
        {
            StampingTypeModel stampingType = new();
            var ret1 = await GetEngravingRotateStation();
            var ret2 = await GetRotatingTurntableInfo();

            if (ret1.Item1 && ret2.Item1)
            {
                try
                {
                    var index = ret1.Item2;
                    var tableInfo = ret2.Item2;

                    stampingType = tableInfo[index];
                    return (true , stampingType);
                }
                catch
                {
                   
                }
            }
            return (false , stampingType);

        }



        /*public async Task<bool> SetEngravingRotateStation(int Station)
        {
          //  await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);

            //return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);
        }*/

        private async Task<(bool,int[])> GetRotatingTurntableInfoINT() 
            =>await  GD_OpcUaClient.AsyncReadNode<int[]>($"{StampingOpcUANode.system.sv_RotateCodeDefinition}");
        
        private async Task<bool> SetRotatingTurntableInfoINT(int[] fonts)     
            => await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_RotateCodeDefinition}", fonts);











        public async Task<(bool,List<StampingTypeModel>)> GetRotatingTurntableInfo()
        {
            var ret =await GetRotatingTurntableInfoINT();
            var turntableInfoList = new List<StampingTypeModel>();
            if (ret.Item1)
            {
                for (int i = 0; i < ret.Item2.Length; i++)
                {
                    turntableInfoList.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = i +1,
                        StampingTypeString = ret.Item2[i].ToChar().ToString()
                    });
                }
            }
            return (ret.Item1, turntableInfoList) ;
        }

        public async Task<bool> SetRotatingTurntableInfo(List<StampingTypeModel> fonts)
        {
            List<char>charList = new List<char>();
            foreach(var font in fonts)
            {
                var charArray = font.StampingTypeString.ToCharArray();
                if(charArray.Length>0)
                    charList.Add(charArray[0]);
                else
                    charList.Add(new char());
            }

            return await SetRotatingTurntableInfoINT(charList.ToIntList().ToArray());
        }


        public async Task<bool> SetRotatingTurntableInfo(int index , char font)
        {
            var ret = await GetRotatingTurntableInfoINT();
            if (ret.Item1)
            {
                var codeInfoArray = ret.Item2;
               //確認沒超出範圍
               if (codeInfoArray.ToList().Count > index)
                {
                    codeInfoArray[index] = font.ToInt();
                    return await SetRotatingTurntableInfoINT(codeInfoArray);
                }
            }

            return false;
        }




        public async Task<bool> SetEngravingRotateCW(bool isActived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCW}", isActived);
        }

        public async Task<bool> SetEngravingRotateCCW(bool isActived)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCCW}", isActived);
        }

        public async Task<(bool, float)> GetFeedingSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}");
        }
        public async Task<bool> SetFeedingSetupVelocity(float percent)
        {
            return await GD_OpcUaClient.AsyncWriteNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedSetupVelocity}", percent);
        }
        public async Task<(bool, float)> GetFeedingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}");
        }
        public async Task<bool> SetFeedingVelocity(float percent)
        {
            return await GD_OpcUaClient.AsyncWriteNode<float>($"{StampingOpcUANode.Feeding1.sv_rFeedVelocity}", percent);
        }



        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模移動速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingFeedingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}");
        }

        /// <summary>
        /// 設定字模移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingFeedingVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rFeedVelocity}", SpeedPercent);

        }




        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_rRotateSetupVelocity}", SpeedPercent);

        }

        /// <summary>
        /// 字模旋轉速度
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, float)> GetEngravingRotateVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}");
        }

        /// <summary>
        /// 設定字模旋轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<bool> SetEngravingRotateVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_rRotateVelocity}", SpeedPercent);
        }


        /// <summary>
        /// 刻印壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingPressure()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump1.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 刻印速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetStampingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump1.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }

        /// <summary>
        /// 裁斷壓力
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingPressure()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump2.sv_PressureLintab.LintabPoints.uNoOfPoints}");
        }
        /// <summary>
        /// 裁斷速度
        /// </summary>
        /// <returns></returns>

        public async Task<(bool, float)> GetShearingVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<UInt32>($"{StampingOpcUANode.Pump2.sv_VelocityLintab.LintabPoints.uNoOfPoints}");
        }








        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixTCPIP()
        {
            return await GD_OpcUaClient.AsyncReadNode<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP);
        }

        /// <summary>
        /// QR機IP
        /// </summary>
        /// <returns></returns>
       /* public Task SubscribeDataMatrixTCPIP(Action<string> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPIP, action);
        }*/


        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> GetDataMatrixPort()
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.DataMatrix1.sv_sContactTCPPort}");
        }

        /// <summary>
        /// QR機Port
        /// </summary>
        /// <returns></returns>
      
        /*public Task SubscribeDataMatrixPort(Action<string> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange<string>(StampingOpcUANode.DataMatrix1.sv_sContactTCPPort,action);
        }*/






        public async Task<(bool, float)> GetFeedingXHomeFwdVelocity()
         {
        return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}");
         }


        public async Task<(bool, float)> GetFeedingXHomeBwdVelocity()
         {
        return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}");
         }


         public async Task<bool> SetFeedingXHomeFwdVelocity(float SpeedPercent)
         {
             return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_rHomeFwdVelocity}", SpeedPercent);
         }


       public async Task<bool> SetFeedingXHomeBwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_rHomeBwdVelocity}", SpeedPercent);
        }

        /// <summary>
        /// 潤滑設定時間
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dLubTime}");
        }

        /// <summary>
        /// 潤滑開設定時間
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, object)> GetLubricationSettingOnTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOnTime}");
        }
        /// <summary>
        /// 潤滑關設定時間
        /// </summary>
        /// <returns></returns>     
        public async Task<(bool, object)> GetLubricationSettingOffTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationSetValues.dOffTime}");
    }


        /// <summary>
        /// 潤滑實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dLubTime}");
        }
        /// <summary>
        /// 潤滑開實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOnTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOnTime}");
        }

        /// <summary>
        /// 潤滑關實際時間
        /// </summary>
        public async Task<(bool, object)> GetLubricationActualOffTime()
        {
            return await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.Lubrication1.sv_LubricationActValues.dOffTime}");
        }






        /*
        public async Task<(bool,float)> GetFeedingXFwdSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstFwdSetup}");
        }*/

        /*public async Task<(bool, float)> GetFeedingXBwdSetupVelocity()
        {

       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstBwdSetup}");
        }*/

        /*public async Task<bool> SetFeedingXFwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstFwdSetup}", SpeedPercent);
        }*/


        /*public async Task<bool> SetFeedingXBwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstBwdSetup}", SpeedPercent);

        }*/

        /*public async Task<(bool,float)> GetFeedingXFwdVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstFwd}");
        }*/

        /* public async Task<(bool, float)> GetFeedingXBwdVelocity()
         {
        return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstBwd}");
         }*/

        /*  public async Task<bool> SetFeedingXFwdVelocity(float SpeedPercent)
          {
              return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstFwd}", SpeedPercent);
          }*/

        /*  public async Task<bool> SetFeedingXBwdVelocity(float SpeedPercent)
          {
              return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstBwd}", SpeedPercent);
          }*/

        /*public async Task<(bool, float)> GetEngravingFeedingYFwdSetupVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwdSetup}");

        }*/

        /*public async Task<(bool, float)> GetEngravingFeedingYBwdSetupVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwdSetup}");

        }*/

        /*public async Task<bool> SetEngravingFeedingYFwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwdSetup}",  SpeedPercent);

        }*/

        /*public async Task<bool> SetEngravingFeedingYBwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwdSetup}",  SpeedPercent);
        }*/

        /*  public async Task<(bool, float)> GetEngravingFeedingYFwdVelocity()
          {
              return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwd}");
          }*/

        /*  public async Task<(bool, float)> GetEngravingFeedingYBwdVelocity()
          {
         return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwd}");
          }*/

        /* public async Task<bool> SetEngravingFeedingYFwdVelocity(float SpeedPercent)
         {
             return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwd}", SpeedPercent);

         }*/

        /*public async Task<bool> SetEngravingFeedingYBwdVelocity(float SpeedPercent)
        {

            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwd}", SpeedPercent);
        }*/


        /*public async Task<(bool, float)> GetEngravingFeedingA_CW_Velocity( )
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCW}");

        }

        public async Task<(bool, float)> GetEngravingFeedingA_CCW_Velocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCCW}");
        }*/

        /*  public async Task<bool> SetEngravingFeedingA_CW_Velocity(float SpeedPercent)
          {
              return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCW}", SpeedPercent);
          }

          public async Task<bool> SetEngravingFeedingA_CCW_Velocity(float SpeedPercent)
          {

              return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCCW}", SpeedPercent);
          }*/



        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
       public Task SubscribeNodeDataChange<T>(string NodeID, Action<T> action)
        {
            return GD_OpcUaClient.SubscribeNodeDataChange<T>(NodeID,action);
        }


        public Task SubscribeNodesDataChange<T>(IList<(string NodeID, Action<T> updateAction)> nodeList)
        {
             return GD_OpcUaClient.SubscribeNodesDataChange<T>(nodeList);
        }




        public async Task<(bool,T)> ReadNode<T>(string NodeTreeString)
        {
            return await GD_OpcUaClient.AsyncReadNode<T>(NodeTreeString);
        }
        public async Task<bool> WriteNode<T>(string NodeTreeString, T WriteValue)
        {
            return await GD_OpcUaClient.AsyncWriteNode(NodeTreeString, WriteValue);
        }



        /*  public async Task<bool> ReadAllReference(string NodeTreeString, out List<GD_OpcUaHelperClient.NodeTypeValue> NodeValue)
          {
              return GD_OpcUaClient.ReadAllReference(NodeTreeString, out NodeValue);
          }*/










        #region 節點
        /// <summary>
        /// 節點對應字串
        /// </summary>
        public class StampingOpcUANode
        {
            /// <summary>
            /// ns=4;s=APPL
            /// </summary>
            public static readonly string NodeHeader = "ns=4;s=APPL";
           
            /// <summary>
            /// 大節點
            /// </summary>
            private enum NodeVariable
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
                /// QrCode機器
                /// </summary>
                DataMatrix1,
                /// <summary>
                /// 系統
                /// </summary>
                OperationMode1,
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
                Stacking1,
                /// <summary>
                /// 刻印
                /// </summary>
                Pump1,
                /// <summary>
                /// 裁斷
                /// </summary>
                Pump2,
                /// <summary>
                /// 潤滑
                /// </summary>
                Lubrication1

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
                sv_bFixtureUp,
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                sv_bFixtureDown
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
            /// 字串1 2 3
            /// </summary>
          /*public  enum sIronPlate
            {
                sIronPlateName1,
                sIronPlateName2,
                sIronPlateName3
            }*/

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
                /// x軸開機回零
                /// </summary>
                public static string sv_bUseHomeing => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_bUseHomeing";

                /// <summary>
                /// 原點訊號
                /// </summary>
                public static string di_ServoHome => $"{NodeHeader}.{NodeVariable.Feeding1}.di_ServoHome";

                


                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{BButton.sv_bButtonBwd}";
                /// <summary>
                /// 自動模式進給速度
                /// </summary>
                public static string sv_rFeedSetupVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_rFeedSetupVelocity";
                /// <summary>
                /// 手動模式進給速度
                /// </summary>
                public static string sv_rFeedVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.sv_rFeedVelocity";
                /// <summary>
                /// homeing前進速度
                /// </summary>
                public static string sv_rHomeFwdVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_rHomeFwdVelocity}";
                /// <summary>
                ///homeing後退速度
                /// </summary>
                public static string sv_rHomeBwdVelocity => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_rHomeBwdVelocity}";
                /// <summary>
                /// 設定模式前進速度
                /// </summary>       
                //public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                //public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                //public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                //public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";


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
            /// QR機
            /// </summary>
            public class DataMatrix1
            {
                /// <summary>
                /// 設定ip
                /// </summary>
                public static string sv_sContactTCPIP => $"{NodeHeader}.{NodeVariable.DataMatrix1}.sv_sContactTCPIP";
                /// <summary>
                /// 設定port
                /// </summary>
                public static string sv_sContactTCPPort => $"{NodeHeader}.{NodeVariable.DataMatrix1}.sv_sContactTCPPort";
            }

            /// <summary>
            /// QR壓座組
            /// </summary>
            public class Fixture1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                public static string sv_bFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.sv_bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture1}.{SMagneticSwitch.sv_bFixtureDown}";
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
                public static string sv_bFixtureUp => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.sv_bFixtureUp}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                public static string sv_bFixtureDown => $"{NodeHeader}.{NodeVariable.Fixture2}.{SMagneticSwitch.sv_bFixtureDown}";
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


            public class OperationMode1
            {
                public static string sv_bButtonManual => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonManual";
                public static string sv_bButtonSetup => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonSetup";
                public static string sv_bButtonHalfAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonHalfAuto";
                public static string sv_bButtonFullAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonFullAuto";
                public static string sv_bButtonAlarmConfirm => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonAlarmConfirm";
                public static string sv_bButtonCycleStart => $"{NodeHeader}.{NodeVariable.OperationMode1}.sv_bButtonCycleStart";


                public static string di_ButtonManual => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonManual";
                public static string di_ButtonSetup => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonSetup";
                public static string di_ButtonHalfAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonHalfAuto";
                public static string di_ButtonFullAuto => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonFullAuto";
                public static string di_ButtonAlarmConfirm => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonAlarmConfirm";
                public static string di_ButtonCycleStart => $"{NodeHeader}.{NodeVariable.OperationMode1}.di_ButtonCycleStart";
            }

            public class Pump1
            {
                public class sv_PressureLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                public class sv_VelocityLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }

                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOPressure";

                /// <summary>
                /// AO_刻印壓力
                /// </summary>
                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOPressure";
                /// <summary>
                ///  AO_刻印速度
                /// </summary>
                //public static string sv_rAOVelocity => $"{NodeHeader}.{NodeVariable.Pump1}.sv_rAOVelocity";
            }

            public class Pump2
            {
                public class sv_PressureLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                public class sv_VelocityLintab
                {
                    public class LintabPoints
                    {
                        /// <summary>
                        /// 壓力校正表段數
                        /// </summary>
                        public static string uNoOfPoints => $"{NodeHeader}.{NodeVariable.Pump1}.sv_PressureLintab.LintabPoints.uNoOfPoints";
                    }
                }
                /// <summary>
                /// AO_裁斷壓力
                /// </summary>
                //public static string sv_rAOPressure => $"{NodeHeader}.{NodeVariable.Pump2}.sv_rAOPressure";
                /// <summary>
                ///  AO_裁斷速度
                /// </summary>
                //public static string sv_rAOVelocity => $"{NodeHeader}.{NodeVariable.Pump2}.sv_rAOVelocity";
            }
            



            //public const string VelocityNode = "Velocity.Output.rOutputValue";


            /// <summary>
            /// 速度模式
            /// </summary>
            private enum ConstSpeedSetup
            {
                /// <summary>
                /// homeing前進速度
                /// </summary>
                sv_rHomeFwdVelocity,
                /// <summary>
                /// homeing後退速度
                /// </summary>
                sv_rHomeBwdVelocity,
                /// <summary>
                /// 設定模式前進速度
                /// </summary>            
                sv_ConstFwdSetup,
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                sv_ConstBwdSetup,
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                sv_ConstFwd,
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                sv_ConstBwd,

                /// <summary>
                /// 設定模式旋轉速度
                /// </summary>            
                sv_ConstRotateSetup,
                /// <summary>
                /// 手動/自動正轉速度
                /// </summary>
                sv_ConstRotateCW,
                /// <summary>
                /// 手動/自動逆轉速度
                /// </summary>
                sv_ConstRotateCCW,
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
                public static string sv_rServoStandbyPos => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{SServoMove.sv_rServoStandbyPos}";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonFwd}";
                /// <summary>
                ///  手動後退
                /// </summary>
                public static string sv_bButtonBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{BButton.sv_bButtonBwd}";


                public static string sv_rFeedSetupVelocity => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.sv_rFeedSetupVelocity";
                public static string sv_rFeedVelocity => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.sv_rFeedVelocity";
                /// <summary>
                /// 設定模式前進速度
                /// </summary>       
                //public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                //public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                //public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                //public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";



            }

            public enum AxisPos
            {
                rXAxisPos1, rYAxisPos1,
                rXAxisPos2, rYAxisPos2,
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

                /// <summary>
                /// 油壓缸上升降狀態 
                /// </summary>
                public static string sv_bEngravingClosed => $"{NodeHeader}.{NodeVariable.Engraving1}.sv_bEngravingClosed";
                /// <summary>
                /// 油壓缸下降狀態 
                /// </summary>
                public static string sv_bEngravingOpen => $"{NodeHeader}.{NodeVariable.Engraving1}.sv_bEngravingOpen";

                /// <summary>
                /// DI_刻印z軸原點位置
                /// </summary>
                public static string di_StopUp => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StopUp";
                /// <summary>
                /// DI_刻印z軸待命位置
                /// </summary>
                public static string di_StandbyPoint => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StandbyPoint";
                /// <summary>
                /// DI_刻印z軸刻印位置
                /// </summary>
                public static string di_StopDown => $"{NodeHeader}.{NodeVariable.Engraving1}.di_StopDown";

                
                    
                    

            }


            /// <summary>
            /// 鋼印旋轉
            /// </summary>
            public class EngravingRotate1
            {
                /// <summary>
                /// 刻印a軸當前站號
                /// </summary>
                public static string sv_iThisStation => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{TargetStation.sv_iThisStation}";
                /// <summary>
                /// 刻印a軸總站數
                /// </summary>
                public static string sv_iTotalSlots => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_iTotalSlots";
                /// <summary>
                /// 刻印a軸當前位置(角度)
                /// </summary>
                //public static string sv_rEngravingRotatePosition => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rEngravingRotatePosition";

                /// <summary>
                /// 手動前進
                /// </summary>
                public static string sv_bButtonCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCW}";
                /// <summary>
                /// 手動後退
                /// </summary>
                public static string sv_bButtonCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{BButton.sv_bButtonCCW}";

                /// <summary>
                /// 設定模式移動速度
                /// </summary>       
                //public static string sv_ConstRotateSetup => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateSetup}.{VelocityNode}";
                public static string sv_rRotateSetupVelocity => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rRotateSetupVelocity";
                public static string sv_rRotateVelocity => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.sv_rRotateVelocity";
                /// <summary>
                /// 手動/自動正轉速度
                /// </summary>
                // public static string sv_ConstRotateCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";
                /// <summary>
                /// 手動/自動逆轉速度
                /// </summary>
                // public static string sv_ConstRotateCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";




            }

            /// <summary>
            /// 系統
            /// </summary>
            public class system
            {
                /// <summary>
                /// 鋼印目前選定的字元 - 變更鋼印目前選定的字元命令
                /// </summary>
                //public static string sv_iTargetAStation => $"{NodeHeader}.{NodeVariable.system}.{TargetStation.sv_iTargetAStation}";

                /// <summary>
                /// 機台模式
                /// </summary>
                public static string sv_OperationMode => $"{NodeHeader}.{NodeVariable.system}.sv_OperationMode";


                /// <summary>
                /// 進行自動加工時需傳入資料 鐵片下一片資訊
                /// </summary>
                //public static string sv_HMIIronPlateName => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}";
                /// <summary>
                /// 鐵片下一片資訊-交握訊號
                /// </summary>
                public static string sv_bRequestDatabit => $"{NodeHeader}.{NodeVariable.system}.sv_bRequestDatabit";

                /// <summary>
                /// 鐵片陣列
                /// </summary>
                public static string sv_IronPlateData => $"{NodeHeader}.{NodeVariable.system}.sv_IronPlateData";
                /// <summary>
                /// 特定鐵片
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                //public static string sv_IronPlateData_Index(int index) =>   $"{NodeHeader}.{NodeVariable.system}.sv_IronPlateData[{index}]";
                /// <summary>
                /// 開啟QR功能
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                public static string sv_DataMatrixMode => $"{NodeHeader}.{NodeVariable.system}.sv_DataMatrixMode";

              //  public static string sv_bResetTotEnergy=> $"{NodeHeader}.{NodeVariable.system}.sv_bResetTotEnergy";

                /// <summary>
                /// 人機上直接輸入對應字碼的ascii code滿共40格
                /// </summary>
                public static string sv_RotateCodeDefinition => $"{NodeHeader}.{NodeVariable.system}.sv_RotateCodeDefinition";
                /// <summary>
                /// 人機上直接輸入對應字碼的ascii code滿共40格
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                public static string sv_RotateCodeDefinitionOfIndex(int index) => $"{NodeHeader}.{NodeVariable.system}.sv_RotateCodeDefinition[{index}]";
                


                public class sv_HMIIronPlateName
                 {
                    public static string NodeName => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}";
                    /// <summary>
                    /// 字串1(第一行)
                    /// </summary>
                    //public static string sIronPlateName1 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{sIronPlate.sIronPlateName1}";

                    /// <summary>
                    /// 字串2(第二行)
                    /// </summary>
                    //public static string sIronPlateName2 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{sIronPlate.sIronPlateName2}";


                    /// <summary>
                    /// 側邊字串
                    /// </summary>
                    // public static string sIronPlateName3 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{sIronPlate.sIronPlateName3}";



                    //public static string rXAxisPos1 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{AxisPos.rXAxisPos1}";
                    //public static string  rYAxisPos1 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{AxisPos.rYAxisPos1}";
                    //public static string rXAxisPos2 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{AxisPos.rXAxisPos2}";
                    //public static string  rYAxisPos2 => $"{NodeHeader}.{NodeVariable.system}.{HMI.sv_HMIIronPlateName}.{AxisPos.rYAxisPos2}";
                }



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
                /// 馬達按鈕啟動
                /// </summary>
                public static string sv_bButtonMotor => $"{NodeHeader}.{NodeVariable.Motor1}.{BButton.sv_bButtonMotor}";
                /// <summary>
                /// 馬達啟動
                /// </summary>
                public static string sv_bMotorStarted => $"{NodeHeader}.{NodeVariable.Motor1}.sv_bMotorStarted";
            }

            /// <summary>
            /// 裁切
            /// </summary>
            public class Cutting1
            {
                /// <summary>
                /// 磁簧訊號上限
                /// </summary>
                //public static string sv_bCuttingOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingOpen}";
                /// <summary>
                /// 磁簧訊號下限
                /// </summary>
                //public static string sv_bCuttingClosed => $"{NodeHeader}.{NodeVariable.Cutting1}.{BCutting.sv_bCuttingClosed}";
                /// <summary>
                /// 手動油壓缸升命令
                /// </summary>
                public static string sv_bButtonOpen => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonOpen}";
                /// <summary>
                /// 手動油壓缸降命令
                /// </summary>
                public static string sv_bButtonClose => $"{NodeHeader}.{NodeVariable.Cutting1}.{BButton.sv_bButtonClose}";


                /// <summary>
                /// 油壓缸原點
                /// </summary>
                public static string di_CuttingOrigin => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingOrigin";

                /// <summary>
                /// 油壓缸待命位置
                /// </summary>
                public static string di_CuttingStandbyPoint => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingStandbyPoint";

                /// <summary>
                /// 油壓缸切割位置
                /// </summary>
                public static string di_CuttingCutPoint => $"{NodeHeader}.{NodeVariable.Cutting1}.di_CuttingCutPoint";
                
    
            }  

            /// <summary>
            /// 潤滑
            /// </summary>
            public class Lubrication1
            {
                /// <summary>
                /// DO_潤滑系統ON/OFF
                /// </summary>
                public static string do_Lubrication => $"{NodeHeader}.{NodeVariable.Lubrication1}.do_Lubrication";
                /// <summary>
                /// SW_潤滑按鈕
                /// </summary>
                public static string sv_bButtonLubrication => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_bButtonLubrication";
             
                public class sv_LubricationSetValues
                {
                    /// <summary>
                    /// 潤滑設定時間
                    /// </summary>
                    public static string dLubTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dLubTime ";

                    /// <summary>
                    /// 潤滑開設定時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOnTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dOnTime ";
                    /// <summary>
                    /// 潤滑關設定時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOffTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationSetValues.dOffTime ";
                }



                public class sv_LubricationActValues
                {
                    /// <summary>
                    /// 潤滑實際時間
                    /// </summary>
                    public static string dLubTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dLubTime ";

                    /// <summary>
                    /// 潤滑開實際時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOnTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dOnTime ";
                    /// <summary>
                    /// 潤滑關實際時間
                    /// </summary>
                    /// <returns></returns>
                    public static string dOffTime => $"{NodeHeader}.{NodeVariable.Lubrication1}.sv_LubricationActValues.dOffTime ";
                }

            }







        }

        #endregion







    }
}
