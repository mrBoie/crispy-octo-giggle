using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Hello");

            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                
                                .Build();

            var lf = NLog.LogManager.LoadConfiguration("nlog.config");

            var di = BuildDi(config);

            using(di as IDisposable){
                var client = di.GetRequiredService<PokerClient>();
                client.OnNameRequested += OnNameRequest;
                client.OnRoundStarting += Client_OnRoundStarting;

                client.Connect("localhost", 5000);

                Console.ReadLine();
            }            

            Console.ReadLine();
        }

        private static void Client_OnRoundStarting(object sender, int e)
        {
            Console.WriteLine("Starting Round: " + e);
        }

        public static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
                            .AddSingleton<PokerClient>()
                            .AddLogging(loggingBuilder =>
                                { 
                                    loggingBuilder.ClearProviders();
                                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                                    loggingBuilder.AddNLog(config);
                                })
                            .BuildServiceProvider();
                                

        }

        private static void OnNameRequest(object sender, string message)
        {
            Console.WriteLine($"Received {message}");
            var client = sender as PokerClient;

            client.SetName(DateTime.Now.Ticks.ToString());
        }

        private static void SetName()
        {
            using(var tcpClient = new TcpClient())
            {
                tcpClient.Connect("localhost", 5000);

                var writer = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);
                var reader = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);

                var result = reader.ReadLine();
                Console.WriteLine(result);
            }
        }

        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port)
        {
            string request = "Name Client#1345\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);

            Byte[] bytesReceived = new Byte[30];
            string page = "";

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port))
            {

                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                

                // Receive the server home page content.
                int bytes = 0;

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, 6, 0);

                    s.Send(bytesSent);
                }
                while (bytes > 0);
            }

            return page;
        }


        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

    }
}
