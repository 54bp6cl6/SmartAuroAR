using OpenCvSharp;

namespace SmartAutoAR
{
	static class ColorCalculation
	{
		public static Mat[] GetLabChennel(Mat inputMat)
		{
			Mat labImage = new Mat();
			Cv2.CvtColor(inputMat, labImage, ColorConversionCodes.BGR2Lab);
			return Cv2.Split(labImage);
		}
	}
}
