
//Various enums and helper function
namespace KinectServer
{
	//If anyone knows of a clever way to share the enum in both the client and the server, that would be a nice-to-have. A dll?

	//copied from LiveScanClient/utils.h. 
	//Must match INCOMING MESSAGE TYPE
	//The names are copied from this client, so the awkward phrasing on "recieve" for things we are sending is a byproduct of the convenience of copying/pasting the enums to be sure their indices match.
	public enum OutgoingMessageType
	{
		MSG_CAPTURE_SINGLE_FRAME,
		MSG_CALIBRATE,
		MSG_RECEIVE_SETTINGS,
		MSG_REQUEST_STORED_FRAME,
		MSG_REQUEST_LAST_FRAME,
		MSG_RECEIVE_CALIBRATION,
		MSG_CLEAR_STORED_FRAMES,
		MSG_CLOSE_CAMERA,
		MSG_INIT_CAMERA,
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

	public struct DepthModeConfiguration
	{
		public string depthModeName;
		public string depthModeDetails;
		public byte value;//The value that gets sent across the network. It must align with the index of the enum documented in the kinect SDK here: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/group___enumerations_ga3507ee60c1ffe1909096e2080dd2a05d.html
		public override string ToString()
		{
			return depthModeName;
		}
		public static DepthModeConfiguration[] DefaultDepthModes = {

			new DepthModeConfiguration()
			{
				depthModeName = "1024 WFOV Unbinned",
				depthModeDetails = "Depth captured at 1024x1024. Passive IR is also captured at 1024x1024.",
				value = 4
			},
			new DepthModeConfiguration()
			{
				depthModeName = "512 WFOV 2x2 Binned",
				depthModeDetails = "Depth captured at 512x512. Passive IR is also captured at 512x512.",
				value = 3
			},new DepthModeConfiguration()
			{
				depthModeName = "640 NFOV Unbinned",
				depthModeDetails = "Depth captured at 640x576. Passive IR is also captured at 640x576.",
				value = 2
			},new DepthModeConfiguration()
			{
				depthModeName = "320 NFOV 2x2 Binned",
				depthModeDetails = "Depth captured at 320x288. Passive IR is also captured at 320x288.",
				value = 1
			}
		};
	}
	
}
