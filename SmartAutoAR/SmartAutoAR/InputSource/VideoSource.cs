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

		/// <summary>
		/// 以指定的檔案路徑初始化物件
		/// </summary>
		/// <param name="path">來源影片之檔案路徑</param>
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

		/// <improvable>
		/// 超過才給下一幀的方式還是有些許誤差
		/// </improvable>
		public Bitmap GetNextFrame()
		{
			if (videoCapture.Get(VideoCaptureProperties.PosFrames) == videoCapture.FrameCount)
				throw new System.ArgumentException("Reach the end of the video.");

			if (!watch.IsRunning)
			{
				videoCapture.Read(frame);
				watch.Start();
			}

			// 為確保影片撥放速度不會因處理速度忽快忽慢
			// 計時超過 msPerFrame 才給他下一幀畫面(可改進)
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

		/// <summary>
		/// 重新撥放影片
		/// </summary>
		public void Replay()
		{
			videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
			EndOfVideo = false;
			watch.Stop();
		}
	}
}
