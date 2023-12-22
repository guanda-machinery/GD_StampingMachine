using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class TcpPing
    {
        /// <summary>
        /// 字串內包含ip位址
        /// </summary>
        /// <returns></returns>
        public static bool IsContainIpAddress(string input)
        {
            return RetrieveIpAddress(input, out var ip);
        }
        /// <summary>
        /// 從字串中擷取出ip
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool RetrieveIpAddress(string input, out IPAddress ipAddress)
        {
            ipAddress = null;
            string pattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b"; // 正则表达式匹配 IP 地址
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                if (IPAddress.TryParse(match.Value, out ipAddress))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPingable(string host)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send(host);
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    return false;
                }
            }
        }

        public static bool IsPingable(IPAddress ip)
        {
            return IsPingable(ip.ToString());
        }

        public async static Task<bool> IsPingableAsync(string host)
        {
            using (Ping ping = new Ping() )
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(host, 1000);
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    return false;
                }
            }
        }

        public async static Task<bool> IsPingableAsync(IPAddress ip)
        {
            return await IsPingableAsync(ip.ToString());
        }




    }
}
