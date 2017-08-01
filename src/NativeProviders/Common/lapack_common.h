#pragma once

#include <cstring>

const int INSUFFICIENT_MEMORY = -999999;

#ifndef LAPACK_MEMORY
#define LAPACK_MEMORY
#include <memory>
template <typename T> using array_ptr = std::unique_ptr<T[]>;

template<typename T>
inline array_ptr<T> array_new(const int size)
{
	return array_ptr<T>(new T[size]);
}
#endif

template<typename T>
inline array_ptr<T> array_clone(const long long size, const T* array)
{
	auto clone = array_new<T>(size);
	memcpy(clone.get(), array, size * sizeof(T));
	return clone;
}

template<typename INT>
inline void shift_ipiv_down(INT m, INT ipiv[])
{
	for(INT i = 0; i < m; ++i )
	{
		ipiv[i] -= 1;
	}
}

template<typename INT>
inline void shift_ipiv_up(INT m, INT ipiv[])
{
	for(INT i = 0; i < m; ++i )
	{
		ipiv[i] += 1;
	}
}

template<typename T> 
inline T* Clone(const long long m, const long long n, const T* a)
{
	auto clone = new T[m*n];
	memcpy(clone, a, m*n*sizeof(T));
	return clone;
}

template<typename T> 
inline void copyBtoX (long long m, long long n, long long bn, T b[], T x[])
{
	for (auto i = 0; i < n; ++i)
	{
		for (auto j = 0; j < bn; ++j)
		{
			x[j * n + i] = b[j * m + i];
		}
	}
}

