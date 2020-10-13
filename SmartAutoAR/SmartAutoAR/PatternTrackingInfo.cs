using System.Collections.Generic;
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
		public Vec3d Tvec { get; set; }
		public Vec3d Rvec { get; set; }
		public double Distance { get; protected set; }
		public double Pitch { get; protected set; }
		public double Roll { get; protected set; }
		public double Yaw { get; protected set; }

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
			Tvec = tvecs[0];
			Rvec = rvecs[0];
		}

		public void ComputeMatrix()
		{
			ViewMatrix = GetViewMatrix(Rvec, Tvec);

			// 計算campos
			Mat rotMat = new Mat();
			Mat tmat = new Mat(3, 1, MatType.CV_64F);
			Mat rmat = new Mat();
			tmat.At<double>(0, 0) = Tvec[0];
			tmat.At<double>(1, 0) = Tvec[1];
			tmat.At<double>(2, 0) = Tvec[2];
			Cv2.Rodrigues(Rvec, rotMat);
			rotMat = rotMat.T();
			tmat = -rotMat * tmat;
			Cv2.Rodrigues(rotMat, rmat);
			CameraPosition = new Vector3((float)tmat.At<double>(0, 0), (float)tmat.At<double>(2, 0), -(float)tmat.At<double>(1, 0));

			Distance = Math.Sqrt(Math.Pow(Tvec[0], 2) + Math.Pow(Tvec[1], 2) + Math.Pow(Tvec[2], 2));
			Pitch = Rvec[0] / Math.PI * 180 % 360;
			Roll = Rvec[1] / Math.PI * 180 % 360;
			Yaw = Rvec[2] / Math.PI * 180 % 360;
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

		public bool HaveBigDifferentWith(PatternTrackingInfo info, double diffD = 3, double diffRX = 3, double diffRY = 3, double diffRZ = 3)
		{
			if (Math.Abs(info.Distance - Distance) <= diffD
				&& Math.Abs(info.Pitch - Pitch) <= diffRX
				&& Math.Abs(info.Roll - Roll) <= diffRY
				&& Math.Abs(info.Yaw - Yaw) <= diffRZ)
			{
				return false;
			}
			return true;
		}

		public void SmoothWith(PatternTrackingInfo info)
		{
			Vec3d newTvec = new Vec3d
			{
				Item0 = info.Tvec[0] + (Tvec[0] - info.Tvec[0]) * 0.5,
				Item1 = info.Tvec[1] + (Tvec[1] - info.Tvec[1]) * 0.5,
				Item2 = info.Tvec[2] + (Tvec[2] - info.Tvec[2]) * 0.5
			};

			Vec3d newRvec = new Vec3d
			{
				Item0 = info.Rvec[0] + (Rvec[0] - info.Rvec[0]) * 0.5,
				Item1 = info.Rvec[1] + (Rvec[1] - info.Rvec[1]) * 0.5,
				Item2 = info.Rvec[2] + (Rvec[2] - info.Rvec[2]) * 0.5
			};

			Tvec = newTvec;
			Rvec = newRvec;
		}
	}
}