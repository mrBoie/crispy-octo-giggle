using System;

namespace PokerPlayer.Models.CardModels
{
    public class Card
    {
        public string CardAsString { get; set; }

        public Color CardColor { get; set; }

        public Value CardValue { get; set; }

        //7h Kc 6s Th 3c
        public Card(string cardString)
        {
            CardAsString = cardString;
            CardValue = (Value) CardAsString[0];
            CardColor = (Color) CardAsString[1];
        }
    }
}
