using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM_BlackJackWIthCardCounting.Models
{
    class ViewPackage
    {
        public int Decks { get; set; }
        public int DeckPercentageRemaining { get; set; }
        public int Bet { get; set; }
        public int SuggestedBet { get; set; }
        public int Wallet { get; set; }
        public int Count { get; set; }
        public double TrueCount { get; set; }
        public List<int> PlayerHand { get; set; }
        public List<int> DealerHand { get; set; }
        public int GamePhase { get; set; }
        public string GameResult { get; set; }
    }
}
