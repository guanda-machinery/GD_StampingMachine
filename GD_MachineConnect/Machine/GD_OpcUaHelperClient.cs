using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GD_CommonLibrary.Extensions;
using GD_MachineConnect.Machine.Interfaces;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUaHelper;

namespace GD_MachineConnect.Machine
{

    //https://www.cnblogs.com/daji2020/p/16627627.html
    [Obsolete]
    public class GD_OpcUaHelperClient: IOpcuaConnect
    {
        private OpcUaClient m_OpcUaClient = new OpcUaClient();

        public int ConntectMillisecondsTimeout = 1000;


        private const int retryCounter = 10;


        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task<bool> ConnectAsync(string hostPath, string user = null, string password = null)
        {
        
            if   (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                m_OpcUaClient.UserIdentity = new Opc.Ua.UserIdentity(user, password);

            if (!m_OpcUaClient.Connected)
            {
                using (var cts = new CancellationTokenSource(3000))
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync(cts.Token);
                    }
                    catch (OperationCanceledException cex)
                    {
                        return false;
                    }
                }
                try
                {
                    if (!m_OpcUaClient.Connected)
                    {
                        await m_OpcUaClient.ConnectServer(hostPath.ToString());
                        ConnectException = null;
                    }
                }
                catch (Exception ex)
                {
                    ConnectException = ex;
                    if (m_OpcUaClient.Connected)
                        m_OpcUaClient.Disconnect();
                }
                finally
                {
                }
                semaphoreSlim.Release();
            }
            return m_OpcUaClient.Connected;
        }


        public Exception ConnectException { get;private set; }

        public void Disconnect()
        {
            m_OpcUaClient.Disconnect();
        }

        public async Task DisconnectAsync()
        {
            m_OpcUaClient.Disconnect();
            if(m_OpcUaClient!=null)
                await WaitForCondition.WaitAsync(()=>m_OpcUaClient.Connected, false);
        }

        /*
    public async Task Disconnect()
    {
        m_OpcUaClient.Disconnect();
        await Task.Run(async () => 
        {
            var tcs = new TaskCompletionSource<bool>();
            var cts = new CancellationTokenSource();
            _ = MonitorConditionAsync(tcs, cts.Token);
            await tcs.Task;
        });
    }

    private async Task MonitorConditionAsync(TaskCompletionSource<bool> tcs, CancellationToken cancellationToken)
    {
        try
        {
            while (!tcs.Task.IsCompleted)
            {
                // 模拟异步操作
                await Task.Delay(100);

                // 检查条件是否已满足
                if (!m_OpcUaClient.Connected)
                {
                    tcs.SetResult(true);
                    break;
                }
                // 检查取消标记
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException)
        {
            // 操作被取消
            tcs.SetCanceled();
        }
        catch (Exception ex)
        {
            // 处理其他异常
            tcs.SetException(ex);
        }
    }
    */



        //public ClientState State =>

        //private bool _isConnected;
        public bool IsConnected => m_OpcUaClient.Connected;



        /// <summary>
        /// 寫入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public async Task<bool> WriteNodeAsync(string NodeTreeString, object WriteValue)
        {
            var ret = false;

            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                        ret = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
                    }
                    if (ret)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    await Task.Delay(10);
                    //Disconnect();
                }
            }

            return ret;
        }


        /// <summary>
        /// 寫入數列組
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public async Task<IEnumerable<bool>> WriteNodesAsync(Dictionary<string,object> NodeTrees)
        {
            if (NodeTrees.Count == 0)
                return new List<bool>();

            var ret = false;
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                            var tags = NodeTrees.Keys.ToArray();
                            var values = NodeTrees.Values.ToArray();
                            ret = m_OpcUaClient.WriteNodes(tags, values);
                    }

                    if (ret)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    await Task.Delay(10);
                }
            }
            return new List<bool>() { ret };
        }







        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public async Task<T> ReadNodeAsync<T>(string NodeID)
        {
            return await m_OpcUaClient.ReadNodeAsync<T>(NodeID);
        }



        public async Task<IEnumerable<T>> ReadNodesAsync<T>(IEnumerable<string> NodeTrees)
        {
            return await m_OpcUaClient.ReadNodesAsync<T>(NodeTrees.ToArray());
        }

        private readonly SemaphoreSlim subscribeSemaphoreSlim = new SemaphoreSlim(1, 1);


        public async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates = true)
        {
            var ret = await SubscribeNodesDataChangeAsync<T>(new List<(string, Action<T>, int, bool checkDuplicates)>
            {
                (NodeID, updateAction , samplingInterval , checkDuplicates)
            });
            return ret.FirstOrDefault();
        }





        public async Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList)
        {
            return await Task.Run(async () =>
            {
                List<bool> retList = new List<bool>();
                await subscribeSemaphoreSlim.WaitAsync();
                try
                {
                    for (int i = 0; i < nodeList.Count(); i++)
                    {
                        var index = i;
                        var (NodeID, updateAction, samplingInterval, checkDuplicates) = nodeList[index];
                        bool ret = false;
                        for (int j = 0; j < 5 && !ret; j++)
                        {
                            try
                            {
                                //將響應時間當作訂閱的名稱進行分類
                                string skey = samplingInterval.ToString();

                                Subscription opcSubscription = m_OpcUaClient.Session.Subscriptions.FirstOrDefault(x => x.DisplayName == skey);
                                if (opcSubscription == null)
                                {
                                    opcSubscription = new Subscription(m_OpcUaClient.Session.DefaultSubscription);
                                    opcSubscription.PublishingEnabled = true;
                                    opcSubscription.KeepAliveCount = uint.MaxValue;
                                    opcSubscription.LifetimeCount = uint.MaxValue;
                                    opcSubscription.MaxNotificationsPerPublish = uint.MaxValue;
                                    opcSubscription.Priority = 100;
                                    opcSubscription.PublishingInterval = samplingInterval;
                                    opcSubscription.DisplayName = samplingInterval.ToString();

                                    //已經存在的訂閱就不再創造
                                    m_OpcUaClient.Session.AddSubscription(opcSubscription);
                                    opcSubscription.Create();
                                }

                                if (checkDuplicates && opcSubscription.MonitoredItems.Any(x => x.StartNodeId == new NodeId(NodeID)))
                                {
                                    //已經有訂閱了
                                    ret = true;
                                    break;
                                }

                                var item = new MonitoredItem();
                                item.StartNodeId = new NodeId(NodeID);
                                item.SamplingInterval = samplingInterval;

                                item.Notification += (sender, e) =>
                                {
                                    //將action返還值
                                    try
                                    {
                                        if (e.NotificationValue is Opc.Ua.MonitoredItemNotification notification)
                                        {
                                            if (notification.Value.Value is T Tvalue)
                                            {
                                                updateAction?.Invoke(Tvalue);
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                };





                                // You can set your own values on the "Tag" property
                                // that allows you to identify the source later.
                                //   item.Tag = index;
                                // Set a custom sampling interval on the 
                                // monitored item.

                                opcSubscription.AddItem(item);
                                // Add the item to the subscription.
                                // opcSubscription.AddMonitoredItem(item);
                                // After adding the items (or configuring the subscription), apply the changes.
                                CancellationTokenSource cts = new CancellationTokenSource(10000);
                                //await WaitForCondition.WaitAsync(() => m_OpcUaClient.State, OpcClientState.Connected, cts.Token);


                                opcSubscription.ApplyChanges();

                                break;
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                Debug.WriteLine(ex.ToString());
                            }
                        }
                        retList.Add(ret);
                    }

                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    Debug.WriteLine(ex.ToString());
                }
                subscribeSemaphoreSlim.Release();
                return retList;
            });
        }


        public async Task<bool> UnsubscribeNodeAsync(string NodeID, int samplingInterval)
        {

            return await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        Subscription opcSubscription = m_OpcUaClient.Session.Subscriptions.FirstOrDefault(x => Equals(x.DisplayName , samplingInterval.ToString()));
                        if (opcSubscription != null)
                        {
                            List<MonitoredItem> removeItems = new List<MonitoredItem>();
                            foreach (var monitoredItem in opcSubscription.MonitoredItems)
                            {
                                if (new NodeId(NodeID) == monitoredItem.StartNodeId)
                                {
                                    removeItems.Add(monitoredItem);
                                }
                            }
                            foreach (var mitem in removeItems)
                            {
                                opcSubscription.RemoveItem(mitem);
                            }
                            opcSubscription.ApplyChanges();
                            return true;
                        }
                        else
                            return true;
                    }
                    catch
                    {

                    }
                }
                return false;
            });
        }


    }
}
