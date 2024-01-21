using MathSharp;
using NUnit.Framework;

namespace SageSharp.Tests;

[TestFixture]
public class MatrixConstructionsTests
{
    [Test]
    public void BlockMatrix_ConstructsCorrectBlockMatrix()
    {
        // Arrange
        Matrix<int>[,] blocks = new Matrix<int>[2, 2];

        // Block 1
        blocks[0, 0] = new Matrix<int>(2, 3);
        blocks[0, 0].SetElement(0, 0, 1);
        blocks[0, 0].SetElement(0, 1, 2);
        blocks[0, 0].SetElement(0, 2, 3);
        blocks[0, 0].SetElement(1, 0, 4);
        blocks[0, 0].SetElement(1, 1, 5);
        blocks[0, 0].SetElement(1, 2, 6);

        // Block 2
        blocks[0, 1] = new Matrix<int>(2, 2);
        blocks[0, 1].SetElement(0, 0, 7);
        blocks[0, 1].SetElement(0, 1, 8);
        blocks[0, 1].SetElement(1, 0, 9);
        blocks[0, 1].SetElement(1, 1, 10);

        // Block 3
        blocks[1, 0] = new Matrix<int>(1, 3);
        blocks[1, 0].SetElement(0, 0, 11);
        blocks[1, 0].SetElement(0, 1, 12);
        blocks[1, 0].SetElement(0, 2, 13);

        // Block 4
        blocks[1, 1] = new Matrix<int>(1, 2);
        blocks[1, 1].SetElement(0, 0, 14);
        blocks[1, 1].SetElement(0, 1, 15);

        // Act
        Matrix<int> result = MatrixConstructions.BlockMatrix(blocks);

        // Assert
        int[,] expectedValues = {
                                    { 1, 2, 3, 7, 8 },
                                    { 4, 5, 6, 9, 10 },
                                    { 11, 12, 13, 14, 15 }
                                };

        Assert.AreEqual(expectedValues.GetLength(0), result.Height);
        Assert.AreEqual(expectedValues.GetLength(1), result.Width);

        for (int i = 0; i < result.Height; i++)
        {
            for (int j = 0; j < result.Width; j++)
            {
                Assert.AreEqual(expectedValues[i, j], result.GetElement(i, j));
            }
        }
    }

    [Test]
    public void NonSquareBlockMatrix_ConstructsCorrectBlockMatrix()
    {
        // Arrange
        Matrix<int>[,] blocks = new Matrix<int>[2, 1];

        // Block 1
        blocks[0, 0] = new Matrix<int>(2, 3);
        blocks[0, 0].SetElement(0, 0, 1);
        blocks[0, 0].SetElement(0, 1, 2);
        blocks[0, 0].SetElement(0, 2, 3);
        blocks[0, 0].SetElement(1, 0, 4);
        blocks[0, 0].SetElement(1, 1, 5);
        blocks[0, 0].SetElement(1, 2, 6);

        // Block 2
        blocks[1, 0] = new Matrix<int>(1, 3);
        blocks[1, 0].SetElement(0, 0, 11);
        blocks[1, 0].SetElement(0, 1, 12);
        blocks[1, 0].SetElement(0, 2, 13);

        // Act
        Matrix<int> result = MatrixConstructions.BlockMatrix(blocks);

        // Assert
        int[,] expectedValues =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 11, 12, 13 }
        };

        Assert.AreEqual(expectedValues.GetLength(0), result.Height);
        Assert.AreEqual(expectedValues.GetLength(1), result.Width);

        for (int i = 0; i < result.Height; i++)
        {
            for (int j = 0; j < result.Width; j++)
            {
                Assert.AreEqual(expectedValues[i, j], result.GetElement(i, j));
            }
        }
    }
}