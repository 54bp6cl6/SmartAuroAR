using SmartAutoAR.InputSource;
using System.Collections.Generic;
using SmartAutoAR.VirtualObject;
using OpenTK.Graphics.OpenGL4;
using Bitmap = System.Drawing.Bitmap;
using SmartAutoAR.VirtualObject.Cameras;
using System;

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

		public bool Show(bool backeground = true)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			Bitmap frame = InputSource.GetInputFrame();
			background.SetImage(frame);
			if (backeground) background.Render();
			last_markers.Clear();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				if (MarkerDetector.Detecte(frame, marker))
				{
					// 偵測到 marker
					Camera.Update(MarkerDetector.ViewMatrix, MarkerDetector.CameraPosition);
					MarkerPairs[marker].Render(Camera);
					last_markers.Add(marker);
					have_last = true;
				}
			}
			GC.Collect();
			return true;
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

		public void Resize(int Width, int Height)
		{
			GL.Viewport(0, 0, Width, Height);
			Camera.AspectRatio = (float)Width / (float)Height;
		}
	}
}
