using System.Drawing;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 處理影像來源為「圖片」的類別
	/// </summary>
	public class ImageSource : IInputSource
	{
		public Bitmap Image { get; set; }
		public int OutputWidth { get { return Image.Width; } }
		public int OutputHeight { get { return Image.Height; } }
		public float AspectRatio { get { return (float)Image.Width / (float)Image.Height; } }


		public ImageSource(Bitmap bitmap)
		{
			this.Image = bitmap;
		}

		public ImageSource(string file)
		{
			this.Image = new Bitmap(file);
		}

		public Bitmap GetInputFrame()
		{
			return this.Image;
		}
	}
}
