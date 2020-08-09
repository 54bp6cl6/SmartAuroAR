using System;
using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	class ArCamera : ICamera
	{
		public Matrix4 ViewMatrix { get; set; }
		public Vector3 Position { get; protected set; }

		public void Update(Matrix4 matrix, Vector3 position)
		{
			ViewMatrix = matrix;
			Position = position;
		}

		public Matrix4 GetProjectionMatrix(float aspectRatio)
		{
			return Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, aspectRatio, 0.01f, 10000f);
		}
	}
}
