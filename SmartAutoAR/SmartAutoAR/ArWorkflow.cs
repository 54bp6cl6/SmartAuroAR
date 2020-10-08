using SmartAutoAR.InputSource;
using System.Collections.Generic;
using SmartAutoAR.VirtualObject;
using OpenTK.Graphics.OpenGL4;
using Bitmap = System.Drawing.Bitmap;
using SmartAutoAR.VirtualObject.Cameras;
using System;
using OpenCVMarkerLessAR;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;

namespace SmartAutoAR
{
	/// <summary>
	/// 整合各種功能，能夠快速製作AR的類別
	/// </summary>
	public class ArWorkflow
	{
		public IInputSource InputSource { get; set; }
		public Dictionary<Bitmap, IScene> MarkerPairs { get; set; }
		public ICamera Camera { get; protected set; }
		public PatternDetector patternDetector { get; protected set; }
		public PatternTrackingInfo patternTrackinginfo { get; protected set; }
		public Pattern pattern;
		private Mat OrgMarker;

		// 儲存最後一個結果
		private bool have_last;
		private List<Bitmap> last_markers;
		public float WindowAspectRatio { get { return background.AspectRatio; } }
		protected Background background;

		public ArWorkflow(IInputSource inputSource, Bitmap marker)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, IScene>();
			background = new Background();
			Camera = new ArCamera();
			last_markers = new List<Bitmap>();
			have_last = false;
			OrgMarker = marker.ToMat();
			Cv2.Resize(OrgMarker, OrgMarker, new OpenCvSharp.Size(400, 400));
			SetMarker(marker);
		}

		public void Show(bool backeground = true)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			Bitmap frame = InputSource.GetInputFrame();
			background.SetImage(frame);
			if (backeground) background.Render();
			last_markers.Clear();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				if (patternDetector.findPattern(frame.ToMat(), patternTrackinginfo))
				{
					patternTrackinginfo.ComputePose();
					// 偵測到 marker
					Camera.Update(
						patternTrackinginfo.ViewMatrix,
						patternTrackinginfo.GetProjectionMatrix(frame.Width, frame.Height),
						patternTrackinginfo.CameraPosition);
					MarkerPairs[marker].Render(Camera);
					last_markers.Add(marker);
					have_last = true;
				}
			}
			GC.Collect();
		}

		public void Simulate(int width, int height)
		{
			Show(false);
			Bitmap objectOnly = Screenshot(width, height);
			Show(true);
			Bitmap ARframe = Screenshot(width, height);
			objectOnly.Save("result1.jpg", ImageFormat.Jpeg);
			ARframe.Save("result2.jpg", ImageFormat.Jpeg);

			// 請把 output 替換進來
			Bitmap output = new Bitmap(width, height);
			background.SetImage(output);
			background.Render();
		}

		private Bitmap Screenshot(int width, int height)
		{
			// 取得畫面
			Bitmap bmp = new Bitmap(width, height);
			BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, bmp.Width, bmp.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
			return bmp;
		}

		public void ShowLast(bool backeground = true)
		{
			if (have_last)
			{
				GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
				if (backeground) background.Render();
				foreach (Bitmap marker in last_markers)
				{
					MarkerPairs[marker].Render(Camera);
				}
			}
			else
			{
				Show(backeground);
			}
		}

		// 測試用
		public void ShowMarker()
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			Bitmap frame = InputSource.GetInputFrame();
			if (patternDetector.findPattern(frame.ToMat(), patternTrackinginfo))
			{
				Mat Dgray = new Mat(), Ogray = new Mat();
				getGray(patternTrackinginfo.detectedMarkerImage, ref Dgray);
				getGray(OrgMarker, ref Ogray);

				Mat white = new Mat(Ogray.Width, Ogray.Height, Ogray.Type());
				white += 255;
				Mat Nlogo = white - Ogray;

				Mat result = Ogray - Dgray;
				//result = result.Resize(new Size(10, 10), interpolation: InterpolationFlags.Area);
				Mat leftTop = result.SubMat(0, result.Width / 3, 0, result.Height / 3);
				Mat midTop = result.SubMat(0, result.Height / 3, result.Width / 3, result.Width / 3 * 2);
				Mat rightTop = result.SubMat(0, result.Width / 3, result.Height / 3 * 2, result.Height - 1);
				Mat leftMid = result.SubMat(result.Width / 3, result.Width / 3 * 2, 0, result.Height / 3);
				Mat mid = result.SubMat(result.Width / 3, result.Width / 3 * 2, result.Width / 3, result.Width / 3 * 2);
				Mat rightMid = result.SubMat(result.Width / 3, result.Width / 3 * 2, result.Height / 3 * 2, result.Height - 1);
				Mat leftBottom = result.SubMat(result.Height / 3 * 2, result.Height - 1, 0, result.Height / 3);
				Mat midBottom = result.SubMat(result.Height / 3 * 2, result.Height - 1, result.Width / 3, result.Width / 3 * 2);
				Mat rightBottom = result.SubMat(result.Height / 3 * 2, result.Height - 1, result.Height / 3 * 2, result.Height - 1);

				background.SetImage(patternTrackinginfo.detectedMarkerImage.ToBitmap());
				background.Render();
			}
			GC.Collect();
		}


		// 測試用
		void getGray(Mat image, ref Mat gray)
		{
			Mat labImage = new Mat();

			// 以 L 二值化，並做邊緣檢測
			Cv2.CvtColor(image, labImage, ColorConversionCodes.BGR2Lab);
			Mat[] labChannel = Cv2.Split(labImage); //分割通道
			Mat[] grayChannel = new Mat[] { labChannel[0] };
			Cv2.MixChannels(labChannel, grayChannel, new int[] { 0, 0 });
			Cv2.Merge(grayChannel, gray);
		}

		public void Resize(int Width, int Height)
		{
			GL.Viewport(0, 0, Width, Height);
			Camera.AspectRatio = (float)Width / (float)Height;
		}

		private void SetMarker(Bitmap marker)
		{
			Mat MARKER = marker.ToMat();
			Cv2.Resize(MARKER, MARKER, new OpenCvSharp.Size(400, 400));
			pattern = new Pattern();
			patternTrackinginfo = new PatternTrackingInfo();
			patternDetector = new PatternDetector(true);
			patternDetector.buildPatternFromImage(MARKER, pattern);
			patternDetector.train();
		}
	}
}
