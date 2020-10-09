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
        public int OutputWidth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public float AspectRatio { get { return (float)OutputWidth / (float)OutputHeight; } }

        VideoCapture videoCapture;

        public StreamSource(int camId = 0)
        {
            OutputWidth = 1;
            OutputHeight = 1;
            videoCapture = new VideoCapture(camId);
        }

        public Bitmap GetInputFrame()
        {
            Mat frame = new Mat();
            videoCapture.Read(frame);
            Bitmap output = frame.ToBitmap();
            OutputWidth = output.Width;
            OutputHeight = output.Height;
            return output;
        }
    }
}
