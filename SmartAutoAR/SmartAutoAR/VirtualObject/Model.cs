using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using SmartAutoAR.VirtualObject.Base;
using Mesh = SmartAutoAR.VirtualObject.Base.Mesh;

namespace SmartAutoAR.VirtualObject
{
	/// <summary>
	/// 儲存與管理模型資料之類別，並提供匯入模型檔之功能
	/// </summary>
	public class Model : IDisposable
	{
		public List<Mesh> Meshes { get; }
		public Matrix4 ModelMatrix { get; set; }
		protected string filepath;

		public Model()
		{
			Meshes = new List<Mesh>();
			ModelMatrix = Matrix4.Identity;
		}

		/// <summary>
		/// 以指定的度數以三軸旋轉模型
		/// </summary>
		/// <param name="x">以x軸旋轉的度數</param>
		/// <param name="y">以y軸旋轉的度數</param>
		/// <param name="z">以z軸旋轉的度數</param>
		public void Rotation(float x = 0f, float y = 0f, float z = 0f)
		{
			ModelMatrix = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(x)) * ModelMatrix;
			ModelMatrix = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(y)) * ModelMatrix;
			ModelMatrix = Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(z)) * ModelMatrix;
		}

		/// <summary>
		/// 以指定的距離在三軸上移動模型
		/// </summary>
		/// <param name="x">依x軸移動的距離</param>
		/// <param name="y">依y軸移動的距離</param>
		/// <param name="z">依z軸移動的距離</param>
		public void Move(float x = 0f, float y = 0f, float z = 0f)
		{
			ModelMatrix *= Matrix4.CreateTranslation(x, y, z);
		}

		/// <summary>
		/// 以指定的比率縮小模型
		/// </summary>
		/// <param name="percent">縮小比率</param>
		public void Resize(float percent)
		{
			foreach (Mesh mesh in Meshes)
			{
				mesh.Resize(percent);
			}
		}

		/// <summary>
		/// 在畫面上渲染模型
		/// </summary>
		/// <param name="shader">欲使用的著色器</param>
		public void Render(Shader shader)
		{
			Matrix4 temp = ModelMatrix;
			GL.UniformMatrix4(shader.GetUniformLocation("model"), false, ref temp);
			foreach (Mesh mesh in Meshes)
			{
				mesh.Render(shader);
			}
		}

		/// <summary>
		/// 從指定檔案路徑匯入模型檔
		/// </summary>
		/// <param name="path">模型檔案路徑</param>
		/// <returns>匯入之 Model 物件</returns>
		public static Model LoadModel(string path)
		{
			AssimpContext importer = new AssimpContext();
			Assimp.Scene aiScene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateNormals);
			if (aiScene.Equals(null) || aiScene.SceneFlags == SceneFlags.Incomplete || aiScene.RootNode.Equals(null))
			{
				throw new FileLoadException();
			}

			Model model = new Model()
			{
				filepath = path
			};
			ProcessNode(aiScene.RootNode, aiScene, ref model);
			model.Resize(0.01f);
			return model;
		}

		/// <summary>
		/// 讀取 Assimp 模型節點
		/// </summary>
		/// <param name="node">Assimp 模型節點</param>
		/// <param name="scene">Assimp 場景物件</param>
		/// <param name="model">輸出模型</param>
		private static void ProcessNode(Node node, Assimp.Scene scene, ref Model model)
		{
			for (int i = 0; i < node.MeshCount; i++)
			{
				Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
				model.Meshes.Add(ProcessMesh(mesh, scene, model.filepath));
			}
			for (int i = 0; i < node.ChildCount; i++)
			{
				ProcessNode(node.Children[i], scene, ref model);
			}
		}

		/// <summary>
		/// 讀取 Assimp Mesh 物件
		/// </summary>
		/// <param name="mesh">Assimp Mesh 物件</param>
		/// <param name="scene">Assimp 場景物件</param>
		/// <param name="filepath">模型檔案路徑</param>
		/// <returns>模型中的一小部分(Mesh)</returns>
		/// <improvable>
		/// 目前只拿檔案中的 vertex、indices、material 與 Texture 資料
		/// </improvable>
		private static Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene, string filepath)
		{
			List<Vertex> vertices = new List<Vertex>();
			List<uint> indices = new List<uint>();

			// vertex
			for (int i = 0; i < mesh.VertexCount; i++)
			{
				Vertex vertex = new Vertex()
				{
					position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z),
					normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z)
				};
				if (mesh.TextureCoordinateChannelCount > 0)
				{
					vertex.texCoord = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
				}
				vertices.Add(vertex);
			}
			Mesh output = new Mesh(vertices.ToArray());

			// indices
			if (mesh.HasFaces)
			{
				for (int i = 0; i < mesh.FaceCount; i++)
				{
					Face face = mesh.Faces[i];
					for (int j = 0; j < face.IndexCount; j++)
					{
						indices.Add((uint)face.Indices[j]);
					}
				}
				output.SetIndices(indices.ToArray());
			}

			// material
			if (mesh.MaterialIndex >= 0)
			{
				Assimp.Material material = scene.Materials[mesh.MaterialIndex];
				output.Material = new Base.Material(
					material.HasColorAmbient ? new Color4(material.ColorAmbient.R, material.ColorAmbient.G, material.ColorAmbient.B, material.ColorAmbient.A) : Color4.White,
					material.HasColorDiffuse ? new Color4(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A) : Color4.White,
					material.HasColorSpecular ? new Color4(material.ColorSpecular.R, material.ColorSpecular.G, material.ColorSpecular.B, material.ColorSpecular.A) : Color4.White,
					material.HasShininess ? material.Shininess : 32
				);
				if (material.HasTextureDiffuse)
				{
					material.GetMaterialTexture(TextureType.Diffuse, 0, out TextureSlot texture1);
					output.SetTexture(Texture.FromFile(@$"{filepath}\..\{texture1.FilePath}"));
				}
			}

			return output;
		}

		/// <summary>
		/// 釋放資源
		/// </summary>
		public void Dispose()
		{
			foreach (Mesh mesh in Meshes)
			{
				mesh.Dispose();
			}
		}
	}
}
