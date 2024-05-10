#pragma once
#include "turbojpeg.h"
#include "opencv2/opencv.hpp"
#include "types.h"

#ifdef IM2PC_API
#define IM2PC_API __declspec(dllexport)
#else
#define IM2PC_API __declspec(dllimport)
#endif



extern "C" IM2PC_API void InitImageProcessing();
extern "C" IM2PC_API void CloseImageProcessing();


extern "C" IM2PC_API void SwapJPEG(ImageSet * imgSetPtr, int jpegWidth);
extern "C" IM2PC_API bool JPEG2BGRA32(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool ErodeDepthImageFilter(ImageSet* imgsetPtr, int filterKernelSize);
extern "C" IM2PC_API bool MapDepthToColor(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool GeneratePointcloud(ImageSet* imgsetPtr);
extern "C" IM2PC_API bool PointCloudImageToPoint3f(ImageSet* imgsetPtr);