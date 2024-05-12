#pragma once
#include "k4a/k4a.h"


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

typedef struct ImageSet
{
	int colorImageWidth;
	int colorImageHeight;

	int depthImageWidth;
	int depthImageHeight;

	k4a_image_t colorImage;
	k4a_image_t colorImageJPEG;
	k4a_image_t depthImage;
	k4a_image_t transformedDepthImage;
	k4a_image_t pointcloud;
	Point3f* pointcloud3f;
	size_t pointcloud3fSize = 0;
	k4a_transformation_t transformation;
	k4a_calibration_t calibration;

	bool jpegConvertedToBGRA = false;
	bool depthConvertedToPointcloud = false;

} ImageSet;
