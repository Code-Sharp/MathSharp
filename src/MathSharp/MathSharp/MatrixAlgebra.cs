namespace MathSharp;

public class MatrixAlgebra<TElement>
{
    private readonly IRing<TElement> _ring;

    public MatrixAlgebra(IRing<TElement> ring)
    {
        _ring = ring;
    }

    public Matrix<TElement> Add(Matrix<TElement> x, Matrix<TElement> y)
    {
        return InnerOperation(x, y, false);
    }

    private Matrix<TElement> InnerOperation(Matrix<TElement> x, Matrix<TElement> y, bool subtract)
    {
        if (x.Width != y.Width || x.Height != y.Height)
        {
            throw new ArgumentException("Expected first matrix's dimensions to match the second matrix's dimensions.");
        }

        var result = new Matrix<TElement>(x.Height, x.Width);

        for (int i = 0; i < result.Height; i++)
        {
            for (int j = 0; j < result.Width; j++)
            {
                TElement xValue = x.GetElement(i, j);
                TElement yValue = y.GetElement(i, j);

                TElement currentSum;

                if (subtract)
                {
                    currentSum = _ring.Subtract(xValue, yValue);
                }
                else
                {
                    currentSum = _ring.Add(xValue, yValue);
                }

                result.SetElement(i, j, currentSum);
            }
        }

        return result;
    }

    public Matrix<TElement> Subtract(Matrix<TElement> x, Matrix<TElement> y)
    {
        return InnerOperation(x, y, true);
    }

    public Matrix<TElement> Negative(Matrix<TElement> x)
    {
        var result = new Matrix<TElement>(x.Height, x.Width);

        for (int i = 0; i < result.Height; i++)
        {
            for (int j = 0; j < result.Width; j++)
            {
                TElement currentXValue = x.GetElement(i, j);
                TElement currentValue = _ring.Negative(currentXValue);
                result.SetElement(i, j, currentValue);
            }
        }

        return result;
    }

    public Matrix<TElement> Multiply(Matrix<TElement> x, Matrix<TElement> y)
    {
        if (x.Width != y.Height)
        {
            throw new ArgumentException("Expected first matrix's width to be the same as second matrix's height.");
        }

        var result = new Matrix<TElement>(x.Height, y.Width);

        for (int i = 0; i < result.Height; i++)
        {
            for (int j = 0; j < result.Width; j++)
            {
                TElement currentEntry = _ring.Zero;

                for (int k = 0; k < x.Width; k++)
                {
                    TElement xValue = x.GetElement(i, k);
                    TElement yValue = y.GetElement(k, j);
                    TElement currentProduct = _ring.Multiply(xValue, yValue);
                    currentEntry = _ring.Add(currentEntry, currentProduct);
                }

                result.SetElement(i, j, currentEntry);
            }
        }

        return result;
    }

    public Matrix<TElement> Identity(int dimension)
    {
        Matrix<TElement> result = new Matrix<TElement>(dimension, dimension);

        for (int i = 0; i < dimension; i++)
        {
            result.SetElement(i, i, _ring.One);
        }

        return result;
    }

    public Matrix<TElement> Inverse(Matrix<TElement> matrix)
    {
        if (matrix.Height != matrix.Width)
        {
            throw new ArgumentException("Expected a square matrix");
        }

        var result = Identity(matrix.Height);
        var current = new Matrix<TElement>(matrix);

        for (int j = 0; j < matrix.Width; j++)
        {
            int pivotIndex = Enumerable.Range(j, matrix.Height - j + 1)
                                       .FirstOrDefault(x => !Equals(matrix.GetElement(x, j), _ring.Zero),
                                                       matrix.Height);

            if (!(pivotIndex < matrix.Height))
            {
                throw new ArgumentException("Expected an invertible matrix");
            }

            TElement pivotValue = matrix.GetElement(pivotIndex, j);

            TElement pivotValueInverse = _ring.Inverse(pivotValue);

            for (int i = 0; i < matrix.Height; i++)
            {
                if (i != pivotIndex)
                {
                    TElement value = _ring.Negative(matrix.GetElement(i, j));
                    value = _ring.Multiply(pivotValueInverse, value);
                    AddMultipliedRow(result, value, pivotIndex, i);
                    AddMultipliedRow(matrix, value, pivotIndex, i);
                }
            }

            MultiplyRow(result, pivotIndex, pivotValueInverse);
            MultiplyRow(matrix, pivotIndex, pivotValueInverse);

            SwapRows(result, j, pivotIndex);
            SwapRows(matrix, j, pivotIndex);
        }

        return result;
    }

    private void AddMultipliedRow(Matrix<TElement> matrix, TElement scalar, int multipliedRowIndex, int rowIndex)
    {
        for (int j = 0; j < matrix.Width; j++)
        {
            TElement currentValueToMultiply = matrix.GetElement(multipliedRowIndex, j);
            TElement multiplied = _ring.Multiply(scalar, currentValueToMultiply);
            TElement currentValue = matrix.GetElement(rowIndex, j);
            currentValue = _ring.Add(currentValue, multiplied);
            matrix.SetElement(rowIndex, j, currentValue);
        }
    }

    private void MultiplyRow(Matrix<TElement> matrix, int row, TElement value)
    {
        for (int j = 0; j < matrix.Width; j++)
        {
            TElement currentValue = matrix.GetElement(row, j);
            TElement multiplied = _ring.Multiply(currentValue, value);
            matrix.SetElement(row, j, multiplied);
        }
    }

    private void SwapRows(Matrix<TElement> result, int row1, int row2)
    {
        if (row1 == row2)
        {
            return;
        }

        TElement[] row1Values = GetRowValues(row1);
        TElement[] row2Values = GetRowValues(row2);

        for (int j = 0; j < row1Values.Length; j++)
        {
            result.SetElement(row1, j, row2Values[j]);
            result.SetElement(row2, j, row1Values[j]);
        }

        TElement[] GetRowValues(int row)
        {
            return Enumerable.Range(0, result.Width)
                             .Select(x => result.GetElement(row, x))
                             .ToArray();
        }
    }
}