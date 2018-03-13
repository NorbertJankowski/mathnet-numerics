// <copyright file="MklLinearAlgebraProvider.Complex32.cs" company="Math.NET">
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

#if NATIVE

using System;
using System.Numerics;
using System.Security;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Providers.Common.Mkl;
using MathNet.Numerics.LinearAlgebra.Storage;
using Anemon;

namespace MathNet.Numerics.Providers.LinearAlgebra.Mkl
{
    /// <summary>
    /// Intel's Math Kernel Library (MKL) linear algebra provider.
    /// </summary>
    public class MklLinearAlgebraProvider_Complex32 : MklLinearAlgebraProviderBM, ILinearAlgebraProviderBM<Complex32>
    {
        public MklLinearAlgebraProvider_Complex32(
            Common.Mkl.MklConsistency consistency = Common.Mkl.MklConsistency.Auto,
            Common.Mkl.MklPrecision precision = Common.Mkl.MklPrecision.Double,
            Common.Mkl.MklAccuracy accuracy = Common.Mkl.MklAccuracy.High)
            : base(consistency, precision, accuracy)
        {
        }


        /// <summary>
        /// Computes the requested <see cref="Norm"/> of the matrix.
        /// </summary>
        /// <param name="norm">The type of norm to compute.</param>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="matrix">The matrix to compute the norm from.</param>
        /// <returns>
        /// The requested <see cref="Norm"/> of the matrix.
        /// </returns>
        [SecuritySafeCritical]
        public double MatrixNorm(Norm norm, int rows, int columns, IStorageBM matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            if (rows <= 0)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "rows");
            }

            if (columns <= 0)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "columns");
            }

            if (matrix.Length < (long)rows * columns)
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, (long)rows * columns), "matrix");
            }

            return SafeNativeMethodsBM_Complex32.c_matrix_norm((byte)norm, rows, columns, matrix.Data);
        }

        /// <summary>
        /// Computes the dot product of x and y.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <param name="y">The vector y.</param>
        /// <returns>The dot product of x and y.</returns>
        /// <remarks>This is equivalent to the DOT BLAS routine.</remarks>
        [SecuritySafeCritical]
        public Complex32 DotProduct(IStorageBM x, IStorageBM y)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            return SafeNativeMethodsBM_Complex32.c_dot_product(x.Length, x.Data, y.Data);
        }

        /// <summary>
        /// Adds a scaled vector to another: <c>result = y + alpha*x</c>.
        /// </summary>
        /// <param name="y">The vector to update.</param>
        /// <param name="alpha">The value to scale <paramref name="x"/> by.</param>
        /// <param name="x">The vector to add to <paramref name="y"/>.</param>
        /// <param name="result">The result of the addition.</param>
        /// <remarks>This is similar to the AXPY BLAS routine.</remarks>
        [SecuritySafeCritical]
        public void AddVectorToScaledVector(IStorageBM y, Complex32 alpha, IStorageBM x, IStorageBM result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y.Length != x.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (!ReferenceEquals(y, result))
            {
                DataTableStorage.DataTableStorage_GetRow_Complex32(y.Data, y.Length, 0, result.Data);
            }

            if (alpha == Complex32.Zero)
            {
                return;
            }

            SafeNativeMethodsBM_Complex32.c_axpy(y.Length, alpha, x.Data, result.Data);
        }

        /// <summary>
        /// Scales an array. Can be used to scale a vector and a matrix.
        /// </summary>
        /// <param name="alpha">The scalar.</param>
        /// <param name="x">The values to scale.</param>
        /// <param name="result">This result of the scaling.</param>
        /// <remarks>This is similar to the SCAL BLAS routine.</remarks>
        [SecuritySafeCritical]
        public void ScaleArray(Complex32 alpha, IStorageBM x, IStorageBM result)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (!ReferenceEquals(x, result))
            {
                DataTableStorage.DataTableStorage_GetRow_Complex32(x.Data, x.Length, 0, result.Data);
            }

            if (alpha == Complex32.One)
            {
                return;
            }

            SafeNativeMethodsBM_Complex32.c_scale(x.Length, alpha, result.Data);
        }

        /// <summary>
        /// Conjugates an array. Can be used to conjugate a vector and a matrix.
        /// </summary>
        /// <param name="x">The values to conjugate.</param>
        /// <param name="result">This result of the conjugation.</param>
        public void ConjugateArray(IStorageBM x, IStorageBM result)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (!ReferenceEquals(x, result))
            {
                DenseColumnMajorMatrixStorageBM<Complex32> _x = x as DenseColumnMajorMatrixStorageBM<Complex32>;
                DenseColumnMajorMatrixStorageBM<Complex32> _result = result as DenseColumnMajorMatrixStorageBM<Complex32>;
                DataTableStorage.DataTableStorage_ConjugateArray_Complex32(_x.Data, _result.Data, _x.Length);
            }
        }
        /// <summary>
        /// Multiples two matrices. <c>result = x * y</c>
        /// </summary>
        /// <param name="x">The x matrix.</param>
        /// <param name="rowsX">The number of rows in the x matrix.</param>
        /// <param name="columnsX">The number of columns in the x matrix.</param>
        /// <param name="y">The y matrix.</param>
        /// <param name="rowsY">The number of rows in the y matrix.</param>
        /// <param name="columnsY">The number of columns in the y matrix.</param>
        /// <param name="result">Where to store the result of the multiplication.</param>
        /// <remarks>This is a simplified version of the BLAS GEMM routine with alpha
        /// set to Complex32.One and beta set to Complex32.Zero, and x and y are not transposed.</remarks>
        public  void MatrixMultiply(IStorageBM x, int rowsX, int columnsX, IStorageBM y, int rowsY, int columnsY, IStorageBM result)
        {
            MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, Complex32.One, x, rowsX, columnsX, y, rowsY, columnsY, Complex32.Zero, result);
        }
        public void MatrixMultiply(IStorageBM x, int rowsX, int columnsX, Complex32[] y, int rowsY, int columnsY, Complex32[] result)
        {
            MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, Complex32.One, x, rowsX, columnsX, y, rowsY, columnsY, Complex32.Zero, result);
        }

        /// <summary>
        /// Multiplies two matrices and updates another with the result. <c>c = alpha*op(a)*op(b) + beta*c</c>
        /// </summary>
        /// <param name="transposeA">How to transpose the <paramref name="a"/> matrix.</param>
        /// <param name="transposeB">How to transpose the <paramref name="b"/> matrix.</param>
        /// <param name="alpha">The value to scale <paramref name="a"/> matrix.</param>
        /// <param name="a">The a matrix.</param>
        /// <param name="rowsA">The number of rows in the <paramref name="a"/> matrix.</param>
        /// <param name="columnsA">The number of columns in the <paramref name="a"/> matrix.</param>
        /// <param name="b">The b matrix</param>
        /// <param name="rowsB">The number of rows in the <paramref name="b"/> matrix.</param>
        /// <param name="columnsB">The number of columns in the <paramref name="b"/> matrix.</param>
        /// <param name="beta">The value to scale the <paramref name="c"/> matrix.</param>
        /// <param name="c">The c matrix.</param>
        [SecuritySafeCritical]
        public void MatrixMultiplyWithUpdate(Transpose transposeA, Transpose transposeB, Complex32 alpha, IStorageBM a, int rowsA, int columnsA, IStorageBM b, int rowsB, int columnsB, Complex32 beta, IStorageBM c)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (c == null)
            {
                throw new ArgumentNullException("c");
            }

            var m = transposeA == Transpose.DontTranspose ? rowsA : columnsA;
            var n = transposeB == Transpose.DontTranspose ? columnsB : rowsB;
            var k = transposeA == Transpose.DontTranspose ? columnsA : rowsA;
            var l = transposeB == Transpose.DontTranspose ? rowsB : columnsB;

            if (c.Length != (long)m *n)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            if (k != l)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            SafeNativeMethodsBM_Complex32.c_matrix_multiply(transposeA, transposeB, m, n, k, alpha, a.Data, b.Data, beta, c.Data);
        }
        public void MatrixMultiplyWithUpdate(Transpose transposeA, Transpose transposeB, Complex32 alpha, IStorageBM a, int rowsA, int columnsA, Complex32[] b, int rowsB, int columnsB, Complex32 beta, Complex32[] c)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (c == null)
            {
                throw new ArgumentNullException("c");
            }

            var m = transposeA == Transpose.DontTranspose ? rowsA : columnsA;
            var n = transposeB == Transpose.DontTranspose ? columnsB : rowsB;
            var k = transposeA == Transpose.DontTranspose ? columnsA : rowsA;
            var l = transposeB == Transpose.DontTranspose ? rowsB : columnsB;

            if (c.Length != (long)m * n)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            if (k != l)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            SafeNativeMethodsBM_Complex32.c_matrix_multiply(transposeA, transposeB, m, n, k, alpha, a.Data, b, beta, c);
        }

        /// <summary>
        /// Computes the LUP factorization of A. P*A = L*U.
        /// </summary>
        /// <param name="data">An <paramref name="order"/> by <paramref name="order"/> matrix. The matrix is overwritten with the
        /// the LU factorization on exit. The lower triangular factor L is stored in under the diagonal of <paramref name="data"/> (the diagonal is always Complex32.One
        /// for the L factor). The upper triangular factor U is stored on and above the diagonal of <paramref name="data"/>.</param>
        /// <param name="order">The order of the square matrix <paramref name="data"/>.</param>
        /// <param name="ipiv">On exit, it contains the pivot indices. The size of the array must be <paramref name="order"/>.</param>
        /// <remarks>This is equivalent to the GETRF LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void LUFactor(IStorageBM data, int order, long[] ipiv)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (data.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "data");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_factor(order, data.Data, ipiv);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Computes the inverse of matrix using LU factorization.
        /// </summary>
        /// <param name="a">The N by N matrix to invert. Contains the inverse On exit.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <remarks>This is equivalent to the GETRF and GETRI LAPACK routines.</remarks>
        [SecuritySafeCritical]
        public void LUInverse(IStorageBM a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_inverse(order, a.Data);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new SingularUMatrixException(info);
            }
        }

        /// <summary>
        /// Computes the inverse of a previously factored matrix.
        /// </summary>
        /// <param name="a">The LU factored N by N matrix.  Contains the inverse On exit.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="ipiv">The pivot indices of <paramref name="a"/>.</param>
        /// <remarks>This is equivalent to the GETRI LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void LUInverseFactored(IStorageBM a, int order, long[] ipiv)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_inverse_factored(order, a.Data, ipiv);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new SingularUMatrixException(info);
            } 
        }

        /// <summary>
        /// Solves A*X=B for X using LU factorization.
        /// </summary>
        /// <param name="columnsOfB">The number of columns of B.</param>
        /// <param name="a">The square matrix A.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <remarks>This is equivalent to the GETRF and GETRS LAPACK routines.</remarks>
        [SecuritySafeCritical]
        public void LUSolve(int columnsOfB, IStorageBM a, int order, IStorageBM b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != (long)columnsOfB *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_solve(order, columnsOfB, a.Data, b.Data);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Solves A*X=B for X using a previously factored A matrix.
        /// </summary>
        /// <param name="columnsOfB">The number of columns of B.</param>
        /// <param name="a">The factored A matrix.</param>
        /// <param name="order">The order of the square matrix <paramref name="a"/>.</param>
        /// <param name="ipiv">The pivot indices of <paramref name="a"/>.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <remarks>This is equivalent to the GETRS LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void LUSolveFactored(int columnsOfB, IStorageBM a, int order, long[] ipiv, IStorageBM b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            if (b.Length != (long)columnsOfB *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_solve_factored(order, columnsOfB, a.Data, ipiv, b.Data);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }
        public void LUSolveFactored(int columnsOfB, IStorageBM a, int order, long[] ipiv, Complex32[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != (long)order * order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            if (b.Length != (long)columnsOfB * order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_lu_solve_factored(order, columnsOfB, a.Data, ipiv, b);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Computes the Cholesky factorization of A.
        /// </summary>
        /// <param name="a">On entry, a square, positive definite matrix. On exit, the matrix is overwritten with the
        /// the Cholesky factorization.</param>
        /// <param name="order">The number of rows or columns in the matrix.</param>
        /// <remarks>This is equivalent to the POTRF LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void CholeskyFactor(IStorageBM a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (order < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "order");
            }

            if (a.Length != (long)order *order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            var info = SafeNativeMethodsBM_Complex32.c_cholesky_factor(order, a.Data);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new ArgumentException(Resources.ArgumentMatrixPositiveDefinite);
            }
        }

        /// <summary>
        /// Solves A*X=B for X using Cholesky factorization.
        /// </summary>
        /// <param name="a">The square, positive definite matrix A.</param>
        /// <param name="orderA">The number of rows and columns in A.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <param name="columnsB">The number of columns in the B matrix.</param>
        /// <remarks>This is equivalent to the POTRF add POTRS LAPACK routines.
        /// </remarks>
        [SecuritySafeCritical]
        public void CholeskySolve(IStorageBM a, int orderA, IStorageBM b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != (long)orderA *columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_cholesky_solve(orderA, columnsB, a.Data, b.Data);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Solves A*X=B for X using a previously factored A matrix.
        /// </summary>
        /// <param name="a">The square, positive definite matrix A.</param>
        /// <param name="orderA">The number of rows and columns in A.</param>
        /// <param name="b">On entry the B matrix; on exit the X matrix.</param>
        /// <param name="columnsB">The number of columns in the B matrix.</param>
        /// <remarks>This is equivalent to the POTRS LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void CholeskySolveFactored(IStorageBM a, int orderA, IStorageBM b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != (long)orderA *columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_cholesky_solve_factored(orderA, columnsB, a.Data, b.Data);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }
        public void CholeskySolveFactored(IStorageBM a, int orderA, Complex32[] b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != (long)orderA * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethodsBM_Complex32.c_cholesky_solve_factored(orderA, columnsB, a.Data, b);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Computes the QR factorization of A.
        /// </summary>
        /// <param name="r">On entry, it is the M by N A matrix to factor. On exit,
        /// it is overwritten with the R matrix of the QR factorization. </param>
        /// <param name="rowsR">The number of rows in the A matrix.</param>
        /// <param name="columnsR">The number of columns in the A matrix.</param>
        /// <param name="q">On exit, A M by M matrix that holds the Q matrix of the
        /// QR factorization.</param>
        /// <param name="tau">A min(m,n) vector. On exit, contains additional information
        /// to be used by the QR solve routine.</param>
        /// <remarks>This is similar to the GEQRF and ORGQR LAPACK routines.</remarks>
        [SecuritySafeCritical]
        public void QRFactor(IStorageBM r, int rowsR, int columnsR, IStorageBM q, Complex32[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (r.Length != (long)rowsR *columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "r");
            }

            if (tau.Length < Math.Min(rowsR, columnsR))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (q.Length != (long)rowsR *rowsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * rowsR"), "q");
            }

            var info = SafeNativeMethodsBM_Complex32.c_qr_factor(rowsR, columnsR, r.Data, tau, q.Data);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Computes the thin QR factorization of A where M &gt; N.
        /// </summary>
        /// <param name="q">On entry, it is the M by N A matrix to factor. On exit,
        /// it is overwritten with the Q matrix of the QR factorization.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="r">On exit, A N by N matrix that holds the R matrix of the
        /// QR factorization.</param>
        /// <param name="tau">A min(m,n) vector. On exit, contains additional information
        /// to be used by the QR solve routine.</param>
        /// <remarks>This is similar to the GEQRF and ORGQR LAPACK routines.</remarks>
        [SecuritySafeCritical]
        public void ThinQRFactor(IStorageBM q, int rowsA, int columnsA, IStorageBM r, Complex32[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (q.Length != (long)rowsA * columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "q");
            }

            if (tau.Length < Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (r.Length != (long)columnsA * columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "columnsA * columnsA"), "r");
            }

            var info = SafeNativeMethodsBM_Complex32.c_qr_thin_factor(rowsA, columnsA, q.Data, tau, r.Data);

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }
        }

        /// <summary>
        /// Solves A*X=B for X using QR factorization of A.
        /// </summary>
        /// <param name="a">The A matrix.</param>
        /// <param name="rows">The number of rows in the A matrix.</param>
        /// <param name="columns">The number of columns in the A matrix.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        /// <param name="method">The type of QR factorization to perform. <seealso cref="QRMethod"/></param>
        /// <remarks>Rows must be greater or equal to columns.</remarks>
        [SecuritySafeCritical]
        public void QRSolve(IStorageBM a, int rows, int columns, Complex32[] b, int columnsB, Complex32[] x, QRMethod method = QRMethod.Full)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (a.Length != (long)rows * columns)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != (long)rows * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != (long)columns * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "x");
            }

            if (rows < columns)
            {
                throw new ArgumentException(Resources.RowsLessThanColumns);
            }

            var info = SafeNativeMethodsBM_Complex32.c_qr_solve(rows, columns, columnsB, a.Data, b, x);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new ArgumentException(Resources.ArgumentMatrixNotRankDeficient, "a");
            }
        }

        /// <summary>
        /// Solves A*X=B for X using a previously QR factored matrix.
        /// </summary>
        /// <param name="q">The Q matrix obtained by calling <see cref="QRFactor(IStorageBM,int,int,IStorageBM,IStorageBM)"/>.</param>
        /// <param name="r">The R matrix obtained by calling <see cref="QRFactor(IStorageBM,int,int,IStorageBM,IStorageBM)"/>. </param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="tau">Contains additional information on Q. Only used for the native solver
        /// and can be <c>null</c> for the managed provider.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        /// <param name="method">The type of QR factorization to perform. <seealso cref="QRMethod"/></param>
        /// <remarks>Rows must be greater or equal to columns.</remarks>
        [SecuritySafeCritical]
        public void QRSolveFactored(IStorageBM q, IStorageBM r, int rowsA, int columnsA, Complex32[] tau, IStorageBM b, int columnsB, IStorageBM x, QRMethod method = QRMethod.Full)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (b == null)
            {
                throw new ArgumentNullException("q");
            }

            if (x == null)
            {
                throw new ArgumentNullException("q");
            }

            int rowsQ, columnsQ, rowsR, columnsR;
            if (method == QRMethod.Full)
            {
                rowsQ = columnsQ = rowsR = rowsA;
                columnsR = columnsA;
            }
            else
            {
                rowsQ = rowsA;
                columnsQ = rowsR = columnsR = columnsA;
            }

            if (r.Length != (long)rowsR * columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsR * columnsR), "r");
            }

            if (q.Length != (long)rowsQ * columnsQ)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsQ * columnsQ), "q");
            }

            if (b.Length != (long)rowsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsA * columnsB), "b");
            }

            if (x.Length != (long)columnsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)columnsA * columnsB), "x");
            }

            if (method == QRMethod.Full)
            {
                var info = SafeNativeMethodsBM_Complex32.c_qr_solve_factored(rowsA, columnsA, columnsB, r.Data, b.Data, tau, x.Data);
                
                if (info == (int)MklError.MemoryAllocation)
                {
                    throw new MemoryAllocationException();
                }

                if (info < 0)
                {
                    throw new InvalidParameterException(Math.Abs(info));
                }
            }
            else
            {
                // we don't have access to the raw Q matrix any more(it is stored in R in the full QR), need to think about this.
                // let just call the managed version in the meantime. The heavy lifting has already been done. -marcus
                //base.QRSolveFactored(q, r, rowsA, columnsA, tau, b, columnsB, x, QRMethod.Thin);
                throw new Exception("There is no thin QRSolveFactored in MKL");
            }
        }
        public void QRSolveFactored(IStorageBM q, IStorageBM r, int rowsA, int columnsA, Complex32[] tau, Complex32[] b, int columnsB, Complex32[] x, QRMethod method = QRMethod.Full)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (b == null)
            {
                throw new ArgumentNullException("q");
            }

            if (x == null)
            {
                throw new ArgumentNullException("q");
            }

            int rowsQ, columnsQ, rowsR, columnsR;
            if (method == QRMethod.Full)
            {
                rowsQ = columnsQ = rowsR = rowsA;
                columnsR = columnsA;
            }
            else
            {
                rowsQ = rowsA;
                columnsQ = rowsR = columnsR = columnsA;
            }

            if (r.Length != (long)rowsR * columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsR * columnsR), "r");
            }

            if (q.Length != (long)rowsQ * columnsQ)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsQ * columnsQ), "q");
            }

            if (b.Length != (long)rowsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)rowsA * columnsB), "b");
            }

            if (x.Length != (long)columnsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, (long)columnsA * columnsB), "x");
            }

            if (method == QRMethod.Full)
            {
                var info = SafeNativeMethodsBM_Complex32.c_qr_solve_factored(rowsA, columnsA, columnsB, r.Data, b, tau, x);

                if (info == (int)MklError.MemoryAllocation)
                {
                    throw new MemoryAllocationException();
                }

                if (info < 0)
                {
                    throw new InvalidParameterException(Math.Abs(info));
                }
            }
            else
            {
                // we don't have access to the raw Q matrix any more(it is stored in R in the full QR), need to think about this.
                // let just call the managed version in the meantime. The heavy lifting has already been done. -marcus
                //base.QRSolveFactored(q, r, rowsA, columnsA, tau, b, columnsB, x, QRMethod.Thin);
                throw new Exception("There is no thin QRSolveFactored in MKL");
            }
        }

        /// <summary>
        /// Solves A*X=B for X using the singular value decomposition of A.
        /// </summary>
        /// <param name="a">On entry, the M by N matrix to decompose.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        public void SvdSolve(IStorageBM a, int rowsA, int columnsA, Complex32[] b, int columnsB, Complex32[] x)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (b.Length != (long)rowsA *columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != (long)columnsA *columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            var s = new Complex32[Math.Min(rowsA, columnsA)];
            var u = new DenseColumnMajorMatrixStorageBM<Complex32>(rowsA, rowsA);
            var vt = new DenseColumnMajorMatrixStorageBM<Complex32>(columnsA, columnsA);

            var clone = new DenseColumnMajorMatrixStorageBM<Complex32>(rowsA, columnsA);
            DataTableStorage.DataTableStorage_GetRow_Complex32(a.Data, a.Length, 0, clone.Data);
            SingularValueDecomposition(SVDVectorsComputation.VectorComputation, clone, rowsA, columnsA, s, u, vt);
            SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }

        /// <summary>
        /// Computes the singular value decomposition of A.
        /// </summary>
        /// <param name="computeVectors">Compute the singular U and VT vectors or not.</param>
        /// <param name="a">On entry, the M by N matrix to decompose. On exit, A may be overwritten.</param>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="s">The singular values of A in ascending value.</param>
        /// <param name="u">If <paramref name="computeVectors"/> is <c>true</c>, on exit U contains the left
        /// singular vectors.</param>
        /// <param name="vt">If <paramref name="computeVectors"/> is <c>true</c>, on exit VT contains the transposed
        /// right singular vectors.</param>
        /// <remarks>This is equivalent to the GESVD LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public void SingularValueDecomposition(SVDVectorsComputation computeVectors, IStorageBM a, int rowsA, int columnsA, Complex32[] s, IStorageBM u, IStorageBM vt)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            if (vt == null)
            {
                throw new ArgumentNullException("vt");
            }

            int nm = Math.Min(rowsA, columnsA);
            if ((computeVectors != SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (u.Length != (long)rowsA * rowsA) ||
                (computeVectors == SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (u.Length != (long)rowsA * nm))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "u");
            }

            if ((computeVectors != SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (vt.Length != (long)columnsA * columnsA) ||
                (computeVectors == SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (vt.Length != (long)columnsA * nm))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "vt");
            }

            if (s.Length != Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "s");
            }

            var info = SafeNativeMethodsBM_Complex32.c_svd_factor((char)computeVectors, rowsA, columnsA, a.Data, s, u.Data, vt.Data);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new NonConvergenceException();
            }
        }

        /// <summary>
        /// Does a point wise add of two arrays <c>z = x + y</c>. This can be used
        /// to add vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the addition.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public void AddArrays(IStorageBM x, IStorageBM y, IStorageBM result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            if (x.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            SafeNativeMethodsBM_Complex32.c_vector_add(x.Length, x.Data, y.Data, result.Data);
        }

        /// <summary>
        /// Does a point wise subtraction of two arrays <c>z = x - y</c>. This can be used
        /// to subtract vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the subtraction.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public void SubtractArrays(IStorageBM x, IStorageBM y, IStorageBM result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            if (x.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            SafeNativeMethodsBM_Complex32.c_vector_subtract(x.Length, x.Data, y.Data, result.Data);
        }

        /// <summary>
        /// Does a point wise multiplication of two arrays <c>z = x * y</c>. This can be used
        /// to multiple elements of vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the point wise multiplication.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public void PointWiseMultiplyArrays(IStorageBM x, IStorageBM y, IStorageBM result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            if (x.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            SafeNativeMethodsBM_Complex32.c_vector_multiply(x.Length, x.Data, y.Data, result.Data);
        }

        /// <summary>
        /// Does a point wise power of two arrays <c>z = x ^ y</c>. This can be used
        /// to raise elements of vectors or matrices to the powers of another vector or matrix.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the point wise power.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public void PointWisePowerArrays(IStorageBM x, IStorageBM y, IStorageBM result)
        {
            if (_vectorFunctionsMajor != 0 || _vectorFunctionsMinor < 1)
            {
                DenseColumnMajorMatrixStorageBM<Complex32> _x = x as DenseColumnMajorMatrixStorageBM<Complex32>;
                DenseColumnMajorMatrixStorageBM<Complex32> _y = y as DenseColumnMajorMatrixStorageBM<Complex32>;
                DenseColumnMajorMatrixStorageBM<Complex32> _result = result as DenseColumnMajorMatrixStorageBM<Complex32>;
                var vx = new Complex32[_x.RowCount];
                var vy = new Complex32[_x.RowCount];
                for (int i = 0; i < _x.ColumnCount; i++)
                {
                    DataTableStorage.DataTableStorage_GetRow_Complex32(_x.Data, i, _x.RowCount, vx);
                    DataTableStorage.DataTableStorage_GetRow_Complex32(_y.Data, i, _y.RowCount, vy);
                    for (int j = 0; j < _x.RowCount; j++)
                    {
                        vx[j] = Complex32.Pow(vx[j], vy[j]);
                    }
                    DataTableStorage.DataTableStorage_SetRow_Complex32(_result.Data, i, _x.RowCount, vx);
                }
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            if (x.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            SafeNativeMethodsBM_Complex32.c_vector_power(x.Length, x.Data, y.Data, result.Data);
        }

        /// <summary>
        /// Does a point wise division of two arrays <c>z = x / y</c>. This can be used
        /// to divide elements of vectors or matrices.
        /// </summary>
        /// <param name="x">The array x.</param>
        /// <param name="y">The array y.</param>
        /// <param name="result">The result of the point wise division.</param>
        /// <remarks>There is no equivalent BLAS routine, but many libraries
        /// provide optimized (parallel and/or vectorized) versions of this
        /// routine.</remarks>
        public void PointWiseDivideArrays(IStorageBM x, IStorageBM y, IStorageBM result)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (x.Length != y.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            if (x.Length != result.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength);
            }

            SafeNativeMethodsBM_Complex32.c_vector_divide(x.Length, x.Data, y.Data, result.Data);
        }

        /// <summary>
        /// Computes the eigenvalues and eigenvectors of a matrix.
        /// </summary>
        /// <param name="isSymmetric">Whether the matrix is symmetric or not.</param>
        /// <param name="order">The order of the matrix.</param>
        /// <param name="matrix">The matrix to decompose. The lenth of the array must be order * order.</param>
        /// <param name="matrixEv">On output, the matrix contains the eigen vectors. The lenth of the array must be order * order.</param>
        /// <param name="vectorEv">On output, the eigen values (¦Ë) of matrix in ascending value. The length of the arry must <paramref name="order"/>.</param>
        /// <param name="matrixD">On output, the block diagonal eigenvalue matrix. The lenth of the array must be order * order.</param>
        public void EigenDecomp(bool isSymmetric, int order, IStorageBM matrix, IStorageBM matrixEv, Complex[] vectorEv, IStorageBM matrixD)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            if (matrix.Length != (long)order *order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, (long)order *order), "matrix");
            }

            if (matrixEv == null)
            {
                throw new ArgumentNullException("matrixEv");
            }

            if (matrixEv.Length != (long)order *order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, (long)order *order), "matrixEv");
            }

            if (vectorEv == null)
            {
                throw new ArgumentNullException("vectorEv");
            }

            if (vectorEv.Length != order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order), "vectorEv");
            }

            if (matrixD == null)
            {
                throw new ArgumentNullException("matrixD");
            }

            if (matrixD.Length != (long)order *order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, (long)order *order), "matrixD");
            }

            var info = SafeNativeMethodsBM_Complex32.c_eigen(isSymmetric, order, matrix.Data, matrixEv.Data, vectorEv, matrixD.Data);

            if (info == (int)MklError.MemoryAllocation)
            {
                throw new MemoryAllocationException();
            }

            if (info < 0)
            {
                throw new InvalidParameterException(Math.Abs(info));
            }

            if (info > 0)
            {
                throw new NonConvergenceException();
            }
        }

        /// <summary>
        /// Solves A*X=B for X using a previously SVD decomposed matrix.
        /// </summary>
        /// <param name="rowsA">The number of rows in the A matrix.</param>
        /// <param name="columnsA">The number of columns in the A matrix.</param>
        /// <param name="s">The s values returned by <see cref="SingularValueDecomposition(bool,Complex32[],int,int,Complex32[],Complex32[],Complex32[])"/>.</param>
        /// <param name="u">The left singular vectors returned by  <see cref="SingularValueDecomposition(bool,Complex32[],int,int,Complex32[],Complex32[],Complex32[])"/>.</param>
        /// <param name="vt">The right singular  vectors returned by  <see cref="SingularValueDecomposition(bool,Complex32[],int,int,Complex32[],Complex32[],Complex32[])"/>.</param>
        /// <param name="b">The B matrix.</param>
        /// <param name="columnsB">The number of columns of B.</param>
        /// <param name="x">On exit, the solution matrix.</param>
        public void SvdSolveFactored(int rowsA, int columnsA, Complex32[] s, IStorageBM u, IStorageBM vt, Complex32[] b, int columnsB, Complex32[] x)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            if (vt == null)
            {
                throw new ArgumentNullException("vt");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (u.Length != (long)rowsA * rowsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "u");
            }

            if (vt.Length != (long)columnsA * columnsA)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "vt");
            }

            if (s.Length != Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "s");
            }

            if (b.Length != (long)rowsA * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != (long)columnsA * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            DataTableStorage.DataTableStorage_SvdSolveFactored_Complex32(rowsA, columnsA, s, u.Data, vt.Data, b, columnsB, x);
        }

        public long TriangularInverse(bool uplo, bool unitTriangular, long n, IStorageBM matrix)
        {
            return SafeNativeMethodsBM_Complex32.c_triangular_inverse(uplo, unitTriangular, n, matrix.Data);
        }

    }
}

#endif
