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


    public class DenseColumnMajorMatrixStorageBM_Float : DenseColumnMajorMatrixStorageBM<float>
    {
        internal DenseColumnMajorMatrixStorageBM_Float(int rows, int columns)
            : base(rows, columns) { }
        internal DenseColumnMajorMatrixStorageBM_Float(int rows, int columns, IntPtr data)
            : base(rows, columns, data) { }
        internal DenseColumnMajorMatrixStorageBM_Float(int rows, int columns, float[] data)
            : base(rows, columns)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            DataTableStorage.DataTableStorage_SetRow_Float(Data, RowCount * ColumnCount, 0, data);
        }

        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        public override float At(int row, int column)
        {
            return DataTableStorage.DataTableStorage_GetElementAt_Float(Data, RowCount, column, row);
        }
        /// <summary>
        /// Sets the element without range checking.
        /// </summary>
        public override void At(int row, int column, float value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Float(Data, RowCount, column, row, value);
        }

        // CLEARING

        public override void Clear()
        {
            DataTableStorage.DataTableStorage_Clear_Float(Data, 0, Length, 0);
            //Array.Clear(Data, 0, Length);
        }

        internal override void ClearUnchecked(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            if (rowIndex == 0 && columnIndex == 0 && rowCount == RowCount && columnCount == ColumnCount)
            {
                DataTableStorage.DataTableStorage_Clear_Float(Data, 0, Length, 0);
                //Array.Clear(Data, 0, Length);
                return;
            }

            for (int j = columnIndex; j < columnIndex + columnCount; j++)
            {
                DataTableStorage.DataTableStorage_Clear_Float(Data, j * RowCount + rowIndex, rowCount, 0);
                //Array.Clear(Data, j*RowCount + rowIndex, rowCount);
            }
        }

        internal override void ClearRowsUnchecked(int[] rowIndices)
        {
            for (var k = 0; k < rowIndices.Length; k++)
                DataTableStorage.DataTableStorage_ClearColumn_Float(Data, RowCount, ColumnCount, rowIndices[k], 0);
        }

        internal override void ClearColumnsUnchecked(int[] columnIndices)
        {
            for (int k = 0; k < columnIndices.Length; k++)
                DataTableStorage.DataTableStorage_ClearRow_Float(Data, RowCount, columnIndices[k], 0);
        }

        // INITIALIZATION

        public static DenseColumnMajorMatrixStorageBM_Float OfMatrix(MatrixStorage<float> matrix)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyToUnchecked(storage, ExistingData.AssumeZeros);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfValue(int rows, int columns, float value)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var data = storage.Data;
            CommonParallel.For(0, storage.Length, 4096, (a, b) =>
            {
                DataTableStorage.DataTableStorage_Clear_Float(storage.Data, a, b - a, value);
            });
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfInit(int rows, int columns, Func<int, int, float> init)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            for (var j = 0; j < columns; j++)
            {
                for (var i = 0; i < rows; i++)
                {
                    storage.At(i, j, init(i, j));
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfDiagonalInit(int rows, int columns, Func<int, float> init)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            for (var i = 0; i < Math.Min(rows, columns); i++)
            {
                storage.At(i,i, init(i));
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfArray(float[,] array)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(array.GetLength(0), array.GetLength(1));
            for (var j = 0; j < storage.ColumnCount; j++)
            {
                for (var i = 0; i < storage.RowCount; i++)
                {
                    storage.At(i,j, array[i, j]);
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfColumnArrays(float[][] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int columns = data.Length;
            int rows = data[0].Length;

            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            for (int j = 0; j < data.Length; j++)
                DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows, j, data[j]);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfRowArrays(float[][] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int rows = data.Length;
            int columns = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            for (int i = 0; i < rows; i++)
                DataTableStorage.DataTableStorage_SetColumn_Float(storage.Data, columns, rows, i, data[i]);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfColumnMajorArray(int rows, int columns, float[] data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows * columns, 0, data);
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfRowMajorArray(int rows, int columns, float[] data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var v = new float[columns];
            for (int i = 0; i < rows; i++)
            {
                int off = i * columns;
                for (int j = 0; j < columns; j++)
                {
                    v[j] = data[off + j];
                }
                DataTableStorage.DataTableStorage_SetColumn_Float(storage.Data, columns, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfColumnVectors(VectorStorage<float>[] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }

            int columns = data.Length;
            int rows = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);

            for (int j = 0; j < data.Length; j++)
            {
                var column = data[j];
                var denseColumn = column as DenseVectorStorage<float>;
                if (denseColumn != null)
                {
                    DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows, j, denseColumn.Data);
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

        public static DenseColumnMajorMatrixStorageBM_Float OfRowVectors(VectorStorage<float>[] data)
        {
            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("data", Resources.MatrixCanNotBeEmpty);
            }
            int rows = data.Length;
            int columns = data[0].Length;
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                if(data[i] is DenseVectorStorage<float>)
                DataTableStorage.DataTableStorage_SetColumn_Float(storage.Data, columns, rows, i, (data[i] as DenseVectorStorage<float>).Data);
                else
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        DataTableStorage.DataTableStorage_SetElementAt_Float(storage.Data, rows, j, i, data[i][j]);
                    }
                }
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfIndexedEnumerable(int rows, int columns, IEnumerable<Tuple<int, int, float>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            foreach (var item in data)
            {
                DataTableStorage.DataTableStorage_SetElementAt_Float(storage.Data, rows, item.Item2, item.Item1, item.Item3);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfColumnMajorEnumerable(int rows, int columns, IEnumerable<float> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var v = new float[rows];
            var e = data.GetEnumerator();
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    e.MoveNext();
                    v[j] = e.Current;
                }
                DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfRowMajorEnumerable(int rows, int columns, IEnumerable<float> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var v = new float[columns];
            var e = data.GetEnumerator();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!e.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows*columns));
                    v[j] = e.Current;
                }
                DataTableStorage.DataTableStorage_SetColumn_Float(storage.Data, columns, rows, i, v);
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfColumnEnumerables(int rows, int columns, IEnumerable<IEnumerable<float>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var v = new float[rows];
            using (var columnIterator = data.GetEnumerator())
            {
                for (int column = 0; column < columns; column++)
                {
                    if (!columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
                    var arrayColumn = columnIterator.Current as float[];
                    if (arrayColumn != null)
                    {
                        DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows, column, arrayColumn);
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
                            DataTableStorage.DataTableStorage_SetRow_Float(storage.Data, rows, column, arrayColumn);
                            if (rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
                        }
                    }
                }
                if (columnIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, columns));
            }
            return storage;
        }

        public static DenseColumnMajorMatrixStorageBM_Float OfRowEnumerables(int rows, int columns, IEnumerable<IEnumerable<float>> data)
        {
            var storage = new DenseColumnMajorMatrixStorageBM_Float(rows, columns);
            var v = new float[columns];
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
                    DataTableStorage.DataTableStorage_SetColumn_Float(storage.Data, columns, rows, row, v);
                }
                if (rowIterator.MoveNext()) throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, rows));
            }
            return storage;
        }

        // MATRIX COPY

        internal override void CopyToUnchecked(MatrixStorage<float> target, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM_Float;
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

        void CopyToUnchecked(DenseColumnMajorMatrixStorageBM_Float target)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                DataTableStorage.DataTableStorage_SetRow_Float(target.Data, RowCount, i, Data);
            }
            DataTableStorage.DataTableStorage_SetRow_Float(target.Data, (long)RowCount * ColumnCount, 0, Data);
        }

        internal override void CopySubMatrixToUnchecked(MatrixStorage<float> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM_Float;
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

        void CopySubMatrixToUnchecked(DenseColumnMajorMatrixStorageBM_Float target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount)
        {
            var v = new float[rowCount];
            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                //Array.Copy(Data, j * RowCount + sourceRowIndex, target.Data, jj * target.RowCount + targetRowIndex, rowCount);
                DataTableStorage.DataTableStorage_GetSubRow_Float(Data, RowCount, j, sourceRowIndex, rowCount, v);
                DataTableStorage.DataTableStorage_SetSubRow_Float(target.Data, target.RowCount, jj, targetRowIndex, rowCount, v);
            }
        }


        // ROW COPY

        internal override void CopySubRowToUnchecked(VectorStorage<float> target, int rowIndex, int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData)
        {
            var targetDense = target as DenseVectorStorage<float>;
            if (targetDense != null)
            {
                var v = new float[columnCount];
                DataTableStorage.DataTableStorage_GetSubColumn_Float(Data, ColumnCount, RowCount, rowIndex, sourceColumnIndex, columnCount, v);
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

        internal override void CopySubColumnToUnchecked(VectorStorage<float> target, int columnIndex, int sourceRowIndex, int targetRowIndex, int rowCount,
            ExistingData existingData)
        {
            var targetDense = target as DenseVectorStorage<float>;
            if (targetDense != null)
            {
                var v = new float[rowCount];
                DataTableStorage.DataTableStorage_GetSubRow_Float(Data, RowCount, columnIndex, sourceRowIndex, rowCount, v);
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

        internal override void TransposeToUnchecked(MatrixStorage<float> target, ExistingData existingData)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorageBM_Float;
            if (denseTarget != null)
            {
                TransposeToUnchecked(denseTarget);
                return;
            }

            var sparseTarget = target as SparseCompressedRowMatrixStorage<float>;
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
                    target.At(j, i, At(i,j));
                }
            }
        }

        void TransposeToUnchecked(DenseColumnMajorMatrixStorageBM_Float target)
        {
            var v = new float[RowCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetColumn_Float(Data, ColumnCount, RowCount, j, v);
                DataTableStorage.DataTableStorage_SetRow_Float(target.Data, target.RowCount, j, v);
            }
        }

        void TransposeToUnchecked(SparseCompressedRowMatrixStorage<float> target)
        {
            var rowPointers = target.RowPointers;
            var columnIndices = new List<int>();
            var values = new List<float>();
            var v = new float[RowCount];

            for (int j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, j, v);
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
            var v = new float[RowCount];
            var h = new float[RowCount];

            for (var j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetSubRow_Float(Data, ColumnCount, j, j, ColumnCount - j, v);
                DataTableStorage.DataTableStorage_GetSubColumn_Float(Data, ColumnCount, ColumnCount, j, j, ColumnCount - j, h);

                DataTableStorage.DataTableStorage_SetSubColumn_Float(Data, ColumnCount, ColumnCount, j, j, ColumnCount - j, v);
                DataTableStorage.DataTableStorage_SetSubRow_Float(Data, ColumnCount, j, j, ColumnCount - j, h);
            }
        }

        // EXTRACT

        public override float[] ToRowMajorArray()
        {
            var ret = new float[Length];
            var v = new float[ColumnCount];
            int p = 0;
            for (int i = 0; i < RowCount; i++)
            {
                DataTableStorage.DataTableStorage_GetColumn_Float(Data, ColumnCount, RowCount, i, v);
                Array.Copy(v, 0, ret, p, ColumnCount);
                p += ColumnCount;
            }
            return ret;
        }

        public override float[] ToColumnMajorArray()
        {
            var ret = new float[Length];
            DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount * ColumnCount, 0, ret);
            return ret;
        }

        public override float[][] ToRowArrays()
        {
            var ret = new float[RowCount][];
            CommonParallel.For(0, RowCount, Math.Max(4096 / ColumnCount, 32), (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    ret[i] = new float[ColumnCount];
                    DataTableStorage.DataTableStorage_GetColumn_Float(Data, ColumnCount, RowCount, i, ret[i]);
                }
            });
            return ret;
        }

        public override float[][] ToColumnArrays()
        {
            var ret = new float[ColumnCount][];
            CommonParallel.For(0, ColumnCount, Math.Max(4096 / RowCount, 32), (a, b) =>
            {
                for (int j = a; j < b; j++)
                {
                    ret[j] = new float[RowCount];
                    DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, j, ret[j]);
                }
            });
            return ret;
        }

        public override float[,] ToArray()
        {
            var ret = new float[RowCount, ColumnCount];
            var v = new float[ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                DataTableStorage.DataTableStorage_GetColumn_Float(Data, ColumnCount, RowCount, i, v);
                for (int j = 0; j < ColumnCount; j++)
                {
                    ret[i, j] = v[j];
                }
            }
            return ret;
        }

        public override float[] AsColumnMajorArray()
        {
            var ret = new float[RowCount * ColumnCount];
            var v = new float[RowCount];
            int p = 0;
            for (int i = 0; i < ColumnCount; i++)
            {
                DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, i, v);
                Array.Copy(v, 0, ret, p, RowCount);
                p += RowCount;
            }
            return ret;
        }

        // ENUMERATION

        public override IEnumerable<float> Enumerate()
        {
            var v = new float[RowCount];
            for (int i = 0; i < ColumnCount; i++)
            {
                DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, i, v);
                for (int j = 0; j < ColumnCount; j++)
                {
                    yield return v[j];
                }
            }
        }

        public override IEnumerable<Tuple<int, int, float>> EnumerateIndexed()
        {
            int index = 0;
            var v = new float[RowCount];
            for (int j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, j, v);
                for (int i = 0; i < RowCount; i++)
                {
                    yield return new Tuple<int, int, float>(i, j, v[i]);
                    index++;
                }
            }
        }

        public override IEnumerable<float> EnumerateNonZero()
        {
            return Enumerate().Where(x => !Zero.Equals(x));
        }

        public override IEnumerable<Tuple<int, int, float>> EnumerateNonZeroIndexed()
        {
            int index = 0;
            var v = new float[RowCount];
            for (int j = 0; j < ColumnCount; j++)
            {
                DataTableStorage.DataTableStorage_GetRow_Float(Data, RowCount, j, v);
                for (int i = 0; i < RowCount; i++)
                {
                    var x = v[i];
                    if (!Zero.Equals(x))
                    {
                        yield return new Tuple<int, int, float>(i, j, x);
                    }
                    index++;
                }
            }
        }

        // FIND

        public override Tuple<int, int, float> Find(Func<float, bool> predicate, Zeros zeros)
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    float f = At(i, j);
                    if (predicate(f))
                    {
                        return new Tuple<int, int, float>(i, j, f);
                    }

                }
            }
            return null;
        }

        internal override Tuple<int, int, float, TOther> Find2Unchecked<TOther>(MatrixStorage<TOther> other, Func<float, TOther, bool> predicate, Zeros zeros)
        {
            throw new Exception("Find2Unchecked not yet implemented");
/*            var denseOther = other as DenseColumnMajorMatrixStorage<TOther>;
            if (denseOther != null)
            {
                TOther[] otherData = denseOther.Data;
                for (int i = 0; i < Data.Length; i++)
                {
                    if (predicate(Data[i], otherData[i]))
                    {
                        int row, column;
                        RowColumnAtIndex(i, out row, out column);
                        return new Tuple<int, int, T, TOther>(row, column, Data[i], otherData[i]);

                    }
                }
                return null;
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
                        if (predicate(Data[k], i == j ? otherData[i] : otherZero))
                        {
                            return new Tuple<int, int, T, TOther>(i, j, Data[k], i == j ? otherData[i] : otherZero);
                        }
                        k++;
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
                            if (predicate(Data[col * RowCount + row], otherValues[k]))
                            {
                                return new Tuple<int, int, T, TOther>(row, col, Data[col * RowCount + row], otherValues[k]);
                            }
                            k++;
                        }
                        else
                        {
                            if (predicate(Data[col * RowCount + row], otherZero))
                            {
                                return new Tuple<int, int, T, TOther>(row, col, Data[col * RowCount + row], otherValues[k]);
                            }
                        }
                    }
                }
                return null;
            }

            // FALL BACK

            return base.Find2Unchecked(other, predicate, zeros);
            */
        }

        // FUNCTIONAL COMBINATORS: MAP

        public override void MapInplace(Func<float, float> f, Zeros zeros = Zeros.AllowSkip)
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

        public override void MapIndexedInplace(Func<int, int, float, float> f, Zeros zeros = Zeros.AllowSkip)
        {
            CommonParallel.For(0, ColumnCount, Math.Max(4096 / RowCount, 32), (a, b) =>
            {
                int index = a * RowCount;
                for (int j = a; j < b; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        At(i,j,  f(i, j, At(i,j)));
                        index++;
                    }
                }
            });
        }

        internal override void MapToUnchecked<TU>(MatrixStorage<TU> target, Func<float, TU> f,
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

            int index = 0;
            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    target.At(i, j, f(At(i, j)));
                }
            }
        }

        internal override void MapIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, float, TU> f,
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

        internal override void MapSubMatrixIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, float, TU> f,
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
                                At(i + sourceRowIndex, j+sourceColumnIndex));
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

        internal override void FoldByRowUnchecked<TU>(TU[] target, Func<TU, float, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
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

        internal override void FoldByColumnUnchecked<TU>(TU[] target, Func<TU, float, TU> f, Func<TU, int, TU> finalize, TU[] state, Zeros zeros)
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

        internal override TState Fold2Unchecked<TOther, TState>(MatrixStorage<TOther> other, Func<TState, float, TOther, TState> f, TState state, Zeros zeros)
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
                        state = f(state, At(j,i), i == j ? otherData[i] : otherZero);
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
                            state = f(state, At(row,col), otherValues[k++]);
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

            return base.Fold2Unchecked(other, f, state, zeros);
        }

    }
}