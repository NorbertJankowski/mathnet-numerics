extern "C" {					// float

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Float(
			void *storage, LL columnCount, LL rowId, float *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRow_Float(
		void *storage, LL columnCount, LL rowId, float *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
	}
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Float(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, float *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Float(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, float *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Float(
		void *storage, LL rowCount, LL columnCount, LL columnId, float *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Float(
		void *storage, LL rowCount, LL columnCount, LL columnId, float *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Float(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, float *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Float(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, float *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Float(
		void *storage, LL columnCount, LL rowId, LL columnId, float& value)
	{
		float* s = (float*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Float(
		void *storage, LL columnCount, LL rowId, LL columnId, float value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Float(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, float *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Float(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, float *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Float(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, float *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Float(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, float *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Float(
		void* storage, LL startPos, LL length, float value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Float(
		void* storage, LL columnCount, LL rowId, float value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Float(
		void* storage, LL columnCount, LL rowCount, LL columnId, float value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Float(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, float value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Float(void* sourceStorage, void* resultStorage,
		LL from, LL count, float value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Float(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<float>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Float(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<float>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Float(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<float>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Float(
		LL rowsA, LL columnsA, float* s, float* u, float* vt, float* b, LL columnsB, float* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}

    DLLEXPORT void DataTableStorage_QRSolveFactored_Float(
        void* q, void* r, LL rowsA, LL columnsA, void* tau, void* b, LL columnsB, void* x, char methodFull)
    {
        DataTableStorage_QRSolveFactored<float>(
            (float*)q, (float*)r, rowsA, columnsA, (float*)tau, (float*)b, columnsB, (float*)x, methodFull);
    }
}
