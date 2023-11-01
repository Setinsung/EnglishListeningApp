namespace Commons.ASPNETCore;

public static class RandomExtensions
{
    public static double NextDouble(this Random random, double minValue, double maxValue)
    {
        if (minValue >= maxValue) (minValue, maxValue) = (maxValue, minValue);
        double x = random.NextDouble();
        return x * maxValue + (1 - x) * minValue;
    }
}