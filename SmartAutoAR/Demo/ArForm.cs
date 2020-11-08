using System;
using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR;
using SmartAutoAR.InputSource;
using SmartAutoAR.VirtualObject;
using SmartAutoAR.VirtualObject.Lights;
using Bitmap = System.Drawing.Bitmap;

namespace Demo
{
	public partial class ArForm : GameWindow
	{
		IInputSource inputSource;
		Model model;
		Scene scene;
		ArWorkflow workflow;
		bool spin;

		public ArForm() :
			base(800, 600,
				GraphicsMode.Default,
				"SmartAutoAR",
				GameWindowFlags.Default,
				DisplayDevice.Default,
				4, 5,
				GraphicsContextFlags.ForwardCompatible)
		{ }

		public enum Input { Image, Video, Stream }
		public void SetParams(Input input, string inputPath, string markerPath, string modelPath, 
			float resizeRate, float rotationX, float rotationY, float rotationZ, bool lightTracking, bool colorHarmonizing, bool spin)
		{
			if (input == Input.Image) inputSource = new ImageSource(inputPath);
			else if (input == Input.Video) inputSource = new VideoSource(inputPath);
			else inputSource = new StreamSource(int.Parse(inputPath));

			workflow = new ArWorkflow(inputSource);
			scene = new Scene();
			model = Model.LoadModel(modelPath);
			model.Resize(resizeRate);
			model.Rotation(x: rotationX, y: rotationY, z: rotationZ);
			scene.Models.Add(model);
			if(!lightTracking) scene.Lights.Add(new AmbientLight(Color4.White, 0.8f));

			Bitmap marker = new Bitmap(markerPath);
			workflow.MarkerPairs[marker] = scene;
			workflow.TrainMarkers();

			workflow.EnableLightTracking = lightTracking;
			workflow.EnableColorHarmonizing = colorHarmonizing;
			this.spin = spin;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}


		protected override void OnRenderFrame(FrameEventArgs e)
		{
			// 確保視窗比例與背景一致
			Width = (int)(Height * workflow.WindowAspectRatio);

			if (spin) model.Rotation(y: 5);

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