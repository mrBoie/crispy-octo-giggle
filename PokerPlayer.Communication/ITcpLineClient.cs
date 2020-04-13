using System;
using System.Threading.Tasks;

namespace PokerPlayer.Communication
{
    public interface ITcpLineClient
    {
        Task Connect(string url, int port);
        void Dispose();
        Task SendMessageAsync(string message);

        event EventHandler<string> OnMessageReceived;
    }
}