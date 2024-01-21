namespace MathSharp;

public static class MatrixConstructions
{
    public static Matrix<TElement> BlockMatrix<TElement>(Matrix<TElement>[,] blocks)
    {
        var blockHeight = blocks.GetLength(0);
        var blockWidth = blocks.GetLength(1);

        ValidateHeights(blocks, blockHeight, blockWidth);
        ValidateWidths(blocks, blockHeight, blockWidth);

        var width = Enumerable.Range(0, blockWidth)
                              .Sum(x => blocks[0, x].Width);

        var height = Enumerable.Range(0, blockHeight)
                               .Sum(x => blocks[x, 0].Height);

        var result = new Matrix<TElement>(height, width);
        
        var i = 0;
        var j = 0;

        for (int blockI = 0; blockI < blockHeight; blockI++)
        {
            var currentBlockHeight = 0;
            j = 0;

            for (int blockJ = 0; blockJ < blockWidth; blockJ++)
            {
                Matrix<TElement> currentBlock = blocks[blockI, blockJ];

                CopyBlock(result, i, j, currentBlock);

                j += currentBlock.Width;
                currentBlockHeight = currentBlock.Height;
            }

            i += currentBlockHeight;
        }

        return result;
    }

    private static void CopyBlock<TElement>(Matrix<TElement> copyTo, int row, int column, Matrix<TElement> value)
    {
        for (int i = 0; i < value.Height; i++)
        {
            for (int j = 0; j < value.Width; j++)
            {
                TElement valueToCopy = value.GetElement(i,j);
                copyTo.SetElement(row + i, column + j, valueToCopy);
            }
        }
    }

    private static void ValidateWidths<TElement>(Matrix<TElement>[,] blocks, int blockHeight, int blockWidth)
    {
        for (int j = 0; j < blockWidth; j++)
        {
            var expectedWidth = blocks[0, j].Width;

            bool heightsMatch =
                Enumerable.Range(0, blockHeight)
                          .All(x => blocks[x, j].Width == expectedWidth);

            if (!heightsMatch)
            {
                throw new ArgumentException("Expected all width to be the same. Error on column " + j);
            }
        }
    }

    private static void ValidateHeights<TElement>(Matrix<TElement>[,] blocks, int blockHeight, int blockWidth)
    {
        for (int i = 0; i < blockHeight; i++)
        {
            var expectedHeight = blocks[i, 0].Height;

            bool heightsMatch =
                Enumerable.Range(0, blockWidth)
                          .All(x => blocks[i, x].Height == expectedHeight);

            if (!heightsMatch)
            {
                throw new ArgumentException("Expected all heights to be the same. Error on row " + i);
            }
        }
    }
}