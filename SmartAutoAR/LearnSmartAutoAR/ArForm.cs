using System;
using OpenTK;
using OpenTK.Graphics;
using SmartAutoAR;
using SmartAutoAR.InputSource;
using SmartAutoAR.VirtualObject;
using SmartAutoAR.VirtualObject.Lights;
using Bitmap = System.Drawing.Bitmap;

namespace LearnSmartAutoAR
{
    public partial class ArForm : GameWindow
    {
        VideoSource videoSource;
        Scene scene;
        Model stoneMan;
        ArWorkflow workflow;

        public ArForm(int width, int height, string title) :
            base(width, height,
                GraphicsMode.Default,
                title,
                GameWindowFlags.Default,
                DisplayDevice.Default,
                4, 5,
                GraphicsContextFlags.ForwardCompatible)
        { }

        protected override void OnLoad(EventArgs e)
        {
            // 請輸入您的影片路徑
            videoSource = new VideoSource(@"..\..\..\resources\video_test.mp4");

            // 創建場景
            scene = new Scene();
            stoneMan = Model.LoadModel(@"..\..\..\resources\models\Stone\Stone.obj"); // 請輸入您的模型路徑
            scene.Models.Add(stoneMan);

            // 調整模型大小
            stoneMan.Resize(0.1f);

            // 加入燈光
            // scene.Lights.Add(new AmbientLight(Color4.White, 0.8f));

            // 建立 workflow 物件
            workflow = new ArWorkflow(videoSource);

            // 設定 marker 對應的 scene
            Bitmap marker = new Bitmap(@"..\..\..\resources\marker.png"); // 請輸入您 Marker 圖檔的路徑
            workflow.MarkerPairs[marker] = scene;
            workflow.TrainMarkers(); // 修改後一定要執行!!

            // 開啟光源追蹤模組
            workflow.EnableLightTracking = true;

            // 開啟色彩調合模組
            workflow.EnableColorHarmonizing = true;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // 確保視窗比例與背景一致
            Width = (int)(Height * workflow.WindowAspectRatio);

            // 當影片撥放完畢後重播
            if (videoSource.EndOfVideo)
            {
                videoSource.Replay();
                workflow.ClearState();
            }

            workflow.Show();
            SwapBuffers(); // 這是 GameWindow 中繪製新畫面必須的語法

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            // 設定影像輸出大小
            workflow.SetOutputSize(Width, Height);

            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            scene.Dispose();

            base.OnUnload(e);
        }
    }
}
