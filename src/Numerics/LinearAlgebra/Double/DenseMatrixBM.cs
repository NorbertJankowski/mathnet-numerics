// <copyright file="DenseMatrixBM.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Numerics.Threading;
using Anemon;

namespace MathNet.Numerics.LinearAlgebra.Double
{
    /// <summary>
    /// A Matrix class with dense storage. The underlying storage is a one dimensional array in column-major order (column by column).
    /// </summary>
    [Serializable]
    [DebuggerDisplay("DenseMatrixBM {RowCount}x{ColumnCount}-Double")]
    public class DenseMatrixBM : Matrix, IDisposable
    {
        public double keepAlive1 = default(double), keepAlive2 = default(double);
        public void KeepAlive(DenseMatrixBM a, DenseMatrixBM b = null)
        {
            keepAlive1 = a[0, 0];
            if (b != null)
                keepAlive2 = b[0, 0];
        }
        public void KeepAlive(double c)
        {
            keepAlive2 = c;
        }
        /// <summary>
        /// Number of rows.
        /// </summary>
        /// <remarks>Using this instead of the RowCount property to speed up calculating
        /// a matrix index in the data array.</remarks>
        readonly int _rowCount;

        /// <summary>
        /// Number of columns.
        /// </summary>
        /// <remarks>Using this instead of the ColumnCount property to speed up calculating
        /// a matrix index in the data array.</remarks>
        readonly int _columnCount;

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        DenseColumnMajorMatrixStorageBM<double> _values;

        static ILinearAlgebraProviderBM<double> linearAlgebraProvider;
        static public void SetLinearAlgebraProvider(ILinearAlgebraProviderBM<double> provider)
        {
            linearAlgebraProvider = provider;
        }
        public ILinearAlgebraProviderBM<double> LinearAlgebraProvider
        {
            get
            {
                if (linearAlgebraProvider == null)
                {
                    var p = Control.LinearAlgebraProvider;
                    if (p == null)
                        throw new Exception("No appropriate linear algebra provider");
                }
                return linearAlgebraProvider;
            }
        }
        /// <summary>
        /// Create a new dense matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double> storage)
            : base(storage)
        {
            _rowCount = storage.RowCount;
            _columnCount = storage.ColumnCount;
            _values = storage;
        }

        /// <summary>
        /// Create a new square dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the order is less than one.</exception>
        public DenseMatrixBM(int order)
            : this(new DenseColumnMajorMatrixStorageBM<double>(order, order))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public DenseMatrixBM(int rows, int columns)
            : this(new DenseColumnMajorMatrixStorageBM<double>(rows, columns))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public DenseMatrixBM(int rows, int columns, double[] storage)
            : this(new DenseColumnMajorMatrixStorageBM<double>(rows, columns, storage))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public DenseMatrixBM(int rows, int columns, IntPtr storage)
            : this(new DenseColumnMajorMatrixStorageBM<double>(rows, columns, storage))
        {
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfMatrix(Matrix<double> matrix)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfMatrix(matrix.Storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfArray(double[,] array)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfArray(array));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfIndexed(int rows, int columns, IEnumerable<Tuple<int, int, double>> enumerable)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfIndexedEnumerable(rows, columns, enumerable));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnMajor(int rows, int columns, IEnumerable<double> columnMajor)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnMajorEnumerable(rows, columns, columnMajor));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumns(IEnumerable<IEnumerable<double>> data)
        {
            return OfColumnArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumns(int rows, int columns, IEnumerable<IEnumerable<double>> data)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnArrays(params double[][] columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnArrays(columns));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnArrays(IEnumerable<double[]> columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnArrays((columns as double[][]) ?? columns.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnVectors(params Vector<double>[] columns)
        {
            var storage = new VectorStorage<double>[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                storage[i] = columns[i].Storage;
            }
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnVectors(IEnumerable<Vector<double>> columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfColumnVectors(columns.Select(c => c.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRows(IEnumerable<IEnumerable<double>> data)
        {
            return OfRowArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRows(int rows, int columns, IEnumerable<IEnumerable<double>> data)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfRowEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowArrays(params double[][] rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfRowArrays(rows));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowArrays(IEnumerable<double[]> rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfRowArrays((rows as double[][]) ?? rows.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowVectors(params Vector<double>[] rows)
        {
            var storage = new VectorStorage<double>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                storage[i] = rows[i].Storage;
            }
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfRowVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowVectors(IEnumerable<Vector<double>> rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfRowVectors(rows.Select(r => r.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfDiagonalVector(Vector<double> diagonal)
        {
            var m = new DenseMatrixBM(diagonal.Count, diagonal.Count);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfDiagonalVector(int rows, int columns, Vector<double> diagonal)
        {
            var m = new DenseMatrixBM(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfDiagonalArray(double[] diagonal)
        {
            var m = new DenseMatrixBM(diagonal.Length, diagonal.Length);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfDiagonalArray(int rows, int columns, double[] diagonal)
        {
            var m = new DenseMatrixBM(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value to the same provided value.
        /// </summary>
        public static DenseMatrixBM Create(int rows, int columns, double value)
        {
            if (value == 0d) return new DenseMatrixBM(rows, columns);
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfValue(rows, columns, value));
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value using the provided init function.
        /// </summary>
        public static DenseMatrixBM Create(int rows, int columns, Func<int, int, double> init)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static DenseMatrixBM CreateDiagonal(int rows, int columns, double value)
        {
            if (value == 0d) return new DenseMatrixBM(rows, columns);
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfDiagonalInit(rows, columns, i => value));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static DenseMatrixBM CreateDiagonal(int rows, int columns, Func<int, double> init)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfDiagonalInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new square sparse identity matrix where each diagonal value is set to One.
        /// </summary>
        public static DenseMatrixBM CreateIdentity(int order)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<double>.OfDiagonalInit(order, order, i => One));
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the provided random distribution.
        /// </summary>
        public static DenseMatrixBM CreateRandom(int rows, int columns, IContinuousDistribution distribution)
        {
            return new DenseMatrixBM(new DenseColumnMajorMatrixStorageBM<double>(rows, columns, Generate.Random(rows * columns, distribution)));
        }

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        public DenseColumnMajorMatrixStorageBM<double> Values
        {
            get { return _values; }
        }

        /// <summary>Calculates the induced L1 norm of this matrix.</summary>
        /// <returns>The maximum absolute column sum of the matrix.</returns>
        public override double L1Norm()
        {
            return LinearAlgebraProvider.MatrixNorm(Norm.OneNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>Calculates the induced infinity norm of this matrix.</summary>
        /// <returns>The maximum absolute row sum of the matrix.</returns>
        public override double InfinityNorm()
        {
            return LinearAlgebraProvider.MatrixNorm(Norm.InfinityNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            return LinearAlgebraProvider.MatrixNorm(Norm.FrobeniusNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult != null)
            {
                LinearAlgebraProvider.ScaleArray(-1, _values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this);
                return;
            }

            base.DoNegate(result);
        }

        /// <summary>
        /// Add a scalar to each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        protected override void DoAdd(double scalar, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoAdd(scalar, result);
                return;
            }

            DataTableStorage.DataTableStorage_Add_Double(_values.Data, denseResult.Values.Data, RowCount, ColumnCount, scalar);
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of add</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<double> other, Matrix<double> result)
        {
            // dense + dense = dense
            var denseOther = other.Storage as DenseColumnMajorMatrixStorageBM<double>;
            var denseResult = result.Storage as DenseColumnMajorMatrixStorageBM<double>;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.AddArrays(_values, denseOther, denseResult);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            // dense + diagonal = any
            var diagonalOther = other.Storage as DiagonalMatrixStorage<double>;
            if (diagonalOther != null)
            {
                Storage.CopyToUnchecked(result.Storage, ExistingData.Clear);
                var diagonal = diagonalOther.Data;
                for (int i = 0; i < diagonal.Length; i++)
                {
                    result.At(i, i, result.At(i, i) + diagonal[i]);
                }
                return;
            }

            base.DoAdd(other, result);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(double scalar, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoSubtract(scalar, result);
                return;
            }

            DataTableStorage.DataTableStorage_Add_Double(_values.Data, denseResult.Values.Data, RowCount, ColumnCount, -scalar);
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(Matrix<double> other, Matrix<double> result)
        {
            // dense + dense = dense
            var denseOther = other.Storage as DenseColumnMajorMatrixStorageBM<double>;
            var denseResult = result.Storage as DenseColumnMajorMatrixStorageBM<double>;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.SubtractArrays(_values, denseOther, denseResult);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            // dense + diagonal = matrix
            var diagonalOther = other.Storage as DiagonalMatrixStorage<double>;
            if (diagonalOther != null)
            {
                CopyTo(result);
                var diagonal = diagonalOther.Data;
                for (int i = 0; i < diagonal.Length; i++)
                {
                    result.At(i, i, result.At(i, i) - diagonal[i]);
                }
                return;
            }

            base.DoSubtract(other, result);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(double scalar, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoMultiply(scalar, result);
            }
            else
            {
                LinearAlgebraProvider.ScaleArray(scalar, _values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this);
            }
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<double> rightSide, Vector<double> result)
        {
            var denseRight = rightSide as DenseVector;
            var denseResult = result as DenseVector;

            if (denseRight == null || denseResult == null)
            {
                base.DoMultiply(rightSide, result);
            }
            else
            {
                LinearAlgebraProvider.MatrixMultiply(
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    denseResult.Values);
                KeepAlive(rightSide[0]);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.MatrixMultiply(
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            var diagonalOther = other.Storage as DiagonalMatrixStorage<double>;
            if (diagonalOther != null)
            {
                var diagonal = diagonalOther.Data;
                var d = Math.Min(ColumnCount, other.ColumnCount);
                if (d < other.ColumnCount)
                {
                    result.ClearSubMatrix(0, RowCount, ColumnCount, other.ColumnCount - ColumnCount);
                }
                for (int j = 0; j < d; j++)
                {
                    DataTableStorage.DataTableStorage_Multiply_Double(_values.Data, denseResult.Values.Data, j * RowCount, RowCount, diagonal[j]);
                }
                return;
            }

            base.DoMultiply(other, result);
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    Providers.LinearAlgebra.Transpose.Transpose,
                    1.0,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0,
                    denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            var diagonalOther = other.Storage as DiagonalMatrixStorage<double>;
            if (diagonalOther != null)
            {
                var diagonal = diagonalOther.Data;
                var d = Math.Min(ColumnCount, other.RowCount);
                if (d < other.RowCount)
                {
                    result.ClearSubMatrix(0, RowCount, ColumnCount, other.RowCount - ColumnCount);
                }
                for (int j = 0; j < d; j++)
                {
                    DataTableStorage.DataTableStorage_Multiply_Double(_values.Data, denseResult.Values.Data, j * RowCount, RowCount, diagonal[j]);
                }
                return;
            }

            base.DoTransposeAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Vector<double> rightSide, Vector<double> result)
        {
            var denseRight = rightSide as DenseVector;
            var denseResult = result as DenseVector;

            if (denseRight == null || denseResult == null)
            {
                base.DoTransposeThisAndMultiply(rightSide, result);
            }
            else
            {
                LinearAlgebraProvider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.Transpose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    0.0,
                    denseResult.Values);
                KeepAlive(rightSide[0]);
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.Transpose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0,
                    denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            var diagonalOther = other.Storage as DiagonalMatrixStorage<double>;
            if (diagonalOther != null)
            {
                var m = this.Transpose();
                m.Multiply(other, result);
                return;
            }

            base.DoTransposeThisAndMultiply(other, result);
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(double divisor, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoDivide(divisor, result);
            }
            else
            {
                LinearAlgebraProvider.ScaleArray(1.0 / divisor, _values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this);
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<double> other, Matrix<double> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;

            if (denseOther == null || denseResult == null)
            {
                base.DoPointwiseMultiply(other, result);
            }
            else
            {
                LinearAlgebraProvider.PointWiseMultiplyArrays(_values, denseOther._values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
            }
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<double> divisor, Matrix<double> result)
        {
            var denseOther = divisor as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;

            if (denseOther == null || denseResult == null)
            {
                base.DoPointwiseDivide(divisor, result);
            }
            else
            {
                LinearAlgebraProvider.PointWiseDivideArrays(_values, denseOther._values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, denseOther as DenseMatrixBM);
            }
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Matrix<double> exponent, Matrix<double> result)
        {
            var denseExponent = exponent as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;

            if (denseExponent == null || denseResult == null)
            {
                base.DoPointwisePower(exponent, result);
            }
            else
            {
                LinearAlgebraProvider.PointWisePowerArrays(_values, denseExponent._values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, denseExponent as DenseMatrixBM);
            }
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoModulus(double divisor, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoModulus(divisor, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, (a, b) =>
            {
                var v = new double[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Double(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[j] = Euclid.Modulus(v[j], divisor);
                    }
                    DataTableStorage.DataTableStorage_SetRow_Double(denseResult.Values.Data, RowCount, i, v);
                }
            });
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulusByThis(double dividend, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoModulusByThis(dividend, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, 4096, (a, b) =>
            {
                var v = new double[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Double(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] = Euclid.Modulus(dividend, v[i]);
                    }
                    DataTableStorage.DataTableStorage_SetRow_Double(denseResult.Values.Data, RowCount, i, v);
                }
            });
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoRemainder(double divisor, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoRemainder(divisor, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, (a, b) =>
            {
                var v = new double[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Double(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[j] %= divisor;
                    }
                    DataTableStorage.DataTableStorage_SetRow_Double(denseResult.Values.Data, RowCount, i, v);
                }
            });
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainderByThis(double dividend, Matrix<double> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoRemainderByThis(dividend, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, 4096, (a, b) =>
            {
                var v = new double[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Double(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] = dividend % v[i];
                    }
                    DataTableStorage.DataTableStorage_SetRow_Double(denseResult.Values.Data, RowCount, i, v);
                }
            });
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override double Trace()
        {
            if (_rowCount != _columnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            var sum = 0.0;
            for (var i = 0; i < _rowCount; i++)
            {
                sum += _values.At(i, i);
            }

            return sum;
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator +(DenseMatrixBM leftSide, DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide._rowCount != rightSide._rowCount || leftSide._columnCount != rightSide._columnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide, rightSide);
            }

            return (DenseMatrixBM)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator +(DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (DenseMatrixBM)rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator -(DenseMatrixBM leftSide, DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide._rowCount != rightSide._rowCount || leftSide._columnCount != rightSide._columnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide, rightSide);
            }

            return (DenseMatrixBM)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator -(DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (DenseMatrixBM)rightSide.Negate();
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator *(DenseMatrixBM leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (DenseMatrixBM)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator *(double leftSide, DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (DenseMatrixBM)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static DenseMatrixBM operator *(DenseMatrixBM leftSide, DenseMatrixBM rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide._columnCount != rightSide._rowCount)
            {
                throw DimensionsDontMatch<ArgumentException>(leftSide, rightSide);
            }

            return (DenseMatrixBM)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a Vector.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseMatrixBM leftSide, DenseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (DenseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a Vector and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseVector leftSide, DenseMatrixBM rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (DenseVector)rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseMatrixBM operator %(DenseMatrixBM leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (DenseMatrixBM)leftSide.Remainder(rightSide);
        }

        /// <summary>
        /// Evaluates whether this matrix is symmetric.
        /// </summary>
        public override bool IsSymmetric()
        {
            if (RowCount != ColumnCount)
            {
                return false;
            }
            var v = new double[ColumnCount];
            var h = new double[ColumnCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetColumn_Double(_values.Data, ColumnCount, ColumnCount, j, v);
                DataTableStorage.DataTableStorage_GetRow_Double(_values.Data, ColumnCount, j, h);
                for (var i = j; i < ColumnCount; i++)
                {
                    if (v[i] != h[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override Cholesky<double> Cholesky()
        {
            return DenseCholeskyBM.Create(this);
        }

        public override LU<double> LU()
        {
            return DenseLUBM.Create(this);
        }

        public override QR<double> QR(QRMethod method = QRMethod.Thin)
        {
            return DenseQRBM.Create(this, method);
        }

        public override GramSchmidt<double> GramSchmidt()
        {
            return DenseGramSchmidtBM.Create(this);
        }

        public override Svd<double> Svd(SVDVectorsComputation computeVectors = SVDVectorsComputation.VectorComputation)
        {
            return DenseSvdBM.Create(this, computeVectors);
        }

        public override Evd<double> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return DenseEvdBM.Create(this, symmetricity);
        }

        public override Matrix<double> InverseTringular(bool upperMatrix, bool unitTriangular = false)
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            Matrix<double> m = this.Clone() as Matrix<double>;
            long i = LinearAlgebraProvider.TriangularInverse(upperMatrix, unitTriangular, this.RowCount,
                (m.Storage as DenseColumnMajorMatrixStorageBM<double>));
            if (i > 0)
                throw new Exception(string.Format("{0}-th diagonal element of A is zero, A is singular, and the inversion could not be completed", i));
            if (i < 0)
                throw new Exception("illegal return value of InverseTringular");
            return m;
        }

        public void Dispose()
        {
            if (_values != null)
                _values.Dispose();
            _values = null;
        }
        ~DenseMatrixBM()
        {
            Dispose();
        }
    }
}
