using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using MathNet.Numerics;
namespace Anemon
{
    public partial class DataTableStorage
    {
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_Free(IntPtr storage);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
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
