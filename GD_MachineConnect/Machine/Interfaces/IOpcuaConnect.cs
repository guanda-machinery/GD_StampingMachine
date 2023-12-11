using GD_CommonLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_MachineConnect.Machine.Interfaces
{
    /// <summary>
    /// 鋼印機連線行為
    /// </summary>
    public interface IOpcuaConnect
    {
        //public ClientState State { get; }
        bool IsConnected { get; }
        Task<bool> ConnectAsync(string hostPath, string user = null, string password = null);

        //Exception ConnectException { get;  }

        void Disconnect();

        Task DisconnectAsync();


        /// <summary>
        /// 寫入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        Task<bool> WriteNodeAsync(string NodeTreeString, object WriteValue);

        /// <summary>
        /// 寫入數列組
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>

        Task<IEnumerable<bool>> WriteNodesAsync(Dictionary<string, object> NodeTrees);

        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        Task<T> ReadNodeAsync<T>(string NodeID);

        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        Task<IEnumerable<object>> ReadNodesAsync(IEnumerable<string> NodeTrees);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <param name="updateAction"></param>
        /// <param name="samplingInterval"></param>
        /// <param name="checkDuplicates"></param>
        /// <returns></returns>
      //  Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates = false);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeList"></param>
        /// <returns></returns>
     //   Task<IList<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <param name="updateAction"></param>
        /// <param name="samplingInterval"></param>
        /// <param name="checkDuplicates"></param>
        /// <returns></returns>
        Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval =100, bool checkDuplicates = true);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList);



        /// <summary>
        /// 移掉訂閱的節點(使用響應速度分類)
        /// </summary>
        /// <param name="NodeID"></param>
        Task<bool> UnsubscribeNodeAsync(string NodeID, int samplingInterval);

    }
}











