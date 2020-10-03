using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	public interface ICamera
	{
		public Matrix4 ViewMatrix { get; }
		public Matrix4 ProjectionMatrix { get; }
		public Vector3 Position { get; }
		public float AspectRatio { get; set; }

		public void Update(Matrix4 viewMat, Matrix4 projectionMat, Vector3 position);
	}
}
