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
		public float WindowAspectRatio { get { return InputSource.AspectRatio; } }
		public bool EnableSimulation { get; set; }
		public bool EnableLightTracking { get; set; }

		protected int windowWidth, windowHeight;
		protected Dictionary<PatternDetector, IScene> patternScene;
		protected ICamera camera;

		protected bool haveLast;
		protected List<IScene> lastScenes;
		protected Background background;

		public ArWorkflow(IInputSource inputSource)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, IScene>();
			EnableSimulation = false;
			EnableLightTracking = false;
			camera = new ArCamera();
			haveLast = false;
			lastScenes = new List<IScene>();
			background = new Background();
		}

		public void TrainMarker()
		{
			patternScene = new Dictionary<PatternDetector, IScene>();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				Mat marker_mat = marker.ToMat();
				Cv2.Resize(marker_mat, marker_mat, new Size(400, 400));
				PatternDetector patternDetector = new PatternDetector(true);
				patternDetector.buildPatternFromImage(marker_mat);
				patternDetector.train();
				patternScene.Add(patternDetector, MarkerPairs[marker]);
			}
		}

		public void Show(bool backeground = true)
		{
			if (EnableSimulation) Simulate();
			else ProcessAR(backeground);
		}

		public void ShowLast(bool backeground = true)
		{
			if (haveLast)
			{
				GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
				if (backeground) background.Render();
				foreach (IScene scene in lastScenes)
				{
					scene.Render(camera);
				}
			}
			else
			{
				Show(backeground);
			}
		}

		private void ProcessAR(bool backeground)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			lastScenes.Clear();

			Bitmap frame = InputSource.GetInputFrame();
			if (backeground)
			{
				background.SetImage(frame);
				background.Render();
			}

			foreach (PatternDetector detector in patternScene.Keys)
			{
				if (detector.Detect(frame.ToMat(), out PatternTrackingInfo info))
				{
					info.ComputePose();
					camera.Update(
						info.ViewMatrix,
						info.GetProjectionMatrix(frame.Width, frame.Height),
						info.CameraPosition);
					patternScene[detector].Render(camera);
					lastScenes.Add(patternScene[detector]);
					if (!haveLast) haveLast = true;
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
			lastScenes.Clear();
		}

		public Bitmap Screenshot()
		{
			Bitmap bmp = new Bitmap(windowWidth, windowWidth);
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
