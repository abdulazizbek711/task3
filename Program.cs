using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

class RockPaperScissors
{
    private readonly string[] moves;
    private byte[]? key;

    public RockPaperScissors(string[] moves)
    {
        if (moves == null || moves.Length < 3 || moves.Length % 2 == 0 || HasDuplicates(moves))
        {
            Console.WriteLine("Incorrect arguments. Please provide an odd number of non-repeating strings.");
            Console.WriteLine("Example: Rock Paper Scissors");
            Environment.Exit(0);
        }

        this.moves = moves;
    }

    private bool HasDuplicates(string[] arr)
    {
        HashSet<string> set = new HashSet<string>();
        foreach (var item in arr)
        {
            if (!set.Add(item))
            {
                return true;
            }
        }
        return false;
    }

    private void GenerateKey()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            int byteLength = 32; // 256 bits
            key = new byte[byteLength];
            rng.GetBytes(key);
        }
    }

    private byte[] CalculateHmac(byte[] message)
    {
        using (var hmac = new HMACSHA256(key!))
        {
            return hmac.ComputeHash(message);
        }
    }

    private void DisplayHelpTable()
    {
        int n = moves.Length;

        Console.WriteLine("Help Table:");
        Console.Write("    ");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"{i + 1}  ");
        }
        Console.WriteLine();

        for (int i = 0; i < n; i++)
        {
            Console.Write($"{i + 1}   ");
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    Console.Write("Draw ");
                else if ((i + 1) % n == j || (i + n - 1) % n == j)
                    Console.Write("Win  ");
                else
                    Console.Write("Lose ");
            }
            Console.WriteLine();
        }
    }

    public void Start()
    {
        GenerateKey();

        Console.WriteLine($"HMAC key: {BitConverter.ToString(key!).Replace("-", "").ToLower()}");

        for (int i = 0; i < moves.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {moves[i]}");
        }

        Console.WriteLine("0 - exit");
        Console.WriteLine("? - help");

        int userMove;
        do
        {
            Console.Write("Enter your move: ");
            string? input = Console.ReadLine()?.Trim();

            if (input == "0")
            {
                Console.WriteLine("Exiting the game. Goodbye!");
                Environment.Exit(0);
            }
            else if (input == "?")
            {
                DisplayHelpTable();
                continue;
            }

            if (int.TryParse(input, out userMove) && userMove >= 1 && userMove <= moves.Length)
            {
                byte[] message = Encoding.UTF8.GetBytes(moves[userMove - 1]);
                byte[] hmac = CalculateHmac(message);

                Console.WriteLine($"HMAC: {BitConverter.ToString(hmac).Replace("-", "").ToLower()}");

                int computerMove = RandomNumberGenerator.GetInt32(moves.Length) + 1;
                Console.WriteLine($"Computer move: {moves[computerMove - 1]}");

                int half = moves.Length / 2;
                int winningMove = (userMove + half) % moves.Length;
                int losingMove = (userMove + moves.Length - half) % moves.Length;

                if (computerMove == userMove)
                {
                    Console.WriteLine("It's a draw!");
                }
                else if (computerMove == winningMove)
                {
                    Console.WriteLine("You win!");
                }
                else if (computerMove == losingMove)
                {
                    Console.WriteLine("You lose!");
                }

                Console.WriteLine($"HMAC key: {BitConverter.ToString(key!).Replace("-", "").ToLower()}");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number from the menu or '0' to exit.");
            }

        } while (true);
    }
}

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Incorrect arguments. Please provide an odd number of non-repeating strings.");
            Console.WriteLine("Example: Rock Paper Scissors");
            return;
        }

        var game = new RockPaperScissors(args);
        game.Start();
    }
}
