using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace WPF_MVVM_BlackJackWIthCardCounting.Models
{
    class Model
    {
        private const int DefaultDecks = 6;
        private const int DefaultBet = 100;
        private const int DefaultWallet = 10000;
        private const int DefaultCount = 0;
        private const int DefaultRunningCount = 0;
        private const int DefaultTrueCount = 0;
        private const int DefaultSuggestedBet = DefaultBet;
        private const int DealerMin = 17;

        public static readonly int MinDecks = 1;
        public static readonly int MaxDecks = 10;
        public static readonly int MinBet = 25;
        public static readonly int MaxBet = 1000;

        private const int BetRaise = 25;

        private Shoe _shoe = new Shoe(DefaultDecks);
        private int _decks;
        private int _shoePercentageRemaining;
        private int _bet;
        private int _suggestedBet;
        private int _wallet;
        private int _count;
        private int _runningCount;
        private double _trueCount;
        private List<int> _playerHand;
        private List<int> _dealerHand;
        private int _gamePhase = 0;
        private string _gameResult = "";
        private int _winState = 0;

        public Model()
        {
            ResetModel();
        }

        public void ResetModel()
        {
            _decks = DefaultDecks;
            _bet = DefaultBet;
            _suggestedBet = DefaultSuggestedBet;
            _wallet = DefaultWallet;
            _count = DefaultCount;
            _runningCount = DefaultRunningCount;
            _trueCount = DefaultTrueCount;
            _gamePhase = 0;
            _playerHand = new List<int>();
            _dealerHand = new List<int>();
        }

        public void Deal()
        {
            _playerHand = new List<int>();
            _dealerHand = new List<int>();
            _runningCount = _count;

            if (_shoe.DeckPercentageRemaining < 50)
            {
                Shuffle();
            }

            for (int i = 0; i < 2; i++)
            {
                _playerHand.Add(_shoe.DealCard());
                _dealerHand.Add(_shoe.DealCard());
            }

            _wallet -= _bet;
            _gameResult = "";
            _gamePhase = 1;
            _winState = 0;

            EvaluateHands();
        }

        public void UpdateDecks(int change)
        {
            _decks += change;

            if (_decks < 1) _decks = 1;
            else if (_decks > 10) _decks = 10;
        }

        public void ChangeBet(int change)
        {
            _bet += change * BetRaise;
        }

        public void Hit(string player)
        {
            if (player.Equals("player")) _playerHand.Add(_shoe.DealCard());
            else _dealerHand.Add(_shoe.DealCard());

            EvaluateHands();
        }

        public void Stay()
        {
            _gamePhase = 2;

            while(CalculateHandSum(_dealerHand) < DealerMin)
            {
                _dealerHand.Add(_shoe.DealCard());
            }

            EvaluateHands();
        }

        public void Shuffle()
        {
            _shoe = new Shoe(_decks);
            _count = DefaultCount;
            _runningCount = DefaultRunningCount;
            _trueCount = DefaultTrueCount;
        }

        private void EvaluateHands()
        {
            CountCards();
            Console.WriteLine(_shoe.GetDecksRemaining());

            int playerSum = CalculateHandSum(_playerHand);
            int dealerSum = CalculateHandSum(_dealerHand);

            if (_gamePhase == 1) EvaluatePhase1Result(playerSum, dealerSum);
            else if(_gamePhase == 2) EvaluatePhase2Result(playerSum, dealerSum);

            if (_winState > 0) _gamePhase = 0;

            CalculatePayout();
        }

        public void EvaluatePhase1Result(int playerSum, int dealerSum)
        {
            if (_playerHand.Count == 2 && playerSum == 21)
            {
                if (dealerSum == 21)
                {
                    _gameResult = "BLACKJACK PUSH! Better luck next time!";
                    _winState = EndStates.Push;
                }
                else
                {
                    _gameResult = "BLACKJACK! You win!";
                    _winState = EndStates.Win;
                }
            }
            else if (_dealerHand.Count == 2 && dealerSum == 21)
            {
                _gameResult = "DEALER BLACKJACK! You lose!";
                _winState = EndStates.Lose;
            }
            else if (playerSum > 21)
            {
                _gameResult = "BUST! You lose!";
                _winState = EndStates.Lose;
            }
        }

        public void EvaluatePhase2Result(int playerSum, int dealerSum)
        {
            if (dealerSum > 21)
            {
                _gameResult = "DEALER BUSTS! You win!";
                _winState = EndStates.Win;
            }
            else if (dealerSum >= DealerMin)
            {
                if (dealerSum > playerSum)
                {
                    _gameResult = "Dealer has the higher score, you lose!";
                    _winState = EndStates.Lose;
                }
                else if (dealerSum == playerSum)
                {
                    _gameResult = "DRAW! No one wins!";
                    _winState = EndStates.Draw;
                }
                else
                {
                    _gameResult = "You win!";
                    _winState = EndStates.Win;
                }
            }
        }

        private void CalculatePayout()
        {
            switch(_winState)
            {
                case EndStates.Blackjack:
                    _wallet += 3 * _bet;
                    break;
                case EndStates.Win:
                    _wallet += 2 * _bet;
                    break;
                case EndStates.Lose:
                    break;
                case EndStates.Push:
                    _wallet += _bet;
                    break;
                case EndStates.Draw:
                    _wallet += _bet;
                    break;
            }
        }

        private int CalculateHandSum(List<int> hand)
        {
            int aces = 0;
            int sum = 0;
            foreach(int card in hand)
            {
                int faceValue = Shoe.GetCardFaceValue(card);
                
                if (faceValue < 14) sum += Shoe.GetCardPointValue(card);
                else aces++;
            }

            for(int i = 0; i < aces; i++)
            {
                if (sum < 10 || (sum == 10 && aces == 1)) sum += 11;
                else sum += 1;
            }
            
            return sum;
        }

        private void CountCards()
        {
            int roundCount = 0;

            foreach (int card in _playerHand)
            {
                int faceValue = Shoe.GetCardFaceValue(card);

                if (faceValue < 7) roundCount++;
                else if (faceValue > 9) roundCount--;
            }

            for(int i = 0; i < _dealerHand.Count; i++)
            {
                int faceValue = Shoe.GetCardFaceValue(_dealerHand[i]);

                if (_gamePhase == 2 || (_gamePhase == 1 && i == 0))
                {
                    Console.WriteLine(faceValue);
                    if (faceValue < 7) roundCount++;
                    else if (faceValue > 9) roundCount--;
                }
            }

            _count = _runningCount + roundCount;

            CalculateTrueCount();

            CalculateSuggestedBet();
        }

        public void CalculateTrueCount()
        {
            double decksRemaining = _shoe.GetDecksRemaining();

            double tempTrueCount = _count / decksRemaining;

            //Round True Count to nearest 0.5
            _trueCount = Math.Round(2 * tempTrueCount) / 2;
        }

        private void CalculateSuggestedBet()
        {
            double tempBet = (_trueCount - 1.0) * DefaultBet;

            //Round answer to the nearest MinBet unit
            int roundedBet = (int)(Math.Round(tempBet / MinBet) * MinBet);

            if (roundedBet < DefaultBet) _suggestedBet = DefaultBet;
            else _suggestedBet = roundedBet;
        }

        public ViewPackage BuildViewPackage()
        {
            ViewPackage viewPackage = new ViewPackage();

            viewPackage.Decks = _decks;
            viewPackage.DeckPercentageRemaining = _shoe.DeckPercentageRemaining;
            viewPackage.Bet = _bet;
            viewPackage.SuggestedBet = _suggestedBet;
            viewPackage.Wallet = _wallet;
            viewPackage.Count = _count;
            viewPackage.TrueCount = _trueCount;
            viewPackage.PlayerHand = _playerHand;
            viewPackage.DealerHand = _dealerHand;
            viewPackage.GamePhase = _gamePhase;
            viewPackage.GameResult = _gameResult;

            return viewPackage;
        }
    }
}
