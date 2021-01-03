using OpenCvSharp;
using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR.VirtualObject.Lights;
using System;

namespace SmartAutoAR
{
	/// <summary>
	/// 提供分析並模擬環境光源，以及生成光照元件等功能
	/// </summary>
	public static class LightSourceTracker
	{
		/// <summary>
		/// 預測光源並生成相應的光照元件
		/// </summary>
		/// <param name="marker">設定之 Marker 影像</param>
		/// <param name="info">影像的 Marker 偵測結果</param>
		/// <returns>模擬的光照元件</returns>
		public static ILight[] PredictLightSource(Mat marker, MarkerTrackingInfo info)
		{
			Mat distribution = GetDistribution(marker, info.DetectedMarkerImage);
			double[] avgBrightness = GetAvgBrightness(distribution);

			int maxIndex = 0, minIndex = 0;
			for (int i = 1; i < 9; i++)
			{
				if (avgBrightness[i] > avgBrightness[maxIndex]) maxIndex = i;
				else if (avgBrightness[i] < avgBrightness[minIndex]) minIndex = i;
			}

			ILight[] output = new ILight[2];
			output[0] = new AmbientLight(Color4.White, (float)avgBrightness[minIndex] / (float)255 * 0.5f);
			float x, z;
			if (maxIndex < 3) z = -50;
			else if (maxIndex < 6) z = 0;
			else z = 50;
			if (maxIndex % 3 == 0) x = -50;
			else if (maxIndex % 3 == 1) x = 0;
			else x = 50;
			output[1] = new DirectionalLight(
				Color4.White, 
				new Vector3(x, 50, z), 
				(float)avgBrightness[maxIndex] / (float)255 * Math.Min((float)(avgBrightness[maxIndex] / avgBrightness[minIndex]), 3f) 
				, 1f);
			return output;
		}

		/// <summary>
		/// 根據 Marker 製作光照分布圖
		/// </summary>
		/// <param name="orgMarker">Marker 設定圖檔</param>
		/// <param name="detectedMarker">偵測到的 Marker 影像</param>
		/// <returns>光照分布圖</returns>
		private static Mat GetDistribution(Mat orgMarker, Mat detectedMarker)
		{
			Mat[] orgLab = ColorCalculation.GetLabChennel(orgMarker);
			Mat[] detectedLab = ColorCalculation.GetLabChennel(detectedMarker);
			return detectedLab[0] / orgLab[0] * 256;
		}

		/// <summary>
		/// 取得光照分布圖中九個方位各自的平均亮度
		/// </summary>
		/// <param name="distribution">光照分布圖</param>
		/// <returns>九個方位各自的平均亮度</returns>
		private static double[] GetAvgBrightness(Mat distribution)
		{
			Mat leftTop = distribution.SubMat(0, distribution.Width / 3, 0, distribution.Height / 3);
			Mat midTop = distribution.SubMat(0, distribution.Height / 3, distribution.Width / 3, distribution.Width / 3 * 2);
			Mat rightTop = distribution.SubMat(0, distribution.Width / 3, distribution.Height / 3 * 2, distribution.Height - 1);
			Mat leftMid = distribution.SubMat(distribution.Width / 3, distribution.Width / 3 * 2, 0, distribution.Height / 3);
			Mat mid = distribution.SubMat(distribution.Width / 3, distribution.Width / 3 * 2, distribution.Width / 3, distribution.Width / 3 * 2);
			Mat rightMid = distribution.SubMat(distribution.Width / 3, distribution.Width / 3 * 2, distribution.Height / 3 * 2, distribution.Height - 1);
			Mat leftBottom = distribution.SubMat(distribution.Height / 3 * 2, distribution.Height - 1, 0, distribution.Height / 3);
			Mat midBottom = distribution.SubMat(distribution.Height / 3 * 2, distribution.Height - 1, distribution.Width / 3, distribution.Width / 3 * 2);
			Mat rightBottom = distribution.SubMat(distribution.Height / 3 * 2, distribution.Height - 1, distribution.Height / 3 * 2, distribution.Height - 1);

			double[] AvgBrightness = new double[] {
				leftTop.Mean().Val0, midTop.Mean().Val0, rightTop.Mean().Val0,
				leftMid.Mean().Val0,mid.Mean().Val0,rightMid.Mean().Val0,
				leftBottom.Mean().Val0, midBottom.Mean().Val0, rightBottom.Mean().Val0
			};

			return AvgBrightness;
		}
	}
}
