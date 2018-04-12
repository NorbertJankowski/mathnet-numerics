namespace Anemon
{
    public partial class DataTableStorage            // bool
    {
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetRow_Bool(
            IntPtr storage, long columnCount, long rowId, [Out] IntPtr row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, IntPtr row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetSubRow_Bool(
            IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetSubRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, long startColumn, long subRowColumnCount, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);


        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, [Out] IntPtr column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, IntPtr column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetSubColumn_Bool(
            IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetSubColumn_Bool(
            [Out] IntPtr storage, long rowCount, long columnCount, long columnId, long startRow, long subColumnRowCount, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetElementAt_Bool(
            IntPtr storage, long columnCount, long rowId, long columnId, [Out, MarshalAs(UnmanagedType.U1)] out bool value);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetElementAt_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, long columnId, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetRowSkip_Bool(
            IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetRowSkip_Bool(
            [Out] IntPtr storage, long columnCount, long[] columnSkip, long skipSize, long rowId, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] row);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_GetColumnSkip_Bool(
            IntPtr storage, long[] rowSkip, long skipSize, long columnId, [Out, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SetColumnSkip_Bool(
            [Out] IntPtr storage, long[] rowSkip, long skipSize, long columnId, [In, MarshalAs( UnmanagedType.LPArray,
                ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)] bool[] column);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_Clear_Bool(
            [Out] IntPtr storage, long startPos, long length, [MarshalAs(UnmanagedType.U1)] bool value);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_ClearRow_Bool(
            [Out] IntPtr storage, long columnCount, long rowId, [MarshalAs(UnmanagedType.U1)] bool value);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_ClearColumn_Bool(
            [Out] IntPtr storage, long columnCount, long rowCount, long columnId, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_Add_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long columnCount, long rowCount, bool value);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_Multiply_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage,
            long form, long count, bool value);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_ConjugateArray_Bool(
            IntPtr sourceStorage, [Out] IntPtr resultStorage, long count);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SvdSolveFactored_Bool(
            long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, IntPtr b, long columnsB, IntPtr x);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_SvdSolveFactored_Bool(
            long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, bool[] b, long columnsB, [Out] bool[] x);

        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_QRSolveFactored_Bool(
            IntPtr q, IntPtr r, long rowsA, long columnsA, bool[] tau, IntPtr b, long columnsB, IntPtr x, char methodFull);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_QRSolveFactored_Bool(
            IntPtr q, IntPtr r, long rowsA, long columnsA, bool[] tau, bool[] b, long columnsB, bool[] x, char methodFull);


        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_PointWiseMultiply_Bool(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataTableStorage_PointWiseDivide_Bool(
            IntPtr x, IntPtr y, [Out] IntPtr result, long length);
        [DllImport(_DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
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

        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, IntPtr b, long columnsB, IntPtr x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Bool(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }
        override public void DataTableStorage_SvdSolveFactored(long rowsA, long columnsA, bool[] s, IntPtr u, IntPtr vt, bool[] b, long columnsB, bool[] x)
        {
            DataTableStorage.DataTableStorage_SvdSolveFactored_Bool(rowsA, columnsA, s, u, vt, b, columnsB, x);
        }

        override public void DataTableStorage_QRSolveFactored(IntPtr q, IntPtr r, long rowsA, long columnsA, bool[] tau, IntPtr b, long columnsB, IntPtr x, char methodFull)
        {
            DataTableStorage.DataTableStorage_QRSolveFactored_Bool(q, r, rowsA, columnsA, tau, b, columnsB, x, methodFull);
        }
        override public void DataTableStorage_QRSolveFactored(IntPtr q, IntPtr r, long rowsA, long columnsA, bool[] tau, bool[] b, long columnsB, bool[] x, char methodFull)
        {
            DataTableStorage.DataTableStorage_QRSolveFactored_Bool(q, r, rowsA, columnsA, tau, b, columnsB, x, methodFull);
        }

    }

}
