using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using IOControllers;
using Utilities;

namespace ControlSystems
{
    public partial class OperationControl
    {
        const int EV_STOP = 1;
        const int EV_UPDATE_PARAMETER = 2;
        const int EV_START_TAG_METER = 3;
        const int EV_START_TAG_PROGRAMMER = 4;
        const int EV_START_REGISTER_GOOD = 5;
        const int EV_START_REGISTER_BAD = 6;
        const int EV_BOX_FINISH = 7;
        const int EV_MESSAGE = 8;
        const int EV_START = 9;
        const int EV_STOP_TAG_METER = 10;

        bool bGoodPartDroped = false;
        bool bRejectPartDroped = false;
        bool bTagProgrammerRunning;
        bool bTagMeterRunning;

        #region <-+- Private Methode -+->
        protected override void Monitor()
        {
            if (IsGoodPartSensorON())
            {
                bGoodPartDroped = true;
            }

            if (IsRejectPartSensorON())
            {
                bRejectPartDroped = true;
            }

            if (!IsElectricityON())
            {
                if (iStatus != SYSTEM_NO_POWER)
                {
                    Raise(EV_STOP);
                }
                iStatus = SYSTEM_NO_POWER;
                bAlarm = true;
            }
            else if (IsEmergencyStop())
            {
                if (iStatus != SYSTEM_ESTOP)
                {
                    Raise(EV_STOP);
                }
                iStatus = SYSTEM_ESTOP;
                bAlarm = true;
            }
            else if (!IsPneumaticPressureSwitchON())
            {
                if (iStatus != SYSTEM_NO_PNEUMATIC_AIR)
                {
                    Raise(EV_STOP);
                }
                iStatus = SYSTEM_NO_PNEUMATIC_AIR;
                bAlarm = true;
            }
            else if (IsLinearActuatorAlarm())
            {
                if (iStatus != SYSTEM_LINEAR_ACTUATOR_ALARM)
                {
                    Raise(EV_STOP);
                }
                iStatus = SYSTEM_LINEAR_ACTUATOR_ALARM;
                bAlarm = true;
            }
            else
            {
                bAlarm = false;
            }

            if(bAlarm)
            {
                bCycleRun = false;
            }
        }

        protected override void CycleRun()
        {
            iStatus = SYSTEM_RUNNING;
            ControlTask();
        }

        protected override void CycleStop()
        {
            if (bAlarm)
            {
                OnRedLamp();
                if (bAlarmReset)
                {
                    if (IsLinearActuatorAlarm())
                    {
                        LinearActuatorReset(true);
                    }
                }
            }
            else
            {
                if (bAlarmReset)
                {
                    LinearActuatorReset(false);
                    bAlarmReset = false;
                }

                if (iStatus != SYSTEM_STOP)
                {
                    Raise(EV_STOP);
                }
                iStatus = SYSTEM_STOP;
            }
        }

        #endregion

        #region <-+- IO Constance -+->
        const bool ON = true;
        const bool OFF = false;

        const int DI_EMERGENCY_STOP = 0;
        const int DI_ELECTRICITY_ON = 1;
        const int DI_PNEUMATIC_PRESSURE_SWITCH = 2;
        const int DI_CYLINDER_PART_ENTRY_CLOSE = 3;
        const int DI_CYLINDER_PART_ENTRY_OPEN = 4 ;
        const int DI_CYLINDER_PART_RELEASE_CLOSE = 5;
        const int DI_CYLINDER_PART_RELEASE_OPEN = 6;
        const int DI_CYLINDER_TAG_METER_UP = 7;
        const int DI_CYLINDER_TAG_METER_DOWN = 8;
        const int DI_CYLINDER_TAG_PROGRAMMER_FONT = 9;
        const int DI_CYLINDER_TAG_PROGRAMMER_BACK = 10;
        const int DI_CYLINDER_LINEAR_FONT = 11;
        const int DI_CYLINDER_LINEAR_BACK = 12;
        const int DI_CYLINDER_SHUTTLE_CLOSE = 13;
        const int DI_CYLINDER_SHUTTLE_OPEN = 14;        
        const int DI_GOOD_PART_SENSOR = 15;
        const int DI_PART_ENTRY_SENSOR_POSITION = 16;
        const int DI_PART_RELEASE_SENSOR_POSITION = 17;
        const int DI_PART_IN_TAG_METER = 18;
        const int DI_PART_IN_TAG_PROGRAMMER = 19;        
        const int DI_REJECT_PART_SENSOR = 20;
        const int DI_LINEAR_ACTUATOR_HEND = 21;
        const int DI_LINEAR_ACTUATOR_PEND = 22;
        const int DI_LINEAR_ACTUATOR_SV = 23;
        const int DI_LINEAR_ACTUATOR_ALM = 24;
        const int DI_LINEAR_ACTUATOR_RMDS = 25;
        const int DI_LINEAR_ACTUATOR_EMGS = 26;
        const int DI_LINEAR_ACTUATOR_MOVE = 27;
        const int DI_LINEAR_ACTUATOR_PM1 = 28;
        const int DI_LINEAR_ACTUATOR_PM2 = 29;
        const int DI_LINEAR_ACTUATOR_PM4 = 30;

        const int DO_PART_FEEDER = 0;
        const int DO_PART_ENTRY_CYLINDER_SOLENOID_VALVE = 1;
        const int DO_PART_RELEASE_CYLINDER_SOLENOID_VALVE = 2;
        const int DO_TAG_METER_DOWN_SOLENOID_VALVE = 3;
        const int DO_TAG_METER_UP_SOLENOID_VALVE = 4;
        const int DO_LINEAR_PART_FRONT = 5;
        const int DO_LINEAR_PART_BACK = 6;
        const int DO_TAG_PROGRAMMER_SOLENOID_VALVE = 7;
        const int DO_SHUTTLE_GATE_SOLENOID_VALVE = 8;
        const int DO_TOWER_LAMP_RED = 9;
        const int DO_TOWER_LAMP_YELLOW = 10;
        const int DO_TOWER_LAMP_GREEN = 11;
        const int DO_TOWER_LAMP_BUZZER1 = 12;
        const int DO_TOWER_LAMP_BUZZER2 = 13;
        const int DO_LINEAR_ACTUATOR_PC1 = 14;
        const int DO_LINEAR_ACTUATOR_PC2 = 15;
        const int DO_LINEAR_ACTUATOR_PC4 = 16;
        const int DO_LINEAR_ACTUATOR_BKRL = 17;
        const int DO_LINEAR_ACTUATOR_PMOD = 18;
        const int DO_LINEAR_ACTUATOR_HOME = 19;
        const int DO_LINEAR_ACTUATOR_CSTR = 20;
        const int DO_LINEAR_ACTUATOR_RES = 21;
        const int DO_LINEAR_ACTUATOR_SON = 22;
        const int DO_LINEAR_ACTUATOR_STP = 23;

        #endregion

        #region <-+- IO Control Methode -+->

        public bool IsElectricityON()
        {
            return GetDigitalInput(DI_ELECTRICITY_ON);
        }

        public bool IsEmergencyStop()
        {
            return !GetDigitalInput(DI_EMERGENCY_STOP);
        }

        public bool IsPneumaticPressureSwitchON()
        {
            return GetDigitalInput(DI_PNEUMATIC_PRESSURE_SWITCH);
        }

        public bool IsCylinderPartEntryClose()
        {
            return GetDigitalInput(DI_CYLINDER_PART_ENTRY_CLOSE);
        }

        public bool IsCylinderPartEntryOpen()
        {
            return GetDigitalInput(DI_CYLINDER_PART_ENTRY_OPEN);
        }

        public bool IsCylinderPartReleaseClose()
        {
            return GetDigitalInput(DI_CYLINDER_PART_RELEASE_CLOSE);
        }

        public bool IsCylinderPartReleaseOpen()
        {
            return GetDigitalInput(DI_CYLINDER_PART_RELEASE_OPEN);
        }

        public bool IsCylinderTagMeterUp()
        {
            return GetDigitalInput(DI_CYLINDER_TAG_METER_UP);
        }

        public bool IsCylinderTagMeterDown()
        {
            return GetDigitalInput(DI_CYLINDER_TAG_METER_DOWN);
        }

        public bool IsCylinderInTagMeter()
        {
            return GetDigitalInput(DI_CYLINDER_LINEAR_BACK);
        }

        public bool IsCylinderInTagProgrammer()
        {
            return GetDigitalInput(DI_CYLINDER_LINEAR_FONT);
        }

        public bool IsCylinderTagProgrammerMoveOut()
        {
            return GetDigitalInput(DI_CYLINDER_TAG_PROGRAMMER_FONT);
        }

        public bool IsCylinderTagProgrammerMoveBack()
        {
            return GetDigitalInput(DI_CYLINDER_TAG_PROGRAMMER_BACK);
        }

        public bool IsCylinderShuttleClose()
        {
            return GetDigitalInput(DI_CYLINDER_SHUTTLE_CLOSE);
        }

        public bool IsCylinderShuttleOpen()
        {
            return GetDigitalInput(DI_CYLINDER_SHUTTLE_OPEN);
        }

        public bool IsGoodPartSensorON()
        {
            return true;// GetDigitalInput(DI_GOOD_PART_SENSOR);
        }

        public bool IsPartFeeding()
        {
            return GetDigitalInput(DI_PART_ENTRY_SENSOR_POSITION);
        }

        public bool IsPartFeederRunning()
        {
            return GetDigitalOutput(DO_PART_FEEDER);
        }

        public bool IsPartEntry()
        {
            return GetDigitalInput(DI_PART_RELEASE_SENSOR_POSITION);
        }

        public bool IsPartInTagMeter()
        {
            return GetDigitalInput(DI_PART_IN_TAG_METER);
        }

        public bool IsPartInTagProgrammer()
        {
            return GetDigitalInput(DI_PART_IN_TAG_PROGRAMMER);
        }

        public bool IsRejectPartSensorON()
        {
            return true;// GetDigitalInput(DI_REJECT_PART_SENSOR);
        }

        public bool IsLinearActuatorHomeEnd()
        {
            // This signal is OFF immediately after the power is input, and turns ON
            // when home return has completed.
            return GetDigitalInput(DI_LINEAR_ACTUATOR_HEND);
        }

        public bool IsLinearActuatorPositionComplete()
        {
            // This signal turns ON when the target position was reached and
            // the actuator has entered the specified in-position range.
            // It is used to determine whether positioning has completed.
            return GetDigitalInput(DI_LINEAR_ACTUATOR_PEND);
        }

        public bool IsLinearActuatorServoReader()
        {
            // This signal is always output once the servo is turned ON and 
            // the controller is ready to operate.
            return GetDigitalInput(DI_LINEAR_ACTUATOR_SV);
        }
        public bool IsLinearActuatorAlarm()
        {
            // This signal remains ON in normal conditions of use and turns OFF
            // when an alarm generates.
            return !GetDigitalInput(DI_LINEAR_ACTUATOR_ALM);
        }

        public bool IsLinearActuatorOperatingModeStatus()
        {
            // This signal will remain OFF during the AUTO mode, and ON during the MANU mode.
            return GetDigitalInput(DI_LINEAR_ACTUATOR_RMDS);
        }

        public bool IsLinearActuatorEmergencyStop()
        {
            //When this signal is OFF, it means that an emergency stop is being actuated.
            return !GetDigitalInput(DI_LINEAR_ACTUATOR_EMGS);
        }

        public bool IsLinearActuatorMoving()
        {
            // This signal will remain ON while the actuator is moving, and OFF
            // while the actuator is standing still.
            // It is used to determine whether the actuator is moving or paused.
            return GetDigitalInput(DI_LINEAR_ACTUATOR_MOVE);
        }

        public bool LinearActuatorSignalPM1()
        {
            // Command position number 1
            return GetDigitalInput(DI_LINEAR_ACTUATOR_PM1);
        }

        public bool LinearActuatorSignalPM2()
        {
            // Command position number 2
            return GetDigitalInput(DI_LINEAR_ACTUATOR_PM2);
        }

        public bool LinearActuatorSignalPM4()
        {
            // Command position number 4
            return GetDigitalInput(DI_LINEAR_ACTUATOR_PM4);
        }


        // Output control
        public void OnPartFeeder()
        {
            SetDigitalOuput(DO_PART_FEEDER, ON);
        }

        public void OffPartFeeder()
        {
            SetDigitalOuput(DO_PART_FEEDER, OFF);
        }

        public void OpenPartEntryCylinder()
        {
            SetDigitalOuput(DO_PART_ENTRY_CYLINDER_SOLENOID_VALVE, ON);
        }

        public void ClosePartEntryCylinder()
        {
            SetDigitalOuput(DO_PART_ENTRY_CYLINDER_SOLENOID_VALVE, OFF);
        }

        public void OpenPartReleaseCylinder()
        {
            SetDigitalOuput(DO_PART_RELEASE_CYLINDER_SOLENOID_VALVE, ON);
        }

        public void ClosePartReleaseCylinder()
        {
            SetDigitalOuput(DO_PART_RELEASE_CYLINDER_SOLENOID_VALVE, OFF);
        }

        public void MoveTagMeterCylinderDown()
        {
            SetDigitalOuput(DO_TAG_METER_UP_SOLENOID_VALVE, OFF);
            SetDigitalOuput(DO_TAG_METER_DOWN_SOLENOID_VALVE, ON);
        }

        public void MoveTagMeterCylinderUp()
        {
            SetDigitalOuput(DO_TAG_METER_DOWN_SOLENOID_VALVE, OFF);
            SetDigitalOuput(DO_TAG_METER_UP_SOLENOID_VALVE, ON);
        }

        public void MoveLinearCylinderToTagMeter()
        {
            SetDigitalOuput(DO_LINEAR_PART_FRONT, OFF);
            SetDigitalOuput(DO_LINEAR_PART_BACK, ON);
        }

        public void MoveLinearCylinderToTagProgrammer()
        {
            SetDigitalOuput(DO_LINEAR_PART_BACK, OFF);
            SetDigitalOuput(DO_LINEAR_PART_FRONT, ON);
        }

        public void OffPartReleaseCylinder()
        {
            SetDigitalOuput(DO_PART_RELEASE_CYLINDER_SOLENOID_VALVE, OFF);
        }


        public void OpenShuttleGate()
        {
            SetDigitalOuput(DO_SHUTTLE_GATE_SOLENOID_VALVE, ON);
        }

        public void CloseShuttleGate()
        {
            SetDigitalOuput(DO_SHUTTLE_GATE_SOLENOID_VALVE, OFF);
        }

        public void OnTagProgrammerSolenoidValve()
        {
            SetDigitalOuput(DO_TAG_PROGRAMMER_SOLENOID_VALVE, ON);
        }

        public void OffTagProgrammerSolenoidValve()
        {
            SetDigitalOuput(DO_TAG_PROGRAMMER_SOLENOID_VALVE, OFF);
        }

        public void OnGreenLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_GREEN, ON);
        }

        public void OffGreenLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_GREEN, OFF);
        }

        public void OnYellowLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_YELLOW, ON);
        }

        public void OffYellowLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_YELLOW, OFF);
        }

        public void OnRedLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_RED, ON);
        }

        public void OffRedLamp()
        {
            SetDigitalOuput(DO_TOWER_LAMP_RED, OFF);
        }

        public void OnBuzzer1()
        {
            SetDigitalOuput(DO_TOWER_LAMP_BUZZER1, ON);
        }

        public void OffBuzzer1()
        {
            SetDigitalOuput(DO_TOWER_LAMP_BUZZER1, OFF);
        }

        public void OnBuzzer2()
        {
            SetDigitalOuput(DO_TOWER_LAMP_BUZZER2, ON);
        }

        public void OffBuzzer2()
        {
            SetDigitalOuput(DO_TOWER_LAMP_BUZZER2, OFF);
        }

        public void LinearActuatorPC1(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_PC1, bState);
        }

        public void LinearActuatorPC2(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_PC2, bState);
        }

        public void LinearActuatorPC4(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_PC4, bState);
        }

        public void LinearActuatorBreakRelease(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_BKRL, bState);
        }

        public void LinearActuatorOperationMode(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_PMOD, bState);
        }

        public void LinearActuatorHome(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_HOME, bState);
        }

        public void LinearActuatorStart(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_CSTR, bState);
        }

        public void LinearActuatorReset(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_RES, bState);
        }

        public void LinearActuatorServo(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_SON, bState);
        }

        public void LinearActuatorPause(bool bState)
        {
            SetDigitalOuput(DO_LINEAR_ACTUATOR_STP, bState);
        }
        #endregion
    }
}