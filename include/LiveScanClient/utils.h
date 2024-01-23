//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
//   License: MIT Software License   See LICENSE.txt for the full license.

//   If you use this software in your research, then please use the following citation:

//    Kowalski, M.; Naruniec, J.; Daniluk, M.: "LiveScan3D: A Fast and Inexpensive 3D Data
//    Acquisition System for Multiple Kinect v2 Sensors". in 3D Vision (3DV), 2015 International Conference on, Lyon, France, 2015

//    @INPROCEEDINGS{Kowalski15,
//        author={Kowalski, M. and Naruniec, J. and Daniluk, M.},
//        booktitle={3D Vision (3DV), 2015 International Conference on},
//        title={LiveScan3D: A Fast and Inexpensive 3D Data Acquisition System for Multiple Kinect v2 Sensors},
//        year={2015},
//    }
#pragma once
#include <stdint.h>
#include <string>

//Do not change order of enums, they are referenced by index in KinectSocket.cs on the server.
enum INCOMING_MESSAGE_TYPE
{
	MSG_CAPTURE_SINGLE_FRAME,
	MSG_CALIBRATE,
	MSG_CANCEL_CALIBRATION,
	MSG_RECEIVE_SETTINGS,
	MSG_REQUEST_STORED_FRAME,
	MSG_REQUEST_LAST_FRAME,
	MSG_RECEIVE_CALIBRATION,
	MSG_CLEAR_STORED_FRAMES,
	MSG_CLOSE_CAMERA,
	MSG_START_CAMERA,
	MSG_SET_CONFIGURATION,
	MSG_REQUEST_CONFIGURATION,
	MSG_CREATE_DIR,
	MSG_PRE_RECORD_PROCESS_START,
	MSG_POST_RECORD_PROCESS_START,
	MSG_START_CAPTURING_FRAMES,
	MSG_STOP_CAPTURING_FRAMES,
	MSG_REQUEST_TIMESTAMP_LIST,
	MSG_RECEIVE_POSTSYNC_LIST
};

enum OUTGOING_MESSAGE_TYPE
{
	MSG_CONFIRM_CAPTURED,
	MSG_CONFIRM_CALIBRATED,
	MSG_STORED_FRAME,
	MSG_LAST_FRAME,
	MSG_CONFIGURATION,
	MSG_CONFIRM_CAMERA_CLOSED,
	MSG_CONFIRM_CAMERA_INIT,
	MSG_CONFIRM_DIR_CREATION,
	MSG_SEND_TIMESTAMP_LIST,
	MSG_CONFIRM_POSTSYNCED,
	MSG_CONFIRM_POST_RECORD_PROCESS,
	MSG_CONFIRM_PRE_RECORD_PROCESS
};

enum SYNC_STATE
{
	Main,
	Subordinate,
	Standalone,
	UnknownState
};

enum CAPTURE_MODE
{
	CM_POINTCLOUD,
	CM_RAW
};

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

typedef struct Point2f
{
	Point2f()
	{
		this->X = 0;
		this->Y = 0;
	}
	Point2f(float X, float Y)
	{
		this->X = X;
		this->Y = Y;
	}
	float X;
	float Y;
} Point2f;

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
		Point3f output = Point3f(0,0,0);

		output.X += lhs.mat[0][0] * v.X + lhs.mat[0][1] * v.Y + lhs.mat[0][2] * v.Z + lhs.mat[0][3];
		output.Y += lhs.mat[1][0] * v.X + lhs.mat[1][1] * v.Y + lhs.mat[1][2] * v.Z + lhs.mat[1][3];
		output.Z += lhs.mat[2][0] * v.X + lhs.mat[2][1] * v.Y + lhs.mat[2][2] * v.Z + lhs.mat[2][3];
		return output;
	}

	friend Point3f operator*(const Point3f& v, const Matrix4x4& rhs)
	{
		return rhs * v;
	}

	static Matrix4x4 GetIdentity();
	Matrix4x4 Inverse();
	Matrix4x4 GetTranspose();
	Matrix4x4 GetR();
	void SetR(Matrix4x4 r);
	Matrix4x4 GetT();
	void SetT(Matrix4x4 t);
	
private:

	Matrix4x4 GetCofactor(int posCol, int posRow, int matSize);
	Matrix4x4 GetAdjoint();
	float GetDeterminant(int matSize);
	float GetDeterminant();

};

typedef struct RGBA
{
	uint8_t blue;
	uint8_t green;
	uint8_t red;
	uint8_t alpha;
};

typedef struct PreviewFrame
{
	RGBA* picture;
	int width;
	int height;
	bool previewDisabled = true;
};

