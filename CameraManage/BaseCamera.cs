namespace CameraManage
{
    /// <summary>
    /// 相机基类，用于定义相机的基本操作
    /// </summary>
    public abstract class BaseCamera
    {
        public abstract void GetCameraInfoDic();//获取相机名称信息
        public abstract void ConnectedCamera(string serialNumber);//连接到指定名称的相机
        public abstract void DisconnectedCamera();//断开相机连接
        public abstract void GrabOnce();//单次采集
        public abstract void GrabContinue();//连续采集
        public abstract void StopGrab();//停止采集
        public abstract void SaveImage(string savePath);//保存图像
        public virtual double Exposure { get; set; }//相机曝光
        public virtual double ImageWidth { get; set; }//相机宽
        public virtual double ImageHeight { get; set; }//相机高

        /// <summary>
        /// 图像信息
        /// </summary>
        public struct CameraData
        {
            public byte[] data; // 数据
            public int width; // 宽
            public int height; // 高
        }
    }
}
