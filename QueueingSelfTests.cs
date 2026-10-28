
using System;

static class QueueingSelfTests
{
    private static int passed;
    private static int failed;

    public static void Run()
    {
        passed = 0;
        failed = 0;

        Console.WriteLine("\n===== QUEUEING SELF-TESTS =====");

        TestMMc();
        TestMGc();
        TestGGc();

        Console.WriteLine("\n===== TEST SUMMARY =====");
        Console.WriteLine($"Passed: {passed}");
        Console.WriteLine($"Failed: {failed}");
    }

    private static void TestMMc()
    {
        QueueMetrics result =
            CalculationEngine.CalculateMMc(0.5, 0.25, 3);

        CheckNear("M/M/C utilization", result.Rho, 2.0 / 3.0);
        CheckNear("M/M/C Lq", result.Lq, 8.0 / 9.0);
        CheckNear("M/M/C Wq", result.Wq, 16.0 / 9.0);
        CheckNear("M/M/C L", result.L, 26.0 / 9.0);
    }

    private static void TestMGc()
    {
        QueueMetrics result =
            CalculationEngine.CalculateMGc(
                0.5, 0.25, 3, 4.0, 8.0);

        CheckNear("M/G/C Lq", result.Lq, 2.0 / 3.0);
        CheckNear("M/G/C Wq", result.Wq, 4.0 / 3.0);
        CheckNear("M/G/C L", result.L, 8.0 / 3.0);
    }

    private static void TestGGc()
    {
        QueueMetrics result =
            CalculationEngine.CalculateGGc(
                0.5, 0.25, 3,
                2.0, 4.0,
                4.0, 8.0);

        CheckNear("G/G/C Lq", result.Lq, 2.0 / 3.0);
        CheckNear("G/G/C Wq", result.Wq, 4.0 / 3.0);
        CheckNear("G/G/C L", result.L, 8.0 / 3.0);
    }

    private static void CheckNear(
        string name,
        double actual,
        double expected)
    {
        if (double.IsFinite(actual) &&
            Math.Abs(actual - expected) < 0.0001)
        {
            Console.WriteLine($"PASS: {name}");
            passed++;
        }
        else
        {
            Console.WriteLine(
                $"FAIL: {name}. Expected {expected:F4}, got {actual:F4}");
            failed++;
        }
    }
}
