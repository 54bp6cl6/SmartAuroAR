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

		/// <summary>
		/// 以指定的影像或路徑初始化物件
		/// </summary>
		/// <param name="bitmap">來源影像</param>
		public ImageSource(Bitmap bitmap)
		{
			this.LastFrame = bitmap;
		}

		/// <summary>
		/// 以指定的影像或路徑初始化物件
		/// </summary>
		/// <param name="file">來源影像之檔案路徑</param>
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
