#include "pch.h"
#include "ImageToPointcloud.h"

static tjhandle turboJpeg;

struct ImageSet
{
	k4a_image_t colorImage;
	k4a_image_t colorImageJPEG;
	k4a_image_t depthImage;
	k4a_image_t transformedDepthImage;
	k4a_image_t pointcloud;
	Point3f* pointcloud3f;	
	k4a_transformation_t transformation;
	k4a_calibration_t calibration;

	ImageSet(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize)
	{
		k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, jpegWidth, jpegHeight, 0, reinterpret_cast<uint8_t*>(jpegBuffer), jpegSize, NULL, NULL, &colorImageJPEG);
	}

	ImageSet(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize, int depthWidth, int depthHeight, char* depthBuffer, int depthSize, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode)
	{
		k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, jpegWidth, jpegHeight, 0, reinterpret_cast<uint8_t*>(jpegBuffer), jpegSize, NULL, NULL, &colorImageJPEG);
		k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_DEPTH16, depthWidth, depthHeight, 0, reinterpret_cast<uint8_t*>(depthBuffer), depthSize, NULL, NULL, &depthImage);
		k4a_calibration_get_from_raw(calibrationBuffer, calibrationSize, k4a_depth_mode_t(depthMode), k4a_color_resolution_t(colorRes), &calibration);
		k4a_transformation_create(&calibration);
	}
};

void InitImageProcessing()
{
	turboJpeg = tjInitDecompress();
}

void CloseImageProcessing()
{
	if (turboJpeg)
		tjDestroy(turboJpeg);
}

ImageSet* CreateColorImageSet(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize)
{
	ImageSet* setPtr = new ImageSet(jpegWidth, jpegHeight, jpegBuffer, jpegSize);
	return setPtr;
}

ImageSet* CreateColorDepthImageSet(int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize, int depthWidth, int depthHeight, char* depthBuffer, int depthSize, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode)
{
	ImageSet* setPtr = new ImageSet(jpegWidth, jpegHeight, jpegBuffer, jpegSize, depthWidth, depthHeight, depthBuffer, depthSize, calibrationBuffer, calibrationSize, colorRes, depthMode);
}

/// <summary>
/// Decompresses a raw MJPEG image to a BGRA cvMat using TurboJpeg
/// </summary>
bool JPEG2BGRA(ImageSet* imgsetPtr)
{
	int jpegSize = k4a_image_get_size(imgsetPtr->colorImageJPEG);
	int jpegWidth = k4a_image_get_width_pixels(imgsetPtr->colorImageJPEG);
	int jpegHeight = k4a_image_get_height_pixels(imgsetPtr->colorImageJPEG);
	uint8_t* jpegBuffer = k4a_image_get_buffer(imgsetPtr->colorImageJPEG);
	
	if (imgsetPtr->colorImage == NULL || k4a_image_get_width_pixels(imgsetPtr->colorImage) != jpegWidth || k4a_image_get_height_pixels(imgsetPtr->colorImage) != jpegHeight)
		k4a_image_create(K4A_IMAGE_FORMAT_COLOR_BGRA32, jpegWidth, jpegHeight, jpegWidth * 4, &imgsetPtr->colorImage);

	tjDecompress2(turboJpeg, jpegBuffer, jpegSize, k4a_image_get_buffer(imgsetPtr->colorImage), jpegWidth, 0, jpegHeight, TJPF_BGRA, TJFLAG_FASTDCT | TJFLAG_FASTUPSAMPLE);
}

void ErodeDepthImageFilter(ImageSet* imgsetPtr, int filterKernelSize)
{
	int depthWidth = k4a_image_get_width_pixels(imgsetPtr->colorImageJPEG);
	int depthHeight = k4a_image_get_height_pixels(imgsetPtr->colorImageJPEG);
	uint8_t* depthBuffer = k4a_image_get_buffer(imgsetPtr->colorImageJPEG);

	cv::Mat cImgD = cv::Mat(depthHeight, depthWidth, CV_16UC1, depthBuffer);
	cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(filterKernelSize, filterKernelSize));
	int CLOSING = 3;
	cv::erode(cImgD, cImgD, kernel);
}

void MapDepthToColor(ImageSet* imgsetPtr)
{
	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	if (imgsetPtr->transformedDepthImage == NULL || k4a_image_get_width_pixels(imgsetPtr->transformedDepthImage) != colorWidth || k4a_image_get_height_pixels(imgsetPtr->transformedDepthImage) != colorHeight)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, colorWidth, colorHeight, colorWidth * (int)sizeof(uint16_t), imgsetPtr->&transformedDepthImage);
	}

	k4a_result_t res = k4a_transformation_depth_image_to_color_camera(imgsetPtr->transformation, imgsetPtr->depthImage, imgsetPtr->transformedDepthImage);

}

/// <summary>
/// Creates a Pointcloud out of the transformedDepthImage and saves it in PointcloudImage. Make sure to run MapDepthToColor before calling this function
/// </summary>
void GeneratePointcloud(ImageSet* imgsetPtr)
{
	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	if (imgsetPtr->pointcloud == NULL || k4a_image_get_width_pixels(imgsetPtr->pointcloud) != colorWidth || k4a_image_get_height_pixels(imgsetPtr->pointcloud) != colorHeight)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_CUSTOM, colorWidth, colorHeight, colorWidth * 3 * (int)sizeof(int16_t), &imgsetPtr->pointcloud);

		if (imgsetPtr->pointcloud3f != NULL)
			delete[] imgsetPtr->pointcloud3f;
	}

	k4a_transformation_depth_image_to_point_cloud(imgsetPtr->transformation, imgsetPtr->transformedDepthImage, K4A_CALIBRATION_TYPE_COLOR, imgsetPtr->pointcloud);
}


///<summary>
///Translates the k4a_image_t pointcloud into a easier to handle Point3f array. Make sure to run MapDepthToColorFrame & GeneratePointcloud before calling this function
///</summary>
///<param name="pCameraSpacePoints"></param>
void PointCloudImageToPoint3f(ImageSet* imgsetPtr)
{
	int16_t* pointCloudData = (int16_t*)k4a_image_get_buffer(imgsetPtr->pointcloud);

	int colorWidth = k4a_image_get_width_pixels(imgsetPtr->colorImage);
	int colorHeight = k4a_image_get_height_pixels(imgsetPtr->colorImage);

	for (int i = 0; i < colorHeight; i++)
	{
		for (int j = 0; j < colorWidth; j++)
		{
			pCameraSpacePoints[j + i * nColorFrameWidth].X = pointCloudData[3 * (j + i * nColorFrameWidth) + 0] / 1000.0f;
			pCameraSpacePoints[j + i * nColorFrameWidth].Y = pointCloudData[3 * (j + i * nColorFrameWidth) + 1] / 1000.0f;
			pCameraSpacePoints[j + i * nColorFrameWidth].Z = pointCloudData[3 * (j + i * nColorFrameWidth) + 2] / 1000.0f;
		}
	}
}
