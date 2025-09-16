using System;

namespace BlackjackGame
{
    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            bool playAgain = true;

            while (playAgain)
            {
                Console.Clear();
                Console.WriteLine("Velkommen til Blackjack!\n");

                StartGame();

                Console.WriteLine("\nVil du spille igen? (j/n)");
                string again = Console.ReadLine().ToLower();
                playAgain = (again == "j");
            }

            Console.WriteLine("Tak for spillet!");
        }

        static void StartGame()
        {
            string[] playerHand = new string[0];
            string[] dealerHand = new string[0];

            // Start-hænder
            playerHand = AddCard(playerHand, DrawCard());
            playerHand = AddCard(playerHand, DrawCard());

            dealerHand = AddCard(dealerHand, DrawCard());
            dealerHand = AddCard(dealerHand, DrawCard());

            bool playerBust = PlayerTurn(ref playerHand, dealerHand);

            if (!playerBust)
            {
                DealerTurn(ref dealerHand);
            }

            DetermineWinner(playerHand, dealerHand, playerBust);
        }

        static bool PlayerTurn(ref string[] playerHand, string[] dealerHand)
        {
            bool playerTurn = true;
            bool playerBust = false;

            while (playerTurn)
            {
                Console.WriteLine("Dine kort: " + string.Join(", ", playerHand) + $" (Total: {HandValue(playerHand)})");
                Console.WriteLine($"Dealers kort: {dealerHand[0]}, ?");

                Console.WriteLine("\nVil du [H]it eller [S]tand?");
                string choice = Console.ReadLine().ToLower();

                if (choice == "h")
                {
                    playerHand = AddCard(playerHand, DrawCard());
                    if (HandValue(playerHand) > 21)
                    {
                        Console.WriteLine("Du trækker et kort og går bust!");
                        playerBust = true;
                        playerTurn = false;
                    }
                }
                else if (choice == "s")
                {
                    playerTurn = false;
                }
            }

            return playerBust;
        }

        static void DealerTurn(ref string[] dealerHand)
        {
            Console.WriteLine("\n--- Dealerens tur ---");
            Console.WriteLine("Dealerens kort: " + string.Join(", ", dealerHand) + $" (Total: {HandValue(dealerHand)})");

            while (HandValue(dealerHand) < 17)
            {
                Console.WriteLine("Dealer trækker et kort...");
                dealerHand = AddCard(dealerHand, DrawCard());
                Console.WriteLine("Dealerens kort: " + string.Join(", ", dealerHand) + $" (Total: {HandValue(dealerHand)})");
            }
        }

        static void DetermineWinner(string[] playerHand, string[] dealerHand, bool playerBust)
        {
            int playerTotal = HandValue(playerHand);
            int dealerTotal = HandValue(dealerHand);

            Console.WriteLine("\n--- Resultat ---");
            Console.WriteLine($"Dine kort: {string.Join(", ", playerHand)} (Total: {playerTotal})");
            Console.WriteLine($"Dealerens kort: {string.Join(", ", dealerHand)} (Total: {dealerTotal})");

            if (playerBust)
            {
                Console.WriteLine("Du tabte! (Bust)");
            }
            else if (dealerTotal > 21)
            {
                Console.WriteLine("Dealer går bust! Du vinder!");
            }
            else if (playerTotal > dealerTotal)
            {
                Console.WriteLine("Du vinder!");
            }
            else if (playerTotal < dealerTotal)
            {
                Console.WriteLine("Dealer vinder!");
            }
            else
            {
                Console.WriteLine("Uafgjort!");
            }
        }

        // ---------------- HJÆLPEFUNKTIONER ----------------

        static string DrawCard()
        {
            string[] cards = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            return cards[random.Next(cards.Length)];
        }

        static int CardValue(string card)
        {
            if (card == "J" || card == "Q" || card == "K")
                return 10;
            else if (card == "A")
                return 11; // håndteres senere hvis bust
            else
                return int.Parse(card);
        }

        static int HandValue(string[] hand)
        {
            int total = 0;
            int aceCount = 0;

            foreach (string card in hand)
            {
                int value = CardValue(card);
                total += value;
                if (card == "A") aceCount++;
            }

            // Juster for esser (A = 1 i stedet for 11 hvis bust)
            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }

            return total;
        }

        static string[] AddCard(string[] hand, string card)
        {
            string[] newHand = new string[hand.Length + 1];
            for (int i = 0; i < hand.Length; i++)
            {
                newHand[i] = hand[i];
            }
            newHand[newHand.Length - 1] = card;
            return newHand;
        }
    }
}
