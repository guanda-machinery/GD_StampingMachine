using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_MachineConnect.Machine.Interfaces
{
    /// <summary>
    /// 鋼印機連線行為
    /// </summary>
    public interface IStampingMachineConnect
    {
        /// <summary>
        /// 連線
        /// </summary>
        /// <param name="HostPath"></param>
        /// <param name="Port"></param>
        /// <param name="DataPath"></param>
        /// <returns></returns>
        bool Connect(string HostPath ,  int Port , string DataPath =null);
        /// <summary>
        /// 離線
        /// </summary>
        void Disconnect();
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
        bool GetSingleStampingType(int Index , out StampingTypeModel StampingType);
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
        bool SetSeparateBoxNumber(SeparateBoxModel SeparateBox);
       
        /// <summary>
        /// 取得箱子
        /// </summary>
        /// <returns></returns>
        bool GetSeparateBoxNumber(int Index ,out SeparateBoxModel SeparateBox);

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
}
