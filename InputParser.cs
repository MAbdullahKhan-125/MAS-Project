
using System;
using System.Globalization;

static class InputParser
{
    public static Distribution ReadDistribution(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            Console.WriteLine("1. Exponential");
            Console.WriteLine("2. General");
            Console.Write("Enter choice (1 or 2): ");

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) &&
                (choice == 1 || choice == 2))
            {
                return (Distribution)choice;
            }

            Console.WriteLine(
                "Invalid choice. Please enter 1 or 2.\n");
        }
    }

    public static double ReadPositiveNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double value) &&
                double.IsFinite(value) &&
                value > 0)
            {
                return value;
            }

            Console.WriteLine(
                "Enter a positive, finite number. Use a decimal point if needed.\n");
        }
    }

    public static double ReadNonNegativeNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double value) &&
                double.IsFinite(value) &&
                value >= 0)
            {
                return value;
            }

            Console.WriteLine(
                "Enter a non-negative, finite number. Use a decimal point if needed.\n");
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value > 0)
            {
                return value;
            }

            Console.WriteLine(
                "Enter a positive whole number.\n");
        }
    }

    public static int ReadPositiveIntOrZero(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= 0)
            {
                return value;
            }

            Console.WriteLine(
                "Enter a whole number greater than or equal to zero.\n");
        }
    }
}
