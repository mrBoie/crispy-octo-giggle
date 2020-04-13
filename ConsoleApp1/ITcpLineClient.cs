using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface ITcpLineClient
    {
        Task Connect(string url, int port);
        void Dispose();
        Task SendMessageAsync(string message);

        event EventHandler<string> OnMessageReceived;
    }
}