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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Threading;
using Anemon;
using System.Diagnostics;
using System.Reflection;

namespace MathNet.Numerics.LinearAlgebra.Storage
{
    public interface IStorageBM
    {
        IntPtr Data { get; }
        long Length { get; }
    }

    [Serializable]
    [DataContract(Namespace = "urn:MathNet/Numerics/LinearAlgebra")]
    [DebuggerDisplay("Count = {Length}")]
    [DebuggerTypeProxy(typeof(DenseStorageViewer))]
    public class DenseColumnMajorMatrixStorageBM<T> : MatrixStorage<T>, IStorageBM, IDisposable
        where T : struct, IEquatable<T>, IFormattable
    {
        // [ruegg] public fields are OK here

        [DataMember(Order = 1)]
        public IntPtr Data { get; private set; }
        internal DataTableStorage<T> dataTableStorage = DataTableStorage<T>.CreateDataTableStorage();
        public long Length { get; private set; }

        public DenseColumnMajorMatrixStorageBM(int rows, int columns)
            : base(rows, columns)
        {
            T t = default(T);
            int sizeOfT = System.Runtime.InteropServices.Marshal.SizeOf(t);
            Length = (long)rows * (long)columns;
            Data = dataTableStorage.DataTableStorage_AllocByte(
                    //RowCount * ColumnCount * sizeof(t);
                    Length * sizeOfT);
            if (Data == IntPtr.Zero)
                throw new Exception("Out ofmemory in DenseColumnMajorMatrixStorageBM");
        }
        public DenseColumnMajorMatrixStorageBM(int rows, int columns, IntPtr data, bool frameOnly = false)
            : base(rows, columns)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Data = data;
            Length = (long)rows * (long)columns;
            this.frameOnly = frameOnly;
        }
        internal DenseColumnMajorMatrixStorageBM(int rows, int columns, T[] data)
            : base(rows, columns)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            T t = default(T);
            int sizeOfT = System.Runtime.InteropServices.Marshal.SizeOf(t);
            Length = (long)rows * (long)columns;
            Data = dataTableStorage.DataTableStorage_AllocByte(
                    Length * sizeOfT);
            if (Data == IntPtr.Zero)
                throw new Exception("Out ofmemory in DenseColumnMajorMatrixStorageBM");

            dataTableStorage.DataTableStorage_SetRow(Data, RowCount * ColumnCount, 0, data);
        }

        bool frameOnly = false;
        internal void Free()
        {
            if (Data != IntPtr.Zero && !frameOnly)
            {
                dataTableStorage.DataTableStorage_Free(Data);
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




        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        public override T At(int row, int column)
        {
            T t;
            dataTableStorage.DataTableStorage_GetElementAt(Data, RowCount, column, row, out t);
            return t;
        }
        /// <summary>
        /// Sets the element without range checking.
        /// </summary>
        public override void At(int row, int column, T value)
        {
            dataTableStorage.DataTableStorage_SetElementAt(Data, RowCount, column, row, value);
        }

        // CLEARING

        public override void Clear()
        {
            dataTableStorage.DataTableStorage_Clear(Data, 0, Length, dataTableStorage.Zero);
            //Array.Clear(Data, 0, Length);
        }

        internal override void ClearUnchecked(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            if (rowIndex == 0 && columnIndex == 0 && rowCount == RowCount && columnCount == ColumnCount)
            {
                dataTableStorage.DataTableStorage_Clear(Data, 0, Length, dataTableStorage.Zero);
                //Array.Clear(Data, 0, Length);
                return;
            }

            for (int j = columnIndex; j < columnIndex + columnCount; j++)
            {
                dataTableStorage.DataTableStorage_Clear(Data, j * RowCount + rowIndex, rowCount, dataTableStorage.Zero);
                //Array.Clear(Data, j*RowCount + rowIndex, rowCount);
            }
        }

        internal override void ClearRowsUnchecked(int[] rowIndices)
        {
            for (var k = 0; k < rowIndices.Length; k++)
                dataTableStorage.DataTableStorage_ClearColumn(Data, RowCount, ColumnCount, rowIndices[k], dataTableStorage.Zero);
        }

        internal override void ClearColumnsUnchecked(int[] columnIndices)
        {
            for (int k = 0; k < columnIndices.Length; k++)
                dataTableStorage.DataTableStorage_ClearRow(Data, RowCount, columnIndices[k], dataTableStorage.Zero);
        }

        // INITIALIZATION

        public static DenseColumnMajorMatrixStorageBM<T> OfMatrix(MatrixStorage<T> matrix)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyToUnchecked(storage, ExistingData.AssumeZeros);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfValue(int rows, int columns, T value)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var data = storage.Data;
            CommonParallel.For(0, storage.Length, 4096, (a, b) =>
            {
                storage.dataTableStorage.DataTableStorage_Clear(storage.Data, a, b - a, value);
            });
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfInit(int rows, int columns, Func<int, int, T> init)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            for (var j = 0; j < columns; j++)
            {
                for (var i = 0; i < rows; i++)
                {
                    storage.At(i, j, init(i, j));
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfDiagonalInit(int rows, int columns, Func<int, T> init)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            for (var i = 0; i < Math.Min(rows, columns); i++)
            {
                storage.At(i, i, init(i));
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfArray(T[,] array)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(array.GetLength(0), array.GetLength(1));
            for (var j = 0; j < storage.ColumnCount; j++)
            {
                for (var i = 0; i < storage.RowCount; i++)
                {
                    storage.At(i, j, array[i, j]);
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfColumnArrays(T[][] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int columns = data.Length;
            int rows = data[0].Length;

            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            for (int j = 0; j < data.Length; j++)
                storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows, j, data[j]);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfRowArrays(T[][] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int rows = data.Length;
            int columns = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            for (int i = 0; i < rows; i++)
                storage.dataTableStorage.DataTableStorage_SetColumn(storage.Data, columns, rows, i, data[i]);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfColumnMajorArray(int rows, int columns, T[] data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows * columns, 0, data);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfRowMajorArray(int rows, int columns, T[] data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var v = new T[columns];
            for (int i = 0; i < rows; i++)
            {
                int off = i * columns;
                for (int j = 0; j < columns; j++)
                {
                    v[j] = data[off + j];
                }
                storage.dataTableStorage.DataTableStorage_SetColumn(storage.Data, columns, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfColumnVectors(VectorStorage<T>[] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int columns = data.Length;
            int rows = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);

            for (int j = 0; j < data.Length; j++)
            {
                var column = data[j];
                var denseColumn = column as DenseVectorStorage<T>;
                if (denseColumn != null)
                {
                    storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows, j, denseColumn.Data);
                }
                else
                {
                    // FALL BACK
                    int offset = j * rows;
                    for (int i = 0; i < rows; i++)
                    {
                        storage.At(i, j, column.At(i));
                    }
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfRowVectors(VectorStorage<T>[] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }
            int rows = data.Length;
            int columns = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                if (data[i] is DenseVectorStorage<T>)
                    storage.dataTableStorage.DataTableStorage_SetColumn(storage.Data, columns, rows, i, (data[i] as DenseVectorStorage<T>).Data);
                else
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        storage.dataTableStorage.DataTableStorage_SetElementAt(storage.Data, rows, j, i, data[i][j]);
                    }
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfIndexedEnumerable(int rows, int columns, IEnumerable<Tuple<int, int, T>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            foreach (var item in data)
            {
                storage.dataTableStorage.DataTableStorage_SetElementAt(storage.Data, rows, item.Item2, item.Item1, item.Item3);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfColumnMajorEnumerable(int rows, int columns, IEnumerable<T> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var v = new T[rows];
            var e = data.GetEnumerator();
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    e.MoveNext();
                    v[j] = e.Current;
                }
                storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfRowMajorEnumerable(int rows, int columns, IEnumerable<T> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var v = new T[columns];
            var e = data.GetEnumerator();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!e.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows * columns));
                    v[j] = e.Current;
                }
                storage.dataTableStorage.DataTableStorage_SetColumn(storage.Data, columns, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfColumnEnumerables(int rows, int columns, IEnumerable<IEnumerable<T>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var v = new T[rows];
            using (var columnIterator = data.GetEnumerator())
            {
                for (int column = 0; column < columns; column++)
                {
                    if (!columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
                    var arrayColumn = columnIterator.Current as T[];
                    if (arrayColumn != null)
                    {
                        storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows, column, arrayColumn);
                    }
                    else
                    {
                        using (var rowIterator = columnIterator.Current.GetEnumerator())
                        {
                            for (int index = 0; index < rows; index++)
                            {
                                if (!rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
                                v[index] = rowIterator.Current;
                            }
                            storage.dataTableStorage.DataTableStorage_SetRow(storage.Data, rows, column, arrayColumn);
                            if (rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
                        }
                    }
                }
                if (columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM<T> OfRowEnumerables(int rows, int columns, IEnumerable<IEnumerable<T>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM<T>(rows, columns);
            var v = new T[columns];
            using (var rowIterator = data.GetEnumerator())
            {
                for (int row = 0; row < rows; row++)
                {
                    if (!rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
                    using (var columnIterator = rowIterator.Current.GetEnumerator())
                    {
                        for (int index = 0; index < columns; index++)
                        {
                            if (!columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
                            v[index] = columnIterator.Current;
                        }
                        if (columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
                    }
                    storage.dataTableStorage.DataTableStorage_SetColumn(storage.Data, columns, rows, row, v);
                }
                if (rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
            }
            return storage;
        }

        // MATRIX COPY

        internal override void CopyToUnchecked(MatrixStorage<T> target, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM<T>;
            if (denseTarget != null)
            {
                CopyToUnchecked(denseTarget);
                return;
            }

            // FALL BACK

            for (int j = 0, offset = 0; j < ColumnCount; j++, offset += RowCount)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    target.At(i, j, At(i, j));
                }
            }
        }

        void CopyToUnchecked(DenseColumnMajorMatrixStorageBM<T> target)
        {
            dataTableStorage.DataTableStorage_SetRow(target.Data, (long)RowCount * ColumnCount, 0, Data);
        }

        internal override void CopySubMatrixToUnchecked(MatrixStorage<T> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM<T>;
            if (denseTarget != null)
            {
                CopySubMatrixToUnchecked(denseTarget, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount);
                return;
            }

            // TODO: Proper Sparse Implementation

            // FALL BACK

            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                int index = sourceRowIndex + j * RowCount;
                for (int ii = targetRowIndex; ii < targetRowIndex + rowCount; ii++)
                {
                    target.At(ii, jj, At(index, j));
                }
            }
        }

        void CopySubMatrixToUnchecked(DenseColumnMajorMatrixStorageBM<T> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount)
        {
            var v = new T[rowCount];
            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                //Array.Copy(Data, j * RowCount + sourceRowIndex, target.Data, jj * target.RowCount + targetRowIndex, rowCount);
                dataTableStorage.DataTableStorage_GetSubRow(Data, RowCount, j, sourceRowIndex, rowCount, v);
                dataTableStorage.DataTableStorage_SetSubRow(target.Data, target.RowCount, jj, targetRowIndex, rowCount, v);
            }
        }


        // ROW COPY

        internal override void CopySubRowToUnchecked(VectorStorage<T> target, int rowIndex, int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData)
        {
            var targetDense = target as DenseVectorStorage<T>;
            if (targetDense != null)
            {
                var v = new T[columnCount];
                dataTableStorage.DataTableStorage_GetSubColumn(Data, ColumnCount, RowCount, rowIndex, sourceColumnIndex, columnCount, v);
                Array.Copy(v, 0, targetDense.Data, targetColumnIndex, columnCount);
                return;
            }

            // FALL BACK

            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                target.At(jj, At(j * RowCount, rowIndex));
            }
        }

        // COLUMN COPY

        internal override void CopySubColumnToUnchecked(VectorStorage<T> target, int columnIndex, int sourceRowIndex, int targetRowIndex, int rowCount,
            ExistingData existingData)
        {
            var targetDense = target as DenseVectorStorage<T>;
            if (targetDense != null)
            {
                var v = new T[rowCount];
                dataTableStorage.DataTableStorage_GetSubRow(Data, RowCount, columnIndex, sourceRowIndex, rowCount, v);
                Array.Copy(v, 0, targetDense.Data, targetRowIndex, rowCount);
                return;
            }

            // FALL BACK

            var offset = columnIndex * RowCount;
            for (int i = sourceRowIndex, ii = targetRowIndex; i < sourceRowIndex + rowCount; i++, ii++)
            {
                target.At(ii, At(i, columnIndex));
            }
        }

        // TRANSPOSE

        internal override void TransposeToUnchecked(MatrixStorage<T> target, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM<T>;
            if (denseTarget != null)
            {
                TransposeToUnchecked(denseTarget);
                return;
            }

            var sparseTarget = target as SparseCompressedRowMatrixStorage<T>;
            if (sparseTarget != null)
            {
                TransposeToUnchecked(sparseTarget);
                return;
            }

            // FALL BACK

            for (int j = 0, offset = 0; j < ColumnCount; j++, offset += RowCount)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    target.At(j, i, At(i, j));
                }
            }
        }

        void TransposeToUnchecked(DenseColumnMajorMatrixStorageBM<T> target)
        {
            var v = new T[RowCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                dataTableStorage.DataTableStorage_GetColumn(Data, ColumnCount, RowCount, j, v);
                dataTableStorage.DataTableStorage_SetRow(target.Data, target.RowCount, j, v);
            }
        }

        void TransposeToUnchecked(SparseCompressedRowMatrixStorage<T> target)
        {
            var rowPointers = target.RowPointers;
            var columnIndices = new List<int>();
            var values = new List<T>();
            var v = new T[RowCount];

            for (int j = 0; j < ColumnCount; j++)
            {
                dataTableStorage.DataTableStorage_GetRow(Data, RowCount, j, v);
                rowPointers[j] = values.Count;
                var index = j * RowCount;
                for (int i = 0; i < RowCount; i++)
                {
                    if (!Zero.Equals(v[i]))
                    {
                        values.Add(v[i]);
                        columnIndices.Add(i);
                    }
                }
            }

            rowPointers[ColumnCount] = values.Count;
            target.ColumnIndices = columnIndices.ToArray();
            target.Values = values.ToArray();
        }

        internal override void TransposeSquareInplaceUnchecked()
        {
            var v = new T[RowCount];
            var h = new T[RowCount];

            for (var j = 0; j < ColumnCount; j++)
            {
                dataTableStorage.DataTableStorage_GetSubRow(Data, ColumnCount, j, j, ColumnCount - j, v);
                dataTableStorage.DataTableStorage_GetSubColumn(Data, ColumnCount, ColumnCount, j, j, ColumnCount - j, h);

                dataTableStorage.DataTableStorage_SetSubColumn(Data, ColumnCount, ColumnCount, j, j, ColumnCount - j, v);
                dataTableStorage.DataTableStorage_SetSubRow(Data, ColumnCount, j, j, ColumnCount - j, h);
            }
        }

        // EXTRACT

        public override T[] ToRowMajorArray()
        {
            var ret = new T[Length];
            var v = new T[ColumnCount];
            int p = 0;
            for (int i = 0; i < RowCount; i++)
            {
                dataTableStorage.DataTableStorage_GetColumn(Data, ColumnCount, RowCount, i, v);
                Array.Copy(v, 0, ret, p, ColumnCount);
                p += ColumnCount;
            }
            return ret;
        }

        public override T[] ToColumnMajorArray()
        {
            var ret = new T[Length];
            dataTableStorage.DataTableStorage_GetRow(Data, RowCount * ColumnCount, 0, ret);
            return ret;
        }

        public override T[][] ToRowArrays()
        {
            var ret = new T[RowCount][];
            CommonParallel.For(0, RowCount, Math.Max(4096 / ColumnCount, 32), (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    ret[i] = new T[ColumnCount];
                    dataTableStorage.DataTableStorage_GetColumn(Data, ColumnCount, RowCount, i, ret[i]);
                }
            });
            return ret;
        }

        public override T[][] ToColumnArrays()
        {
            var ret = new T[ColumnCount][];
            CommonParallel.For(0, ColumnCount, Math.Max(4096 / RowCount, 32), (a, b) =>
            {
                for (int j = a; j < b; j++)
                {
                    ret[j] = new T[RowCount];
                    dataTableStorage.DataTableStorage_GetRow(Data, RowCount, j, ret[j]);
                }
            });
            return ret;
        }

        public override T[,] ToArray()
        {
            var ret = new T[RowCount, ColumnCount];
            var v = new T[ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                dataTableStorage.DataTableStorage_GetColumn(Data, ColumnCount, RowCount, i, v);
                for (int j = 0; j < ColumnCount; j++)
                {
                    ret[i, j] = v[j];
                }
            }
            return ret;
        }

        public override T[] AsColumnMajorArray()
        {
            var ret = new T[RowCount * ColumnCount];
            var v = new T[RowCount];
            int p = 0;
            for (int i = 0; i < ColumnCount; i++)
            {
                dataTableStorage.DataTableStorage_GetRow(Data, RowCount, i, v);
                Array.Copy(v, 0, ret, p, RowCount);
                p += RowCount;
            }
            return ret;
        }

        // ENUMERATION

        public override IEnumerable<T> Enumerate()
        {
            var v = new T[RowCount];
            for (int i = 0; i < ColumnCount; i++)
            {
                dataTableStorage.DataTableStorage_GetRow(Data, RowCount, i, v);
                for (int j = 0; j < ColumnCount; j++)
                {
                    yield return v[j];
                }
            }
        }

        public override IEnumerable<Tuple<int, int, T>> EnumerateIndexed()
        {
            int index = 0;
            var v = new T[RowCount];
            for (int j = 0; j < ColumnCount; j++)
            {
                dataTableStorage.DataTableStorage_GetRow(Data, RowCount, j, v);
                for (int i = 0; i < RowCount; i++)
                {
                    yield return new Tuple<int, int, T>(i, j, v[i]);
                    index++;
                }
            }
        }

        public override IEnumerable<T> EnumerateNonZero()
        {
            return Enumerate().Where(x => !Zero.Equals(x));
        }

        public override IEnumerable<Tuple<int, int, T>> EnumerateNonZeroIndexed()
        {
            int index = 0;
            var v = new T[RowCount];
            for (int j = 0; j < ColumnCount; j++)
            {
                dataTableStorage.DataTableStorage_GetRow(Data, RowCount, j, v);
                for (int i = 0; i < RowCount; i++)
                {
                    var x = v[i];
                    if (!Zero.Equals(x))
                    {
                        yield return new Tuple<int, int, T>(i, j, x);
                    }
                    index++;
                }
            }
        }

        // FIND

        public override Tuple<int, int, T> Find(Func<T, bool> predicate, Zeros zeros)
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    T f = At(i, j);
                    if (predicate(f))
                    {
                        return new Tuple<int, int, T>(i, j, f);
                    }

                }
            }
            return null;
        }

        internal override Tuple<int, int, T, TOther> Find2Unchecked<TOther>(MatrixStorage<TOther> other, Func<T, TOther, bool> predicate, Zeros zeros)
        {
            //throw new Exception("Find2Unchecked not yet implemented");

            var denseOther = other as DenseColumnMajorMatrixStorageBM<TOther>;
            if (denseOther != null)
            {
                for (int i = 0; i < RowCount; i++)
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        if (predicate(At(i,j), denseOther.At(i,j)))
                    {
                        return new Tuple<int, int, T, TOther>(i, j, At(i, j), denseOther.At(i, j));

                    }
                }
                return null;
            }

            var denseOther2 = other as DenseColumnMajorMatrixStorage<TOther>;
            if (denseOther2 != null)
            {
                for (int i = 0; i < RowCount; i++)
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        if (predicate(At(i, j), denseOther2.At(i, j)))
                        {
                            return new Tuple<int, int, T, TOther>(i, j, At(i, j), denseOther2.At(i, j));

                        }
                    }
                return null;
            }            

            var diagonalOther = other as DiagonalMatrixStorage<TOther>;
            if (diagonalOther != null)
            {
                TOther[] otherData = diagonalOther.Data;
                TOther otherZero = BuilderInstance<TOther>.Matrix.Zero;
                for (int j = 0; j < ColumnCount; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        if (predicate(At(i,j), i == j ? otherData[i] : otherZero))
                        {
                            return new Tuple<int, int, T, TOther>(i, j, At(i,j), i == j ? otherData[i] : otherZero);
                        }
                    }
                }
                return null;
            }

            var sparseOther = other as SparseCompressedRowMatrixStorage<TOther>;
            if (sparseOther != null)
            {
                int[] otherRowPointers = sparseOther.RowPointers;
                int[] otherColumnIndices = sparseOther.ColumnIndices;
                TOther[] otherValues = sparseOther.Values;
                TOther otherZero = BuilderInstance<TOther>.Matrix.Zero;
                int k = 0;
                for (int row = 0; row < RowCount; row++)
                {
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        if (k < otherRowPointers[row + 1] && otherColumnIndices[k] == col)
                        {
                            if (predicate(At(row, col), otherValues[k]))
                            {
                                return new Tuple<int, int, T, TOther>(row, col, At(row, col), otherValues[k]);
                            }
                            k++;
                        }
                        else
                        {
                            if (predicate(At(row, col), otherZero))
                            {
                                return new Tuple<int, int, T, TOther>(row, col, At(row, col), otherValues[k]);
                            }
                        }
                    }
                }
                return null;
            }

            // FALL BACK

            return base.Find2Unchecked(other, predicate, zeros);
        }

        // FUNCTIONAL COMBINATORS: MAP

        public override void MapInplace(Func<T, T> f, Zeros zeros = Zeros.AllowSkip)
        {
            CommonParallel.For(0, ColumnCount, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    for (int j = 0; j < RowCount; j++)
                    {
                        At(j, i, f(At(j, i)));
                    }
                }
            });
        }

        public override void MapIndexedInplace(Func<int, int, T, T> f, Zeros zeros = Zeros.AllowSkip)
        {
            CommonParallel.For(0, ColumnCount, Math.Max(4096 / RowCount, 32), (a, b) =>
            {
                int index = a * RowCount;
                for (int j = a; j < b; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        At(i, j, f(i, j, At(i, j)));
                        index++;
                    }
                }
            });
        }

        internal override void MapToUnchecked<TU>(MatrixStorage<TU> target, Func<T, TU> f,
            Zeros zeros, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorage<TU>;
            if (denseTarget != null)
            {
                CommonParallel.For(0, ColumnCount, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            denseTarget.At(j, i, f(At(j, i)));
                        }
                    }
                });
                return;
            }

            // FALL BACK

            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    target.At(i, j, f(At(i, j)));
                }
            }
        }

        internal override void MapIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
            Zeros zeros, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorage<TU>;
            if (denseTarget != null)
            {
                CommonParallel.For(0, ColumnCount, Math.Max(4096 / RowCount, 32), (a, b) =>
                {
                    for (int j = a; j < b; j++)
                    {
                        for (int i = 0; i < RowCount; i++)
                        {
                            denseTarget.At(i, j, f(i, j, At(i, j)));
                        }
                    }
                });
                return;
            }

            // FALL BACK

            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    target.At(i, j, f(i, j, At(i, j)));
                }
            }
        }

        internal override void MapSubMatrixIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            Zeros zeros, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorage<TU>;
            if (denseTarget != null)
            {
                CommonParallel.For(0, columnCount, Math.Max(4096 / rowCount, 32), (a, b) =>
                {
                    for (int j = a; j < b; j++)
                    {
                        int sourceIndex = sourceRowIndex + (j + sourceColumnIndex) * RowCount;
                        int targetIndex = targetRowIndex + (j + targetColumnIndex) * target.RowCount;
                        for (int i = 0; i < rowCount; i++)
                        {
                            denseTarget.Data[targetIndex++] = f(targetRowIndex + i, targetColumnIndex + j,
                                At(i + sourceRowIndex, j + sourceColumnIndex));
                        }
                    }
                });
                return;
            }

            // TODO: Proper Sparse Implementation

            // FALL BACK

            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                int index = sourceRowIndex + j * RowCount;
                for (int ii = targetRowIndex; ii < targetRowIndex + rowCount; ii++)
                {
                    target.At(ii, jj, f(ii, jj, At(ii, jj)));
                }
            }
        }


        // FUNCTIONAL COMBINATORS: FOLD

        internal override void FoldByRowUnchecked<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
        {
            for (int i = 0; i < RowCount; i++)
            {
                TU s = state[i];
                for (int j = 0; j < ColumnCount; j++)
                {
                    s = f(s, At(i, j));
                }
                target[i] = finalize(s, ColumnCount);
            }
        }

        internal override void FoldByColumnUnchecked<TU>(TU[] target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                int offset = j * RowCount;
                TU s = state[j];
                for (int i = 0; i < RowCount; i++)
                {
                    s = f(s, At(i, j));
                }
                target[j] = finalize(s, RowCount);
            }
        }

        internal override TState Fold2Unchecked<TOther, TState>(MatrixStorage<TOther> other, Func<TState, T, TOther, TState> f, TState state, Zeros zeros)
        {
            var denseOther = other as DenseColumnMajorMatrixStorage<TOther>;
            if (denseOther != null)
            {
                TOther[] otherData = denseOther.Data;
                for (int i = 0; i < ColumnCount; i++)
                {
                    for (int j = 0; j < RowCount; j++)
                    {
                        state = f(state, At(i, j), other.At(i, j));
                    }
                }
                return state;
            }

            var diagonalOther = other as DiagonalMatrixStorage<TOther>;
            if (diagonalOther != null)
            {
                TOther[] otherData = diagonalOther.Data;
                TOther otherZero = BuilderInstance<TOther>.Matrix.Zero;
                int k = 0;
                for (int j = 0; j < ColumnCount; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        state = f(state, At(j, i), i == j ? otherData[i] : otherZero);
                        k++;
                    }
                }
                return state;
            }

            var sparseOther = other as SparseCompressedRowMatrixStorage<TOther>;
            if (sparseOther != null)
            {
                int[] otherRowPointers = sparseOther.RowPointers;
                int[] otherColumnIndices = sparseOther.ColumnIndices;
                TOther[] otherValues = sparseOther.Values;
                TOther otherZero = BuilderInstance<TOther>.Matrix.Zero;
                int k = 0;
                for (int row = 0; row < RowCount; row++)
                {
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        if (k < otherRowPointers[row + 1] && otherColumnIndices[k] == col)
                        {
                            state = f(state, At(row, col), otherValues[k++]);
                        }
                        else
                        {
                            state = f(state, At(row, col), otherZero);
                        }
                    }
                }
                return state;
            }

            // FALL BACK

            return Fold2Unchecked(other, f, state, zeros);
        }

    }


    internal class DenseStorageViewer
    {
        object storageForView;

        public DenseStorageViewer(object _storage)
        {
            storageForView = _storage;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IEnumerable Values
        {
            get
            {
                DenseColumnMajorMatrixStorageBM<float> storageF = 
                    storageForView as DenseColumnMajorMatrixStorageBM<float>;
                if (storageF != null)
                    return GetValues(storageF);
                DenseColumnMajorMatrixStorageBM<double> storageD = 
                    storageForView as DenseColumnMajorMatrixStorageBM<double>;
                if (storageD != null)
                    return GetValues(storageD);
                DenseColumnMajorMatrixStorageBM<System.Numerics.Complex> storageC = 
                    storageForView as DenseColumnMajorMatrixStorageBM<System.Numerics.Complex>;
                if (storageC != null)
                    return GetValues(storageC);
                DenseColumnMajorMatrixStorageBM<MathNet.Numerics.Complex32> storageC32 = 
                    storageForView as DenseColumnMajorMatrixStorageBM<MathNet.Numerics.Complex32>;
                if (storageC32 != null)
                    return GetValues(storageC32);

                return null;
            }
        }

        IEnumerable GetValues<T>(DenseColumnMajorMatrixStorageBM<T> storage) where T : struct, IEquatable<T>, IFormattable
        {
            for (int i = 0; i < storage.ColumnCount; i++)
            {
                for (int j = 0; j < storage.RowCount; j++)
                {
                    //yield return new Tuple<int, int, T>(j, i, storage.At(j, i));
                    yield return storage.At(j, i);
                }
            }
        }
    }
}
