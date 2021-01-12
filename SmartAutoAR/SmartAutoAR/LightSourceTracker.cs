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
		/// 1. 將偵測到的Marker與原始圖檔比對，得到光照分布圖
		/// 2. 將光照分布圖切成九宮格，參考最暗一格的亮度來生成一個環境光元件
		/// 3. 參考最亮一格的位置與亮度生成一個定向光元件
		/// 4. 根據最亮與最暗的對比來調整模擬光的強度，增加對比
		/// 5. Return 模擬光
		/// </summary>
		/// <param name="marker">設定之 Marker 影像</param>
		/// <param name="info">影像的 Marker 偵測結果</param>
		/// <returns>模擬的光照元件</returns>
		/// <improvable>
		/// 1. 不要單純分成九格，用更精確的方式分析光源方向(包含高度)
		/// 2. 從學理上來建立計算光強度的公式，不要單靠經驗法則
		/// 3. 不只模擬兩盞燈(AmbientLight 與 DirectionalLight)，找出最真實的模擬光組合
		/// 4. 將顏色也加入分析模擬(目前沒有分析顏色是因為我想到的辦法都會對Marker的自由度有限制)
		/// </improvable>
		public static ILight[] PredictLightSource(Mat marker, MarkerTrackingInfo info)
		{
			// 取得光照分布資訊
			Mat distribution = GetDistribution(marker, info.DetectedMarkerImage);
			// 分成九格取亮度平均值
			double[] avgBrightness = GetAvgBrightness(distribution);

			// 找出最暗與最亮的格子
			int maxIndex = 0, minIndex = 0;
			for (int i = 1; i < 9; i++)
			{
				if (avgBrightness[i] > avgBrightness[maxIndex]) maxIndex = i;
				else if (avgBrightness[i] < avgBrightness[minIndex]) minIndex = i;
			}

			// 固定模擬一個 AmbientLight 與一個 DirectionalLight
			ILight[] output = new ILight[2];

			// Ambient Light 強度取 (最暗格子的L / 255) 來轉為 0~1 之間
			// 因為看起來還是太亮，所以再乘以 0.5 效果比較好(經驗法則，可改進)
			output[0] = new AmbientLight(Color4.White, (float)avgBrightness[minIndex] / (float)255 * 0.5f);

			// 根據最亮的格子決定 Directional Light 的 x,z 方位
			// 由於 Directional Light 算的是方位，因此 50 只是一個參考值，數字放多少都可以
			float x, z;
			if (maxIndex < 3) z = -50;
			else if (maxIndex < 6) z = 0;
			else z = 50;
			if (maxIndex % 3 == 0) x = -50;
			else if (maxIndex % 3 == 1) x = 0;
			else x = 50;

			// Directional Light 高度(y)固定在50，因為不知道怎麼分析光的高度(可改進)
			// 強度的部分採 (最亮格子的L / 255) * (最亮L/最暗L) 來增加模擬光的明暗對比(經驗法則)
			// 由於 (最亮L/最暗L) 數字有可能太大導致光強到失控，因此用min()限定不能超過 3 (超過3的亮度太誇張，經驗法則，可改進)
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
		/// <improvable>
		/// 1. 偵測到的Marker圖像不會 100% 轉正(會有點歪)，導致只有大色塊的運算結果比較正確
		/// 2. 黑色部分L是0，沒有辦法運算，會影響到算平均時的數值(所以Marker黑色部分過多的話，這個系統應該會失效)，最好想辦法排除黑色的影響
		/// </improvable>
		private static Mat GetDistribution(Mat orgMarker, Mat detectedMarker)
		{
			// 將 偵測到的Marker圖像L / 原始Marker的L
			// 得出每一個像素變亮的比率
			// 由於每個顏色被相同的光打到，增加的L不同
			// 因此用比率來算
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
