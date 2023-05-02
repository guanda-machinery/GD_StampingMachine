using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using OpcUaHelper;

namespace GD_OPCUA
{
    public class ServerUrlModel
    {
        public ServerUrlModel(IPAddress _IP , int _Port, string _Path)
        {
            IP = _IP;
            Port = _Port;
            Path = _Path;
        }

        /// <summary>
        /// 合併後網址
        /// </summary>
        public string ServerUrlString
        {
            get


            {
                var BaseUrl = new Uri ($"opc.tcp://{IP}:{Port}");
                var CombineUrl = new Uri(BaseUrl , Path);
                return CombineUrl.ToString(); 
            }
        }
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }

    }


    //https://www.cnblogs.com/daji2020/p/16627627.html

    public class GD_OpcUaClient
    {
        OpcUaClient m_OpcUaClient;
        public GD_OpcUaClient()
        {
            m_OpcUaClient = new();
        }

        public GD_OpcUaClient(IUserIdentity UserIdentity = null)
        {
            m_OpcUaClient = new();
            m_OpcUaClient.UserIdentity ??= UserIdentity;
        }
        public GD_OpcUaClient(X509Certificate2 Certificate)
        {
            m_OpcUaClient = new();
            m_OpcUaClient.UserIdentity ??= new UserIdentity(Certificate);
        }

        public async Task<bool> ConnectAsync(ServerUrlModel ServerUrl)
        {
            try
            {
                await m_OpcUaClient.ConnectServer(ServerUrl.ServerUrlString);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public void ReadNote()
        {
            try
            {
                Int32 value = m_OpcUaClient.ReadNode<Int32>("ns=2;s=数据类型示例.16 位设备.R 寄存器.Long4");
            }
            catch (Exception ex)
            {
         
            }

        }



    }
}
