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
        public static extern IntPtr DataTableStorage_AllocByte(long size);
    }
}
﻿
namespace Anemon
{
    public partial class DataTableStorage            // float
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Float(
            IntPtr storage, long columnCount, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Float(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Float(
            IntPtr storage, long columnCount, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Float(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Float(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Float(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, float[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Float(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, float[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern float DataTableStorage_GetElementAt_Float(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Float(
            IntPtr storage, long columnCount, long rowId, long columnId, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Float(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Float(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, float[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Float(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, float[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Float(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, float[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Float(
            IntPtr storage, long startPos, long length, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Float(
            IntPtr storage, long columnCount, long rowId, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Float(
            IntPtr storage, long columnCount, long rowCount, long columnId, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Float(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, float value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Float(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, float value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Float(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Float(
            long rowsA, long columnsA, float[] s, IntPtr u, IntPtr vt, 
            float[] b, long columnsB, float[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Float(
                  storage, ColumnCount, rowId, columnId);
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

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // double
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Double(
            IntPtr storage, long columnCount, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Double(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Double(
            IntPtr storage, long columnCount, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Double(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Double(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Double(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, double[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Double(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, double[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern double DataTableStorage_GetElementAt_Double(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Double(
            IntPtr storage, long columnCount, long rowId, long columnId, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Double(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Double(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, double[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Double(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, double[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Double(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, double[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Double(
            IntPtr storage, long startPos, long length, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Double(
            IntPtr storage, long columnCount, long rowId, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Double(
            IntPtr storage, long columnCount, long rowCount, long columnId, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Double(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, double value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Double(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, double value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Double(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Double(
            long rowsA, long columnsA, double[] s, IntPtr u, IntPtr vt, 
            double[] b, long columnsB, double[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Double(
                  storage, ColumnCount, rowId, columnId);
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

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // byte
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Byte(
            IntPtr storage, long columnCount, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Byte(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Byte(
            IntPtr storage, long columnCount, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Byte(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Byte(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Byte(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, byte[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Byte(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, byte[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern byte DataTableStorage_GetElementAt_Byte(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Byte(
            IntPtr storage, long columnCount, long rowId, long columnId, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Byte(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Byte(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, byte[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Byte(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, byte[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Byte(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, byte[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Byte(
            IntPtr storage, long startPos, long length, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Byte(
            IntPtr storage, long columnCount, long rowId, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Byte(
            IntPtr storage, long columnCount, long rowCount, long columnId, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Byte(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, byte value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Byte(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, byte value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Byte(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Byte(
            long rowsA, long columnsA, byte[] s, IntPtr u, IntPtr vt, 
            byte[] b, long columnsB, byte[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Byte(
                  storage, ColumnCount, rowId, columnId);
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

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // bool
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Bool(
            IntPtr storage, long columnCount, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Bool(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Bool(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Bool(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, bool[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, bool[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern bool DataTableStorage_GetElementAt_Bool(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Bool(
            IntPtr storage, long columnCount, long rowId, long columnId, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Bool(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Bool(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, bool[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Bool(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, bool[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Bool(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, bool[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Bool(
            IntPtr storage, long startPos, long length, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Bool(
            IntPtr storage, long columnCount, long rowId, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Bool(
            IntPtr storage, long columnCount, long rowCount, long columnId, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Bool(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, bool value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Bool(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, bool value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Bool(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Bool(
            long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, 
            bool[] b, long columnsB, bool[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Bool(
                  storage, ColumnCount, rowId, columnId);
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

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // Complex
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex(
            IntPtr storage, long columnCount, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex(
            IntPtr storage, long columnCount, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Complex(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Complex(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Complex(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern Complex DataTableStorage_GetElementAt_Complex(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Complex(
            IntPtr storage, long columnCount, long rowId, long columnId, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Complex(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Complex(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Complex(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Complex(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Complex(
            IntPtr storage, long startPos, long length, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Complex(
            IntPtr storage, long columnCount, long rowId, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Complex(
            IntPtr storage, long columnCount, long rowCount, long columnId, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Complex(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, Complex value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Complex(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, Complex value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Complex(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Complex(
            long rowsA, long columnsA, Complex[] s, IntPtr u, IntPtr vt, 
            Complex[] b, long columnsB, Complex[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Complex(
                  storage, ColumnCount, rowId, columnId);
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

}

﻿
namespace Anemon
{
    public partial class DataTableStorage            // Complex32
    {
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRow_Complex32(
            IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubRow_Complex32(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubRow_Complex32(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, Complex32[] row);


        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetSubColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetSubColumn_Complex32(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, Complex32[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern Complex32 DataTableStorage_GetElementAt_Complex32(
            IntPtr storage, long columnCount, long rowId, long columnId);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetElementAt_Complex32(
            IntPtr storage, long columnCount, long rowId, long columnId, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetRowSkip_Complex32(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetRowSkip_Complex32(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, Complex32[] row);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_GetColumnSkip_Complex32(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex32[] column);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SetColumnSkip_Complex32(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, Complex32[] column);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Clear_Complex32(
            IntPtr storage, long startPos, long length, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearRow_Complex32(
            IntPtr storage, long columnCount, long rowId, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ClearColumn_Complex32(
            IntPtr storage, long columnCount, long rowCount, long columnId, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Add_Complex32(
            IntPtr sourceStorage, IntPtr resultStorage, 
            long columnCount, long rowCount, Complex32 value);
        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_Multiply_Complex32(
            IntPtr sourceStorage, IntPtr resultStorage,
            long form, long count, Complex32 value);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_ConjugateArray_Complex32(
            IntPtr sourceStorage, IntPtr resultStorage, long count);

        [DllImport("DataTableStorage.dll")]
        public static extern void DataTableStorage_SvdSolveFactored_Complex32(
            long rowsA, long columnsA, Complex32[] s, IntPtr u, IntPtr vt, 
            Complex32[] b, long columnsB, Complex32[] x);

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
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount *
                    System.Runtime.InteropServices.Marshal.SizeOf(x));
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
                return DataTableStorage.DataTableStorage_GetElementAt_Complex32(
                  storage, ColumnCount, rowId, columnId);
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

}

