using System;

namespace MathSharp.Tests;

public class IntegersRing : IRing<int>
{
    public int Add(int x, int y) => x + y;

    public int Subtract(int x, int y) => x - y;

    public int Negative(int x) => -x;

    public int Multiply(int x, int y) => x * y;

    public int One => 1;

    public int Zero => 0;

    public int Inverse(int x)
    {
        if (x == 1 || x == -1)
        {
            return x;
        }

        throw new ArgumentException("Element is not invertible");
    }
}