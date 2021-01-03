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
		/// 是否啟用色彩融合模組
		/// </summary>
		public bool EnableColorHarmonizing
		{
			get { return _enableColorHarmonizing; }
			set
			{
				_enableColorHarmonizing = value;
				if (!value && !(colorHarmonize is null) && !colorHarmonize.IsDisposed)
					colorHarmonize.Dispose();
			}
		}

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
		/// 用於對影像作色彩融合的物件
		/// </summary>
		protected ColorHarmonization colorHarmonize;

		/// <summary>
		/// 用於渲染背景圖片(輸入影像)的物件
		/// </summary>
		protected Background background;

		private bool _enableColorHarmonizing;

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
			foreach (MarkerDetector detector in markerScene.Keys)
			{
				detector.lastInfo = null;
			}
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
		public void Show(bool nextFrame = true)
		{
			Bitmap inputFrame = nextFrame ? InputSource.GetNextFrame() : InputSource.LastFrame;
			List<Tuple<MarkerTrackingInfo, Scene>> infoScene_tuples = DetectMarkers(inputFrame);

			if (EnableColorHarmonizing)
			{
				if (colorHarmonize is null || colorHarmonize.IsDisposed)
					colorHarmonize = new ColorHarmonization();
				Simulate(inputFrame, infoScene_tuples);
			}
			else
			{
				ProcessAR(inputFrame, infoScene_tuples);
			}
		}

		/// <summary>
		/// 偵測並計算輸入畫面中出現的marker資訊
		/// </summary>
		private List<Tuple<MarkerTrackingInfo, Scene>> DetectMarkers(Bitmap inputFrame)
		{
			List<Tuple<MarkerTrackingInfo, Scene>> infoScene = new List<Tuple<MarkerTrackingInfo, Scene>>();

			foreach (MarkerDetector detector in markerScene.Keys)
			{
				if (detector.Detect(inputFrame.ToMat(), out MarkerTrackingInfo info))
				{
					info.ComputePose();
					if (detector.lastInfo != null) info.SmoothWith(detector.lastInfo);
					detector.lastInfo = info;
					info.ComputeMatrix();
					infoScene.Add(new Tuple<MarkerTrackingInfo, Scene>(info, markerScene[detector]));
				}
			}

			return infoScene;
		}

		/// <summary>
		/// 根據 marker 偵測結果渲染畫面
		/// </summary>
		/// <param name="inputFrame">環境(背景)影像</param>
		/// <param name="infoScene_tuples">DetectMarkers()之偵測結果</param>
		/// <param name="forMask">此項為 true 則只渲染無擬真效果之虛擬物體</param>
		private void ProcessAR(Bitmap inputFrame, List<Tuple<MarkerTrackingInfo, Scene>> infoScene_tuples, bool forMask = false)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			if (!forMask)
			{
				background.SetImage(inputFrame);
				background.Render();
			}

			foreach (Tuple<MarkerTrackingInfo, Scene> infoScene in infoScene_tuples)
			{
				camera.Update(
					infoScene.Item1.ViewMatrix,
					infoScene.Item1.GetProjectionMatrix(inputFrame.Width, inputFrame.Height),
					infoScene.Item1.CameraPosition);

				// 為防止取得 mask 時沒有打燈，畫面會是全黑的，影響後續運作，因此給他個白光
				if (forMask)
				{
					infoScene.Item2.Lights.Add(new AmbientLight(OpenTK.Graphics.Color4.White, 1.0f));
					infoScene.Item2.Render(camera);
					infoScene.Item2.Lights.RemoveAt(infoScene.Item2.Lights.Count - 1);
				}
				else if (EnableLightTracking)
				{
					ILight[] predictLights = LightSourceTracker.PredictLightSource(infoScene.Item1.MarkerImageMat, infoScene.Item1);
					infoScene.Item2.Lights.AddRange(predictLights);
					infoScene.Item2.Render(camera);
					infoScene.Item2.Lights.RemoveAt(infoScene.Item2.Lights.Count - 1);
					infoScene.Item2.Lights.RemoveAt(infoScene.Item2.Lights.Count - 1);
				}
				else infoScene.Item2.Render(camera);
			}

			GC.Collect();
		}

		/// <summary>
		/// 使用色彩融合模組對AR影像進行擬真畫處理
		/// </summary>
		/// <param name="inputFrame">環境(背景)影像</param>
		/// <param name="infoScene_tuples">DetectMarkers()之偵測結果</param>
		private void Simulate(Bitmap inputFrame, List<Tuple<MarkerTrackingInfo, Scene>> infoScene_tuples)
		{
			// 取得遮罩
			ProcessAR(inputFrame, infoScene_tuples, forMask: true);
			Mat mask = Screenshot().ToMat(); // 由於套件需要所以轉成mat
			mask = colorHarmonize.MaskImgProcess(mask);

			// 取得 AR 影像
			ProcessAR(inputFrame, infoScene_tuples, false);
			Mat input_img = Screenshot().ToMat(); // 由於套件需要所以轉成mat
			input_img = colorHarmonize.InputImgProcess(input_img); // Preprocess

			// 把截下來的圖傳去做前處理
			Mat output = colorHarmonize.NetForwardProcess(input_img, mask);
			output = colorHarmonize.OutputImgProcess(output, windowWidth, windowHeight);

			// 因爲background.SetImage()需要bitmap所以回傳回來又.ToBitmap了
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
		public void SetOutputSize(int width, int Height)
		{
			windowWidth = width;
			windowHeight = Height;
			GL.Viewport(0, 0, windowWidth, windowHeight);
			camera.AspectRatio = WindowAspectRatio;
		}
	}
}
