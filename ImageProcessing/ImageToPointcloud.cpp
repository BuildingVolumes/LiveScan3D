#include "pch.h"
#include "ImageToPointcloud.h"

static tjhandle turboJpeg;


void InitImageProcessing()
{
	turboJpeg = tjInitDecompress();
}

void CloseImageProcessing()
{
	if (turboJpeg)
		tjDestroy(turboJpeg);
}

extern "C" IM2PC_API ImageSet* CreateImageSetFromJPEG(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize)
{
	if (jpegWidth < 1 || jpegHeight < 1 || jpegBuffer == NULL || jpegSize < 1)
		return NULL;

	ImageSet* setPtr = new ImageSet(jpegWidth, jpegHeight, jpegBuffer, jpegSize);
	return setPtr;
}

extern "C" IM2PC_API ImageSet* CreateImageSetFromJPEGAndDepth(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize, int depthWidth, int depthHeight, char* depthBuffer, int depthSize, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode)
{
	if (jpegWidth < 1 || jpegHeight < 1 || jpegBuffer == NULL || jpegSize < 1)
		return NULL;

	if (depthWidth < 1 || depthHeight < 1 || depthBuffer == NULL || depthSize < 1 || depthSize != depthWidth * depthHeight * sizeof(int16_t))
		return NULL;

	if (calibrationBuffer == NULL || calibrationSize < 1)
		return NULL;

	ImageSet* setPtr = new ImageSet(jpegWidth, jpegHeight, jpegBuffer, jpegSize, depthWidth, depthHeight, depthBuffer, depthSize, calibrationBuffer, calibrationSize, colorRes, depthMode);
	return setPtr;
}

extern "C" IM2PC_API ImageSet * CreateImageSetFromBGRA32AndDepth(int colorWidth, int colorHeight, char* BGRA32Buffer, int BGRA32Size, int depthWidth, int depthHeight, char* depthBuffer, int depthSize, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode)
{
	if (colorWidth < 1 || colorHeight < 1 || BGRA32Buffer == NULL || BGRA32Size < 1 || colorWidth * colorHeight * sizeof(int32_t))
		return NULL;

	if (depthWidth < 1 || depthHeight < 1 || depthBuffer == NULL || depthSize < 1 || depthSize != depthWidth * depthHeight * sizeof(int16_t))
		return NULL;

	if (calibrationBuffer == NULL || calibrationSize < 1)
		return NULL;

	ImageSet* setPtr = new ImageSet(colorWidth, colorHeight, BGRA32Buffer, BGRA32Size, depthWidth, depthHeight, depthBuffer, depthSize, calibrationBuffer, calibrationSize, colorRes, depthMode);
	return setPtr;
}



extern "C" IM2PC_API bool CreatePointcloudFromImages(ImageSet* imgsetPtr)
{
}

/// <summary>
/// Decompresses a raw MJPEG image to a BGRA cvMat using TurboJpeg
/// </summary>
extern "C" IM2PC_API bool JPEG2BGRA32(ImageSet* imgsetPtr)
{
	if (imgsetPtr->colorImageJPEG == NULL)
		return false;
	if (k4a_image_get_buffer(imgsetPtr->colorImage) == NULL)
		return false;

	int jpegSize = k4a_image_get_size(imgsetPtr->colorImageJPEG);
	int jpegWidth = k4a_image_get_width_pixels(imgsetPtr->colorImageJPEG);
	int jpegHeight = k4a_image_get_height_pixels(imgsetPtr->colorImageJPEG);
	uint8_t* jpegBuffer = k4a_image_get_buffer(imgsetPtr->colorImageJPEG);
	
	if (imgsetPtr->colorImage == NULL || k4a_image_get_width_pixels(imgsetPtr->colorImage) != jpegWidth || k4a_image_get_height_pixels(imgsetPtr->colorImage) != jpegHeight)
		k4a_image_create(K4A_IMAGE_FORMAT_COLOR_BGRA32, jpegWidth, jpegHeight, jpegWidth * 4, &imgsetPtr->colorImage);

	if (tjDecompress2(turboJpeg, jpegBuffer, jpegSize, k4a_image_get_buffer(imgsetPtr->colorImage), jpegWidth, 0, jpegHeight, TJPF_BGRA, TJFLAG_FASTDCT | TJFLAG_FASTUPSAMPLE))
		return false;
	else
	{
		imgsetPtr->jpegConvertedToBGRA = true;
		return true;
	}
}

extern "C" IM2PC_API bool ErodeDepthImageFilter(ImageSet* imgsetPtr, int filterKernelSize)
{
	if (imgsetPtr->depthImage == NULL)
		return false;
	if (k4a_image_get_buffer(imgsetPtr->depthImage) == NULL)
		return false;

	int depthWidth = k4a_image_get_width_pixels(imgsetPtr->colorImageJPEG);
	int depthHeight = k4a_image_get_height_pixels(imgsetPtr->colorImageJPEG);
	uint8_t* depthBuffer = k4a_image_get_buffer(imgsetPtr->colorImageJPEG);

	cv::Mat cImgD = cv::Mat(depthHeight, depthWidth, CV_16UC1, depthBuffer);
	cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(filterKernelSize, filterKernelSize));
	int CLOSING = 3;
	cv::erode(cImgD, cImgD, kernel);
}

extern "C" IM2PC_API bool MapDepthToColor(ImageSet* imgsetPtr)
{
	if (imgsetPtr->colorImage == NULL || imgsetPtr->depthImage == NULL)
		return false;
	if (k4a_image_get_buffer(imgsetPtr->colorImage) == NULL || k4a_image_get_buffer(imgsetPtr->depthImage) == NULL)
		return false;

	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	if (imgsetPtr->transformedDepthImage == NULL || k4a_image_get_width_pixels(imgsetPtr->transformedDepthImage) != colorWidth || k4a_image_get_height_pixels(imgsetPtr->transformedDepthImage) != colorHeight)
	{
		if (k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, colorWidth, colorHeight, colorWidth * (int)sizeof(uint16_t), &imgsetPtr->transformedDepthImage) == K4A_RESULT_FAILED)
			return false;
	}

	if (k4a_transformation_depth_image_to_color_camera(imgsetPtr->transformation, imgsetPtr->depthImage, imgsetPtr->transformedDepthImage) == K4A_RESULT_FAILED)
		return false;
	else
	{
		imgsetPtr->depthMappedToColor = true;
		return true;
	}

}

/// <summary>
/// Creates a Pointcloud out of the transformedDepthImage and saves it in PointcloudImage. Make sure to run MapDepthToColor before calling this function
/// </summary>
extern "C" IM2PC_API bool GeneratePointcloud(ImageSet* imgsetPtr)
{
	if (imgsetPtr->colorImage == NULL || imgsetPtr->transformedDepthImage == NULL)
		return false;
	if (k4a_image_get_buffer(imgsetPtr->colorImage) == NULL || k4a_image_get_buffer(imgsetPtr->transformedDepthImage) == NULL)
		return false;

	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	if (imgsetPtr->pointcloud == NULL || k4a_image_get_width_pixels(imgsetPtr->pointcloud) != colorWidth || k4a_image_get_height_pixels(imgsetPtr->pointcloud) != colorHeight)
	{
		if (k4a_image_create(K4A_IMAGE_FORMAT_CUSTOM, colorWidth, colorHeight, colorWidth * 3 * (int)sizeof(int16_t), &imgsetPtr->pointcloud) == K4A_RESULT_FAILED)
			return false;
	}

	if (k4a_transformation_depth_image_to_point_cloud(imgsetPtr->transformation, imgsetPtr->transformedDepthImage, K4A_CALIBRATION_TYPE_COLOR, imgsetPtr->pointcloud))
		return false;
	else
	{
		imgsetPtr->depthConvertedToPointcloud = true;
		return true;

	}
}


///<summary>
///Translates the k4a_image_t pointcloud into a easier to handle Point3f array. Make sure to run MapDepthToColorFrame & GeneratePointcloud before calling this function
///</summary>
///<param name="pCameraSpacePoints"></param>
extern "C" IM2PC_API bool PointCloudImageToPoint3f(ImageSet* imgsetPtr)
{
	if (imgsetPtr->pointcloud == NULL)
		return false;

	if (k4a_image_get_buffer(imgsetPtr->pointcloud) == NULL)
		return false;

	if (imgsetPtr->pointcloud3f != NULL)
		delete[] imgsetPtr->pointcloud3f;

	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	Point3f* pCameraSpacePoints = new Point3f[colorWidth * colorHeight];
	int16_t * pointCloudData = (int16_t*)k4a_image_get_buffer(imgsetPtr->pointcloud);

	for (int i = 0; i < colorHeight; i++)
	{
		for (int j = 0; j < colorWidth; j++)
		{
			pCameraSpacePoints[j + i * colorWidth].X = pointCloudData[3 * (j + i * colorWidth) + 0] / 1000.0f;
			pCameraSpacePoints[j + i * colorWidth].Y = pointCloudData[3 * (j + i * colorWidth) + 1] / 1000.0f;
			pCameraSpacePoints[j + i * colorWidth].Z = pointCloudData[3 * (j + i * colorWidth) + 2] / 1000.0f;
		}
	}

	imgsetPtr->pointcloudConvertedToPoint3f = true;
	return true;
}

extern "C" IM2PC_API bool DisposeImageSet(ImageSet * imgsetPtr)
{
	if (imgsetPtr->colorImageJPEG != NULL)
	{
		k4a_image_release(imgsetPtr->colorImageJPEG);
		imgsetPtr->colorImageJPEG = NULL;
	}

	if (imgsetPtr->colorImage != NULL)
	{
		k4a_image_release(imgsetPtr->colorImage);
		imgsetPtr->colorImage = NULL;
	}

	if (imgsetPtr->depthImage != NULL)
	{
		k4a_image_release(imgsetPtr->depthImage);
		imgsetPtr->depthImage = NULL;
	}

	if (imgsetPtr->transformedDepthImage != NULL)
	{
		k4a_image_release(imgsetPtr->transformedDepthImage);
		imgsetPtr->transformedDepthImage = NULL;
	}

	if (imgsetPtr->pointcloud != NULL)
	{
		k4a_image_release(imgsetPtr->pointcloud);
		imgsetPtr->pointcloud = NULL;
	}

	delete[] imgsetPtr->pointcloud3f;
	k4a_transformation_destroy(imgsetPtr->transformation);

	delete imgsetPtr;
	imgsetPtr = NULL;
}
