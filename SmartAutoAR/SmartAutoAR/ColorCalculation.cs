using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAutoAR
{
	static class ColorCalculation
	{
		public static void GetGrayImage(Mat inputMat, ref Mat outputMat)
		{
			Mat labImage = new Mat();

			// 以 L 二值化，並做邊緣檢測
			Cv2.CvtColor(inputMat, labImage, ColorConversionCodes.BGR2Lab);
			Mat[] labChannel = Cv2.Split(labImage); //分割通道
			Mat[] grayChannel = new Mat[] { labChannel[0] };
			Cv2.MixChannels(labChannel, grayChannel, new int[] { 0, 0 });
			Cv2.Merge(grayChannel, outputMat);
		}
	}
}
