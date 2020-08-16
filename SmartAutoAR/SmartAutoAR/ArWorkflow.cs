using System.Drawing;
using SmartAutoAR.InputSource;
using System.Collections.Generic;
using SmartAutoAR.VirtualObject;
using OpenTK.Graphics.OpenGL4;
using Bitmap = System.Drawing.Bitmap;
using OpenTK;

namespace SmartAutoAR
{
	/// <summary>
	/// 整合各種功能，能夠快速製作AR的類別
	/// </summary>
	public class ArWorkflow
	{
		public IInputSource InputSource { get; set; }
		public Dictionary<Bitmap, IScene> MarkerPairs { get; set; }
		public MarkerDetector MarkerDetector { get; protected set; }
		public double AspectRatio { get; protected set; }


		protected Background background;

		public ArWorkflow(IInputSource inputSource)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, IScene>();
			MarkerDetector = new MarkerDetector();
			background = new Background();
		}

		public void DoWork()
		{
			Bitmap frame = InputSource.GetInputFrame();
			background.SetImage(frame);
			background.Render();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				if (MarkerDetector.Detecte(frame, marker))
				{
					// 偵測到 marker
					MarkerPairs[marker].Camera.Update(MarkerDetector.ViewMatrix, MarkerDetector.CameraPosition);
					//MarkerPairs[marker].Camera.Update(Matrix4.LookAt(new Vector3(0,0,10), Vector3.Zero, Vector3.UnitY), new Vector3(0, 0, 10));
					MarkerPairs[marker].Render((float)AspectRatio);
				}
			}
		}

		public void Resize(int Width, int Height)
		{
			GL.Viewport(0, 0, Width, Height);
			AspectRatio = Width / Height;
		}
	}
}
