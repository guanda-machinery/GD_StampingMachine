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

    public class GD_OpcUaHelperClient: IOpcuaConnect
    {
        private OpcUaClient m_OpcUaClient = new OpcUaClient();

        public int ConntectMillisecondsTimeout = 1000;

        private Uri CombineUrl(string hostPath , int? port ,string dataPath)
        {
            //var hostPath = HostPath;
            if (!hostPath.Contains("opc.tcp://"))
                hostPath = "opc.tcp://" + hostPath;
            if (port.HasValue)
                hostPath += $":{port}";
            var BaseUrl = new Uri(hostPath);
            return new Uri(BaseUrl, dataPath);
        }

        private const int retryCounter = 10;



        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);


        public async Task<bool> AsyncConnect(string hostPath, int port = 0, string dataPath = null, string user = null, string password = null)
        {
            m_OpcUaClient.UserIdentity = new Opc.Ua.UserIdentity(user, password);



            if (!m_OpcUaClient.Connected)
            {
                if (TcpPing.RetrieveIpAddress(hostPath, out var _ip))
                {
                    if (!await TcpPing.IsPingableAsync(_ip))
                    {
                        ConnectException = new PingException($"Ping Host: {_ip} is Failed");
                        return false;
                    }
                }

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

                await Task.Run(async () =>
                {
                    try
                    {
                        if (!m_OpcUaClient.Connected)
                        {
                            var baseUrl = CombineUrl(hostPath, port, dataPath); 
                            await m_OpcUaClient.ConnectServer(baseUrl.ToString());
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
                });

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

        public bool IsConnected => m_OpcUaClient.Connected;



        /// <summary>
        /// 寫入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public async Task<bool> AsyncWriteNode<T>(string NodeTreeString, T WriteValue)
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
        public async Task<IEnumerable<bool>> AsyncWriteNodes(Dictionary<string,object> NodeTrees)
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
        public async Task<(bool , T)> AsyncReadNode<T>(string NodeID)
        {
            //T NodeValue = default(T);
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                        T NodeValue;
                         NodeValue = await m_OpcUaClient.ReadNodeAsync<T>(NodeID);                
                        return (true, NodeValue);
                    }
                }
                catch (Exception ex)
                {
                    //Disconnect();
                }
            }
            return (false, default(T));
        }



        public async Task<(bool result, IEnumerable<T> values)> AsyncReadNodes<T>(IEnumerable<string> NodeTrees)
        {
            //T NodeValue = default(T);
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                        List<T> NodeValue;
                        NodeValue = await m_OpcUaClient.ReadNodesAsync<T>(NodeTrees.ToArray());
                        return (true, NodeValue);
                    }
                }
                catch (Exception ex)
                {
                    //Disconnect();
                }
            }
            return (false, default(IEnumerable<T>));
        }

        public async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates = false)
        {
            var ret = await SubscribeNodesDataChangeAsync(new List<(string, Action<T>, int, bool checkDuplicates)>
            {
                (NodeID, updateAction , samplingInterval , checkDuplicates)
            });
            return ret.FirstOrDefault();
        }




        private readonly SemaphoreSlim subscribeSemaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<IList<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList)
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

                            string skey = samplingInterval.ToString();
                                //string stag = index.ToString();

                                Subscription opcSubscription = m_OpcUaClient.Session.Subscriptions.FirstOrDefault(x => x.DisplayName == skey);
                                if (opcSubscription == null)
                                {
                                    //opcSubscription = new Subscription();

                                    opcSubscription = new Subscription(m_OpcUaClient.Session.DefaultSubscription);
                                    opcSubscription.PublishingEnabled = true;
                                    opcSubscription.KeepAliveCount = uint.MaxValue;
                                    opcSubscription.LifetimeCount = uint.MaxValue;
                                    opcSubscription.MaxNotificationsPerPublish = uint.MaxValue;
                                    opcSubscription.Priority = 100;
                                    opcSubscription.PublishingInterval = samplingInterval;
                                    opcSubscription.DisplayName = samplingInterval.ToString();
                                }

                                if (checkDuplicates && opcSubscription.MonitoredItems.Any(x => x.StartNodeId == new NodeId(NodeID)))
                                {
                                    ret = true;
                                    break;
                                }


                                var item = new MonitoredItem();
                                item.StartNodeId = new NodeId(NodeID);
                                item.SamplingInterval = samplingInterval;
                                item.Notification += (sender, e) =>
                                {
                                    try
                                    {
                                        if (e.NotificationValue is T Tvalue)
                                        {
                                            updateAction?.Invoke(Tvalue);
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


                                if (!m_OpcUaClient.Session.Subscriptions.Contains(opcSubscription))
                                    m_OpcUaClient.Session.AddSubscription(opcSubscription); 
                                opcSubscription.Create();
                                opcSubscription.ApplyChanges();

                                break;
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                //Debugger.Break();
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

                        Subscription opcSubscription = m_OpcUaClient.Session.Subscriptions.FirstOrDefault(x => Equals(x.DisplayName , samplingInterval));
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
