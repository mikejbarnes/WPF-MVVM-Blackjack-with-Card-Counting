using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_MVVM_BlackJackWIthCardCounting.Models;
using WPF_MVVM_BlackJackWIthCardCounting.ViewModels.Commands;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_MVVM_BlackJackWIthCardCounting.ViewModels
{
    class ViewModel : INotifyPropertyChanged
    {
        private readonly string[] Suites = new string[] { "♠", "♣", "♥", "♦" };
        private const string FaceCards = "JQKA";

        private Model _model = new Model();
        private int _decks;
        private int _deckPercentageRemaining;
        private int _bet;
        private int _suggestedBet;
        private int _wallet;
        private int _count;
        private double _trueCount;
        private List<string[]> _playerHand = new List<string[]>();
        private List<string[]> _dealerHand = new List<string[]>();
        private string _gameResult;
        private int _gamePhase;

        #region Properties with getters and setters to be used by the View
        public int Decks
        {
            get { return _decks; }
            set
            {
                _decks = value;
                OnPropertyChanged();
            }
        }

        public int DeckPercentageRemaining
        {
            get { return _deckPercentageRemaining; }
            set
            {
                _deckPercentageRemaining = value;
                OnPropertyChanged();
            }
        }

        public int Bet
        {
            get { return _bet; }
            set
            {
                _bet = value;
                OnPropertyChanged();
            }
        }

        public int SuggestedBet
        {
            get { return _suggestedBet; }
            set
            {
                _suggestedBet = value;
                OnPropertyChanged();
            }
        }

        public int Wallet
        {
            get { return _wallet; }
            set
            {
                _wallet = value;
                OnPropertyChanged();
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }

        public double TrueCount
        {
            get { return _trueCount; }
            set
            {
                _trueCount = value;
                OnPropertyChanged();
            }
        }

        public List<string[]> PlayerHand
        {
            get { return _playerHand; }
            set
            {
                _playerHand = value;
                OnPropertyChanged();
            }
        }

        public List<string[]> DealerHand
        {
            get { return _dealerHand; }
            set
            {
                _dealerHand = value;
                OnPropertyChanged();
            }
        }

        public string GameResult
        {
            get { return _gameResult; }
            set
            {
                _gameResult = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public RelayCommand IncreaseDecksCommand { get; set; }
        public RelayCommand DecreaseDecksCommand { get; set; }
        public RelayCommand IncreaseBetCommand { get; set; }
        public RelayCommand DecreaseBetCommand { get; set; }
        public RelayCommand DealCommand { get; set; }
        public RelayCommand HitCommand { get; set; }
        public RelayCommand StayCommand { get; set; }
        public RelayCommand ShuffleCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            IncreaseDecksCommand = new RelayCommand(IncreaseDecks, CanUseIncreaseDecks);
            DecreaseDecksCommand = new RelayCommand(DecreaseDecks, CanUseDecreaseDecks);
            IncreaseBetCommand = new RelayCommand(IncreaseBet, CanUseIncreaseBet);
            DecreaseBetCommand = new RelayCommand(DecreaseBet, CanUseDecreaseBet);
            HitCommand = new RelayCommand(Hit, CanUseHit);
            StayCommand = new RelayCommand(Stay, CanUseStay);
            DealCommand = new RelayCommand(Deal, CanUseDeal);
            ShuffleCommand = new RelayCommand(Shuffle, CanUseShuffle);
            ResetCommand = new RelayCommand(Reset, CanUseReset);

            UpdateProperties();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateProperties()
        {
            ViewPackage viewData = _model.BuildViewPackage();

            Decks = viewData.Decks;
            DeckPercentageRemaining = viewData.DeckPercentageRemaining;
            Bet = viewData.Bet;
            SuggestedBet = viewData.SuggestedBet;
            Wallet = viewData.Wallet;
            Count = viewData.Count;
            TrueCount = viewData.TrueCount;
            GameResult = viewData.GameResult;
            _gamePhase = viewData.GamePhase;

            PlayerHand = FormatCards(viewData.PlayerHand, _gamePhase, "player");
            DealerHand = FormatCards(viewData.DealerHand, _gamePhase, "dealer");
        }

        private List<string[]> FormatCards(List<int> hand, int gamePhase, string player)
        {
            List<string[]> formattedList = new List<string[]>();

            for (int i = 0; i < hand.Count; i++)
            {
                int faceValue = Shoe.GetCardFaceValue(hand[i]);
                int suiteIndex = Shoe.GetSuiteIndex(hand[i]);

                string cardString;
                if (player.Equals("dealer") && gamePhase == 1 && i == 1) cardString = "??";
                else cardString = BuildCardString(faceValue, suiteIndex);

                string cardColor = "Red";
                if (cardString.Equals("??") || suiteIndex < 2) cardColor = "Black";

                formattedList.Add(new string[] { cardString, cardColor });
            }

            //Fill in the rest of the list up to 11 to fill in the rest of the cards in the View, since a hand can be at most 11 cards long
            for (int i = 0; i < 11 - hand.Count; i++)
            {
                formattedList.Add(new string[] { "", "Black" });
            }

            return formattedList;
        }

        private string BuildCardString(int faceValue, int suiteIndex)
        {
            string suite = Suites[suiteIndex];

            string cardString = "";
            if (faceValue <= 10) cardString = String.Format("{0}{1}", faceValue, suite);
            else cardString = String.Format("{0}{1}", FaceCards[faceValue - 11], suite); //Converts the face card values to their equivalent index in the FaceCards string

            return cardString;
        }

        #region IncreaseDecks()
        public void IncreaseDecks()
        {
            _model.UpdateDecks(1);
            UpdateProperties();
        }
        
        public bool CanUseIncreaseDecks()
        {
            return _decks < Model.MaxDecks;
        }
        #endregion

        #region DecreaseDecks()
        public void DecreaseDecks()
        {
            _model.UpdateDecks(-1);
            UpdateProperties();
        }

        public bool CanUseDecreaseDecks()
        {
            return _decks > Model.MinDecks;
        }
        #endregion

        #region IncreaseBet()
        public void IncreaseBet()
        {
            _model.ChangeBet(1);
            UpdateProperties();
        }

        public bool CanUseIncreaseBet()
        {
            return _bet < Model.MaxBet && _gamePhase == 0;
        }

        #endregion

        #region DecreaseBet()
        public void DecreaseBet()
        {
            _model.ChangeBet(-1);
            UpdateProperties();
        }

        public bool CanUseDecreaseBet()
        {
            return _bet > Model.MinBet && _gamePhase == 0;
        }
        #endregion

        #region Shuffle
        public void Shuffle()
        {
            _model.Shuffle();
            UpdateProperties();
        }

        public bool CanUseShuffle()
        {
            return _gamePhase == 0;
        }

        #endregion

        #region Deal
        public void Deal()
        {
            _model.Deal();
            UpdateProperties();
        }

        public bool CanUseDeal()
        {
            return _gamePhase == 0;
        }
        #endregion

        #region Hit
        public void Hit()
        {
            _model.Hit("player");
            UpdateProperties();
        }

        public bool CanUseHit()
        {
            return _gamePhase == 1;
        }
        #endregion

        #region Stay
        public void Stay()
        {
            _model.Stay();
            UpdateProperties();
        }

        public bool CanUseStay()
        {
            return _gamePhase == 1;
        }
        #endregion

        #region Reset
        public void Reset()
        {
            _model.ResetModel();
            UpdateProperties();
        }

        public bool CanUseReset()
        {
            return true;
        }


        #endregion
    }
}
