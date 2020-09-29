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
		Mat frame;

		public VideoSource(string path)
		{
			videoCapture = new VideoCapture(path);
			frame = new Mat();
		}

		public Bitmap GetInputFrame()
		{
			videoCapture.Read(frame);

			return frame.ToBitmap();
		}
	}
}
