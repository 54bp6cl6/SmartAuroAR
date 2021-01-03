namespace SmartAutoAR.VirtualObject.Lights
{
	/// <summary>
	/// 此介面定義了燈光元件的行為
	/// </summary>
	public interface ILight
	{
		/// <summary>
		/// 將燈光係數寫入著色器
		/// </summary>
		/// <param name="shader">欲寫入的 Shader 物件</param>
		/// <param name="index">指定Shader內燈光陣列之位置</param>
		void SetShader(Base.Shader shader, int index);
	}
}
