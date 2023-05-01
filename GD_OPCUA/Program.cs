using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GD_OPCUA
{
    internal class Program
    {
        static void Main(string[] args)
        {

     
            Program.Test();
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static async void Test()
        {
            string IpString = "127.0.0.1";
            if (IPAddress.TryParse(IpString, out var _IPAddress))
            {
                var Opcua = new GD_OpcUaClient();

                if (await Opcua.ConnectAsync(new ServerUrlModel(_IPAddress, 100, "123")))
                {

                }
            }




        }


    }
}
