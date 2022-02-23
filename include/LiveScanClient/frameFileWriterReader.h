#pragma once

#include <stdio.h>
#include <string>
#include <vector>
#include <chrono>
#include "utils.h"
#include <k4a/k4a.h>
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/opencv.hpp>
#include <filesystem>

class FrameFileWriterReader
{
public:
    FrameFileWriterReader();
	void openNewFileForWriting(int deviceID);
	void openCurrentFileForReading();

	// leave filename blank if you want the filename to be generated from the date
	void setCurrentFilename(std::string filename = "");

	void writeFrame(Point3s* points, int pointsSize, RGB* colors, uint64_t timestamp, int deviceID);
	bool CreateRecordDirectory(std::string dirToCreate, int deviceID);
	bool DirExists(std::string path);
	void WriteColorJPGFile(void* buffer, size_t bufferSize, int frameIndex);
	void WriteDepthTiffFile(const k4a_image_t& im, int frameIndex);
	void WriteTimestampLog(std::vector<int> frames, std::vector<uint64_t> timestamps, int deviceIndex);
	void WriteTimestampLog();
	void WriteCalibrationJSON(int deviceIndex, const std::vector<uint8_t> calibration_buffer, size_t calibration_size);
	bool readFrame(Point3s* &outPoints, RGB* &outColors, int& outPointsSize);

	bool openedForWriting() { return m_bFileOpenedForWriting; }
	bool openedForReading() { return m_bFileOpenedForReading; }


	void closeFileIfOpened();

    ~FrameFileWriterReader();

private:
	void resetTimer();
	int getRecordingTimeMilliseconds();
	bool CreateDir(const std::filesystem::path dirToCreate);

	FILE *m_pFileHandle = nullptr;
	bool m_bFileOpenedForWriting = false;
	bool m_bFileOpenedForReading = false;

	std::string m_sFilename = "";

	std::string m_sFrameRecordingsDir = "";

	std::chrono::steady_clock::time_point recording_start_time;

};