using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class PokerClient : IPokerClient, IDisposable
    {
        private ITcpLineClient _tcpLineClient;
        private readonly ILogger<PokerClient> _logger;

        public event EventHandler<string> OnNameRequested;
        public event EventHandler<int> OnRoundStarting;

        public PokerClient(ILogger<PokerClient> logger)
        {
            _tcpLineClient = new TcpLineClient();
            _logger = logger;
            _logger.LogInformation("ctor PokerClient");
        }

        public void Connect(string url, int port)
        {
            _tcpLineClient.OnMessageReceived += _tcpLineClient_OnMessageReceived;
            _tcpLineClient.Connect(url, port);
        }

        private void _tcpLineClient_OnMessageReceived(object sender, string message)
        {
            _logger.LogInformation($"Received for :{message}");
            var messageDivided = message.Split(' ');
            switch (messageDivided.First())
            {
                case "Name?":
                    OnNameRequested.Invoke(this, message);
                    break;
                case "Round":
                    OnRoundStarting.Invoke(this, int.Parse(messageDivided.Last()));
                    break;
                case "Cards": //Cards 7h Kc 6s Th 3c
                case "Chips": //Chips Client#3526 163
                case "Round_Win_Undisputed": //'Round_Win_Undisputed Client#107 22'
                case "Ante_Changed": //Ante_Changed 10
                case "Forced_Bet": //Forced_Bet Client#548 10
                case "Player_Call": //Player_Call Client#3526 
                case "Player_All-in": //Player_All-in Client#3698 200
                case "Player_Fold": //Player_Fold Client#107 
                case "Player_Draw": //Player_Draw Client#3434 0
                case "Player_Hand": //'Player_Hand Client#3434 9h Ad 8d 7s Qc'
                case "Call/Raise?": //Call/Raise? 18 26 10 190
                case "Draw?":
                case "Open?": //'Open? 11 10 680'
                case "Result": //Result Client#548 2 0
                case "Game_Over":
                default:
                    _logger.LogInformation($"missing Response for :{message}");
                    break;
            }
        }

        public async void SetName(string name)
        {
            var namePrefix = "Name ";
            await _tcpLineClient.SendMessageAsync(namePrefix + name);
        }

        public void Dispose()
        {
            _tcpLineClient.OnMessageReceived -= _tcpLineClient_OnMessageReceived;
            _tcpLineClient?.Dispose();
        }
    }

    public interface IPokerClient
    {
        void Connect(string url, int port);

        event EventHandler<string> OnNameRequested;

        event EventHandler<int> OnRoundStarting;
    }
}
