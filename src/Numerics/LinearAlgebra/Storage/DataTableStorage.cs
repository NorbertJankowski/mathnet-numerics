using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using MathNet.Numerics;
namespace Anemon
{
    public partial class DataTableStorage
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Free(IntPtr storage);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_AllocByte(long size, [Out] out IntPtr p);
        //public static extern IntPtr DataTableStorage_AllocByte(long size);
    }

    public abstract class DataTableStorage<T>
    {
        public void DataTableStorage_Free(IntPtr storage)
        {
            DataTableStorage.DataTableStorage_Free(storage);
        }
        public IntPtr DataTableStorage_AllocByte(long size)
        {
            IntPtr p;
            DataTableStorage.DataTableStorage_AllocByte(size, out p);
            return p;
            //return DataTableStorage.DataTableStorage_AllocByte(size);
        }
        static public DataTableStorage<T> CreateDataTableStorage()
        {
            if (typeof(T).Equals(typeof(float)))
                return new DataTableStorageFloat() as DataTableStorage<T>;
            else
            if (typeof(T).Equals(typeof(double)))
                return new DataTableStorageDouble() as DataTableStorage<T>;
            else
            if (typeof(T).Equals(typeof(Complex)))
                return new DataTableStorageComplex() as DataTableStorage<T>;
            else
            if (typeof(T).Equals(typeof(Complex32)))
                return new DataTableStorageComplex32() as DataTableStorage<T>;
            else
                new Exception("Error: Cannot create DataTableStorage of type " + typeof(T).ToString());
            return null;
        }

        abstract public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, T[] row);
        abstract public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row);
        abstract public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, T[] row);
        abstract public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row);
        abstract public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, T[] row);
        abstract public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, T[] row);

        abstract public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, T[] column);
        abstract public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        abstract public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, T[] column);
        abstract public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        abstract public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, T[] column);
        abstract public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, T[] column);

        abstract public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out T value);
        abstract public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, T value);
        abstract public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, T[] row);
        abstract public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, T[] row);
        abstract public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, T[] column);
        abstract public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, T[] column);

        public T Zero { get { return default(T);  } }
        abstract public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, T value);
        abstract public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, T value);
        abstract public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, T value);

        abstract public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, T value);
        abstract public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, T value);

        abstract public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count);
        abstract public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, T[] s, IntPtr u, IntPtr vt, T[] b, long columnsB, T[] x);
    }
}
﻿
namespace Anemon
{
    public partial class DataTableStorage            // float
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Float(
            IntPtr storage, long columnCount, long rowId, [Out] float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Float(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Float(
            [Out] IntPtr storage, long columnCount, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Float(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Float(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Float(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, float[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Float(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Float(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Float(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, float[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Float(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Float(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Float(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Float(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Float(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Float(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, float[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Float(
            [Out] IntPtr storage, long startPos, long length, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Float(
            [Out] IntPtr storage, long columnCount, long rowId, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Float(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Float(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Float(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Float(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Float(
            long rowsA, long columnsA, float[] s, IntPtr u, IntPtr vt, 
            float[] b, long columnsB, [Out] float[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Float(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Float(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Float(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Float : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Float()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                float x = default(float);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(float));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, float[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Float(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, float[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Float(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, float[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Float(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, float[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Float(storage, RowCount, ColumnCount, columnId, t);
        }
        public float this[long rowId, long columnId]
        {
            get
            {
                float v;
                DataTableStorage.DataTableStorage_GetElementAt_Float(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Float(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageFloat : DataTableStorage<float>            // float
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, float[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Float(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Float(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, float[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Float(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Float(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, float[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Float(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, float[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Float(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, float[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Float(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Float(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, float[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Float(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Float(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, float[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Float(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, float[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Float(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out float value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Float(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, float value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Float(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, float[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Float(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, float[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Float(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, float[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Float(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, float[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Float(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, float value)
        {
            DataTableStorage.DataTableStorage_Clear_Float(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, float value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Float(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, float value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Float(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, float value)
        {
            DataTableStorage.DataTableStorage_Add_Float(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, float value)
        {
            DataTableStorage.DataTableStorage_Multiply_Float(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Float(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, float[] s, IntPtr u, IntPtr vt, float[] b, long columnsB, float[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Float(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // double
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Double(
            IntPtr storage, long columnCount, long rowId, [Out] double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Double(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Double(
            [Out] IntPtr storage, long columnCount, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Double(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Double(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Double(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, double[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Double(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Double(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Double(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, double[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Double(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Double(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Double(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Double(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Double(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Double(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, double[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Double(
            [Out] IntPtr storage, long startPos, long length, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Double(
            [Out] IntPtr storage, long columnCount, long rowId, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Double(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Double(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Double(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Double(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Double(
            long rowsA, long columnsA, double[] s, IntPtr u, IntPtr vt, 
            double[] b, long columnsB, [Out] double[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Double(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Double(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Double(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Double : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Double()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                double x = default(double);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(double));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, double[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Double(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, double[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Double(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, double[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Double(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, double[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Double(storage, RowCount, ColumnCount, columnId, t);
        }
        public double this[long rowId, long columnId]
        {
            get
            {
                double v;
                DataTableStorage.DataTableStorage_GetElementAt_Double(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Double(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageDouble : DataTableStorage<double>            // double
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, double[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Double(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Double(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, double[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Double(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Double(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, double[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Double(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, double[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Double(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, double[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Double(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Double(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, double[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Double(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Double(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, double[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Double(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, double[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Double(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out double value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Double(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, double value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Double(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, double[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Double(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, double[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Double(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, double[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Double(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, double[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Double(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, double value)
        {
            DataTableStorage.DataTableStorage_Clear_Double(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, double value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Double(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, double value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Double(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, double value)
        {
            DataTableStorage.DataTableStorage_Add_Double(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, double value)
        {
            DataTableStorage.DataTableStorage_Multiply_Double(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Double(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, double[] s, IntPtr u, IntPtr vt, double[] b, long columnsB, double[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Double(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // byte
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Byte(
            IntPtr storage, long columnCount, long rowId, [Out] byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Byte(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Byte(
            [Out] IntPtr storage, long columnCount, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Byte(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Byte(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Byte(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, byte[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Byte(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Byte(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Byte(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, byte[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Byte(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Byte(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Byte(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Byte(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Byte(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Byte(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, byte[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Byte(
            [Out] IntPtr storage, long startPos, long length, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Byte(
            [Out] IntPtr storage, long columnCount, long rowId, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Byte(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Byte(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Byte(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Byte(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Byte(
            long rowsA, long columnsA, byte[] s, IntPtr u, IntPtr vt, 
            byte[] b, long columnsB, [Out] byte[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Byte(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Byte(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Byte(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Byte : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Byte()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                byte x = default(byte);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(byte));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, byte[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Byte(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, byte[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Byte(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, byte[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Byte(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, byte[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Byte(storage, RowCount, ColumnCount, columnId, t);
        }
        public byte this[long rowId, long columnId]
        {
            get
            {
                byte v;
                DataTableStorage.DataTableStorage_GetElementAt_Byte(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Byte(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageByte : DataTableStorage<byte>            // byte
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, byte[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Byte(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Byte(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, byte[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Byte(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Byte(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, byte[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Byte(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, byte[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Byte(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, byte[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Byte(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Byte(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, byte[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Byte(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Byte(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, byte[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Byte(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, byte[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Byte(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out byte value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Byte(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, byte value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Byte(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, byte[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Byte(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, byte[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Byte(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, byte[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Byte(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, byte[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Byte(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, byte value)
        {
            DataTableStorage.DataTableStorage_Clear_Byte(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, byte value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Byte(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, byte value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Byte(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, byte value)
        {
            DataTableStorage.DataTableStorage_Add_Byte(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, byte value)
        {
            DataTableStorage.DataTableStorage_Multiply_Byte(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Byte(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, byte[] s, IntPtr u, IntPtr vt, byte[] b, long columnsB, byte[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Byte(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // bool
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, [Out] bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Bool(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, bool[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, bool[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Bool(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Bool(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Bool(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Bool(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Bool(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, bool[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Bool(
            [Out] IntPtr storage, long startPos, long length, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Bool(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Bool(
            long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, 
            bool[] b, long columnsB, [Out] bool[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Bool(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Bool(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Bool(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Bool : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Bool()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                bool x = default(bool);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(bool));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, bool[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Bool(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, bool[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Bool(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, bool[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Bool(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, bool[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Bool(storage, RowCount, ColumnCount, columnId, t);
        }
        public bool this[long rowId, long columnId]
        {
            get
            {
                bool v;
                DataTableStorage.DataTableStorage_GetElementAt_Bool(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Bool(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageBool : DataTableStorage<bool>            // bool
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, bool[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Bool(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Bool(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, bool[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Bool(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Bool(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, bool[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Bool(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, bool[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Bool(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, bool[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Bool(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Bool(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, bool[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Bool(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Bool(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, bool[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Bool(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, bool[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Bool(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out bool value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Bool(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, bool value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Bool(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, bool[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Bool(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, bool[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Bool(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, bool[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Bool(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, bool[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Bool(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, bool value)
        {
            DataTableStorage.DataTableStorage_Clear_Bool(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, bool value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Bool(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, bool value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Bool(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, bool value)
        {
            DataTableStorage.DataTableStorage_Add_Bool(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, bool value)
        {
            DataTableStorage.DataTableStorage_Multiply_Bool(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Bool(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, bool[] b, long columnsB, bool[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Bool(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // Complex
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex(
            IntPtr storage, long columnCount, long rowId, [Out] Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex(
            [Out] IntPtr storage, long columnCount, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Complex(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Complex(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Complex(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Complex(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Complex(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Complex(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Complex(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Complex(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Complex(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Complex(
            [Out] IntPtr storage, long startPos, long length, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Complex(
            [Out] IntPtr storage, long columnCount, long rowId, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Complex(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Complex(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Complex(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Complex(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Complex(
            long rowsA, long columnsA, Complex[] s, IntPtr u, IntPtr vt, 
            Complex[] b, long columnsB, [Out] Complex[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Complex(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Complex(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Complex(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Complex : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Complex()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                Complex x = default(Complex);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(Complex));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, Complex[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, Complex[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, Complex[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, Complex[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex(storage, RowCount, ColumnCount, columnId, t);
        }
        public Complex this[long rowId, long columnId]
        {
            get
            {
                Complex v;
                DataTableStorage.DataTableStorage_GetElementAt_Complex(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Complex(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageComplex : DataTableStorage<Complex>            // Complex
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, Complex[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, Complex[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Complex(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Complex(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, Complex[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, Complex[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Complex(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Complex(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out Complex value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Complex(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, Complex value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Complex(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Complex(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Complex(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Complex(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Complex(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, Complex value)
        {
            DataTableStorage.DataTableStorage_Clear_Complex(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, Complex value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Complex(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, Complex value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Complex(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, Complex value)
        {
            DataTableStorage.DataTableStorage_Add_Complex(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, Complex value)
        {
            DataTableStorage.DataTableStorage_Multiply_Complex(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Complex(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, Complex[] s, IntPtr u, IntPtr vt, Complex[] b, long columnsB, Complex[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Complex(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // Complex32
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, [Out] Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex32(
            [Out] IntPtr storage, long columnCount, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex32(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Complex32(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out] Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Complex32(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex32[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex32(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex32(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out] Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Complex32(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex32[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetElementAt_Complex32(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out] out Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Complex32(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Complex32(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out] Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Complex32(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Complex32(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out] Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Complex32(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex32[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Complex32(
            [Out] IntPtr storage, long startPos, long length, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Complex32(
            [Out] IntPtr storage, long columnCount, long rowId, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Complex32(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Complex32(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, 
            long columnCount, long rowCount, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Complex32(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Complex32(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Complex32(
            long rowsA, long columnsA, Complex32[] s, IntPtr u, IntPtr vt, 
            Complex32[] b, long columnsB, [Out] Complex32[] x);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseMultiply_Complex32(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWiseDivide_Complex32(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_PointWisePower_Complex32(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
    }

    public class DataTableStorage_Complex32 : IDisposable
    {
        public IntPtr storage;
        public long RowCount { get; private set; }
        public long ColumnCount { get; private set; }

        ~DataTableStorage_Complex32()
        {
            Free();
        }
        public void Free()
        {
            if (storage != IntPtr.Zero)
            {
                DataTableStorage.DataTableStorage_Free(storage);
                storage = IntPtr.Zero;
            }
        }
        public void Dispose()
        {
            Free();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < RowCount; i++)
            {
                for (long j = 0; j < ColumnCount; j++)
                    sb.AppendFormat("{0}, ", this[i, j]);
                sb.Append("\n");
            }

            return sb.ToString();
        }
        public void Init(long rowCount, long columnCount)
        {
            if (storage == IntPtr.Zero)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                Complex32 x = default(Complex32);
                DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x), out storage);
                    //sizeof(Complex32));
                if (storage == IntPtr.Zero)
                    throw new Exception("ERROR: Out of memory in DataTableStorage");
            }
            else
                throw new Exception("Data storage is already allocated. First free it....");
        }

        public void GetRow(long rowId, Complex32[] t)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex32(storage, ColumnCount, rowId, t);
        }
        public void SetRow(long rowId, Complex32[] t)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex32(storage, ColumnCount, rowId, t);
        }
        public void GetColumn(long columnId, Complex32[] t)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex32(storage, RowCount, ColumnCount, columnId, t);
        }
        public void SetColumn(long columnId, Complex32[] t)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex32(storage, RowCount, ColumnCount, columnId, t);
        }
        public Complex32 this[long rowId, long columnId]
        {
            get
            {
                Complex32 v;
                DataTableStorage.DataTableStorage_GetElementAt_Complex32(
                  storage, ColumnCount, rowId, columnId, out v);
                return v;
            }
            set
            {
                DataTableStorage.DataTableStorage_SetElementAt_Complex32(
                    storage, ColumnCount, rowId, columnId, value);
            }
        }

        public int GetHash()
        {
            return 0;
        }
    }

    public class DataTableStorageComplex32 : DataTableStorage<Complex32>            // Complex32
    {
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex32(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_GetRow_Complex32(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex32(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_SetRow(IntPtr storage, long columnCount, long rowId, IntPtr row)
        {
            DataTableStorage.DataTableStorage_SetRow_Complex32(storage, columnCount, rowId, row);
        }
        override public void DataTableStorage_GetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_GetSubRow_Complex32(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }
        override public void DataTableStorage_SetSubRow(IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_SetSubRow_Complex32(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
        }

        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex32(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_GetColumn_Complex32(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex32(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_SetColumn(IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column)
        {
            DataTableStorage.DataTableStorage_SetColumn_Complex32(storage, rowCount, columnCount, columnId, column);
        }
        override public void DataTableStorage_GetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_GetSubColumn_Complex32(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }
        override public void DataTableStorage_SetSubColumn(IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_SetSubColumn_Complex32(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
        }

        override public void DataTableStorage_GetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, out Complex32 value)
        {
            DataTableStorage.DataTableStorage_GetElementAt_Complex32(storage, columnCount, rowId, columnId, out value);
        }
        override public void DataTableStorage_SetElementAt(IntPtr storage, long columnCount, long rowId, long columnId, Complex32 value)
        {
            DataTableStorage.DataTableStorage_SetElementAt_Complex32(storage, columnCount, rowId, columnId, value);
        }
        override public void DataTableStorage_GetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_GetRowSkip_Complex32(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_SetRowSkip(IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex32[] row)
        {
            DataTableStorage.DataTableStorage_SetRowSkip_Complex32(storage, columnCount, columnSkip, skipSize, rowId, row);
        }
        override public void DataTableStorage_GetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_GetColumnSkip_Complex32(storage, rowSkip, skipSize, columnId, column);
        }
        override public void DataTableStorage_SetColumnSkip(IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex32[] column)
        {
            DataTableStorage.DataTableStorage_SetColumnSkip_Complex32(storage, rowSkip, skipSize, columnId, column);
        }

        override public void DataTableStorage_Clear(IntPtr storage, long startPos, long length, Complex32 value)
        {
            DataTableStorage.DataTableStorage_Clear_Complex32(storage, startPos, length, value);
        }
        override public void DataTableStorage_ClearRow(IntPtr storage, long columnCount, long rowId, Complex32 value)
        {
            DataTableStorage.DataTableStorage_ClearRow_Complex32(storage, columnCount, rowId, value);
        }
        override public void DataTableStorage_ClearColumn(IntPtr storage, long columnCount, long rowCount, long columnId, Complex32 value)
        {
            DataTableStorage.DataTableStorage_ClearColumn_Complex32(storage, columnCount, rowCount, columnId, value);
        }

        override public void DataTableStorage_Add(IntPtr sourceStorage, IntPtr resultStorage, long columnCount, long rowCount, Complex32 value)
        {
            DataTableStorage.DataTableStorage_Add_Complex32(sourceStorage, resultStorage, columnCount, rowCount, value);
        }
        override public void DataTableStorage_Multiply(IntPtr sourceStorage, IntPtr resultStorage, long form, long count, Complex32 value)
        {
            DataTableStorage.DataTableStorage_Multiply_Complex32(sourceStorage, resultStorage, form, count, value);
        }

        override public void DataTableStorage_ConjugateArray(IntPtr sourceStorage, IntPtr resultStorage, long count)
        {
            DataTableStorage.DataTableStorage_ConjugateArray_Complex32(sourceStorage, resultStorage, count);
        }

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, Complex32[] s, IntPtr u, IntPtr vt, Complex32[] b, long columnsB, Complex32[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Complex32(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
    }

}

