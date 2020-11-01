using System.Drawing;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 處理影像來源為「圖片」的類別
	/// </summary>
	public class ImageSource : IInputSource
	{
		public Bitmap LastFrame { get; protected set; }
		public int OutputWidth { get { return LastFrame.Width; } }
		public int OutputHeight { get { return LastFrame.Height; } }
		public float AspectRatio { get { return (float)LastFrame.Width / (float)LastFrame.Height; } }


		public ImageSource(Bitmap bitmap)
		{
			this.LastFrame = bitmap;
		}

		public ImageSource(string file)
		{
			this.LastFrame = new Bitmap(file);
		}

		public Bitmap GetNextFrame()
		{
			return this.LastFrame;
		}
	}
}
