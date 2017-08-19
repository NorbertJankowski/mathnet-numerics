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
	DLLEXPORT void * DataTableStorage_AllocByte(LL size)
	{
		return calloc(size,1);
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