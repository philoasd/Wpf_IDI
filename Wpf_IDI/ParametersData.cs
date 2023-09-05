using System.ComponentModel;

namespace Wpf_IDI
{
    public class ParametersData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 相机曝光
        /// </summary>
        private double _CameraExposure = 0;
        public double CameraExposure
        {
            get { return _CameraExposure; }
            set
            {
                if (_CameraExposure != value)
                {
                    _CameraExposure = value;
                    OnPropertyChanged(nameof(CameraExposure));
                }
            }
        }

        /// <summary>
        /// 相机宽度
        /// </summary>
        private double _ImageWidth = 0;
        public double ImageWidth
        {
            get { return _ImageWidth; }
            set
            {
                if (_ImageWidth != value)
                {
                    _ImageWidth = value;
                    OnPropertyChanged(nameof(ImageWidth));
                }
            }
        }

        /// <summary>
        /// 相机高度
        /// </summary>
        private double _ImageHeight = 0;
        public double ImageHeight
        {
            get { return _ImageHeight; }
            set
            {
                if (_ImageHeight != value)
                {
                    _ImageHeight = value;
                    OnPropertyChanged(nameof(ImageHeight));
                }
            }
        }

        /// <summary>
        /// 标定参数：横向间距
        /// </summary>
        private double _GRID_ROW_SPACINGt = 0;
        public double GRID_ROW_SPACING
        {
            get { return _GRID_ROW_SPACINGt; }
            set
            {
                if (_GRID_ROW_SPACINGt != value)
                {
                    _GRID_ROW_SPACINGt = value;
                    OnPropertyChanged(nameof(GRID_ROW_SPACING));
                }
            }
        }

        /// <summary>
        /// 标定参数：纵向间距
        /// </summary>
        private double _GRID_COLUMN_SPACING = 0;
        public double GRID_COLUMN_SPACING
        {
            get { return _GRID_COLUMN_SPACING; }
            set
            {
                if (_GRID_COLUMN_SPACING != value)
                {
                    _GRID_COLUMN_SPACING = value;
                    OnPropertyChanged(nameof(GRID_COLUMN_SPACING));
                }
            }
        }

        /// <summary>
        /// 标定参数：横向点数
        /// </summary>
        private double _GRID_ROW_NUMBER = 0;
        public double GRID_ROW_NUMBER
        {
            get { return _GRID_ROW_NUMBER; }
            set
            {
                if (_GRID_ROW_NUMBER != value)
                {
                    _GRID_ROW_NUMBER = value;
                    OnPropertyChanged(nameof(GRID_ROW_NUMBER));
                }
            }
        }

        /// <summary>
        /// 标定参数：纵向点数
        /// </summary>
        private double _GRID_COLUMN_NUMBER = 0;
        public double GRID_COLUMN_NUMBER
        {
            get { return _GRID_COLUMN_NUMBER; }
            set
            {
                if (_GRID_COLUMN_NUMBER != value)
                {
                    _GRID_COLUMN_NUMBER = value;
                    OnPropertyChanged(nameof(GRID_COLUMN_NUMBER));
                }
            }
        }

        /// <summary>
        /// 标定参数：像素和实际距离的比例
        /// </summary>
        private double _Pixel_To_World_Ratio = 0;
        public double Pixel_To_World_Ratio
        {
            get { return _Pixel_To_World_Ratio; }
            set
            {
                if (_Pixel_To_World_Ratio != value)
                {
                    _Pixel_To_World_Ratio = value;
                    OnPropertyChanged(nameof(Pixel_To_World_Ratio));
                }
            }
        }

        /// <summary>
        /// 修改标定参数按钮的点击bool值，点击后为true，可修改参数，自身背景色变红；
        /// 再点一次变为false，不可修改参数，自身背景色变绿
        /// </summary>
        private bool _ModifyParameterButtonClicked = false;
        public bool ModifyParameterButtonClicked
        {
            get { return _ModifyParameterButtonClicked; }
            set
            {
                if (_ModifyParameterButtonClicked != value)
                {
                    _ModifyParameterButtonClicked = value;
                    OnPropertyChanged(nameof(ModifyParameterButtonClicked));
                }
            }
        }

        /// <summary>
        /// 所有轴是否使能
        /// </summary>
        private bool _IsAllAxisEnable = false;
        public bool IsAllAxisEnable
        {
            get { return _IsAllAxisEnable; }
            set
            {
                if (_IsAllAxisEnable != value)
                {
                    _IsAllAxisEnable = value;
                    OnPropertyChanged(nameof(IsAllAxisEnable));
                }
            }
        }
        private void UpdateIsAllAxisEnable()
        {
            IsAllAxisEnable = IsEnable0 && IsEnable1 && IsEnable2;
        }

        #region Axis 0 参数及状态
        /// <summary>
        /// 速度
        /// </summary>
        private double _Velocity0 = 0;
        public double Velocity0
        {
            get { return _Velocity0; }
            set
            {
                if (_Velocity0 != value)
                {
                    _Velocity0 = value;
                    OnPropertyChanged(nameof(Velocity0));
                }
            }
        }

        /// <summary>
        /// 加速度
        /// </summary>
        private double _Acceleration0 = 0;
        public double Acceleration0
        {
            get { return _Acceleration0; }
            set
            {
                if (_Acceleration0 != value)
                {
                    _Acceleration0 = value;
                    OnPropertyChanged(nameof(Acceleration0));
                }
            }
        }

        /// <summary>
        /// 减速度
        /// </summary>
        private double _Deceleration0 = 0;
        public double Deceleration0
        {
            get { return _Deceleration0; }
            set
            {
                if (_Deceleration0 != value)
                {
                    _Deceleration0 = value;
                    OnPropertyChanged(nameof(Deceleration0));
                }
            }
        }

        /// <summary>
        /// 加速度的加速度
        /// </summary>
        private double _KillDeceleration0 = 0;
        public double KillDeceleration0
        {
            get { return _KillDeceleration0; }
            set
            {
                if (_KillDeceleration0 != value)
                {
                    _KillDeceleration0 = value;
                    OnPropertyChanged(nameof(KillDeceleration0));
                }
            }
        }

        /// <summary>
        /// 减速度的减速度
        /// </summary>
        private double _Jerk0 = 0;
        public double Jerk0
        {
            get { return _Jerk0; }
            set
            {
                if (_Jerk0 != value)
                {
                    _Jerk0 = value;
                    OnPropertyChanged(nameof(Jerk0));
                }
            }
        }

        /// <summary>
        /// 参考位置
        /// </summary>
        private double _ReferencePosition0 = 0;
        public double ReferencePosition0
        {
            get { return _ReferencePosition0; }
            set
            {
                if (_ReferencePosition0 != value)
                {
                    _ReferencePosition0 = value;
                    OnPropertyChanged(nameof(ReferencePosition0));
                }
            }
        }

        /// <summary>
        /// 反馈位置
        /// </summary>
        private double _FeedbackPosition0 = 0;
        public double FeedbackPosition0
        {
            get { return _FeedbackPosition0; }
            set
            {
                if (_FeedbackPosition0 != value)
                {
                    _FeedbackPosition0 = value;
                    OnPropertyChanged(nameof(FeedbackPosition0));
                }
            }
        }

        /// <summary>
        /// 当前速度
        /// </summary>
        private double _ActualVelocity0 = 0;
        public double ActualVelocity0
        {
            get { return _ActualVelocity0; }
            set
            {
                if (_ActualVelocity0 != value)
                {
                    _ActualVelocity0 = value;
                    OnPropertyChanged(nameof(ActualVelocity0));
                }
            }
        }

        /// <summary>
        /// 位置偏差
        /// </summary>
        private double _PositionError0 = 0;
        public double PositionError0
        {
            get { return _PositionError0; }
            set
            {
                if (_PositionError0 != value)
                {
                    _PositionError0 = value;
                    OnPropertyChanged(nameof(PositionError0));
                }
            }
        }

        /// <summary>
        /// 是否在运动
        /// </summary>
        private bool _IsMoving0 = false;
        public bool IsMoving0
        {
            get { return _IsMoving0; }
            set
            {
                if (_IsMoving0 != value)
                {
                    _IsMoving0 = value;
                    OnPropertyChanged(nameof(IsMoving0));
                }
            }
        }

        /// <summary>
        /// 是否在加速/减速
        /// </summary>
        private bool _IsAcceleration0 = false;
        public bool IsAcceleration0
        {
            get { return _IsAcceleration0; }
            set
            {
                if (_IsAcceleration0 != value)
                {
                    _IsAcceleration0 = value;
                    OnPropertyChanged(nameof(IsAcceleration0));
                }
            }
        }

        /// <summary>
        /// 是否到位
        /// </summary>
        private bool _IsInPosition0 = false;
        public bool IsInPosition0
        {
            get { return _IsInPosition0; }
            set
            {
                if (_IsInPosition0 != value)
                {
                    _IsInPosition0 = value;
                    OnPropertyChanged(nameof(IsInPosition0));
                }
            }
        }

        /// <summary>
        /// 是否使能
        /// </summary>
        private bool _IsEnable0 = false;
        public bool IsEnable0
        {
            get { return _IsEnable0; }
            set
            {
                if (_IsEnable0 != value)
                {
                    _IsEnable0 = value;
                    UpdateIsAllAxisEnable();
                    OnPropertyChanged(nameof(IsEnable0));
                }
            }
        }

        /// <summary>
        /// 0轴左限位
        /// </summary>
        private bool _IsLeftLimit0 = true;
        public bool IsLeftLimit0
        {
            get { return _IsLeftLimit0; }
            set
            {
                if (_IsLeftLimit0 != value)
                {
                    _IsLeftLimit0 = value;
                    OnPropertyChanged(nameof(IsLeftLimit0));
                }
            }
        }

        /// <summary>
        /// 0轴右限位
        /// </summary>
        private bool _IsRightLimit0 = true;
        public bool IsRightLimit0
        {
            get { return _IsRightLimit0; }
            set
            {
                if (_IsRightLimit0 != value)
                {
                    _IsRightLimit0 = value;
                    OnPropertyChanged(nameof(IsRightLimit0));
                }
            }
        }
        #endregion

        #region Axis 1 参数及状态
        /// <summary>
        /// 速度
        /// </summary>
        private double _Velocity1 = 0;
        public double Velocity1
        {
            get { return _Velocity1; }
            set
            {
                if (_Velocity1 != value)
                {
                    _Velocity1 = value;
                    OnPropertyChanged(nameof(Velocity1));
                }
            }
        }

        /// <summary>
        /// 加速度
        /// </summary>
        private double _Acceleration1 = 0;
        public double Acceleration1
        {
            get { return _Acceleration1; }
            set
            {
                if (_Acceleration1 != value)
                {
                    _Acceleration1 = value;
                    OnPropertyChanged(nameof(Acceleration1));
                }
            }
        }

        /// <summary>
        /// 减速度
        /// </summary>
        private double _Deceleration1 = 0;
        public double Deceleration1
        {
            get { return _Deceleration1; }
            set
            {
                if (_Deceleration1 != value)
                {
                    _Deceleration1 = value;
                    OnPropertyChanged(nameof(Deceleration1));
                }
            }
        }

        /// <summary>
        /// 加速度的加速度
        /// </summary>
        private double _KillDeceleration1 = 0;
        public double KillDeceleration1
        {
            get { return _KillDeceleration1; }
            set
            {
                if (_KillDeceleration1 != value)
                {
                    _KillDeceleration1 = value;
                    OnPropertyChanged(nameof(KillDeceleration1));
                }
            }
        }

        /// <summary>
        /// 减速度的减速度
        /// </summary>
        private double _Jerk1 = 0;
        public double Jerk1
        {
            get { return _Jerk1; }
            set
            {
                if (_Jerk1 != value)
                {
                    _Jerk1 = value;
                    OnPropertyChanged(nameof(Jerk1));
                }
            }
        }

        /// <summary>
        /// 参考位置
        /// </summary>
        private double _ReferencePosition1 = 0;
        public double ReferencePosition1
        {
            get { return _ReferencePosition1; }
            set
            {
                if (_ReferencePosition1 != value)
                {
                    _ReferencePosition1 = value;
                    OnPropertyChanged(nameof(ReferencePosition1));
                }
            }
        }

        /// <summary>
        /// 反馈位置
        /// </summary>
        private double _FeedbackPosition1 = 0;
        public double FeedbackPosition1
        {
            get { return _FeedbackPosition1; }
            set
            {
                if (_FeedbackPosition1 != value)
                {
                    _FeedbackPosition1 = value;
                    OnPropertyChanged(nameof(FeedbackPosition1));
                }
            }
        }

        /// <summary>
        /// 当前速度
        /// </summary>
        private double _ActualVelocity1 = 0;
        public double ActualVelocity1
        {
            get { return _ActualVelocity1; }
            set
            {
                if (_ActualVelocity1 != value)
                {
                    _ActualVelocity1 = value;
                    OnPropertyChanged(nameof(ActualVelocity1));
                }
            }
        }

        /// <summary>
        /// 位置偏差
        /// </summary>
        private double _PositionError1 = 0;
        public double PositionError1
        {
            get { return _PositionError1; }
            set
            {
                if (_PositionError1 != value)
                {
                    _PositionError1 = value;
                    OnPropertyChanged(nameof(PositionError1));
                }
            }
        }

        /// <summary>
        /// 是否在运动
        /// </summary>
        private bool _IsMoving1 = false;
        public bool IsMoving1
        {
            get { return _IsMoving1; }
            set
            {
                if (_IsMoving1 != value)
                {
                    _IsMoving1 = value;
                    OnPropertyChanged(nameof(IsMoving1));
                }
            }
        }

        /// <summary>
        /// 是否在加速/减速
        /// </summary>
        private bool _IsAcceleration1 = false;
        public bool IsAcceleration1
        {
            get { return _IsAcceleration1; }
            set
            {
                if (_IsAcceleration1 != value)
                {
                    _IsAcceleration1 = value;
                    OnPropertyChanged(nameof(IsAcceleration1));
                }
            }
        }

        /// <summary>
        /// 是否到位
        /// </summary>
        private bool _IsInPosition1 = false;
        public bool IsInPosition1
        {
            get { return _IsInPosition1; }
            set
            {
                if (_IsInPosition1 != value)
                {
                    _IsInPosition1 = value;
                    OnPropertyChanged(nameof(IsInPosition1));
                }
            }
        }

        /// <summary>
        /// 是否使能
        /// </summary>
        private bool _IsEnable1 = false;
        public bool IsEnable1
        {
            get { return _IsEnable1; }
            set
            {
                if (_IsEnable1 != value)
                {
                    _IsEnable1 = value;
                    UpdateIsAllAxisEnable();
                    OnPropertyChanged(nameof(IsEnable1));
                }
            }
        }

        /// <summary>
        /// 1轴左限位
        /// </summary>
        private bool _IsLeftLimit1 = true;
        public bool IsLeftLimit1
        {
            get { return _IsLeftLimit1; }
            set
            {
                if (_IsLeftLimit1 != value)
                {
                    _IsLeftLimit1 = value;
                    OnPropertyChanged(nameof(IsLeftLimit1));
                }
            }
        }

        /// <summary>
        /// 1轴右限位
        /// </summary>
        private bool _IsRightLimit1 = true;
        public bool IsRightLimit1
        {
            get { return _IsRightLimit1; }
            set
            {
                if (_IsRightLimit1 != value)
                {
                    _IsRightLimit1 = value;
                    OnPropertyChanged(nameof(IsRightLimit1));
                }
            }
        }
        #endregion

        #region Axis 2 参数及状态
        /// <summary>
        /// 速度
        /// </summary>
        private double _Velocity2 = 0;
        public double Velocity2
        {
            get { return _Velocity2; }
            set
            {
                if (_Velocity2 != value)
                {
                    _Velocity2 = value;
                    OnPropertyChanged(nameof(Velocity2));
                }
            }
        }

        /// <summary>
        /// 加速度
        /// </summary>
        private double _Acceleration2 = 0;
        public double Acceleration2
        {
            get { return _Acceleration2; }
            set
            {
                if (_Acceleration2 != value)
                {
                    _Acceleration2 = value;
                    OnPropertyChanged(nameof(Acceleration2));
                }
            }
        }

        /// <summary>
        /// 减速度
        /// </summary>
        private double _Deceleration2 = 0;
        public double Deceleration2
        {
            get { return _Deceleration2; }
            set
            {
                if (_Deceleration2 != value)
                {
                    _Deceleration2 = value;
                    OnPropertyChanged(nameof(Deceleration2));
                }
            }
        }

        /// <summary>
        /// 加速度的加速度
        /// </summary>
        private double _KillDeceleration2 = 0;
        public double KillDeceleration2
        {
            get { return _KillDeceleration2; }
            set
            {
                if (_KillDeceleration2 != value)
                {
                    _KillDeceleration2 = value;
                    OnPropertyChanged(nameof(KillDeceleration2));
                }
            }
        }

        /// <summary>
        /// 减速度的减速度
        /// </summary>
        private double _Jerk2 = 0;
        public double Jerk2
        {
            get { return _Jerk2; }
            set
            {
                if (_Jerk2 != value)
                {
                    _Jerk2 = value;
                    OnPropertyChanged(nameof(Jerk2));
                }
            }
        }

        /// <summary>
        /// 参考位置
        /// </summary>
        private double _ReferencePosition2 = 0;
        public double ReferencePosition2
        {
            get { return _ReferencePosition2; }
            set
            {
                if (_ReferencePosition2 != value)
                {
                    _ReferencePosition2 = value;
                    OnPropertyChanged(nameof(ReferencePosition2));
                }
            }
        }

        /// <summary>
        /// 反馈位置
        /// </summary>
        private double _FeedbackPosition2 = 0;
        public double FeedbackPosition2
        {
            get { return _FeedbackPosition2; }
            set
            {
                if (_FeedbackPosition2 != value)
                {
                    _FeedbackPosition2 = value;
                    OnPropertyChanged(nameof(FeedbackPosition2));
                }
            }
        }

        /// <summary>
        /// 当前速度
        /// </summary>
        private double _ActualVelocity2 = 0;
        public double ActualVelocity2
        {
            get { return _ActualVelocity2; }
            set
            {
                if (_ActualVelocity2 != value)
                {
                    _ActualVelocity2 = value;
                    OnPropertyChanged(nameof(ActualVelocity2));
                }
            }
        }

        /// <summary>
        /// 位置偏差
        /// </summary>
        private double _PositionError2 = 0;
        public double PositionError2
        {
            get { return _PositionError2; }
            set
            {
                if (_PositionError2 != value)
                {
                    _PositionError2 = value;
                    OnPropertyChanged(nameof(PositionError2));
                }
            }
        }

        /// <summary>
        /// 是否在运动
        /// </summary>
        private bool _IsMoving2 = false;
        public bool IsMoving2
        {
            get { return _IsMoving2; }
            set
            {
                if (_IsMoving2 != value)
                {
                    _IsMoving2 = value;
                    OnPropertyChanged(nameof(IsMoving2));
                }
            }
        }

        /// <summary>
        /// 是否在加速/减速
        /// </summary>
        private bool _IsAcceleration2 = false;
        public bool IsAcceleration2
        {
            get { return _IsAcceleration2; }
            set
            {
                if (_IsAcceleration2 != value)
                {
                    _IsAcceleration2 = value;
                    OnPropertyChanged(nameof(IsAcceleration2));
                }
            }
        }

        /// <summary>
        /// 是否到位
        /// </summary>
        private bool _IsInPosition2 = false;
        public bool IsInPosition2
        {
            get { return _IsInPosition2; }
            set
            {
                if (_IsInPosition2 != value)
                {
                    _IsInPosition2 = value;
                    OnPropertyChanged(nameof(IsInPosition2));
                }
            }
        }

        /// <summary>
        /// 是否使能
        /// </summary>
        private bool _IsEnable2 = false;
        public bool IsEnable2
        {
            get { return _IsEnable2; }
            set
            {
                if (_IsEnable2 != value)
                {
                    _IsEnable2 = value;
                    UpdateIsAllAxisEnable();
                    OnPropertyChanged(nameof(IsEnable2));
                }
            }
        }

        /// <summary>
        /// 2轴左限位
        /// </summary>
        private bool _IsLeftLimit2 = true;
        public bool IsLeftLimit2
        {
            get { return _IsLeftLimit2; }
            set
            {
                if (_IsLeftLimit2 != value)
                {
                    _IsLeftLimit2 = value;
                    OnPropertyChanged(nameof(IsLeftLimit2));
                }
            }
        }

        /// <summary>
        /// 2轴右限位
        /// </summary>
        private bool _IsRightLimit2 = true;
        public bool IsRightLimit2
        {
            get { return _IsRightLimit2; }
            set
            {
                if (_IsRightLimit2 != value)
                {
                    _IsRightLimit2 = value;
                    OnPropertyChanged(nameof(IsRightLimit2));
                }
            }
        }
        #endregion

        #region PEIe-1730 IO

        #region input

        private bool _Emergency = true;
        public bool Emergency
        {
            get { return _Emergency; }
            set
            {
                if (_Emergency != value)
                {
                    _Emergency = value;
                    OnPropertyChanged(nameof(Emergency));
                }
            }
        }

        private bool _SafetyDoor = true;
        public bool SafetyDoor
        {
            get { return _SafetyDoor; }
            set
            {
                if (_SafetyDoor != value)
                {
                    _SafetyDoor = value;
                    OnPropertyChanged(nameof(SafetyDoor));
                }
            }
        }

        private bool _StartButton = true;
        public bool StartButton
        {
            get { return _StartButton; }
            set
            {
                if (_StartButton != value)
                {
                    _StartButton = value;
                    OnPropertyChanged(nameof(StartButton));
                }
            }
        }

        private bool _StopButton = true;
        public bool StopButton
        {
            get { return _StopButton; }
            set
            {
                if (_StopButton != value)
                {
                    _StopButton = value;
                    OnPropertyChanged(nameof(StopButton));
                }
            }
        }

        private bool _ResetButton = true;
        public bool ResetButton
        {
            get { return _ResetButton; }
            set
            {
                if (_ResetButton != value)
                {
                    _ResetButton = value;
                    OnPropertyChanged(nameof(ResetButton));
                }
            }
        }

        private bool _LaserReady = true;
        public bool LaserReady
        {
            get { return _LaserReady; }
            set
            {
                if (_LaserReady != value)
                {
                    _LaserReady = value;
                    OnPropertyChanged(nameof(LaserReady));
                }
            }
        }

        private bool _ClampingCylOpen = true;
        public bool ClampingCylOpen
        {
            get { return _ClampingCylOpen; }
            set
            {
                if (_ClampingCylOpen != value)
                {
                    _ClampingCylOpen = value;
                    OnPropertyChanged(nameof(ClampingCylOpen));
                }
            }
        }

        private bool _ClampingCylClose = true;
        public bool ClampingCylClose
        {
            get { return _ClampingCylClose; }
            set
            {
                if (_ClampingCylClose != value)
                {
                    _ClampingCylClose = value;
                    OnPropertyChanged(nameof(ClampingCylClose));
                }
            }
        }

        private bool _SlidingCylOpen = true;
        public bool SlidingCylOpen
        {
            get { return _SlidingCylOpen; }
            set
            {
                if (_SlidingCylOpen != value)
                {
                    _SlidingCylOpen = value;
                    OnPropertyChanged(nameof(SlidingCylOpen));
                }
            }
        }

        private bool _SlidingCylClose = true;
        public bool SlidingCylClose
        {
            get { return _SlidingCylClose; }
            set
            {
                if (_SlidingCylClose != value)
                {
                    _SlidingCylClose = value;
                    OnPropertyChanged(nameof(SlidingCylClose));
                }
            }
        }

        private bool _StopperCyl1Open = true;
        public bool StopperCyl1Open
        {
            get { return _StopperCyl1Open; }
            set
            {
                if (_StopperCyl1Open != value)
                {
                    _StopperCyl1Open = value;
                    OnPropertyChanged(nameof(StopperCyl1Open));
                }
            }
        }

        private bool _StopperCyl1Close = true;
        public bool StopperCyl1Close
        {
            get { return _StopperCyl1Close; }
            set
            {
                if (_StopperCyl1Close != value)
                {
                    _StopperCyl1Close = value;
                    OnPropertyChanged(nameof(StopperCyl1Close));
                }
            }
        }

        private bool _StopperCyl2Open = true;
        public bool StopperCyl2Open
        {
            get { return _StopperCyl2Open; }
            set
            {
                if (_StopperCyl2Open != value)
                {
                    _StopperCyl2Open = value;
                    OnPropertyChanged(nameof(StopperCyl2Open));
                }
            }
        }

        private bool _StopperCyl2Close = true;
        public bool StopperCyl2Close
        {
            get { return _StopperCyl2Close; }
            set
            {
                if (_StopperCyl2Close != value)
                {
                    _StopperCyl2Close = value;
                    OnPropertyChanged(nameof(StopperCyl2Close));
                }
            }
        }

        private bool _StopperCyl3Open = true;
        public bool StopperCyl3Open
        {
            get { return _StopperCyl3Open; }
            set
            {
                if (_StopperCyl3Open != value)
                {
                    _StopperCyl3Open = value;
                    OnPropertyChanged(nameof(StopperCyl3Open));
                }
            }
        }

        private bool _StopperCyl3Close = true;
        public bool StopperCyl3Close
        {
            get { return _StopperCyl3Close; }
            set
            {
                if (_StopperCyl3Close != value)
                {
                    _StopperCyl3Close = value;
                    OnPropertyChanged(nameof(StopperCyl3Close));
                }
            }
        }

        #endregion

        #region output

        private bool _GATE = false;
        public bool GATE
        {
            get { return _GATE; }
            set
            {
                if (_GATE != value)
                {
                    _GATE = value;
                    OnPropertyChanged(nameof(GATE));
                }
            }
        }

        private bool _ClampingCylFwd = false;
        public bool ClampingCylFwd
        {
            get { return _ClampingCylFwd; }
            set
            {
                if (_ClampingCylFwd != value)
                {
                    _ClampingCylFwd = value;
                    OnPropertyChanged(nameof(ClampingCylFwd));
                }
            }
        }

        private bool _SlidingCylFwd = false;
        public bool SlidingCylFwd
        {
            get { return _SlidingCylFwd; }
            set
            {
                if (_SlidingCylFwd != value)
                {
                    _SlidingCylFwd = value;
                    OnPropertyChanged(nameof(SlidingCylFwd));
                }
            }
        }

        private bool _SlidingCylBwd = false;
        public bool SlidingCylBwd
        {
            get { return _SlidingCylBwd; }
            set
            {
                if (_SlidingCylBwd != value)
                {
                    _SlidingCylBwd = value;
                    OnPropertyChanged(nameof(SlidingCylBwd));
                }
            }
        }

        private bool _StopperCyl1Fwd = false;
        public bool StopperCyl1Fwd
        {
            get { return _StopperCyl1Fwd; }
            set
            {
                if (_StopperCyl1Fwd != value)
                {
                    _StopperCyl1Fwd = value;
                    OnPropertyChanged(nameof(StopperCyl1Fwd));
                }
            }
        }

        private bool _StopperCyl23Fwd = false;
        public bool StopperCyl23Fwd
        {
            get { return _StopperCyl23Fwd; }
            set
            {
                if (_StopperCyl23Fwd != value)
                {
                    _StopperCyl23Fwd = value;
                    OnPropertyChanged(nameof(StopperCyl23Fwd));
                }
            }
        }

        #endregion

        #endregion

        #region PCIe-1760 IO

        #region input

        private bool _CDAPressureSwitch = true;
        public bool CDAPressureSwitch
        {
            get { return _CDAPressureSwitch; }
            set
            {
                if (_CDAPressureSwitch != value)
                {
                    _CDAPressureSwitch = value;
                    OnPropertyChanged(nameof(CDAPressureSwitch));
                }
            }
        }

        private bool _ClampAirPresSwitch = true;
        public bool ClampAirPresSwitch
        {
            get { return _ClampAirPresSwitch; }
            set
            {
                if (_ClampAirPresSwitch != value)
                {
                    _ClampAirPresSwitch = value;
                    OnPropertyChanged(nameof(ClampAirPresSwitch));
                }
            }
        }

        private bool _PositionInput1 = true;
        public bool PositionInput1
        {
            get { return _PositionInput1; }
            set
            {
                if (_PositionInput1 != value)
                {
                    _PositionInput1 = value;
                    OnPropertyChanged(nameof(PositionInput1));
                }
            }
        }

        private bool _PositionInput2 = true;
        public bool PositionInput2
        {
            get { return _PositionInput2; }
            set
            {
                if (_PositionInput2 != value)
                {
                    _PositionInput2 = value;
                    OnPropertyChanged(nameof(PositionInput2));
                }
            }
        }

        private bool _PositionInput3 = true;
        public bool PositionInput3
        {
            get { return _PositionInput3; }
            set
            {
                if (_PositionInput3 != value)
                {
                    _PositionInput3 = value;
                    OnPropertyChanged(nameof(PositionInput3));
                }
            }
        }

        #endregion

        #region output

        private bool _Tower_G = false;
        public bool Tower_G
        {
            get { return _Tower_G; }
            set
            {
                if (_Tower_G != value)
                {
                    _Tower_G = value;
                    OnPropertyChanged(nameof(Tower_G));
                }
            }
        }

        private bool _Tower_R = false;
        public bool Tower_R
        {
            get { return _Tower_R; }
            set
            {
                if (_Tower_R != value)
                {
                    _Tower_R = value;
                    OnPropertyChanged(nameof(Tower_R));
                }
            }
        }

        private bool _Tower_A = false;
        public bool Tower_A
        {
            get { return _Tower_A; }
            set
            {
                if (_Tower_A != value)
                {
                    _Tower_A = value;
                    OnPropertyChanged(nameof(Tower_A));
                }
            }
        }

        private bool _TowerBuzzer = false;
        public bool TowerBuzzer
        {
            get { return _TowerBuzzer; }
            set
            {
                if (_TowerBuzzer != value)
                {
                    _TowerBuzzer = value;
                    OnPropertyChanged(nameof(TowerBuzzer));
                }
            }
        }

        private bool _LightController = false;
        public bool LightController
        {
            get { return _LightController; }
            set
            {
                if (_LightController != value)
                {
                    _LightController = value;
                    OnPropertyChanged(nameof(LightController));
                }
            }
        }

        private bool _PowerSupDischarge = false;
        public bool PowerSupDischarge
        {
            get { return _PowerSupDischarge; }
            set
            {
                if (_PowerSupDischarge != value)
                {
                    _PowerSupDischarge = value;
                    OnPropertyChanged(nameof(PowerSupDischarge));
                }
            }
        }

        #endregion

        #endregion

        #region 激光参数

        private double _PulseFrequency = 0;
        public double PulseFrequency
        {
            get { return _PulseFrequency; }
            set
            {
                if (_PulseFrequency != value)
                {
                    _PulseFrequency = value;
                    OnPropertyChanged(nameof(PulseFrequency));
                }
            }
        }

        private double _PulseWidth = 0;
        public double PulseWidth
        {
            get { return _PulseWidth; }
            set
            {
                if (_PulseWidth != value)
                {
                    _PulseWidth = value;
                    OnPropertyChanged(nameof(PulseWidth));
                }
            }
        }

        private double _DiodeCurrent = 0;
        public double DiodeCurrent
        {
            get { return _DiodeCurrent; }
            set
            {
                if (_DiodeCurrent != value)
                {
                    _DiodeCurrent = value;
                    OnPropertyChanged(nameof(DiodeCurrent));
                }
            }
        }
        #endregion

        #region 画图参数

        /// <summary>
        /// 是否正在画图
        /// </summary>
        private bool _IsDrawing = false;
        public bool IsDrawing
        {
            get { return _IsDrawing; }
            set
            {
                if (_IsDrawing != value)
                {
                    _IsDrawing = value;
                    OnPropertyChanged(nameof(IsDrawing));
                }
            }
        }
        #endregion
    }
}
