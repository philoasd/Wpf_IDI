using Matrox.MatroxImagingLibrary;

namespace ImageProcessing
{
    public class MatroxImageLibrary
    {
        public MatroxImageLibrary()
        {
            MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, MIL.M_NULL, MIL.M_NULL);
        }
        ~MatroxImageLibrary()
        {
            // Free defaults.
            MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MIL.M_NULL, MIL.M_NULL);
        }

        MIL_ID MilApplication = MIL.M_NULL;                             // Application identifier.
        MIL_ID MilSystem = MIL.M_NULL;                                  // System identifier.
        MIL_ID MilDisplay = MIL.M_NULL;                                 // Display identifier.

        #region 标定参数
        // Grid offset specifications.
        private const int GRID_OFFSET_X = 0;
        private const int GRID_OFFSET_Y = 0;
        private const int GRID_OFFSET_Z = 0;

        // World description of the calibration grid.
        private int GRID_ROW_SPACING = 1;
        private int GRID_COLUMN_SPACING = 1;
        private int GRID_ROW_NUMBER = 17;
        private int GRID_COLUMN_NUMBER = 17;
        #endregion

        #region 图像大小，可在该类初始化之前进行修改
        public static int ImageWdith = 2840;
        public static int ImageHeight = 2840;
        #endregion

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="img"></param>
        public void SaveImage(string path, MIL_ID img)
        {
            MIL.MbufSave(path, img);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SaveImage(string path, byte[] imageData, int width, int height)
        {
            MIL_ID saveImage = MIL.M_NULL;
            MIL.MbufAlloc2d(MilSystem, width, height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref saveImage);
            MIL.MbufPut2d(saveImage, 0, 0, width, height, imageData);

            MIL.MbufSave(path, saveImage);
        }

        /// <summary>
        /// 图像拼接
        /// </summary>
        /// <param name="ImageFilesSource">需要拼接的图像名称列表</param>
        public MIL_ID StitchImage(string[] ImageFilesSource)
        {
            int ImageNumber = ImageFilesSource.Length;
            MIL_ID[] MilSourceImages = new MIL_ID[ImageNumber];
            for (int i = 0; i < ImageNumber; i++)
            {
                MIL.MbufRestore(ImageFilesSource[i], MilSystem, ref MilSourceImages[i]);
            }

            // Allocate a new empty registration context.
            MIL_ID MilRegContext = MIL.M_NULL;
            MIL.MregAlloc(MilSystem, MIL.M_STITCHING, MIL.M_DEFAULT, ref MilRegContext);
            // Allocate a new empty registration result buffer.
            MIL_ID MilRegResult = MIL.M_NULL;
            MIL.MregAllocResult(MilSystem, MIL.M_DEFAULT, ref MilRegResult);

            // Set the transformation type to translation.
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_NUMBER_OF_REGISTRATION_ELEMENTS, ImageNumber);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_TRANSFORMATION_TYPE, MIL.M_TRANSLATION);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_SCORE_TYPE, MIL.M_CORRELATION);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_LOCATION_DELTA, 100);

            // Calculate the registration on the images.
            MIL.MregCalculate(MilRegContext, MilSourceImages, MilRegResult, ImageNumber, MIL.M_DEFAULT);
            // Verify if registration is successful.
            MIL_INT Result = 0;
            MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_RESULT + MIL.M_TYPE_MIL_INT, ref Result);

            MIL_ID MilMosaicImage = MIL.M_NULL;
            if (Result == MIL.M_SUCCESS)
            {
                MIL_INT MosaicSizeX = 0;
                MIL_INT MosaicSizeY = 0;
                MIL_INT MosaicSizeBand = 0;
                MIL_INT MosaicType = 0;

                // Get the size of the required mosaic buffer.
                MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_MOSAIC_SIZE_X + MIL.M_TYPE_MIL_INT, ref MosaicSizeX);
                MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_MOSAIC_SIZE_Y + MIL.M_TYPE_MIL_INT, ref MosaicSizeY);

                // The mosaic type will be the same as the source images.
                MIL.MbufInquire(MilSourceImages[0], MIL.M_SIZE_BAND, ref MosaicSizeBand);
                MIL.MbufInquire(MilSourceImages[0], MIL.M_TYPE, ref MosaicType);

                // Allocate mosaic image.
                MIL.MbufAllocColor(MilSystem, MosaicSizeBand, MosaicSizeX, MosaicSizeY, MosaicType, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilMosaicImage);

                // Compose the mosaic from the source images.
                MIL.MregTransformImage(MilRegResult, MilSourceImages, MilMosaicImage, ImageNumber, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT);
            }

            foreach (var item in MilSourceImages)
            {
                MIL.MbufFree(item);
            }

            return MilMosaicImage;
        }

        /// <summary>
        /// 图像拼接（图像大小相同）
        /// </summary>
        /// <param name="imageList"></param>
        /// <param name="width"></param>
        public MIL_ID StitchImage(List<byte[]> imageList, int width, int height)
        {
            int ImageNumber = imageList.Count();
            MIL_ID[] MilSourceImages = new MIL_ID[ImageNumber];
            for (int i = 0; i < ImageNumber; i++)
            {
                MIL.MbufAlloc2d(MilSystem, width, height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilSourceImages[i]);
                MIL.MbufPut2d(MilSourceImages[i], 0, 0, width, height, imageList[i]);
            }

            // 校准图像
            MIL_ID MilCalibration = MIL.M_NULL;
            if (File.Exists("CalibrationParam.mim") && MilCalibration == MIL.M_NULL)
            {
                CalibrateImage(ref MilSourceImages);
            }

            // Allocate a new empty registration context.
            MIL_ID MilRegContext = MIL.M_NULL;
            MIL.MregAlloc(MilSystem, MIL.M_STITCHING, MIL.M_DEFAULT, ref MilRegContext);
            // Allocate a new empty registration result buffer.
            MIL_ID MilRegResult = MIL.M_NULL;
            MIL.MregAllocResult(MilSystem, MIL.M_DEFAULT, ref MilRegResult);

            // Set the transformation type to translation.
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_NUMBER_OF_REGISTRATION_ELEMENTS, ImageNumber);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_TRANSFORMATION_TYPE, MIL.M_TRANSLATION);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_SCORE_TYPE, MIL.M_CORRELATION);
            MIL.MregControl(MilRegContext, MIL.M_CONTEXT, MIL.M_LOCATION_DELTA, 100);

            // Calculate the registration on the images.
            MIL.MregCalculate(MilRegContext, MilSourceImages, MilRegResult, ImageNumber, MIL.M_DEFAULT);
            // Verify if registration is successful.
            MIL_INT Result = 0;
            MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_RESULT + MIL.M_TYPE_MIL_INT, ref Result);

            MIL_ID MilMosaicImage = MIL.M_NULL;
            if (Result == MIL.M_SUCCESS)
            {
                MIL_INT MosaicSizeX = 0;
                MIL_INT MosaicSizeY = 0;
                MIL_INT MosaicSizeBand = 0;
                MIL_INT MosaicType = 0;

                // Get the size of the required mosaic buffer.
                MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_MOSAIC_SIZE_X + MIL.M_TYPE_MIL_INT, ref MosaicSizeX);
                MIL.MregGetResult(MilRegResult, MIL.M_GENERAL, MIL.M_MOSAIC_SIZE_Y + MIL.M_TYPE_MIL_INT, ref MosaicSizeY);

                // The mosaic type will be the same as the source images.
                MIL.MbufInquire(MilSourceImages[0], MIL.M_SIZE_BAND, ref MosaicSizeBand);
                MIL.MbufInquire(MilSourceImages[0], MIL.M_TYPE, ref MosaicType);

                // Allocate mosaic image.
                MIL.MbufAllocColor(MilSystem, MosaicSizeBand, MosaicSizeX, MosaicSizeY, MosaicType, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilMosaicImage);

                // Compose the mosaic from the source images.
                MIL.MregTransformImage(MilRegResult, MilSourceImages, MilMosaicImage, ImageNumber, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT);
            }

            // free
            foreach (var item in MilSourceImages)
            {
                MIL.MbufFree(item);
            }

            return MilMosaicImage;
        }

        public enum StitchType
        {
            Horizontal,//水平
            Vertical,//垂直
            Adaptation//自动，以矩形方式
        }

        /// <summary>
        /// 图像拼接（通过 one-by-one方式）
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public MIL_ID StitchImage(List<byte[]> imageData, StitchType type, int width = 2840, int height = 2840, int rows = 0, int cols = 0)
        {
            MIL_ID MilMosaicImage = MIL.M_NULL;

            int imageNumber = imageData.Count;
            switch (type)
            {
                case StitchType.Horizontal: //横向
                    {
                        //创建一个空的图片
                        MIL.MbufAllocColor(MilSystem, 1, imageNumber * width, height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilMosaicImage);

                        for (int i = 0; i < imageNumber; i++)
                        {
                            MIL.MbufPut2d(MilMosaicImage, i * width, 0, width, height, imageData[i]);
                        }
                        break;
                    }
                case StitchType.Vertical: //纵向
                    {
                        //创建一个空的图片
                        MIL.MbufAllocColor(MilSystem, 1, width, imageNumber * height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilMosaicImage);

                        for (int i = 0; i < imageNumber; i++)
                        {
                            MIL.MbufPut2d(MilMosaicImage, 0, i * height, width, height, imageData[i]);
                        }
                        break;
                    }
                case StitchType.Adaptation: //按照矩形
                    {
                        //创建一个空的图片
                        MIL.MbufAllocColor(MilSystem, 1, rows * width, cols * height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilMosaicImage);

                        int index = 0;
                        for (int i = 0; i < cols; i++)
                        {
                            int startIndex = i % 2 == 0 ? 0 : rows - 1;
                            int endIndex = i % 2 == 0 ? rows : -1;
                            int step = i % 2 == 0 ? 1 : -1;

                            for (int j = startIndex; j != endIndex; j += step)
                            {
                                MIL.MbufPut2d(MilMosaicImage, j * width, i * height, width, height, imageData[index]);
                                index++;
                            }
                        }
                        break;
                    }
            }

            return MilMosaicImage;
        }

        /// <summary>
        /// 将图像数据转为Matrox图像
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private MIL_ID TransformImageDataToMIL(byte[] data, int width, int height)
        {
            MIL_ID MilSourceImages = MIL.M_NULL;

            MIL.MbufAlloc2d(MilSystem, width, height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilSourceImages);
            MIL.MbufPut2d(MilSourceImages, 0, 0, width, height, data);

            return MilSourceImages;
        }

        /// <summary>
        /// Find the two largest factors of a number
        /// In the result returned, the first number is the largest
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private int[] FindLargestFactors(int number)
        {
            int factor1 = 1;
            int factor2 = number;

            for (int i = 2; i <= number / 2; i++)
            {
                if (number % i == 0)
                {
                    factor1 = i;
                    factor2 = number / i;
                }
            }

            int max = Math.Max(factor1, factor2);
            int min = Math.Min(factor1, factor2);
            return new int[] { max, min };
        }

        /// <summary>
        /// 对相机进行标定,并返回先像素和实际距离的比值
        /// </summary>
        /// <param name="imageFile"></param>
        public double Calibration(string imageFile, Dictionary<string, Dictionary<string, object>> parametr, string parameterName)
        {
            //if (!File.Exists("MilCalibration.mim"))
            {
                MIL_ID MilImage = MIL.M_NULL;          // Image buffer identifier.
                MIL_ID MilOverlayImage = MIL.M_NULL;   // Overlay image.
                MIL_INT CalibrationStatus = 0;

                // Clear the display.
                MIL.MdispControl(MilDisplay, MIL.M_OVERLAY_CLEAR, MIL.M_DEFAULT);
                // Restore source image into an automatically allocated image buffer.
                MIL.MbufRestore(imageFile, MilSystem, ref MilImage);

                // Allocate a camera calibration context.
                MIL_ID MilCalibration = MIL.M_NULL;
                MIL.McalAlloc(MilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilCalibration);

                MIL.McalControl(MilCalibration, MIL.M_GRID_PARTIAL, MIL.M_DISABLE);
                MIL.McalControl(MilCalibration, MIL.M_GRID_FIDUCIAL, MIL.M_DEFAULT);
                MIL.McalControl(MilCalibration, MIL.M_GRID_HINT_PIXEL_X, MIL.M_DEFAULT);
                MIL.McalControl(MilCalibration, MIL.M_GRID_HINT_PIXEL_Y, MIL.M_DEFAULT);
                MIL.McalControl(MilCalibration, MIL.M_GRID_HINT_ANGLE_X, MIL.M_DEFAULT);

                // 读取标定参数
                if (parametr.Count > 0)
                {
                    GRID_ROW_SPACING = (int)parametr[parameterName]["GRID_ROW_SPACING"];
                    GRID_COLUMN_SPACING = (int)parametr[parameterName]["GRID_COLUMN_SPACING"];
                    GRID_ROW_NUMBER = (int)parametr[parameterName]["GRID_ROW_NUMBER"];
                    GRID_COLUMN_NUMBER = (int)parametr[parameterName]["GRID_COLUMN_NUMBER"];
                }

                // Calibrate the camera with the image of the grid and its world description.
                MIL.McalGrid(MilCalibration, MilImage, GRID_OFFSET_X, GRID_OFFSET_Y, GRID_OFFSET_Z, GRID_ROW_NUMBER, GRID_COLUMN_NUMBER, GRID_ROW_SPACING, GRID_COLUMN_SPACING, MIL.M_DEFAULT, MIL.M_DEFAULT);
                MIL.McalInquire(MilCalibration, MIL.M_CALIBRATION_STATUS + MIL.M_TYPE_MIL_INT, ref CalibrationStatus);

                if (CalibrationStatus == MIL.M_CALIBRATED)
                {
                    // Perform a first image transformation with the calibration grid.
                    MIL.McalTransformImage(MilImage, MilImage, MilCalibration, MIL.M_BILINEAR | MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT, MIL.M_DEFAULT);
                    MIL.McalSave("CalibrationParam.mim", MilCalibration, MIL.M_DEFAULT);
                    // 获取像素和实际距离的比值
                    double ratio = 0;
                    MIL.McalTransformResult(MilImage, MIL.M_PIXEL_TO_WORLD, MIL.M_LENGTH, 1.0, ref ratio);
                    MIL.MbufFree(MilImage);
                    return ratio;
                }

                return -1;
            }
        }

        /// <summary>
        /// 对图像进行校正（从文件）
        /// </summary>
        /// <param name="imageFile"></param>
        public MIL_ID CalibrateImage(string imageFile)
        {
            MIL_ID MilCalibration = MIL.M_NULL;
            if (File.Exists("CalibrationParam.mim") && MilCalibration == MIL.M_NULL)
            {
                MIL_ID MilImage = MIL.M_NULL;
                MIL.MbufRestore(imageFile, MilSystem, ref MilImage);

                // Read the image of the board and associate the calibration to the image.
                MilCalibration = MIL.McalRestore("CalibrationParam.mim", MilSystem, MIL.M_DEFAULT, ref MilCalibration);

                // Transform the image of the board.
                MIL.McalTransformImage(MilImage, MilImage, MilCalibration, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT, MIL.M_DEFAULT);

                MIL.MbufSave("CalibrationResult.bmp", MilImage);
                return MilImage;
            }
            return MIL.M_NULL;
        }

        /// <summary>
        /// 对图像进行校正（从相机）
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        public MIL_ID CalibrateImage(byte[] imageData, int width)
        {
            MIL_ID MilCalibration = MIL.M_NULL;
            if (File.Exists("CalibrationParam.mim") && MilCalibration == MIL.M_NULL)
            {
                MIL_ID MilImage = MIL.M_NULL;

                MIL.MbufAlloc2d(MilSystem, width, imageData.Length / width, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilImage);
                MIL.MbufPut2d(MilImage, 0, 0, width, imageData.Length / width, imageData);

                // Read the image of the board and associate the calibration to the image.
                MilCalibration = MIL.McalRestore("CalibrationParam.mim", MilSystem, MIL.M_DEFAULT, ref MilCalibration);

                // Transform the image of the board.
                MIL.McalTransformImage(MilImage, MilImage, MilCalibration, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT, MIL.M_DEFAULT);

                //MIL.MbufSave("CalibrationResult.bmp", MilImage);
                return MilImage;
            }
            return MIL.M_NULL;
        }

        /// <summary>
        /// 对图像进行校正（从Matrox图像）
        /// </summary>
        /// <param name="MilImage"></param>
        public void CalibrateImage(ref MIL_ID MilImage)
        {
            MIL_ID MilCalibration = MIL.M_NULL;
            if (File.Exists("CalibrationParam.mim") && MilCalibration == MIL.M_NULL)
            {
                // Read the image of the board and associate the calibration to the image.
                MilCalibration = MIL.McalRestore("CalibrationParam.mim", MilSystem, MIL.M_DEFAULT, ref MilCalibration);

                // Transform the image of the board.
                MIL.McalTransformImage(MilImage, MilImage, MilCalibration, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT, MIL.M_DEFAULT);
            }
        }

        /// <summary>
        /// 对图像进行校正（从多个Matrox图像）
        /// </summary>
        /// <param name="MilImages"></param>
        public void CalibrateImage(ref MIL_ID[] MilImages)
        {
            MIL_ID MilCalibration = MIL.M_NULL;
            if (File.Exists("CalibrationParam.mim") && MilCalibration == MIL.M_NULL)
            {
                for (int i = 0; i < MilImages.Length; i++)
                {
                    // Read the image of the board and associate the calibration to the image.
                    MilCalibration = MIL.McalRestore("CalibrationParam.mim", MilSystem, MIL.M_DEFAULT, ref MilCalibration);

                    // Transform the image of the board.
                    MIL.McalTransformImage(MilImages[i], MilImages[i], MilCalibration, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_DEFAULT, MIL.M_DEFAULT);
                }
            }
        }
    }
}