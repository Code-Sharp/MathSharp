namespace MathSharp.QuadraticRings;

public class QuadraticRing : IRing<QuadraticRingElement>
{
    private readonly int _alpha;

    public QuadraticRing(int alpha)
    {
        _alpha = alpha;
        this.One = this.Element(1, 0);
        this.Zero = this.Element(0, 0);
    }

    public QuadraticRingElement Element(double a, double b)
    {
        return new QuadraticRingElement(this, a, b);
    }

    public QuadraticRingElement Add(QuadraticRingElement z1, QuadraticRingElement z2)
    {
        return new QuadraticRingElement(this, z1.a + z2.a, z1.b + z2.b);
    }

    public QuadraticRingElement Subtract(QuadraticRingElement z1, QuadraticRingElement z2)
    {
        return new QuadraticRingElement(this, z1.a - z2.a, z1.b - z2.b);
    }

    public QuadraticRingElement Negative(QuadraticRingElement x)
    {
        return this.Element(-x.a, -x.b);
    }

    public QuadraticRingElement Multiply(QuadraticRingElement z1, QuadraticRingElement z2)
    {
        return new QuadraticRingElement(this, z1.a * z2.a + _alpha * z1.b * z2.b,
                                        z1.a * z2.b + z1.b * z2.a);
    }

    public QuadraticRingElement One { get; }
    public QuadraticRingElement Zero { get; }

    public QuadraticRingElement Inverse(QuadraticRingElement z)
    {
        double norm = Norm(z);
        return new QuadraticRingElement(this, z.a / norm, -z.b / norm);
    }

    public QuadraticRingElement Divide(QuadraticRingElement z1, QuadraticRingElement z2)
    {
        QuadraticRingElement result = Multiply(z1, Conjugate(z2));
        return Element(result.a / z2.Norm, result.b / z2.Norm);
    }

    public bool IsIntegral(QuadraticRingElement z)
    {
        return (z.a == (int)z.a) && (z.b == (int)z.b);
    }

    public QuadraticRingElement Conjugate(QuadraticRingElement element)
    {
        return this.Element(element.a, -element.b);
    }

    public double Norm(QuadraticRingElement z)
    {
        return z.a * z.a - _alpha * z.b * z.b;
    }

    public string ToString(QuadraticRingElement z)
    {
        var realPart = string.Empty;
        var imaginaryPart = string.Empty;

        if (z.a == 0 && z.b == 0)
        {
            return "0";
        }

        if (z.a != 0)
        {
            realPart = NiceString(z.a);
            if (z.b > 0)
            {
                imaginaryPart = "+";
            }
        }

        if (z.b != 0)
        {
            imaginaryPart = imaginaryPart + NiceString(z.b) + $"\\sqrt{_alpha}";
        }

        return realPart + imaginaryPart;
    }

    private string NiceString(double value)
    {
        if (value == (int)value)
        {
            return (((int)value).ToString());
        }
        else
        {
            return value.ToString();
        }
    }
}