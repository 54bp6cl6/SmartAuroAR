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

namespace SmartAutoAR
{
	/// <summary>
	/// 整合各種功能，能夠快速製作AR的類別
	/// </summary>
	public class ArWorkflow
	{
		public IInputSource InputSource { get; set; }
		public Dictionary<Bitmap, IScene> MarkerPairs { get; set; }
		public ICamera Camera { get; protected set; }
		public PatternDetector patternDetector { get; protected set; }
		public PatternTrackingInfo patternTrackinginfo { get; protected set; }
		public Pattern pattern;

		// 儲存最後一個結果
		private bool have_last;
		private List<Bitmap> last_markers;
		public float WindowAspectRatio { get { return background.AspectRatio; } }
		protected Background background;

		public ArWorkflow(IInputSource inputSource, Bitmap marker)
		{
			InputSource = inputSource;
			MarkerPairs = new Dictionary<Bitmap, IScene>();
			background = new Background();
			Camera = new ArCamera();
			last_markers = new List<Bitmap>();
			have_last = false;
			SetMarker(marker);
		}

		public void Show(bool backeground = true)
		{
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			Bitmap frame = InputSource.GetInputFrame();
			background.SetImage(frame);
			if (backeground) background.Render();
			last_markers.Clear();
			foreach (Bitmap marker in MarkerPairs.Keys)
			{
				if (patternDetector.findPattern(frame.ToMat(), patternTrackinginfo))
				{
					patternTrackinginfo.ComputePose();
					// 偵測到 marker
					Camera.Update(
						patternTrackinginfo.ViewMatrix,
						patternTrackinginfo.GetProjectionMatrix(frame.Width,frame.Height),
						patternTrackinginfo.CameraPosition);
					MarkerPairs[marker].Render(Camera);
					last_markers.Add(marker);
					have_last = true;
				}
			}
			GC.Collect();
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

		private void SetMarker(Bitmap marker)
		{
			Mat MARKER = marker.ToMat();
			Cv2.Resize(MARKER, MARKER, new OpenCvSharp.Size(400, 400));
			pattern = new Pattern();
			patternTrackinginfo = new PatternTrackingInfo();
			patternDetector = new PatternDetector(true);
			patternDetector.buildPatternFromImage(MARKER, pattern);
			patternDetector.train();
		}
	}
}
