#pragma once
#include "turbojpeg.h"
#include "opencv2/opencv.hpp"
#include "k4a/k4a.h"

#ifdef IM2PC_API
#define IM2PC_API __declspec(dllexport)
#else
#define IM2PC_API __declspec(dllimport)
#endif

extern "C" IM2PC_API void InitImageProcessing();
extern "C" IM2PC_API void CloseImageProcessing();

extern "C" IM2PC_API void JPEG2BGRA(int width, int height, long jpegSize, const unsigned char* jpegBuffer, unsigned char* bgraBuffer);

typedef struct Point3f
{
	Point3f()
	{
		this->X = 0;
		this->Y = 0;
		this->Z = 0;
		this->Invalid = false;
	}
	Point3f(float X, float Y, float Z, bool invalid)
	{
		this->X = X;
		this->Y = Y;
		this->Z = Z;
		this->Invalid = invalid;
	}
	Point3f(float X, float Y, float Z)
	{
		this->X = X;
		this->Y = Y;
		this->Z = Z;
		this->Invalid = false;
	}
	float X;
	float Y;
	float Z;
	bool Invalid = false;
} Point3f;
