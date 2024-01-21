namespace MathSharp;

public class Matrix<TElement>
{
    private readonly TElement[,] _elements;

    public Matrix(Matrix<TElement> other) :
        this(other.Height, other.Width)
    {
        for (int i = 0; i < other.Height; i++)
        {
            for (int j = 0; j < other.Width; j++)
            {
                TElement element = other.GetElement(i,j);
                this.SetElement(i,j, element);
            }
        }
    }

    public Matrix(int height, int width)
    {
        _elements = new TElement[height, width];
    }

    public TElement GetElement(int i, int j)
    {
        return _elements[i, j];
    }

    public void SetElement(int i, int j, TElement value)
    {
        _elements[i, j] = value;
    }

    public int Height => _elements.GetLength(0);

    public int Width => _elements.GetLength(1);

    public override string ToString()
    {
        List<string> result = new List<string>(Height);

        for (int i = 0; i < Height; i++)
        {
            string currentRow =
                string.Join(" & ", Enumerable.Range(0, Width).Select(x => _elements[i, x]));

            result.Add(currentRow);
        }

        var allRows = string.Join(@" \\", result);

        string matrixCode = $"\\begin{{pmatrix}} {allRows} \\end{{pmatrix}}";

        return matrixCode;
    }
}