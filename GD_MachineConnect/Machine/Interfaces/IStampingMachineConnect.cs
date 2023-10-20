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
       // Task<bool>IsConnected { get; }
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //Task<bool>Connect(string HostPath, int Port);

        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //Task<bool>Connect(string HostPath, int Port, string UserName, string Password);
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        //Task<bool>Connect(string HostPath, int Port, string DataPath, string UserName , string Password );


        /// <summary>
        /// 離線
        /// </summary>
        //void Disconnect();
        //在這邊寫取得機台參數等功能
        Task<(bool, Enums.MachineStatus)> GetMachineStatus();

        /// <summary>
        /// 取得機台設定
        /// </summary>
        /// <param name="MachanicalSpecification"></param>
        /// <returns></returns>
        Task<(bool, MachanicalSpecificationModel)> GetMachanicalSpecification();
        Task<bool>SetMachanicalSpecification(MachanicalSpecificationModel MachanicalSpecification);


        /// <summary>
        /// 取得單一字模
        /// </summary>
        /// <param name="StampingType"></param>
        /// <returns></returns>
        Task<(bool, StampingTypeModel)> GetSingleStampingType(int Index);
        /// <summary>
        /// 設定單一字模
        /// </summary>
        /// <param name="StampingType"></param>
        /// <returns></returns>
        Task<bool>SetSingleStampingTypeList(int Index, StampingTypeModel StampingType);

        /// <summary>
        /// 取得軸向設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<(bool, AxisSettingModel)> GetAxisSetting();
        /// <summary>
        /// 設定軸向設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<bool>SetAxisSetting(AxisSettingModel AxisSetting);

        /// <summary>
        /// 取得計時設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<(bool, TimingSettingModel)> GetTimingSetting();
        /// <summary>
        /// 設定計時設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<bool>SetTimingSetting(TimingSettingModel TimingSetting);

        /// <summary>
        /// 設定箱子
        /// </summary>
        /// <returns></returns>
        Task<bool>SetSeparateBoxNumber(int boxIndex);

        /// <summary>
        /// 取得箱子編號
        /// </summary>
        /// <returns></returns>
        Task<(bool,int)>GetSeparateBoxNumber();

        /// <summary>
        /// 取得分料設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<(bool, SeparateSettingModel)> GetSeparateSetting();
        /// <summary>
        /// 設定分料設定
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<bool>SetSeparateSetting(SeparateSettingModel SeparateSetting);

        /// <summary>
        /// 取得InputOutput
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<(bool, InputOutputModel)> GetInputOutput();
        /// <summary>
        /// 設定InputOutput
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<bool>SetInputOutput(InputOutputModel InputOutput);
        /// <summary>
        /// 取得工程模式
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<(bool, EngineerSettingModel)> GetEngineerSetting();
        /// <summary>
        /// 設定工程模式
        /// </summary>
        /// <param name="AxisSetting"></param>
        /// <returns></returns>
        Task<bool>SetEngineerSetting(EngineerSettingModel EngineerSetting);
  //  }
 
    
    //public partial interface IStampingMachineConnect
   // {

        Task<bool> AsyncConnect();
         Exception ConnectException { get; }
        void Disconnect();
        /// <summary>
        /// 取得馬達目前位置
        /// </summary>
        Task<(bool, float)> GetFeedingPosition();
        /// <summary>
        /// 設定馬達目前位置
        /// </summary>
        Task<bool>SetFeedingPosition(float Position);
        /// <summary>
        /// 回歸基準點命令
        /// </summary>
        /// <returns></returns>
        Task<bool>FeedingPositionReturnToStandbyPosition();
        /// <summary>
        /// 進料手動前進
        /// </summary>
        /// <param name="Active">啟用</param>
        /// <returns></returns>
        Task<bool>FeedingPositionFwd(bool Active);
        /// <summary>
        /// 進料手動後退
        /// </summary>
        /// <param name="Active">啟用</param>
        /// <returns></returns>
        Task<bool>FeedingPositionBwd(bool Active);

        /// <summary>
        /// 取得氣壓/油壓缸實際位置(磁簧訊號)
        /// </summary>
        /// <returns></returns>
        Task<(bool,bool)>GetCylinderActualPosition(StampingCylinderType stampingCylinder, DirectionsEnum direction);


        /// <summary>
        /// 氣壓/油壓缸控制命令
        /// </summary>
        /// <returns></returns>
        Task<bool>Set_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction);

        /// <summary>
        /// 取得氣壓/油壓缸控制命令
        /// </summary>
        /// <param name="stampingCylinder">氣壓缸類型</param>
        /// <param name="direction">方向</param>
        /// <param name="status">現在值</param>
        /// <returns></returns>
        Task<(bool,bool)>Get_IO_CylinderControl(StampingCylinderType stampingCylinder, DirectionsEnum direction);


        /// <summary>
        /// 油壓單元控制
        /// </summary>
        Task<bool>SetHydraulicPumpMotor(bool Active);


        /// <summary>
        /// 取得油壓單元控制
        /// </summary>
       Task<(bool, bool)> GetHydraulicPumpMotor();



        /// <summary>
        /// 鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        Task<(bool, bool)> GetRequestDatabit();

        /// <summary>
        /// 鐵片下一片資訊-交握訊號
        /// </summary>
        /// <param name="databit"></param>
        /// <returns></returns>
        Task<bool>SetRequestDatabit(bool databit);






        /// <summary>
        /// 取得下一個鋼印字串
        /// </summary>
       // Task<(bool, string)> GetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType);
        /// <summary>
        /// 設定下一個鋼印字串
        /// </summary>
        //Task<bool>SetHMIIronPlateName(StampingOpcUANode.sIronPlate ironPlateType, string StringLine);

        /// <summary>
        /// 取得下一個鋼印
        /// </summary>

        Task<(bool, IronPlateDataModel)> GetHMIIronPlate();
        /// <summary>
        /// 設定下一個鋼印
        /// </summary>
        Task<bool> SetHMIIronPlate(IronPlateDataModel ironPlateDataList);

        /// <summary>
        /// 是否要使用QR
        /// </summary>
        /// <param name="IsUse"></param>
        /// <returns></returns>
        Task<bool> SetDataMatrixMode(bool IsUse);


        /// <summary>
        /// 取得鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        Task<(bool, List<IronPlateDataModel>) >GetIronPlateDataCollection();
        /// <summary>
        /// 設定鐵片群資訊
        /// </summary>
        /// <param name="ironPlateType"></param>
        /// <param name="StringLine"></param>
        /// <returns></returns>
        Task<bool> SetIronPlateDataCollection(List<IronPlateDataModel> ironPlateDataList);





        /// <summary>
        /// 鋼印Y軸現在位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingYAxisPosition();
        
        /// <summary>
        /// 鋼印Z軸現在位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingZAxisPosition();


        /// <summary>
        /// 鋼印油壓缸升起命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //Task<bool>GetEngravingZAxisHydraulicUp(out Task<bool>IsActived);
        /// <summary>
        /// 鋼印油壓缸下降命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //Task<bool>GetEngravingZAxisHydraulicDown(out Task<bool>IsActived);

        /// <summary>
        /// 鋼印油壓缸升起命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>

        //Task<bool>SetEngravingZAxisHydraulicUp(bool Actived);

        /// <summary>
        /// 鋼印油壓缸下降命令
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>

        //Task<bool>SetEngravingZAxisHydraulicDown(bool Actived);




        /// <summary>
        /// 鋼印Y軸回原點
        /// </summary>
        /// <returns></returns>
        Task<bool>SetEngravingYAxisToStandbyPos();


        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public Task<(bool,bool)>GetEngravingYAxisBwd();
        /// <summary>
        ///  鋼印Y軸前進
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public Task<(bool, bool)> GetEngravingYAxisFwd();




        /// <summary>
        /// 鋼印Y軸後退
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public Task<bool>SetEngravingYAxisBwd(bool Active);
        /// <summary>
        ///  鋼印Y軸前進
        /// </summary>
        /// <param name="Active"></param>
        /// <returns></returns>
        public Task<bool>SetEngravingYAxisFwd(bool Active);

        /// <summary>
        /// 鋼印目前選定的字元「位置」（非字元）
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        Task<(bool, int)> GetEngravingRotateStation();
      
        /// <summary>
        /// 取得鋼印目前選定的字元
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        Task<(bool, StampingTypeModel)> GetEngravingRotateStationChar();






        /// <summary>
        /// 變更鋼印目前選定的字元命令
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        Task<bool>SetEngravingRotateStation(int Station);


        /// <summary>
        /// 取得轉盤上所有的字模 編號-字元
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        Task<(bool, List<StampingTypeModel>)> GetRotatingTurntableInfo();
        /// <summary>
        /// 設定轉盤上單一字模
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        Task<bool>SetRotatingTurntableInfo(int index, char font);
        /// <summary>
        /// 設定轉盤上所有的字模
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        Task<bool>SetRotatingTurntableInfo(List<StampingTypeModel> font);



        /// <summary>
        /// 鋼印轉盤順時針旋轉
        /// </summary>
        /// <returns></returns>
        Task<bool>SetEngravingRotateCW();
        /// <summary>
        /// 鋼印轉盤逆時針旋轉
        /// </summary>
        /// <returns></returns>
        Task<bool>SetEngravingRotateCCW();



        /// <summary>
        /// 取得進料x軸 homeing前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXHomeFwdVelocity();
        /// <summary>
        /// 取得進料x軸 homeing後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXHomeBwdVelocity();

        /// <summary>
        /// 設定進料x軸 homeing前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXHomeFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 homein後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXHomeBwdVelocity(float SpeedPercent);


        /// <summary>
        /// 取得進料x軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXFwdSetupVelocity();
        /// <summary>
        /// 取得進料x軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXBwdSetupVelocity();

        /// <summary>
        /// 設定進料x軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXFwdSetupVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXBwdSetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得進料x軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXFwdVelocity();
        /// <summary>
        /// 取得進料x軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetFeedingXBwdVelocity();

        /// <summary>
        /// 設定進料x軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定進料x軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetFeedingXBwdVelocity(float SpeedPercent);





        /// <summary>
        /// 取得字碼刻印y軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingYFwdSetupVelocity();
        /// <summary>
        /// 取得字碼刻印y軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingYBwdSetupVelocity();

        /// <summary>
        /// 設定字碼刻印y軸 設定模式前進速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingYFwdSetupVelocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印y軸 設定模式後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingYBwdSetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得字碼刻印y軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingYFwdVelocity();
        /// <summary>
        /// 取得字碼刻印y軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingYBwdVelocity();

        /// <summary>
        /// 設定字碼刻印y軸 手動/自動前進速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingYFwdVelocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印y軸 手動/自動後退速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingYBwdVelocity(float SpeedPercent);






        /// <summary>
        /// 取得字碼刻印A軸(旋轉) 設定模式移動速度
        /// </summary>
        /// <param name="SpeedValue"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingASetupVelocity();
        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 設定模式移動速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingASetupVelocity(float SpeedPercent);



        /// <summary>
        /// 取得字碼刻印y軸 手動/自動正轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingA_CW_Velocity();
        /// <summary>
        /// 取得字碼刻印y軸 手動/自動逆轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<(bool, float)> GetEngravingFeedingA_CCW_Velocity();

        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 手動/自動正轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingA_CW_Velocity(float SpeedPercent);
        /// <summary>
        /// 設定字碼刻印A軸(旋轉) 手動/自動逆轉速度
        /// </summary>
        /// <param name="SpeedPercent"></param>
        /// <returns></returns>
        Task<bool>SetEngravingFeedingA_CCW_Velocity(float SpeedPercent);

        //




    }










}
