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
	k4a_image_t colorImage;
	k4a_image_t colorImageJPEG;
	k4a_image_t depthImage;
	k4a_image_t transformedDepthImage;
	k4a_image_t pointcloud;
	Point3f* pointcloud3f;
	k4a_transformation_t transformation;
	k4a_calibration_t calibration;

	bool jpegConvertedToBGRA = false;
	bool depthMappedToColor = false;
	bool depthConvertedToPointcloud = false;
	bool pointcloudConvertedToPoint3f = false;

	ImageSet(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize)
	{
		k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, jpegWidth, jpegHeight, 0, reinterpret_cast<uint8_t*>(jpegBuffer), jpegSize, NULL, NULL, &colorImageJPEG);
	}

	ImageSet(int colorWidth, int colorHeight, char* colorBuffer, int colorSize, bool colorIsJPEG, int depthWidth, int depthHeight, char* depthBuffer, int depthSize, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode)
	{
		if (colorIsJPEG)
			k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, colorWidth, colorHeight, 0, reinterpret_cast<uint8_t*>(colorBuffer), colorSize, NULL, NULL, &colorImageJPEG);

		else
			k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_BGRA32, colorWidth, colorHeight, colorWidth * 4 * sizeof(uint8_t), reinterpret_cast<uint8_t*>(colorBuffer), colorSize, NULL, NULL, &colorImage);

		k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_DEPTH16, depthWidth, depthHeight, 0, reinterpret_cast<uint8_t*>(depthBuffer), depthSize, NULL, NULL, &depthImage);
		k4a_calibration_get_from_raw(calibrationBuffer, calibrationSize, k4a_depth_mode_t(depthMode), k4a_color_resolution_t(colorRes), &calibration);
		k4a_transformation_create(&calibration);
	}

} ImageSet;
