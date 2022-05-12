#include "frameFileWriterReader.h"
#include <stdio.h>
#include <ctime>
#include <fstream>
#include <assert.h>
//#define _SILENCE_EXPERIMENTAL_FILESYSTEM_DEPRECATION_WARNING //Otherwise VS yells

namespace fs = std::filesystem;


FrameFileWriterReader::FrameFileWriterReader()
{

}

void FrameFileWriterReader::closeFileIfOpened()
{
	if (!m_pFileHandle)
		return;

	log.LogDebug("Closing current .bin file");

	fclose(m_pFileHandle);
	m_pFileHandle = nullptr;
	m_bFileOpenedForReading = false;
	m_bFileOpenedForWriting = false;
}

void FrameFileWriterReader::closeAndDeleteFile()
{
	if (!m_pFileHandle)
		return;

	log.LogDebug("Closing and deleting .bin file: ");
	log.LogDebug(m_sBinFilePath);

	fclose(m_pFileHandle);
	remove(m_sBinFilePath.c_str());

	m_pFileHandle = nullptr;
	m_sBinFilePath = "";
	m_bFileOpenedForReading = false;
	m_bFileOpenedForWriting = false;
}

void FrameFileWriterReader::resetTimer()
{
	recording_start_time = std::chrono::steady_clock::now();
}

int FrameFileWriterReader::getRecordingTimeMilliseconds()
{
	std::chrono::steady_clock::time_point end = std::chrono::steady_clock::now();
	return static_cast<int>(std::chrono::duration_cast<std::chrono::milliseconds >(end - recording_start_time).count());
}

void FrameFileWriterReader::openCurrentBinFileForReading()
{
	closeFileIfOpened();

	log.LogDebug("Opening current bin file for reading");

	m_pFileHandle = fopen(m_sBinFilePath.c_str(), "rb");
	m_bFileOpenedForReading = true;
	m_bFileOpenedForWriting = false;
	m_nCurrentReadFrameID = 0;
}

/// <summary>
/// Opens a .bin recording file from anywhere on disk
/// </summary>
/// <param name="path">The absolute or relative path to the bin file, including the file and file-ending </param>
void FrameFileWriterReader::openNewBinFileForReading(std::string path) {
	
	closeFileIfOpened();

	log.LogDebug("Opening new .bin file for reading at path:");
	log.LogDebug(path);

	m_pFileHandle = fopen(path.c_str(), "rb");

	m_bFileOpenedForReading = true;
	m_bFileOpenedForWriting = false;
	m_nCurrentReadFrameID = 0;
}

/// <summary>
/// Opens a new .bin file for writing in the current recording directoy. Use setRecordingDirPath() to set the recording directory
/// </summary>
/// <param name="deviceID">The device ID, as given by the server</param>
/// <param name="prefix">Optional prefix for the file name</param>
void FrameFileWriterReader::openNewBinFileForWriting(int deviceID, std::string prefix)
{
	closeFileIfOpened();

	char filename[1024];
	time_t t = time(0);
	struct tm* now = localtime(&t);
	sprintf(filename, "recording_%01d_%04d_%02d_%02d_%02d_%02d.bin", deviceID, now->tm_year + 1900, now->tm_mon + 1, now->tm_mday, now->tm_hour, now->tm_min, now->tm_sec);

	m_sBinFilePath = m_sFrameRecordingsDir;

	if (prefix.size() > 0) {
		m_sBinFilePath += prefix + "_";
	}

	m_sBinFilePath += filename;
	m_pFileHandle = fopen(m_sBinFilePath.c_str(), "wb");

	log.LogDebug("Opening new .bin file for writing at path:");
	log.LogDebug(m_sBinFilePath);

	m_bFileOpenedForReading = false;
	m_bFileOpenedForWriting = true;

	resetTimer();
}

/// <summary>
/// Reads the next frame from the opened .bin file. If you need to read a certain frame, first seek to it with seekBinaryReaderToFrame() and the use this function.
/// </summary>
/// <param name="outPoints"> Point buffer to be filled </param>
/// <param name="outColors"> RGB buffer to be filled </param>
/// <param name="outTimestamp"> The timestamp at which the frame was taken. Returns -1 when timestamp could not be retrieved </param>
/// <returns></returns>
bool FrameFileWriterReader::readNextBinaryFrame(Point3s* &outPoints, RGB* &outColors, int &outPointsSize, int &outTimestamp)
{
	log.LogCaptureDebug("Reading next binary frame. Frame number: ");
	log.LogCaptureDebug(std::to_string(m_nCurrentReadFrameID));

	if (!m_bFileOpenedForReading)
		openCurrentBinFileForReading();

	FILE *f = m_pFileHandle;
	int nPoints, timestamp; 
	char tmp[1024]; 
	int nread = fscanf_s(f, "%s %d %s %d", tmp, 1024, &nPoints, tmp, 1024, &timestamp);

	if (nread < 4)
		return false;

	if (nPoints > 0) {

		fgetc(f);		//  '\n'
		outPoints = new Point3s[nPoints];
		outColors = new RGB[nPoints];

		fread((void*)outPoints, sizeof(outPoints[0]), nPoints, f);
		fread((void*)outColors, sizeof(outColors[0]), nPoints, f);
		fgetc(f);		// '\n'
	}

	outPointsSize = nPoints;
	outTimestamp = timestamp;
	m_nCurrentReadFrameID++;
	return true;
}

/// <summary>
/// Append a frame to the openend .bin file
/// </summary>
/// <param name="points"></param>
/// <param name="colors"></param>
/// <param name="timestamp"></param>
/// <param name="deviceID"></param>
/// <returns></returns>
bool FrameFileWriterReader::writeNextBinaryFrame(Point3s* points, int pointsSize, RGB* colors, uint64_t timestamp, int deviceID)
{
	log.LogCaptureDebug("Writing next binary frame with timestamp: ");
	log.LogCaptureDebug(std::to_string(timestamp));

	if (!m_bFileOpenedForWriting)
		openNewBinFileForWriting(deviceID, "");

	FILE *f = m_pFileHandle;
	
	//The Timestamp is generated by the Kinect instead of the system. If temporal Sync is enabled, Master and Subordinate have a synced timestamp
	fprintf(f, "n_points= %d\nframe_timestamp= %d\n", pointsSize, timestamp);

	int wroteCount = 0;

	if (pointsSize > 0)
	{
		wroteCount += fwrite(points, sizeof(points[0]), pointsSize, f);
		wroteCount += fwrite(colors, sizeof(colors[0]), pointsSize, f);
	}

	if (wroteCount != pointsSize * 2)
		return false;

	else
		return true;

	fprintf(f, "\n");
}

/// <summary>
/// Seek to a certain frame in the opened .bin file. 
/// </summary>
/// <param name="frameID"></param>
void FrameFileWriterReader::seekBinaryReaderToFrame(int frameID) {

	log.LogDebug("Seeking .bin reader to frame: ");
	log.LogDebug(std::to_string(frameID));

	if (frameID > m_nCurrentReadFrameID) {

		while (frameID > m_nCurrentReadFrameID) {
			skipOneFrameBinaryReader();
		}
		return;
	}

	else if (frameID == m_nCurrentReadFrameID)
		return;

	//If the desired frame ID is below the current frame ID, we need to search from the start of the file again,
	//because we can't read the file backwards (yet)
	else if (frameID < m_nCurrentReadFrameID) {

		m_nCurrentReadFrameID = 0;
		FILE* f = m_pFileHandle; 
		fseek(f, 0, SEEK_SET); //Reset the file stream reader to the beginning of the file

		while (frameID > m_nCurrentReadFrameID) {
			skipOneFrameBinaryReader();
		}
	}
}

void FrameFileWriterReader::skipOneFrameBinaryReader() {

	FILE* f = m_pFileHandle;
	long start = ftell(f);
	int nPoints, timestamp;
	char tmp[1024];
	int nread = fscanf_s(f, "%s %d %s %d", tmp, 1024, &nPoints, tmp, 1024, &timestamp); //Get the size of nPoints so that we can skip them
	long offsetVerts = nPoints * sizeof(Point3s);
	long offsetColors = nPoints * sizeof(RGB);

	fgetc(f);		//  '\n'
	fseek(f, offsetVerts, SEEK_CUR); //Skip vertices
	fseek(f, offsetColors, SEEK_CUR); //Skip colors
	fgetc(f);		// '\n'
	long end = ftell(f);
	m_nCurrentReadFrameID++;
}


/// <summary>
/// Given a dir in which all clients/server should store their recordings in, creates a client-specific dir.
/// Also creates all parent directorys neccessary.
/// </summary>
/// <returns> Returns true on success, returns false if there are errors during file path creation.</returns>
bool FrameFileWriterReader::CreateRecordDirectory(std::string newDirToCreate, const int deviceID)
{

	fs::path generalOutputPath = fs::current_path();
	generalOutputPath /= "out\\"; //The directory in which all recordings are stored

	if (!CreateDir(generalOutputPath))
	{
		log.LogError("Failed to create recording out directory");
		log.LogError(generalOutputPath.string());
		return false;
	}

	fs::path takeDir = generalOutputPath; 
	takeDir /= newDirToCreate; //The take dir in which the recordings of this take are saved

	if (!CreateDir(takeDir))
	{
		log.LogError("Failed to create Take directory: ");
		log.LogError(takeDir.string());
		return false;
	}

	fs::path clientTakeDir = takeDir;
	std::string deviceIDDir = "client_";
	deviceIDDir += std::to_string(deviceID);
	deviceIDDir += "\\";
	clientTakeDir /= deviceIDDir; //The directory in which we store the recordings of this specific client

	if (!CreateDir(clientTakeDir))
	{
		log.LogError("Failed to create recording directory at path: ");
		log.LogError(clientTakeDir.string());
		return false;

	}

	m_sFrameRecordingsDir = clientTakeDir.string();

	log.LogInfo("Created new recording directory, path: ");
	log.LogInfo(clientTakeDir.string());

	return true;
}

void FrameFileWriterReader::WriteColorJPGFile(void* buffer, size_t bufferSize, int frameIndex, std::string optionalPrefix)
{
	
	std::string colorFileName;
	if (optionalPrefix.size() > 0) {
		colorFileName += optionalPrefix + "_";
	}
	colorFileName += "Color_";
	colorFileName += std::to_string(frameIndex);
	colorFileName += ".jpg";

	std::string filePath = m_sFrameRecordingsDir;	
	filePath += colorFileName;

	log.LogCaptureDebug("Writing Color JPG file: ");
	log.LogCaptureDebug(filePath);

	assert(buffer != NULL);

	std::ofstream hFile;
	hFile.open(filePath.c_str(), std::ios::out | std::ios::trunc | std::ios::binary);
	if (hFile.is_open())
	{
		hFile.write((char*)buffer, static_cast<std::streamsize>(bufferSize));
		hFile.close();
	}
}

void FrameFileWriterReader::WriteCalibrationJSON(int deviceIndex, const std::vector<uint8_t> calibration_buffer, const size_t calibration_size)
{
	std::string filename = m_sFrameRecordingsDir;
	filename += "Intrinsics_Calib_";
	filename += std::to_string(deviceIndex);
	filename += ".json";

	log.LogInfo("Writing Calibration JSON file: ");
	log.LogInfo(filename);

	std::ofstream file(filename, std::ofstream::binary);
	file.write(reinterpret_cast<const char*>(&calibration_buffer[0]), (long)calibration_size);
	file.close();
}

void FrameFileWriterReader::WriteDepthTiffFile(const k4a_image_t &im, int frameIndex, std::string optionalPrefix)
{
	std::string depthFileName;
	if (optionalPrefix.size() > 0) {
		depthFileName += optionalPrefix + "_";
	} 
	
	depthFileName += "Depth_";
	depthFileName += std::to_string(frameIndex);
	depthFileName += ".tiff";

	std::string filePath = m_sFrameRecordingsDir;
	filePath += depthFileName;
	
	log.LogCaptureDebug("Writing Depth tiff file: ");
	log.LogCaptureDebug(filePath);

	cv::Mat depthMat = cv::Mat(k4a_image_get_height_pixels(im), k4a_image_get_width_pixels(im), CV_16U, k4a_image_get_buffer(im), static_cast<size_t>(k4a_image_get_stride_bytes(im)));

	bool result = false;

	try
	{
		cv::imwrite(filePath, depthMat);
	}
	catch (const cv::Exception& ex)
	{
		fprintf(stderr, "Exception converting image to Tiff format: %s\n", ex.what());
	}
}

void FrameFileWriterReader::WriteTimestampLog(std::vector<int> frames, std::vector<uint64_t> timestamps, int deviceIndex) 
{
	std::string filename = m_sFrameRecordingsDir;
	filename += "Timestamps_Client";
	filename += std::to_string(deviceIndex);
	filename += ".txt";

	log.LogInfo("Writing Timestamp log to: ");
	log.LogInfo(filename);

	if (frames.size() < 1)
		return;

	std::ofstream file;
	file.open(filename);
	
	for (int i = 0; i < frames.size(); i++)
	{
		file << frames[i] << "\t" << timestamps[i] << "\n";
	}

	file.close();
}

/// <summary>
/// Renames a raw frame pair (Color JPEG & Depth TIFF File). Changes the index in the filename and optionally adds a prefix 
/// </summary>
/// <param name="oldFrameIndex"></param>
/// <param name="newFrameIndex"></param>
/// <param name="newPrefix"></param>
/// <returns></returns>
bool FrameFileWriterReader::RenameRawFramePair(int oldFrameIndex, int newFrameIndex, std::string newPrefix)
{
	fs::path oldFilePathColor = m_sFrameRecordingsDir + std::string("Color_") + std::to_string(oldFrameIndex) + ".jpg";
	fs::path oldFilePathDepth = m_sFrameRecordingsDir + std::string("Depth_") + std::to_string(oldFrameIndex) + ".tiff";

	fs::path newFilePathColor = m_sFrameRecordingsDir + newPrefix + std::string("Color_") + std::to_string(newFrameIndex) + ".jpg";
	fs::path newFilePathDepth = m_sFrameRecordingsDir + newPrefix + std::string("Depth_") + std::to_string(newFrameIndex) + ".tiff";

	try {
		fs::rename(oldFilePathColor, newFilePathColor);
		fs::rename(oldFilePathDepth, newFilePathDepth);
	}

	catch (const fs::filesystem_error& ex) {
		std::ostringstream ss;
		ss << "Error trying to rename: " << ex.path1() << ex.path2() << ex.what() << std::endl;
		log.LogError(ss.str());
		return false;
	}

	return true;
}

/// <summary>
/// Creates a directory. Should be given an absolute path
/// </summary>
/// <param name="path"></param>
/// <returns>Returns true if the directory exists, false when an error has occured during creation</returns>
bool FrameFileWriterReader::CreateDir(const fs::path dirToCreate)
{
	if (!fs::exists(dirToCreate))
	{
		try
		{
			if (!fs::create_directory(dirToCreate))
				return false; //Could not create dir

			else
				return true;
		}

		catch (fs::filesystem_error const& ex)
		{
			return false; //Error during dir creation
		}
	}

	else
		return true;
}

bool FrameFileWriterReader::DirExists(std::string path) 
{
	fs::path pathToCheck = path;

	return fs::exists(pathToCheck);
}

//Returns a relative path of the current recording directory
std::string FrameFileWriterReader::GetRecordingDirPath() {
	return m_sFrameRecordingsDir;
}

//Returns a relative path of the current .bin file
std::string FrameFileWriterReader::GetBinFilePath() {
	return m_sBinFilePath;
}

//Set the recording directory in which .bin files are stored/read from
void FrameFileWriterReader::SetRecordingDirPath(std::string path) {
	log.LogInfo("Setting the .bin read/write location to: ");
	log.LogInfo(path);
	m_sFrameRecordingsDir = path;
}

FrameFileWriterReader::~FrameFileWriterReader()
{
	closeFileIfOpened();
}
