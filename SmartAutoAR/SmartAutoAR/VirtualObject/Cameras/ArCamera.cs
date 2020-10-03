using System;
using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	class ArCamera : ICamera
	{
		public Matrix4 ViewMatrix { get; set; }
		public Matrix4 ProjectionMatrix { get; set; }
		public Vector3 Position { get; set; }
		public float AspectRatio { get; set; }

		public void Update(Matrix4 viewMat, Matrix4 projectionMat, Vector3 position)
		{
			ViewMatrix = viewMat;
			ProjectionMatrix = projectionMat;
			Position = position;
		}
	}
}
