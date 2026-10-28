using System;

static class StabilityValidator
{
    public static double CalculateUtilization(
        double lambda,
        double mu,
        int servers)
    {
        if (!double.IsFinite(lambda) || lambda <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lambda),
                "Arrival rate must be positive and finite.");
        }

        if (!double.IsFinite(mu) || mu <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mu),
                "Service rate must be positive and finite.");
        }

        if (servers < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(servers),
                "Server count must be at least 1.");
        }

        return lambda / (servers * mu);
    }

    public static bool IsStable(double rho)
    {
        return double.IsFinite(rho) && rho < 1.0;
    }
}