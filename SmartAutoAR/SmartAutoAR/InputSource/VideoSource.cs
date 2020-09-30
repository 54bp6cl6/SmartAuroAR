using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Diagnostics;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 處理影像來源為「影片」的類別
	/// </summary>
	public class VideoSource : IInputSource
	{
		VideoCapture videoCapture;
		Mat frame;
		Stopwatch watch;
		double msPerFrame;
		public bool EndOfVideo { get; protected set; }

		public VideoSource(string path)
		{
			videoCapture = new VideoCapture(path);
			msPerFrame = 1000 / videoCapture.Fps;
			watch = new Stopwatch();
			frame = new Mat();
			EndOfVideo = false;
		}

		public Bitmap GetInputFrame()
		{
			if (videoCapture.Get(VideoCaptureProperties.PosFrames) == videoCapture.FrameCount)
				throw new System.ArgumentException("Reach the end of video");

			if (!watch.IsRunning)
			{
				videoCapture.Read(frame);
				watch.Start();
			}

			if (watch.ElapsedMilliseconds > msPerFrame)
			{
				for(double ms = 0; ms < watch.ElapsedMilliseconds; ms += msPerFrame)
					if (videoCapture.Get(VideoCaptureProperties.PosFrames) < videoCapture.FrameCount)
						videoCapture.Read(frame);
				watch.Reset();
			}

			if (videoCapture.Get(VideoCaptureProperties.PosFrames) == videoCapture.FrameCount)
				EndOfVideo = true;

			return frame.ToBitmap();
		}

		public void Replay()
		{
			videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
			EndOfVideo = false;
		}
	}
}
