using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 處理影像來源為「影片」的類別
	/// </summary>
	public class VideoSource : IInputSource
	{
		VideoCapture videoCapture;
		Mat frame, next_frame;
		public bool EndOfVideo { get; protected set; }

		public VideoSource(string path)
		{
			videoCapture = new VideoCapture(path);
			next_frame = new Mat();
			videoCapture.Read(next_frame);
			EndOfVideo = false;
		}

		public Bitmap GetInputFrame()
		{
			if (next_frame.Cols + next_frame.Rows == 0) 
				throw new System.ArgumentException("Reach the end of video");
			frame = next_frame.Clone();
			videoCapture.Read(next_frame);
			if (next_frame.Cols + next_frame.Rows == 0)
				EndOfVideo = true;

			return frame.ToBitmap();
		}

		public void Replay()
		{
			videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
			videoCapture.Read(next_frame);
			EndOfVideo = false;
		}
	}
}
