using OpenTK;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Bitmap = System.Drawing.Bitmap;
using System;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace SmartAutoAR
{
	/// <summary>
	/// 用於偵測與分析 Marker 的類別
	/// </summary>
	public class MarkerDetector
	{
		private double[,] cameraMatrix = new double[3, 3]
		{
			{ 1.2195112968898779e+003, 0, 3.6448211117862780e+002 },
			{ 0, 1.2414409169216196e+003, 2.4321803868732076e+002 },
			{ 0, 0, 1 }
		};

		private double[] distCoeffs = new double[]
		{ -4.1802327018241026e-001, 5.0715243805833121e-001, 0, 0, -5.7843596847939704e-001 };

		// 指定要使用的 aruco marker
		Dictionary dictionary;

		public bool Validity { get; protected set; }
		public Matrix4 ViewMatrix { get; protected set; }
		public Vector3 CameraPosition { get; protected set; }

		public MarkerDetector()
		{
			dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.DictArucoOriginal);
		}


		public bool Detecte(System.Drawing.Bitmap frame, System.Drawing.Bitmap marker)
		{
			bool result = false;

			// 輸入影像轉為 mat
			Mat inputImage = frame.ToMat();

			// 偵測 marker 結果的參數
			int[] markerIds;
			Point2f[][] markerCorners, rejectedCandidates;
			DetectorParameters parameters = DetectorParameters.Create();

			// 偵測
			CvAruco.DetectMarkers(inputImage, dictionary, out markerCorners, out markerIds, parameters, out rejectedCandidates);

			// 偵測到 marker 後畫出來
			if (markerCorners.Length > 0)
			{
				result = true;
				//Mat outputImage = inputImage.Clone();
				//CvAruco.DrawDetectedMarkers(outputImage, markerCorners, markerIds);
				//Cv2.ImWrite("result.png", outputImage);

				// 計算姿態參數
				List<Vec3d> rvecs = new List<Vec3d>(), tvecs = new List<Vec3d>();
				CvAruco.EstimatePoseSingleMarkers(
					markerCorners,
					1,
					InputArray.Create(cameraMatrix),
					InputArray.Create(distCoeffs),
					OutputArray.Create<Vec3d>(rvecs),
					OutputArray.Create<Vec3d>(tvecs));

				// 畫出軸
				/*CvAruco.DrawAxis(
					outputImage,
					InputArray.Create(cameraMatrix),
					InputArray.Create(distCoeffs),
					InputArray.Create(rvecs),
					InputArray.Create(tvecs),
					1f);
				Cv2.ImWrite("result.png", outputImage);*/

				// 開始計算 view matrix
				ViewMatrix = GetViewMatrix2(rvecs[0], tvecs[0]);
			}

			return result;
		}

		// 手動抓 lookAt
		private Matrix4 getViewMatrix(Vec3d rvec, Vec3d tvec)
		{
			Mat rotMat = new Mat();
			Mat tmat = new Mat(3, 1, MatType.CV_64F);
			Mat rmat = new Mat();
			tmat.At<double>(0, 0) = tvec[0];
			tmat.At<double>(1, 0) = tvec[1];
			tmat.At<double>(2, 0) = tvec[2];
			Cv2.Rodrigues(rvec, rotMat);

			rotMat = rotMat.T();
			tmat = -rotMat * tmat;
			Cv2.Rodrigues(rotMat, rmat);

			Vector3 tVector3 = new Vector3((float)tmat.At<double>(0, 0), (float)tmat.At<double>(2, 0), -(float)tmat.At<double>(1, 0));
			Vector3 rVector3 = new Vector3((float)rmat.At<double>(0, 0), (float)rmat.At<double>(2, 0), -(float)rmat.At<double>(1, 0));

			Matrix4 guest = Matrix4.LookAt(tVector3, new Vector3(0.9f, 0f, 0.3f), new Vector3(-1f, 0f, 0f));

			return guest;
		}

		// 自動算ViewMatrix
		private Matrix4 GetViewMatrix2(Vec3d rvec, Vec3d tvec)
		{
			Mat rotMat = new Mat();
			Cv2.Rodrigues(rvec, rotMat);

			Matrix4 output = new Matrix4(
				(float)rotMat.At<double>(0, 0), (float)rotMat.At<double>(0, 2), -(float)rotMat.At<double>(0, 1), (float)tvec[0],
				-(float)rotMat.At<double>(1, 0), -(float)rotMat.At<double>(1, 2), (float)rotMat.At<double>(1, 1),  -(float)tvec[1],
				-(float)rotMat.At<double>(2, 0), -(float)rotMat.At<double>(2, 2), (float)rotMat.At<double>(2, 1),  -(float)tvec[2],
				0, 0, 0, 1);

			output.Transpose();

			rotMat.Dispose();

			return output;
		}
	}
}