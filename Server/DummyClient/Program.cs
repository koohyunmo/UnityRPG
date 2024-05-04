// See https://aka.ms/new-console-template for more information
using DummyClient.Session;
using ServerCore;
using System.Diagnostics;
using System.Net;


namespace DummyClient
{


    class Program
    {
        static int DummyClientCount { get; } = 10;
        static void Main(string[] args)
        {

            Thread.Sleep(3000);
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[6];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            Console.WriteLine("Connect..");

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                Program.DummyClientCount);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}


