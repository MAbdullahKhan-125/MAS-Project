static class ModelDetector
{
    public static QueueModel DetectSingleServerModel(
        Distribution arrival,
        Distribution service)
    {
        if (arrival == Distribution.Exponential &&
            service == Distribution.Exponential)
        {
            return QueueModel.MM1;
        }

        if (arrival == Distribution.Exponential &&
            service == Distribution.General)
        {
            return QueueModel.MG1;
        }

        return QueueModel.GG1;
    }
}