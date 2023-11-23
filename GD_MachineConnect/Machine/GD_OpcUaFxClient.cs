using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GD_CommonLibrary.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.UaFx;
using Opc.UaFx.Client;
using Org.BouncyCastle.Security;

namespace GD_MachineConnect.Machine
{

    //https://docs.traeger.de/en/software/sdk/opc-ua/net/client.development.guide#reading-values

    public class GD_OpcUaFxClient: IAsyncDisposable
    {

        private bool disposedValue;
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                await semaphoreSlim.WaitAsync();
                await DisconnectAsync();
                semaphoreSlim.Release();
                disposedValue = true;
            }

        }

        OpcClient m_OpcUaClient = new OpcClient();

        public GD_OpcUaFxClient()
        {

        }


        public GD_OpcUaFxClient(string hostPath , int port =0 , string dataPath = null, OpcUserIdentity userIdentity = null)
        {
            HostPath = hostPath;
            var baseUrl = CombineUrl(hostPath, port, dataPath);
            m_OpcUaClient = new OpcClient(baseUrl.ToString());
            m_OpcUaClient.Security.UserIdentity = userIdentity;
            m_OpcUaClient.SessionTimeout = 10000;
        }
        public int ConntectMillisecondsTimeout = 3000;
        private string HostPath;

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

        private const int retryCounter = 5;

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        
        public bool IsConnected { get => m_OpcUaClient.State == OpcClientState.Connected; }
        public async Task<bool> AsyncConnect()
        {
            if(disposedValue)
                return false;

            if (TcpPing.RetrieveIpAddress(HostPath, out var _ip))
            {
                if (!await TcpPing.IsPingableAsync(_ip))
                {
                    ConnectException = new PingException($"Ping Host: {_ip} is Failed");
                    return false;
                }
            }
            var ret = false;

            await WaitForCondition.WaitAsync(()=>m_OpcUaClient.State, OpcClientState.Connecting ,false);

            if (m_OpcUaClient.State == OpcClientState.Connected)
            {
                ret = true;
            }
            else
            { 
                using (var cts = new CancellationTokenSource(ConntectMillisecondsTimeout))
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
              
                if (disposedValue)
                    return false;

                await Task.Run(async () =>
                {
                    try
                    {
                        if (!IsConnected)
                        {
                            m_OpcUaClient.OperationTimeout = 5000;
                            m_OpcUaClient.Connect();
                            ConnectException = null;
                            ret = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ConnectException = ex;
                       await DisconnectAsync();
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });
            }

            return ret;
        }

        public Exception ConnectException { get;private set; }

        public async Task DisconnectAsync()
        {
            m_OpcUaClient.Disconnect();
            var isCreated = WaitForCondition.WaitAsync(() => m_OpcUaClient.State, OpcClientState.Created);
            var isDisconnected = WaitForCondition.WaitAsync(() => m_OpcUaClient.State, OpcClientState.Disconnected);
       
            m_OpcUaClient.Disconnect();
          await  Task.WhenAny(isCreated, isDisconnected);
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
                        var result = await Task.Run(() =>
                        {
                            OpcStatus opcresult = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
                            return opcresult;
                        });

                        if (result.IsGood)
                            return true;
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
            var ret = new List<bool>();
            if (NodeTrees.Count == 0)
                return ret;

           var commands = new List<OpcWriteNode>();
            foreach(var node in NodeTrees)
            {
                commands.Add(new OpcWriteNode(node.Key, node.Value));
            }

            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                        // var tags = NodeTrees.Keys.ToArray();
                        //var values = NodeTrees.Values.ToArray();
                        // ret = m_OpcUaClient.WriteNodes(tags, values);

                        var result = await Task.Run(() =>
                        {
                            return m_OpcUaClient.WriteNodes(commands);
                        });
                        ret = result.ToList().Select(x => x.IsGood).ToList();
                    }

                }
                catch (Opc.Ua.ServiceResultException sex)
                {
                    //Disconnect();
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
                        //T NodeValue;
                        // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                        OpcValue rNode = await Task.Run(() =>
                        {
                            return m_OpcUaClient.ReadNode(NodeID);
                        });
                           
                        if(rNode.Value  is T NodeValue) 
                        {
                            return (true, NodeValue);
                        }
                        return (false, default(T));
                    }
                }
                catch(Opc.Ua.ServiceResultException sex)
                {
                   // Disconnect();
                }
                catch (Exception ex)
                {
                    await Task.Delay(50);
                    //Disconnect();
                }
                await Task.Delay(10);
            }

            return (false, default(T));
        }



        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public async Task<(bool result, IEnumerable<T>values)> AsyncReadNodes<T>(IEnumerable<string> NodeTrees)
        {
            var commands = new List<OpcReadNode>();
            foreach (var node in NodeTrees)
            {
                commands.Add(new OpcReadNode(node));
            }


            //T NodeValue = default(T);
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (IsConnected)
                    {
                        //T NodeValue;
                        // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);


                        var job = await Task.Run(() =>
                        {
                            return m_OpcUaClient.ReadNodes(commands).ToList();
                        });

                        var values = job.Select(x => x.Value).ToList();
                        if (values.Exists(x => x is not T))
                            return (false, null);
                        else
                        {
                            var Tvalues = values.OfType<T>();
                            return (true, Tvalues);
                        }
                    }
                }
                catch (Opc.Ua.ServiceResultException sex)
                {
                    // Disconnect();
                }
                catch (Exception ex)
                {
                    await Task.Delay(50);
                    //Disconnect();
                }
                await Task.Delay(1);
            }
            return (false, null);
        }



        public async Task SubscribeNodeDataChange<T>( string NodeID, Action<T> updateAction, int samplingInterval = 200)
        {
            await SubscribeNodeDataChange<T>(NodeID, updateAction, samplingInterval);
        }





        /// <summary>
        /// 存放ID
        /// </summary>
        //private Dictionary<int,long> subscribeDictionary = new Dictionary<int,long>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public async Task SubscribeNodesDataChange<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval)> nodeList)
        {
            await Task.Run(() =>
            {
                try
                {
                    for (int index = 0; index < nodeList.Count(); index++)
                    {
                        try
                        {
                            //使用設定的響應速度進行分類
                            OpcSubscription opcSubscription = m_OpcUaClient.Subscriptions.FirstOrDefault(x => Equals(x.Tag, nodeList[index].samplingInterval));
                            if (opcSubscription == null)
                            {
                                // If no subscription exists, create a new one
                                opcSubscription = m_OpcUaClient.SubscribeNodes();
                                opcSubscription.Tag = nodeList[index].samplingInterval;
                            }

                            // Create an OpcMonitoredItem for the NodeId.
                            var item = new OpcMonitoredItem(nodeList[index].NodeID, OpcAttribute.Value);
                            item.DataChangeReceived += (sender, e) =>
                            {
                                OpcMonitoredItem item = (OpcMonitoredItem)sender;
                                Console.WriteLine(
                                        "Data Change from Index {0}: {1}",
                                        item.Tag,
                                        e.Item.Value);
                                if (e.Item.Value.Value is T Tvalue)
                                {
                                    nodeList[index].updateAction?.Invoke(Tvalue);
                                }
                            };
                            // You can set your own values on the "Tag" property
                            // that allows you to identify the source later.
                            item.Tag = index;
                            // Set a custom sampling interval on the 
                            // monitored item.
                            item.SamplingInterval = nodeList[index].samplingInterval;
                            // Add the item to the subscription.
                            opcSubscription.AddMonitoredItem(item);

                            // After adding the items (or configuring the subscription), apply the changes.
                            opcSubscription.ApplyChanges();
                        }
                        catch (Exception ex)
                        {
                            Debugger.Break();
                            Debug.WriteLine(ex.ToString());
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    Debug.WriteLine(ex.ToString());
                }
            });
        }

        /// <summary>
        /// 移掉所有訂閱的節點
        /// </summary>
        /// <param name="NodeID"></param>
        public void UnSubscribeNode(string NodeID)
        {
            try
            {
                foreach(var item in m_OpcUaClient.Subscriptions)
                {
                    List<OpcMonitoredItem> removeItems = new();
                   foreach(var monitoredItem in item.MonitoredItems)
                    {
                        if(NodeID == monitoredItem.NodeId)
                        {
                            removeItems.Add(monitoredItem);
                        }
                    }


                   foreach(var mitem in removeItems)
                    {
                        item.RemoveMonitoredItem(mitem);
                        item.ApplyChanges();
                    }
                }
            }
            catch
            {

            }

        }




        /*
        public async Task<List<OpcNodeAttribute>> ReadNoteAttributes(string NodeTreeString)
        {
            var nodeAttributesList = new List<OpcNodeAttribute>() ;
            try
            {
                if (await this.AsyncConnect())
                {
                    OpcNodeAttribute[] nodeAttributes = m_OpcUaClient.ReadNoteAttributes(NodeTreeString);
                    //nodeAttributes = m_OpcUaClient.ReadNoteAttributes(NodeTreeString);
                    foreach (var item in nodeAttributes)
                    {
                        Console.Write(string.Format("{0,-30}", item.Name));
                        Console.Write(string.Format("{0,-20}", item.Type));
                        Console.Write(string.Format("{0,-20}", item.StatusCode));
                        Console.WriteLine(string.Format("{0,20}", item.Value));
                    }
                    Console.WriteLine("-------------------------------------");
                    nodeAttributesList = nodeAttributes.ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return nodeAttributesList;
        }

        */

        /*public bool ReadAllReference(string NodeTreeString , out List<NodeTypeValue> NodeValue)
        {
            NodeValue = new List<NodeTypeValue>();

            m_OpcUaClient.ConnectServer();
            //取得所有節點
            try
            {
                List<ReferenceDescription> referencesList = new List<ReferenceDescription>();

                var NodeIDStringList = new List<string>() { NodeTreeString };
                var ExistedNodeIDStringList = new List<string>();
                while (true)
                {
                    var NodeSearchList = NodeIDStringList.Except(ExistedNodeIDStringList);
                    var NextSearchList = new List<string>();
                    foreach (var NodeIDString in NodeSearchList)
                    {
                        try
                        {
                            ReferenceDescription[] references = m_OpcUaClient.BrowseNodeReference(NodeIDString);
                            foreach (var Ref in references)
                            {

                                //展開
                                NextSearchList.Add(Ref.NodeId.ToString());
                                referencesList.Add(Ref);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    ExistedNodeIDStringList.AddRange(NodeSearchList);
                    NodeIDStringList.AddRange(NextSearchList);

                    if (NextSearchList.Count == 0)
                        break;
                }

                //展開所有節點
                var GetNodeValue = new List<NodeTypeValue>();
                referencesList.ForEach(reference =>
                {
                    try
                    {
                        //ReadNoteAttributes(reference.NodeId.ToString());
                        ReadNode(reference.NodeId.ToString(), out object NValue);

                        GetNodeValue.Add(new NodeTypeValue()
                        {
                            NodeID = reference.NodeId,
                            NodeDisplayName = reference.DisplayName,
                            NodeValue = NValue
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                //印出節點資料
                GetNodeValue.ForEach(NValue =>
                {
                    try { 
                    Type NodeType = null;
                    if (NValue.NodeValue != null)
                        NodeType = NValue.NodeValue.GetType();

                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeID)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeID));
                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeDisplayName)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeDisplayName));
                    Console.Write(string.Format("{0,-20}", nameof(Type)));
                    Console.WriteLine(string.Format("{0,0}", NodeType));
                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeValue)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeValue));
                    Console.WriteLine("".PadLeft(50, '-'));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                });


                NodeValue = GetNodeValue;
                return true;

            }
            catch (Exception ex)
            {

            }
            finally 
            {
                m_OpcUaClient.Disconnect();
            }
            return false;
        }*/



    }
}
