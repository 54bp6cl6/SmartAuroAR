using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 處理影像來源為「串流」的類別
	/// </summary>
	public class StreamSource : IInputSource
	{
        public Bitmap LastFrame { get; protected set; }
        public int OutputWidth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public float AspectRatio { get { return (float)OutputWidth / (float)OutputHeight; } }

        VideoCapture videoCapture;

        /// <summary>
        /// 以指定的內建攝影鏡頭編號初始化物件
        /// </summary>
        /// <param name="camId">指定之內建攝影鏡頭編號</param>
        public StreamSource(int camId = 0)
        {
            OutputWidth = 1;
            OutputHeight = 1;
            videoCapture = new VideoCapture(camId);
        }

        public Bitmap GetNextFrame()
        {
            Mat frame = new Mat();
            videoCapture.Read(frame);
            LastFrame = frame.ToBitmap();
            OutputWidth = LastFrame.Width;
            OutputHeight = LastFrame.Height;
            return LastFrame;
        }
    }
}
