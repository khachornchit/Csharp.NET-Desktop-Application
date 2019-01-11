using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using IOControllers;
using Utilities;

namespace ControlSystems
{
    public partial class OperationControl : Controls
    {
        #region <-+- Public Constance -+->
        public const int SYSTEM_IDLE = 1;
        public const int SYSTEM_STOP = 2;
        public const int SYSTEM_ESTOP = 3;
        public const int SYSTEM_NO_POWER = 4;
        public const int SYSTEM_NO_PNEUMATIC_AIR = 5;
        public const int SYSTEM_LINEAR_ACTUATOR_ALARM = 6;
        public const int SYSTEM_RUNNING = 255;        
        #endregion

        #region <-+- Private Constance -+->
        #endregion

        #region <-+- Public Object -+->
        #endregion

        #region <-+- Protected Object -+->
        #endregion

        #region <-+- Private Object -+->
        #endregion

        #region <-+- Private Variable -+->
        bool bCycleStop = false;
        bool bAlarm = false;
        int iStatus = 0;
        int iCycleStep = 0;
        int iMessageType;
        bool bPause = false;
        bool bAlarmReset;
        string sMessage;
        #endregion

        #region <-+- Constructor -+->
        public OperationControl(BackgroundWorker bgwTask)
            : base (bgwTask)
        {
            iStatus = SYSTEM_IDLE;
        }
        #endregion

        #region <-+- Dispose -+->
        private bool bDisposed = false;
        public override void Dispose()
        {
            base.Dispose();
            Dispose(true);
        }

        protected override void Dispose(bool bDisposing)
        {
            base.Dispose(bDisposing);
            if (!this.bDisposed)
            {
                if (bDisposing)
                {
                }
                this.bDisposed = true;
            }
        }

        public bool IsDisposed
        {
            get { return bDisposed; }
        }
        ~OperationControl()
        {
            Dispose();
        }
        #endregion

        #region <-+- Public Methode -+->

        public void Start()
        {
            bCycleStop = false;
            bTagProgrammerRunning = false;
            TagMeterRunning = false;
            if (bPause)
            {
                OnPartFeeder();
                bPause = false;
                iCycleStep = CYC_RUN_RUNNING;
            }
            else
            {
                iCycleStep = CYC_RUN_START;
                bCycleRun = true;
            }

            Raise(EV_START);
        }

        public void Stop()
        {
            if (bBoxFinished)
            {
                bCycleRun = false;
            }
            bCycleStop = true;
            bPause = false;
        }

        public void Reset()
        {
            iCycleStep = 0;
            iPartInGoodBox = 0;
            for (int iPos = 0; iPos < aiPartInRejectBox.Length; iPos++)
            {
                aiPartInRejectBox[iPos] = 0;
            }
            bBoxFinished = bWorkOrderFinished = false;
            bPause = false;
            bCycleStop = false;
            bCycleRun = false;
        }

        public void AlarmReset()
        {
            bAlarmReset = true;
        }

        public void Pause()
        {
            bPause = !bPause;
        }

        #endregion
        
        #region <-+- Public Accessor -+->
        public string ErrorMessage
        {
            get { return sErrorMessage; }
        }

        public bool Alarm
        {
            get { return bAlarm; }
        }

        public bool Paused
        {
            get { return bPause; }
        }

        public bool TagProgrammerRunning
        {
            set { bTagProgrammerRunning = value; }
        }

        public bool TagMeterRunning
        {
            set { bTagMeterRunning = value; }
        }

        public int Status
        {
            get { return iStatus; }
        }

        public int MessageType
        {
            get { return iMessageType; }
        }

        public string Message
        {
            get { return sMessage; }
        }
        #endregion
    }
}
