namespace MathSharp.QuadraticRings;

public class QuadraticRingElement
{
    public QuadraticRingElement(QuadraticRing quadraticRing, double a, double b)
    {
        this.a = a;
        this.b = b;
        this.Norm = quadraticRing.Norm(this);
    }

    public double b { get; set; }

    public double a { get; set; }

    public double Norm { get; }
}