using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
	public partial class Form1 : Form
	{
		ArForm arForm;
		const string ResourcesPath = @"..\..\..\..\resources\";

		public Form1()
		{
			InitializeComponent();
		}

		private void InputSource_RB_CheckedChanged(object sender, EventArgs e)
		{
			InputSource_OFD.FileName = "";

			if (StreamSource_RB.Checked)
			{
				SelectedInput_Lb.Text = "選擇WebCam ID";
				InputSelection_Bt.Visible = false;
				CameraID_Tb.Visible = true;
			}
			else
			{
				SelectedInput_Lb.Text = "尚未選擇檔案";
				InputSelection_Bt.Visible = true;
				CameraID_Tb.Visible = false;
			}
		}

		private void FileSelection_Bt_Click(object sender, EventArgs e)
		{
			if (ImageSource_RB.Checked) InputSource_OFD.Filter = "圖片檔|*.jpg;*.png";
			else if (VideoSource_RB.Checked) InputSource_OFD.Filter = "影片檔|*.mp4";
			else InputSource_OFD.Filter = "";

			DialogResult dialogResult = InputSource_OFD.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				SelectedInput_Lb.Text = InputSource_OFD.FileName.Split('\\')[^1];
			}
		}

		private void ModelSelection_Bt_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = Model_OFD.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				SelectedModel_Lb.Text = Model_OFD.FileName.Split('\\')[^1];
			}
		}

		private void Restart_Bt_Click(object sender, EventArgs e)
		{
			if (arForm != null) arForm.Dispose();
			arForm = new ArForm();

			ArForm.Input input = ImageSource_RB.Checked ? ArForm.Input.Image : (VideoSource_RB.Checked ? ArForm.Input.Video : ArForm.Input.Stream);
			string inputPath = StreamSource_RB.Checked ? CameraID_Tb.Text : InputSource_OFD.FileName;
			string markerPath = Marker_OFD.FileName;
			string modelPath = Model_OFD.FileName;
			float resizeRate = float.TryParse(Resize_TB.Text, out float temp) ? temp : 1;
			float rotationX = float.TryParse(RotaationX_TB.Text, out temp) ? temp : 0;
			float rotationY = float.TryParse(RotaationY_TB.Text, out temp) ? temp : 0;
			float rotationZ = float.TryParse(RotaationZ_TB.Text, out temp) ? temp : 0;
			if (inputPath == "" || markerPath == "" || modelPath == "")
			{
				MessageBox.Show("設置錯誤");
				return;
			}
			arForm.SetParams(input, inputPath, markerPath, modelPath, resizeRate, rotationX, rotationY, rotationZ, LightTracking_CB.Checked, ColorHarmonizing_CB.Checked, Spin_CB.Checked);

			arForm.Run(60.0);
		}

		private void MarkerSelection_Bt_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = Marker_OFD.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				SelectedMarker_Lb.Text = Marker_OFD.FileName.Split('\\')[^1];
			}
		}

		private void Exemple1_Bt_Click(object sender, EventArgs e)
		{
			StreamSource_RB.Checked = true;
			Marker_OFD.FileName = ResourcesPath+"marker2.jpg";
			Model_OFD.FileName = ResourcesPath+@"models\Ray\ch4.obj";
			Resize_TB.Text = "0.05";
			CameraID_Tb.Text = "1";
			RotaationX_TB.Text = "";
			RotaationY_TB.Text = "";
			RotaationZ_TB.Text = "";
			LightTracking_CB.Checked = false;
			ColorHarmonizing_CB.Checked = false;
			Spin_CB.Checked = true;

			SelectedInput_Lb.Text = InputSource_OFD.FileName.Split('\\')[^1];
			SelectedMarker_Lb.Text = Marker_OFD.FileName.Split('\\')[^1];
			SelectedModel_Lb.Text = Model_OFD.FileName.Split('\\')[^1];
		}

		private void Exemple2_Bt_Click(object sender, EventArgs e)
		{
			VideoSource_RB.Checked = true;
			InputSource_OFD.FileName = ResourcesPath+"video_test.mp4";
			Marker_OFD.FileName = ResourcesPath+"marker.png";
			Model_OFD.FileName = ResourcesPath+@"models\Stone\Stone.obj";
			Resize_TB.Text = "";
			RotaationX_TB.Text = "";
			RotaationY_TB.Text = "";
			RotaationZ_TB.Text = "";
			LightTracking_CB.Checked = true;
			ColorHarmonizing_CB.Checked = false;
			Spin_CB.Checked = true;

			SelectedInput_Lb.Text = InputSource_OFD.FileName.Split('\\')[^1];
			SelectedMarker_Lb.Text = Marker_OFD.FileName.Split('\\')[^1];
			SelectedModel_Lb.Text = Model_OFD.FileName.Split('\\')[^1];
		}

		private void Exemple3_Bt_Click(object sender, EventArgs e)
		{
			ImageSource_RB.Checked = true;
			InputSource_OFD.FileName = ResourcesPath+"image_test.jpg";
			Marker_OFD.FileName = ResourcesPath+"marker.png";
			Model_OFD.FileName = ResourcesPath+@"models\cat\12221_Cat_v1_l3.obj";
			Resize_TB.Text = "0.35";
			RotaationX_TB.Text = "-100";
			RotaationY_TB.Text = "";
			RotaationZ_TB.Text = "-70";
			LightTracking_CB.Checked = true;
			ColorHarmonizing_CB.Checked = true;
			Spin_CB.Checked = false;

			SelectedInput_Lb.Text = InputSource_OFD.FileName.Split('\\')[^1];
			SelectedMarker_Lb.Text = Marker_OFD.FileName.Split('\\')[^1];
			SelectedModel_Lb.Text = Model_OFD.FileName.Split('\\')[^1];
		}
	}
}
