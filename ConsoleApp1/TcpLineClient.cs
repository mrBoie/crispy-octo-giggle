using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TcpLineClient : IDisposable, ITcpLineClient
    {
        private TcpClient tcpClient;
        private StreamReader _reader;
        private StreamWriter _writer;

        public event EventHandler<string> OnMessageReceived;

        public TcpLineClient()
        {
            tcpClient = new TcpClient();
        }

        public async Task Connect(string url, int port)
        {
            tcpClient.Connect("localhost", 5000);
            _reader = new StreamReader(tcpClient.GetStream(), Encoding.ASCII);
            _writer = new StreamWriter(tcpClient.GetStream(), Encoding.ASCII);

            while (tcpClient.Connected)
            {
                var result = await _reader.ReadLineAsync();
                Thread.Sleep(1000);
                OnMessageReceived.Invoke(this, result);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            await _writer.WriteLineAsync(message);
            _writer.Flush();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            tcpClient?.Dispose();
        }
    }
}
