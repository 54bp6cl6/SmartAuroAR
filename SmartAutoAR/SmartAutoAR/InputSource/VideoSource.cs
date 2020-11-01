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
		public Bitmap LastFrame { get; protected set; }
		public int OutputWidth { get; protected set; }
		public int OutputHeight { get; protected set; }
		public float AspectRatio { get { return (float)OutputWidth / (float)OutputHeight; } }
		public bool EndOfVideo { get; protected set; }

		VideoCapture videoCapture;
		Mat frame;
		Stopwatch watch;
		double msPerFrame;

		public VideoSource(string path)
		{
			OutputWidth = 1;
			OutputHeight = 1;
			EndOfVideo = false;
			videoCapture = new VideoCapture(path);
			msPerFrame = 1000 / videoCapture.Fps;
			watch = new Stopwatch();
			frame = new Mat();
		}

		public Bitmap GetNextFrame()
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

			LastFrame = frame.ToBitmap();
			OutputWidth = LastFrame.Width;
			OutputHeight = LastFrame.Height;

			return LastFrame;
		}

		public void Replay()
		{
			videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
			EndOfVideo = false;
		}
	}
}
