using SmartAutoAR.InputSource;
using System.Collections.Generic;
using SmartAutoAR.VirtualObject;
using OpenTK.Graphics.OpenGL4;
using Bitmap = System.Drawing.Bitmap;
using SmartAutoAR.VirtualObject.Cameras;
using System;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using SmartAutoAR.VirtualObject.Lights;

namespace SmartAutoAR
{
	/// <summary>
	/// 整合各種功能，能夠快速製作AR的類別
	/// </summary>
	public class ArWorkflow
	{
		public IInputSource InputSource { get; set; }
		public Dictionary<Bitmap, Scene> MarkerPairs { get; set; }
		public float WindowAspectRatio { get { return InputSource.AspectRatio; } }
		public bool EnableSimulation { get; set; }
		public bool EnableLightTracking { get; set; }

		protected int windowWidth, windowHeight;
		protected Dictionary<PatternDetector, Scene> patternScene;
		protected ICamera camera;
		protected PatternTrackingInfo lastInfo;

		protected Background background;

		public ArWorkflow(IInputSource inputSource)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, Scene>();
			EnableSimulation = false;
			EnableLightTracking = false;
			camera = new ArCamera();
			background = new Background();
		}

		public void TrainMarkers()
		{
			patternScene = new Dictionary<PatternDetector, Scene>();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				Mat marker_mat = marker.ToMat();
				Cv2.Resize(marker_mat, marker_mat, new OpenCvSharp.Size(400, 400));
				PatternDetector patternDetector = new PatternDetector(true);
				patternDetector.buildPatternFromImage(marker_mat);
				patternDetector.train();
				patternScene.Add(patternDetector, MarkerPairs[marker]);
			}
		}

		public void Show()
		{
			if (EnableSimulation) Simulate();
			else ProcessAR(true);
		}

		private void ProcessAR(bool backeground)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			Bitmap frame = InputSource.GetInputFrame();
			if (backeground)
			{
				background.SetImage(frame);
				background.Render();
			}

			foreach (PatternDetector detector in patternScene.Keys)
			{
				// 如果偵測到 marker 把偵測結果放在 info 中
				if (detector.Detect(frame.ToMat(), out PatternTrackingInfo info))
				{
					// 如果這是第一幀 就不用防震動
					if (lastInfo == null) lastInfo = info;
					else
					{
						// 如果這一幀跟上一幀差太多
						if (lastInfo.HaveBigDifferentWith(info))
						{
							// 把這一幀存起來
							lastInfo = info;
							// 計算mat
							info.ComputePose();
						}
						// 差不多
						else
						{
							// 拿上一幀的 info 蓋掉新的
							info = lastInfo;
						}
					}

					camera.Update(
						info.ViewMatrix,
						info.GetProjectionMatrix(frame.Width, frame.Height),
						info.CameraPosition);

					if (EnableLightTracking)
					{
						ILight[] predictLights = LightSourceTracker.PredictLightSource(detector.MarkerMat, info);
						patternScene[detector].Lights.AddRange(predictLights);
						patternScene[detector].Render(camera);
						patternScene[detector].Lights.RemoveAt(patternScene[detector].Lights.Count - 1);
						patternScene[detector].Lights.RemoveAt(patternScene[detector].Lights.Count - 1);
					}
					else patternScene[detector].Render(camera);
				}
			}

			GC.Collect();
		}

		private void Simulate()
		{
			ProcessAR(false);
			Bitmap objectOnly = Screenshot();
			ProcessAR(true);
			Bitmap ARframe = Screenshot();

			// 請把 output 替換進來
			Bitmap output = new Bitmap(windowWidth, windowWidth);
			background.SetImage(output);
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			background.Render();
		}

		public Bitmap Screenshot()
		{
			Bitmap bmp = new Bitmap(windowWidth, windowHeight);
			BitmapData data = bmp.LockBits(
				new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.WriteOnly,
				System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, bmp.Width, bmp.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
			return bmp;
		}

		public void OutputResize(int Width, int Height)
		{
			windowWidth = Width;
			windowHeight = Height;
			GL.Viewport(0, 0, windowWidth, windowHeight);
			camera.AspectRatio = WindowAspectRatio;
		}
	}
}
