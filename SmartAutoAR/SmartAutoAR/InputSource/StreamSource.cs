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
        VideoCapture videoCapture;
        Mat frame;

        public StreamSource(int camId = 0)
        {
            videoCapture = new VideoCapture(camId);
            frame = new Mat();
        }

        public Bitmap GetInputFrame()
        {
            videoCapture.Read(frame);
            return frame.ToBitmap();
        }
    }
}
