using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	public interface ICamera
	{
		public Matrix4 ViewMatrix { get; }
		public Vector3 Position { get; }

		public void Update(Matrix4 matrix, Vector3 position);

		public Matrix4 GetProjectionMatrix(float aspectRatio);
	}
}
