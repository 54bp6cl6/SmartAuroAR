﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace SmartAutoAR.VirtualObject.Base
{
	/// <summary>
	/// 用於管理貼圖的類別
	/// </summary>
	public class Texture : IDisposable
	{
		protected readonly int Handle;
		public double Width { get; protected set; }
		public double Height { get; protected set; }

		public Texture()
		{
			Handle = GL.GenTexture();
			Width = 0;
			Height = 0;
		}

		/// <summary>
		/// 從指定檔案路徑導入貼圖
		/// </summary>
		/// <param name="file">貼圖檔案路徑</param>
		/// <returns>導入之 Texture 物件</returns>
		public static Texture FromFile(string file)
		{
			Texture texture = new Texture();
			using (var image = new Bitmap(file))
			{
				texture.SetImage(image);
			}
			return texture;
		}

		/// <summary>
		/// 設定貼圖圖像
		/// </summary>
		/// <param name="image">欲設定之圖像</param>
		public void SetImage(Bitmap image)
		{
			Use();

			Bitmap CopyImage = new Bitmap(image); 

			Width = CopyImage.Width;
			Height = CopyImage.Height;

			try
			{
				var data = CopyImage.LockBits(
					new Rectangle(0, 0, CopyImage.Width, CopyImage.Height),
					ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				GL.TexImage2D(TextureTarget.Texture2D,
					0,
					PixelInternalFormat.Rgba,
					CopyImage.Width,
					CopyImage.Height,
					0,
					PixelFormat.Bgra,
					PixelType.UnsignedByte,
					data.Scan0);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			}
			catch { }
		}

		public void Use(TextureUnit unit = TextureUnit.Texture0)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}

		public void Dispose()
		{
			GL.DeleteTexture(Handle);
			GC.SuppressFinalize(this);
		}
	}
}
