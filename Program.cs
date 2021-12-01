using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace PokerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HandParser();
            string input = Console.ReadLine();
            Console.WriteLine(parser.GetHandName(input));
        }
    }

    public class HandParser
    {
        private readonly AToBConverter<char, Rank> _rankConverter = new AToBConverter<char, Rank>(_rankConversions);
        private readonly AToBConverter<char, Suit> _suitConverter = new AToBConverter<char, Suit>(_suitConversions);

        private static readonly IDictionary<char, Rank> _rankConversions = new Dictionary<char, Rank>()
    {
        {'A', Rank.Ace},
        {'2', Rank.Two},
        {'3', Rank.Three},
        {'4', Rank.Four},
        {'5', Rank.Five},
        {'6', Rank.Six},
        {'7', Rank.Seven},
        {'8', Rank.Eight},
        {'9', Rank.Nine},
        {'T', Rank.Ten},
        {'t', Rank.Ten},
        {'J', Rank.Jack},
        {'Q', Rank.Queen},
        {'O', Rank.Queen},
        {'K', Rank.King},
    };
        private static readonly IDictionary<char, Suit> _suitConversions = new Dictionary<char, Suit>()
    {
        {'H', Suit.Hearts},
        {'S', Suit.Spades},
        {'D', Suit.Diamonds},
        {'C', Suit.Diamonds}
    };
        public string GetHandName(string input)
        {
            var splitter = new StringSplitter();
            splitter.AddCharacterToSplitOn(',');
            IEnumerable<string> tokens = splitter.Split(input.ToUpper());

            List<Card<Suit, Rank>> tempList = new List<Card<Suit, Rank>>();
            var SuitRank = new char[2];

            // TODO: Get list of cards from input rather than hard coding
            for (int count = 0; count < tokens.Count(); count++)
            {
                SuitRank = tokens.ElementAt(count).ToCharArray();
                tempList.Add(new Card<Suit, Rank>((Suit)ConvertSuit(SuitRank[1]).Value, (Rank)ConvertRank(SuitRank[0]).Value));
            }

            // Pair
            IMatch<Suit, Rank> twoOfAKind = new TwoOfAKind<Suit, Rank>();

            if (twoOfAKind.IsMatch(tempList))
            {
                return "Pair";
            }

            // Flush (All cards)
            IMatch<Suit, Rank> flush = new Flush<Suit, Rank>();

            if (flush.IsMatch(tempList))
            {
                return "Flush";
            }

            // Three of a kind
            IMatch<Suit, Rank> threeOfAKind = new ThreeOfAKind<Suit, Rank>();

            if (threeOfAKind.IsMatch(tempList))
            {
                return "ThreeOfAKind";
            }

            return "Not Implemented";
        }



    private Rank? ConvertRank(char input)
    {
        return _rankConverter.Convert(input);
    }

    private Suit? ConvertSuit(char input)
    {
        return _suitConverter.Convert(input);
    }

        public interface IMatch<TSuit, TRank>
    {
        bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards);
    }

    public class TwoOfAKind<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            return cards.GroupBy(c => c.Rank).Any(g => g.Count() >= 2);
        }
    }

    public class Flush<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            return cards.GroupBy(c => c.Suit).Any(g => g.Count() == g.Count());
        }
    }

    public class ThreeOfAKind<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            return cards.GroupBy(c => c.Rank).Any(g => g.Count() >= 3);
        }
    }

    public enum Rank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum Suit
    {
        Hearts,
        Spades,
        Diamonds,
        Clubs
    }

    public class Card<TSuit, TRank>
    {
        public Card(TSuit suit, TRank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public TSuit Suit { get; private set; }
        public TRank Rank { get; private set; }
    }

    public class AToBConverter<TA, TB> where TB : struct
    {
        private readonly IDictionary<TA, TB> _conversions;

        public AToBConverter(IDictionary<TA, TB> conversions)
        {
            _conversions = conversions;
        }

        public TB? Convert(TA input)
        {
            TB output;
            if (_conversions.TryGetValue(input, out output))
            {
                return output;
            }

            return null;
        }
    }

    public class StringSplitter
    {
        IList<char> _splitChars = new List<char>();

        public void AddCharacterToSplitOn(char splitCharacter)
        {
            _splitChars.Add(splitCharacter);
        }

        public IEnumerable<string> Split(string input)
        {
            return input.Split(_splitChars.ToArray())
                .Where(c => !String.IsNullOrEmpty(c));
        }
    }
}
}