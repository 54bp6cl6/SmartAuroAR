using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using SmartAutoAR.VirtualObject.Base;

namespace SmartAutoAR.VirtualObject.Lights
{
	/// <summary>
	/// 提供全局照明的環境光元件
	/// </summary>
	public class AmbientLight : ILight
	{
		public Color4 Color { get; set; }
		public float Main_strength { get; set; }

		/// <summary>
		/// 以指定的顏色與光照強度初始化物件
		/// </summary>
		/// <param name="color">環境光顏色</param>
		/// <param name="main_strength">光照強度</param>
		public AmbientLight(Color4 color, float main_strength)
		{
			Color = color;
			Main_strength = main_strength;
		}

		public void SetShader(Shader shader, int index)
		{
			GL.Uniform4(shader.GetUniformLocation($"ambient_lights[{index}].color"), Color);
			GL.Uniform1(shader.GetUniformLocation($"ambient_lights[{index}].main_strength"), Main_strength);
		}
	}
}
