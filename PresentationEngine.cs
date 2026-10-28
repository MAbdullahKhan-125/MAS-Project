
using System;

static class PresentationEngine
{
    public static void DisplaySingleServer(
        string model,
        QueueMetrics result)
    {
        Console.WriteLine($"\n--- {model} Queue Results ---");

        if (model == "G/G/1")
            Console.WriteLine("Method: Kingman's approximation");
        else if (model == "M/G/1")
            Console.WriteLine("Method: Pollaczek-Khinchine formula");
        else
            Console.WriteLine("Method: Exact M/M/1 formulas");

        DisplayCommonMetrics(result);

        Console.WriteLine(
            $"P0 (empty-system probability): {result.P0:F4}");

        Console.WriteLine(
            "Probability of waiting: Not calculated");

        DisplayConsistencyChecks(result);

        Console.WriteLine("\nAnalysis completed.");
    }

    public static void DisplayMMc(QueueMetrics result)
    {
        Console.WriteLine("\n--- M/M/C Queue Results ---");
        Console.WriteLine("Method: Exact M/M/C formulas");

        DisplayCommonMetrics(result);

        Console.WriteLine(
            $"P0 (empty-system probability): {result.P0:F4}");

        Console.WriteLine(
            $"Probability of waiting: {result.ProbabilityWait:F4}");

        DisplayConsistencyChecks(result);

        Console.WriteLine("\nAnalysis completed.");
    }

    public static void DisplayMGc(QueueMetrics result)
    {
        Console.WriteLine("\n--- M/G/C Queue Results ---");
        Console.WriteLine("Method: Allen-Cunneen approximation");

        DisplayCommonMetrics(result);

        Console.WriteLine("P0: Not calculated exactly");
        Console.WriteLine("Probability of waiting: Not calculated exactly");
        Console.WriteLine(
            $"Idle proportion (1 - rho): {1.0 - result.Rho:F4}");

        DisplayConsistencyChecks(result);

        Console.WriteLine(
            "\nNote: M/G/C results are approximate, not exact.");

        Console.WriteLine("\nAnalysis completed.");
    }

    public static void DisplayGGc(QueueMetrics result)
    {
        Console.WriteLine("\n--- G/G/C Queue Results ---");
        Console.WriteLine("Method: Allen-Cunneen approximation");

        DisplayCommonMetrics(result);

        Console.WriteLine("P0: Not calculated exactly");
        Console.WriteLine("Probability of waiting: Not calculated exactly");

        DisplayConsistencyChecks(result);

        Console.WriteLine(
            "\nNote: G/G/C results are approximate, not exact.");

        Console.WriteLine("\nAnalysis completed.");
    }

    private static void DisplayCommonMetrics(QueueMetrics result)
    {
        Console.WriteLine(
            $"Arrival rate (lambda): {result.Lambda:F4}");

        Console.WriteLine(
            $"Service rate per server (mu): {result.Mu:F4}");

        Console.WriteLine(
            $"Number of servers (C): {result.Servers}");

        Console.WriteLine(
            $"Utilization (rho): {result.Rho:F4}");

        Console.WriteLine(
            $"Lq (average queue length): {result.Lq:F4}");

        Console.WriteLine(
            $"Wq (average queue time): {result.Wq:F4}");

        Console.WriteLine(
            $"L (average system size): {result.L:F4}");

        Console.WriteLine(
            $"W (average system time): {result.W:F4}");
    }

    private static void DisplayConsistencyChecks(QueueMetrics result)
    {
        double calculatedL = result.Lambda * result.W;
        double calculatedLq = result.Lambda * result.Wq;

        Console.WriteLine("\n--- Consistency Checks ---");

        Console.WriteLine(
            $"L = lambda * W: {result.L:F4} = {calculatedL:F4}");

        Console.WriteLine(
            $"Lq = lambda * Wq: {result.Lq:F4} = {calculatedLq:F4}");
    }
}
