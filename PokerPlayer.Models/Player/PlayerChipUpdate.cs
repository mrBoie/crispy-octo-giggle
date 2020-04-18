using System;
using System.Collections.Generic;
using System.Text;

namespace PokerPlayer.Models.Player
{
    public class PlayerChipUpdate : Player
    {
        public int Chips { get; set; }

        public PlayerChipUpdate(string[] input) : base(input[1])
        {
            Chips = int.Parse(input[2]);
        }
    }
}
