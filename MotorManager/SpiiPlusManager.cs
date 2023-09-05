using ACS.SPiiPlusNET;

namespace MotorManager
{
    public class SpiiPlusManager
    {
        private Api _ACS = new Api();
        public bool InitSpii = false;
        private const double Scale = 1000; //比例

        /// <summary>
        /// get and set the Velocity of specified axis
        /// </summary>
        public double GetVelocity(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetVelocity((Axis)axis);
            return res /= Scale;
        }
        public void SetVelocity(int axis, double velocity)
        {
            if (!InitSpii) { return; }
            _ACS.SetVelocity((Axis)axis, velocity *= Scale);
        }

        /// <summary>
        /// get and set the Acceleration of specified axis
        /// </summary>
        public double GetAcceleration(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetAcceleration((Axis)axis);
            return res /= Scale;
        }
        public void SetAcceleration(int axis, double acceleration)
        {
            if (!InitSpii) { return; }
            _ACS.SetAcceleration((Axis)axis, acceleration *= Scale);
        }

        /// <summary>
        /// get and set the Deceleration of specified axis
        /// </summary>
        public double GetDeceleration(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetDeceleration((Axis)axis);
            return res /= Scale;
        }
        public void SetDeceleration(int axis, double deceleration)
        {
            if (!InitSpii) { return; }
            _ACS.SetDeceleration((Axis)axis, deceleration *= Scale);
        }

        /// <summary>
        /// get and set the KillDeceleration of specified axis
        /// </summary>
        public double GetKillDeceleration(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetKillDeceleration((Axis)axis);
            return res /= Scale;
        }
        public void SetKillDeceleration(int axis, double killDeceleration)
        {
            if (!InitSpii) { return; }
            _ACS.SetKillDeceleration((Axis)axis, killDeceleration *= Scale);
        }

        /// <summary>
        /// get and set the Jerk of specified axis
        /// </summary>
        public double GetJerk(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetJerk((Axis)axis);
            return res /= Scale;
        }
        public void SetJerk(int axis, double jerk)
        {
            if (!InitSpii) { return; }
            _ACS.SetJerk((Axis)axis, jerk *= Scale);
        }

        /// <summary>
        /// get motor state
        /// </summary>
        public MotorStates GetMotorState(int axis)
        {
            if (!InitSpii) { return 0; }
            try
            {
                var status = _ACS.GetMotorState((Axis)axis);
                return status;
            }
            catch (Exception e)
            {
                return MotorStates.ACSC_NONE;
            }
        }

        /// <summary>
        /// program buffer state
        /// </summary>
        public ProgramStates GetProgramState(int bufferNumber)
        {
            if (!InitSpii) { return 0; }
            return _ACS.GetProgramState((ProgramBuffer)bufferNumber);
        }

        public object _ObjReadVar
        {
            get
            {
                return _ACS.ReadVariableAsVector("FAULT", ProgramBuffer.ACSC_NONE, 0, Convert.ToInt32(_ACS.Transaction("?SYSINFO(13)").Trim()) - 1, -1, -1);
            }
        }

        /// <summary>
        /// 返回参考位置
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public double GetReferencePosition(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetRPosition((Axis)axis);
            return res /= Scale;
        }

        /// <summary>
        /// 返回动态位置
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public double GetFeedbackPosition(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = _ACS.GetFPosition((Axis)axis);
            return res /= Scale;
        }

        /// <summary>
        /// 返回动态速度
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public double GetActualVelocity(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = (double)_ACS.ReadVariable("FVEL", ProgramBuffer.ACSC_NONE, axis, axis);
            return res /= Scale;
        }

        /// <summary>
        /// 返回位置偏差
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public double GetPositionError(int axis)
        {
            if (!InitSpii) { return 0; }
            double res = (double)_ACS.ReadVariable("PE", ProgramBuffer.ACSC_NONE, axis, axis);
            return res /= Scale;
        }

        /// <summary>
        /// connect to the spii, return the list of Axis and buffer
        /// </summary>
        /// <param name="ipAddress">the ip address</param>
        /// <param name="port">the port</param>
        /// <returns></returns>
        public (Axis[], int) ConnectedSpii(string ipAddress, int port)
        {
            try
            {
                _ACS.OpenCommEthernetTCP(ipAddress, port);
            }
            catch (Exception ex)
            {
                return (null, -1);
            }

            int nTotalAxis = Convert.ToInt32(_ACS.Transaction("?SYSINFO(13)").Trim());
            Axis[] arrAxis = new Axis[nTotalAxis + 1];
            for (int i = 0; i < nTotalAxis; i++)
            {
                arrAxis[i] = (Axis)i;
            }
            arrAxis[nTotalAxis] = Axis.ACSC_NONE;

            int nTotalBuffer = Convert.ToInt32(_ACS.Transaction("?SYSINFO(10)").Trim());

            return (arrAxis, nTotalBuffer);
        }

        /// <summary>
        /// the spii disconnected
        /// </summary>
        public void DisConnectedSpii()
        {
            if (_ACS.IsConnected)
            {
                _ACS.CloseComm();
            }
        }

        /// <summary>
        /// enable the specified Axis
        /// </summary>
        /// <param name="nAxisNumber"></param>
        public void EnableSpecifiedAxis(int axis)
        {
            if (!InitSpii) { return; }
            if (_ACS.IsConnected)
            {
                _ACS.Enable((Axis)axis);
            }
        }

        /// <summary>
        /// disable the specified Axis
        /// </summary>
        /// <param name="nAxisNumber"></param>
        public void DisableSpecifiedAxis(int axis)
        {
            if (!InitSpii) { return; }
            _ACS.Disable((Axis)axis);
        }

        /// <summary>
        /// disbale all the Axis
        /// </summary>
        public void DisableAllAxis()
        {
            if (!InitSpii) { return; }
            if (_ACS.IsConnected)
            {
                _ACS.DisableAll();
            }
        }

        /// <summary>
        /// set current position of axis as the zero position
        /// </summary>
        public void SetZeroPosition(int axis)
        {
            if (!InitSpii) { return; }
            _ACS.SetFPosition((Axis)axis, 0);
        }

        public bool IsAxisEnable(int axis)
        {
            if (!InitSpii) { return false; }
            return (GetMotorState(axis) & MotorStates.ACSC_MST_ENABLE) != 0;
        }

        /// <summary>
        /// stop the specified axis
        /// </summary>
        /// <param name="axis"></param>
        public void StopSpecifiedAxis(int axis)
        {
            if (!InitSpii) { return; }
            if (_ACS.IsConnected)
            {
                _ACS.Halt((Axis)axis);
            }
        }

        /// <summary>
        /// stop all axis
        /// </summary>
        public void StopAllAxis(Axis[] arrAxisList)
        {
            if (!InitSpii) { return; }
            if (_ACS.IsConnected)
            {
                _ACS.HaltM(arrAxisList);
            }
        }

        /// <summary>
        /// 运行buffer
        /// </summary>
        /// <param name="bufferNumber"></param>
        public void RunBuffer(int bufferNumber)
        {
            _ACS.RunBufferAsync((ProgramBuffer)bufferNumber, null);

            //_ACS.WaitProgramEnd((ProgramBuffer)bufferNumber, 240 * 1000);
            while (true)
            {
                if (bufferNumber < 3)
                {
                    if (!WaitBufEnd(bufferNumber))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// stop the program buffer of the specified axis
        /// </summary>
        public void StopBuffer(int bufferNumber)
        {
            if (!InitSpii) { return; }
            _ACS.StopBuffer((ProgramBuffer)bufferNumber);
        }

        /// <summary>
        /// jog motion
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="flag">if flag is true, positive direction, else negative direction</param>
        public void Jog(int axis, bool flag)
        {
            if (!InitSpii) { return; }
            if (flag)
            {
                _ACS.Jog(MotionFlags.ACSC_NONE, (Axis)axis, (double)GlobalDirection.ACSC_POSITIVE_DIRECTION);
            }
            else
            {
                _ACS.Jog(MotionFlags.ACSC_NONE, (Axis)axis, (double)GlobalDirection.ACSC_NEGATIVE_DIRECTION);
            }
        }

        /// <summary>
        /// 等轴运动完毕
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool WaitAxisEnd(int axis)
        {
            if (!InitSpii) { return false; }
            if ((GetMotorState(axis) & MotorStates.ACSC_MST_MOVE) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 等buffer运动完毕
        /// </summary>
        /// <param name="bufferNumber"></param>
        /// <returns></returns>
        private bool WaitBufEnd(int bufferNumber)
        {
            if (!InitSpii) { return false; }
            if ((GetProgramState(bufferNumber) & ProgramStates.ACSC_PST_RUN) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// move to specifed position(PTP)
        /// </summary>
        /// <param name="distance"></param>
        public void MoveToPosition(int axis, double distance)
        {
            if (!InitSpii) { return; }
            //_ACS.ToPoint(0, (Axis)axis, distance*=Scale);
            _ACS.ToPointAsync(0, (Axis)axis, distance *= Scale);
        }
    }
}