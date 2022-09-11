using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_MVVM_BlackJackWIthCardCounting.Exceptions;

namespace WPF_MVVM_BlackJackWIthCardCounting.Models
{
    class Shoe
    {
        private const int CardsInDeck = 52;
        private const int CardsInSuite = 13;
        private const int RebuildLimit = 50;
        private int[] _cards;
        private int _shoeFullLength;
        private int _currentShoeLength;
        private int _cardsRemoved = 0;
        private int _deckPercentageRemaining;
        private Random _random = new Random();

        public int DeckPercentageRemaining { get { return _deckPercentageRemaining; } private set { _deckPercentageRemaining = value; } }

        public Shoe(int decks)
        {
            Shuffle(decks);
        }

        private void Shuffle(int decks)
        {
            _shoeFullLength = decks * CardsInDeck;
            _currentShoeLength = _shoeFullLength;
            _cards = new int[_shoeFullLength];
            _deckPercentageRemaining = 100;

            //Set all the cards in the shoe to their value, then later we can mark them as -1 when they are drawn to flag them from being drawn again and be removed upon a rebuild
            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i] = i;
            }
        }

        public int DealCard()
        {
            int drawnCard; ;
            do
            {
                drawnCard = _random.Next(_cards.Length);
            } while (_cards[drawnCard] < 0);

            //Change value to -1 to flag the card as being already drawn
            _cards[drawnCard] = -1;
            _cardsRemoved++;
            _currentShoeLength--;
            _deckPercentageRemaining = (_currentShoeLength * 100) / _shoeFullLength;

            return drawnCard;
        }

        public static int GetCardFaceValue(int card)
        {
            //Add 2 to each card since the first card of each suite grouping is at index 0 and the lowest card value is 2
            return card % CardsInSuite + 2;
        }

        public static int GetCardPointValue(int card)
        {
            ///Returns the point value of a card that is not an Ace. Uses raw card value, not face value.
            
            int faceValue = GetCardFaceValue(card);
            
            if (faceValue <= 9) return faceValue;
            else if (faceValue < 14) return 10;
            else throw new IncorrectAcePointValueException();
        }

        public static int GetAcePointValue(int handSum, int aces)
        {
            if (handSum < 9 || (handSum == 10 && aces == 1)) return 11;
            else return 1;
        }

        public static int GetSuiteIndex(int card)
        {
            return (card % CardsInDeck) / CardsInSuite;
        }

        public double GetDecksRemaining()
        {
            int cardsRemaining = 0;
            foreach(int card in _cards)
            {
                if (card >= 0) cardsRemaining++;
            }

            double decksRemaining = (int)Math.Round(((double)cardsRemaining / CardsInDeck));

            //Round result to the nearest half-deck
            double roundedResult = Math.Round(2 * decksRemaining) / 2;

            return roundedResult;
        }
    }
}
