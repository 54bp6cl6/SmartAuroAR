using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SmartAutoAR.VirtualObject;
using SmartAutoAR.VirtualObject.Base;
using SmartAutoAR.VirtualObject.Cameras;
using SmartAutoAR.VirtualObject.Lights;

namespace SmartAutoAR
{
	/// <summary>
	/// 用於管理模型與燈光的類別
	/// </summary>
	public class Scene
	{
		public List<Model> Models { get; set; }
		public List<ILight> Lights { get; set; }

		protected Shader shader;

		public Scene()
		{
			Models = new List<Model>();
			Lights = new List<ILight>();
			shader = Shader.StandardShader;
		}

		public void Render(ICamera camera)
		{
			// 開啟深度檢測
			GL.Enable(EnableCap.DepthTest);
			//GL.Clear(ClearBufferMask.DepthBufferBit);

			// 設定攝影機
			Matrix4 temp = camera.ViewMatrix;
			GL.UniformMatrix4(shader.GetUniformLocation("view"), false, ref temp);
			temp = camera.ProjectionMatrix;
			GL.UniformMatrix4(shader.GetUniformLocation("projection"), false, ref temp);
			GL.Uniform3(shader.GetUniformLocation("view_position"), camera.Position);

			// 設定光源
			Vector3 count = new Vector3(0, 0, 0);
			for (int i = 0; i < Lights.Count; i++)
			{
				if (Lights[i] is AmbientLight && count.X < 8)
				{
					count.X += 1;
					Lights[i].SetShader(shader, (int)count.X - 1);
				}
				else if (Lights[i] is PointLight && count.Y < 8)
				{
					count.Y += 1;
					Lights[i].SetShader(shader, (int)count.Y - 1);
				}
				else if (Lights[i] is DirectionalLight && count.Z < 8)
				{
					count.Z += 1;
					Lights[i].SetShader(shader, (int)count.Z - 1);
				}
			}
			GL.Uniform3(shader.GetUniformLocation("lights_num"), count);

			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].Render(shader);
			}
		}

		public void Dispose()
		{
			foreach (Model model in Models) model.Dispose();
		}
	}
}
