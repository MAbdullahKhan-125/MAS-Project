
using System;

static class ResultsValidator
{
    public static bool Validate(QueueMetrics? result, out string error)
    {
        error = "";

        if (result == null)
        {
            error = "The calculation result is missing.";
            return false;
        }

        if (result.Servers < 1)
        {
            error = "The number of servers must be at least 1.";
            return false;
        }

        if (!double.IsFinite(result.Lambda) || result.Lambda <= 0 ||
            !double.IsFinite(result.Mu) || result.Mu <= 0)
        {
            error = "Arrival and service rates must be positive and finite.";
            return false;
        }

        if (!double.IsFinite(result.Rho) ||
            !double.IsFinite(result.Lq) ||
            !double.IsFinite(result.Wq) ||
            !double.IsFinite(result.L) ||
            !double.IsFinite(result.W))
        {
            error = "One or more calculated metrics are not finite.";
            return false;
        }

        if (result.Rho < 0 || result.Rho >= 1 ||
            result.Lq < 0 || result.Wq < 0 ||
            result.L < 0 || result.W < 0)
        {
            error = "One or more calculated metrics are outside valid ranges.";
            return false;
        }

        if (result.Wq > result.W + 1e-8 ||
            result.Lq > result.L + 1e-8)
        {
            error = "Queue metrics cannot exceed their corresponding system metrics.";
            return false;
        }

        if (!double.IsNaN(result.P0) &&
            (!double.IsFinite(result.P0) ||
             result.P0 < 0 || result.P0 > 1))
        {
            error = "The empty-system probability must be between 0 and 1.";
            return false;
        }

        if (!double.IsNaN(result.ProbabilityWait) &&
            (!double.IsFinite(result.ProbabilityWait) ||
             result.ProbabilityWait < 0 || result.ProbabilityWait > 1))
        {
            error = "The waiting probability must be between 0 and 1.";
            return false;
        }

        double expectedL = result.Lambda * result.W;
        double expectedLq = result.Lambda * result.Wq;

        double toleranceL =
            1e-8 * Math.Max(1.0, Math.Abs(result.L));

        double toleranceLq =
            1e-8 * Math.Max(1.0, Math.Abs(result.Lq));

        if (!double.IsFinite(expectedL) ||
            Math.Abs(result.L - expectedL) > toleranceL)
        {
            error = "Little's Law failed for the total system metrics.";
            return false;
        }

        if (!double.IsFinite(expectedLq) ||
            Math.Abs(result.Lq - expectedLq) > toleranceLq)
        {
            error = "Little's Law failed for the queue metrics.";
            return false;
        }

        return true;
    }
}
