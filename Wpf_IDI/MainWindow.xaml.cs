using ACS.SPiiPlusNET;
using CameraManage;
using Microsoft.Win32;
using MotorManager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using IOControl;
using Automation.BDaq;
using LaserControlManager;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Input;
using XDrawerLib;
using XDrawerLib.Drawers;
using XDrawerLib.Helpers;

namespace Wpf_IDI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _ParametersData;

            GetCameraInfo(); // 获取相机参数
            LoadCameraCalibrationParameters(ParametersManager.GetPatameteName(ParametersManager.PName.Calibration)); // 加载标定参数

            LoadSpiiTcpIPAndPort(ParametersManager.GetPatameteName(ParametersManager.PName.SpiiTcp)); // 加载spii的tcp参数

            InitIO();
            UpdateIO();

            InitDrawing();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // 断开运动轴的连接
            if (_SpiiPlusManager.InitSpii)
            {
                this.ConnectedSpii.IsChecked = false;
                ConnectedSpii_Click(null, null);
            }
        }

        ParametersData _ParametersData = new ParametersData(); //参数数据

        #region Camera

        #region 相机参数相关
        BaslerCamera _BaslerCamera = new BaslerCamera();
        private bool InitCamera = false; //相机是否初始化
        #endregion

        /// <summary>
        /// 获取相机信息
        /// </summary>
        private void GetCameraInfo()
        {
            if (_BaslerCamera.CameraNameDic.Count > 0)
            {
                //this.ReSearchCameraButton.IsEnabled = false;
                foreach (var cameraInfo in _BaslerCamera.CameraNameDic)
                {
                    if (!this.CameraNameList.Items.Contains(cameraInfo.Key))
                    {
                        this.CameraNameList.Items.Add(cameraInfo.Key);
                    }
                }
            }
            else
            {
                //this.ReSearchCameraButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 连接/断开相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectedCamera_Click(object sender, RoutedEventArgs e)
        {
            if (this.CameraNameList.Text != null && this.CameraNameList.Text != "" && (bool)this.ConnectedCameraButton.IsChecked)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _BaslerCamera.ConnectedCamera((string)_BaslerCamera.CameraNameDic[this.CameraNameList.Text]["SerialNumber"]);

                    // 显示相机曝光值及大小
                    _ParametersData.CameraExposure = _BaslerCamera.Exposure;
                    _ParametersData.ImageWidth = _BaslerCamera.ImageWidth;
                    _ParametersData.ImageHeight = _BaslerCamera.ImageHeight;

                    //this.GrabButton.IsEnabled = true;
                    //this.SaveImageButton.IsEnabled = true;
                    //this.CameraCalibrationButton.IsEnabled = true;
                    _BaslerCamera.ImageCaptured += OnImageCaptured;

                    InitCamera = true;
                    LoggerManager.LogInfo("相机连接成功");
                }));
            }
            else //断开相机
            {
                this.ConnectedCameraButton.IsChecked = false;
                // 停止取图
                if (this.CameraGrabMovement.Text.ToLower().Contains("stop"))
                {
                    GrabButton_Click(sender, e);
                }

                _BaslerCamera.DisconnectedCamera();

                //this.GrabButton.IsEnabled = false;
                //this.SaveImageButton.IsEnabled = false;
                //this.CameraCalibrationButton.IsEnabled = false;
                _BaslerCamera.ImageCaptured -= OnImageCaptured;

                InitCamera = false;

                if (this.CameraNameList.Text == null || this.CameraNameList.Text == "")
                {
                    MessageBox.Show("No Camera!!!");
                    LoggerManager.LogError("没有找到相机");
                }
                else
                {
                    LoggerManager.LogInfo("相机断开连接");
                }

            }
        }

        /// <summary>
        /// 相机回调取图，通过委托传输数据
        /// </summary>
        /// <param name="imageData"></param>
        private void OnImageCaptured(BaslerCamera.CameraData imageData)
        {
            try
            {
                BitmapImage img = ImageConvert.ConvertImageDataToBitmapImage(imageData.data, imageData.width, imageData.height);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (this.TabControl.SelectedIndex == 0)
                    {
                        this.Picture.Source = img;
                    }
                    else if (this.TabControl.SelectedIndex == 1)
                    {
                        this.Picture_Motor.Source = img;
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 相机 取图/停止取图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrabButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InitCamera) { return; }

            if (this.CameraGrabMovement.Text.ToLower().Contains("start")) // 相机取图
            {
                if ((bool)this.GrabOnceButton.IsChecked)
                {
                    _BaslerCamera.GrabOnce();
                }
                if ((bool)this.GrabContinueButton.IsChecked)
                {
                    _BaslerCamera.GrabContinue();
                    this.CameraGrabMovement.Text = "Grab Stop";
                    this.GrabButton.Background = Brushes.Red;
                }
            }
            else //停止取图
            {
                _BaslerCamera.StopGrab();

                this.CameraGrabMovement.Text = "Grab Start";
                this.GrabButton.Background = Brushes.Green;
            }
        }

        /// <summary>
        /// 相机曝光修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraExposure_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!InitCamera) { return; }
            try
            {
                _BaslerCamera.Exposure = _ParametersData.CameraExposure;
            }
            catch (Exception ex)
            {
                LoggerManager.LogError($"设置相机曝光失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 重新搜索相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReSearchCameraButton_Click(object sender, RoutedEventArgs e)
        {
            _BaslerCamera.GetCameraInfoDic();

            // 如果找到相机
            if (_BaslerCamera.CameraNameDic.Count > 0)
            {
                GetCameraInfo();
            }
            else
            {
                MessageBox.Show("No camera found!!!");
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InitCamera) { return; }

            // 停止取图
            if (this.CameraGrabMovement.Text.ToLower().Contains("stop"))
            {
                GrabButton_Click(sender, e);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "image(*.bmp)|*.bmp|all file (*.*)|*.*";
            saveFileDialog.Title = "Save Image";

            if (saveFileDialog.ShowDialog() == true)
            {
                string strSavePath = saveFileDialog.FileName;
                _BaslerCamera.SaveImage(strSavePath);
            }
        }

        /// <summary>
        /// 相机标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraCalibrationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 修改相机标定参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyParameterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.ModifyParameterButtonClicked)
            {
                _ParametersData.ModifyParameterButtonClicked = true;

                LoadCameraCalibrationParameters(ParametersManager.GetPatameteName(ParametersManager.PName.Calibration));
            }
            else
            {
                _ParametersData.ModifyParameterButtonClicked = false;

                SaveCameraCalibrationParameters(ParametersManager.GetPatameteName(ParametersManager.PName.Calibration));
            }
        }

        /// <summary>
        /// 读取标定参数
        /// </summary>
        /// <param name="parameterName"></param>
        private void LoadCameraCalibrationParameters(string parameterName)
        {
            var parametr = ParametersManager.LoadParameters(parameterName);
            if (parametr.Count > 0)
            {
                _ParametersData.GRID_ROW_SPACING = double.Parse((string)parametr[parameterName][GRID_ROW_SPACING.Name]);
                _ParametersData.GRID_COLUMN_SPACING = double.Parse((string)parametr[parameterName][GRID_COLUMN_SPACING.Name]);
                _ParametersData.GRID_ROW_NUMBER = double.Parse((string)parametr[parameterName][GRID_ROW_NUMBER.Name]);
                _ParametersData.GRID_COLUMN_NUMBER = double.Parse((string)parametr[parameterName][GRID_COLUMN_NUMBER.Name]);
                _ParametersData.Pixel_To_World_Ratio = double.Parse((string)parametr[parameterName][Pixel_To_World_Ratio.Name]);
            }
            else
            {
                this.GRID_ROW_SPACING.Text = "1";
                this.GRID_COLUMN_SPACING.Text = "1";
                this.GRID_ROW_NUMBER.Text = "1";
                this.GRID_COLUMN_NUMBER.Text = "1";
            }
        }

        /// <summary>
        /// 保存标定参数
        /// </summary>
        /// <param name="parameterName"></param>
        private void SaveCameraCalibrationParameters(string parameterName)
        {
            List<TextBox> keys = new List<TextBox>()
            {
                GRID_ROW_SPACING, GRID_COLUMN_SPACING, GRID_ROW_NUMBER, GRID_COLUMN_NUMBER, Pixel_To_World_Ratio
            };

            ParametersManager.SaveParameters(keys, parameterName);
        }
        #endregion

        #region SpiiPlus

        #region 运动参数相关
        private SpiiPlusManager _SpiiPlusManager = new SpiiPlusManager();
        private const int AxisNumber = 3;
        Axis[] m_arrAxisList;//轴列表
        int m_nTotalBuffer;//buffer总数
        /// <summary>
        /// 加载spii的ip和port参数
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadSpiiTcpIPAndPort(string fileName)
        {
            var ipAndPort = ParametersManager.LoadParameters(fileName);
            if (ipAndPort.Count > 0)
            {
                this.IpOfSpii.Text = ipAndPort[fileName][IpOfSpii.Name].ToString();
                this.PortOfSpii.Text = ipAndPort[fileName][PortOfSpii.Name].ToString();
            }
            else
            {
                this.IpOfSpii.Text = "192.168.3.100";
                this.PortOfSpii.Text = "701";
            }
        }

        /// <summary>
        /// 保存spii的ip和port参数
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveSpiiTcpIPAndPort(string fileName)
        {
            List<TextBox> keys = new List<TextBox>()
            {
                IpOfSpii, PortOfSpii
            };

            ParametersManager.SaveParameters(keys, fileName);
        }

        /// <summary>
        /// 读取运动轴的参数
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadMotorParameters(string fileName)
        {
            var parameters = ParametersManager.LoadParameters(fileName);
            if (parameters.Count > 0)
            {
                _ParametersData.Velocity0 = double.Parse((string)parameters[fileName][Velocity0.Name]);
                _SpiiPlusManager.SetVelocity(0, _ParametersData.Velocity0);
                _ParametersData.Velocity1 = double.Parse((string)parameters[fileName][Velocity1.Name]);
                _SpiiPlusManager.SetVelocity(1, _ParametersData.Velocity1);
                _ParametersData.Velocity2 = double.Parse((string)parameters[fileName][Velocity2.Name]);
                _SpiiPlusManager.SetVelocity(2, _ParametersData.Velocity2);

                _ParametersData.Acceleration0 = double.Parse((string)parameters[fileName][Acceleration0.Name]);
                _SpiiPlusManager.SetAcceleration(0, _ParametersData.Acceleration0);
                _ParametersData.Acceleration1 = double.Parse((string)parameters[fileName][Acceleration1.Name]);
                _SpiiPlusManager.SetAcceleration(1, _ParametersData.Acceleration1);
                _ParametersData.Acceleration2 = double.Parse((string)parameters[fileName][Acceleration2.Name]);
                _SpiiPlusManager.SetAcceleration(2, _ParametersData.Acceleration2);

                _ParametersData.Deceleration0 = double.Parse((string)parameters[fileName][Deceleration0.Name]);
                _SpiiPlusManager.SetDeceleration(0, _ParametersData.Deceleration0);
                _ParametersData.Deceleration1 = double.Parse((string)parameters[fileName][Deceleration1.Name]);
                _SpiiPlusManager.SetDeceleration(1, _ParametersData.Deceleration1);
                _ParametersData.Deceleration2 = double.Parse((string)parameters[fileName][Deceleration2.Name]);
                _SpiiPlusManager.SetDeceleration(2, _ParametersData.Deceleration2);

                _ParametersData.KillDeceleration0 = double.Parse((string)parameters[fileName][KillDeceleration0.Name]);
                _SpiiPlusManager.SetKillDeceleration(0, _ParametersData.KillDeceleration0);
                _ParametersData.KillDeceleration1 = double.Parse((string)parameters[fileName][KillDeceleration1.Name]);
                _SpiiPlusManager.SetKillDeceleration(1, _ParametersData.KillDeceleration1);
                _ParametersData.KillDeceleration2 = double.Parse((string)parameters[fileName][KillDeceleration2.Name]);
                _SpiiPlusManager.SetKillDeceleration(2, _ParametersData.KillDeceleration2);

                _ParametersData.Jerk0 = double.Parse((string)parameters[fileName][Jerk0.Name]);
                _SpiiPlusManager.SetJerk(0, _ParametersData.Jerk0);
                _ParametersData.Jerk1 = double.Parse((string)parameters[fileName][Jerk1.Name]);
                _SpiiPlusManager.SetJerk(1, _ParametersData.Jerk1);
                _ParametersData.Jerk2 = double.Parse((string)parameters[fileName][Jerk2.Name]);
                _SpiiPlusManager.SetJerk(2, _ParametersData.Jerk2);
            }
        }

        /// <summary>
        /// 保存运动轴的参数
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveMotorParameters(string fileName)
        {
            List<TextBox> keys = new List<TextBox>()
            {
                Velocity0, Velocity1, Velocity2,
                Acceleration0, Acceleration1, Acceleration2,
                Deceleration0, Deceleration1, Deceleration2,
                KillDeceleration0, KillDeceleration1, KillDeceleration2,
                Jerk0, Jerk1, Jerk2,
            };
            ParametersManager.SaveParameters(keys, fileName);
        }
        #endregion

        /// <summary>
        /// 连接spii
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectedSpii_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)this.ConnectedSpii.IsChecked)
                {
                    string ip = this.IpOfSpii.Text;
                    int port = int.Parse(this.PortOfSpii.Text);

                    (m_arrAxisList, m_nTotalBuffer) = _SpiiPlusManager.ConnectedSpii(ip, port);
                    if (m_arrAxisList == null || m_nTotalBuffer == -1)
                    {
                        this.ConnectedSpii.IsChecked = false;
                        MessageBox.Show("Connected to the Axis failed!!! Pls check the ip or port");
                        return;
                    }

                    _SpiiPlusManager.InitSpii = !_SpiiPlusManager.InitSpii;
                    SaveSpiiTcpIPAndPort(ParametersManager.GetPatameteName(ParametersManager.PName.SpiiTcp));
                    LoadMotorParameters(ParametersManager.GetPatameteName(ParametersManager.PName.SpiiMotor));

                    Task.Run(() =>
                    {
                        while (_SpiiPlusManager.InitSpii)
                        {
                            for (int i = 0; i < AxisNumber; i++)
                            {
                                UpdateAxisParameters(i);
                            }
                            Thread.Sleep(30);
                        }
                    });
                }
                else
                {
                    _SpiiPlusManager.DisableAllAxis();
                    _SpiiPlusManager.InitSpii = !_SpiiPlusManager.InitSpii;
                    _SpiiPlusManager.DisConnectedSpii();
                    SaveMotorParameters(ParametersManager.GetPatameteName(ParametersManager.PName.SpiiMotor));
                }
            }
            catch (Exception ex)
            {
                this.ConnectedSpii.IsChecked = false;
                _SpiiPlusManager.InitSpii = false;
                LoggerManager.LogError($"Connected the spii error, because{ex}");
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 更新轴的数据
        /// </summary>
        private void UpdateAxisParameters(int axis)
        {
            // 轴的运动状态
            var motorState = _SpiiPlusManager.GetMotorState(axis);
            switch (axis)
            {
                case 0:
                    {
                        if ((motorState & MotorStates.ACSC_MST_MOVE) != 0) { _ParametersData.IsMoving0 = true; } else { _ParametersData.IsMoving0 = false; }
                        if ((motorState & MotorStates.ACSC_MST_INPOS) != 0) { _ParametersData.IsInPosition0 = true; } else { _ParametersData.IsInPosition0 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ACC) != 0) { _ParametersData.IsAcceleration0 = true; } else { _ParametersData.IsAcceleration0 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ENABLE) != 0) { _ParametersData.IsEnable0 = true; } else { _ParametersData.IsEnable0 = false; }

                        _ParametersData.ReferencePosition0 = _SpiiPlusManager.GetReferencePosition(axis);
                        _ParametersData.FeedbackPosition0 = _SpiiPlusManager.GetFeedbackPosition(axis);
                        _ParametersData.ActualVelocity0 = _SpiiPlusManager.GetActualVelocity(axis);
                        _ParametersData.PositionError0 = _SpiiPlusManager.GetPositionError(axis);

                        break;
                    }
                case 1:
                    {
                        if ((motorState & MotorStates.ACSC_MST_MOVE) != 0) { _ParametersData.IsMoving1 = true; } else { _ParametersData.IsMoving1 = false; }
                        if ((motorState & MotorStates.ACSC_MST_INPOS) != 0) { _ParametersData.IsInPosition1 = true; } else { _ParametersData.IsInPosition1 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ACC) != 0) { _ParametersData.IsAcceleration1 = true; } else { _ParametersData.IsAcceleration1 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ENABLE) != 0) { _ParametersData.IsEnable1 = true; } else { _ParametersData.IsEnable1 = false; }

                        _ParametersData.ReferencePosition1 = _SpiiPlusManager.GetReferencePosition(axis);
                        _ParametersData.FeedbackPosition1 = _SpiiPlusManager.GetFeedbackPosition(axis);
                        _ParametersData.ActualVelocity1 = _SpiiPlusManager.GetActualVelocity(axis);
                        _ParametersData.PositionError1 = _SpiiPlusManager.GetPositionError(axis);

                        break;
                    }
                case 2:
                    {
                        if ((motorState & MotorStates.ACSC_MST_MOVE) != 0) { _ParametersData.IsMoving2 = true; } else { _ParametersData.IsMoving2 = false; }
                        if ((motorState & MotorStates.ACSC_MST_INPOS) != 0) { _ParametersData.IsInPosition2 = true; } else { _ParametersData.IsInPosition2 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ACC) != 0) { _ParametersData.IsAcceleration2 = true; } else { _ParametersData.IsAcceleration2 = false; }
                        if ((motorState & MotorStates.ACSC_MST_ENABLE) != 0) { _ParametersData.IsEnable2 = true; } else { _ParametersData.IsEnable2 = false; }

                        _ParametersData.ReferencePosition2 = _SpiiPlusManager.GetReferencePosition(axis);
                        _ParametersData.FeedbackPosition2 = _SpiiPlusManager.GetFeedbackPosition(axis);
                        _ParametersData.ActualVelocity2 = _SpiiPlusManager.GetActualVelocity(axis);
                        _ParametersData.PositionError2 = _SpiiPlusManager.GetPositionError(axis);

                        break;
                    }
            }

            // 轴的限位状态
            var objReadVar = _SpiiPlusManager._ObjReadVar;
            if (objReadVar != null)
            {
                Array arrReadVector = objReadVar as Array;
                if (arrReadVector != null)
                {
                    UpdateAxisLimitState(axis, (int)arrReadVector.GetValue(axis));
                }
            }
        }

        /// <summary>
        /// 更新轴的限位状态
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="fault"></param>
        private void UpdateAxisLimitState(int axis, int fault)
        {
            if ((fault & (int)SafetyControlMasks.ACSC_SAFETY_LL) != 0)
            {
                switch (axis)
                {
                    case 0:
                        {
                            _ParametersData.IsLeftLimit0 = false;
                            break;
                        }
                    case 1:
                        {
                            _ParametersData.IsLeftLimit1 = false;
                            break;
                        }
                    case 2:
                        {
                            _ParametersData.IsLeftLimit2 = false;
                            break;
                        }
                }
            }
            else
            {
                switch (axis)
                {
                    case 0:
                        {
                            _ParametersData.IsLeftLimit0 = true;
                            break;
                        }
                    case 1:
                        {
                            _ParametersData.IsLeftLimit1 = true;
                            break;
                        }
                    case 2:
                        {
                            _ParametersData.IsLeftLimit2 = true;
                            break;
                        }
                }
            }

            if ((fault & (int)SafetyControlMasks.ACSC_SAFETY_RL) != 0)
            {
                switch (axis)
                {
                    case 0:
                        {
                            _ParametersData.IsRightLimit0 = false;
                            break;
                        }
                    case 1:
                        {
                            _ParametersData.IsRightLimit1 = false;
                            break;
                        }
                    case 2:
                        {
                            _ParametersData.IsRightLimit2 = false;
                            break;
                        }
                }
            }
            else
            {
                switch (axis)
                {
                    case 0:
                        {
                            _ParametersData.IsRightLimit0 = true;
                            break;
                        }
                    case 1:
                        {
                            _ParametersData.IsRightLimit1 = true;
                            break;
                        }
                    case 2:
                        {
                            _ParametersData.IsRightLimit2 = true;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 设置轴的运动参数
        /// </summary>
        private void SetAxisMovingParameters(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            switch (textBox.Tag)
            {
                case "v0":
                    {
                        _SpiiPlusManager.SetVelocity(0, _ParametersData.Velocity0);
                        break;
                    }
                case "v1":
                    {
                        _SpiiPlusManager.SetVelocity(1, _ParametersData.Velocity1);
                        break;
                    }
                case "v2":
                    {
                        _SpiiPlusManager.SetVelocity(2, _ParametersData.Velocity2);
                        break;
                    }

                case "a0":
                    {
                        _SpiiPlusManager.SetAcceleration(0, _ParametersData.Acceleration0);
                        break;
                    }
                case "a1":
                    {
                        _SpiiPlusManager.SetAcceleration(1, _ParametersData.Acceleration1);
                        break;
                    }
                case "a2":
                    {
                        _SpiiPlusManager.SetAcceleration(2, _ParametersData.Acceleration2);
                        break;
                    }

                case "d0":
                    {
                        _SpiiPlusManager.SetDeceleration(0, _ParametersData.Deceleration0);
                        break;
                    }
                case "d1":
                    {
                        _SpiiPlusManager.SetDeceleration(1, _ParametersData.Deceleration1);
                        break;
                    }
                case "d2":
                    {
                        _SpiiPlusManager.SetDeceleration(2, _ParametersData.Deceleration2);
                        break;
                    }

                case "k0":
                    {
                        _SpiiPlusManager.SetKillDeceleration(0, _ParametersData.KillDeceleration0);
                        break;
                    }
                case "k1":
                    {
                        _SpiiPlusManager.SetKillDeceleration(1, _ParametersData.KillDeceleration1);
                        break;
                    }
                case "k2":
                    {
                        _SpiiPlusManager.SetKillDeceleration(2, _ParametersData.KillDeceleration2);
                        break;
                    }

                case "j0":
                    {
                        _SpiiPlusManager.SetJerk(0, _ParametersData.Jerk0);
                        break;
                    }
                case "j1":
                    {
                        _SpiiPlusManager.SetJerk(1, _ParametersData.Jerk1);
                        break;
                    }
                case "j2":
                    {
                        _SpiiPlusManager.SetJerk(2, _ParametersData.Jerk2);
                        break;
                    }
            }
        }

        /// <summary>
        /// 使能所有轴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableAllAxisButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)EnableAllAxisButton.IsChecked)
                {
                    for (int i = 0; i < AxisNumber; i++)
                    {
                        _SpiiPlusManager.EnableSpecifiedAxis(i);
                    }
                }
                else
                {
                    _SpiiPlusManager.DisableAllAxis();
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogError($"Enable all axis failed, because{ex}");
                _SpiiPlusManager.DisableAllAxis();
            }
        }

        /// <summary>
        /// 0轴使能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableAxis0Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)EnableAxis0Button.IsChecked)
                {
                    _SpiiPlusManager.EnableSpecifiedAxis(0);
                }
                else
                {
                    _SpiiPlusManager.DisableSpecifiedAxis(0);
                }
            }
            catch (Exception ex)
            {
                _SpiiPlusManager.DisableSpecifiedAxis(0);
                EnableAxis0Button.IsChecked = false;
                LoggerManager.LogError($"Enable axis 0 failed, because {ex}");
            }
        }

        /// <summary>
        /// 1轴使能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableAxis1Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)EnableAxis1Button.IsChecked)
                {
                    _SpiiPlusManager.EnableSpecifiedAxis(1);
                }
                else
                {
                    _SpiiPlusManager.DisableSpecifiedAxis(1);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogError($"Enable axis 0 failed, because {ex}");
                _SpiiPlusManager.DisableSpecifiedAxis(1);
                EnableAxis1Button.IsChecked = false;
            }
        }

        /// <summary>
        /// 2轴使能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableAxis2Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)EnableAxis2Button.IsChecked)
                {
                    _SpiiPlusManager.EnableSpecifiedAxis(2);
                }
                else
                {
                    _SpiiPlusManager.DisableSpecifiedAxis(2);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogError($"Enable axis 0 failed, because {ex}");
                _SpiiPlusManager.DisableSpecifiedAxis(2);
                EnableAxis2Button.IsChecked = false;
            }
        }

        /// <summary>
        /// 0轴PTP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis0PTPButton_Click(object sender, RoutedEventArgs e)
        {
            double distance = double.Parse(this.Axis0PTPDistance.Text);
            _SpiiPlusManager.MoveToPosition(0, distance);
        }

        /// <summary>
        /// 0轴反向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis0JogNegative_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(0, true);
        }

        /// <summary>
        /// 0轴正向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis0JogPositive_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(0, false);
        }

        /// <summary>
        /// 1轴PTP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis1PTPButton_Click(object sender, RoutedEventArgs e)
        {
            double distance = double.Parse(this.Axis1PTPDistance.Text);
            _SpiiPlusManager.MoveToPosition(1, distance);
        }

        /// <summary>
        /// 1轴反向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis1JogNegative_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(1, false);
        }

        /// <summary>
        /// 1轴正向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis1JogPositive_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(1, true);
        }

        /// <summary>
        /// 2轴PTP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis2PTPButton_Click(object sender, RoutedEventArgs e)
        {
            double distance = double.Parse(this.Axis2PTPDistance.Text);
            _SpiiPlusManager.MoveToPosition(2, distance);
        }

        /// <summary>
        /// 2轴反向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis2JogNegative_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(2, false);
        }

        /// <summary>
        /// 2轴正向移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Axis2JogPositive_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.Jog(2, true);
        }

        /// <summary>
        /// 停止正在进行的运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMotorButton_Click(object sender, RoutedEventArgs e)
        {
            _SpiiPlusManager.StopAllAxis(m_arrAxisList);
            for (int i = 0; i < m_nTotalBuffer; i++)
            {
                _SpiiPlusManager.StopBuffer(i);
            }
        }

        /// <summary>
        /// 回零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Homing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Homing.IsEnabled = false;
                Task.Run(() =>
                {
                    _SpiiPlusManager.RunBuffer(2);
                    _SpiiPlusManager.RunBuffer(0);
                    _SpiiPlusManager.RunBuffer(1);

                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.Homing.IsEnabled = true;
                        MessageBox.Show("Homing Success !!!");
                    }));
                });
            }
            catch (Exception ex)
            {
                this.Homing.IsEnabled = true;
                MessageBox.Show("Homing Failed !!!");
                LoggerManager.LogError($"Homing failed, because {ex}");
            }
        }
        #endregion

        #region PCIe Control

        #region 参数相关
        private PCIE _PCIe = new PCIE();
        private IPGLaser _IPGLaser = new IPGLaser();

        List<bool> PCIe1730Input = new List<bool>();
        //List<bool> PCIe1730Output = new List<bool>();
        List<bool> PCIe1760Input = new List<bool>();
        //List<bool> PCIe1760Output = new List<bool>();

        private const int PCIe1730InputIO = 16;
        private const int PCIe1760InputIO = 8;

        //private Dictionary<Button, ButtonProfile> ButtonStates = new Dictionary<Button, ButtonProfile>();
        #endregion

        #region output按钮事件
        private void GATE_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.GATE)
            {
                MessageBoxResult result = MessageBox.Show("Whether to perform this operation?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var err = _PCIe.WritePortData(1, 0, 0, 1);
                    if (err != ErrorCode.Success)
                    {
                        MessageBox.Show($"Can't write the port data, because {err}");
                        return;
                    }
                }
                _ParametersData.GATE = !_ParametersData.GATE;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 0, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.GATE = !_ParametersData.GATE;
            }
        }

        private void ClampingCylFwd_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.ClampingCylFwd)
            {
                var err = _PCIe.WritePortData(1, 0, 1, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.ClampingCylFwd = !_ParametersData.ClampingCylFwd;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 1, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.ClampingCylFwd = !_ParametersData.ClampingCylFwd;
            }
        }

        private void SlidingCylFwd_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.SlidingCylFwd)
            {
                var err = _PCIe.WritePortData(1, 0, 3, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.SlidingCylFwd = !_ParametersData.SlidingCylFwd;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 3, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.SlidingCylFwd = !_ParametersData.SlidingCylFwd;
            }
        }

        private void SlidingCylBwd_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.SlidingCylBwd)
            {
                var err = _PCIe.WritePortData(1, 0, 4, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.SlidingCylBwd = !_ParametersData.SlidingCylBwd;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 4, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.SlidingCylBwd = !_ParametersData.SlidingCylBwd;
            }
        }

        private void StopperCyl1Fwd_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.StopperCyl1Fwd)
            {
                var err = _PCIe.WritePortData(1, 0, 5, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.StopperCyl1Fwd = !_ParametersData.StopperCyl1Fwd;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 5, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.StopperCyl1Fwd = !_ParametersData.StopperCyl1Fwd;
            }
        }

        private void StopperCyl23Fwd_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.StopperCyl23Fwd)
            {
                var err = _PCIe.WritePortData(1, 0, 7, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.StopperCyl23Fwd = !_ParametersData.StopperCyl23Fwd;
            }
            else
            {
                var err = _PCIe.WritePortData(1, 0, 7, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.StopperCyl23Fwd = !_ParametersData.StopperCyl23Fwd;
            }
        }

        private void Tower_G_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.Tower_G)
            {
                var err = _PCIe.WritePortData(2, 0, 0, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_G = !_ParametersData.Tower_G;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 0, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_G = !_ParametersData.Tower_G;
            }
        }

        private void Tower_R_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.Tower_R)
            {
                var err = _PCIe.WritePortData(2, 0, 1, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_R = !_ParametersData.Tower_R;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 1, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_R = !_ParametersData.Tower_R;
            }
        }

        private void Tower_A_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.Tower_A)
            {
                var err = _PCIe.WritePortData(2, 0, 2, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_A = !_ParametersData.Tower_A;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 2, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.Tower_A = !_ParametersData.Tower_A;
            }
        }

        private void TowerBuzzer_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.TowerBuzzer)
            {
                var err = _PCIe.WritePortData(2, 0, 3, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.TowerBuzzer = !_ParametersData.TowerBuzzer;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 3, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.TowerBuzzer = !_ParametersData.TowerBuzzer;
            }
        }

        private void LightController_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.LightController)
            {
                var err = _PCIe.WritePortData(2, 0, 4, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.LightController = !_ParametersData.LightController;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 4, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.LightController = !_ParametersData.LightController;
            }
        }

        private void PowerSupDischarge_Click(object sender, RoutedEventArgs e)
        {
            if (!_ParametersData.PowerSupDischarge)
            {
                var err = _PCIe.WritePortData(2, 0, 5, 1);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.PowerSupDischarge = !_ParametersData.PowerSupDischarge;
            }
            else
            {
                var err = _PCIe.WritePortData(2, 0, 5, 0);
                if (err != ErrorCode.Success)
                {
                    MessageBox.Show($"Can't write the port data, because {err}");
                    return;
                }
                _ParametersData.PowerSupDischarge = !_ParametersData.PowerSupDischarge;
            }
        }
        #endregion

        //private void OutputButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    if (ButtonStates.ContainsKey(button))
        //    {
        //        int port = ButtonStates[button].Port;
        //        int bit = ButtonStates[button].Bit;

        //        if (button.Name == "GATE" && !ButtonStates[button].IsClicked)
        //        {
        //            MessageBoxResult result = MessageBox.Show("Whether to perform this operation?",
        //                                              "Confirmation",
        //                                              MessageBoxButton.YesNo,
        //                                              MessageBoxImage.Question);
        //            if (result != MessageBoxResult.Yes) { return; }
        //        }

        //        switch (ButtonStates[button].IoIndex)
        //        {
        //            case 0:
        //                {
        //                    if (ButtonStates[button].IsClicked)
        //                    {
        //                        var err = _PCIe.WritePortData(0, port, bit, 0);
        //                        if (err != ErrorCode.Success)
        //                        {
        //                            MessageBox.Show($"Can't write the port data, because {err}");
        //                            return;
        //                        }
        //                        ButtonStates[button].IsClicked = !ButtonStates[button].IsClicked;
        //                    }
        //                    else
        //                    {
        //                        var err = _PCIe.WritePortData(0, port, bit, 1);
        //                        if (err != ErrorCode.Success)
        //                        {
        //                            MessageBox.Show($"Can't write the port data, because {err}");
        //                            return;
        //                        }
        //                        ButtonStates[button].IsClicked = !ButtonStates[button].IsClicked;
        //                    }

        //                    break;
        //                }
        //            case 1:
        //                {
        //                    if (ButtonStates[button].IsClicked)
        //                    {
        //                        var err = _PCIe.WritePortData(1, port, bit, 0);
        //                        if (err != ErrorCode.Success)
        //                        {
        //                            MessageBox.Show($"Can't write the port data, because {err}");
        //                            return;
        //                        }
        //                        ButtonStates[button].IsClicked = !ButtonStates[button].IsClicked;
        //                    }
        //                    else
        //                    {
        //                        var err = _PCIe.WritePortData(1, port, bit, 1);
        //                        if (err != ErrorCode.Success)
        //                        {
        //                            MessageBox.Show($"Can't write the port data, because {err}");
        //                            return;
        //                        }
        //                        ButtonStates[button].IsClicked = !ButtonStates[button].IsClicked;
        //                    }
        //                    break;
        //                }
        //        }
        //    }
        //}

        /// <summary>
        /// 连接激光服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectedIPGLaserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)this.ConnectedIPGLaserButton.IsChecked)
                {
                    string ip = this.IpOfIPGLaser.Text;
                    int port = int.Parse(this.PortOfIPGLaser.Text);
                    _IPGLaser.Connected(ip, port);
                    _IPGLaser.InitLaser = true;
                }
                else
                {
                    _IPGLaser.CloseConnected();
                    _IPGLaser.InitLaser = false;
                }
            }
            catch (Exception ex)
            {
                this.ConnectedIPGLaserButton.IsChecked = false;
                _IPGLaser.InitLaser = false;
                LoggerManager.LogError(ex.Message);
            }
        }

        private void DiodeCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (double.Parse(this.DiodeCurrent.Text) > this.DiodeCurrentSlider.Maximum)
                {
                    _ParametersData.DiodeCurrent = this.DiodeCurrentSlider.Maximum;
                }
                _IPGLaser.SetDiodeCurrent(_ParametersData.DiodeCurrent);
            }
            catch (Exception ex)
            {
                LoggerManager.LogError(ex.Message);
            }
        }

        private void PulseWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _IPGLaser.SetPulseWidth(_ParametersData.PulseWidth);
            }
            catch (Exception ex)
            {
                LoggerManager.LogError(ex.Message);
            }
        }

        private void PulseFrequency_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _IPGLaser.SetPulseFrequency(_ParametersData.PulseFrequency);
            }
            catch (Exception ex)
            {
                LoggerManager.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 将IO状态添加进列表中，便于操作
        /// </summary>
        private void InitIO()
        {
            PCIe1730Input = new List<bool>
            {
                _ParametersData.Emergency, _ParametersData.SafetyDoor ,_ParametersData.StartButton, _ParametersData.StopButton,
                _ParametersData.ResetButton, _ParametersData.LaserReady, _ParametersData.ClampingCylOpen, _ParametersData.ClampingCylClose,
                _ParametersData.SlidingCylOpen, _ParametersData.SlidingCylClose, _ParametersData.StopperCyl1Open, _ParametersData.StopperCyl1Close,
                _ParametersData.StopperCyl2Open, _ParametersData.StopperCyl2Close, _ParametersData.StopperCyl3Open, _ParametersData.StopperCyl3Close,
            };
            PCIe1760Input = new List<bool>
            {
                _ParametersData.CDAPressureSwitch, _ParametersData.ClampAirPresSwitch, _ParametersData.PositionInput1,
                _ParametersData.PositionInput2, _ParametersData.PositionInput3,
            };

            //ButtonStates[this.GATE] = new ButtonProfile(0, 0, 0, _ParametersData.GATE);
            //ButtonStates[this.ClampingCylFwd] = new ButtonProfile(0, 0, 1, _ParametersData.ClampingCylFwd);
            //ButtonStates[this.SlidingCylFwd] = new ButtonProfile(0, 0, 3, _ParametersData.SlidingCylFwd);
            //ButtonStates[this.SlidingCylBwd] = new ButtonProfile(0, 0, 4, _ParametersData.SlidingCylBwd);
            //ButtonStates[this.StopperCyl1Fwd] = new ButtonProfile(0, 0, 5, _ParametersData.StopperCyl1Fwd);
            //ButtonStates[this.StopperCyl23Fwd] = new ButtonProfile(0, 0, 7, _ParametersData.StopperCyl23Fwd);

            //ButtonStates[this.Tower_G] = new ButtonProfile(1, 0, 0, _ParametersData.Tower_G);
            //ButtonStates[this.Tower_R] = new ButtonProfile(1, 0, 1, _ParametersData.Tower_R);
            //ButtonStates[this.Tower_A] = new ButtonProfile(1, 0, 2, _ParametersData.Tower_A);
            //ButtonStates[this.TowerBuzzer] = new ButtonProfile(1, 0, 3, _ParametersData.TowerBuzzer);
            //ButtonStates[this.LightController] = new ButtonProfile(1, 0, 4, _ParametersData.LightController);
            //ButtonStates[this.PowerSupDischarge] = new ButtonProfile(1, 0, 5, _ParametersData.PowerSupDischarge);
        }

        /// <summary>
        /// 更新IO状态
        /// </summary>
        private void UpdateIO()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var err = _PCIe.GetPortData(1, i, out byte inPortData);
                        if (err == ErrorCode.Success)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                int index = i * 8 + j;
                                if (index < PCIe1730Input.Count)
                                {
                                    PCIe1730Input[index] = ((inPortData >> j) & 0x1) == 0 ? false : true;
                                    switch (index)
                                    {
                                        case 0:
                                            {
                                                _ParametersData.Emergency = PCIe1730Input[index];
                                                break;
                                            }
                                        case 1:
                                            {
                                                _ParametersData.SafetyDoor = PCIe1730Input[index];
                                                break;
                                            }
                                        case 2:
                                            {
                                                _ParametersData.StartButton = PCIe1730Input[index];
                                                break;
                                            }
                                        case 3:
                                            {
                                                _ParametersData.StopButton = PCIe1730Input[index];
                                                break;
                                            }
                                        case 4:
                                            {
                                                _ParametersData.ResetButton = PCIe1730Input[index];
                                                break;
                                            }
                                        case 5:
                                            {
                                                _ParametersData.LaserReady = PCIe1730Input[index];
                                                break;
                                            }
                                        case 6:
                                            {
                                                _ParametersData.ClampingCylOpen = PCIe1730Input[index];
                                                break;
                                            }
                                        case 7:
                                            {
                                                _ParametersData.ClampingCylClose = PCIe1730Input[index];
                                                break;
                                            }
                                        case 8:
                                            {
                                                _ParametersData.SlidingCylOpen = PCIe1730Input[index];
                                                break;
                                            }
                                        case 9:
                                            {
                                                _ParametersData.SlidingCylClose = PCIe1730Input[index];
                                                break;
                                            }
                                        case 10:
                                            {
                                                _ParametersData.StopperCyl1Open = PCIe1730Input[index];
                                                break;
                                            }
                                        case 11:
                                            {
                                                _ParametersData.StopperCyl1Close = PCIe1730Input[index];
                                                break;
                                            }
                                        case 12:
                                            {
                                                _ParametersData.StopperCyl2Open = PCIe1730Input[index];
                                                break;
                                            }
                                        case 13:
                                            {
                                                _ParametersData.StopperCyl2Close = PCIe1730Input[index];
                                                break;
                                            }
                                        case 14:
                                            {
                                                _ParametersData.StopperCyl3Open = PCIe1730Input[index];
                                                break;
                                            }
                                        case 15:
                                            {
                                                _ParametersData.StopperCyl3Close = PCIe1730Input[index];
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    for (int i = 0; i < _PCIe.FeaturesInPortCount; i++)
                    {
                        var err = _PCIe.GetPortData(2, i, out byte inPortData);
                        if (err == ErrorCode.Success)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                int index = i * 8 + j;
                                if (index < PCIe1760Input.Count)
                                {
                                    PCIe1760Input[index] = ((inPortData >> j) & 0x1) == 0 ? false : true;
                                    switch (index)
                                    {
                                        case 0:
                                            {
                                                _ParametersData.CDAPressureSwitch = PCIe1760Input[index];
                                                break;
                                            }
                                        case 1:
                                            {
                                                _ParametersData.ClampAirPresSwitch = PCIe1760Input[index];
                                                break;
                                            }
                                        case 2:
                                            {
                                                _ParametersData.PositionInput1 = PCIe1760Input[index];
                                                break;
                                            }
                                        case 3:
                                            {
                                                _ParametersData.PositionInput2 = PCIe1760Input[index];
                                                break;
                                            }
                                        case 4:
                                            {
                                                _ParametersData.PositionInput3 = PCIe1760Input[index];
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
            });
        }
        #endregion

        #region 画图
        #endregion

        #region 画图参数
        private Point startPoint; // 起始点
        public Drawer Drawer;

        /// <summary>
        /// 画图模式
        /// </summary>
        private enum DrawingMode
        {
            None,// 无
            Line,// 直线
            Rectangle,// 矩形
            Circle,// 圆
            Triangle,// 三角形
            BrokenLine,// 折线
            Ellipse,// 椭圆
            SquareRounded,// 圆角矩形
            Barcode,// 条码
            Text,// 文本
            Image,// 图片
        }
        private DrawingMode _DrawingMode;

        private Line _Line;
        private Rectangle _Rectangle;
        private Ellipse _Circle;
        private Polygon _Triangle;
        private Polygon _BrokenLine;
        private Ellipse _Ellipse;
        private Rectangle _SquareRounded;
        private TextBlock _Barcode;
        private TextBlock _Text;
        private Image _Image;

        Path path;

        #endregion

        private void InitDrawing()
        {
            Drawer = new Drawer(this.CanvasDXF);
            Drawer.ContinuousDraw = false;

            this.LineScan.Click += delegate (object sender, RoutedEventArgs e)
            {
                Drawer.DrawTool = Tool.Line;
            };
            this.SquareOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                Drawer.DrawTool = Tool.Rectangle;
            };
            this.CircleOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                //Drawer.DrawTool = Tool.Circle;
            };
            this.TriangleOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                Drawer.DrawTool = Tool.Triangle;
            };
            this.BrokenLine.Click += delegate (object sender, RoutedEventArgs e)
            {
                //Drawer.DrawTool = Tool.BrokenLine;
            };
            this.EllipseOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                Drawer.DrawTool = Tool.Ellipse;
            };
            this.SquareRoundedOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                //Drawer.DrawTool = Tool.SquareRounded;
            };
            this.Barcode.Click += delegate (object sender, RoutedEventArgs e)
            {
                //Drawer.DrawTool = Tool.Barcode;
            };
            this.FormatTextVariantOutline.Click += delegate (object sender, RoutedEventArgs e)
            {
                Drawer.DrawTool = Tool.Text;
            };
            this.Image.Click += delegate (object sender, RoutedEventArgs e)
            {
                //Drawer.DrawTool = Tool.Image;
            };
        }


        ///// <summary>
        ///// 点击画图按钮，修改画图模式
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void DrawingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    switch (int.Parse((string)button.Tag))
        //    {
        //        case 0:
        //            {
        //                _DrawingMode = DrawingMode.Line;
        //                break;
        //            }
        //        case 1:
        //            {
        //                _DrawingMode = DrawingMode.Rectangle;
        //                break;
        //            }
        //        case 2:
        //            {
        //                _DrawingMode = DrawingMode.Circle;
        //                break;
        //            }
        //        case 3:
        //            {
        //                _DrawingMode = DrawingMode.Triangle;
        //                break;
        //            }
        //        case 4:
        //            {
        //                _DrawingMode = DrawingMode.BrokenLine;
        //                break;
        //            }
        //        case 5:
        //            {
        //                _DrawingMode = DrawingMode.Ellipse;
        //                break;
        //            }
        //        case 6:
        //            {
        //                _DrawingMode = DrawingMode.SquareRounded;
        //                break;
        //            }
        //        case 7:
        //            {
        //                _DrawingMode = DrawingMode.Barcode;
        //                break;
        //            }
        //        case 8:
        //            {
        //                _DrawingMode = DrawingMode.Text;
        //                break;
        //            }
        //        case 9:
        //            {
        //                _DrawingMode = DrawingMode.Image;
        //                break;
        //            }
        //    }


        //}

        //private void CanvasDXF_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_ParametersData.IsDrawing)
        //    {
        //        Point endPoint = e.GetPosition(this.CanvasDXF);
        //        double width = Math.Abs(endPoint.X - startPoint.X);
        //        double height = Math.Abs(endPoint.Y - startPoint.Y);

        //        switch (_DrawingMode)
        //        {
        //            case DrawingMode.Line:
        //                {
        //                    path.Data = new LineGeometry(startPoint, endPoint);
        //                    break;
        //                }
        //            case DrawingMode.Rectangle:
        //                {
        //                    if (endPoint.X < startPoint.X)
        //                    {
        //                        Canvas.SetLeft(_Rectangle, endPoint.X);
        //                    }
        //                    if (endPoint.Y < startPoint.Y)
        //                    {
        //                        Canvas.SetTop(_Rectangle, endPoint.Y);
        //                    }
        //                    _Rectangle.Width = width;
        //                    _Rectangle.Height = height;
        //                    break;
        //                }
        //            case DrawingMode.Circle:
        //                {
        //                    if (endPoint.X < startPoint.X)
        //                    {
        //                        Canvas.SetLeft(_Circle, endPoint.X);
        //                    }
        //                    if (endPoint.Y < startPoint.Y)
        //                    {
        //                        Canvas.SetTop(_Circle, endPoint.Y);
        //                    }
        //                    _Circle.Width = width;
        //                    _Circle.Height = width;
        //                    break;
        //                }
        //            case DrawingMode.Triangle:
        //                {
        //                    if (endPoint.X < startPoint.X)
        //                    {
        //                        Canvas.SetLeft(_Triangle, endPoint.X);
        //                    }
        //                    if (endPoint.Y < startPoint.Y)
        //                    {
        //                        Canvas.SetTop(_Triangle, endPoint.Y);
        //                    }
        //                    _Triangle.Points = new PointCollection()
        //                    {
        //                        new Point(width / 2, 0),
        //                        new Point(0, height),
        //                        new Point(width, height),
        //                    };
        //                    break;
        //                }
        //            case DrawingMode.BrokenLine:
        //                {
        //                    break;
        //                }
        //            case DrawingMode.Ellipse:
        //                {
        //                    if (endPoint.X < startPoint.X)
        //                    {
        //                        Canvas.SetLeft(_Ellipse, endPoint.X);
        //                    }
        //                    if (endPoint.Y < startPoint.Y)
        //                    {
        //                        Canvas.SetTop(_Ellipse, endPoint.Y);
        //                    }
        //                    _Ellipse.Width = width;
        //                    _Ellipse.Height = height;
        //                    break;
        //                }
        //            case DrawingMode.SquareRounded:
        //                {
        //                    if (endPoint.X < startPoint.X)
        //                    {
        //                        Canvas.SetLeft(_SquareRounded, endPoint.X);
        //                    }
        //                    if (endPoint.Y < startPoint.Y)
        //                    {
        //                        Canvas.SetTop(_SquareRounded, endPoint.Y);
        //                    }
        //                    _SquareRounded.Width = width;
        //                    _SquareRounded.Height = height;
        //                    _SquareRounded.RadiusX = width / 10;
        //                    _SquareRounded.RadiusY = width / 10;
        //                    break;
        //                }
        //        }

        //    }
        //}

        //private void CanvasDXF_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (_ParametersData.IsDrawing)
        //    {
        //        _ParametersData.IsDrawing = false;
        //        return;
        //    }
        //    if (DrawingMode.None == _DrawingMode)
        //    {
        //        return;
        //    }
        //    _ParametersData.IsDrawing = true;

        //    startPoint = e.GetPosition(this.CanvasDXF);

        //    LineGeometry lineGeometry = new LineGeometry()
        //    {
        //        StartPoint = startPoint,
        //        EndPoint = startPoint,
        //    };
        //    path = new Path()
        //    {
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 1,
        //        Data = lineGeometry,
        //    };

        //    switch (_DrawingMode)
        //    {
        //        case DrawingMode.Line:
        //            {
        //                this.CanvasDXF.Children.Add(path);
        //                break;
        //            }
        //        case DrawingMode.Rectangle:
        //            {
        //                _Rectangle = new Rectangle()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                    //Fill = Brushes.Transparent,
        //                };
        //                Canvas.SetLeft(_Rectangle, startPoint.X);
        //                Canvas.SetTop(_Rectangle, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_Rectangle);
        //                break;
        //            }
        //        case DrawingMode.Circle:
        //            {
        //                _Circle = new Ellipse()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                };
        //                Canvas.SetLeft(_Circle, startPoint.X);
        //                Canvas.SetTop(_Circle, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_Circle);
        //                break;
        //            }
        //        case DrawingMode.Triangle:
        //            {
        //                _Triangle = new Polygon()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                };
        //                Canvas.SetLeft(_Triangle, startPoint.X);
        //                Canvas.SetTop(_Triangle, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_Triangle);
        //                break;
        //            }
        //        case DrawingMode.BrokenLine:
        //            {
        //                _BrokenLine = new Polygon()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                };
        //                Canvas.SetLeft(_BrokenLine, startPoint.X);
        //                Canvas.SetTop(_BrokenLine, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_BrokenLine);
        //                break;
        //            }
        //        case DrawingMode.Ellipse:
        //            {
        //                _Ellipse = new Ellipse()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                };
        //                Canvas.SetLeft(_Ellipse, startPoint.X);
        //                Canvas.SetTop(_Ellipse, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_Ellipse);
        //                break;
        //            }
        //        case DrawingMode.SquareRounded:
        //            {
        //                _SquareRounded = new Rectangle()
        //                {
        //                    Stroke = Brushes.Black,
        //                    StrokeThickness = 1,
        //                };
        //                Canvas.SetLeft(_SquareRounded, startPoint.X);
        //                Canvas.SetTop(_SquareRounded, startPoint.Y);
        //                this.CanvasDXF.Children.Add(_SquareRounded);
        //                break;
        //            }
        //        case DrawingMode.Barcode:
        //            {

        //                break;
        //            }
        //        case DrawingMode.Text:
        //            {
        //                break;
        //            }
        //        case DrawingMode.Image:
        //            {
        //                break;
        //            }
        //    }


        //}

        //private void CanvasDXF_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    //_ParametersData.IsDrawing = false;
        //    return;
        //    switch (_DrawingMode)
        //    {
        //        case DrawingMode.Line:
        //            {
        //                _Line = null;
        //                break;
        //            }
        //        case DrawingMode.Rectangle:
        //            {
        //                _Rectangle = null;
        //                break;
        //            }
        //        case DrawingMode.Circle:
        //            {
        //                _Circle = null;
        //                break;
        //            }
        //        case DrawingMode.Triangle:
        //            {
        //                _Triangle = null;
        //                break;
        //            }
        //        case DrawingMode.BrokenLine:
        //            {
        //                //_BrokenLine = null;
        //                break;
        //            }
        //        case DrawingMode.Ellipse:
        //            {
        //                _Ellipse = null;
        //                break;
        //            }
        //        case DrawingMode.SquareRounded:
        //            {
        //                _SquareRounded = null;
        //                break;
        //            }
        //        case DrawingMode.Barcode:
        //            {
        //                break;
        //            }
        //        case DrawingMode.Text:
        //            {
        //                break;
        //            }
        //        case DrawingMode.Image:
        //            {
        //                break;
        //            }
        //    }
        //}
    }

    //public class ButtonProfile
    //{
    //    public int IoIndex { get; set; }
    //    public int Port { get; set; }
    //    public int Bit { get; set; }
    //    public bool IsClicked { get; set; }

    //    public ButtonProfile(int _IoIndex, int _Port, int _Bit, bool _IsClicked)
    //    {
    //        IoIndex = _IoIndex;
    //        Port = _Port;
    //        Bit = _Bit;
    //        IsClicked = _IsClicked;
    //    }
    //}
}
