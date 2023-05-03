using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Opc.Ua;
using OpcUaHelper;

namespace GD_StampingMachineConnect
{

    //https://www.cnblogs.com/daji2020/p/16627627.html

    public class GD_StampingMachineConnectHelperClient : StampingMachineConnectInterface
    {
        readonly OpcUaClient  m_OpcUaClient;
        public GD_StampingMachineConnectHelperClient()
        {
            m_OpcUaClient = new();
        }
        //X509憑證 X509Certificate2 Certificate
        //m_OpcUaClient.UserIdentity ??= new UserIdentity(Certificate);
        public IUserIdentity UserIdentity
        {
            get=> m_OpcUaClient.UserIdentity; 
            set => m_OpcUaClient.UserIdentity = value;
        }
        
        public async Task<bool> ConnectAsync(string HostPath, int Port, string DataPath = null)
        {
            var BaseUrl = new Uri($"opc.tcp://{HostPath}:{Port}");
            var CombineUrl = new Uri(BaseUrl, DataPath);
            try
            {
                await m_OpcUaClient.ConnectServer(CombineUrl.ToString());
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        
        public bool WriteNode<T>(T WriteValue , string NodeTreeString)
        {
            try
            {
                return m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool ReadNode<T>(string NodeID ,out T NodeValue)
        {
            NodeValue = default(T);
            try
            {
                NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                return true;
            }
            catch (Exception ex)
            {

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

        public void ReadAllReference(string NodeTreeString = "ns=2;s=Devices")
        {
            List<ReferenceDescription> referencesList = new List<ReferenceDescription>();
            var NodeIDStringList = new List<string>() { NodeTreeString }; 
            var ExistedNodeIDStringList = new List<string>() ;
            while (true)
            {
                var NodeSearchList = NodeIDStringList.Except(ExistedNodeIDStringList);
                var NextSearchList = new List<string>() ;
                foreach (var NodeIDString in NodeSearchList)
                {
                    ReferenceDescription[] references = m_OpcUaClient.BrowseNodeReference(NodeIDString);
                    foreach (var Ref in references)
                    {
                        //展開
                        NextSearchList.Add(Ref.NodeId.ToString());
                        referencesList.Add(Ref);
                    }
                }
                ExistedNodeIDStringList.AddRange(NodeSearchList);
                NodeIDStringList.AddRange(NextSearchList);

                if (NextSearchList.Count == 0)
                    break;
            }


            List<NodeTypeValue> GetNodeValue = new List<NodeTypeValue>();
            referencesList.ForEach(reference =>
            {
                if (reference.NodeClass == NodeClass.Variable)
                {
                    ReadNoteAttributes(reference.NodeId.ToString());
                    ReadNode(reference.NodeId.ToString(), out double NValue);
                    GetNodeValue.Add(new NodeTypeValue()
                    {
                        NodeID = reference.NodeId,
                        NodeDisplayName = reference.DisplayName,
                        NodeType = typeof(string),
                        NodeValue = NValue
                    });
                }

            });
        }

        public class NodeTypeValue
        {
            public ExpandedNodeId NodeID { get; set; }
            public LocalizedText NodeDisplayName { get; set; }
            public Type NodeType { get; set; }
            public object NodeValue { get; set; }
        }




        public void Disconnect()
        {
            m_OpcUaClient.Disconnect();
        }

        public bool GetMachineStatus(out string Status)
        {
            throw new NotImplementedException();
        }
    }
}
