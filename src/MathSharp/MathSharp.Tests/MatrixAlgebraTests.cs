using MathSharp;
using MathSharp.Tests;
using NUnit.Framework;

namespace SageSharp.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class MatrixAlgebraTests
    {
        [Test]
        public void AddMatrices_Success()
        {
            // Arrange
            var ring = new IntegersRing();
            var matrixAlgebra = new MatrixAlgebra<int>(ring);

            // Create matrices with specific values
            var matrix1 = new Matrix<int>(2, 2);
            matrix1.SetElement(0, 0, 1);
            matrix1.SetElement(0, 1, 2);
            matrix1.SetElement(1, 0, 3);
            matrix1.SetElement(1, 1, 4);

            var matrix2 = new Matrix<int>(2, 2);
            matrix2.SetElement(0, 0, 5);
            matrix2.SetElement(0, 1, 6);
            matrix2.SetElement(1, 0, 7);
            matrix2.SetElement(1, 1, 8);

            // Act
            var result = matrixAlgebra.Add(matrix1, matrix2);

            // Assert
            Assert.AreEqual(6, result.GetElement(0, 0));
            Assert.AreEqual(8, result.GetElement(0, 1));
            Assert.AreEqual(10, result.GetElement(1, 0));
            Assert.AreEqual(12, result.GetElement(1, 1));
        }


        [Test]
        public void MultiplyMatrices_Success()
        {
            // Arrange
            var ring = new IntegersRing();
            var matrixAlgebra = new MatrixAlgebra<int>(ring);
            var matrix1 = new Matrix<int>(2, 3);
            var matrix2 = new Matrix<int>(3, 2);

            // Set elements for matrix1
            matrix1.SetElement(0, 0, 1);
            matrix1.SetElement(0, 1, 2);
            matrix1.SetElement(0, 2, 3);
            matrix1.SetElement(1, 0, 4);
            matrix1.SetElement(1, 1, 5);
            matrix1.SetElement(1, 2, 6);

            // Set elements for matrix2
            matrix2.SetElement(0, 0, 7);
            matrix2.SetElement(0, 1, 8);
            matrix2.SetElement(1, 0, 9);
            matrix2.SetElement(1, 1, 10);
            matrix2.SetElement(2, 0, 11);
            matrix2.SetElement(2, 1, 12);

            // Act
            var result = matrixAlgebra.Multiply(matrix1, matrix2);

            // Assert
            Assert.AreEqual(58, result.GetElement(0, 0));
            Assert.AreEqual(64, result.GetElement(0, 1));
            Assert.AreEqual(139, result.GetElement(1, 0));
            Assert.AreEqual(154, result.GetElement(1, 1));
        }

        [Test]
        public void Inverse_SpecificMatrix_ReturnsExpectedInverse()
        {
            // Arrange
            MatrixAlgebra<double> matrixAlgebra = new MatrixAlgebra<double>(new DoubleRing());
            Matrix<double> matrix = new Matrix<double>(3, 3);
            matrix.SetElement(0, 0, 2);
            matrix.SetElement(0, 1, 0);
            matrix.SetElement(0, 2, 1);
            matrix.SetElement(1, 0, 0);
            matrix.SetElement(1, 1, 1);
            matrix.SetElement(1, 2, 0);
            matrix.SetElement(2, 0, -1);
            matrix.SetElement(2, 1, 0);
            matrix.SetElement(2, 2, 2);

            // Define the expected inverse matrix
            Matrix<double> expectedInverse = new Matrix<double>(3, 3);
            expectedInverse.SetElement(0, 0, 0.4);
            expectedInverse.SetElement(0, 1, 0.0);
            expectedInverse.SetElement(0, 2, -0.2);
            expectedInverse.SetElement(1, 0, 0.0);
            expectedInverse.SetElement(1, 1, 1.0);
            expectedInverse.SetElement(1, 2, 0.0);
            expectedInverse.SetElement(2, 0, 0.2);
            expectedInverse.SetElement(2, 1, 0.0);
            expectedInverse.SetElement(2, 2, 0.4);

            // Act
            Matrix<double> actualInverse = matrixAlgebra.Inverse(matrix);

            // Assert
            Assert.AreEqual(expectedInverse.Height, actualInverse.Height);
            Assert.AreEqual(expectedInverse.Width, actualInverse.Width);

            for (int i = 0; i < expectedInverse.Height; i++)
            {
                for (int j = 0; j < expectedInverse.Width; j++)
                {
                    Assert.AreEqual(expectedInverse.GetElement(i, j), actualInverse.GetElement(i, j), 1e-10);
                }
            }
        }
    }
}