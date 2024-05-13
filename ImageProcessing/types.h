#pragma once
#include "k4a/k4a.h"
#include "opencv2/opencv.hpp"

namespace ImageProcessing
{
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

	typedef struct Point3s
	{
		Point3s()
		{
			this->X = 0;
			this->Y = 0;
			this->Z = 0;
		}
		Point3s(short X, short Y, short Z)
		{
			this->X = X;
			this->Y = Y;
			this->Z = Z;
		}
		//meters to milimeters
		Point3s(Point3f& other)
		{
			this->X = static_cast<short>(1000 * other.X);
			this->Y = static_cast<short>(1000 * other.Y);
			this->Z = static_cast<short>(1000 * other.Z);
		}
		short X;
		short Y;
		short Z;
	} Point3s;

	typedef struct Matrix4x4
	{
	public:

		float mat[4][4];

		Matrix4x4()
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					mat[i][j] = 0;
				}
			}
		}

		Matrix4x4(float* floatArray)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int index = (i * 4) + j;

					mat[i][j] = floatArray[i];
				}
			}
		}

		Matrix4x4
		(
			float m00, float m01, float m02, float m03,
			float m10, float m11, float m12, float m13,
			float m20, float m21, float m22, float m23,
			float m30, float m31, float m32, float m33)
		{
			mat[0][0] = m00, mat[0][1] = m01, mat[0][2] = m02, mat[0][3] = m03,
				mat[1][0] = m10, mat[1][1] = m11, mat[1][2] = m12, mat[1][3] = m13,
				mat[2][0] = m20, mat[2][1] = m21, mat[2][2] = m22, mat[2][3] = m23,
				mat[3][0] = m30, mat[3][1] = m31, mat[3][2] = m32, mat[3][3] = m33;
		}

		//Overload multiply operator to allow matrix multiplication
		Matrix4x4 operator*(const Matrix4x4& rhs)
		{
			Matrix4x4 resultM = Matrix4x4();

			for (int i = 0; i < 4; i++)
			{
				for (int k = 0; k < 4; k++)
				{
					for (int j = 0; j < 4; j++)
					{
						resultM.mat[i][j] += this->mat[i][k] * rhs.mat[k][j];
					}
				}
			}

			return resultM;
		}

		//Overload multiply operator to allow multiplication with vectors/points
		friend Point3f operator*(const Matrix4x4& lhs, const Point3f& v)
		{
			Point3f output = Point3f(0, 0, 0);

			output.X += lhs.mat[0][0] * v.X + lhs.mat[0][1] * v.Y + lhs.mat[0][2] * v.Z + lhs.mat[0][3];
			output.Y += lhs.mat[1][0] * v.X + lhs.mat[1][1] * v.Y + lhs.mat[1][2] * v.Z + lhs.mat[1][3];
			output.Z += lhs.mat[2][0] * v.X + lhs.mat[2][1] * v.Y + lhs.mat[2][2] * v.Z + lhs.mat[2][3];
			return output;
		}

		friend Point3f operator*(const Point3f& v, const Matrix4x4& rhs)
		{
			return rhs * v;
		}
	} Matrix4x4;

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
		Point3s pointcloudMinified;
		cv::Mat colorMinified;
		size_t pointcloud3fSize = 0;
		k4a_transformation_t transformation;
		k4a_calibration_t calibration;

		bool jpegConvertedToBGRA = false;
		bool depthConvertedToPointcloud = false;

	} ImageSet;

}

