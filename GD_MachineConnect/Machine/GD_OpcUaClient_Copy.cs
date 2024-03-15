using GD_CommonLibrary;
using GD_MachineConnect.Machine.Interfaces;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GD_MachineConnect.Machine
{
    public class GD_OpcUaClient : AbstractOpcuaConnect, IDisposable
    {
        private Session m_session;

        public readonly ApplicationConfiguration AppConfig;



        private int m_reconnectPeriod = 10;
        public int ReconnectPeriod
        {
            get
            {
                return m_reconnectPeriod;
            }
            set
            {
                m_reconnectPeriod = value;
            }
        }



        private readonly string OpcUaName = "OpcUa";

        public GD_OpcUaClient()
        {
            //dic_subscriptions = new Dictionary<string, Subscription>();
            CertificateValidator certificateValidator = new CertificateValidator();
            certificateValidator.CertificateValidation += delegate (CertificateValidator sender, CertificateValidationEventArgs eventArgs)
            {
                if (ServiceResult.IsGood(eventArgs.Error))
                {
                    eventArgs.Accept = true;
                }
                else
                {
                    if (eventArgs.Error.StatusCode.Code != 2149187584u)
                    {
                        throw new Exception($"Failed to validate certificate with error code {eventArgs.Error.Code}: {eventArgs.Error.AdditionalInfo}");
                    }

                    eventArgs.Accept = true;
                }
            };
            SecurityConfiguration configuration = new SecurityConfiguration
            {
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false,
                MinimumCertificateKeySize = 1024
            };
            certificateValidator.Update(configuration);
            ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration
            {
                ApplicationName = OpcUaName,
                ApplicationType = ApplicationType.Client,
                CertificateValidator = certificateValidator,
                ApplicationUri = "urn:MyClient",
                ProductUri = "OpcUaClient",
                ServerConfiguration = new ServerConfiguration
                {
                    MaxSubscriptionCount = 100000,
                    MaxMessageQueueSize = 1000000,
                    MaxNotificationQueueSize = 1000000,
                    MaxPublishRequestCount = 10000000
                },
                SecurityConfiguration = new SecurityConfiguration
                {
                    AutoAcceptUntrustedCertificates = true,
                    RejectSHA1SignedCertificates = false,
                    MinimumCertificateKeySize = 1024,
                    SuppressNonceValidationErrors = true,
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "X509Store",
                        StorePath = "CurrentUser\\My",
                        SubjectName = OpcUaName
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "X509Store",
                        StorePath = "CurrentUser\\Root"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "X509Store",
                        StorePath = "CurrentUser\\Root"
                    }
                },
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 30000,
                    MaxStringLength = int.MaxValue,
                    MaxByteStringLength = int.MaxValue,
                    MaxArrayLength = 65535,
                    MaxMessageSize = 419430400,
                    MaxBufferSize = 65535,
                    ChannelLifetime = -1,
                    SecurityTokenLifetime = -1
                },
                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = 30000,
                    MinSubscriptionLifetime = -1
                },
                DisableHiResClock = true
            };
            applicationConfiguration.Validate(ApplicationType.Client);
            AppConfig = applicationConfiguration;
        }

        //public int ConntectMillisecondsTimeout = 1000;

        public bool UseSecurity { get; set; }




        private EventHandler _connectComplete;
        /// <summary>
        /// 連線成功
        /// </summary>
        public event EventHandler ConnectComplete
        {
            add
            {
                _connectComplete = (EventHandler)Delegate.Combine(_connectComplete, value);
            }
            remove
            {
                _connectComplete = (EventHandler)Delegate.Remove(_connectComplete, value);
            }
        }



        private EventHandler m_KeepAliveComplete;
        public event EventHandler KeepAliveComplete
        {
            add
            {
                m_KeepAliveComplete = (EventHandler)Delegate.Combine(m_KeepAliveComplete, value);
            }
            remove
            {
                m_KeepAliveComplete = (EventHandler)Delegate.Remove(m_KeepAliveComplete, value);
            }
        }


        private EventHandler _reconnectStarting;
        public event EventHandler ReconnectStarting
        {
            add
            {
                _reconnectStarting = (EventHandler)Delegate.Combine(_reconnectStarting, value);
            }
            remove
            {
                _reconnectStarting = (EventHandler)Delegate.Remove(_reconnectStarting, value);
            }
        }

        private EventHandler _reconnectEnd;
        /// <summary>
        /// 重連結束
        /// </summary>
        public event EventHandler ReconnectEnd
        {
            add
            {
                _reconnectEnd = (EventHandler)Delegate.Combine(_reconnectEnd, value);
            }
            remove
            {
                _reconnectEnd = (EventHandler)Delegate.Remove(_reconnectEnd, value);
            }
        }

        private SessionReconnectHandler _reConnectHandler;

        private bool disposedValue;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public override async Task<bool> ConnectAsync(string hostPath, string? user = null, string? password = null)
        {
            IUserIdentity? userIdentity = null;
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                userIdentity = new Opc.Ua.UserIdentity(user, password);

            if (IsConnected)
                await DisconnectAsync();

            EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(hostPath, UseSecurity);
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(AppConfig);
            m_session = await Session.Create(endpoint: new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration), configuration: AppConfig, updateBeforeConnect: false, checkDomain: false, sessionName: string.IsNullOrEmpty(OpcUaName) ? AppConfig.ApplicationName : OpcUaName, sessionTimeout: 30000u, identity: userIdentity, preferredLocales: new string[0]);
            m_session.OperationTimeout = 30000;
            m_session.KeepAlive += Session_KeepAlive;
            IsConnected = true;
            return true;
        }


        private void Session_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            try
            {
                if (session != m_session)
                {
                    return;
                }

                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_reconnectPeriod <= 0)
                    {
                        return;
                    }

                    if (_reConnectHandler == null)
                    {
                        _reconnectStarting?.Invoke(this, e);
                        _reConnectHandler = new SessionReconnectHandler();
                        _reConnectHandler.BeginReconnect(m_session, m_reconnectPeriod * 1000, _reconnectEnd);
                    }
                }
                else
                {
                    //UpdateStatus(false, e.CurrentTime, "Connected [{0}]", session.Endpoint.EndpointUrl);
                    m_KeepAliveComplete?.Invoke(this, e);
                }
            }
            catch (Exception)
            {
                // ClientUtils.HandleException(OpcUaName, e2);
            }
        }


        public Exception ConnectException { get; private set; }

        public override void Disconnect()
        {
            if (_reConnectHandler != null)
            {
                _reConnectHandler.Dispose();
                _reConnectHandler = null;
            }

            if (m_session != null)
            {
                m_session.OperationTimeout = 1000;
                m_session.Close(10000);
                m_session = null;
            }
            IsConnected = false;
        }

        public override async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                this.Disconnect();
            });
            IsConnected = false;
        }


        /// <summary>
        ///  寫入數列組
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public override async Task<bool> WriteNodeAsync<T>(string tag, T value)
        {
            return (await WriteNodesAsync(new Dictionary<string, object> { { tag, value } })).FirstOrDefault();
        }


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public override Task<IEnumerable<bool>> WriteNodesAsync(Dictionary<string, object> NodeTrees)
        {

            WriteValueCollection valuesToWrite = new WriteValueCollection();
            foreach (var node in NodeTrees)
            {
                WriteValue writeValue = new WriteValue
                {
                    NodeId = new NodeId(node.Key),
                    AttributeId = 13u
                };
                writeValue.Value.Value = node.Value;
                writeValue.Value.StatusCode = 0u;
                writeValue.Value.ServerTimestamp = DateTime.MinValue;
                writeValue.Value.SourceTimestamp = DateTime.MinValue;
                valuesToWrite.Add(writeValue);
            }

            TaskCompletionSource<IEnumerable<bool>> taskCompletionSource = new TaskCompletionSource<IEnumerable<bool>>();
            m_session.BeginWrite(null, valuesToWrite, delegate (IAsyncResult ar)
            {
                StatusCodeCollection results;
                DiagnosticInfoCollection diagnosticInfos;
                ResponseHeader responseHeader = m_session.EndWrite(ar, out results, out diagnosticInfos);
                try
                {
                    ClientBase.ValidateResponse(results, valuesToWrite);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);
                    var goodList = results.Select(x => StatusCode.IsGood(x));
                    taskCompletionSource.TrySetResult(goodList);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.TrySetException(exception);
                }
            }, null);
            return taskCompletionSource.Task;

        }







        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public override async Task<T> ReadNodeAsync<T>(string tag)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

            _ = Task.Run(() =>
            {
                try
                {
                    ReadValueIdCollection nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId
                {
                    NodeId = new NodeId(tag),
                    AttributeId = 13u
                }
            };
                    m_session.Read(null, 0.0, TimestampsToReturn.Neither, nodesToRead, out var results, out var diagnosticInfos);
                    ClientBase.ValidateResponse(results, nodesToRead);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
                    m_session.BeginRead(null, 0.0, TimestampsToReturn.Neither, nodesToRead, delegate (IAsyncResult ar)
                    {
                        DataValueCollection results;
                        DiagnosticInfoCollection diagnosticInfos;
                        ResponseHeader responseHeader = m_session.EndRead(ar, out results, out diagnosticInfos);
                        try
                        {
                            if (!StatusCode.IsGood(responseHeader.ServiceResult))
                                throw new Exception($"Invalid response from the server.");
                            if (!StatusCode.IsGood(results[0].StatusCode))
                                throw new Exception($"Invalid response from the server.");

                            DataValue dataValue = results[0];
                            taskCompletionSource.TrySetResult((T)dataValue.Value);
                        }
                        catch (Exception exception)
                        {
                            taskCompletionSource.TrySetException(exception);
                        }
                    }, null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }



        public override async Task<IEnumerable<object>> ReadNodesAsync(IEnumerable<string> nodeIds)
        {
            TaskCompletionSource<IEnumerable<object>> taskCompletionSource = new TaskCompletionSource<IEnumerable<object>>();

            _ = Task.Run(() =>
            {
                try
                {
                    ReadValueIdCollection readValueIdCollection = new ReadValueIdCollection();
                    foreach (string nodeId in nodeIds)
                    {
                        readValueIdCollection.Add(new ReadValueId
                        {
                            NodeId = nodeId,
                            AttributeId = 13u
                        });
                    }
                    m_session?.BeginRead(null, 0.0, TimestampsToReturn.Neither, readValueIdCollection, delegate (IAsyncResult ar)
                    {
                        DataValueCollection results;
                        DiagnosticInfoCollection diagnosticInfos;
                        ResponseHeader responseHeader = m_session.EndRead(ar, out results, out diagnosticInfos);
                        try
                        {
                            if (!StatusCode.IsGood(responseHeader.ServiceResult))
                                throw new Exception($"Invalid response from the server.");

                            List<object> list = new List<object>();
                            foreach (DataValue item in results)
                            {
                                list.Add(item.Value);
                            }
                            taskCompletionSource.TrySetResult(list);
                        }
                        catch (Exception exception)
                        {
                            taskCompletionSource.TrySetException(exception);
                        }
                    }, null);
                }
                catch(Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }

            });
            return await taskCompletionSource.Task;
        }


        public override async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval = 1000, bool checkDuplicates = true)
        {
            var ret = await SubscribeNodesDataChangeAsync(new List<(string, Action<T>, int, bool checkDuplicates)>
            {
                (NodeID, updateAction , samplingInterval , checkDuplicates)
            });
            return ret.FirstOrDefault();
        }

        private readonly SemaphoreSlim subscribeSemaphoreSlim = new SemaphoreSlim(1, 1);
        public override async Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList)
        {
            TaskCompletionSource<IEnumerable<bool>> taskCompletionSource = new TaskCompletionSource<IEnumerable<bool>>();
            await subscribeSemaphoreSlim.WaitAsync();
            try
            {
                List<bool> retList = new List<bool>();
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

                            Subscription opcSubscription;
                            var Existed = m_session.Subscriptions.Any(x => x.DisplayName == skey);
                            if (Existed)
                            {
                                opcSubscription = m_session.Subscriptions.First(x => x.DisplayName == skey);
                            }
                            else
                            {
                                opcSubscription = new Subscription(m_session.DefaultSubscription);
                                opcSubscription.PublishingEnabled = true;
                                opcSubscription.KeepAliveCount = uint.MaxValue;
                                opcSubscription.LifetimeCount = uint.MaxValue;
                                opcSubscription.MaxNotificationsPerPublish = uint.MaxValue;
                                opcSubscription.Priority = 100;
                                opcSubscription.PublishingInterval = samplingInterval;
                                opcSubscription.DisplayName = samplingInterval.ToString();

                                m_session.AddSubscription(opcSubscription);
                                opcSubscription.Create();
                            }

                            if (checkDuplicates && opcSubscription.MonitoredItems.Any(x => x.StartNodeId == new NodeId(NodeID)))
                            {
                                //已經有訂閱了
                                //throw new Exception($"Node ID {NodeID} is Subscribed");
                                ret = true;
                                break;
                            }
                            var item = new MonitoredItem()
                            {
                                StartNodeId = new NodeId(NodeID),
                                AttributeId = 13u,
                                DisplayName = NodeID,
                                SamplingInterval = samplingInterval
                            };

                            item.Notification += (sender, e) =>
                            {
                                //將action返還值
                                try
                                {
                                    if (e.NotificationValue is Opc.Ua.MonitoredItemNotification notification)
                                    {
                                        if (notification.Value.Value is T tValue)
                                        {
                                            updateAction?.Invoke(tValue);
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
                            //CancellationTokenSource cts = new CancellationTokenSource(10000);
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
                taskCompletionSource.TrySetResult(retList);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Debug.WriteLine(ex.ToString());

                taskCompletionSource.TrySetException(ex);
            }
            subscribeSemaphoreSlim.Release();
            return await taskCompletionSource.Task;
        }


        public override async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, EventHandler<ValueChangedEventArgs<T>> updateHandler, int samplingInterval = 1000, bool checkDuplicates = true)
        {
            var ret = await SubscribeNodesDataChangeAsync<T>(new List<(string, EventHandler<ValueChangedEventArgs<T>>, int, bool checkDuplicates)>
            {
                (NodeID, updateHandler , samplingInterval , checkDuplicates)
            });
            return ret.FirstOrDefault();
        }

        public override async Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, EventHandler<ValueChangedEventArgs<T>> updateHandler, int samplingInterval, bool checkDuplicates)> nodeList)
        {

            TaskCompletionSource<IEnumerable<bool>> taskCompletionSource = new TaskCompletionSource<IEnumerable<bool>>();
            _ = Task.Run(async () =>
            {
                await subscribeSemaphoreSlim.WaitAsync();
                try
                {
                List<bool> retList = new List<bool>();
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

                            Subscription opcSubscription = m_session.Subscriptions.First(x => x.DisplayName == skey);
                            if (opcSubscription == null)
                            {
                                opcSubscription = new Subscription(m_session.DefaultSubscription);
                                opcSubscription.PublishingEnabled = true;
                                opcSubscription.KeepAliveCount = uint.MaxValue;
                                opcSubscription.LifetimeCount = uint.MaxValue;
                                opcSubscription.MaxNotificationsPerPublish = uint.MaxValue;
                                opcSubscription.Priority = 100;
                                opcSubscription.PublishingInterval = samplingInterval;
                                opcSubscription.DisplayName = samplingInterval.ToString();

                                m_session.AddSubscription(opcSubscription);
                                opcSubscription.Create();
                            }

                            if (checkDuplicates && opcSubscription.MonitoredItems.Any(x => x.StartNodeId == new NodeId(NodeID)))
                            {
                                //已經有訂閱了
                                //throw new Exception($"Node ID {NodeID} is Subscribed");
                                ret = true;
                                break;
                            }

                            var item = new MonitoredItem()
                            {
                                StartNodeId = new NodeId(NodeID),
                                AttributeId = 13u,
                                DisplayName = NodeID,
                                SamplingInterval = samplingInterval
                            };

                            T oldValue = default(T);
                            item.Notification += (sender, e) =>
                            {
                                //將action返還值
                                try
                                {
                                    if (e.NotificationValue is Opc.Ua.MonitoredItemNotification notification)
                                    {
                                        if (notification.Value.Value is T newValue)
                                        { 
                                            updateAction?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, newValue));
                                            oldValue = newValue;
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
                            //CancellationTokenSource cts = new CancellationTokenSource(10000);
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
                taskCompletionSource.TrySetResult(retList);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Debug.WriteLine(ex.ToString());

                taskCompletionSource.TrySetException(ex);
            }
                subscribeSemaphoreSlim.Release();
            });
            return await taskCompletionSource.Task;
        }


        public override async Task<bool> UnsubscribeNodeAsync(string NodeID, int samplingInterval)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            _ = Task.Run(async () =>
            {
                await subscribeSemaphoreSlim.WaitAsync();
                try
                {
                    Subscription opcSubscription = m_session.Subscriptions.First(x => Equals(x.DisplayName, samplingInterval.ToString()));
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

                        if (opcSubscription.MonitoredItemCount == 0)
                        {
                            m_session.RemoveSubscription(opcSubscription);
                        }


                        taskCompletionSource.TrySetResult(true);
                    }
                    else
                        taskCompletionSource.TrySetResult(false);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.TrySetException(ex);
                }
                subscribeSemaphoreSlim.Release();
            });
            return await taskCompletionSource.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                Disconnect();
                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~GD_OpcUaClient()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
