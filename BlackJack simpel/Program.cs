using System;

namespace BlackjackGame
{
    class Program
    {
        static Random random = new Random();
        static int playerBalance = 100; // Start penge

        static void Main(string[] args)
        {
            bool playAgain = true;

            while (playAgain && playerBalance > 0)
            {
                Console.Clear();
                Console.WriteLine("Velkommen til Blackjack!\n");
                Console.WriteLine($"Din nuværende saldo: {playerBalance} chips");

                int bet = GetBet();

                StartGame(bet);

                if (playerBalance <= 0)
                {
                    Console.WriteLine("\nDu har ingen penge tilbage! Spillet er slut.");
                    break;
                }

                Console.WriteLine("\nVil du spille igen? (j/n)");
                string again = Console.ReadLine().ToLower();
                playAgain = (again == "j");
            }

            Console.WriteLine("Tak for spillet!");
        }

        static int GetBet()
        {
            int bet = 0;
            bool validBet = false;

            while (!validBet)
            {
                Console.Write("Hvor meget vil du satse? ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out bet) && bet > 0 && bet <= playerBalance)
                {
                    validBet = true;
                }
                else
                {
                    Console.WriteLine("Ugyldigt bet. Indtast et tal mellem 1 og din saldo.");
                }
            }

            return bet;
        }

        static void StartGame(int bet)
        {
            string[] playerHand = new string[0];
            string[] dealerHand = new string[0];

            // Start-hænder
            playerHand = AddCard(playerHand, DrawCard());
            playerHand = AddCard(playerHand, DrawCard());

            dealerHand = AddCard(dealerHand, DrawCard());
            dealerHand = AddCard(dealerHand, DrawCard());

            // Tjek for Blackjack
            bool playerBlackjack = (HandValue(playerHand) == 21 && playerHand.Length == 2);
            bool dealerBlackjack = (HandValue(dealerHand) == 21 && dealerHand.Length == 2);

            if (playerBlackjack || dealerBlackjack)
            {
                DetermineBlackjackWinner(playerHand, dealerHand, bet, playerBlackjack, dealerBlackjack);
                return; // runden slutter her
            }

            bool playerBust = PlayerTurn(ref playerHand, dealerHand);

            if (!playerBust)
            {
                DealerTurn(ref dealerHand);
            }

            DetermineWinner(playerHand, dealerHand, playerBust, bet);
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

        static void DetermineBlackjackWinner(string[] playerHand, string[] dealerHand, int bet, bool playerBlackjack, bool dealerBlackjack)
        {
            Console.WriteLine("\n--- Blackjack! ---");
            Console.WriteLine($"Dine kort: {string.Join(", ", playerHand)} (Total: {HandValue(playerHand)})");
            Console.WriteLine($"Dealerens kort: {string.Join(", ", dealerHand)} (Total: {HandValue(dealerHand)})");

            if (playerBlackjack && dealerBlackjack)
            {
                Console.WriteLine("Begge har Blackjack! Uafgjort – du får dit bet tilbage.");
            }
            else if (playerBlackjack)
            {
                int winAmount = (int)(bet * 1.5);
                Console.WriteLine($"Blackjack! Du vinder {winAmount} chips!");
                playerBalance += winAmount;
            }
            else
            {
                Console.WriteLine($"Dealer har Blackjack! Du tabte {bet} chips.");
                playerBalance -= bet;
            }

            Console.WriteLine($"Din nye saldo: {playerBalance} chips");
        }

        static void DetermineWinner(string[] playerHand, string[] dealerHand, bool playerBust, int bet)
        {
            int playerTotal = HandValue(playerHand);
            int dealerTotal = HandValue(dealerHand);

            Console.WriteLine("\n--- Resultat ---");
            Console.WriteLine($"Dine kort: {string.Join(", ", playerHand)} (Total: {playerTotal})");
            Console.WriteLine($"Dealerens kort: {string.Join(", ", dealerHand)} (Total: {dealerTotal})");

            if (playerBust)
            {
                Console.WriteLine($"Du tabte {bet} chips! (Bust)");
                playerBalance -= bet;
            }
            else if (dealerTotal > 21)
            {
                Console.WriteLine($"Dealer går bust! Du vinder {bet} chips!");
                playerBalance += bet;
            }
            else if (playerTotal > dealerTotal)
            {
                Console.WriteLine($"Du vinder {bet} chips!");
                playerBalance += bet;
            }
            else if (playerTotal < dealerTotal)
            {
                Console.WriteLine($"Dealer vinder! Du tabte {bet} chips.");
                playerBalance -= bet;
            }
            else
            {
                Console.WriteLine("Uafgjort! Du får dit bet tilbage.");
            }

            Console.WriteLine($"Din nye saldo: {playerBalance} chips");
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
