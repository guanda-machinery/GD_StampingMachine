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

    public class GD_OpcUaFxClient:IDisposable
    {
        ~GD_OpcUaFxClient()
        {
            Dispose(disposing: false);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_OpcUaClient?.Disconnect();
                    m_OpcUaClient = null;
                    // TODO: 處置受控狀態 (受控物件)
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }








        OpcClient m_OpcUaClient = new OpcClient();
        public GD_OpcUaFxClient(string hostPath , int port =0 , string dataPath = null, OpcUserIdentity userIdentity = null)
        {
            HostPath = hostPath;
            var baseUrl = CombineUrl(hostPath, port, dataPath);
            m_OpcUaClient = new OpcClient(baseUrl.ToString());
            m_OpcUaClient.Security.UserIdentity = userIdentity;
        }
        public int ConntectMillisecondsTimeout = 3000;
        private string HostPath;
        private bool disposedValue;

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
        public async Task<bool> AsyncConnect()
        {
            if (TcpPing.RetrieveIpAddress(HostPath, out var _ip))
            {
                if (!await TcpPing.IsPingableAsync(_ip))
                {
                    ConnectException = new PingException($"Ping Host: {_ip} is Failed");
                    return false;
                }
            }
            var ret = false;
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
                await Task.Run(() =>
                {
                    try
                    {
                        if (m_OpcUaClient.State != OpcClientState.Connected)
                        {
                            m_OpcUaClient.Connect();
                            ConnectException = null;
                            ret = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ConnectException = ex;
                        m_OpcUaClient.Disconnect();
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

        public void Disconnect()
        {
            m_OpcUaClient.Disconnect();
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
                    if (await AsyncConnect())
                    {
                        //ret = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
                        OpcStatus result = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);

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

        public async Task<bool> AsyncWriteNodes(Dictionary<string,object> NodeTrees)
        {
            if (NodeTrees.Count == 0)
                return false;

           var commands = new List<OpcWriteNode>();
            foreach(var node in NodeTrees)
            {
                commands.Add(new OpcWriteNode(node.Key, node.Value));
            }

            var ret = false;
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (await AsyncConnect())
                    {
                        
                           // var tags = NodeTrees.Keys.ToArray();
                           //var values = NodeTrees.Values.ToArray();
                           // ret = m_OpcUaClient.WriteNodes(tags, values);
                        m_OpcUaClient.WriteNodes(commands);
                    }

                    if (ret)
                    {
                        break;
                    }
                }
                catch (Opc.Ua.ServiceResultException sex)
                {
                    Disconnect();
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
                    if (await AsyncConnect())
                    {
                        //T NodeValue;
                       // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                        OpcValue rNode = m_OpcUaClient.ReadNode(NodeID);
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
        public async Task<(bool, IEnumerable<T>)> AsyncReadNodes<T>(IEnumerable<string> NodeTrees)
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
                    if (await AsyncConnect())
                    {
                        //T NodeValue;
                        // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                        var job = m_OpcUaClient.ReadNodes(commands).ToList();
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





        private List<(string NodeID, Func<object> conditionFunc)> commands = new ();
        public void SubscribeNodeDataChange<T>(string NodeID , Action<T> updateAction)
        {
           // Func<T> func = null;
            var command = new OpcSubscribeDataChange(NodeID,
                (sender, e) =>
                {
                    // The tag property contains the previously set value.
                     OpcMonitoredItem item = (OpcMonitoredItem)sender;
                    Console.WriteLine("Data Change from NodeId '{0}': {1}",item.NodeId,e.Item.Value);
                    if (e.Item.Value.Value is T Tvalue)
                    {
                        updateAction?.Invoke(Tvalue);
                    }
                });
            if (m_OpcUaClient.Subscriptions.ToList().Exists(x => x.Id.ToString() == NodeID))
            {

            }
            OpcSubscription subscription = m_OpcUaClient.SubscribeNode(command);

        }




        public void UnSubscribeNode(string NodeID)
        {
              m_OpcUaClient.UnregisterNode(NodeID);
        }






        /*   public void SubscribeNodeDataChange<T>(Func<T> func)
           {
               OpcSubscription subscription = m_OpcUaClient.SubscribeDataChange(
                   "ns=2;s=Machine/IsRunning",
                   HandleDataChanged);
           }

           private static void HandleDataChanged(object sender,OpcDataChangeReceivedEventArgs e)
           {
               // Your code to execute on each data change.
               // The 'sender' variable contains the OpcMonitoredItem with the NodeId.
               OpcMonitoredItem item = (OpcMonitoredItem)sender;
               Console.WriteLine(
                       "Data Change from NodeId '{0}': {1}",
                       item.NodeId,
                       e.Item.Value);
           }*/




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
