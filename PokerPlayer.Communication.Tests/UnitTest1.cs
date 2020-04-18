using Microsoft.Extensions.Logging;
using NSubstitute;
using PokerPlayer.Models.CardModels;
using PokerPlayer.Models.Player;
using System;
using System.Collections.Generic;
using Xunit;

namespace PokerPlayer.Communication.Tests
{
    public class PokerClientTests : IDisposable
    {
        private readonly ITcpLineClient _tcpClient;
        private readonly ILogger<PokerClient> _logger;
        private readonly PokerClient _pokerClient;

        public PokerClientTests()
        {
            _tcpClient = Substitute.For<ITcpLineClient>();
            _logger = Substitute.For<ILogger<PokerClient>>();

            _pokerClient = new PokerClient(_tcpClient, _logger);
            _pokerClient.Connect("localhost", 5000);
        }

        [Fact]
        public void PokerClient_OnMessageReceived_NotifiedNameRequest()
        { 
            var messagesReceived = new List<(object sender, string message)>();
            _pokerClient.OnNameRequested += (sender, message) => messagesReceived.Add((sender, message));

            _tcpClient.OnMessageReceived += Raise.Event<EventHandler<string>>(_tcpClient, "Name?");

            _tcpClient.Received(1).Connect("localhost", 5000);
            Assert.Single(messagesReceived);
            Assert.Equal(_pokerClient, messagesReceived[0].sender);
            Assert.Equal("Name?", messagesReceived[0].message);
        }

        [Fact]
        public void PokerClient_OnMessageReceived_NotifiedRoundNumberStart()
        {
            var messagesReceived = new List<(object sender, int message)>();
            _pokerClient.OnRoundStarting += (sender, message) => messagesReceived.Add((sender, message));

            _tcpClient.OnMessageReceived += Raise.Event<EventHandler<string>>(_tcpClient, "Round 1");

            _tcpClient.Received(1).Connect("localhost", 5000);
            Assert.Single(messagesReceived);
            Assert.Equal(_pokerClient, messagesReceived[0].sender);
            Assert.Equal(1, messagesReceived[0].message);
        }

        [Fact]
        public void PokerClient_OnMessageReceived_NotifiedHandedCards()
        {
            var messagesReceived = new List<(object sender, IEnumerable<Card> message)>();
            _pokerClient.OnHandedCards += (sender, message) => messagesReceived.Add((sender, message));

            _tcpClient.OnMessageReceived += Raise.Event<EventHandler<string>>(_tcpClient, "Cards 7h Kc 6s Th 3c");

            _tcpClient.Received(1).Connect("localhost", 5000);
            Assert.Single(messagesReceived);
            Assert.Equal(_pokerClient, messagesReceived[0].sender);

            var cards = messagesReceived[0].message;

            Assert.Collection(cards,
                (c) => AssertCard(c, Color.Hearts, Value.Seven, "7h"),
                (c) => AssertCard(c, Color.Clubs, Value.King, "Kc"),
                (c) => AssertCard(c, Color.Spades, Value.Six, "6s"),
                (c) => AssertCard(c, Color.Hearts, Value.Ten, "Th"),
                (c) => AssertCard(c, Color.Clubs, Value.Three, "3c")
                );
        }

        private void AssertCard(Card card, Color color, Value value, string cardAsString)
        {
            Assert.Equal(color, card.CardColor);
            Assert.Equal(value, card.CardValue);
            Assert.Equal(cardAsString, card.CardAsString);
        }

        [Fact]
        public void PokerClient_OnMessageReceived_NotifiedPlayerChipUpdated()
        {
            var messagesReceived = new List<(object sender, PlayerChipUpdate message)>();
            _pokerClient.OnPlayerChipUpdate += (sender, message) => messagesReceived.Add((sender, message));

            _tcpClient.OnMessageReceived += Raise.Event<EventHandler<string>>(_tcpClient, "Chips Client#3526 163");

            _tcpClient.Received(1).Connect("localhost", 5000);
            Assert.Single(messagesReceived);
            Assert.Equal(_pokerClient, messagesReceived[0].sender);
            Assert.Equal("Client#3526", messagesReceived[0].message.Name);
            Assert.Equal(163, messagesReceived[0].message.Chips);
        }

        public void Dispose()
        {
            _pokerClient.Dispose();
        }
    }
}
