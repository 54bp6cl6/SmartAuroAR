﻿using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using OpenTK;
using System;

namespace SmartAutoAR
{
	/// <summary>
	/// Pattern tracking info.
	/// This code is a rewrite of https://github.com/MasteringOpenCV/code/tree/master/Chapter3_MarkerlessAR using "OpenCV for Unity".
	/// </summary>
	public class PatternTrackingInfo
	{
		public Mat homography;
		public Mat points2d;
		public Point2f[] pts;
		public Matrix4 pose3d;
		public Point2d center;
		public Mat imgPts;
		public Mat DetectedMarkerImage { get; protected set; }
		public Matrix4 ViewMatrix { get; protected set; }
		public Vector3 CameraPosition { get; protected set; }

		protected float[,] cameraMatrix = new float[3, 3]
		{
			{ 1.72933044e+03f, 0.00000000e+00f, 6.38250933e+02f },
		    { 0.00000000e+00f, 1.52595386e+03f, 3.21003959e+02f },
			{ 0, 0, 1 }
		};

		protected float[] dist = new float[]
		{ 4.83589141e-01f ,- 5.85278583e+00f ,- 1.42979696e-02f ,- 1.31067357e-02f,  1.86111848e+01f };

		public PatternTrackingInfo()
		{
			homography = new Mat();
			points2d = new Mat();
			DetectedMarkerImage = new Mat();
		}

		public void ComputePose()
		{

			List<Vec3d> rvecs = new List<Vec3d>(), tvecs = new List<Vec3d>();
			CvAruco.EstimatePoseSingleMarkers(
				new Point2f[][] { pts },
				1,
				InputArray.Create(cameraMatrix),
				InputArray.Create(dist),
				OutputArray.Create<Vec3d>(rvecs),
				OutputArray.Create<Vec3d>(tvecs));
			ViewMatrix = GetViewMatrix(rvecs[0], tvecs[0]);

			// 計算campos
			Mat rotMat = new Mat();
			Mat tmat = new Mat(3, 1, MatType.CV_64F);
			Mat rmat = new Mat();
			tmat.At<double>(0, 0) = tvecs[0][0];
			tmat.At<double>(1, 0) = tvecs[0][1];
			tmat.At<double>(2, 0) = tvecs[0][2];
			Cv2.Rodrigues(rvecs[0], rotMat);
			rotMat = rotMat.T();
			tmat = -rotMat * tmat;
			Cv2.Rodrigues(rotMat, rmat);
			CameraPosition = new Vector3((float)tmat.At<double>(0, 0), (float)tmat.At<double>(2, 0), -(float)tmat.At<double>(1, 0));
		}

		private Matrix4 GetViewMatrix(Vec3d rvec, Vec3d tvec)
		{
			Mat rotMat = new Mat();
			Cv2.Rodrigues(rvec, rotMat);

			Matrix4 output = new Matrix4(
				(float)rotMat.At<double>(0, 0), (float)rotMat.At<double>(0, 2), -(float)rotMat.At<double>(0, 1), (float)tvec[0],
				-(float)rotMat.At<double>(1, 0), -(float)rotMat.At<double>(1, 2), (float)rotMat.At<double>(1, 1), -(float)tvec[1],
				-(float)rotMat.At<double>(2, 0), -(float)rotMat.At<double>(2, 2), (float)rotMat.At<double>(2, 1), -(float)tvec[2],
				0, 0, 0, 1);

			output.Transpose();

			rotMat.Dispose();

			return output;
		}

		public Matrix4 GetProjectionMatrix(float width, float height, float near_plane = 0.0001f, float far_plane = 10000f)
		{
			float fx = cameraMatrix[0, 0];
			float fy = cameraMatrix[1, 1];
			float cx = cameraMatrix[0, 2];
			float cy = cameraMatrix[1, 2];

			Matrix4 pm = new Matrix4()
			{
				Row0 = new Vector4(2 * fx / width, 0, 0, 0),
				Row1 = new Vector4(0, 2 * fy / height, 0, 0),
				Row2 = new Vector4(1 - 2 * cx / width, 2 * cy / height - 1, -(far_plane + near_plane) / (far_plane - near_plane), -1),
				Row3 = new Vector4(0, 0, -(2 * far_plane * near_plane) / (far_plane - near_plane), 0)
			};
			return pm;
		}

		// diffRange是調整防震動的域值 1~100
		public bool HaveBigDifferentWith(PatternTrackingInfo info, int diffRange = 50)
		{
			if (diffRange < 1 || diffRange > 100) throw new ArgumentException("diffRange的有效範圍為1~100");

			// 比較 this 與 info 

			// 如果差異太大就 return true
			return true;
		}
	}
}