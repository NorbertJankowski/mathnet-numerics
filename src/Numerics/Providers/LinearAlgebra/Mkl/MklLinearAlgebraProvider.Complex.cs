﻿// <copyright file="MklLinearAlgebraProvider.Complex.cs" company="Math.NET">
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

namespace MathNet.Numerics.Providers.LinearAlgebra.Mkl
{
    /// <summary>
    /// Intel's Math Kernel Library (MKL) linear algebra provider.
    /// </summary>
    public partial class MklLinearAlgebraProvider
    {
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
        public override double MatrixNorm(Norm norm, int rows, int columns, Complex[] matrix)
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

            if (matrix.Length < rows * columns)
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, rows * columns), "matrix");
            }

            return SafeNativeMethods.z_matrix_norm((byte)norm, rows, columns, matrix);
        }

        /// <summary>
        /// Computes the dot product of x and y.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <param name="y">The vector y.</param>
        /// <returns>The dot product of x and y.</returns>
        /// <remarks>This is equivalent to the DOT BLAS routine.</remarks>
        [SecuritySafeCritical]
        public override Complex DotProduct(Complex[] x, Complex[] y)
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

            return SafeNativeMethods.z_dot_product(x.Length, x, y);
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
        public override void AddVectorToScaledVector(Complex[] y, Complex alpha, Complex[] x, Complex[] result)
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
                Array.Copy(y, 0, result, 0, y.Length);
            }

            if (alpha == Complex.Zero)
            {
                return;
            }

            SafeNativeMethods.z_axpy(y.Length, alpha, x, result);
        }

        /// <summary>
        /// Scales an array. Can be used to scale a vector and a matrix.
        /// </summary>
        /// <param name="alpha">The scalar.</param>
        /// <param name="x">The values to scale.</param>
        /// <param name="result">This result of the scaling.</param>
        /// <remarks>This is similar to the SCAL BLAS routine.</remarks>
        [SecuritySafeCritical]
        public override void ScaleArray(Complex alpha, Complex[] x, Complex[] result)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (!ReferenceEquals(x, result))
            {
                Array.Copy(x, 0, result, 0, x.Length);
            }

            if (alpha == Complex.One)
            {
                return;
            }

            SafeNativeMethods.z_scale(x.Length, alpha, result);
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
        /// set to Complex.One and beta set to Complex.Zero, and x and y are not transposed.</remarks>
        public override void MatrixMultiply(Complex[] x, int rowsX, int columnsX, Complex[] y, int rowsY, int columnsY, Complex[] result)
        {
            MatrixMultiplyWithUpdate(Transpose.DontTranspose, Transpose.DontTranspose, Complex.One, x, rowsX, columnsX, y, rowsY, columnsY, Complex.Zero, result);
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
        public override void MatrixMultiplyWithUpdate(Transpose transposeA, Transpose transposeB, Complex alpha, Complex[] a, int rowsA, int columnsA, Complex[] b, int rowsB, int columnsB, Complex beta, Complex[] c)
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

            if (c.Length != m*n)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            if (k != l)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            SafeNativeMethods.z_matrix_multiply(transposeA, transposeB, m, n, k, alpha, a, b, beta, c);
        }

        /// <summary>
        /// Computes the LUP factorization of A. P*A = L*U.
        /// </summary>
        /// <param name="data">An <paramref name="order"/> by <paramref name="order"/> matrix. The matrix is overwritten with the
        /// the LU factorization on exit. The lower triangular factor L is stored in under the diagonal of <paramref name="data"/> (the diagonal is always Complex.One
        /// for the L factor). The upper triangular factor U is stored on and above the diagonal of <paramref name="data"/>.</param>
        /// <param name="order">The order of the square matrix <paramref name="data"/>.</param>
        /// <param name="ipiv">On exit, it contains the pivot indices. The size of the array must be <paramref name="order"/>.</param>
        /// <remarks>This is equivalent to the GETRF LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public override void LUFactor(Complex[] data, int order, long[] ipiv)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (data.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "data");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            var info = SafeNativeMethods.z_lu_factor(order, data, ipiv);

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
        public override void LUInverse(Complex[] a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            var info = SafeNativeMethods.z_lu_inverse(order, a);

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
        public override void LUInverseFactored(Complex[] a, int order, long[] ipiv)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            var info = SafeNativeMethods.z_lu_inverse_factored(order, a, ipiv);

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
        public override void LUSolve(int columnsOfB, Complex[] a, int order, Complex[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != columnsOfB*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethods.z_lu_solve(order, columnsOfB, a, b);

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
        public override void LUSolveFactored(int columnsOfB, Complex[] a, int order, long[] ipiv, Complex[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (ipiv == null)
            {
                throw new ArgumentNullException("ipiv");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (ipiv.Length != order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "ipiv");
            }

            if (b.Length != columnsOfB*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethods.z_lu_solve_factored(order, columnsOfB, a, ipiv, b);

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
        /// Computes the Cholesky factorization of A.
        /// </summary>
        /// <param name="a">On entry, a square, positive definite matrix. On exit, the matrix is overwritten with the
        /// the Cholesky factorization.</param>
        /// <param name="order">The number of rows or columns in the matrix.</param>
        /// <remarks>This is equivalent to the POTRF LAPACK routine.</remarks>
        [SecuritySafeCritical]
        public override void CholeskyFactor(Complex[] a, int order)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (order < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "order");
            }

            if (a.Length != order*order)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            var info = SafeNativeMethods.z_cholesky_factor(order, a);

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
        public override void CholeskySolve(Complex[] a, int orderA, Complex[] b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != orderA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethods.z_cholesky_solve(orderA, columnsB, a, b);

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
        public override void CholeskySolveFactored(Complex[] a, int orderA, Complex[] b, int columnsB)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != orderA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (ReferenceEquals(a, b))
            {
                throw new ArgumentException(Resources.ArgumentReferenceDifferent);
            }

            var info = SafeNativeMethods.z_cholesky_solve_factored(orderA, columnsB, a, b);

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
        public override void QRFactor(Complex[] r, int rowsR, int columnsR, Complex[] q, Complex[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (r.Length != rowsR*columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "r");
            }

            if (tau.Length < Math.Min(rowsR, columnsR))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (q.Length != rowsR*rowsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * rowsR"), "q");
            }

            var info = SafeNativeMethods.z_qr_factor(rowsR, columnsR, r, tau, q);

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
        public override void ThinQRFactor(Complex[] q, int rowsA, int columnsA, Complex[] r, Complex[] tau)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            if (q == null)
            {
                throw new ArgumentNullException("q");
            }

            if (q.Length != rowsA * columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "rowsR * columnsR"), "q");
            }

            if (tau.Length < Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(string.Format(Resources.ArrayTooSmall, "min(m,n)"), "tau");
            }

            if (r.Length != columnsA * columnsA)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, "columnsA * columnsA"), "r");
            }

            var info = SafeNativeMethods.z_qr_thin_factor(rowsA, columnsA, q, tau, r);

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
        public override void QRSolve(Complex[] a, int rows, int columns, Complex[] b, int columnsB, Complex[] x, QRMethod method = QRMethod.Full)
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

            if (a.Length != rows * columns)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "a");
            }

            if (b.Length != rows * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != columns * columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "x");
            }

            if (rows < columns)
            {
                throw new ArgumentException(Resources.RowsLessThanColumns);
            }

            var info = SafeNativeMethods.z_qr_solve(rows, columns, columnsB, a, b, x);

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
        /// <param name="q">The Q matrix obtained by calling <see cref="QRFactor(Complex[],int,int,Complex[],Complex[])"/>.</param>
        /// <param name="r">The R matrix obtained by calling <see cref="QRFactor(Complex[],int,int,Complex[],Complex[])"/>. </param>
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
        public override void QRSolveFactored(Complex[] q, Complex[] r, int rowsA, int columnsA, Complex[] tau, Complex[] b, int columnsB, Complex[] x, QRMethod method = QRMethod.Full)
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

            if (r.Length != rowsR * columnsR)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsR * columnsR), "r");
            }

            if (q.Length != rowsQ * columnsQ)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsQ * columnsQ), "q");
            }

            if (b.Length != rowsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, rowsA * columnsB), "b");
            }

            if (x.Length != columnsA * columnsB)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentArrayWrongLength, columnsA * columnsB), "x");
            }

            if (method == QRMethod.Full)
            {
                var info = SafeNativeMethods.z_qr_solve_factored(rowsA, columnsA, columnsB, r, b, tau, x);

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
                base.QRSolveFactored(q, r, rowsA, columnsA, tau, b, columnsB, x, QRMethod.Thin);
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
        public override void SvdSolve(Complex[] a, int rowsA, int columnsA, Complex[] b, int columnsB, Complex[] x)
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

            if (b.Length != rowsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            if (x.Length != columnsA*columnsB)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "b");
            }

            var s = new Complex[Math.Min(rowsA, columnsA)];
            var u = new Complex[rowsA*rowsA];
            var vt = new Complex[columnsA*columnsA];

            var clone = new Complex[a.Length];
            a.Copy(clone);
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
        public override void SingularValueDecomposition(SVDVectorsComputation computeVectors, Complex[] a, int rowsA, int columnsA, Complex[] s, Complex[] u, Complex[] vt)
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
                    (u.Length != rowsA * rowsA) ||
                (computeVectors == SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (u.Length != rowsA * nm))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "u");
            }

            if ((computeVectors != SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (vt.Length != columnsA * columnsA) ||
                (computeVectors == SVDVectorsComputation.SimplifiedVectorComputation) &&
                    (vt.Length != columnsA * nm))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "vt");
            }

            if (s.Length != Math.Min(rowsA, columnsA))
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "s");
            }

            var info = SafeNativeMethods.z_svd_factor((char)computeVectors, rowsA, columnsA, a, s, u, vt);

            if (info == (int) MklError.MemoryAllocation)
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
        public override void AddArrays(Complex[] x, Complex[] y, Complex[] result)
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

            SafeNativeMethods.z_vector_add(x.Length, x, y, result);
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
        public override void SubtractArrays(Complex[] x, Complex[] y, Complex[] result)
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

            SafeNativeMethods.z_vector_subtract(x.Length, x, y, result);
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
        public override void PointWiseMultiplyArrays(Complex[] x, Complex[] y, Complex[] result)
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

            SafeNativeMethods.z_vector_multiply(x.Length, x, y, result);
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
        public override void PointWisePowerArrays(Complex[] x, Complex[] y, Complex[] result)
        {
            if (_vectorFunctionsMajor != 0 || _vectorFunctionsMinor < 1)
            {
                base.PointWisePowerArrays(x, y, result);
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

            SafeNativeMethods.z_vector_power(x.Length, x, y, result);
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
        public override void PointWiseDivideArrays(Complex[] x, Complex[] y, Complex[] result)
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

            SafeNativeMethods.z_vector_divide(x.Length, x, y, result);
        }

        /// <summary>
        /// Computes the eigenvalues and eigenvectors of a matrix.
        /// </summary>
        /// <param name="isSymmetric">Whether the matrix is symmetric or not.</param>
        /// <param name="order">The order of the matrix.</param>
        /// <param name="matrix">The matrix to decompose. The lenth of the array must be order * order.</param>
        /// <param name="matrixEv">On output, the matrix contains the eigen vectors. The lenth of the array must be order * order.</param>
        /// <param name="vectorEv">On output, the eigen values (λ) of matrix in ascending value. The length of the arry must <paramref name="order"/>.</param>
        /// <param name="matrixD">On output, the block diagonal eigenvalue matrix. The lenth of the array must be order * order.</param>
        public override void EigenDecomp(bool isSymmetric, int order, Complex[] matrix, Complex[] matrixEv, Complex[] vectorEv, Complex[] matrixD)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            if (matrix.Length != order * order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order * order), "matrix");
            }

            if (matrixEv == null)
            {
                throw new ArgumentNullException("matrixEv");
            }

            if (matrixEv.Length != order * order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order * order), "matrixEv");
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

            if (matrixD.Length != order * order)
            {
                throw new ArgumentException(String.Format(Resources.ArgumentArrayWrongLength, order * order), "matrixD");
            }

            var info = SafeNativeMethods.z_eigen(isSymmetric, order, matrix, matrixEv, vectorEv, matrixD);

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

        public override long TriangularInverse(bool uplo, bool unitTriangular, long n, Complex[] matrix)
        {
            return SafeNativeMethods.z_triangular_inverse(uplo, unitTriangular, n, matrix);
        }
    }
}

#endif
