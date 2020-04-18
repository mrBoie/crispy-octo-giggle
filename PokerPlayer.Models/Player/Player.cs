using System;
using System.Collections.Generic;
using System.Text;

namespace PokerPlayer.Models.Player
{
    public class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }
    }
}
