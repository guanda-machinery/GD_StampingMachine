using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using OpcUaHelper;

namespace GD_MachineConnect.Machine
{

    //https://www.cnblogs.com/daji2020/p/16627627.html

    public class GD_OpcUaHelperClient
    {
        readonly OpcUaClient  m_OpcUaClient;
        public GD_OpcUaHelperClient()
        {
            m_OpcUaClient = new OpcUaClient();
        }
        //X509憑證 X509Certificate2 Certificate
        //m_OpcUaClient.UserIdentity ??= new UserIdentity(Certificate);
        public IUserIdentity UserIdentity
        {
            get=> m_OpcUaClient.UserIdentity; 
            set => m_OpcUaClient.UserIdentity = value;
        }

        public bool IsConnected => m_OpcUaClient.Connected;

        public async Task<bool> OpcuaConnectAsync(string HostPath, int? Port, string DataPath = null)
        {
            if (Port.HasValue)
                return await OpcuaConnectAsync($"{HostPath}:{Port}", DataPath);
            else
                return await OpcuaConnectAsync($"{HostPath}", DataPath);
        }
        public async Task<bool> OpcuaConnectAsync(string HostPath, string DataPath = null)
        {
            if (!HostPath.Contains("opc.tcp://"))
                HostPath = "opc.tcp://" + HostPath;

            var BaseUrl = new Uri(HostPath);
            var CombineUrl = new Uri(BaseUrl, DataPath);
            try
            {
                await m_OpcUaClient.ConnectServer(CombineUrl.ToString());
                return true;
            }
            catch (Exception ex)
            {
                /*
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ClientUtils.HandleException("Connected Failed", ex);
                });*/
                return false;
            }
        }

        private const int retryCounter = 5;


        public bool WriteNode<T>( string NodeTreeString , T WriteValue)
        {
            bool ret = false;
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    ret = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
                    if (ret)
                        break;
                }
                catch (Exception ex)
                {

                }
            }
            return ret;
        }

        public bool ReadNode<T>(string NodeID ,out T NodeValue)
        {
            NodeValue = default;
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                    return true;
                }
                catch (Exception ex)
                {

                }
            }
            return false;
        }




        public void ReadNoteAttributes(string NodeTreeString)
        {
            OpcNodeAttribute[] nodeAttributes = m_OpcUaClient.ReadNoteAttributes(NodeTreeString);
            foreach (var item in nodeAttributes)
            {
                Console.Write(string.Format("{0,-30}", item.Name));
                Console.Write(string.Format("{0,-20}", item.Type));
                Console.Write(string.Format("{0,-20}", item.StatusCode));
                Console.WriteLine(string.Format("{0,20}", item.Value));
            }
            Console.WriteLine("-------------------------------------");
        }



        public bool ReadAllReference(string NodeTreeString , out List<NodeTypeValue> NodeValue)
        {
            NodeValue = new List<NodeTypeValue>();
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
            return false;
        }

        public class NodeTypeValue
        {
            public ExpandedNodeId NodeID { get; set; }
            public LocalizedText NodeDisplayName { get; set; }
            public object NodeValue { get; set; }
        }
        public void Disconnect()
        {
            m_OpcUaClient.Disconnect();
        }

    }
}
