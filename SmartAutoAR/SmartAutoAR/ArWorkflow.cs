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
		/// <summary>
		/// 設定或取得影像輸入來源
		/// </summary>
		public IInputSource InputSource { get; set; }

		/// <summary>
		/// 設定或取得Marker與其對應的虛擬場景。須執行TrainMarker()後才會生效
		/// </summary>
		public Dictionary<Bitmap, Scene> MarkerPairs { get; set; }

		/// <summary>
		/// 是否啟用光源追蹤模組
		/// </summary>
		public bool EnableLightTracking { get; set; }

		/// <summary>
		/// 是否啟用色彩調和模組
		/// </summary>
		public bool EnableColorHarmonizing { 
			get { return _enableColorHarmonizing; }
			set { 
				_enableColorHarmonizing = value;
				if (!value && !(colorHarmonize is null) && !colorHarmonize.IsDisposed)
					colorHarmonize.Dispose();
			} }

		/// <summary>
		/// 取得輸出影像的長寬比
		/// </summary>
		public float WindowAspectRatio { get { return InputSource.AspectRatio; } }

		/// <summary>
		/// 輸出影像的大小
		/// </summary>
		protected int windowWidth, windowHeight;

		/// <summary>
		/// 真正參與運算的 Marker Scene 對應關係
		/// </summary>
		protected Dictionary<MarkerDetector, Scene> markerScene;

		/// <summary>
		/// 用於OpenTK渲染的虛擬鏡頭
		/// </summary>
		protected ICamera camera;

		/// <summary>
		/// 上一幀來源影像的計算結果
		/// </summary>
		protected MarkerTrackingInfo lastInfo;

		/// <summary>
		/// 用於對影像作色彩調和的物件
		/// </summary>
		protected ColorHarmonization colorHarmonize;

		/// <summary>
		/// 用於渲染背景圖片(輸入影像)的物件
		/// </summary>
		protected Background background;

		private bool _enableColorHarmonizing;
		private Bitmap arFrame;

		/// <summary>
		/// 以指定的影像輸入元件，初始化 SmartAutoAR.ArWorkFlow 類別的物件
		/// </summary>
		public ArWorkflow(IInputSource inputSource)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, Scene>();
			EnableColorHarmonizing = false;
			EnableLightTracking = false;
			camera = new ArCamera();
			background = new Background();
		}

		/// <summary>
		/// 將暫存資料清除
		/// </summary>
		public void ClearState()
		{
			lastInfo = null;
		}

		/// <summary>
		/// 設定並部署 Marker 與其關聯的虛擬場影，使 MarkerPairs 屬性的改動生效
		/// </summary>
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

		/// <summary>
		/// 在目前綁定的 OpenGL Context 上渲染 AR 畫面
		/// </summary>
		public void Show()
		{
			if (EnableColorHarmonizing)
			{
				if (colorHarmonize is null || colorHarmonize.IsDisposed)
					colorHarmonize = new ColorHarmonization();
				Simulate();
			}
			else
			{
				ProcessAR(true);
			}
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

		private void ProcessAR()
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			Bitmap frame = InputSource.GetInputFrame();

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

					GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);


					background.SetImage(frame);
					background.Render();

					if (EnableLightTracking)
					{
						ILight[] predictLights = LightSourceTracker.PredictLightSource(detector.MarkerMat, info);
						markerScene[detector].Lights.AddRange(predictLights);
						markerScene[detector].Render(camera);

						markerScene[detector].Lights.RemoveAt(markerScene[detector].Lights.Count - 1);
						markerScene[detector].Lights.RemoveAt(markerScene[detector].Lights.Count - 1);
						arFrame = Screenshot();
					}
					else markerScene[detector].Render(camera);

				}
			}

			GC.Collect();
		}

		private void Simulate()
		{
			ProcessAR();

			//由於套件需要所以轉成mat
			Mat input_img = arFrame.ToMat();

			//Preprocess
			input_img = colorHarmonize.inputImg_Process(input_img);

			//這個只有截AR物體
			//一樣截圖了轉Mat
			Mat mask = arFrame.ToMat();
			mask = colorHarmonize.maskImg_Process(mask);
			//把截下來的圖傳去做前處理，因爲background.SetImage()需要bitmap所以回傳回來又.ToBitmap了
			Mat output = colorHarmonize.netForward_Process(input_img, mask);
			output = colorHarmonize.outputImg_Process(output, windowWidth, windowHeight);
			background.SetImage(output.ToBitmap());
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			background.Render();
		}

		/// <summary>
		/// 輸出目前綁定之 OpenGL Context 上已經繪製的影像
		/// </summary>
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

		/// <summary>
		/// 設定輸出影像的大小
		/// </summary>
		public void SetOutputSize(int Width, int Height)
		{
			windowWidth = Width;
			windowHeight = Height;
			GL.Viewport(0, 0, windowWidth, windowHeight);
			camera.AspectRatio = WindowAspectRatio;
		}
	}
}
