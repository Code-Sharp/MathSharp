namespace MathSharp;

public interface IRing<TElement>
{
    TElement Add(TElement x, TElement y);
    TElement Subtract(TElement x, TElement y);
    TElement Negative(TElement x);
    TElement Multiply(TElement x, TElement y);
    TElement One { get; }
    TElement Zero { get; }
    TElement Inverse(TElement x);
}