// <copyright file="DenseColumnMajorMatrixStorage.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Threading;
using Anemon;

namespace MathNet.Numerics.LinearAlgebra.Storage
{
    [Serializable]
    [DataContract(Namespace = "urn:MathNet/Numerics/LinearAlgebra")]
    public abstract class DenseColumnMajorMatrixStorageBM<T> : MatrixStorage<T>, IDisposable
        where T : struct, IEquatable<T>, IFormattable
    {
        // [ruegg] public fields are OK here

        [DataMember(Order = 1)]
        public IntPtr Data { get; private set; }
        public long Length { get; private set; }

        internal DenseColumnMajorMatrixStorageBM(int rows, int columns)
            : base(rows, columns)
        {
            Data = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount * sizeof(float));
            Length = (long)rows * (long)columns;
        }

        internal DenseColumnMajorMatrixStorageBM(int rows, int columns, IntPtr data)
            : base(rows, columns)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Data = data;
            Length = (long)rows * (long)columns;
        }
        public void Free()
        {
            if (Data != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(Data);
                Data = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        ~DenseColumnMajorMatrixStorageBM()
        {
            Free();
        }

        public static implicit operator IntPtr(DenseColumnMajorMatrixStorageBM<T> storage)
        {
            return storage.Data;
        }

        /// <summary>
        /// True if the matrix storage format is dense.
        /// </summary>
        public override bool IsDense
        {
            get { return true; }
        }

        /// <summary>
        /// True if all fields of this matrix can be set to any value.
        /// False if some fields are fixed, like on a diagonal matrix.
        /// </summary>
        public override bool IsFullyMutable
        {
            get { return true; }
        }

        /// <summary>
        /// True if the specified field can be set to any value.
        /// False if the field is fixed, like an off-diagonal field on a diagonal matrix.
        /// </summary>
        public override bool IsMutableAt(int row, int column)
        {
            return true;
        }

        /// <summary>
        /// Evaluate the row and column at a specific data index.
        /// </summary>
        void RowColumnAtIndex(int index, out int row, out int column)
        {
#if PORTABLE
            row = index % RowCount;
            column = index / RowCount;
#else
            column = Math.DivRem(index, RowCount, out row);
#endif
        }

    }
}
