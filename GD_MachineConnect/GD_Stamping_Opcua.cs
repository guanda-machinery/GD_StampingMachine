using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_MachineConnect.Enums;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model.KEBA_Model;
using Opc.Ua;
using OpcUaHelper;
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

namespace GD_MachineConnect
{
    public class GD_Stamping_Opcua : IStampingMachineConnect , IDisposable
    {
        /// <summary>
        /// 實作長連接
        /// </summary>
        private readonly GD_OpcUaHelperClient GD_OpcUaClient;
        ~GD_Stamping_Opcua()
        {
            Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
        }

        private bool disposed = false;
        public void Dispose(bool disposing)
        {
            if (!this.disposed)      //如果Dispose沒有成功執行過
            {
                try
                {
                    if (disposing)   //釋放Managed
                    {

                    }
                    this.Disconnect();
                    //釋放Unmanaged資源
                    disposed = true;
                    GC.SuppressFinalize(this);
                }
                catch (Exception ex)
                {

                }
                finally
                {

                }

            }
        }

        public const int ConntectMillisecondsTimeout = 1000;
        public const int MoveTimeout = 5000;




        public GD_Stamping_Opcua(string HostIP, int Port, string DataPath, string UserName, string Password)
        {
            IUserIdentity UserIdentity = new UserIdentity(UserName, Password);
            GD_OpcUaClient = new GD_OpcUaHelperClient(HostIP, Port, DataPath, UserIdentity);
        }
        public GD_Stamping_Opcua(string HostIP, string DataPath, string UserName, string Password)
        {
            IUserIdentity UserIdentity = new UserIdentity(UserName, Password);
            GD_OpcUaClient = new GD_OpcUaHelperClient(HostIP, null, DataPath, UserIdentity);
        }
        public async Task<bool> AsyncConnect()
        {
           return await GD_OpcUaClient.AsyncConnect();
        }
        public Exception ConnectException { get => GD_OpcUaClient.ConnectException; }

        public void Disconnect()
        {
            GD_OpcUaClient.Disconnect();
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
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Feeding1.sv_rServoStandbyPos, true);
        }



        public async Task<(bool, AxisSettingModel)> GetAxisSetting()
        {
           throw new NotImplementedException();
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



        public async Task<bool> Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction)
        {
            bool ret;
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown, true);
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
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown, true);
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
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture1.sv_bButtonDown, true);
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
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonUp, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Fixture2.sv_bButtonDown, true);
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
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonUp, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bButtonDown, true);
                            break;
                        default:
                            var O_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp, false);
                            var C_ret = GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown, false);
                            ret = await O_ret && await C_ret;
                            break;
                    }
                    break;
                case StampingCylinderType.HydraulicEngraving:
                    var IsUp = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
                    var IsStandby = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
                    var IsDown = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            if (IsUp.Item2)
                                return true;
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, true);
                            if (ret)
                            {
                                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                                CancellationToken token = cancellationToken.Token;
                                //開始運作 偵測是否到下一個節點
                                var ReadTask = Task.Run (async() =>
                                {
                                    while (true)
                                    {
                                        var _rIsUp = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
                                        var _rIsStand = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
                                        var _rIsDown = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
                                        //從中間往上
                                        if (IsStandby.Item2)
                                        {
                                            if(_rIsUp.Item2)
                                                break;
                                        }
                                        //從下面往中或上
                                        else if (IsDown.Item2)
                                        {
                                            if(_rIsUp.Item2 || _rIsStand.Item2)
                                                break;
                                        }
                                        //未知初始位置 看到東西就停
                                        else
                                        {
                                            if (_rIsUp.Item2 || _rIsStand.Item2 || _rIsDown.Item2)
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
                            if (IsDown.Item2)
                                return true;
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, true);
                            if (ret)
                            {
                                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                                CancellationToken token = cancellationToken.Token;
                                var ReadTask = Task.Run(async() =>
                                {
                                    while (true)
                                    {
                                        var _rIsUp = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
                                        var _rIsStand = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
                                        var _rIsDown = await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
                                        //從中間往下
                                        if (IsStandby.Item2)
                                        {
                                            if (_rIsDown.Item2)
                                                break;
                                        }
                                        //從上面往中或下
                                        else if (IsUp.Item2)
                                        {
                                            if ( _rIsStand.Item2 || _rIsDown.Item2)
                                                break;
                                        }
                                        //未知初始位置 看到東西就停
                                        else
                                        {
                                            if (_rIsUp.Item2 || _rIsStand.Item2 || _rIsDown.Item2)
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
                        ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonOpen, false);
                        ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Engraving1.sv_bButtonClose, false);
                        if (ret)
                            break;
                    }
                    break;
                case StampingCylinderType.HydraulicCutting:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, true);
                            break;
                        case DirectionsEnum.Down:
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonOpen, false);
                            ret = await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Cutting1.sv_bButtonClose, true);
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


        public async Task<(bool,bool)> Get_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction)
        {
            (bool,bool) ret;
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bButtonUp),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bButtonDown),
                        _ => (false, false),
                    };
                    break;
                case StampingCylinderType.GuideRod_Fixed:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bButtonUp),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bButtonDown),
                        _ => (false, false),
                    };
                    break;
                case StampingCylinderType.QRStamping:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bButtonUp),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bButtonDown),
                        _ => (false, false),
                    };
                    break;
                case StampingCylinderType.StampingSeat:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bButtonUp),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bButtonDown),
                        _ => (false, false),
                    };
                    break;
                case StampingCylinderType.BlockingCylinder:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown),
                        _ => (false,false),
                    };
                    break;
                case StampingCylinderType.HydraulicCutting:
                    ret = direction switch
                    {
                        DirectionsEnum.Up => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.sv_bButtonOpen),
                        DirectionsEnum.Down => await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.sv_bButtonClose),
                        _ => (false, false),
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            return ret;
            //throw new NotImplementedException();
        }

        public async Task<(bool, bool)> GetCylinderActualPosition(StampingCylinderType stampingCylinder, DirectionsEnum direction)
        {
            switch (stampingCylinder)
            {
                case StampingCylinderType.GuideRod_Move:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureUp);
                        case DirectionsEnum.Down:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture1.sv_bGuideRodsFixtureDown);
                        case DirectionsEnum.None:
                            return (false, false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.GuideRod_Fixed:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureUp);
                        case DirectionsEnum.Down:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.GuideRodsFixture2.sv_bGuideRodsFixtureDown);
                        case DirectionsEnum.None:
                            return (false, false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.QRStamping:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureUp);
                        case DirectionsEnum.Down:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture1.sv_bFixtureDown);
                        case DirectionsEnum.None:
                            return (false,false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.StampingSeat:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureUp);
                        case DirectionsEnum.Down:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Fixture2.sv_bFixtureDown);
                        case DirectionsEnum.None:
                            return (false, false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.BlockingCylinder:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsUp);
                        case DirectionsEnum.Down:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.BlockingClips1.sv_bBlockingClipsDown);
                        case DirectionsEnum.None:
                            return (false,false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.HydraulicEngraving:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopUp);
                        case DirectionsEnum.Middle:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StandbyPoint);
                        case DirectionsEnum.Down:
                            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Engraving1.di_StopDown);
                        case DirectionsEnum.None:
                            return (false, false);
                        default: throw new NotImplementedException();
                    }

                case StampingCylinderType.HydraulicCutting:
                    switch (direction)
                    {
                        case DirectionsEnum.Up:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.sv_bCuttingOpen);
                        case DirectionsEnum.Down:
                       return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Cutting1.sv_bCuttingClosed);
                        case DirectionsEnum.None:
                            return (false, false);
                        default: throw new NotImplementedException();
                    }

                default: throw new NotImplementedException();
            }

        }


        public async Task<bool> SetHydraulicPumpMotor(bool Active)
        {
            return await GD_OpcUaClient.AsyncWriteNode(StampingOpcUANode.Motor1.sv_bButtonMotor, Active);
        }
        public async Task<(bool, bool)> GetHydraulicPumpMotor()
        {
            return await GD_OpcUaClient.AsyncReadNode<bool>(StampingOpcUANode.Motor1.sv_bButtonMotor);
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
        /// 取得下一片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<(bool,string)> GetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType)
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}");
        }

        /// <summary>
        /// 設定下一片資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<bool> SetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}", StringLine);
        }

        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
       /* public async Task<bool> GetIronPlateGroup(out List<object> PlateGroup)
        {
            PlateGroup = new List<object> { };
       return await GD_OpcUaClient.AsyncReadNode($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{ironPlateType}", out PlateGroup);
        }*/


        /// <summary>
        /// 設定打點位置
        /// </summary>
        /// <param name="AxisPos"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<bool> SetAxisPos(StampingOpcUANode.AxisPos AxisPos, string Pos)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{AxisPos}", Pos);
        }

        /// <summary>
        ///取得打點位置
        /// </summary>
        /// <param name="AxisPos"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        public async Task<(bool, string)> GetAxisPos(StampingOpcUANode.AxisPos AxisPos)
        {
            return await GD_OpcUaClient.AsyncReadNode<string>($"{StampingOpcUANode.system.sv_HMIIronPlateName.NodeName}.{AxisPos}");
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
            var ret = await GD_OpcUaClient.AsyncReadNode<int>($"{StampingOpcUANode.system.sv_iTargetAStation}");

            if (ret.Item1)
            {
                //※須注意 目前使用的函式回傳index時是1為起始 所以需-1才能符合其他位置
                var StationIndex = ret.Item2;
                Station = StationIndex - 1;
            }

            return (ret.Item1 , Station);
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



        public async Task<bool> SetEngravingRotateStation(int Station)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);
            //return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_iTargetAStation}", Station + 1);
        }

        private async Task<(bool,int[])> GetRotatingTurntableInfoINT() 
            =>await  GD_OpcUaClient.AsyncReadNode<int[]>($"{StampingOpcUANode.system.sv_RotateCodeDefinition}");
        
        private async Task<bool> SetRotatingTurntableInfoINT(int[] fonts)     
            => await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.system.sv_RotateCodeDefinition}", fonts);





        public async Task<(bool, List<IronPlateDataModel>)> GetIronPlateDataCollection()
        {
            for(int i = 0; i < 25; i++)
            {
                var a = await GD_OpcUaClient.AsyncReadNode<object>($"{StampingOpcUANode.system.sv_IronPlateData}.[{i+1}]");




                var c = await GD_OpcUaClient.AsyncReadNode<IronPlateDataModel>($"{StampingOpcUANode.system.sv_IronPlateData}[{i + 1}]");

                var newIronPlateData = new IronPlateDataModel();






                
            }
            return (false, null); 
        }





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




        public async Task<bool> SetEngravingRotateCW()
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCW}", true);
        }

        public async Task<bool> SetEngravingRotateCCW()
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_bButtonCCW}", true);
        }



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

        public async Task<(bool,float)> GetFeedingXFwdSetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstFwdSetup}");
        }

        public async Task<(bool, float)> GetFeedingXBwdSetupVelocity()
        {

       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstBwdSetup}");
        }

        public async Task<bool> SetFeedingXFwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstFwdSetup}", SpeedPercent);
        }


        public async Task<bool> SetFeedingXBwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstBwdSetup}", SpeedPercent);

        }

        public async Task<(bool,float)> GetFeedingXFwdVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstFwd}");
        }

        public async Task<(bool, float)> GetFeedingXBwdVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.Feeding1.sv_ConstBwd}");
        }

        public async Task<bool> SetFeedingXFwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstFwd}", SpeedPercent);
        }

        public async Task<bool> SetFeedingXBwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.Feeding1.sv_ConstBwd}", SpeedPercent);
        }

        public async Task<(bool, float)> GetEngravingFeedingYFwdSetupVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwdSetup}");

        }

        public async Task<(bool, float)> GetEngravingFeedingYBwdSetupVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwdSetup}");

        }

        public async Task<bool> SetEngravingFeedingYFwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwdSetup}",  SpeedPercent);

        }

        public async Task<bool> SetEngravingFeedingYBwdSetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwdSetup}",  SpeedPercent);
        }

        public async Task<(bool, float)> GetEngravingFeedingYFwdVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwd}");
        }

        public async Task<(bool, float)> GetEngravingFeedingYBwdVelocity()
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwd}");
        }

        public async Task<bool> SetEngravingFeedingYFwdVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstFwd}", SpeedPercent);

        }

        public async Task<bool> SetEngravingFeedingYBwdVelocity(float SpeedPercent)
        {

            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingFeeding1.sv_ConstBwd}", SpeedPercent);
        }



        public async Task<(bool,float)> GetEngravingFeedingASetupVelocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateSetup}");
        }

        public async Task<bool> SetEngravingFeedingASetupVelocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateSetup}", SpeedPercent);

        }

        public async Task<(bool, float)> GetEngravingFeedingA_CW_Velocity( )
        {
       return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCW}");

        }

        public async Task<(bool, float)> GetEngravingFeedingA_CCW_Velocity()
        {
            return await GD_OpcUaClient.AsyncReadNode<float>($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCCW}");
        }

        public async Task<bool> SetEngravingFeedingA_CW_Velocity(float SpeedPercent)
        {
            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCW}", SpeedPercent);
        }

        public async Task<bool> SetEngravingFeedingA_CCW_Velocity(float SpeedPercent)
        {

            return await GD_OpcUaClient.AsyncWriteNode($"{StampingOpcUANode.EngravingRotate1.sv_ConstRotateCCW}", SpeedPercent);
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
                public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.Feeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";


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

            public const string VelocityNode = "Velocity.Output.rOutputValue";

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

                /// <summary>
                /// 設定模式前進速度
                /// </summary>       
                public static string sv_ConstFwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwdSetup}.{VelocityNode}";
                /// <summary>
                /// 設定模式後退速度
                /// </summary>
                public static string sv_ConstBwdSetup => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwdSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動前進速度
                /// </summary>
                public static string sv_ConstFwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstFwd}.{VelocityNode}";
                /// <summary>
                /// 手動/自動後退速度
                /// </summary>
                public static string sv_ConstBwd => $"{NodeHeader}.{NodeVariable.EngravingFeeding1}.{ConstSpeedSetup.sv_ConstBwd}.{VelocityNode}";



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
                public static string sv_ConstRotateSetup => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateSetup}.{VelocityNode}";
                /// <summary>
                /// 手動/自動正轉速度
                /// </summary>
                public static string sv_ConstRotateCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";
                /// <summary>
                /// 手動/自動逆轉速度
                /// </summary>
                public static string sv_ConstRotateCCW => $"{NodeHeader}.{NodeVariable.EngravingRotate1}.{ConstSpeedSetup.sv_ConstRotateCCW}.{VelocityNode}";




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
