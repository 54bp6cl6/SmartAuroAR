using OpenTK;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Bitmap = System.Drawing.Bitmap;
using System;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;

namespace SmartAutoAR
{
	/// <summary>
	/// 用於偵測與分析 Marker 的類別
	/// </summary>
	public class MarkerDetector
	{
		/*private double[,] cameraMatrix = new double[3, 3]
		{
			{ 1.2195112968898779e+003, 0, 3.6448211117862780e+002 },
			{ 0, 1.2414409169216196e+003, 2.4321803868732076e+002 },
			{ 0, 0, 1 }
		};*/

		public bool Validity { get; protected set; }
		public Matrix4 ViewMatrix { get; protected set; }
		public Vector3 CameraPosition { get; protected set; }


		public bool Detecte(System.Drawing.Bitmap frame, System.Drawing.Bitmap marker)
		{
			bool result = false;

			Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);
			Mat inputImage = frame.ToMat();
			int[] markerIds;
			Point2f[][] markerCorners, rejectedCandidates;
			DetectorParameters parameters = DetectorParameters.Create();
			CvAruco.DetectMarkers(inputImage, dictionary, out markerCorners, out markerIds, parameters, out rejectedCandidates);
			if (markerCorners.Length > 0) result = true;
			Mat outputImage = inputImage.Clone();
			CvAruco.DrawDetectedMarkers(outputImage, markerCorners, markerIds);
			Cv2.ImWrite("result.png", outputImage);

			return result;
		}


	}
}
