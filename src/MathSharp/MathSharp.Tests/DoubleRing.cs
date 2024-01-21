namespace MathSharp.Tests;

public class DoubleRing : IRing<double>
{
    public double Add(double x, double y)
    {
        return x + y;
    }

    public double Subtract(double x, double y)
    {
        return x - y;
    }

    public double Negative(double x)
    {
        return -x;
    }

    public double Multiply(double x, double y)
    {
        return x * y;
    }

    public double One => 1;
    public double Zero => 0;
    public double Inverse(double x) => 1 / x;
}