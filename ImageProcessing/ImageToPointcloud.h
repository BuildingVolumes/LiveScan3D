#pragma once
#include "turbojpeg/turbojpeg.h"
#include "opencv2/opencv.hpp"
#include "types.h"

#ifdef IM2PC_API
#define IM2PC_API __declspec(dllexport)
#else
#define IM2PC_API __declspec(dllimport)
#endif


//++++ Initialization

/// <summary>
/// Before you start using this DLL, always Initialize it
/// </summary>
extern "C" IM2PC_API void InitImageProcessing();

/// <summary>
/// When done using the DLL, or application lifetime ends, close it properly
/// </summary>
extern "C" IM2PC_API void CloseImageProcessing();


//++++ Image and File creation

/// <summary>
/// Create an imageset. Recommended to create one imageset per camera.
/// The data of the ImageSet will live in the unmanaged DLL memory, so that unneccesary copies can be avoided
/// </summary>
/// <returns>Returns a Pointer to the imageset. Keep this pointer until disposing of the set.</returns>
extern "C" IM2PC_API ImageSet* CreateImageSet();

/// <summary>
/// Change the JPEG image in the imageset to a newer one, created from a provided buffer. Note that the buffer won't be copied,
/// only referenced. You need to manage and dispose of the buffer in the runtime where it was created
/// </summary>
/// <param name="imgsetPtr">The pointer to the imageset you want to change the image in</param>
/// <param name="jpegWidth">The width of the JPEG</param>
/// <param name="jpegHeight">The height of the JPEG</param>
/// <param name="jpegBuffer">The pointer to the buffer of the JPEG</param>
/// <param name="jpegSize">The size of the JPEG buffer in bytes</param>
/// <returns></returns>
extern "C" IM2PC_API bool ChangeJPEGFromBuffer(ImageSet* imgsetPtr, int jpegWidth, int jpegHeight, char* jpegBuffer, int jpegSize);

/// <summary>
/// Change the color BGRA32 (4 channels, 1 byte per Channel) image in the imageset to a newer one, created from a provided buffer. Note that the buffer won't be copied,
/// only referenced. You need to manage and dispose of the buffer in the runtime where it was created
/// </summary>
/// <param name="imgsetPtr">The pointer to the imageset you want to change the image in</param>
/// <param name="colorWidth">The width of the color image</param>
/// <param name="colorHeight">The height of the color image</param>
/// <param name="colorBuffer">The BGRA32 color image buffer</param>
/// <returns></returns>
extern "C" IM2PC_API bool ChangeBGRA32FromBuffer(ImageSet* imgsetPtr, int colorWidth, int colorHeight, char* colorBuffer);
extern "C" IM2PC_API bool ChangeDepthFromBuffer(ImageSet* imgsetPtr, int depthWidth, int depthHeight, char* depthBuffer);
extern "C" IM2PC_API bool ChangeCalibrationFromBuffer(ImageSet* imgsetPtr, char* calibrationBuffer, int calibrationSize, char colorRes, char depthMode);

extern "C" IM2PC_API bool JPEG2BGRA32(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool ErodeDepthImageFilter(ImageSet* imgsetPtr, int filterKernelSize);
extern "C" IM2PC_API bool MapDepthToColor(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool GeneratePointcloud(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool PointCloudImageToPoint3f(ImageSet* imgsetPtr);

extern "C" IM2PC_API int GetColorImageWidth(ImageSet * imgsetPtr);
extern "C" IM2PC_API int GetColorImageHeight(ImageSet * imgsetPtr);
extern "C" IM2PC_API int GetColorImageSize(ImageSet * imgsetPtr);
extern "C" IM2PC_API int GetDepthImageWidth(ImageSet * imgsetPtr);
extern "C" IM2PC_API int GetDepthImageSize(ImageSet * imgsetPtr);
extern "C" IM2PC_API int GetPointcloud3fSize(ImageSet * imgsetPtr);

extern "C" IM2PC_API Point3f* GetPointCloudBuffer(ImageSet * imgsetPtr, int pointcloudSize);
extern "C" IM2PC_API uint8_t* GetColorImageBuffer(ImageSet * imgsetPtr, int colorBufferSize);

extern "C" IM2PC_API void DisposeImageSet(ImageSet * imgsetPtr);