using System;
using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR;
using SmartAutoAR.InputSource;
using SmartAutoAR.VirtualObject;
using SmartAutoAR.VirtualObject.Lights;
using Bitmap = System.Drawing.Bitmap;

namespace Debug
{
	public partial class ArForm : GameWindow
	{
		IInputSource inputSource;
		Scene scene;
		Model model;
		ArWorkflow workflow;

		public ArForm(int width, int height, string title) :
			base(width, height,
				GraphicsMode.Default,
				title,
				GameWindowFlags.Default,
				DisplayDevice.Default,
				4, 5,
				GraphicsContextFlags.ForwardCompatible)
		{ }

		protected override void OnLoad(EventArgs e)
		{
			// 設定影像輸入
			inputSource = new ImageSource(@"..\..\..\resources\image_test.jpg");
			//inputSource = new VideoSource(@"..\..\..\resources\video_test.mp4");
			//inputSource = new StreamSource();

			// 創建場景
			scene = new Scene();
			model = Model.LoadModel(@"..\..\..\resources\models\Cyclopropane\Cyclopropane.obj"); // 請輸入您的模型路徑
			//model = Model.LoadModel(@"..\..\..\resources\models\IronMan\IronMan.obj"); // 請輸入您的模型路徑
			scene.Models.Add(model);

			// 調整模型大小
			model.Resize(0.00035f);
			model.Rotation(x: -100f, z: -70);

			// 加入燈光
			// scene.Lights.Add(new AmbientLight(Color4.White, 0.8f));

			// 建立 workflow 物件
			workflow = new ArWorkflow(inputSource);

			// 設定 marker 對應的 scene
			Bitmap marker = new Bitmap(@"..\..\..\resources\marker.png"); // 請輸入您 Marker 圖檔的路徑
			workflow.MarkerPairs[marker] = scene;
			workflow.TrainMarkers(); // 修改後一定要執行!!

			// 開啟光源追蹤模組
			workflow.EnableLightTracking = true;

			// 開啟色彩調合模組
			//workflow.EnableColorHarmonizing = true;

			base.OnLoad(e);
		}


		protected override void OnRenderFrame(FrameEventArgs e)
		{
			// 確保視窗比例與背景一致
			Width = (int)(Height * workflow.WindowAspectRatio);

			//model.Rotation(y: 5);

			// 對下一幀做處理，包含偵測、渲染、擬真
			if (inputSource is VideoSource && (inputSource as VideoSource).EndOfVideo)
			{
				(inputSource as VideoSource).Replay();
				workflow.ClearState();
			}
			workflow.Show();
			//workflow.ShowMarker();

			// 針對視窗本身做繪製
			SwapBuffers();

			base.OnRenderFrame(e);
		}

		protected override void OnResize(EventArgs e)
		{
			workflow.SetOutputSize(Width, Height);

			base.OnResize(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			scene.Dispose();

			base.OnUnload(e);
		}

		private Tuple<IInputSource, Model> Cat()
		{
			Model cat = Model.LoadModel(@"..\..\..\resources\models\cat\12221_Cat_v1_l3.obj");
			model.Resize(0.0035f);
			model.Rotation(x: -100f, z: -70);

			return new Tuple<IInputSource, Model>(new ImageSource(@"..\..\..\resources\image_test.jpg"), model);
		}
	}
}