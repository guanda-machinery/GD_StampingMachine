using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachineConnect
{
    public class ServerUrlModel
    {
        public ServerUrlModel(string _HostString, int _Port, string _Path)
        {
            Host = _HostString;
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
                var BaseUrl = new Uri($"opc.tcp://{Host}:{Port}");
                var CombineUrl = new Uri(BaseUrl, Path);
                return CombineUrl.ToString();
            }
        }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }

    }
}
