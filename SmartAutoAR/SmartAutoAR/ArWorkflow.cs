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
		public bool EnableSimulation { get; set; }
		public bool EnableLightTracking { get; set; }

		public float WindowAspectRatio { get { return InputSource.AspectRatio; } }

		protected int windowWidth, windowHeight;
		protected Dictionary<MarkerDetector, Scene> markerScene;
		protected ICamera camera;
		protected MarkerTrackingInfo lastInfo;
		protected ColorHarmonization colorHarmonize;
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


		public void ClearState()
		{
			lastInfo = null;
		}

		public void TrainMarkers()
		{
			markerScene = new Dictionary<MarkerDetector, Scene>();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				Mat markerMat = marker.ToMat();
				Cv2.Resize(markerMat, markerMat, new Size(400, 400));
				MarkerDetector markerDetector = new MarkerDetector(true);
				markerDetector.buildMarkerFromImage(markerMat);
				markerDetector.train();
				markerScene.Add(markerDetector, MarkerPairs[marker]);
			}
		}

		public void Show()
		{
			if (EnableSimulation) Simulate();
			else ProcessAR(true);
		}

		private void ProcessAR(bool withBackeground)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			Bitmap frame = InputSource.GetInputFrame();
			if (withBackeground)
			{
				background.SetImage(frame);
				background.Render();
			}

			foreach (MarkerDetector detector in markerScene.Keys)
			{
				if (detector.Detect(frame.ToMat(), out MarkerTrackingInfo info))
				{
					info.ComputePose();
					if (lastInfo != null) info.SmoothWith(lastInfo);
					lastInfo = info;
					info.ComputeMatrix();

					camera.Update(
						info.ViewMatrix,
						info.GetProjectionMatrix(frame.Width, frame.Height),
						info.CameraPosition);

					if (EnableLightTracking)
					{
						ILight[] predictLights = LightSourceTracker.PredictLightSource(detector.MarkerMat, info);
						markerScene[detector].Lights.AddRange(predictLights);
						markerScene[detector].Render(camera);
						markerScene[detector].Lights.RemoveAt(markerScene[detector].Lights.Count - 1);
						markerScene[detector].Lights.RemoveAt(markerScene[detector].Lights.Count - 1);
					}
					else markerScene[detector].Render(camera);
				}
			}

			GC.Collect();
		}

		private void Simulate()
		{
			if (colorHarmonize is null || colorHarmonize.IsDisposed) 
				colorHarmonize = new ColorHarmonization();

			ProcessAR(true);
			//截圖(有背景)
			Bitmap ARframe = Screenshot();
			//由於套件需要所以轉成mat
			Mat input_img = ARframe.ToMat();

			//Preprocess
			input_img = colorHarmonize.inputImg_Process(input_img);

			//這個沒有布林值的只能在 #175行有布林值的ProcessAR執行後才能執行
			//這個只有截AR物體，借用 #175行已經計算過的值所以每次大概只有0~1ms之間
			ProcessAR(false);
			//一樣截圖了轉Mat
			Bitmap objectOnly = Screenshot();
			Mat mask = objectOnly.ToMat();
			mask = colorHarmonize.maskImg_Process(mask);
			//把截下來的圖傳去做前處理，因爲background.SetImage()需要bitmap所以回傳回來又.ToBitmap了
			Mat output = colorHarmonize.netForward_Process(input_img, mask);
			output = colorHarmonize.outputImg_Process(output, windowWidth, windowHeight);
			background.SetImage(output.ToBitmap());
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			background.Render();
		}

		public Bitmap Screenshot()
		{
			Bitmap output = new Bitmap(windowWidth, windowHeight);
			BitmapData data = output.LockBits(
				new System.Drawing.Rectangle(0, 0, output.Width, output.Height),
				ImageLockMode.WriteOnly,
				System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, output.Width, output.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
			output.UnlockBits(data);
			output.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
			return output;
		}

		public void SetOutputSize(int Width, int Height)
		{
			windowWidth = Width;
			windowHeight = Height;
			GL.Viewport(0, 0, windowWidth, windowHeight);
			camera.AspectRatio = WindowAspectRatio;
		}
	}
}
