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
#include <Log.h>

class FrameFileWriterReader
{
public:
    FrameFileWriterReader();
	void openNewBinFileForWriting(int deviceID, std::string prefix);
	void openNewBinFileForReading(std::string path);
	void openCurrentBinFileForReading();
	std::string GetBinFilePath();
	std::string GetRecordingDirPath();
	void SetRecordingDirPath(std::string path);
	bool CreateRecordDirectory(std::string dirToCreate, int deviceID);
	bool DirExists(std::string path);

	bool writeNextBinaryFrame(Point3s* points, int pointsSize, RGB* colors, uint64_t timestamp, int deviceID);
	bool readNextBinaryFrame(Point3s*& outPoints, RGB*& outColors, int& outPointsSize, int& outTimestamp);
	void seekBinaryReaderToFrame(int frameID);
	void skipOneFrameBinaryReader();
	void WriteColorJPGFile(void* buffer, size_t bufferSize, int frameIndex, std::string optionalPrefix);
	void WriteDepthTiffFile(const k4a_image_t& im, int frameIndex, std::string optionalPrefix);
	bool RenameRawFramePair(int oldFrameIndex, int newFrameIndex, std::string optionalPrefix);
	void WriteTimestampLog(std::vector<int> frames, std::vector<uint64_t> timestamps, int deviceIndex);
	void WriteCalibrationJSON(int deviceIndex, const std::vector<uint8_t> calibration_buffer, size_t calibration_size);

	bool openedForWriting() { return m_bFileOpenedForWriting; }
	bool openedForReading() { return m_bFileOpenedForReading; }

	void closeFileIfOpened();
	void closeAndDeleteFile();

    ~FrameFileWriterReader();

private:
	void resetTimer();
	int getRecordingTimeMilliseconds();
	bool CreateDir(const std::filesystem::path dirToCreate);

	FILE *m_pFileHandle = nullptr;
	bool m_bFileOpenedForWriting = false;
	bool m_bFileOpenedForReading = false;
	int m_nCurrentReadFrameID = 0;

	std::string m_sBinFilePath = "";

	std::string m_sFrameRecordingsDir = "";

	std::chrono::steady_clock::time_point recording_start_time;

	Log& log = Log::Get();

};