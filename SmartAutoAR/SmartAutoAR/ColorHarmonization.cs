using System;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SmartAutoAR
{
	public class ColorHarmonization : IDisposable
	{
		public bool IsDisposed { get; protected set; }

		private Net net;

		public ColorHarmonization()
		{
			//Net initialize 
			net = CvDnn.ReadNetFromCaffe("Resources\\Caffe\\SmartAutoAR.prototxt", "Resources\\Caffe\\SmartAutoAR.caffemodel");
			//By using OPENCL we get the best speed ever
			net.SetPreferableBackend(Net.Backend.OPENCV);
			net.SetPreferableTarget(Net.Target.OPENCL_FP16);
			first_forward("Resources\\Caffe\\ini_input.jpg");
		}

		//先預設讀一個最基本的照片來forward
		//第一次forward的時間比較慢-1000ms
		//之後每次只需要100ms左右
		public void first_forward(string ini_input)
		{
			Mat input_img = Cv2.ImRead(ini_input);
			Mat blob_InputImg = CvDnn.BlobFromImage(input_img, 1, swapRB: false);
			Mat mask = Cv2.ImRead(ini_input, ImreadModes.Grayscale);
			Mat blob_mask = CvDnn.BlobFromImage(mask, 1, swapRB: false);
			net.SetInput(blob_InputImg, "data");
			net.SetInput(blob_mask, "mask");
			net.Forward();
		}

		public Mat inputImg_Process(Mat input_img)
		{

			//先把得到的圖轉成RGB,Bitmap截圖得到的是4-channel,此套件只要3-channel
			Cv2.CvtColor(input_img, input_img, ColorConversionCodes.RGBA2RGB);
			//做Resize的動作 應模組所需,512x512, InterpolationFlags可以選擇 但目前LinearExact和 Lanczos4最好,後者比較快
			input_img = input_img.Resize(new Size(512, 512), interpolation: InterpolationFlags.LinearExact);

			//這是套件前處理的部分
			Scalar scalar_InputImg = new Scalar(104.00699, 116.66877, 122.67892);
			//轉成blob才能做input
			Mat blob_InputImg = CvDnn.BlobFromImage(input_img, 1, size: default, scalar_InputImg, swapRB: false);

			return blob_InputImg;
		}

		public Mat maskImg_Process(Mat mask)
		{

			//同上,但這是mask的部分，然後mask一定要轉成1channel的圖
			Cv2.CvtColor(mask, mask, ColorConversionCodes.RGBA2GRAY);
			Scalar scalar_Mask = new Scalar(128.0, 128.0, 128.0);
			mask = mask.Resize(new Size(512, 512), interpolation: InterpolationFlags.LinearExact);
			Mat blob_mask = CvDnn.BlobFromImage(mask, 1, size: default, scalar_Mask, swapRB: false);

			return blob_mask;
		}

		public Mat netForward_Process(Mat blob_InputImg, Mat blob_mask)
		{
			//把input img和mask都丟進input内，"data" 和"mask"是套件取好的
			net.SetInput(blob_InputImg, "data");
			net.SetInput(blob_mask, "mask");

			//Forward出去,處理後得到的照片還沒辦法直接用
			Mat preProcess_output = net.Forward();

			return preProcess_output;
		}

		public Mat outputImg_Process(Mat preProcess_output, int windowWidth, int windowHeight)
		{

			//Output出去前的處理
			int H = preProcess_output.Size(2);
			int W = preProcess_output.Size(3);
			//Reshape成容易取資料的方式
			Mat strip = preProcess_output.Reshape(1, H * 3);
			//把顔色給抽出來,planes也有channels的含義
			Mat[] planes = new Mat[3];
			planes[0] = strip.SubMat(0, H, 0, W);
			planes[1] = strip.SubMat(H, 2 * H, 0, W);
			planes[2] = strip.SubMat(2 * H, 3 * H, 0, W);

			//宣告一個Mat給output用的，把剛剛抽出來的planes融合起來
			Mat output_Img = new Mat();
			Cv2.Merge(planes, output_Img);
			//這裏是要把在forward前消去的顔色補回來
			Scalar scalar_outputImg = new Scalar(+104.00699, +116.66877, +122.67892);
			Cv2.Add(output_Img, scalar_outputImg, output_Img);

			//References : https://stackoverflow.com/questions/57517702/having-trouble-converting-code-from-python-to-c-sharp
			//Output Processing
			//最後的處理，數值形態的關係需要局限出來的圖不會超過0~255之外
			double minVal, maxVal;
			output_Img.MinMaxLoc(out minVal, out maxVal);
			output_Img.ConvertTo(output_Img, MatType.CV_8UC1, 255.0 / (maxVal - minVal), -255.0 / minVal);
			output_Img = output_Img.Resize(new Size(windowWidth, windowHeight), interpolation: InterpolationFlags.LinearExact);
			return output_Img;
		}

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
