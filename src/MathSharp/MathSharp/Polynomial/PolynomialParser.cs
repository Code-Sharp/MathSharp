using System.Text.RegularExpressions;

namespace MathSharp.Polynomial;

public class PolynomialParser
{
    public static int PowerMod(int baseValue, int exponent, int modulo)
    {
        int result = 1;
        baseValue = baseValue % modulo;

        while (exponent > 0)
        {
            if (exponent % 2 == 1)
            {
                result = (result * baseValue) % modulo;
            }

            baseValue = (baseValue * baseValue) % modulo;
            exponent /= 2;
        }

        return result;
    }

    public static string PolynomialToString(IDictionary<int, int> polynomial)
    {
        List<string> toConcat = new List<string>();
        bool isFirst = true;

        foreach (int power in polynomial.Keys.OrderByDescending(x => x))
        {
            var coefficient = polynomial[power];
            if (coefficient != 0)
            {
                if (coefficient > 0 && !isFirst)
                {
                    toConcat.Add("+");
                }

                if (power == 0)
                {
                    toConcat.Add(coefficient.ToString());
                }
                else
                {
                    var monomial = $"x^{{{power}}}";

                    if (power == 1)
                    {
                        monomial = "x";
                    }

                    if (coefficient == 1)
                    {
                        toConcat.Add(monomial);
                    }
                    else if (coefficient == -1)
                    {
                        toConcat.Add($"-{monomial}");
                    }
                    else
                    {
                        toConcat.Add($"{coefficient} {monomial}");
                    }

                    isFirst = false;                    
                }
            }
        }

        var result = string.Join("", toConcat);
        return result;
    }

    public static int PolynomialEvaluation(IDictionary<int, int> polynomial, int x, int n)
    {
        int result = 0;
        int lastPower = polynomial.Keys.Max(x => x);

        foreach (int currentPower in polynomial.Keys.OrderByDescending(x => x))
        {
            var difference = lastPower - currentPower;
            var powerForDifference = PowerMod(x, difference, n);
            result = result * powerForDifference % n;

            int currentCoefficient = polynomial[currentPower];
            result = result + currentCoefficient % n;
            lastPower = currentPower;
        }

        if (lastPower != 0)
        {
            var powerForLast = PowerMod(x, lastPower, n);
            result = result * powerForLast % n;
        }

        return result;
    }

    public static Dictionary<int, int> Parse(string input)
    {
        // Parse a polynomial in the form of a string
        // Returns a dictionary with powers as keys and coefficients as values
        Dictionary<int, int> poly = new Dictionary<int, int>();
        Dictionary<string, int> signs = new Dictionary<string, int> { { "+", 1 }, { "-", -1 } };

        // Define a regular expression to match coefficients, x, and exponents
        Regex xRegex = new Regex(@"([-+]?\d*)(x?)\^?(\d*)");

        // Remove spaces and replace '**' with '^' in the input string
        input = input.Replace(" ", "").Replace("**", "^");

        // Iterate through matches found by the regular expression
        foreach (Match match in xRegex.Matches(input))
        {
            // Extract coefficient, x, and exponent from the match
            string c = match.Groups[1].Value;
            string x = match.Groups[2].Value;
            string p = match.Groups[3].Value;

            // Skip empty matches
            if (string.IsNullOrEmpty(c) && string.IsNullOrEmpty(x) && string.IsNullOrEmpty(p))
            {
                continue;
            }

            // Determine coefficient, power, and handle special cases
            int coefficient;
            if (signs.TryGetValue(c, out var sign))
            {
                coefficient = sign;
            }
            else
            {
                coefficient = int.Parse(string.IsNullOrEmpty(c) ? "1" : c);
            }

            int power;
            
            if (x.Length > 0)
            {
                power = string.IsNullOrEmpty(p) ? 1 : int.Parse(p);
            }
            else
            {
                power = 0;
            }

            // Handle multiple monomials with the same degree
            if (poly.ContainsKey(power))
            {
                poly[power] += coefficient;
            }
            else
            {
                poly.Add(power, coefficient);
            }
        }

        return poly;
    }
}