using MathSharp.QuadraticRings;

internal class FactorizationHelperFunctions
{
    public static QuadraticRingElement[] FindIrreducibleElements(int n, QuadraticRing ring)
    {
        List<QuadraticRingElement> elements = new List<QuadraticRingElement>();

        for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= n; j++)
            {
                elements.Add(ring.Element(i, j));
            }
        }

        IEnumerable<QuadraticRingElement> relevantElements =
            elements.Where(x => Math.Abs(ring.Norm(x)) > 1 &&
                                (x.a == 0 || x.b == 0 || GCD((int)x.a, (int)x.b) == 1))
                    .OrderBy(x => Math.Abs(ring.Norm(x)))
                    .ThenBy(x => Math.Max(x.a, x.b));

        if (ring.Element(0, 1).Norm == 1)
        {
            relevantElements = relevantElements.Where(x => x.b != 0);
        }

        var lastIndex = 0;
        var irreducibleElements = relevantElements.ToArray();

        while (lastIndex < irreducibleElements.Length)
        {
            var lastElement = irreducibleElements[lastIndex];
            var lastElementConjugate = ring.Conjugate(lastElement);

            irreducibleElements =
                irreducibleElements.Where((x, i) => !(i > lastIndex &&
                                                      (IsDivisible(ring, x, lastElement) ||
                                                       IsDivisible(ring, x, lastElementConjugate))))
                                   .ToArray();

            lastIndex += 1;
        }

        return irreducibleElements;
    }

    private static List<QuadraticRingElement> FindFactorsInRing
    (QuadraticRing ring, int number,
     QuadraticRingElement[] irreducibleElements)
    {
        var numberSquared = number * number;
        var numberAsElement = ring.Element(number, 0);

        List<QuadraticRingElement> result = new List<QuadraticRingElement>();

        foreach (QuadraticRingElement element in irreducibleElements)
        {
            if (element.Norm > numberSquared)
            {
                break;
            }

            if (IsDivisible(ring, numberAsElement, element))
            {
                result.Add(element);
            }
        }

        return result;
    }

    private static List<QuadraticRingElement[]> FindFactorizations
        (QuadraticRing ring, int number, QuadraticRingElement[] factors)
    {
        List<QuadraticRingElement[]> result = new List<QuadraticRingElement[]>();
        List<QuadraticRingElement> factorsLeft = factors.ToList();

        while (factorsLeft.Count != 0)
        {
            List<QuadraticRingElement> initialFactors = new List<QuadraticRingElement>(2);
            QuadraticRingElement current = ring.Element(number, 0);
            QuadraticRingElement currentFactor = factorsLeft.First();
            current = ring.Divide(current, currentFactor);
            factorsLeft.RemoveAt(0);
            initialFactors.Add(currentFactor);

            QuadraticRingElement currentFactorConjugate = ring.Conjugate(currentFactor);

            if (IsDivisible(ring, current, currentFactorConjugate))
            {
                current = ring.Divide(current, currentFactorConjugate);
                factorsLeft.Remove(currentFactorConjugate);
                initialFactors.Add(currentFactorConjugate);
            }

            List<QuadraticRingElement> currentDecomposition = FindDecomposition(ring, factors, current, factorsLeft);
            result.Add(initialFactors.Concat(currentDecomposition).ToArray());
        }

        return result;
    }

    private static List<QuadraticRingElement> FindDecomposition(QuadraticRing ring, QuadraticRingElement[] factors,
                                                                QuadraticRingElement current,
                                                                List<QuadraticRingElement> factorsLeft)
    {
        List<QuadraticRingElement> currentDecomposition = new List<QuadraticRingElement>();

        while (Math.Abs(current.Norm) != 1)
        {
            foreach (QuadraticRingElement factor in factors.Concat(factors.Select(x => ring.Conjugate(x))))
            {
                if (IsDivisible(ring, current, factor))
                {
                    currentDecomposition.Add(factor);
                    factorsLeft.Remove(factor);
                    current = ring.Divide(current, factor);
                    break;
                }
            }
        }

        currentDecomposition.Add(current);

        return currentDecomposition;
    }


    private static int GCD(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return Math.Max(a, b);
    }

    private static bool IsDivisible(QuadraticRing ring, QuadraticRingElement first,
                                    QuadraticRingElement second)
    {
        return first.Norm % second.Norm == 0 && ring.IsIntegral(ring.Divide(first, second));
    }

    public static bool IsPrime(int n)
    {
        for (int i = 2; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static QuadraticRingElement[] FindFactors(QuadraticRing ring, int p)
    {
        QuadraticRingElement pElement = ring.Element(p, 0);

        for (int a = 1; a <= Math.Sqrt(p); a++)
        {
            QuadraticRingElement loopElement =
                ring.Element(a, 1);

            for (int b = 1; ring.Norm(loopElement) <= p; b++)
            {
                QuadraticRingElement quotient = ring.Divide(pElement, loopElement);

                if (ring.IsIntegral(quotient))
                {
                    return new QuadraticRingElement[] { loopElement, quotient };
                }

                loopElement = ring.Element(a, b);
            }
        }

        return new QuadraticRingElement[] { pElement };
    }
}