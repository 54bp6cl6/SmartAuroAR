using OpenTK;

namespace SmartAutoAR.VirtualObject.Cameras
{
	/// <summary>
	/// 定義此介面以抽象鏡頭的概念，可能有人想透過鏡頭完玩一些花樣
	/// </summary>
	public interface ICamera
	{
		public Matrix4 ViewMatrix { get; }
		public Matrix4 ProjectionMatrix { get; }
		public Vector3 Position { get; }
		public float AspectRatio { get; set; }

		public void Update(Matrix4 viewMat, Matrix4 projectionMat, Vector3 position);
	}
}
