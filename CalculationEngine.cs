
using System;

public sealed class QueueMetrics
{
    public double Lambda { get; init; }
    public double Mu { get; init; }
    public int Servers { get; init; }
    public double Rho { get; init; }
    public double P0 { get; init; }
    public double ProbabilityWait { get; init; }
    public double Lq { get; init; }
    public double Wq { get; init; }
    public double L { get; init; }
    public double W { get; init; }
}

static class CalculationEngine
{
    public static QueueMetrics CalculateMMc(
        double lambda,
        double mu,
        int servers)
    {
        ValidateRates(lambda, mu);

        if (servers < 1)
            throw new ArgumentOutOfRangeException(
                nameof(servers), "Server count must be at least 1.");

        double rho =
            StabilityValidator.CalculateUtilization(
                lambda, mu, servers);

        if (!StabilityValidator.IsStable(rho))
            throw new InvalidOperationException(
                "The queue is unstable. Utilization must be less than 1.");

        double a = lambda / mu;

        if (!double.IsFinite(a) || a <= 0)
            throw new InvalidOperationException(
                "The offered load is outside the supported numeric range.");

        // Sum a^n / n! from n = 0 to C - 1.
        double term = 1.0;
        double sum = 1.0;

        for (int n = 1; n < servers; n++)
        {
            term *= a / n;

            if (!double.IsFinite(term))
                throw new InvalidOperationException(
                    "Numerical overflow while calculating M/M/C.");

            sum += term;

            if (!double.IsFinite(sum))
                throw new InvalidOperationException(
                    "Numerical overflow while calculating M/M/C.");
        }

        // a^C / C!
        double lastTerm = term * a / servers;

        if (!double.IsFinite(lastTerm))
            throw new InvalidOperationException(
                "Numerical overflow while calculating M/M/C.");

        double tail = lastTerm / (1.0 - rho);

        if (!double.IsFinite(tail))
            throw new InvalidOperationException(
                "Numerical overflow while calculating M/M/C.");

        double denominator = sum + tail;

        if (!double.IsFinite(denominator) || denominator <= 0)
            throw new InvalidOperationException(
                "Unable to calculate the M/M/C empty-system probability.");

        double p0 = 1.0 / denominator;
        double probabilityWait = tail * p0;
        double lq = probabilityWait * rho / (1.0 - rho);
        double wq = lq / lambda;
        double w = wq + 1.0 / mu;
        double l = lambda * w;

        EnsureFinite(p0, probabilityWait, lq, wq, w, l);

        return new QueueMetrics
        {
            Lambda = lambda,
            Mu = mu,
            Servers = servers,
            Rho = rho,
            P0 = p0,
            ProbabilityWait = probabilityWait,
            Lq = lq,
            Wq = wq,
            L = l,
            W = w
        };
    }

    public static QueueMetrics CalculateMGc(
        double lambda,
        double mu,
        int servers,
        double meanService,
        double serviceVariance)
    {
        ValidateMeanAndVariance(
            meanService, serviceVariance,
            nameof(meanService), nameof(serviceVariance));

        ValidateRates(lambda, mu);

        if (!RatesMatch(mu, 1.0 / meanService))
            throw new ArgumentException(
                "Service rate must equal the reciprocal of mean service time.");

        QueueMetrics mmc = CalculateMMc(lambda, mu, servers);

        double cs2 =
            serviceVariance / meanService / meanService;

        double adjustment = (1.0 + cs2) / 2.0;
        double lq = mmc.Lq * adjustment;
        double wq = lq / lambda;
        double w = wq + meanService;
        double l = lambda * w;

        EnsureFinite(cs2, adjustment, lq, wq, w, l);

        return new QueueMetrics
        {
            Lambda = lambda,
            Mu = mu,
            Servers = servers,
            Rho = mmc.Rho,
            P0 = double.NaN,
            ProbabilityWait = double.NaN,
            Lq = lq,
            Wq = wq,
            L = l,
            W = w
        };
    }

    public static QueueMetrics CalculateGGc(
        double lambda,
        double mu,
        int servers,
        double meanArrival,
        double arrivalVariance,
        double meanService,
        double serviceVariance)
    {
        ValidateMeanAndVariance(
            meanArrival, arrivalVariance,
            nameof(meanArrival), nameof(arrivalVariance));

        ValidateMeanAndVariance(
            meanService, serviceVariance,
            nameof(meanService), nameof(serviceVariance));

        ValidateRates(lambda, mu);

        if (!RatesMatch(lambda, 1.0 / meanArrival) ||
            !RatesMatch(mu, 1.0 / meanService))
        {
            throw new ArgumentException(
                "Arrival and service rates must match the reciprocals " +
                "of their respective mean times.");
        }

        // Exact M/M/C baseline, adjusted for variability.
        QueueMetrics mmc = CalculateMMc(lambda, mu, servers);

        double ca2 =
            arrivalVariance / meanArrival / meanArrival;

        double cs2 =
            serviceVariance / meanService / meanService;

        double adjustment = (ca2 + cs2) / 2.0;
        double lq = mmc.Lq * adjustment;
        double wq = lq / lambda;
        double w = wq + meanService;
        double l = lambda * w;

        EnsureFinite(ca2, cs2, adjustment, lq, wq, w, l);

        return new QueueMetrics
        {
            Lambda = lambda,
            Mu = mu,
            Servers = servers,
            Rho = mmc.Rho,
            P0 = double.NaN,
            ProbabilityWait = double.NaN,
            Lq = lq,
            Wq = wq,
            L = l,
            W = w
        };
    }

    public static QueueMetrics CalculateSingleServer(
        QueueModel model,
        double lambda,
        double mu,
        double meanArrival,
        double arrivalVariance,
        double meanService,
        double serviceVariance)
    {
        ValidateRates(lambda, mu);

        ValidateMeanAndVariance(
            meanArrival, arrivalVariance,
            nameof(meanArrival), nameof(arrivalVariance));

        ValidateMeanAndVariance(
            meanService, serviceVariance,
            nameof(meanService), nameof(serviceVariance));

        if (!RatesMatch(lambda, 1.0 / meanArrival) ||
            !RatesMatch(mu, 1.0 / meanService))
        {
            throw new ArgumentException(
                "Rates must match the reciprocals of the supplied means.");
        }

        double rho =
            StabilityValidator.CalculateUtilization(lambda, mu, 1);

        if (!StabilityValidator.IsStable(rho))
            throw new InvalidOperationException(
                "The queue is unstable. Utilization must be less than 1.");

        double lq;

        switch (model)
        {
            case QueueModel.MM1:
                lq = rho * rho / (1.0 - rho);
                break;

            case QueueModel.MG1:
                // Pollaczek-Khinchine formula.
                lq = lambda * lambda *
                    (serviceVariance + meanService * meanService) /
                    (2.0 * (1.0 - rho));
                break;

            case QueueModel.GG1:
                // Kingman's approximation.
                double ca2 =
                    arrivalVariance / meanArrival / meanArrival;

                double cs2 =
                    serviceVariance / meanService / meanService;

                EnsureFinite(ca2, cs2);

                lq = (rho * rho / (1.0 - rho)) *
                    ((ca2 + cs2) / 2.0);
                break;

            default:
                throw new ArgumentException(
                    "The selected model is not a single-server model.",
                    nameof(model));
        }

        double wq = lq / lambda;
        double w = wq + meanService;
        double l = lambda * w;

        EnsureFinite(lq, wq, w, l);

        return new QueueMetrics
        {
            Lambda = lambda,
            Mu = mu,
            Servers = 1,
            Rho = rho,
            P0 = 1.0 - rho,
            ProbabilityWait = double.NaN,
            Lq = lq,
            Wq = wq,
            L = l,
            W = w
        };
    }

    private static void ValidateRates(double lambda, double mu)
    {
        if (!double.IsFinite(lambda) || lambda <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lambda),
                "Arrival rate must be positive and finite.");

        if (!double.IsFinite(mu) || mu <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(mu),
                "Service rate must be positive and finite.");
    }

    private static void ValidateMeanAndVariance(
        double mean,
        double variance,
        string meanName,
        string varianceName)
    {
        if (!double.IsFinite(mean) || mean <= 0)
            throw new ArgumentOutOfRangeException(
                meanName, "Mean time must be positive and finite.");

        if (!double.IsFinite(variance) || variance < 0)
            throw new ArgumentOutOfRangeException(
                varianceName,
                "Variance must be non-negative and finite.");
    }

    private static bool RatesMatch(double actual, double expected)
    {
        return double.IsFinite(actual) &&
               double.IsFinite(expected) &&
               Math.Abs(actual - expected) <=
               1e-10 * Math.Max(1.0, Math.Abs(expected));
    }

    private static void EnsureFinite(params double[] values)
    {
        foreach (double value in values)
        {
            if (!double.IsFinite(value))
                throw new InvalidOperationException(
                    "The result is outside the supported numeric range.");
        }
    }
}
