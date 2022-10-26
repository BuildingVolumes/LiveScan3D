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
#include "utils.h"

Matrix4x4 Matrix4x4::GetIdentity()
{
	Matrix4x4 identity = Matrix4x4();
	identity.mat[0][0] = 1;
	identity.mat[1][1] = 1;
	identity.mat[2][2] = 1;
	identity.mat[3][3] = 1;
	return identity;
}

Matrix4x4 Matrix4x4::Inverse()
{
	Matrix4x4 inverse = Matrix4x4::GetIdentity();

	float det = GetDeterminant();
	if (det == 0)
		return inverse;

	Matrix4x4 adj = GetAdjoint();
	Matrix4x4 transpose = adj.GetTranspose();

	for (int i = 0; i < 4; i++)
		for (int j = 0; j < 4; j++)
			inverse.mat[i][j] = transpose.mat[i][j] / det;

	return inverse;
}

Matrix4x4 Matrix4x4::GetR()
{
	Matrix4x4 r = Matrix4x4::GetIdentity();

	for (size_t i = 0; i < 3; i++)
	{
		for (size_t j = 0; j < 3; j++)
		{
			r.mat[i][j] = mat[i][j];
		}
	}

	return r;
}

void Matrix4x4::SetR(Matrix4x4 r)
{
	for (size_t i = 0; i < 3; i++)
	{
		for (size_t j = 0; j < 3; j++)
		{
			mat[i][j] = r.mat[i][j];
		}
	}
}

Matrix4x4 Matrix4x4::GetT()
{
	Matrix4x4 t = Matrix4x4::GetIdentity();

	for (size_t i = 0; i < 4; i++)
	{		
		t.mat[i][3] = mat[i][3];
	}

	return t;
}

void Matrix4x4::SetT(Matrix4x4 t)
{
	for (size_t i = 0; i < 4; i++)
	{
		mat[i][3] = t.mat[i][3];
	}
}

Matrix4x4 Matrix4x4::GetTranspose()
{
	Matrix4x4 transpose = Matrix4x4::GetIdentity();

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			transpose.mat[j][i] = mat[i][j];
		}
	}

	return transpose;
}

Matrix4x4 Matrix4x4::GetCofactor(int posRow, int posCol, int matSize)
{
	Matrix4x4 cofactorMat = Matrix4x4::GetIdentity();

	int i = 0;
	int j = 0;

	for (int row = 0; row < matSize; row++)
	{
		for (int col = 0; col < matSize; col++)
		{
			if (row != posRow && col != posCol)
			{
				cofactorMat.mat[i][j] = mat[row][col];

				j++;
				if (j == matSize - 1)
				{
					i++;
					j = 0;
				}
			}
		}
	}

	return cofactorMat;
}

float Matrix4x4::GetDeterminant(int matSize)
{
	float determinant = 0;

	int recursionDepth = 4 - matSize;

	if (matSize == 1)
		return mat[0][0];

	Matrix4x4 tempMat = Matrix4x4::GetIdentity();

	int sign = 1;

	for (int f = 0; f < matSize; f++)
	{
		tempMat = GetCofactor(0, f, matSize);
		determinant += sign * mat[0][f]	* tempMat.GetDeterminant(matSize - 1);
		sign = -sign;
	}

	return determinant;
}

float Matrix4x4::GetDeterminant()
{
	return GetDeterminant(4);
}

Matrix4x4 Matrix4x4::GetAdjoint()
{
	Matrix4x4 adj = Matrix4x4::GetIdentity();
	Matrix4x4 tempMat = Matrix4x4::GetIdentity();
	int sign = 1;

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			tempMat = GetCofactor(i, j, 4);

			sign = ((i + j) % 2 == 0) ? 1 : -1; //Switch signs every other value

			adj.mat[i][j] = sign * tempMat.GetDeterminant(3);
		}
	}

	return adj;
}

