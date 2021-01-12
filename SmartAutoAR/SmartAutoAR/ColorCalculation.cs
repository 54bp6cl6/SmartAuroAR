using OpenCvSharp;

namespace SmartAutoAR
{
	/// <summary>
	/// 提供色彩相關運算函式
	/// </summary>
	public static class ColorCalculation
	{
		/// <summary>
		/// 將影像從RGB轉為Lab
		/// </summary>
		/// <param name="inputMat">待轉換影像</param>
		/// <returns>Lab影像</returns>
		public static Mat[] GetLabChennel(Mat inputMat)
		{
			Mat labImage = new Mat();
			Cv2.CvtColor(inputMat, labImage, ColorConversionCodes.BGR2Lab);
			return Cv2.Split(labImage);
		}
	}
}