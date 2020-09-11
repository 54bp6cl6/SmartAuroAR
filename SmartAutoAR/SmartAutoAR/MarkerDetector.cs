using OpenTK;
using OpenCvSharp;

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
		private Point3f[] objPts = new Point3f[]
		{
            //18 mean marker cm
            new Point3f(0,0,0),
			new Point3f(18,0,0),
			new Point3f(18,18,0),
			new Point3f(0,18,0)
		};

		private double[,] cameraMatrix = new double[3, 3]
		{
			{ 1.2195112968898779e+003, 0, 3.6448211117862780e+002 },
			{ 0, 1.2414409169216196e+003, 2.4321803868732076e+002 },
			{ 0, 0, 1 }
		};

		public bool Validity { get; protected set; }
		public Matrix4 ViewMatrix { get; protected set; }
		public Vector3 CameraPosition { get; protected set; }


		public bool Detecte(Bitmap frame, Bitmap marker)
		{
			bool result = FindMarker(frame.ToMat(), marker.ToMat());
			this.Validity = result;
			CameraPosition = Vector3.Zero;
			return result;
		}

		private double Angle(OpenCvSharp.Point pt1, OpenCvSharp.Point pt2, OpenCvSharp.Point pt0)
		{
			double dx1 = pt1.X - pt0.X;
			double dy1 = pt1.Y - pt0.Y;
			double dx2 = pt2.X - pt0.X;
			double dy2 = pt2.Y - pt0.Y;
			return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
		}

		private bool Hist(Mat marker, Mat target)
		{
			// wat
			Mat hist1 = new Mat();
			Mat hist2 = new Mat();
			int histogramSize = 256;
			int[] dimensions = { histogramSize };               // Histogram size for each dimension
			Rangef[] ranges = { new Rangef(0, histogramSize) };
			Cv2.CalcHist(
				images: new[] { marker },
				channels: new[] { 0 }, //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
				mask: null,
				hist: hist1,
				dims: 1, //The histogram dimensionality.
				histSize: dimensions,
				ranges: ranges);
			Cv2.Normalize(hist1, hist1);

			Cv2.CalcHist(
				images: new[] { target },
				channels: new[] { 0 }, //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
				mask: null,
				hist: hist2,
				dims: 1, //The histogram dimensionality.
				histSize: dimensions,
				ranges: ranges);
			Cv2.Normalize(hist2, hist2);

			return Cv2.CompareHist(hist1, hist2, 0) >= 0.8;
		}


		private bool FindMarker(Mat image, Mat MARKER)
		{
			Mat copyImage = new Mat();
			image.CopyTo(copyImage);
			Mat labImage = new Mat();
			Mat grayImage = new Mat();
			Mat binaryImage = new Mat();
			Mat edgeImage = new Mat();

			// 以 L 二值化，並做邊緣檢測
			Cv2.CvtColor(copyImage, labImage, ColorConversionCodes.BGR2Lab);
			Mat[] labChannel = Cv2.Split(labImage); //分割通道
			Mat[] grayChannel = new Mat[] { labChannel[0] };
			Cv2.MixChannels(labChannel, grayChannel, new int[] { 0, 0 });
			Cv2.Merge(grayChannel, grayImage);
			Cv2.AdaptiveThreshold(grayImage, binaryImage, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 101, 10);
			Cv2.Laplacian(binaryImage, edgeImage, MatType.CV_8UC1, 5, 1, 0);

			OpenCvSharp.Point[][] contours;

			Cv2.FindContours(edgeImage, out contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
			Mat approx = new Mat();

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.ApproxPolyDP(InputArray.Create(contours[i]), approx, Cv2.ArcLength(contours[i], true) * 0.01, true);
				if (approx.Rows == 4 &&
					Math.Abs(Cv2.ContourArea(approx)) > 1000 &&
					Math.Abs(Cv2.ContourArea(approx)) < 100000 &&
					Cv2.IsContourConvex(approx))
				{
					double maxCos = 0;
					for (int j = 2; j < 5; j++)//problem
					{
						OpenCvSharp.Point p1 = new OpenCvSharp.Point(approx.Get<float>(j % 4, 0), approx.Get<float>(j % 4, 1));
						OpenCvSharp.Point p2 = new OpenCvSharp.Point(approx.Get<float>(j - 2, 0), approx.Get<float>(j - 2, 1));
						OpenCvSharp.Point p0 = new OpenCvSharp.Point(approx.Get<float>(j - 1, 0), approx.Get<float>(j - 1, 1));
						double cos = Math.Abs(Angle(p0, p2, p1));

						maxCos = Math.Max(maxCos, cos);
					}


					if (maxCos < 0.4)
					{
						RotatedRect rect = Cv2.MinAreaRect(contours[i]);
						Mat vertices = new Mat();
						Cv2.BoxPoints(rect, vertices);

						// 可能有誤
						//我改ㄉ 這裡數字指的是index 0>top left(row(1)); 1>top right; 2>bottom left; 3>bottom right
						//Point2f[] points = { vertices.row(1), vertices.row(2), vertices.row(0), vertices.row(3) };
						float[] temp = new float[8];
						vertices.GetArray<float>(out temp);


						Point2f[] points = new Point2f[] {
							new Point2f(temp[0], temp[1]),
							new Point2f(temp[2], temp[3]),
							new Point2f(temp[4], temp[5]),
							new Point2f(temp[6], temp[7]) };

						double perimeter = Math.Sqrt(Math.Pow((points[1].X - points[2].X), 2) + Math.Pow((points[1].Y - points[2].Y), 2)); //周長
						double perimeter2 = Math.Sqrt(Math.Pow((points[1].X - points[0].X), 2) + Math.Pow((points[1].Y - points[0].Y), 2));
						double tempmaxp, tempminp;
						tempmaxp = (perimeter > perimeter2) ? perimeter : perimeter2;
						tempminp = (perimeter > perimeter2) ? perimeter2 : perimeter;
						var temp1 = tempmaxp / tempminp;
						if (tempmaxp / tempminp > 2)
						{
							continue;
						}

						Point2f[] dst = {
							new Point2f(0, 0),
							new Point2f((float)perimeter, 0),
							new Point2f(0, (float)perimeter),
							new Point2f((float)perimeter, (float)perimeter)
						};
						Mat P = Cv2.GetPerspectiveTransform(points, dst);
						Mat result = new Mat();

						Cv2.WarpPerspective(binaryImage, result, P, binaryImage.Size());
						Mat correct = result[new Rect(0, 0, (int)perimeter, (int)perimeter)];

						if (Hist(correct, MARKER))
						{
							var rvec = new double[] { 0, 0, 0 };
							var tvec = new double[] { 0, 0, 0 };

							var dist = new double[] { 0, 0, 0, 0, 0 };
							//var jacobian = new Mat();
							//double[,] jacobian;
							// Point2f[] imgPts;
							var imgPts = new Mat();
							var rvecMat = new Mat();
							var tvecMat = new Mat();
							//solvePnP(objectPoints, vertices, camera_matrix, dist_coeffs, rotation_vector, translation_vector);
							Cv2.SolvePnP(
								InputArray.Create(objPts),
								vertices,
								InputArray.Create(cameraMatrix),
								InputArray.Create(dist),
								rvecMat, tvecMat);

							Cv2.ProjectPoints(InputArray.Create(objPts), InputArray.Create(rvec), InputArray.Create(tvec), InputArray.Create(cameraMatrix), InputArray.Create(dist), imgPts, new Mat());
							//21s
							//646 vs 869(c++
							Mat rotation = new Mat();

							rotation.Create(new OpenCvSharp.Size(4, 4), MatType.CV_64F);


							Cv2.Rodrigues(rvecMat, rotation);


							ViewMatrix = new Matrix4()
							{
								Row0 = new Vector4(rotation.Get<float>(0, 0), rotation.Get<float>(0, 1), rotation.Get<float>(0, 2), tvecMat.Get<float>(0, 0)),
								Row1 = new Vector4(rotation.Get<float>(1, 0), rotation.Get<float>(1, 1), rotation.Get<float>(1, 2), tvecMat.Get<float>(1, 0)),
								Row2 = new Vector4(rotation.Get<float>(2, 0), rotation.Get<float>(2, 1), rotation.Get<float>(2, 2), tvecMat.Get<float>(2, 0)),
								Row3 = new Vector4(0, 0, 0, 1.0f),
							};
							return true;
							//DrawSquare(image, points.ToList());

						}
					}
				}

			}
			return false;
		}
	}
}
