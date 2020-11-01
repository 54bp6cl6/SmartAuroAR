using System.Drawing;

namespace SmartAutoAR.InputSource
{
	/// <summary>
	/// 此介面定義了影像來源的行為
	/// </summary>
	public interface IInputSource
	{
		/// <summary>
		/// 呼叫影像來源回傳欲取得的影像
		/// </summary>
		public Bitmap GetNextFrame();

		public Bitmap LastFrame { get; }

		/// <summary>
		/// 回傳影像的寬度
		/// </summary>
		public int OutputWidth { get; }

		/// <summary>
		/// 回傳影像的高度
		/// </summary>
		public int OutputHeight { get; }

		/// <summary>
		/// 回傳影像的長寬比
		/// </summary>
		public float AspectRatio { get; }
	}
}
