#include "windows.h"
#include<stdlib.h>
#include<stdio.h>
#include<complex>
using namespace std;

#define DLLEXPORT __declspec(dllexport)
typedef long long LL;
#define Complex32 complex<float>
#define Complex complex<double>

BOOL APIENTRY DllMain(HANDLE, DWORD, LPVOID) {
	return TRUE;
}

template<typename T> T Min(T t1, T t2)
{
	return t1 < t2 ? t1 : t2;
}
template<typename T> T Abs(T t1)
{
	return t1 < 0 ? -t1 : t1;
}

// [get | set] row
template<typename T> inline void DataTableStorage_GetRow(
	void *storage, LL columnCount, LL rowId, T *row)
{
	T *s = (T*)storage + columnCount*rowId, *r = row;
	for (LL i = columnCount; i--;)
		*r++ = *s++;
}
template<typename T> inline void DataTableStorage_SetRow(
	void *storage, LL columnCount, LL rowId, T *row)
{
	T *s = (T*)storage + columnCount*rowId, *r = row;
	for (LL i = columnCount; i--;)
		*s++ = *r++;
}
template<typename T1, typename T2> inline void DataTableStorage_SetRowTT(
    void *storage, LL columnCount, LL rowId, T2 *row)
{
    T1 *s = (T1*)storage + columnCount*rowId;
    T2 *r = row;
    for (LL i = columnCount; i--;)
        *s++ = *r++;
}
// [get | set] subrow
template<typename T> inline void DataTableStorage_GetSubRow(
	void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, T *row)
{
	T *s = (T*)storage + columnCount*rowId + startColumn, *r = row;
	for (LL i = subRowColumnCount; i--;)
		*r++ = *s++;
}
template<typename T> inline void DataTableStorage_SetSubRow(
	void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, T *row)
{
	T *s = (T*)storage + columnCount*rowId + startColumn, *r = row;
	for (LL i = subRowColumnCount; i--;)
		*s++ = *r++;
}

// [get | set] column
template<typename T> inline void DataTableStorage_GetColumn(
	void *storage, LL rowCount, LL columnCount, LL columnId, T *column)
{
	T *s = (T*)storage + columnId, *r = column;
	for (LL i = rowCount; i--; s += columnCount)
		*r++ = *s;
}
template<typename T> inline void DataTableStorage_SetColumn(
	void *storage, LL rowCount, LL columnCount, LL columnId, T *column)
{
	T *s = (T*)storage + columnId, *r = column;
	for (LL i = rowCount; i--; s += columnCount)
		*s = *r++;
}
// [get | set] subcolumn
template<typename T> inline void DataTableStorage_GetSubColumn(
	void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, T *column)
{
	T *s = (T*)storage + columnId + startRow*columnCount, *r = column;
	for (LL i = subColumnRowCount; i--; s += columnCount)
		*r++ = *s;
}
template<typename T> inline void DataTableStorage_SetSubColumn(
	void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, T *column)
{
	T *s = (T*)storage + columnId + startRow*columnCount, *r = column;
	for (LL i = subColumnRowCount; i--; s += columnCount)
		*s = *r++;
}

// [get | set] element
template<typename T> inline void DataTableStorage_GetElementAt(
	void *storage, LL columnCount, LL rowId, LL columnId, T& outValue)
{
	T* s = (T*)storage;
	outValue = s[rowId*columnCount + columnId];
}
template<typename T> inline void DataTableStorage_SetElementAt(
	void *storage, LL columnCount, LL rowId, LL columnId, T value)
{
	T* s = (T*)storage;
	s[rowId*columnCount + columnId] = value;
}

/////// G R I D
// [get | set] row
template<typename T> inline void DataTableStorage_GetRowSkip(
	void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, T *row)
{
	T *s = (T*)storage + columnCount*rowId, *r = row;
	for (LL i = skipSize; i--;)
		s += *columnSkip++, *r++ = *s;
}
template<typename T> inline void DataTableStorage_SetRowSkip(
	void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, T *row)
{
	T *s = (T*)storage + columnCount*rowId, *r = row;
	for (LL i = skipSize; i--;)
		s += *columnSkip++, *s = *r++;
}
// [get | set] column
template<typename T> inline void DataTableStorage_GetColumnSkip(
	void *storage, LL *rowSkip, LL skipSize, LL columnId, T *column)
{
	T *s = (T*)storage + columnId, *r = column;
	for (LL i = skipSize; i--;)
		s += *rowSkip++, *r++ = *s;
}
template<typename T> inline void DataTableStorage_SetColumnSkip(
	void *storage, LL *rowSkip, LL skipSize, LL columnId, T *column)
{
	T *s = (T*)storage + columnId, *r = column;
	for (LL i = skipSize; i--;)
		s += *rowSkip++, *s = *r++;
}

template<typename T> inline void DataTableStorage_Clear(
	void* storage, LL startPos, LL length, T value)
{
	T *s = (T*)storage + startPos;
	for (LL i = length; i--; )
		*s++ = value;
}
template<typename T> inline void DataTableStorage_ClearRow(
	void* storage, LL columnCount, LL rowId, T value)
{
	T *s = (T*)storage + rowId *columnCount;
	for (LL i = columnCount; i--; )
		*s++ = value;
}
template<typename T> inline void DataTableStorage_ClearColumn(
	void* storage, LL columnCount, LL rowCount, LL columnId, T value)
{
	T *s = (T*)storage + columnId;
	for (LL i = rowCount; i--; s+=columnCount)
		*s = value;
}

template<typename T> inline void DataTableStorage_Add(
	void* sourceStorage, void* resultStorage,
	LL columnCount, LL rowCount, T value) 
{
	T *s = (T*)sourceStorage, *r = (T*)resultStorage;
	for (LL i = 0; i < rowCount*columnCount; i++)
	{
		*r++ = *s++ + value;
	}
}
template<typename T> inline void DataTableStorage_Multiply(
	void* sourceStorage, void* resultStorage,
	LL from, LL count, T value)
{
	T *s = (T*)sourceStorage + from, *r = (T*)resultStorage + from;
	for (LL i = 0; i < count; i++)
	{
		*r++ = *s++ * value;
	}
}

template<typename T> inline void  DataTableStorage_PointWiseMultiply(
	void*  x, void*  y, void*  result, LL length)
{
	T *_x = (T*)x, *_y = (T*)y, *_result = (T*)result;
	for (LL i = 0; i < length; i++)
	{
		*_result++ = *_x++ * *_y++;
	}
}
template<typename T> inline void DataTableStorage_PointWiseDivide(
	void*  x, void*  y, void*  result, LL length)
{
	T *_x = (T*)x, *_y = (T*)y, *_result = (T*)result;
	for (LL i = 0; i < length; i++)
	{
		*_result++ = *_x++ / *_y++;
	}
}
template<typename T> inline void DataTableStorage_PointWisePower(
	void*  x, void*  y, void*  result, LL length)
{
	T *_x = (T*)x, *_y = (T*)y, *_result = (T*)result;
	for (LL i = 0; i < length; i++)
	{
		*_result++ = pow(*_x++, *_y++);
	}
}


extern "C" {

	DLLEXPORT void DataTableStorage_ConjugateArray_Complex(
		void* sourceStorage, void* resultStorage, LL count)
	{
		Complex *s = (Complex*)sourceStorage, *t = (Complex*)resultStorage,
			tmp;
		for (LL i = 0; i < count; i++)
		{
			tmp = conj(s[i]);
			*t++ = tmp;
			s++;
		}
	}
	DLLEXPORT void DataTableStorage_ConjugateArray_Complex32(
		void* sourceStorage, void* resultStorage, LL count)
	{
		Complex32 *s = (Complex32*)sourceStorage, *t = (Complex32*)resultStorage,
			tmp;
		for (LL i = 0; i < count; i++)
		{
			tmp = conj(s[i]);
			*t++ = tmp;
			s++;
		}
	}
	DLLEXPORT void DataTableStorage_ConjugateArray_Double(
		void* sourceStorage, void* resultStorage, LL count)
	{
		memcpy(resultStorage, sourceStorage, count * sizeof(double));
	}
	DLLEXPORT void DataTableStorage_ConjugateArray_Float(
		void* sourceStorage, void* resultStorage, LL count)
	{
		memcpy(resultStorage, sourceStorage, count * sizeof(float));
	}
}

extern "C" {
	DLLEXPORT void DataTableStorage_Free(void* p)
	{
		free(p);
	}
    DLLEXPORT void DataTableStorage_AllocByte(LL size, void* &p)
    {
        p = calloc(size, 1);
    }
    /*DLLEXPORT void * DataTableStorage_AllocByte(LL size)
    {
        return calloc(size, 1);
    }*/
}

extern "C" {
    DLLEXPORT void DataTableStorage_SetRowDF(
        void *storage, LL columnCount, LL rowId, float *row)
    {
        DataTableStorage_SetRowTT<double, float>(storage, columnCount, rowId, row);
    }
}
template<typename T> inline void DataTableStorage_SvdSolveFactored(
	LL rowsA, LL columnsA, T* s, T* u, T* vt, T* b, LL columnsB, T* x)
{

	LL mn = Min(rowsA, columnsA);
	T* tmp = new T[columnsA];

	for (LL k = 0; k < columnsB; k++)
	{
		for (LL j = 0; j < columnsA; j++)
		{
			T value = 0;
			if (j < mn)
			{
				for (LL i = 0; i < rowsA; i++)
				{
					value += u[(j * rowsA) + i] * b[(k * rowsA) + i];
				}

				value /= s[j];
			}

			tmp[j] = value;
		}

		for (LL j = 0; j < columnsA; j++)
		{
			T value = 0;
			for (LL i = 0; i < columnsA; i++)
			{
				value += vt[(j * columnsA) + i] * tmp[i];
			}

			x[(k * columnsA) + j] = value;
		}
	}
	delete[] tmp;
}
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
}
extern "C" {					// double

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Double(
			void *storage, LL columnCount, LL rowId, double *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRow_Double(
		void *storage, LL columnCount, LL rowId, double *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
	}
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Double(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, double *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Double(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, double *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Double(
		void *storage, LL rowCount, LL columnCount, LL columnId, double *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Double(
		void *storage, LL rowCount, LL columnCount, LL columnId, double *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Double(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, double *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Double(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, double *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Double(
		void *storage, LL columnCount, LL rowId, LL columnId, double& value)
	{
		double* s = (double*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Double(
		void *storage, LL columnCount, LL rowId, LL columnId, double value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Double(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, double *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Double(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, double *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Double(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, double *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Double(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, double *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Double(
		void* storage, LL startPos, LL length, double value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Double(
		void* storage, LL columnCount, LL rowId, double value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Double(
		void* storage, LL columnCount, LL rowCount, LL columnId, double value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Double(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, double value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Double(void* sourceStorage, void* resultStorage,
		LL from, LL count, double value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Double(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<double>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Double(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<double>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Double(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<double>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Double(
		LL rowsA, LL columnsA, double* s, double* u, double* vt, double* b, LL columnsB, double* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}
}
extern "C" {					// byte

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Byte(
			void *storage, LL columnCount, LL rowId, byte *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRow_Byte(
		void *storage, LL columnCount, LL rowId, byte *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
    }
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Byte(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, byte *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Byte(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, byte *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Byte(
		void *storage, LL rowCount, LL columnCount, LL columnId, byte *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Byte(
		void *storage, LL rowCount, LL columnCount, LL columnId, byte *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Byte(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, byte *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Byte(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, byte *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Byte(
		void *storage, LL columnCount, LL rowId, LL columnId, byte& value)
	{
		byte* s = (byte*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Byte(
		void *storage, LL columnCount, LL rowId, LL columnId, byte value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Byte(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, byte *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Byte(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, byte *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Byte(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, byte *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Byte(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, byte *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Byte(
		void* storage, LL startPos, LL length, byte value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Byte(
		void* storage, LL columnCount, LL rowId, byte value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Byte(
		void* storage, LL columnCount, LL rowCount, LL columnId, byte value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Byte(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, byte value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Byte(void* sourceStorage, void* resultStorage,
		LL from, LL count, byte value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Byte(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<byte>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Byte(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<byte>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Byte(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<byte>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Byte(
		LL rowsA, LL columnsA, byte* s, byte* u, byte* vt, byte* b, LL columnsB, byte* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}
}
extern "C" {					// bool

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Bool(
			void *storage, LL columnCount, LL rowId, bool *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
    }
	DLLEXPORT void DataTableStorage_SetRow_Bool(
		void *storage, LL columnCount, LL rowId, bool *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
    }
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Bool(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, bool *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Bool(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, bool *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Bool(
		void *storage, LL rowCount, LL columnCount, LL columnId, bool *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Bool(
		void *storage, LL rowCount, LL columnCount, LL columnId, bool *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Bool(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, bool *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Bool(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, bool *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Bool(
		void *storage, LL columnCount, LL rowId, LL columnId, bool& value)
	{
		bool* s = (bool*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Bool(
		void *storage, LL columnCount, LL rowId, LL columnId, bool value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Bool(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, bool *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Bool(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, bool *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Bool(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, bool *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Bool(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, bool *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Bool(
		void* storage, LL startPos, LL length, bool value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Bool(
		void* storage, LL columnCount, LL rowId, bool value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Bool(
		void* storage, LL columnCount, LL rowCount, LL columnId, bool value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Bool(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, bool value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Bool(void* sourceStorage, void* resultStorage,
		LL from, LL count, bool value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Bool(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<bool>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Bool(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<bool>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Bool(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<bool>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Bool(
		LL rowsA, LL columnsA, bool* s, bool* u, bool* vt, bool* b, LL columnsB, bool* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}
}
extern "C" {					// Complex

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Complex(
			void *storage, LL columnCount, LL rowId, Complex *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRow_Complex(
		void *storage, LL columnCount, LL rowId, Complex *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
	}
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Complex(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, Complex *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Complex(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, Complex *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Complex(
		void *storage, LL rowCount, LL columnCount, LL columnId, Complex *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Complex(
		void *storage, LL rowCount, LL columnCount, LL columnId, Complex *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Complex(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, Complex *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Complex(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, Complex *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Complex(
		void *storage, LL columnCount, LL rowId, LL columnId, Complex& value)
	{
		Complex* s = (Complex*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Complex(
		void *storage, LL columnCount, LL rowId, LL columnId, Complex value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Complex(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, Complex *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Complex(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, Complex *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Complex(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, Complex *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Complex(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, Complex *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Complex(
		void* storage, LL startPos, LL length, Complex value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Complex(
		void* storage, LL columnCount, LL rowId, Complex value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Complex(
		void* storage, LL columnCount, LL rowCount, LL columnId, Complex value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Complex(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, Complex value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Complex(void* sourceStorage, void* resultStorage,
		LL from, LL count, Complex value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Complex(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<Complex>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Complex(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<Complex>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Complex(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<Complex>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Complex(
		LL rowsA, LL columnsA, Complex* s, Complex* u, Complex* vt, Complex* b, LL columnsB, Complex* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}
}
extern "C" {					// Complex32

	// [get | set] row
	DLLEXPORT void DataTableStorage_GetRow_Complex32(
			void *storage, LL columnCount, LL rowId, Complex32 *row)
	{
		DataTableStorage_GetRow(storage, columnCount, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRow_Complex32(
		void *storage, LL columnCount, LL rowId, Complex32 *row)
	{
		DataTableStorage_SetRow(storage, columnCount, rowId, row);
	}
	// [get | set] subrow
	DLLEXPORT void DataTableStorage_GetSubRow_Complex32(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, Complex32 *row)
	{
		DataTableStorage_GetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}
	DLLEXPORT void DataTableStorage_SetSubRow_Complex32(
		void *storage, LL columnCount, LL rowId, LL startColumn, LL subRowColumnCount, Complex32 *row)
	{
		DataTableStorage_SetSubRow(storage, columnCount, rowId, startColumn, subRowColumnCount, row);
	}

	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumn_Complex32(
		void *storage, LL rowCount, LL columnCount, LL columnId, Complex32 *column)
	{
		DataTableStorage_GetColumn(storage, rowCount, columnCount, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumn_Complex32(
		void *storage, LL rowCount, LL columnCount, LL columnId, Complex32 *column)
	{
		DataTableStorage_SetColumn(storage, rowCount, columnCount, columnId, column);
	}
	// [get | set] subcolumn
	DLLEXPORT void DataTableStorage_GetSubColumn_Complex32(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, Complex32 *column)
	{
		DataTableStorage_GetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}
	DLLEXPORT void DDataTableStorage_SetSubColumn_Complex32(
		void *storage, LL rowCount, LL columnCount, LL columnId, LL startRow, LL subColumnRowCount, Complex32 *column)
	{
		DataTableStorage_SetSubColumn(storage, rowCount, columnCount, columnId, startRow, subColumnRowCount, column);
	}

	// [get | set] element
	DLLEXPORT void DataTableStorage_GetElementAt_Complex32(
		void *storage, LL columnCount, LL rowId, LL columnId, Complex32& value)
	{
		Complex32* s = (Complex32*)storage;
		value = s[rowId*columnCount + columnId];
	}
	DLLEXPORT void DataTableStorage_SetElementAt_Complex32(
		void *storage, LL columnCount, LL rowId, LL columnId, Complex32 value)
	{
		DataTableStorage_SetElementAt(storage, columnCount, rowId, columnId, value);
	}

	// [get | set] row   skip
	DLLEXPORT void DataTableStorage_GetRowSkip_Complex32(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, Complex32 *row)
	{
		DataTableStorage_GetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	DLLEXPORT void DataTableStorage_SetRowSkip_Complex32(
		void *storage, LL columnCount, LL *columnSkip, LL skipSize, LL rowId, Complex32 *row)
	{
		DataTableStorage_SetRowSkip(storage, columnCount, columnSkip, skipSize, rowId, row);
	}
	// [get | set] column
	DLLEXPORT void DataTableStorage_GetColumnSkip_Complex32(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, Complex32 *column)
	{
		DataTableStorage_GetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}
	DLLEXPORT void DataTableStorage_SetColumnSkip_Complex32(
		void *storage, LL *rowSkip, LL skipSize, LL columnId, Complex32 *column)
	{
		DataTableStorage_SetColumnSkip(storage, rowSkip, skipSize, columnId, column);
	}

	DLLEXPORT void DataTableStorage_Clear_Complex32(
		void* storage, LL startPos, LL length, Complex32 value)
	{
		DataTableStorage_Clear(storage, startPos, length, value);
	}
	DLLEXPORT void DataTableStorage_ClearRow_Complex32(
		void* storage, LL columnCount, LL rowId, Complex32 value)
	{
		DataTableStorage_ClearRow(storage, columnCount, rowId, value);
	}
	DLLEXPORT void DataTableStorage_ClearColumn_Complex32(
		void* storage, LL columnCount, LL rowCount, LL columnId, Complex32 value)
	{
		DataTableStorage_ClearColumn(storage, columnCount, rowCount, columnId, value);
	}

	DLLEXPORT void DataTableStorage_Add_Complex32(void* sourceStorage, void* resultStorage,
		LL columnCount, LL rowCount, Complex32 value)
	{
		DataTableStorage_Add(sourceStorage, resultStorage,
			columnCount, rowCount, value);
	}
	DLLEXPORT void DataTableStorage_Multiply_Complex32(void* sourceStorage, void* resultStorage,
		LL from, LL count, Complex32 value) 
	{
		DataTableStorage_Multiply(sourceStorage, resultStorage,
			 from,  count,  value);
	}

	DLLEXPORT void  DataTableStorage_PointWiseMultiply_Complex32(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseMultiply<Complex32>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWiseDivide_Complex32(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWiseDivide<Complex32>(x, y, result, length);
	}
	DLLEXPORT void DataTableStorage_PointWisePower_Complex32(
		void*  x, void*  y, void*  result, LL length)
	{
		DataTableStorage_PointWisePower<Complex32>(x, y, result, length);
	}

	DLLEXPORT void DataTableStorage_SvdSolveFactored_Complex32(
		LL rowsA, LL columnsA, Complex32* s, Complex32* u, Complex32* vt, Complex32* b, LL columnsB, Complex32* x)
	{
		DataTableStorage_SvdSolveFactored(rowsA, columnsA, s, u, vt, b, columnsB, x);
	}
}
