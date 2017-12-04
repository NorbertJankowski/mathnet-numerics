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
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Numerics.Threading;
using Anemon;

namespace MathNet.Numerics.LinearAlgebra.Single
{
    /// <summary>
    /// A Matrix class with dense storage. The underlying storage is a one dimensional array in column-major order (column by column).
    /// </summary>
    [Serializable]
    [DebuggerDisplay("DenseMatrixBM {RowCount}x{ColumnCount}-Single")]
    public class DenseMatrixBM : Matrix, IDisposable
    {
        public float keepAlive1 = default(float), keepAlive2 = default(float);
        public void KeepAlive(DenseMatrixBM a, DenseMatrixBM b = null)
        {
            keepAlive1 = a[0, 0];
            if (b != null)
                keepAlive2 = b[0, 0];
        }
        public void KeepAlive(float c)
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
        DenseColumnMajorMatrixStorageBM<float> _values;

        static ILinearAlgebraProviderBM<float> linearAlgebraProvider;
        static public void SetLinearAlgebraProvider(ILinearAlgebraProviderBM<float> provider)
        {
            linearAlgebraProvider = provider;
        }
        public ILinearAlgebraProviderBM<float> LinearAlgebraProvider
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
        public DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float> storage)
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
            : this(new DenseColumnMajorMatrixStorageBM<float>(order, order))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public DenseMatrixBM(int rows, int columns)
            : this(new DenseColumnMajorMatrixStorageBM<float>(rows, columns))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public DenseMatrixBM(int rows, int columns, float[] storage)
            : this(new DenseColumnMajorMatrixStorageBM<float>(rows, columns, storage))
        {
        }


        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public DenseMatrixBM(int rows, int columns, IntPtr storage)
            : this(new DenseColumnMajorMatrixStorageBM<float>(rows, columns, storage))
        {
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfMatrix(Matrix<float> matrix)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfMatrix(matrix.Storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfArray(float[,] array)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfArray(array));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfIndexed(int rows, int columns, IEnumerable<Tuple<int, int, float>> enumerable)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfIndexedEnumerable(rows, columns, enumerable));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnMajor(int rows, int columns, IEnumerable<float> columnMajor)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnMajorEnumerable(rows, columns, columnMajor));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumns(IEnumerable<IEnumerable<float>> data)
        {
            return OfColumnArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumns(int rows, int columns, IEnumerable<IEnumerable<float>> data)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnArrays(params float[][] columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnArrays(columns));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnArrays(IEnumerable<float[]> columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnArrays((columns as float[][]) ?? columns.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnVectors(params Vector<float>[] columns)
        {
            var storage = new VectorStorage<float>[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                storage[i] = columns[i].Storage;
            }
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfColumnVectors(IEnumerable<Vector<float>> columns)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfColumnVectors(columns.Select(c => c.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRows(IEnumerable<IEnumerable<float>> data)
        {
            return OfRowArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRows(int rows, int columns, IEnumerable<IEnumerable<float>> data)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfRowEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowArrays(params float[][] rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfRowArrays(rows));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowArrays(IEnumerable<float[]> rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfRowArrays((rows as float[][]) ?? rows.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowVectors(params Vector<float>[] rows)
        {
            var storage = new VectorStorage<float>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                storage[i] = rows[i].Storage;
            }
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfRowVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfRowVectors(IEnumerable<Vector<float>> rows)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfRowVectors(rows.Select(r => r.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrixBM OfDiagonalVector(Vector<float> diagonal)
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
        public static DenseMatrixBM OfDiagonalVector(int rows, int columns, Vector<float> diagonal)
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
        public static DenseMatrixBM OfDiagonalArray(float[] diagonal)
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
        public static DenseMatrixBM OfDiagonalArray(int rows, int columns, float[] diagonal)
        {
            var m = new DenseMatrixBM(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value to the same provided value.
        /// </summary>
        public static DenseMatrixBM Create(int rows, int columns, float value)
        {
            if (value == 0f) return new DenseMatrixBM(rows, columns);
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfValue(rows, columns, value));
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value using the provided init function.
        /// </summary>
        public static DenseMatrixBM Create(int rows, int columns, Func<int, int, float> init)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static DenseMatrixBM CreateDiagonal(int rows, int columns, float value)
        {
            if (value == 0f) return new DenseMatrixBM(rows, columns);
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfDiagonalInit(rows, columns, i => value));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static DenseMatrixBM CreateDiagonal(int rows, int columns, Func<int, float> init)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfDiagonalInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new square sparse identity matrix where each diagonal value is set to One.
        /// </summary>
        public static DenseMatrixBM CreateIdentity(int order)
        {
            return new DenseMatrixBM(DenseColumnMajorMatrixStorageBM<float>.OfDiagonalInit(order, order, i => One));
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the provided random distribution.
        /// </summary>
        public static DenseMatrixBM CreateRandom(int rows, int columns, IContinuousDistribution distribution)
        {
            return new DenseMatrixBM(new DenseColumnMajorMatrixStorageBM<float>(rows, columns, Generate.RandomSingle(rows*columns, distribution)));
        }

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        public DenseColumnMajorMatrixStorageBM<float> Values
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
        protected override void DoNegate(Matrix<float> result)
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
        protected override void DoAdd(float scalar, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoAdd(scalar, result);
                return;
            }

            DataTableStorage.DataTableStorage_Add_Float(_values.Data, denseResult.Values.Data, RowCount, ColumnCount, scalar);
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of add</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<float> other, Matrix<float> result)
        {
            // dense + dense = dense
            var denseOther = other.Storage as DenseColumnMajorMatrixStorageBM<float>;
            var denseResult = result.Storage as DenseColumnMajorMatrixStorageBM<float>;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.AddArrays(_values, denseOther, denseResult);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            // dense + diagonal = any
            var diagonalOther = other.Storage as DiagonalMatrixStorage<float>;
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
        protected override void DoSubtract(float scalar, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoSubtract(scalar, result);
                return;
            }

            DataTableStorage.DataTableStorage_Add_Float(_values.Data, denseResult.Values.Data, RowCount, ColumnCount, -scalar);
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(Matrix<float> other, Matrix<float> result)
        {
            // dense + dense = dense
            var denseOther = other.Storage as DenseColumnMajorMatrixStorageBM<float>;
            var denseResult = result.Storage as DenseColumnMajorMatrixStorageBM<float>;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.SubtractArrays(_values, denseOther, denseResult);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            // dense + diagonal = matrix
            var diagonalOther = other.Storage as DiagonalMatrixStorage<float>;
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
        protected override void DoMultiply(float scalar, Matrix<float> result)
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
        protected override void DoMultiply(Vector<float> rightSide, Vector<float> result)
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
        protected override void DoMultiply(Matrix<float> other, Matrix<float> result)
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

            var diagonalOther = other.Storage as DiagonalMatrixStorage<float>;
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
                    DataTableStorage.DataTableStorage_Multiply_Float(_values.Data, denseResult.Values.Data, j * RowCount, RowCount, diagonal[j]);
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
        protected override void DoTransposeAndMultiply(Matrix<float> other, Matrix<float> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    Providers.LinearAlgebra.Transpose.Transpose,
                    1.0f,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0f,
                    denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            var diagonalOther = other.Storage as DiagonalMatrixStorage<float>;
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
                    DataTableStorage.DataTableStorage_Multiply_Float(_values.Data, denseResult.Values.Data, j * RowCount, RowCount, diagonal[j]);
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
        protected override void DoTransposeThisAndMultiply(Vector<float> rightSide, Vector<float> result)
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
                    1.0f,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    0.0f,
                    denseResult.Values);
                KeepAlive(rightSide[0]);
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Matrix<float> other, Matrix<float> result)
        {
            var denseOther = other as DenseMatrixBM;
            var denseResult = result as DenseMatrixBM;
            if (denseOther != null && denseResult != null)
            {
                LinearAlgebraProvider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.Transpose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0f,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0f,
                    denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this, other as DenseMatrixBM);
                return;
            }

            var diagonalOther = other.Storage as DiagonalMatrixStorage<float>;
            if (diagonalOther != null)
            {
                this.Transpose(result);
                result.Multiply(other, result);
                return;
            }

            base.DoTransposeThisAndMultiply(other, result);
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(float divisor, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoDivide(divisor, result);
            }
            else
            {
                LinearAlgebraProvider.ScaleArray(1.0f/divisor, _values, denseResult._values);
                (result as DenseMatrixBM).KeepAlive(this);
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<float> other, Matrix<float> result)
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
        protected override void DoPointwiseDivide(Matrix<float> divisor, Matrix<float> result)
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
        protected override void DoPointwisePower(Matrix<float> exponent, Matrix<float> result)
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
        protected override void DoModulus(float divisor, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoModulus(divisor, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, (a, b) =>
            {
                var v = new float[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Float(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] = Euclid.Modulus(v[i], divisor);
                    }
                    DataTableStorage.DataTableStorage_SetRow_Float(denseResult.Values.Data, RowCount, i, v);
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
        protected override void DoModulusByThis(float dividend, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoModulusByThis(dividend, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, 4096, (a, b) =>
            {
                var v = new float[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Float(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] = Euclid.Modulus(dividend, v[i]);
                    }
                    DataTableStorage.DataTableStorage_SetRow_Float(denseResult.Values.Data, RowCount, i, v);
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
        protected override void DoRemainder(float divisor, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoRemainder(divisor, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, (a, b) =>
            {
                var v = new float[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Float(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] %= divisor;
                    }
                    DataTableStorage.DataTableStorage_SetRow_Float(denseResult.Values.Data, RowCount, i, v);
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
        protected override void DoRemainderByThis(float dividend, Matrix<float> result)
        {
            var denseResult = result as DenseMatrixBM;
            if (denseResult == null)
            {
                base.DoRemainderByThis(dividend, result);
                return;
            }

            CommonParallel.For(0, _values.ColumnCount, 4096, (a, b) =>
            {
                var v = new float[_values.RowCount];
                for (int i = a; i < b; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Float(_values.Data, RowCount, i, v);
                    for (int j = 0; j < RowCount; j++)
                    {
                        v[i] = dividend % v[i];
                    }
                    DataTableStorage.DataTableStorage_SetRow_Float(denseResult.Values.Data, RowCount, i, v);
                }
            });
            (result as DenseMatrixBM).KeepAlive(this);
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override float Trace()
        {
            if (_rowCount != _columnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            var sum = 0.0f;
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
        public static DenseMatrixBM operator *(DenseMatrixBM leftSide, float rightSide)
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
        public static DenseMatrixBM operator *(float leftSide, DenseMatrixBM rightSide)
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
        public static DenseMatrixBM operator %(DenseMatrixBM leftSide, float rightSide)
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
            var v = new float[ColumnCount];
            var h = new float[ColumnCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetColumn_Float(_values.Data, ColumnCount, ColumnCount, j, v);
                DataTableStorage.DataTableStorage_GetRow_Float(_values.Data, ColumnCount, j, h);
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

        public override Cholesky<float> Cholesky()
        {
            return DenseCholeskyBM.Create(this);
        }

        public override LU<float> LU()
        {
            return DenseLUBM.Create(this);
        }

        public override QR<float> QR(QRMethod method = QRMethod.Thin)
        {
            return DenseQRBM.Create(this, method);
        }

        public override GramSchmidt<float> GramSchmidt()
        {
            return DenseGramSchmidt.Create(this);
        }

        public override Svd<float> Svd(SVDVectorsComputation computeVectors = SVDVectorsComputation.VectorComputation)
        {
            return DenseSvdBM.Create(this, computeVectors);
        }

        public override Evd<float> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return DenseEvdBM.Create(this, symmetricity);
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
