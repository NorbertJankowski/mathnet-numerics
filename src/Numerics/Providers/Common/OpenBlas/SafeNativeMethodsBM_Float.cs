﻿// <copyright file="SafeNativeMethods.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// https://numerics.mathdotnet.com
//
// Copyright (c) 2009-2015 Math.NET
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

using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Numerics.Providers.LinearAlgebra.OpenBlas;
using System;

namespace MathNet.Numerics.Providers.Common.OpenBlas
{
    /// <summary>
    /// P/Invoke methods to the native math libraries.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal static class SafeNativeMethodsBM_Float
    {
        /// <summary>
        /// Name of the native DLL.
        /// </summary>
        const string _DllName = "MathNET.Numerics.OpenBLAS.dll";
        internal static string DllName { get { return _DllName; } }

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int query_capability(int capability);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern string get_build_config();

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern string get_cpu_core();

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ParallelType get_parallel_type();

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void set_max_threads(int num_threads);

        #region BLAS

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void s_axpy(long n, float alpha, IntPtr x, [In, Out] IntPtr y);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void s_scale(long n, float alpha, [Out] IntPtr x);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float s_dot_product(long n, IntPtr x, IntPtr y);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void s_matrix_multiply(Transpose transA, Transpose transB, long m, long n, long k, float alpha, IntPtr x, IntPtr y, float beta, [In, Out] IntPtr c);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void s_matrix_multiply(Transpose transA, Transpose transB, long m, long n, long k, float alpha, IntPtr x, float[] y, float beta, [In, Out] float[] c);

        #endregion BLAS

        #region LAPACK

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float s_matrix_norm(byte norm, long rows, long columns, [In] IntPtr a);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_cholesky_factor(long n, [In, Out] IntPtr a);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_factor(long n, [In, Out] IntPtr a, [In, Out] long[] ipiv);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_inverse(long n, [In, Out] IntPtr a);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_inverse_factored(long n, [In, Out] IntPtr a, [In, Out] long[] ipiv);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_solve_factored(long n, long nrhs, IntPtr a, [In, Out] long[] ipiv, [In, Out] IntPtr b);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_solve_factored(long n, long nrhs, IntPtr a, [In, Out] long[] ipiv, [In, Out] float[] b);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_lu_solve(long n, long nrhs, IntPtr a, [In, Out] IntPtr b);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_cholesky_solve(long n, long nrhs, IntPtr a, [In, Out] IntPtr b);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_cholesky_solve_factored(long n, long nrhs, IntPtr a, [In, Out] IntPtr b);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_cholesky_solve_factored(long n, long nrhs, IntPtr a, [In, Out] float[] b);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_qr_factor(long m, long n, [In, Out] IntPtr r, [In, Out] float[] tau, [In, Out] IntPtr q);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_qr_thin_factor(long m, long n, [In, Out] IntPtr q, [In, Out] float[] tau, [In, Out] IntPtr r);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_qr_solve(long m, long n, long bn, IntPtr r, float[] b, [In, Out] float[] x);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_qr_solve_factored(long m, long n, long bn, IntPtr r, IntPtr b, float[] tau, [In, Out] IntPtr x);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_qr_solve_factored(long m, long n, long bn, IntPtr r, float[] b, float[] tau, [In, Out] float[] x);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_svd_factor([MarshalAs(UnmanagedType.U1)] char computeVectors, long m, long n, [In, Out] IntPtr a, [In, Out] float[] s, [In, Out] IntPtr u, [In, Out] IntPtr v);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_eigen([MarshalAs(UnmanagedType.U1)] bool isSymmetric, long n, [In] IntPtr a, [In, Out] IntPtr vectors, [In, Out] Complex[] values, [In, Out] IntPtr d);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long s_triangular_inverse([MarshalAs(UnmanagedType.U1)] bool uplo, [MarshalAs(UnmanagedType.U1)] bool unitTriangular, long n, [In, Out] IntPtr a);

        #endregion LAPACK
    }
}

#endif