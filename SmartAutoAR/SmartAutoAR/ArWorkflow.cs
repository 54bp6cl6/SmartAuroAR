using SmartAutoAR.InputSource;
using System.Collections.Generic;
using SmartAutoAR.VirtualObject;
using OpenTK.Graphics.OpenGL4;
using Bitmap = System.Drawing.Bitmap;
using OpenTK;
using SmartAutoAR.VirtualObject.Cameras;

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
		public ICamera Camera { get; protected set; }

		// 儲存最後一個結果
		private bool have_last;
		private List<Bitmap> last_markers;

		public float WindowAspectRatio { get { return background.AspectRatio; } }

		protected Background background;

		public ArWorkflow(IInputSource inputSource)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, IScene>();
			MarkerDetector = new MarkerDetector();
			background = new Background();
			Camera = new ArCamera();
			last_markers = new List<Bitmap>();
			have_last = false;
		}

		public void Show()
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			Bitmap frame = InputSource.GetInputFrame();
			background.SetImage(frame);
			background.Render();
			last_markers.Clear();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				if (MarkerDetector.Detecte(frame, marker))
				{
					// 偵測到 marker
					/*Camera.Update(
						Matrix4.LookAt(
							new Vector3(4.145535f,8.7422075f,-0.42383f), 
							new Vector3(0.0f,0f,0.0f), 
							Vector3.UnitY),
						new Vector3(4.145535f, 8.7422075f, 0.42383f));*/
					Camera.Update(
						Matrix4.LookAt(
							new Vector3(3f, 6f, 3f),
							new Vector3(0.8f, 0f, 0.8f),
							Vector3.UnitY),
						new Vector3(3f, 6f, 3f));
					MarkerPairs[marker].Render(Camera);
					last_markers.Add(marker);
					have_last = true;
				}
			}
		}

		public void ShowLast()
		{
			if (have_last)
			{
				GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
				background.Render();
				foreach(Bitmap marker in last_markers)
				{
					MarkerPairs[marker].Render(Camera);
				}
			}
			else
			{
				Show();
			}
		}

		public void Resize(int Width, int Height)
		{
			GL.Viewport(0, 0, Width, Height);
			Camera.AspectRatio = (float)Width / (float)Height;
		}
	}
}
