using System.Collections;

namespace MathSharp.FiniteField
{
    public class GaloisFieldWithTable: IEquatable<GaloisFieldWithTable>, IRing<GaloisFieldElement>
    {
        private readonly IDictionary<GaloisFieldElement, int> mLogarithmicTable = 
            new Dictionary<GaloisFieldElement, int>();

        private readonly GaloisFieldElement[] mExponentialTable;

        public int NumberOfElements { get; }

        public int[] IrreduciblePolynomial { get; }

        public int Characteristic { get; }

        public int Degree { get; }

        public GaloisFieldElement Zero { get; }

        public GaloisFieldElement One { get; }

        public GaloisFieldWithTable(int characteristic, int degree)
        {
            this.Degree = degree;
            this.Characteristic = characteristic;
            this.NumberOfElements = (int) Math.Pow(Characteristic, Degree);
            this.Zero = new GaloisFieldElement(new int[Degree], this);
            this.One = new GaloisFieldElement(new int[Degree-1].Prepend(1).ToArray(), this);
            mExponentialTable = new GaloisFieldElement[this.NumberOfElements - 1];

            if (!(NumberOfElements < Math.Pow(2, 16)))
            {
                throw new ArgumentException("Too many elements for a table based finite field.");
            }

            IrreduciblePolynomial = ConwayPolynomials.Get(characteristic, degree);

            GaloisFieldElement current;

            if (degree > 1)
            {
                current = new GaloisFieldElement(new[] { 0, 1 }.Concat(new int[degree - 2]).ToArray(), this);
            }
            else
            {
                current = new GaloisFieldElement(new[] { Modulo(-IrreduciblePolynomial[0]) }, this);
            }

            mLogarithmicTable[One] = 0;
            mExponentialTable[0] = One;

            for (int i = 1; i < Math.Pow(characteristic, degree) - 1; i++)
            {
                mLogarithmicTable[current] = i;
                mExponentialTable[i] = current;
                current = current.CyclicShift(IrreduciblePolynomial);
            }
        }

        public IEnumerable<GaloisFieldElement> Elements => mLogarithmicTable.Keys;

        public GaloisFieldElement Generator => mExponentialTable[1];

        public GaloisFieldElement Add(GaloisFieldElement x, GaloisFieldElement y)
        {
            int[] result = new int[this.Degree];

            for (int i = 0; i < this.Degree; i++)
            {
                result[i] = this.Modulo(x[i] + y[i]);
            }

            return new GaloisFieldElement(result, this);
        }

        public GaloisFieldElement Subtract(GaloisFieldElement x, GaloisFieldElement y)
        {
            return Add(x, Negative(y));
        }

        public GaloisFieldElement Negative(GaloisFieldElement x)
        {
            int[] result = new int[this.Degree];

            for (int i = 0; i < this.Degree; i++)
            {
                result[i] = this.Modulo(-x[i]);
            }

            return new GaloisFieldElement(result, this);
        }

        public GaloisFieldElement Multiply(GaloisFieldElement x, GaloisFieldElement y)
        {
            int exponent = mLogarithmicTable[x] + mLogarithmicTable[y];
            exponent = Modulo(exponent, NumberOfElements - 1);
            return mExponentialTable[exponent];
        }

        public IEqualityComparer<GaloisFieldElement> Comparer => EqualityComparer<GaloisFieldElement>.Default;

        public GaloisFieldElement Inverse(GaloisFieldElement x)
        {
            if (Zero.Equals(x))
            {
                throw new DivideByZeroException();
            }

            int exponent = -mLogarithmicTable[x];
            exponent = Modulo(exponent, NumberOfElements - 1);
            return mExponentialTable[exponent];
        }

        public int Modulo(int value)
        {
            return Modulo(value, Characteristic);
        }

        private int Modulo(int value, int prime)
        {
            int result = value % prime;

            if (value >= 0)
            {
                return result;
            }

            return result + prime;
        }

        public bool Equals(GaloisFieldWithTable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Characteristic == other.Characteristic && Degree == other.Degree;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GaloisFieldWithTable)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Characteristic * 397) ^ Degree;
            }
        }
    }

    public class GaloisFieldElement : IEquatable<GaloisFieldElement>
    {
        private readonly int[] mCoefficients;

        public GaloisFieldElement(int[] coefficients, GaloisFieldWithTable field)
        {
            Field = field;
            mCoefficients = coefficients;
        }

        public GaloisFieldWithTable Field { get; }

        public int this[int index]
        {
            get
            {
                if (index < mCoefficients.Length)
                {
                    return mCoefficients[index];
                }

                return 0;
            }
        }

        internal GaloisFieldElement CyclicShift(int[] fieldPolynomial)
        {
            var shiftedCoefficients = mCoefficients.Take(mCoefficients.Length - 1).Prepend(0);

            var lastCoefficient = mCoefficients.Last();

            var currentPolynomial = fieldPolynomial.Take(mCoefficients.Length)
                                                   .Select(x => Field.Modulo(-x * lastCoefficient));

            var result =
                currentPolynomial.Zip(shiftedCoefficients, (x, y) => (x, y))
                                 .Select(tuple => Field.Modulo(tuple.x + tuple.y))
                                 .ToArray();

            return new GaloisFieldElement(result, Field);
        }

        public GaloisFieldElement Inverse()
        {
            return this.Field.Inverse(this);
        }

        public static GaloisFieldElement operator +(GaloisFieldElement first, GaloisFieldElement second)
        {
            if (!Equals(first.Field, second.Field))
            {
                throw new ArgumentException("The arguments must have the same field");
            }

            return first.Field.Add(first, second);
        }

        public static GaloisFieldElement operator -(GaloisFieldElement first, GaloisFieldElement second)
        {
            if (!Equals(first.Field, second.Field))
            {
                throw new ArgumentException("The arguments must have the same field");
            }

            return first.Field.Add(first, first.Field.Negative(second));
        }

        public static GaloisFieldElement operator -(GaloisFieldElement value)
        {
            return value.Field.Negative(value);
        }

        public static GaloisFieldElement operator *(GaloisFieldElement first, GaloisFieldElement second)
        {
            if (!Equals(first.Field, second.Field))
            {
                throw new ArgumentException("The arguments must have the same field");
            }

            return first.Field.Multiply(first, second);
        }

        public static GaloisFieldElement operator /(GaloisFieldElement first, GaloisFieldElement second)
        {
            if (!Equals(first.Field, second.Field))
            {
                throw new ArgumentException("The arguments must have the same field");
            }

            return first.Field.Multiply(first, first.Field.Inverse(second));
        }

        public bool Equals(GaloisFieldElement other)
        {
            return other != null &&
                   (mCoefficients.Length == other.mCoefficients.Length &&
                    Equals(this.Field, other.Field) &&
                    StructuralComparisons.StructuralEqualityComparer.Equals(mCoefficients, other.mCoefficients));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GaloisFieldElement)obj);
        }

        public override int GetHashCode()
        {
            return (mCoefficients != null ? mCoefficients.Sum().GetHashCode() : 0);
        }

        private string mToString;

        public override string ToString()
        {
            if (mToString == null)
            {
                if (this.Equals(this.Field.Zero))
                {
                    mToString = "0";
                }
                else
                {
                    mToString = string.Join(" + ", mCoefficients.Select((x, i) => (x, i))
                                                                .Where(x => x.x != 0)
                                                                .Select(x => Format(x)));
                }
            }

            return mToString;

            string Format((int x, int i) x)
            {
                string formatedCoefficient;

                if (x.x == 1)
                {
                    formatedCoefficient = string.Empty;
                }
                else
                {
                    formatedCoefficient = x.x.ToString();
                }

                if (x.i == 0)
                {
                    return x.x.ToString();
                }
                else if (x.i == 1)
                {
                    return $"{formatedCoefficient}X";
                }
                else if (x.i < 10)
                {
                    return $"{formatedCoefficient}X^{x.i}";

                }

                return $"{formatedCoefficient}X^{{{x.i}}}";
            }
        }
    }
}