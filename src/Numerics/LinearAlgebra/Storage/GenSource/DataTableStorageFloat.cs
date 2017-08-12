
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
        public static extern void DataTableStorage_GetElementAt_Float(
            IntPtr storage, long columnCount, long rowId, long columnId, out float value);
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

