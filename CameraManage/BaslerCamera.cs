using Basler.Pylon;

namespace CameraManage
{
    public class BaslerCamera : BaseCamera
    {
        public BaslerCamera()
        {
            GetCameraInfoDic();
        }
        ~BaslerCamera()
        {
            _Camera.Dispose();
        }

        private Camera _Camera;

        /// <summary>
        /// 储存相机信息的字典
        /// string: camera index; <string, object>: <information name, information>
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> CameraNameDic = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// send the image data to main function
        /// </summary>
        public event Action<CameraData> ImageCaptured;

        public bool bSaveImage = false; //是否需要保存图像
        private string strSavePath = ""; //图像保存的路径

        /// <summary>
        /// 相机曝光
        /// </summary>
        public override double Exposure
        {
            get
            {
                if (_Camera.IsConnected && _Camera.IsOpen)
                {
                    return _Camera.Parameters[PLCamera.ExposureTime].GetValue();
                }
                else
                {
                    throw new Exception("the camera is non't connected or open");
                }
            }
            set
            {
                if (_Camera.IsConnected && _Camera.IsOpen)
                {
                    _Camera.Parameters[PLCamera.ExposureTime].SetValue(value);
                }
            }
        }

        /// <summary>
        /// 相机宽
        /// </summary>
        public override double ImageWidth
        {
            get
            {
                return _Camera.Parameters[PLCamera.Width].GetValue();
            }
        }

        /// <summary>
        /// 相机高
        /// </summary>
        public override double ImageHeight
        {
            get
            {
                return _Camera.Parameters[PLCamera.Height].GetValue();
            }
        }

        /// <summary>
        /// 获取相机名称信息
        /// </summary>
        public override void GetCameraInfoDic()
        {
            var cameraFindList = CameraFinder.Enumerate();
            foreach (var camera in cameraFindList)
            {
                CameraNameDic[camera["FriendlyName"]] = new Dictionary<string, object>
                {
                    {"SerialNumber",camera["SerialNumber"] },
                };
            }
        }

        /// <summary>
        /// 连接到指定名称的相机
        /// </summary>
        /// <param name="serialNumber"></param>
        public override void ConnectedCamera(string serialNumber)
        {
            _Camera = new Camera(serialNumber);
            _Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;

            if (!_Camera.IsOpen)
            {
                _Camera.CameraOpened += Configuration.AcquireContinuous;
                _Camera.Open();
            }
        }

        /// <summary>
        /// 断开相机连接
        /// </summary>
        public override void DisconnectedCamera()
        {
            if (_Camera != null)
            {
                if (_Camera.IsConnected)
                {
                    _Camera.StreamGrabber.ImageGrabbed -= StreamGrabber_ImageGrabbed;
                    _Camera.Close();
                }
            }
        }

        /// <summary>
        /// 相机回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StreamGrabber_ImageGrabbed(object? sender, ImageGrabbedEventArgs e)
        {
            using var grabResult = e.GrabResult;
            if (grabResult.GrabSucceeded)
            {
                // 保存图像
                if (bSaveImage)
                {
                    ImagePersistence.Save(ImageFileFormat.Bmp, strSavePath, grabResult);
                    bSaveImage = false;
                    return;
                }

                byte[] cameraData = grabResult.PixelData as byte[];
                if (cameraData != null)
                {
                    CameraData temp = new CameraData
                    {
                        data = cameraData,
                        width = grabResult.Width,
                        height = grabResult.Height
                    };
                    ImageCaptured?.Invoke(temp);
                }
            }
        }

        /// <summary>
        /// 单次取图
        /// </summary>
        public override void GrabOnce()
        {
            if (_Camera == null || !_Camera.IsOpen) { return; }
            if (_Camera.StreamGrabber.IsGrabbing)
            {
                _Camera.StreamGrabber.Stop();
            }
            _Camera.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
        }

        /// <summary>
        /// 连续取图
        /// </summary>
        public override void GrabContinue()
        {
            if (_Camera == null || !_Camera.IsOpen) { return; }
            _Camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
        }

        /// <summary>
        /// 停止取图
        /// </summary>
        public override void StopGrab()
        {
            if (_Camera == null || !_Camera.IsOpen) { return; }
            _Camera.StreamGrabber.Stop();
            GC.Collect();
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="savePath">图像路径</param>
        public override void SaveImage(string savePath)
        {
            if (_Camera == null || !_Camera.IsOpen) { return; }

            bSaveImage = true;
            strSavePath = savePath;
            GrabOnce();
        }
    }
}