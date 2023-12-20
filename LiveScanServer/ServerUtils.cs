
//Various enums and helper function
namespace LiveScanServer
{
	//If anyone knows of a clever way to share the enum in both the client and the server, that would be a nice-to-have. A dll?

	//copied from LiveScanClient/utils.h. 
	//Must match INCOMING MESSAGE TYPE
	//The names are copied from this client, so the awkward phrasing on "recieve" for things we are sending is a byproduct of the convenience of copying/pasting the enums to be sure their indices match.
	public enum OutgoingMessageType
	{
		MSG_CAPTURE_SINGLE_FRAME,
		MSG_CALIBRATE,
		MSG_CALIBRATE_CANCEL,
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
	//copied from LiveScanClient/utils.h. 
	//Must match OUTGOING_MESSAGE_TYPE
	public enum IncomingMessageType
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
}
