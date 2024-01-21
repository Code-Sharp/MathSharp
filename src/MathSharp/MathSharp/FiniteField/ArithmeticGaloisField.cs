namespace MathSharp.FiniteField
{
    public class ArithmeticGaloisFieldWithTable: IEquatable<ArithmeticGaloisFieldWithTable>, IRing<int>
    {
        private readonly int[] mLogarithmicTable;

        private readonly int[] mExponentialTable;

        public int NumberOfElements { get; }

        public int[] IrreduciblePolynomial { get; }

        private int[] Powers { get; }

        public int Characteristic { get; }

        public int Degree { get; }

        public int Zero => 0;

        public int One => 1;

        public ArithmeticGaloisFieldWithTable(int characteristic, int degree)
        {
            this.Degree = degree;
            this.Characteristic = characteristic;
            this.NumberOfElements = (int) Math.Pow(Characteristic, Degree);
            mExponentialTable = new int[this.NumberOfElements - 1];
            mLogarithmicTable = new int[this.NumberOfElements];
            
            this.Powers = Enumerable.Range(0, degree).Select(x => (int)Math.Pow(Characteristic, x))
                                    .ToArray();

            if (!(NumberOfElements < Math.Pow(2, 16)))
            {
                throw new ArgumentException("Too many elements for a table based finite field.");
            }

            IrreduciblePolynomial = ConwayPolynomials.Get(characteristic, degree);

            int current;

            if (degree > 1)
            {
                current = Characteristic;
            }
            else
            {
                current = Modulo(-IrreduciblePolynomial[0]);
            }

            mLogarithmicTable[One] = 0;
            mExponentialTable[0] = One;

            for (int i = 1; i < Math.Pow(characteristic, degree) - 1; i++)
            {
                mLogarithmicTable[current] = i;
                mExponentialTable[i] = current;
                int next = CyclicShift(current);
                current = next;
            }
        }

        private int CyclicShift(int current)
        {
            int result = 0;
            int lastCoefficient = (current / this.Powers[Degree - 1]) % Characteristic;
            int currentCoefficient = 0;

            for (int i = 0; i < this.Degree; i++)
            {
                int coefficientToAdd = Modulo(currentCoefficient - this.IrreduciblePolynomial[i] * lastCoefficient);
                result += coefficientToAdd * this.Powers[i];
                currentCoefficient = current % Characteristic;
                current /= Characteristic;
            }

            return result;
        }

        public IEnumerable<int> Elements => this.mExponentialTable;

        public int Generator => mExponentialTable[1];

        public int Add(int x, int y)
        {
            if (Characteristic == 2)
            {
                return x ^ y;
            }

            int result = 0;

            for (int i = 0; i < this.Degree; i++)
            {
                int currentXCoefficient = x % Characteristic;
                int currentYCoefficient = y % Characteristic;
                int currentSumCoefficient = (currentXCoefficient + currentYCoefficient) % Characteristic;
                result += currentSumCoefficient * this.Powers[i];
                x /= Characteristic;
                y /= Characteristic;
            }

            return result;
        }

        public int Subtract(int x, int y)
        {
            return Add(x, Negative(y));
        }

        public int Negative(int x)
        {
            if (Characteristic == 2)
            {
                return x;
            }

            int result = 0;

            for (int i = 0; i < this.Degree; i++)
            {
                int currentXCoefficient = x % Characteristic;
                int currentSumCoefficient = (Characteristic - currentXCoefficient) % Characteristic;
                result += currentSumCoefficient * this.Powers[i];
                x /= Characteristic;
            }

            return result;
        }

        public int Multiply(int x, int y)
        {
            if (x == 0 || y == 0)
            {
                return 0;
            }

            int exponent = mLogarithmicTable[x] + mLogarithmicTable[y];
            exponent = Modulo(exponent, NumberOfElements - 1);
            return mExponentialTable[exponent];
        }

        public int Inverse(int x)
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

            if (result >= 0)
            {
                return result;
            }

            return result + prime;
        }

        public string ToString(int fieldElement)
        {
            if (fieldElement < this.Characteristic)
            {
                return fieldElement.ToString();
            }

            return string.Join(" + ",
                               GetCoefficients(fieldElement).Select((x, i) => (x, i))
                                            .Where(x => x.x != 0)
                                            .Select(x => Format(x)));

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

        private IEnumerable<int> GetCoefficients(int fieldElement)
        {
            for (int i = 0; i < this.Degree; i++)
            {
                var currentCoefficient = fieldElement % Characteristic;
                yield return currentCoefficient;
                fieldElement /= Characteristic;
            }
        }

        public bool Equals(ArithmeticGaloisFieldWithTable other)
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
            return Equals((ArithmeticGaloisFieldWithTable)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Characteristic * 397) ^ Degree;
            }
        }
    }
}