
using System;

enum Distribution
{
    Exponential = 1,
    General = 2
}

class Program
{
    static void Main(string[] args)
    {
        // Run automated tests when requested.
        if (args.Length > 0 &&
            args[0] == "--self-test")
        {
            QueueingSelfTests.Run();
            return;
        }

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n==============================");
            Console.WriteLine(" Queueing Systems Analyzer");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Single-server queue");
            Console.WriteLine("2. Multi-server M/M/C");
            Console.WriteLine("3. Multi-server M/G/C");
            Console.WriteLine("4. Multi-server G/G/C");
            Console.WriteLine("0. Exit");

            int modelChoice =
                ReadMenuChoice("Choose a model (0-4): ", 0, 4);

            switch (modelChoice)
            {
                case 0:
                    exit = true;
                    Console.WriteLine(
                        "Exiting Queueing Systems Analyzer.");
                    break;

                case 1:
                    RunSingleServer();
                    break;

                case 2:
                    RunMMc();
                    break;

                case 3:
                    RunMGc();
                    break;

                case 4:
                    RunGGc();
                    break;
            }

            if (!exit)
            {
                Console.WriteLine(
                    "\nReturning to the main menu...");
            }
        }
    }

    private static void RunSingleServer()
    {
        Console.WriteLine(
            "\n--- Single-Server Queue Analysis ---");

        Distribution arrival =
            InputParser.ReadDistribution(
                "Select the arrival-time distribution:");

        Distribution service =
            InputParser.ReadDistribution(
                "Select the service-time distribution:");

        double meanArrival =
            InputParser.ReadPositiveNumber(
                "Enter mean interarrival time: ");

        double arrivalVariance =
            arrival == Distribution.Exponential
                ? meanArrival * meanArrival
                : InputParser.ReadNonNegativeNumber(
                    "Enter arrival-time variance: ");

        double meanService =
            InputParser.ReadPositiveNumber(
                "Enter mean service time: ");

        double serviceVariance =
            service == Distribution.Exponential
                ? meanService * meanService
                : InputParser.ReadNonNegativeNumber(
                    "Enter service-time variance: ");

        double lambda = 1.0 / meanArrival;
        double mu = 1.0 / meanService;

        try
        {
            QueueModel model =
                ModelDetector.DetectSingleServerModel(
                    arrival, service);

            string modelName = model switch
            {
                QueueModel.MM1 => "M/M/1",
                QueueModel.MG1 => "M/G/1",
                QueueModel.GG1 => "G/G/1",
                _ => throw new InvalidOperationException(
                    "Unsupported single-server model.")
            };

            QueueMetrics result =
                CalculationEngine.CalculateSingleServer(
                    model,
                    lambda,
                    mu,
                    meanArrival,
                    arrivalVariance,
                    meanService,
                    serviceVariance);

            if (!ResultsValidator.Validate(
                result, out string error))
            {
                Console.WriteLine(
                    $"Validation failed: {error}");
                return;
            }

            PresentationEngine.DisplaySingleServer(
                modelName, result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(
                $"Calculation failed: {ex.Message}");
        }
    }

    private static void RunMMc()
    {
        Console.WriteLine("\n--- M/M/C Queue Analysis ---");

        double meanArrival =
            InputParser.ReadPositiveNumber(
                "Enter mean interarrival time: ");

        double meanService =
            InputParser.ReadPositiveNumber(
                "Enter mean service time: ");

        int servers =
            InputParser.ReadPositiveInt(
                "Enter number of servers: ");

        double lambda = 1.0 / meanArrival;
        double mu = 1.0 / meanService;

        try
        {
            QueueMetrics result =
                CalculationEngine.CalculateMMc(
                    lambda, mu, servers);

            if (!ResultsValidator.Validate(
                result, out string error))
            {
                Console.WriteLine(
                    $"Validation failed: {error}");
                return;
            }

            PresentationEngine.DisplayMMc(result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(
                $"Calculation failed: {ex.Message}");
        }
    }

    private static void RunMGc()
    {
        Console.WriteLine("\n--- M/G/C Queue Analysis ---");

        double meanArrival =
            InputParser.ReadPositiveNumber(
                "Enter mean interarrival time: ");

        double meanService =
            InputParser.ReadPositiveNumber(
                "Enter mean service time: ");

        double serviceVariance =
            InputParser.ReadNonNegativeNumber(
                "Enter service-time variance: ");

        int servers =
            InputParser.ReadPositiveInt(
                "Enter number of servers: ");

        double lambda = 1.0 / meanArrival;
        double mu = 1.0 / meanService;

        try
        {
            QueueMetrics result =
                CalculationEngine.CalculateMGc(
                    lambda,
                    mu,
                    servers,
                    meanService,
                    serviceVariance);

            if (!ResultsValidator.Validate(
                result, out string error))
            {
                Console.WriteLine(
                    $"Validation failed: {error}");
                return;
            }

            PresentationEngine.DisplayMGc(result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(
                $"Calculation failed: {ex.Message}");
        }
    }

    private static void RunGGc()
    {
        Console.WriteLine("\n--- G/G/C Queue Analysis ---");

        double meanArrival =
            InputParser.ReadPositiveNumber(
                "Enter mean interarrival time: ");

        double arrivalVariance =
            InputParser.ReadNonNegativeNumber(
                "Enter arrival-time variance: ");

        double meanService =
            InputParser.ReadPositiveNumber(
                "Enter mean service time: ");

        double serviceVariance =
            InputParser.ReadNonNegativeNumber(
                "Enter service-time variance: ");

        int servers =
            InputParser.ReadPositiveInt(
                "Enter number of servers: ");

        double lambda = 1.0 / meanArrival;
        double mu = 1.0 / meanService;

        try
        {
            QueueMetrics result =
                CalculationEngine.CalculateGGc(
                    lambda,
                    mu,
                    servers,
                    meanArrival,
                    arrivalVariance,
                    meanService,
                    serviceVariance);

            if (!ResultsValidator.Validate(
                result, out string error))
            {
                Console.WriteLine(
                    $"Validation failed: {error}");
                return;
            }

            PresentationEngine.DisplayGGc(result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(
                $"Calculation failed: {ex.Message}");
        }
    }

    private static int ReadMenuChoice(
        string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) &&
                choice >= minimum &&
                choice <= maximum)
            {
                return choice;
            }

            Console.WriteLine(
                $"Invalid option. Please enter a number from {minimum} to {maximum}.\n");
        }
    }
}
