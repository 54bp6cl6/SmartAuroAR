using System;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SmartAutoAR
{
	/// <summary>
	/// 提供以機器學習技術對影像做色彩融合的效果
	/// 
	/// 使用步驟
	/// 1.呼叫 ColorHarmonization class起始
	/// 2.input需要兩張圖，一張背景圖，一張是虛擬物體的圖
	/// 2.1.把背景圖傳進去InputImgProcess，會回傳得到一個處理成能輸入caffe模組的Mat格式
	/// 2.2.把虛擬物體圖傳進去MaskImgProcess，會回傳得到一個處理成能輸入caffe模組的Mat格式
	/// 3.把2.1及2.2回傳回來的Mat傳進去NetForwardProcess，得到一張需要後製的Mat圖像
	/// 4.把3得到的Mat圖像傳到OutputImgProcess，回傳回來的便是最終色彩融合後的擬真化影像
	/// </summary>
	public class ColorHarmonization : IDisposable
	{
		/// <summary>
		/// 取得該物件是否已經回收(Disposed)
		/// </summary>
		public bool IsDisposed { get; protected set; }

		private Net net;

		/// <summary>
		/// 初始化物件
		/// 
		/// 1.Net的初始化（類似tensorflow匯入模組一樣) 
		/// 2.設定Net的運算處理器，根據選擇將決定程式的快慢以及對於環境的要求【注1】
		/// 3.把Net給forward出去 (forward是輸入input給模組以取得Output的動作）
		///   Net在第一次forward時會比較久(約1000ms），但是後續再度呼叫的時候就會大幅度減少了 (每次約80ms~100ms)【注1】
		/// 
		///	注
		/// 1.模組呼叫的時間計算是於下列的電腦規格中測試
		///	  運算處理器設定爲 BackEnd: OPENCV , Target: OPENCL_FP16，此設定在各測試后得到最理想的結果（每張約80ms~100ms）
		///	  其他設定如BackEnd: OPENCV , Target: CPU，每張將需要~600ms左右
		///	  設定OpenCL是因爲OpenCL將會把電腦的CPU及GPU來加速，所以電腦設備越好得到的效果會越好，也不需要使用者額外的環境設定
		///	  設定中也有Cuda的選擇，由於環境設定複雜故此排除使用，但不確定是否會得到更好的效果
		///	  
		/// 
		/// 電腦規格
		/// Operating System: Windows 10 Home 64-bit (10.0, Build 18363)
		/// Processor: Intel(R) Core(TM) i7-8750H CPU @ 2, 20GHz (12 CPUS)
		/// Installed memory (RAM): 16.0 GB (15.9 GB usable)
		/// Graphic Card : GeForce GTX 1060
		/// Directx Version: DirectX 12
		/// </summary>
		/// 
		/// <Reference>
		/// 1.深度學習-色彩融合 
		///	  https://github.com/wasidennis/DeepHarmonization
		/// </Reference>
		public ColorHarmonization()
		{
			//Net起始
			//第一個參數均爲.prototxt的檔案，第二個參數為.caffemodel
			//prototxt的檔案内容是描述caffemodel的layer
			//caffemodel是用Caffe訓練出來的模組			
			net = CvDnn.ReadNetFromCaffe("Resources\\Caffe\\SmartAutoAR.prototxt", "Resources\\Caffe\\SmartAutoAR.caffemodel");

			//這裏可以設定不同的運算加速處理器【注1】
			net.SetPreferableBackend(Net.Backend.OPENCV);
			net.SetPreferableTarget(Net.Target.OPENCL_FP16);

			//先Forward第一次以便之後呼叫會得到較短的運算時間
			//這裏使用的圖像是一張全黑的圖片，因爲我們只是要執行一次Net讓後續的處理得到最快的結果
			FirstForward("Resources\\Caffe\\ini_input.jpg");
		}

		/// <summary>
		/// 預先執行一次模型運算，以加速未來的運算速度
		/// 1.輸入預設圖
		/// 2.用BlobFromImage把圖像轉成Net適用的Mat格式
		/// 3.此模組需要一個 “data",和一個"mask",爲了減少運算速度，兩個都用了全黑圖來當input
		/// 4.把設定好的blob input進去給net
		/// 5.forward出去（forward出去后將會得到經由模組處理後的圖像,但由於我們只爲了速度所以執行完畢就好）
		/// </summary>
		/// <param name="ini_input">預執行的輸入影像</param>
		public void FirstForward(string ini_input)
		{
			Mat input_img = Cv2.ImRead(ini_input);
			Mat blob_InputImg = CvDnn.BlobFromImage(input_img, 1, swapRB: false);
			Mat mask = Cv2.ImRead(ini_input, ImreadModes.Grayscale);
			Mat blob_mask = CvDnn.BlobFromImage(mask, 1, swapRB: false);
			net.SetInput(blob_InputImg, "data");
			net.SetInput(blob_mask, "mask");
			net.Forward();
		}

		/// <summary>
		/// 將接收到的影像資料轉為適合模型的輸入
		/// 
		/// 1.先把接收到的圖像轉成3-channel
		/// 2.把圖像Resize成512x512(模型需求，若模型有改變也要跟著改）
		/// 3.定義一套Scalar
		/// 4.使用BlobFromImage，把影像及Scalar輸入進去，會得到符合Net的input格式的Mat
		/// </summary>
		/// <param name="input_img">欲作為輸入的影像資料(背景圖）</param>
		public Mat InputImgProcess(Mat input_img)
		{
			//先把得到的圖轉成RGB
			//OpenGL截圖回傳得到的Bitmap圖像會是4-channel,此套件只要3-channel
			//因此使用RGBA(4-channel) 2 RGB(3-channel)
			Cv2.CvtColor(input_img, input_img, ColorConversionCodes.RGBA2RGB);

			//做Resize的動作 應模組所需,512x512
			//InterpolationFlags(Resize的方式）可以選擇，但目前LinearExact和 Lanczos4出來的效果最好,後者運算速度稍微比較快（差別~10ms左右）
			input_img = input_img.Resize(new Size(512, 512), interpolation: InterpolationFlags.LinearExact);

			//這是套件前處理的部分,定義一套scalar，用於消去原圖像的顔色，在下一步的時候需要輸入blob内
			//至於Scalar的數字定義部分是跟著參考中的作者定義的【參考 Summary : Reference】
			Scalar scalar_InputImg = new Scalar(104.00699, 116.66877, 122.67892);
			//轉成blob才能當成input輸入給Net
			Mat blob_InputImg = CvDnn.BlobFromImage(input_img, 1, size: default, scalar_InputImg, swapRB: false);

			return blob_InputImg;
		}

		/// <summary>
		/// 將虛擬物體影像處理為適合模型輸入的格式
		/// 
		/// 1.先把接收到的圖像轉成1-channel
		/// 2.把圖像Resize成512x512(模型需求，若模型有改變也要跟著改）
		/// 3.定義一套Scalar，用於消去原圖像的顔色
		/// 4.使用BlobFromImage，把影像及Scalar輸入進去，會得到符合Net的input格式的Mat
		/// </summary>
		/// <param name="mask">虛擬物體影像</param>
		public Mat MaskImgProcess(Mat mask)
		{
			//與InputImgProcess一樣,但這是mask的部分（虛擬物體影像），要轉成1channel的圖
			//所以選擇了RGBA(4-channel) 2 Gray(1-channel)
			Cv2.CvtColor(mask, mask, ColorConversionCodes.RGBA2GRAY);
			Scalar scalar_Mask = new Scalar(128.0, 128.0, 128.0);
			mask = mask.Resize(new Size(512, 512), interpolation: InterpolationFlags.LinearExact);
			Mat blob_mask = CvDnn.BlobFromImage(mask, 1, size: default, scalar_Mask, swapRB: false);

			return blob_mask;
		}

		/// <summary>
		/// 執行Net以得到模型處理后的圖片
		/// 1.設定第一個input:背景圖
		/// 2.設定第二個input:虛擬物體圖
		/// 3.forward出去后將會得到經由模組處理後的圖像，但此圖像還需要經過OutputImgProcess才能輸出出去使用！
		/// </summary>
		/// <param name="blob_InputImg">背景影像</param>
		/// <param name="blob_mask">虛擬物體影像</param>
		/// <returns>模組處理後的影像（尚未最終後製的影像）</returns>
		public Mat NetForwardProcess(Mat blob_InputImg, Mat blob_mask)
		{
			//把input img和mask都丟進input内，"data" 和"mask"是套件取好的
			net.SetInput(blob_InputImg, "data");
			net.SetInput(blob_mask, "mask");

			//Forward出去,處理後得到的照片還沒辦法直接用，需要經過OutputImgProcess才能輸出出去使用！
			Mat preProcess_output = net.Forward();

			return preProcess_output;
		}

		/// <summary>
		/// 將模型的輸出處理為影像資料
		/// 
		/// 爲得到最終輸出要先做幾個步驟處理從 NetForwardProcess 回傳得到的影像
		///
		/// 步驟一到五參考了【Reference 1】
		/// 1.先得到preProcess影像的 H and W dimensions
		/// 2.根據1.得到的H,把preProcess影像 Reshape成直立的Mat
		/// 3.把2.定義的直立mat的顔色抽出來，放進一個list内
		/// 4.把3.抽出來的顔色Merge成一個Mat
		/// 5.用Cv2.Add的方式把在前處理時消去的顔色補回來
		/// 6.最後Resize成OpenGL的視窗大小輸出出去便是色彩融合後的擬真化影像 【Reference 2】
		/// </summary>
		/// 
		/// <Reference>
		/// 1.forward后得到的影像處理方式 
		///	  https://answers.opencv.org/question/205824/dnn-forward-result-has-rows-1-colums-1/
		///	2.輸出前的圖像處理
		///	  https://stackoverflow.com/questions/57517702/having-trouble-converting-code-from-python-to-c-sharp
		/// </Reference>
		/// 
		/// 
		/// <returns>色彩融合後的擬真化影像</returns>
		public Mat OutputImgProcess(Mat preProcess_output, int windowWidth, int windowHeight)
		{
			//Output出去前的處理
			int H = preProcess_output.Size(2);
			int W = preProcess_output.Size(3);
			//Reshape成直立的Mat
			Mat strip = preProcess_output.Reshape(1, H * 3);
			//把顔色給抽出來,planes也有channels的含義
			Mat[] planes = new Mat[3];
			planes[0] = strip.SubMat(0, H, 0, W);
			planes[1] = strip.SubMat(H, 2 * H, 0, W);
			planes[2] = strip.SubMat(2 * H, 3 * H, 0, W);

			//宣告一個Mat給output用的，把剛剛抽出來的planes融合起來
			Mat output_Img = new Mat();
			Cv2.Merge(planes, output_Img);
			//這裏是要把在前處理時消去的顔色補回來
			Scalar scalar_outputImg = new Scalar(+104.00699, +116.66877, +122.67892);
			Cv2.Add(output_Img, scalar_outputImg, output_Img);

			//Output Processing
			//最後的處理，數值形態的關係需要局限出來的圖不會超過0~255之外
			double minVal, maxVal;
			output_Img.MinMaxLoc(out minVal, out maxVal);
			output_Img.ConvertTo(output_Img, MatType.CV_8UC1, 255.0 / (maxVal - minVal), -255.0 / minVal);
			output_Img = output_Img.Resize(new Size(windowWidth, windowHeight), interpolation: InterpolationFlags.LinearExact);
			return output_Img;
		}

		/// <summary>
		/// 釋放物件相關之記憶體
		/// </summary>
		public void Dispose()
		{
			if (!IsDisposed)
			{
				net.Dispose();
				IsDisposed = true;
			}
		}

	}//end class
}//end namespace
