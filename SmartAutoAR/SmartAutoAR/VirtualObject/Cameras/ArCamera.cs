using System;
using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	class ArCamera : ICamera
	{
		public Matrix4 ViewMatrix { get; set; }
		public Vector3 Position { get; set; }
		public float AspectRatio { get; set; }

		public void Update(Matrix4 matrix, Vector3 position)
		{
			ViewMatrix = matrix;
			Position = position;
		}

		public Matrix4 GetProjectionMatrix()
		{
			return Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, AspectRatio, 0.01f, 10000f);
		}
	}
}
