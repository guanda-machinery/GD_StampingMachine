using GD_CommonLibrary.Method;
using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static GD_MachineConnect.GD_Stamping_Opcua.StampingOpcUANode;

namespace GD_MachineConnect
{
    public class GD_Stamping_Opcua : IStampingMachineConnect
    {
        private readonly GD_OpcUaHelperClient GD_OpcUaClient = new();


        public bool Connect(string HostPath, int Port)
        {
            return this.Connect(HostPath, Port, null, null, null);
        }
        public bool Connect(string HostPath, int Port, string UserName, string Password)
        {
            return this.Connect(HostPath, Port, null, UserName, Password);
        }

        public const int ConntectMillisecondsTimeout = 5000;
        public const int MoveTimeout = 5000;

        public bool Connect(string HostIP, int Port, string DataPath, string UserName, string Password)
        {
            bool Result = false;
            try
            {
                if (IPAddress.TryParse(HostIP, out var ipAddress))
                {
                    Ping myPing = new();
                    PingReply reply = myPing.Send(ipAddress, 1000);
                    if (reply == null)
                    {
                        return false;
                    }
                }

                string _hostPath = HostIP;
                if (!_hostPath.Contains(@"opc.tcp://"))
                {
                    _hostPath = @"opc.tcp://" + _hostPath;
                }
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                CancellationToken token = cancellationToken.Token;


                var ConnectTask = Task.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                        GD_OpcUaClient.UserIdentity = new UserIdentity(UserName, Password);

                    Result = await GD_OpcUaClient.OpcuaConnectAsync(_hostPath, Port, DataPath);
                }, token);

                //if (Task.WhenAny(ConnectTask, Task.Delay(MoveTimeout)) == ConnectTask)
                if (Task.WaitAny(ConnectTask, Task.Delay(ConntectMillisecondsTimeout)) == 0)
                {
                    ConnectTask.Wait();
                }
                else
                {
                    cancellationToken.Cancel();
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return Result;
        }
        public void Disconnect()
        {
            try
            {
                GD_OpcUaClient.Disconnect();
            }
            catch
            {

            }
        }

        public bool FeedingPositionBwd(bool Active)
        {
            GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, false);
            return GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, Active);
        }

        public bool FeedingPositionFwd(bool Active)
        {
            GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_bButtonBwd, false);
            return GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_bButtonFwd, Active);
        }

        public bool FeedingPositionReturnToStandbyPosition()
        {
            return GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_rServoStandbyPos, true);
            //throw new NotImplementedException();
        }

        public bool GetAxisSetting(out AxisSettingModel AxisSetting)
        {

            throw new NotImplementedException();
        }
        public bool GetFeedingPosition(out float Position)
        {
            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Feeding1.sv_rFeedingPosition, out Position);
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

            //return GD_OpcUaClient.ReadNode(StampingOpcUANode.Feeding1.sv_rFeedingPosition, out Position);
            return false;
        }

        public bool GetSeparateBoxNumber(out int Index)
        {
            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Stacking1.sv_iThisStation, out Index);
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

        public bool SetFeedingPosition(float Position)
        {
            if (CheckHydraulicPumpMotor())
            {
                return GD_OpcUaClient.WriteNode(StampingOpcUANode.Feeding1.sv_rServoMovePos, Position);
            }
            return false;
        }

        public bool SetInputOutput(InputOutputModel InputOutput)
        {
            throw new NotImplementedException();
        }

        public bool SetMachanicalSpecification(MachanicalSpecificationModel MachanicalSpecification)
        {
            throw new NotImplementedException();
        }

        public bool SetSeparateBoxNumber(int boxIndex)
        {
            return GD_OpcUaClient.WriteNode(StampingOpcUANode.Stacking1.sv_iThisStation, boxIndex);
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



        public bool Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction)
        {
            bool ret;
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, true);
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
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, true);
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
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, true);
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
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, true);
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
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonDown, true);
                            break;
                        default:
                            var O_ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, false);
                            var C_ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, false);
                            ret = O_ret && C_ret;
                            break;
                    }
                    break;
                case StampingCylinderType.HydraulicEngraving:
                    ret = GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopUp, out bool IsUp);
                    ret = GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StandbyPoint, out bool IsStandby);
                    ret = GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopDown, out bool IsDown);
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            if (IsUp)
                                return true;
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, true);
                            if (ret)
                            {
                                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                                CancellationToken token = cancellationToken.Token;
                                //開始運作 偵測是否到下一個節點
                                var ReadTask = Task.Run (async() =>
                                {
                                    while (true)
                                    {
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopUp, out bool _rIsUp);
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StandbyPoint, out bool _rIsStand);
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopDown, out bool _rIsDown);
                                     //從中間往上
                                        if(IsStandby)
                                        {
                                            if(_rIsUp)
                                                break;
                                        }
                                        //從下面往中或上
                                        else if (IsDown)
                                        {
                                            if(_rIsUp || _rIsStand)
                                                break;
                                        }
                                        //未知初始位置 看到東西就停
                                        else
                                        {
                                            if (_rIsUp || _rIsStand || _rIsDown)
                                                break;
                                        }
                                        await Task.Delay(10);
                                    }
                                }, token);


                                if (Task.WaitAny(ReadTask, Task.Delay(MoveTimeout)) == 0)
                                {
                                    ReadTask.Wait();
                                }
                                else
                                {
                                    cancellationToken.Cancel();
                                }
                            }
                            //等待到點
                            break;
                        case DirectionsEnum.Down:
                            if (IsDown)
                                return true;
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, true);
                            if (ret)
                            {
                                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                                CancellationToken token = cancellationToken.Token;
                                var ReadTask = Task.Run(async() =>
                                {
                                    while (true)
                                    {
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopUp, out bool _rIsUp);
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StandbyPoint, out bool _rIsStand);
                                        GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopDown, out bool _rIsDown);
                                        //從中間往下
                                        if (IsStandby)
                                        {
                                            if (_rIsDown)
                                                break;
                                        }
                                        //從上面往中或下
                                        else if (IsUp)
                                        {
                                            if ( _rIsStand || _rIsDown)
                                                break;
                                        }
                                        //未知初始位置 看到東西就停
                                        else
                                        {
                                            if (_rIsUp || _rIsStand || _rIsDown)
                                                break;
                                        }
                                        await Task.Delay(10);
                                    }
                                },token);


                                if (Task.WaitAny(ReadTask, Task.Delay(MoveTimeout)) == 0)
                                {
                                    ReadTask.Wait();
                                }
                                else
                                {
                                    cancellationToken.Cancel();
                                }

                            }
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                        ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                        if (ret)
                            break;
                    }
                    break;
                case StampingCylinderType.HydraulicCutting:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, false);
                            ret = GD_OpcUaClient.WriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, true);
                            break;
                        default:
                            ret = false;
                            break;
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return ret;
        }


        public bool Get_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction, out bool status)
        {
            status = false;
            bool ret;
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, out status),
                        _ => false,
                    };
                    break;
                case StampingCylinderType.GuideRod_Fixed:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, out status),
                        _ => false,
                    };
                    break;
                case StampingCylinderType.QRStamping:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bButtonUp, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bButtonDown, out status),
                        _ => false,
                    };
                    break;
                case StampingCylinderType.StampingSeat:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bButtonUp, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bButtonDown, out status),
                        _ => false,
                    };
                    break;
                case StampingCylinderType.BlockingCylinder:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, out status),
                        _ => false,
                    };
                    break;
                case StampingCylinderType.HydraulicCutting:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => GD_OpcUaClient.ReadNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, out status),
                        DirectionsEnum.Down => GD_OpcUaClient.ReadNode(StampingOpcUANode.Cutting1.sv_bButtonClose, out status),
                        _ => false,
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            return ret;
            //throw new NotImplementedException();
        }

        public bool GetCylinderActualPosition(StampingCylinderType stampingCylinder, DirectionsEnum direction, out bool singal)
        {
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureUp, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.GuideRod_Fixed:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureUp, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.QRStamping:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture1.sv_bFixtureUp, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture1.sv_bFixtureDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.StampingSeat:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bFixtureUp, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Fixture2.sv_bFixtureDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.BlockingCylinder:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.HydraulicEngraving:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopUp, out singal);
                        case DirectionsEnum.Middle:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StandbyPoint, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Engraving1.di_StopDown, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.HydraulicCutting:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Cutting1.sv_bCuttingOpen, out singal);
                        case DirectionsEnum.Down:
                            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Cutting1.sv_bCuttingClosed, out singal);
                        case DirectionsEnum.None:
                            singal = false;
                            return false;
                        default: throw new NotImplementedException();
                    }

                default: throw new NotImplementedException();
            }

        }


        public bool SetHydraulicPumpMotor(bool Active)
        {
            return GD_OpcUaClient.WriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, Active);
        }
        public bool GetHydraulicPumpMotor(out bool isActive)
        {
            return GD_OpcUaClient.ReadNode(StampingOpcUANode.Motor1.sv_bButtonMotor, out isActive);
        }

        /// <summary>
        /// 檢查油壓馬達 若未開啟則跳出選項詢問是否開啟
        /// </summary>
        /// <returns></returns>
        public bool CheckHydraulicPumpMotor()
        {
            if (GetHydraulicPumpMotor(out var MotorIsActived))
            {
                if (MotorIsActived)
                    return true;
                else
                {
                    //詢問後設定
                    //油壓馬達尚未啟動，是否要啟動油壓馬達？



                    var Result = MessageBoxResultShow.ShowYesNo((string)Application.Current.TryFindResource("Text_notify"),
                        (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorIsNotAcitved") +
                        "\r\n" +
                        (string)Application.Current.TryFindResource("Text_AskActiveHydraulicPumpMotor"));
                    if (Result == MessageBoxResult.Yes)
                    {
                        if (SetHydraulicPumpMotor(true))
                        {
                            return true;
                        }
                        else
                        {
                            MessageBoxResultShow.ShowOK((string)Application.Current.TryFindResource("Text_notify"),
                                (string)Application.Current.TryFindResource("Text_HydraulicPumpMotorAcitvedFailure"));
                        }
                    }


                }
            }

            return false;
        }



        public bool GetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, out string StringLine)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.system.sv_HMIIronPlateName}.{ironPlateType}", out StringLine);
        }
        public bool SetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine)
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.system.sv_HMIIronPlateName}.{ironPlateType}", StringLine);
        }

        public bool GetEngravingYAxisPosition(out float position)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.EngravingFeeding1.sv_rEngravingFeedingPosition}", out position);
        }

        public bool GetEngravingZAxisPosition(out float position)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.Engraving1.sv_rEngravingPosition}", out position);            
        }


       /* public bool GetEngravingZAxisHydraulicUp(out bool IsActived)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", out IsActived);
        }

        public bool GetEngravingZAxisHydraulicDown(out bool IsActived)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", out IsActived);
        }
        public bool SetEngravingZAxisHydraulicUp(bool Actived)
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonOpen}", Actived);
        }

        public bool SetEngravingZAxisHydraulicDown(bool Actived)
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.Engraving1.sv_bButtonClose}", Actived);
        }*/





















        public bool SetEngravingYAxisToStandbyPos()
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_rServoStandbyPos}", true);
        }
        

        public bool GetEngravingYAxisBwd(out bool isActived)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}", out isActived);
        }
        public bool GetEngravingYAxisFwd(out bool isActived)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}", out isActived);
        }

        public bool SetEngravingYAxisBwd(bool Active)
        {
            //檢查油壓
            if (Active && !CheckHydraulicPumpMotor())
                return false;

            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonBwd}", Active);
        }
        public bool SetEngravingYAxisFwd(bool Active)
        {
            //檢查油壓
            if (Active && !CheckHydraulicPumpMotor())
                return false;

            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_bButtonFwd}", Active);
        }




        public bool GetEngravingRotateStation(out int Station)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.system.sv_iTargetAStation}", out Station);
        }

        public bool SetEngravingRotateStation(int Station)
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station);
        }

        public bool SetEngravingRotateCW()
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCW}", true);
        }

        public bool SetEngravingRotateCCW()
        {
            return GD_OpcUaClient.WriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCCW}", true);
        }



        public bool GetSpeed(out double SpeedValue)
        {
            return GD_OpcUaClient.ReadNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwdSetup.Velocity.Output.rOutputValue}", out SpeedValue);
        }

        public bool SetSpeed(double SpeedValue)
        {
            //設定模式前進速度
            //EngravingFeeding1.sv_ConstFwdSetup.Velocity.Output.rOutputValue
            //設定模式後退速度
            //EngravingFeeding1.sv_ConstBwdSetup.Velocity.Output.rOutputValue

            //手動/自動前進速度
            // EngravingFeeding1.sv_ConstFwd.Velocity.Output.rOutputValue
            //手動/自動後退速度
            // EngravingFeeding1.sv_ConstBwd.Velocity.Output.rOutputValue
            return false;
        }






        public bool ReadNode<T>(string NodeTreeString, out T ReadValue)
        {
            return GD_OpcUaClient.ReadNode(NodeTreeString, out ReadValue);
        }
        public bool WriteNode<T>(string NodeTreeString, T WriteValue)
        {
            return GD_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
        }
            











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
          public  enum sIronPlate
            {
                sIronPlateName1,
                sIronPlateName2,
                sIronPlateName3
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

            /// <summary>
            /// 鋼印進料
            /// </summary>
            public class EngravingFeeding1
            {
                public class sv_ConstFwdSetup
                {
                    public class Velocity
                    {
                        public class Output
                        {
                            public static string rOutputValue => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.sv_ConstFwdSetup.Velocity.Output.rOutputValue";
                        }
                    }
                }



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

                /// <summary>
                /// 字串1(第一行)
                /// </summary>
                public static string sIronPlateName1 => $"{sv_HMIIronPlateName}.{sIronPlate.sIronPlateName1}";

                /// <summary>
                /// 字串2(第二行)
                /// </summary>
                public static string sIronPlateName2 => $"{sv_HMIIronPlateName}.{sIronPlate.sIronPlateName2}";


                /// <summary>
                /// 側邊字串
                /// </summary>
                public static string sIronPlateName3 => $"{sv_HMIIronPlateName}.{sIronPlate.sIronPlateName3}";


                /// <summary>
                /// 鐵片下一片資訊-交握訊號
                /// </summary>
                public static string sv_bRequestDatabit => $"{sv_HMIIronPlateName}.sv_bRequestDatabit";


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
