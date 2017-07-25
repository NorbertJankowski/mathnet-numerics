using System;
using System.Runtime.InteropServices;
using System.Text;

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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount * sizeof(float));
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount * sizeof(double));
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount * sizeof(byte));
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
                storage = DataTableStorage.DataTableStorage_AllocByte(
                    RowCount * ColumnCount * sizeof(bool));
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

