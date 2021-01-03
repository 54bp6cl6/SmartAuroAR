using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace SmartAutoAR.VirtualObject.Base
{
	/// <summary>
	/// 用於管理著色器的類別
	/// </summary>
	public class Shader : IDisposable
	{
		public static Shader StandardShader
		{
			get
			{
				return new Shader(@"Resources\Shaders\standard_vert.shader",
								  @"Resources\Shaders\standard_frag.shader");
			}
		}

		public static Shader BackgroundShader
		{
			get
			{
				return new Shader(@"Resources\Shaders\background_vert.shader",
								  @"Resources\Shaders\background_frag.shader");
			}
		}

		protected readonly int handle;

		/// <summary>
		/// 以指定的頂點著色器與片段著色器初始化物件
		/// </summary>
		/// <param name="vertexPath">頂點著色器檔案路徑</param>
		/// <param name="fragmentPath">片段著色器檔案路徑</param>
		public Shader(string vertexPath, string fragmentPath)
		{
			// vertex shader
			string vertexShaderSource = File.ReadAllText(vertexPath);
			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexShaderSource);
			GL.CompileShader(vertexShader);
			string infoLogVert = GL.GetShaderInfoLog(vertexShader);
			if (infoLogVert != string.Empty) throw new Exception(infoLogVert);

			// fragment shader
			string fragmentShaderSource = File.ReadAllText(fragmentPath);
			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentShaderSource);
			GL.CompileShader(fragmentShader);
			string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
			if (infoLogFrag != string.Empty) throw new Exception(infoLogFrag);

			handle = GL.CreateProgram();
			GL.AttachShader(handle, vertexShader);
			GL.AttachShader(handle, fragmentShader);
			GL.LinkProgram(handle);

			GL.DetachShader(handle, vertexShader);
			GL.DetachShader(handle, fragmentShader);
			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		/// <summary>
		/// 呼叫顯示卡使用此著色器程式
		/// </summary>
		public void Use()
		{
			GL.UseProgram(handle);
		}

		public int GetUniformLocation(string name)
		{
			Use();
			return GL.GetUniformLocation(handle, name);
		}

		public void Dispose()
		{
			GL.DeleteProgram(handle);
			GC.SuppressFinalize(this);
		}
	}
}
