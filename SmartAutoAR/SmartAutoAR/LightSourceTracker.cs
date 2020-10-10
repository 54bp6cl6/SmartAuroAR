using OpenCvSharp;
using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR.VirtualObject.Lights;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAutoAR
{
	public static class LightSourceTracker
	{
		public static ILight[] PredictLightSource(Mat marker, PatternTrackingInfo info)
		{
			Mat distribution = GetDistribution(marker, info.DetectedMarkerImage);
			double[] AvgBrightness = GetAvgBrightness(distribution);

			int maxIndex = 0, minIndex = 0;
			for (int i = 1; i < 9; i++)
			{
				if (AvgBrightness[i] > AvgBrightness[maxIndex]) maxIndex = i;
				else if (AvgBrightness[i] < AvgBrightness[minIndex]) minIndex = i;
			}

			ILight[] output = new ILight[2];
			output[0] = new AmbientLight(Color4.White, (float)AvgBrightness[minIndex] / (float)255 * 0.5f);
			float x, z;
			if (maxIndex < 3) z = -50;
			else if (maxIndex < 6) z = 0;
			else z = 50;
			if (maxIndex % 3 == 0) x = -50;
			else if (maxIndex % 3 == 1) x = 0;
			else x = 50;
			output[1] = new DirectionalLight(Color4.White, new Vector3(x, 50, z), (float)AvgBrightness[maxIndex] / (float)255 * (float)(AvgBrightness[maxIndex] / AvgBrightness[minIndex]) , 1f);
			return output;
		}

		private static Mat GetDistribution(Mat OrgMarker, Mat DetectedMarker)
		{
			Mat OrgGray = new Mat(), DetectedGray = new Mat();
			ColorCalculation.GetGrayImage(OrgMarker, ref OrgGray);
			ColorCalculation.GetGrayImage(DetectedMarker, ref DetectedGray);
			return DetectedGray / OrgGray * 255;
		}

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
