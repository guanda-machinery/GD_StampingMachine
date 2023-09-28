using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static GD_MachineConnect.GD_Stamping_Opcua;

namespace GD_MachineConnect.Machine.Interfaces
{
    /// <summary>
    /// 鋼印機連線行為
    /// </summary>
    public partial interface IStampingMachineConnect
    {
        /// <summary>
        /// 是否已連接
        /// </summary>
       // bool IsConnected { get; }
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //bool Connect(string HostPath, int Port);

        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //bool Connect(string HostPath, int Port, string UserName, string Password);
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //bool Connect(string HostPath, int Port, string DataPath, string UserName , string Password );


        bool IsConnectSuccessfuＣonnection { get; }

        /// <summary>
        /// 離線
        /// </summary>
        //void Disconnect();
        //在這邊寫取得機台參數等功能
        bool GetMachineStatus(out Enums.MachineStatus Status);

        /// <summary>
        /// 取得機台設定
        /// </summary>
        /// <param name="MachanicalSpecification"></param>
        /// <returns></returns>
        bool GetMachanicalSpecification(out MachanicalSpecificationModel MachanicalSpecification);
        bool SetMachanicalSpecification(MachanicalSpecificationModel MachanicalSpecification);

        /// <summary>
        /// 取得所有鋼印字模
        /// </summary>
        /// <param name="StampingType"></param>
        /// <returns></returns>
        bool GetStampingTypeList(out ObservableCollection<StampingTypeModel> StampingTypeList);
        /// <summary>
        /// 設定所有鋼印字模
        /// </summary>
        /// <param name="StampingTypeList"></param>
        /// <returns></returns>
        bool SetStampingTypeList(ObservableCollection<StampingTypeModel> StampingTypeList);

        /// <summary>
        /// 取得單一字模
        /// </summary>
        /// <param name="StampingType"></param>
        /// <returns></returns>
        bool GetSingleStampingType(int Index, out StampingTypeModel StampingType);
        /// <summary>
        /// 設定單一字模
        /// </summary>
        /// <param name="StampingType"></param>
        /// <returns></returns>
        bool SetSingleStampingTypeList(int Index, StampingTypeModel StampingType);

        /// <summary>
        /// 取得軸向設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool GetAxisSetting(out AxisSettingModel AxisSetting);
        /// <summary>
        /// 設定軸向設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool SetAxisSetting(AxisSettingModel AxisSetting);

        /// <summary>
        /// 取得計時設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool GetTimingSetting(out TimingSettingModel TimingSetting);
        /// <summary>
        /// 設定計時設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool SetTimingSetting(TimingSettingModel TimingSetting);

        /// <summary>
        /// 設定箱子
        /// </summary>
        /// <returns></returns>
        bool SetSeparateBoxNumber(int boxIndex);

        /// <summary>
        /// 取得箱子編號
        /// </summary>
        /// <returns></returns>
        bool GetSeparateBoxNumber(out int Index);

        /// <summary>
        /// 取得分料設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool GetSeparateSetting(out SeparateSettingModel SeparateSetting);
        /// <summary>
        /// 設定分料設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool SetSeparateSetting(SeparateSettingModel SeparateSetting);

        /// <summary>
        /// 取得InputOutput
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool GetInputOutput(out InputOutputModel InputOutput);
        /// <summary>
        /// 設定InputOutput
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool SetInputOutput(InputOutputModel InputOutput);
        /// <summary>
        /// 取得工程模式
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool GetEngineerSetting(out EngineerSettingModel EngineerSetting);
        /// <summary>
        /// 設定工程模式
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        bool SetEngineerSetting(EngineerSettingModel EngineerSetting);
    }
 
    
    public partial interface IStampingMachineConnect
    {
        /// <summary>
        /// 取得馬達目前位置
        /// </summary>
        bool GetFeedingPosition(out float Position);
        /// <summary>
        /// 設定馬達目前位置
        /// </summary>
        bool SetFeedingPosition(float Position);
        /// <summary>
        /// 回歸基準點命令
        /// </summary>
        /// <returns></returns>
        bool FeedingPositionReturnToStandbyPosition();
        /// <summary>
        /// 進料手動前進
        /// </summary>
        /// <param name="Active">啟用</param>
        /// <returns></returns>
        bool FeedingPositionFwd(bool Active);
        /// <summary>
        /// 進料手動後退
        /// </summary>
        /// <param name="Active">啟用</param>
        /// <returns></returns>
        bool FeedingPositionBwd(bool Active);

        /// <summary>
        /// 取得氣壓/油壓缸實際位置(磁簧訊號)
        /// </summary>
        /// <returns></returns>
        bool GetCylinderActualPosition(StampingCylinderType stampingCylinder, DirectionsEnum direction ,out bool singal);


        /// <summary>
        /// 氣壓/油壓缸控制命令
        /// </summary>
        /// <returns></returns>
        bool Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction);

        /// <summary>
        /// 取得氣壓/油壓缸控制命令
        /// </summary>
        /// <param name="stampingCylinder">氣壓缸類型</param>
        /// <param name="direction">方向</param>
        /// <param name="status">現在值</param>
        /// <returns></returns>
        bool Get_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction , out bool status);


        /// <summary>
        /// 油壓單元控制
        /// </summary>
        bool SetHydraulicPumpMotor(bool Active);


        /// <summary>
        /// 取得油壓單元控制
        /// </summary>
        bool GetHydraulicPumpMotor(out bool isActive);



        /// <summary>
        /// 鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        bool GetRequestDatabit(out bool databit);

        /// <summary>
        /// 鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        bool SetRequestDatabit(bool databit);

        /// <summary>
        /// 取得鋼印字串
        /// </summary>
        bool GetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, out string StringLine);
        /// <summary>
        /// 設定鋼印字串
        /// </summary>
        bool SetIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine);

        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        //bool GetIronPlateGroup(out List<object> PlateGroup);


        /// <summary>
        /// 設定打點位置
        /// </summary>
        /// <param name="AxisPos"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool SetAxisPos(StampingOpcUANode.AxisPos AxisPos, string Pos);
        /// <summary>
        /// 取得打點位置
        /// </summary>
        /// <param name="AxisPos"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool GetAxisPos(StampingOpcUANode.AxisPos AxisPos, out string Pos);



        /// <summary>
        /// 鋼印Y軸現在位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool GetEngravingYAxisPosition(out float position);
        
        /// <summary>
        /// 鋼印Z軸現在位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool GetEngravingZAxisPosition(out float position);


        /// <summary>
        /// 鋼印油壓缸升起命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //bool GetEngravingZAxisHydraulicUp(out bool IsActived);
        /// <summary>
        /// 鋼印油壓缸下降命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //bool GetEngravingZAxisHydraulicDown(out bool IsActived);

        /// <summary>
        /// 鋼印油壓缸升起命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>

        //bool SetEngravingZAxisHydraulicUp(bool Actived);

        /// <summary>
        /// 鋼印油壓缸下降命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>

        //bool SetEngravingZAxisHydraulicDown(bool Actived);




        /// <summary>
        /// 鋼印Y軸回原點
        /// </summary>
        /// <returns></returns>
        bool SetEngravingYAxisToStandbyPos();


        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public bool GetEngravingYAxisBwd(out bool isActived);
        /// <summary>
        ///  鋼印Y軸前進
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public bool GetEngravingYAxisFwd(out bool isActived);




        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public bool SetEngravingYAxisBwd(bool Active);
        /// <summary>
        ///  鋼印Y軸前進
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public bool SetEngravingYAxisFwd(bool Active);

        /// <summary>
        /// 鋼印目前選定的字元「位置」（非字元）
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        bool GetEngravingRotateStation(out int stationIndex);
      
        /// <summary>
        /// 鋼印目前選定的字元
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        bool GetEngravingRotateStationChar(out char stationChar);






        /// <summary>
        /// 變更鋼印目前選定的字元命令
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        bool SetEngravingRotateStation(int Station);


        /// <summary>
        /// 取得轉盤上所有的字模
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        bool GetRotatingTurntableInfo(out List<char> fonts);
        /// <summary>
        /// 設定轉盤上單一字模
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        bool SetRotatingTurntableInfo(int index, char font);
        /// <summary>
        /// 設定轉盤上所有的字模
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        bool SetRotatingTurntableInfo(List<char> font);




        /// <summary>
        /// 鋼印轉盤順時針旋轉
        /// </summary>
        /// <returns></returns>
        bool SetEngravingRotateCW();
        /// <summary>
        /// 鋼印轉盤逆時針旋轉
        /// </summary>
        /// <returns></returns>
        bool SetEngravingRotateCCW();



        /// <summary>
        /// 取得進料x軸 homeing前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetFeedingXHomeFwdVelocity(out float SpeedPercent);
        /// <summary>
        /// 取得進料x軸 homeing後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetFeedingXHomeBwdVelocity(out float SpeedPercent);

        /// <summary>
        /// 設定進料x軸 homeing前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool SetFeedingXHomeFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 homein後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetFeedingXHomeBwdVelocity(float SpeedPercent);


        /// <summary>
        /// 取得進料x軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetFeedingXFwdSetupVelocity(out float SpeedPercent);
        /// <summary>
        /// 取得進料x軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetFeedingXBwdSetupVelocity(out float SpeedPercent);

        /// <summary>
        /// 設定進料x軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool SetFeedingXFwdSetupVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetFeedingXBwdSetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得進料x軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetFeedingXFwdVelocity(out float SpeedPercent);
        /// <summary>
        /// 取得進料x軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetFeedingXBwdVelocity(out float SpeedPercent);

        /// <summary>
        /// 設定進料x軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetFeedingXFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetFeedingXBwdVelocity(float SpeedPercent);





        /// <summary>
        /// 取得字碼刻印y軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetEngravingFeedingYFwdSetupVelocity(out float SpeedPercent);
        /// <summary>
        /// 取得字碼刻印y軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetEngravingFeedingYBwdSetupVelocity(out float SpeedPercent);

        /// <summary>
        /// 設定字碼刻印y軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool SetEngravingFeedingYFwdSetupVelocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印y軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingYBwdSetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得字碼刻印y軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetEngravingFeedingYFwdVelocity(out float SpeedPercent);
        /// <summary>
        /// 取得字碼刻印y軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetEngravingFeedingYBwdVelocity(out float SpeedPercent);

        /// <summary>
        /// 設定字碼刻印y軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingYFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印y軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingYBwdVelocity(float SpeedPercent);






        /// <summary>
        /// 取得字碼刻印A軸(旋轉) 設定模式移動速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        bool GetEngravingFeedingASetupVelocity(out float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 設定模式移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingASetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得字碼刻印y軸 手動/自動正轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetEngravingFeedingA_CW_Velocity(out float SpeedPercent);
        /// <summary>
        /// 取得字碼刻印y軸 手動/自動逆轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool GetEngravingFeedingA_CCW_Velocity(out float SpeedPercent);

        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 手動/自動正轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingA_CW_Velocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 手動/自動逆轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        bool SetEngravingFeedingA_CCW_Velocity(float SpeedPercent);

        //




    }










}
