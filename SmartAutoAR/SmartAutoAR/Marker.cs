//using OpenCVForUnity.CoreModule;
using OpenCvSharp;
namespace SmartAutoAR
{
    /// <summary>
    /// 從系統的角度 知道有關於marker的資訊 像是 marker的特徵點 
    /// This code is a rewrite of https://github.com/MasteringOpenCV/code/tree/master/Chapter3_MarkerlessAR using "OpenCV for Unity".
    /// </summary>
    class Marker
    {
        /// <summary>
        /// marker的大小
        /// </summary>
        public Size size;

        /// <summary>
        /// 讓系統讀取的marker圖像
        /// </summary>
        public Mat frame;

        /// <summary>
        /// The gray marker image.
        /// </summary>
        public Mat grayImg;

        /// <summary>
        /// The keypoints.
        /// </summary>
        /// 
        public KeyPoint[] keyPoints;
        //public MatOfKeyPoint keypoints;

        /// <summary>
        /// The descriptors.
        /// </summary>
        public Mat descriptors;

        /// <summary>
        /// The points2d.
        /// </summary>
        public Point2f[] points2d;// map to 236行
        //public MatOfPoint2f points2d;

        /// <summary>
        /// The points3d.
        /// </summary>
        /// 
        public Point3f[] points3d;
        //public MatOfPoint3f points3d;

        /// <summary>
        /// Initializes a new instance of the <see cref="Marker"/> class.
        /// </summary>
        /// 
        public Marker ()
        {
            size = new Size ();
            frame = new Mat ();
            grayImg = new Mat ();
            //keypoints = new MatOfKeyPoint ();
            descriptors = new Mat ();

        }
    }
}