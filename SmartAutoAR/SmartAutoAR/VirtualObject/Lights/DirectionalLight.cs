using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using SmartAutoAR.VirtualObject.Base;

namespace SmartAutoAR.VirtualObject.Lights
{
	/// <summary>
	/// 提供全局同向照明的平行光元件
	/// </summary>
	public class DirectionalLight : ILight
	{
		public Color4 Color { get; set; }
		public Vector3 Direction { get; set; }
		public float Main_strength { get; set; }
		public float Specular_strength { get; set; }

		/// <summary>
		/// 以指定的顏色、光源方向、光照強度與鏡面反射強度初始化物件
		/// </summary>
		/// <param name="color">光照顏色</param>
		/// <param name="position">相對於原點之假想光源位置</param>
		/// <param name="main_strength">光照強度</param>
		/// <param name="specular_strength">鏡面反射強度</param>
		public DirectionalLight(Color4 color, Vector3 position, float main_strength, float specular_strength)
		{
			Color = color;
			Direction = position * -1;
			Main_strength = main_strength;
			Specular_strength = specular_strength;
		}

		public void SetShader(Shader shader, int index)
		{
			GL.Uniform4(shader.GetUniformLocation($"directional_lights[{index}].color"), Color);
			GL.Uniform3(shader.GetUniformLocation($"directional_lights[{index}].direction"), Direction);
			GL.Uniform1(shader.GetUniformLocation($"directional_lights[{index}].main_strength"), Main_strength);
			GL.Uniform1(shader.GetUniformLocation($"directional_lights[{index}].specular_strength"), Specular_strength);
		}
	}
}
