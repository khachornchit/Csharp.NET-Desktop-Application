using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using IOControllers;
using Utilities;

namespace ControlSystems
{
    public partial class OperationControl
    {
        const int CYC_RUN_STOP = 0;
        const int CYC_RUN_START = 1;
        const int CYC_RUN_INITIAL = 2;
        const int CYC_RUN_RUNNING = 3;
        const int CYC_RUN_BOX_FINISH = 4;
        const int CYC_RUN_WORK_FINISH = 5;
        const int PART_EMPTY_DELATY_TIME = 30;

        public const int MESSAGE_TYPE_UPDATE_PARAMETER = 0;
        public const int MESSAGE_TYPE_INFORMATION = 1;
        public const int MESSAGE_TYPE_WARNING = 2;
        public const int MESSAGE_TYPE_ERROR = 3;

        Stopwatch swPartEmpty = new Stopwatch();

        int iPartInGoodBox = 0;
        int[] aiPartInRejectBox = new int[4];
        int iQuantityWorked;
        int iQuantityPerBox;
        int iWorkOrderTarget;
        bool bBoxFinished;
        bool bWorkOrderFinished;
        bool bRegisterFinish;
        bool bRegisterSuccess;
        bool bTransferFinish;

        bool bEnRegister;
        bool bEnTagMeter;
        bool bEnTagProgrammer;
        bool bSkipTagMeter;
        bool bPartEmpty;

        private void ControlTask()
        {
            switch (iCycleStep)
            {
                case CYC_RUN_STOP:
                    ClearDigitalOuput();
                    break;

                case CYC_RUN_START:
                    OnGreenLamp();
                    bPartEmpty = false;
                    bPause = false;
                    swPartEmpty.Reset();
                    Raise(EV_UPDATE_PARAMETER);
                    iCycleStep = CYC_RUN_INITIAL;
                    iInitialStationStep = CYC_INITIAL_STATION_START;
                    break;

                case CYC_RUN_INITIAL:
                    InitialStation();
                    if(bCycleStop)
                    {
                        bCycleStop = false;
                        bCycleRun = false;
                    }
                    break;

                case CYC_RUN_RUNNING:
                    PartEntryTask();
                    PartReleaseTask();
                    if (bEnTagMeter)
                    {
                        TagMeterTask();
                    }
                    if (bEnTagProgrammer)
                    {
                        TagProgrammerTask();
                    }
                    LinearActuatorTask();
                    if (!IsPartEntry())
                    {
                        swPartEmpty.Start();
                        if (swPartEmpty.Elapsed.Seconds >= PART_EMPTY_DELATY_TIME)
                        {
                            if (!bPartEmpty)
                            {
                                bPartEmpty = true;
                                OnBuzzer2();
                                OnYellowLamp();
                                Raise(EV_UPDATE_PARAMETER);
                            }
                        }
                    }
                    else
                    {
                        if (bPartEmpty)
                        {
                            bPartEmpty = false;
                            OffBuzzer2();
                            OffYellowLamp();
                            Raise(EV_UPDATE_PARAMETER);
                        }
                        swPartEmpty.Reset();
                    }

                    if (bCycleStop)
                    {
                        if (!bTagMeterBusy && !bTagProgrammerBusy && bLinearActuatorReady)
                        {
                            bCycleStop = false;
                            bCycleRun = false;
                        }
                    }
                    else
                    {
                        if(bPause)
                        {
                            OffPartFeeder();
                        }
                        else
                        {
                            OnPartFeeder();
                        }
                    }
                    break;

                case CYC_RUN_BOX_FINISH:
                    OffPartFeeder();
                    OffGreenLamp();
                    OnYellowLamp();
                    if (bTransferFinish)
                    {
                        OnBuzzer1();
                    }
                    break;

                case CYC_RUN_WORK_FINISH:
                    OffPartFeeder();
                    OffGreenLamp();
                    OnYellowLamp();
                    OnBuzzer2();
                    break;
            }
        }

        const int CYC_INITIAL_STATION_START = 1;
        const int CYC_INITIAL_STATION_STEP100 = 100;
        const int CYC_INITIAL_STATION_STEP101 = 101;
        const int CYC_INITIAL_STATION_STEP103 = 103;
        const int CYC_INITIAL_STATION_STEP104 = 104;
        const int CYC_INITIAL_STATION_STEP105 = 105;
        const int CYC_INITIAL_STATION_STEP200 = 200;
        const int CYC_INITIAL_STATION_STEP201 = 201;
        const int CYC_INITIAL_STATION_STEP202 = 202;
        const int CYC_INITIAL_STATION_STEP203 = 203;
        const int CYC_INITIAL_STATION_STEP300 = 300;
        const int CYC_INITIAL_STATION_STEP301 = 301;
        const int CYC_INITIAL_STATION_STEP302 = 302;
        const int CYC_INITIAL_STATION_STEP303 = 303;
        const int CYC_INITIAL_STATION_STEP1000 = 1000;
        int iInitialStationStep;

        void InitialStation()
        {
            switch (iInitialStationStep)
            {
                case CYC_INITIAL_STATION_START:
                    swPartDrop.Stop();
                    swLinearActuator.Stop();
                    swPartEntryCylinder.Stop();
                    swPartReleseCylinder.Stop();
                    iPartEntryStep = PART_ENTRY_CYCLE_START;
                    iPartReleaseStep = PART_RELEASE_CYCLE_START;
                    iTagMeterCycleStep = TAG_METER_CYCLE_START;
                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_START;
                    iLinearActuatorStep = LINEAR_ACTUATOR_START;
                    iInitialStationStep = CYC_INITIAL_STATION_STEP100;
                    break;

                // Initial Linear Actuator
                case CYC_INITIAL_STATION_STEP100:
                    LinearActuatorTask();
                    if (bLinearActuatorReady)
                    {
                        if (bEnTagMeter)
                        {
                            iInitialStationStep = CYC_INITIAL_STATION_STEP101;
                        }
                        else if (bEnTagProgrammer)
                        {
                            iInitialStationStep = CYC_INITIAL_STATION_STEP200;
                        }
                    }
                    break;

                // Initial Tag Meter Station
                case CYC_INITIAL_STATION_STEP101:
                    if (!IsCylinderInTagMeter())
                    {
                        MoveTagMeterCylinderDown();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP103;
                    }
                    else
                    {
                        iInitialStationStep = CYC_INITIAL_STATION_STEP104;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP103:
                    if (IsCylinderTagMeterDown())
                    {
                        MoveLinearCylinderToTagMeter();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP104;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP104:
                    if (IsCylinderInTagMeter())
                    {
                        MoveTagMeterCylinderUp();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP105;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP105:
                    if (IsCylinderTagMeterUp())
                    {
                        if (bEnTagProgrammer)
                        {
                            iInitialStationStep = CYC_INITIAL_STATION_STEP200;
                        }
                        else
                        {
                            iInitialStationStep = CYC_INITIAL_STATION_STEP1000;
                        }
                    }
                    break;

                // Initial Tag Programmer Station
                case CYC_INITIAL_STATION_STEP200:
                    if (IsPartInTagProgrammer())
                    {
                        iInitialStationStep = CYC_INITIAL_STATION_STEP201;
                    }
                    else
                    {
                        OffTagProgrammerSolenoidValve();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP1000;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP201:
                    if (IsCylinderShuttleClose())
                    {
                        OnTagProgrammerSolenoidValve();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP202;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP202:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iInitialStationStep = CYC_INITIAL_STATION_STEP203;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP203:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iInitialStationStep = CYC_INITIAL_STATION_STEP300;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP300:
                    if (bLinearActuatorReady)
                    {
                        if (bEnTagMeter)
                        {
                            iLinearActuatorPosition = REJECT_BOX4_POSITION;
                        }
                        else
                        {
                            iLinearActuatorPosition = REJECT_BOX1_POSITION;
                        }
                        bStartLinearActuator = true;
                        iInitialStationStep = CYC_INITIAL_STATION_STEP301;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP301:
                    LinearActuatorTask();
                    if (bLinearActuatorReady)
                    {
                        iInitialStationStep = CYC_INITIAL_STATION_STEP1000;
                    }
                    break;

                case CYC_INITIAL_STATION_STEP1000:
                    OnPartFeeder();
                    iInitialStationStep = 0;
                    iCycleStep = CYC_RUN_RUNNING;
                    break;
            }
        }

        const int PART_ENTRY_WAIT_TIME = 250;
        const int PART_ENTRY_CYCLE_START = 1;
        const int PART_ENTRY_CYCLE_STEP100 = 100;
        const int PART_ENTRY_CYCLE_STEP101 = 101;
        const int PART_ENTRY_CYCLE_STEP102 = 102;
        const int PART_ENTRY_CYCLE_STEP103 = 103;
        const int PART_ENTRY_CYCLE_STEP104 = 104;
        Stopwatch swPartEntryCylinder = new Stopwatch();
        int iPartEntryStep;

        private void PartEntryTask()
        {
            switch (iPartEntryStep)
            {
                case PART_ENTRY_CYCLE_START:
                    iPartEntryStep = PART_ENTRY_CYCLE_STEP100;
                    break;

                case PART_ENTRY_CYCLE_STEP100:
                    if (!IsPartEntry())
                    {
                        if (IsCylinderPartReleaseClose())
                        {
                            OpenPartEntryCylinder();
                            iPartEntryStep = PART_ENTRY_CYCLE_STEP101;
                        }
                    }
                    break;

                case PART_ENTRY_CYCLE_STEP101:
                    if (IsCylinderPartEntryOpen())
                    {
                        iPartEntryStep = PART_ENTRY_CYCLE_STEP102;
                    }
                    break;

                case PART_ENTRY_CYCLE_STEP102:
                    if (IsPartEntry())
                    {
                        swPartEntryCylinder.Reset();
                        swPartEntryCylinder.Start();
                        iPartEntryStep = PART_ENTRY_CYCLE_STEP103;
                    }
                    break;

                case PART_ENTRY_CYCLE_STEP103:
                    if (IsPartEntry())
                    {
                        if (swPartEntryCylinder.ElapsedMilliseconds >= PART_ENTRY_WAIT_TIME)
                        {
                            ClosePartEntryCylinder();
                            iPartEntryStep = PART_ENTRY_CYCLE_STEP104;
                        }
                    }
                    break;

                case PART_ENTRY_CYCLE_STEP104:
                    if (IsCylinderPartReleaseClose())
                    {
                        iPartEntryStep = PART_ENTRY_CYCLE_STEP100;
                    }
                    break;
            }
        }

        const int PART_RELEASE_WAIT_TIME = 250;
        const int PART_RELEASE_CYCLE_START = 1;
        const int PART_RELEASE_CYCLE_STEP100 = 100;
        const int PART_RELEASE_CYCLE_STEP101 = 101;
        const int PART_RELEASE_CYCLE_STEP102 = 102;
        const int PART_RELEASE_CYCLE_STEP103 = 103;
        Stopwatch swPartReleseCylinder = new Stopwatch();
        int iPartReleaseStep;
        bool bEnRelease;

        private void PartReleaseTask()
        {
            switch (iPartReleaseStep)
            {
                case PART_RELEASE_CYCLE_START:
                    bEnRelease = false;
                    iPartReleaseStep = PART_RELEASE_CYCLE_STEP100;
                    break;

                case PART_RELEASE_CYCLE_STEP100:
                    if (!bCycleStop)
                    {
                        if (IsPartEntry())
                        {
                            if (IsCylinderPartEntryClose())
                            {
                                if (!bPause)
                                {
                                    if (bEnRelease)
                                    {
                                        bEnRelease = false;
                                        OpenPartReleaseCylinder();
                                        iPartReleaseStep = PART_RELEASE_CYCLE_STEP101;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case PART_RELEASE_CYCLE_STEP101:
                    if (!IsPartEntry())
                    {
                        swPartReleseCylinder.Reset();
                        swPartReleseCylinder.Start();
                        iPartReleaseStep = PART_RELEASE_CYCLE_STEP102;
                    }
                    break;

                case PART_RELEASE_CYCLE_STEP102:
                    if (swPartReleseCylinder.ElapsedMilliseconds >= PART_RELEASE_WAIT_TIME)
                    {
                        swPartReleseCylinder.Stop();
                        ClosePartReleaseCylinder();
                        iPartReleaseStep = PART_RELEASE_CYCLE_STEP103;
                    }
                    break;

                case PART_RELEASE_CYCLE_STEP103:
                    if (IsCylinderPartReleaseClose())
                    {
                        iPartReleaseStep = PART_RELEASE_CYCLE_STEP100;
                    }
                    break;
            }
        }

        const int TAG_METER_CYCLE_START = 1;
        const int TAG_METER_CYCLE_STEP100 = 100;
        const int TAG_METER_CYCLE_STEP101 = 101;
        const int TAG_METER_CYCLE_STEP102 = 102;
        const int TAG_METER_CYCLE_STEP103 = 103;
        const int TAG_METER_CYCLE_STEP104 = 104;
        const int TAG_METER_CYCLE_STEP105 = 105;
        const int TAG_METER_CYCLE_STEP106 = 106;
        const int TAG_METER_CYCLE_STEP110 = 110;
        const int TAG_METER_CYCLE_STEP111 = 111;
        const int TAG_METER_CYCLE_STEP112 = 112;
        const int TAG_METER_CYCLE_STEP113 = 113;
        const int TAG_METER_CYCLE_STEP114 = 114;
        const int TAG_METER_CYCLE_STEP115 = 115;

        const int TAG_METER_CYCLE_STEP120 = 120;
        const int TAG_METER_CYCLE_STEP121 = 121;

        const int TAG_METER_CYCLE_STEP200 = 200;
        const int TAG_METER_CYCLE_STEP201 = 201;
        const int TAG_METER_CYCLE_STEP202 = 202;
        const int TAG_METER_CYCLE_STEP203 = 203;
        const int TAG_METER_CYCLE_STEP204 = 204;
        const int TAG_METER_CYCLE_STEP205 = 205;

        const int TAG_METER_CYCLE_STEP220 = 220;
        const int TAG_METER_CYCLE_STEP221 = 221;
        const int TAG_METER_CYCLE_STEP222 = 222;

        const int TAG_METER_RESULT_OK = 0;
        const int TAG_METER_RESULT_HW_ERROR = 1;
        const int TAG_METER_RESULT_FULL = 2;
        const int TAG_METER_RESULT_FULU = 3;
        const int TAG_METER_RESULT_QULL = 4;
        const int TAG_METER_RESULT_QULU = 5;
        const int TAG_METER_RESULT_FULP = 6;
        const int TAG_METER_RESULT_FLL = 7;
        const int TAG_METER_RESULT_FLU = 8;
        const int TAG_METER_RESULT_QLL = 9;
        const int TAG_METER_RESULT_QLU = 10;
        const int TAG_METER_RESULT_FLP = 11;
        const int TAG_METER_RESULT_CRC_12 = 12;
        const int TAG_METER_RESULT_CRC_13 = 13;

        int iTagMeterCycleStep;
        int iTagMeterResult;
        bool bStartTagMeter;
        bool bTagMeterBusy;

        private void TagMeterTask()
        {
            switch (iTagMeterCycleStep)
            {
                case TAG_METER_CYCLE_START:
                    bStartTagMeter = false;
                    bTagMeterBusy = false;
                    iTagMeterResult = -1;
                    if (!IsCylinderInTagMeter())
                    {
                        MoveTagMeterCylinderDown();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                    }
                    else
                    {
                        MoveLinearCylinderToTagMeter();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP101;
                    }
                    break;

                case TAG_METER_CYCLE_STEP100:
                    if (IsCylinderTagMeterDown())
                    {
                        MoveLinearCylinderToTagMeter();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP101;
                    }
                    else
                    {
                        if (IsCylinderTagMeterUp())
                        {
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP101;
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP101:
                    if (IsCylinderInTagMeter())
                    {
                        MoveTagMeterCylinderUp();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP102;
                    }
                    break;

                case TAG_METER_CYCLE_STEP102:
                    if (IsCylinderTagMeterUp())
                    {
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP103;
                    }
                    break;

                case TAG_METER_CYCLE_STEP103:
                    if (!IsPartInTagMeter())
                    {
                        bEnRelease = true;
                    }
                    bTagMeterBusy = false;
                    iTagMeterCycleStep = TAG_METER_CYCLE_STEP104;
                    break;

                case TAG_METER_CYCLE_STEP104:
                    if (IsPartInTagMeter())
                    {
                        if (iPartInGoodBox == iQuantityPerBox-1)
                        {
                            if (!bTagMeterRunning)
                            {
                                bTagMeterBusy = true;
                                iTagMeterResult = -1;
                                bStartTagMeter = true;
                                Raise(EV_START_TAG_METER);
                                iTagMeterCycleStep = TAG_METER_CYCLE_STEP105;
                            }
                            else
                            {
                                Raise(EV_STOP_TAG_METER);
                            }
                        }
                        else
                        {
                            if (iPartInGoodBox < iQuantityPerBox)
                            {
                                if (IsCylinderTagProgrammerMoveBack())
                                {
                                    if (!IsPartInTagProgrammer())
                                    {
                                        if (bLinearActuatorReady)
                                        {
                                            if (!bTagMeterRunning)
                                            {
                                                bTagMeterBusy = true;
                                                iTagMeterResult = -1;
                                                bStartTagMeter = true;
                                                Raise(EV_START_TAG_METER);
                                                iTagMeterCycleStep = TAG_METER_CYCLE_STEP105;
                                            }
                                            else
                                            {
                                                Raise(EV_STOP_TAG_METER);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP105:
                    if (!bStartTagMeter)
                    {
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP106;
                    }
                    break;

                case TAG_METER_CYCLE_STEP106:
                    if (bEnTagProgrammer)
                    {
                        if (!bTagProgrammerBusy)
                        {
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP110;
                        }
                    }
                    else
                    {
                        if (!IsPartInTagProgrammer())
                        {
                            if (IsCylinderTagProgrammerMoveBack())
                            {
                                iTagMeterCycleStep = TAG_METER_CYCLE_STEP110;
                            }
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP110:
                    if (bEnTagProgrammer)
                    {
                        if (bLinearActuatorReady)
                        {
                            switch (iTagMeterResult)
                            {
                                case TAG_METER_RESULT_FULL:
                                    iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                    break;
                                case TAG_METER_RESULT_FULU:
                                    iLinearActuatorPosition = REJECT_BOX2_POSITION;
                                    break;
                                case TAG_METER_RESULT_FULP:
                                    iLinearActuatorPosition = REJECT_BOX3_POSITION;
                                    break;
                                case TAG_METER_RESULT_FLP:
                                    iLinearActuatorPosition = REJECT_BOX3_POSITION;
                                    break;
                                default:
                                    iLinearActuatorPosition = REJECT_BOX4_POSITION;
                                    break;
                            }
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP111;
                        }
                        else
                        {
                            switch (iTagMeterResult)
                            {
                                case TAG_METER_RESULT_OK:
                                case TAG_METER_RESULT_CRC_13:
                                    iTagMeterCycleStep = TAG_METER_CYCLE_STEP111;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (bLinearActuatorReady)
                        {
                            switch (iTagMeterResult)
                            {
                                case TAG_METER_RESULT_OK:
                                    break;
                                case TAG_METER_RESULT_FULL:
                                    iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                    break;
                                case TAG_METER_RESULT_FULU:
                                    iLinearActuatorPosition = REJECT_BOX2_POSITION;
                                    break;
                                case TAG_METER_RESULT_FULP:
                                    iLinearActuatorPosition = REJECT_BOX3_POSITION;
                                    break;
                                case TAG_METER_RESULT_FLP:
                                    iLinearActuatorPosition = REJECT_BOX3_POSITION;
                                    break;
                                default:
                                    iLinearActuatorPosition = REJECT_BOX4_POSITION;
                                    break;
                            }
                            MoveTagMeterCylinderDown();
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP112;
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP111:
                    if (!IsPartInTagProgrammer())
                    {
                        if (IsCylinderTagProgrammerMoveBack())
                        {
                            MoveTagMeterCylinderDown();
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP112;
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP112:
                    if (IsCylinderTagMeterDown())
                    {
                        MoveLinearCylinderToTagProgrammer();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP113;
                    }
                    break;

                case TAG_METER_CYCLE_STEP113:
                    if (IsCylinderInTagProgrammer())
                    {
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP114;
                    }
                    break;

                case TAG_METER_CYCLE_STEP114:
                    if (IsPartInTagProgrammer())
                    {
                        MoveLinearCylinderToTagMeter();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP115;
                    }
                    break;

                case TAG_METER_CYCLE_STEP115:
                    if (IsCylinderInTagMeter())
                    {
                        MoveTagMeterCylinderUp();
                        if (bEnTagProgrammer)
                        {
                            if (iTagMeterResult == TAG_METER_RESULT_OK || iTagMeterResult == TAG_METER_RESULT_CRC_13)
                            {
                                iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                            }
                            else
                            {
                                if (IsCylinderShuttleClose())
                                {
                                    OnTagProgrammerSolenoidValve();
                                    iTagMeterCycleStep = TAG_METER_CYCLE_STEP120;
                                }
                                else
                                {
                                    CloseShuttleGate();
                                }
                            }
                        }
                        else
                        {
                            if (bLinearActuatorReady)
                            {
                                if (iTagMeterResult == TAG_METER_RESULT_OK)
                                {
                                    iTagMeterCycleStep = TAG_METER_CYCLE_STEP200;
                                }
                                else
                                {
                                    CloseShuttleGate();
                                    iTagMeterCycleStep = TAG_METER_CYCLE_STEP220;
                                }
                            }
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP120:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP121;
                    }
                    break;

                case TAG_METER_CYCLE_STEP121:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        bStartLinearActuator = true;
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                    }
                    break;

                case TAG_METER_CYCLE_STEP200:
                    if (IsCylinderShuttleOpen())
                    {
                        bGoodPartDroped = false;
                        OnTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP202;
                    }
                    else
                    {
                        OpenShuttleGate();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP201;
                    }
                    break;

                case TAG_METER_CYCLE_STEP201:
                    if (IsCylinderShuttleOpen())
                    {
                        bGoodPartDroped = false;
                        OnTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP202;
                    }
                    break;

                case TAG_METER_CYCLE_STEP202:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP203;
                    }
                    break;

                case TAG_METER_CYCLE_STEP203:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP204;
                    }
                    break;

                case TAG_METER_CYCLE_STEP204:
                    if (iTagMeterResult == TAG_METER_RESULT_OK)
                    {
                        swPartDrop.Reset();
                        swPartDrop.Start();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP205;
                    }
                    else
                    {
                        bStartLinearActuator = true;
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                    }
                    break;

                case TAG_METER_CYCLE_STEP205:
                    if (bGoodPartDroped)
                    {
                        bGoodPartDroped = false;
                        swPartDrop.Stop();
                        IncreaseQuantity();
                        if (!bBoxFinished && !bWorkOrderFinished)
                        {
                            iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                        }
                    }
                    else
                    {
                        if (swPartDrop.Elapsed.Seconds >= PART_DROP_TIME)
                        {
                            swPartDrop.Stop();
                            bCycleRun = false;
                            iMessageType = MESSAGE_TYPE_ERROR;
                            sMessage = "Good part jam.";
                            Raise(EV_MESSAGE);
                        }
                    }
                    break;

                case TAG_METER_CYCLE_STEP220:
                    if (IsCylinderShuttleClose())
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP221;
                    }
                    break;

                case TAG_METER_CYCLE_STEP221:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP222;
                    }
                    break;

                case TAG_METER_CYCLE_STEP222:
                    if (!IsPartInTagProgrammer())
                    {
                        bStartLinearActuator = true;
                        iTagMeterCycleStep = TAG_METER_CYCLE_STEP100;
                    }
                    break;
            }
        }

        const int TAG_PROGRAMMER_WAIT_TIME = 5000;
        const int TAG_PROGRAMMER_CYCLE_START = 1;
        const int TAG_PROGRAMMER_CYCLE_STEP100 = 100;
        const int TAG_PROGRAMMER_CYCLE_STEP101 = 101;
        const int TAG_PROGRAMMER_CYCLE_STEP102 = 102;
        const int TAG_PROGRAMMER_CYCLE_STEP103 = 103;
        const int TAG_PROGRAMMER_CYCLE_STEP104 = 104;
        const int TAG_PROGRAMMER_CYCLE_STEP105 = 105;
        const int TAG_PROGRAMMER_CYCLE_STEP106 = 106;
        const int TAG_PROGRAMMER_CYCLE_STEP107 = 107;
        const int TAG_PROGRAMMER_CYCLE_STEP108 = 108;
        const int TAG_PROGRAMMER_CYCLE_STEP109 = 109;
        const int TAG_PROGRAMMER_CYCLE_STEP110 = 110;
        const int TAG_PROGRAMMER_CYCLE_STEP111 = 111;
        const int TAG_PROGRAMMER_CYCLE_STEP112 = 112;
        const int TAG_PROGRAMMER_CYCLE_STEP113 = 113;
        const int TAG_PROGRAMMER_CYCLE_STEP114 = 114;
        const int TAG_PROGRAMMER_CYCLE_STEP115 = 115;
        const int TAG_PROGRAMMER_CYCLE_STEP120 = 120;
        const int TAG_PROGRAMMER_CYCLE_STEP121 = 121;
        const int TAG_PROGRAMMER_CYCLE_STEP122 = 122;
        const int TAG_PROGRAMMER_CYCLE_STEP123 = 123;
        const int TAG_PROGRAMMER_CYCLE_STEP124 = 124;
        const int TAG_PROGRAMMER_CYCLE_STEP125 = 125;

        const int TAG_PROGRAMMER_CYCLE_STEP200 = 200;
        const int TAG_PROGRAMMER_CYCLE_STEP201 = 201;
        const int TAG_PROGRAMMER_CYCLE_STEP202 = 202;
        const int TAG_PROGRAMMER_CYCLE_STEP203 = 203;
        const int TAG_PROGRAMMER_CYCLE_STEP204 = 204;
        const int TAG_PROGRAMMER_CYCLE_STEP205 = 205;
        const int TAG_PROGRAMMER_CYCLE_STEP209 = 209;
        const int TAG_PROGRAMMER_CYCLE_STEP210 = 210;
        const int TAG_PROGRAMMER_CYCLE_STEP211 = 211;
        const int TAG_PROGRAMMER_CYCLE_STEP212 = 212;
        const int TAG_PROGRAMMER_CYCLE_STEP213 = 213;
        const int TAG_PROGRAMMER_CYCLE_STEP214 = 214;
        const int TAG_PROGRAMMER_CYCLE_STEP215 = 215;

        const int TAG_PROGRAMMER_CYCLE_STEP220 = 220;
        const int TAG_PROGRAMMER_CYCLE_STEP221 = 221;
        const int TAG_PROGRAMMER_CYCLE_STEP222 = 222;
        const int TAG_PROGRAMMER_CYCLE_STEP223 = 223;
        const int TAG_PROGRAMMER_CYCLE_STEP224 = 224;
        const int TAG_PROGRAMMER_CYCLE_STEP225 = 225;

        const int TAG_PROGRAMMER_CYCLE_STEP230 = 230;
        const int TAG_PROGRAMMER_CYCLE_STEP231 = 231;

        const int TAG_PROGRAMMER_RESULT_OK = 0;

        Stopwatch swPartDrop = new Stopwatch();
        int iTagProgrammerCycleStep;
        int iTagProgrammerResult;
        int iLastTagMeterResult;
        bool bStartTagProgrammer;
        bool bTagProgrammerBusy = true;
        
        private void TagProgrammerTask()
        {
            switch (iTagProgrammerCycleStep)
            {
                case TAG_PROGRAMMER_CYCLE_START:
                    bStartTagProgrammer = false;
                    if (bEnTagMeter)
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP200;
                    }
                    else
                    {
                        bEnRelease = false;
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP100;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP100:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        if (!IsPartInTagProgrammer())
                        {
                            bTagProgrammerBusy = false;
                            bEnRelease = true;
                        }
                        else
                        {
                            bTagProgrammerBusy = true;
                        }
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP101;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP101:
                    if (IsPartInTagProgrammer())
                    {
                        bTagProgrammerBusy = true;
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP102;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP102:
                    if (!bTagProgrammerRunning)
                    {
                        iTagProgrammerResult = -1;
                        bStartTagProgrammer = true;
                        Raise(EV_START_TAG_PROGRAMMER);
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP103;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP103:
                    if (!bStartTagProgrammer)
                    {
                        if (bEnRegister)
                        {
                            bRegisterFinish = false;
                            bRegisterSuccess = false;
                            if (iTagProgrammerResult == TAG_PROGRAMMER_RESULT_OK)
                            {
                                bRegisterFinish = false;
                                bRegisterSuccess = false;
                                Raise(EV_START_REGISTER_GOOD);
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP104;
                            }
                            else
                            {
                                bRegisterFinish = false;
                                bRegisterSuccess = false;
                                Raise(EV_START_REGISTER_BAD);
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP105;
                            }
                        }
                        else
                        {
                            if (bLinearActuatorReady)
                            {
                                if (iTagProgrammerResult == TAG_PROGRAMMER_RESULT_OK)
                                {
                                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP110;
                                }
                                else
                                {
                                    iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP120;
                                }
                            }
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP104:
                    if (bRegisterFinish)
                    {
                        if (bLinearActuatorReady)
                        {
                            if (bRegisterSuccess)
                            {
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP110;
                            }
                            else
                            {
                                bCycleRun = false;
                            }
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP105:
                    if (bRegisterFinish)
                    {
                        if (bRegisterSuccess)
                        {
                            if (bLinearActuatorReady)
                            {
                                iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP120;
                            }
                        }
                        else
                        {
                            bCycleRun = false;
                        }
                    }
                    break;

                // Good part release
                case TAG_PROGRAMMER_CYCLE_STEP110:
                    if (IsCylinderShuttleOpen())
                    {
                        bGoodPartDroped = false;
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP112;
                    }
                    else
                    {
                        OpenShuttleGate();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP111;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP111:
                    if (IsCylinderShuttleOpen())
                    {
                        bGoodPartDroped = false;
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP112;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP112:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP113;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP113:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP114;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP114:
                    if (!IsPartInTagProgrammer())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP115;
                        swPartDrop.Reset();
                        swPartDrop.Start();
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP115:
                    if (bGoodPartDroped)
                    {
                        bGoodPartDroped = false;
                        swPartDrop.Stop();
                        IncreaseQuantity();
                        if (!bBoxFinished && !bWorkOrderFinished)
                        {
                            iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP100;
                        }
                    }
                    else
                    {
                        if (swPartDrop.Elapsed.Seconds >= PART_DROP_TIME)
                        {
                            swPartDrop.Stop();
                            bCycleRun = false;
                            iMessageType = MESSAGE_TYPE_ERROR;
                            sMessage = "Good part jam.";
                            Raise(EV_MESSAGE);
                        }
                    }
                    break;

                // Reject part release
                case TAG_PROGRAMMER_CYCLE_STEP120:
                    if (!IsCylinderShuttleClose())
                    {
                        CloseShuttleGate();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP121;
                    }
                    else
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP122;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP121:
                    if (IsCylinderShuttleClose())
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP122;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP122:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP123;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP123:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP124;
                    }
                    break;
                case TAG_PROGRAMMER_CYCLE_STEP124:
                    if (!IsPartInTagProgrammer())
                    {
                        bStartLinearActuator = true;
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP100;
                    }
                    break;

                // Enable Tag Meter
                case TAG_PROGRAMMER_CYCLE_STEP200:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        if (IsPartInTagProgrammer())
                        {
                            bTagProgrammerBusy = true;
                            iLastTagMeterResult = iTagMeterResult;
                            iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP201;
                        }
                        else
                        {
                            bTagProgrammerBusy = false;
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP201:
                    switch (iLastTagMeterResult)
                    {
                        case TAG_METER_RESULT_OK:
                            if (bLinearActuatorReady)
                            {
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP210;
                            }
                            break;
                        case TAG_METER_RESULT_CRC_13:
                            if (!bTagProgrammerRunning)
                            {
                                iTagProgrammerResult = -1;
                                bStartTagProgrammer = true;
                                Raise(EV_START_TAG_PROGRAMMER);
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP202;
                            }
                            break;
                        default:
                            if (bLinearActuatorReady)
                            {
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP230;
                            }
                            break;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP202:
                    if (IsCylinderInTagMeter())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP203;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP203:
                    if (!bStartTagProgrammer)
                    {
                        if (bEnRegister)
                        {
                            bRegisterFinish = false;
                            bRegisterSuccess = false;
                            if (iTagProgrammerResult == TAG_PROGRAMMER_RESULT_OK)
                            {
                                bRegisterFinish = false;
                                bRegisterSuccess = false;
                                Raise(EV_START_REGISTER_GOOD);
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP204;
                            }
                            else
                            {
                                bRegisterFinish = false;
                                bRegisterSuccess = false;
                                Raise(EV_START_REGISTER_BAD);
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP205;
                            }
                        }
                        else
                        {
                            if (bLinearActuatorReady)
                            {
                                if (iTagProgrammerResult == TAG_PROGRAMMER_RESULT_OK)
                                {
                                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP210;
                                }
                                else
                                {
                                    if (bSkipTagMeter)
                                    {
                                        iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                    }
                                    else
                                    {
                                        iLinearActuatorPosition = REJECT_BOX4_POSITION;
                                    }
                                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP220;
                                }
                            }
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP204:
                    if (bRegisterFinish)
                    {
                        if (bRegisterSuccess)
                        {
                            if (bLinearActuatorReady)
                            {
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP210;
                            }
                        }
                        else
                        {
                            bCycleRun = false;
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP205:
                    if (bRegisterFinish)
                    {
                        if (bRegisterSuccess)
                        {
                            if (bLinearActuatorReady)
                            {
                                if (bSkipTagMeter)
                                {
                                    iLinearActuatorPosition = REJECT_BOX1_POSITION;
                                }
                                else
                                {
                                    iLinearActuatorPosition = REJECT_BOX4_POSITION;
                                }
                                iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP220;
                            }
                        }
                        else
                        {
                            bCycleRun = false;
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP210:
                    bGoodPartDroped = false;
                    OpenShuttleGate();
                    iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP211;
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP211:
                    if (IsCylinderShuttleOpen())
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP212;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP212:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP213;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP213:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP214;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP214:
                    if (!IsPartInTagProgrammer())
                    {
                        swPartDrop.Reset();
                        swPartDrop.Start();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP215;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP215:
                    if (bGoodPartDroped)
                    {
                        bGoodPartDroped = false;
                        swPartDrop.Stop();
                        IncreaseQuantity();
                        if (!bBoxFinished && !bWorkOrderFinished)
                        {
                            iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP200;
                        }
                    }
                    else
                    {
                        if (swPartDrop.Elapsed.Seconds >= PART_DROP_TIME)
                        {
                            swPartDrop.Stop();
                            bCycleRun = false;
                            iMessageType = MESSAGE_TYPE_ERROR;
                            sMessage = "Good part jam.";
                            Raise(EV_MESSAGE);
                        }
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP220:
                    if (IsCylinderShuttleClose())
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP222;
                    }
                    else
                    {
                        CloseShuttleGate();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP221;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP221:
                    if (IsCylinderShuttleClose())
                    {
                        OnTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP222;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP222:
                    if (IsCylinderTagProgrammerMoveOut())
                    {
                        OffTagProgrammerSolenoidValve();
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP223;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP223:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        bStartLinearActuator = true;
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP224;
                    }
                    break;
                case TAG_PROGRAMMER_CYCLE_STEP224:
                    if (!IsPartInTagProgrammer())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP200;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP230:
                    if (!IsPartInTagProgrammer())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP231;
                    }
                    break;

                case TAG_PROGRAMMER_CYCLE_STEP231:
                    if (IsCylinderTagProgrammerMoveBack())
                    {
                        iTagProgrammerCycleStep = TAG_PROGRAMMER_CYCLE_STEP200;
                    }
                    break;
            }
        }

        const int LINEAR_ACTUATOR_START = 1;

        const int LINEAR_ACTUATOR_STEP100 = 100;
        const int LINEAR_ACTUATOR_STEP101 = 101;
        const int LINEAR_ACTUATOR_STEP102 = 102;
        const int LINEAR_ACTUATOR_STEP103 = 103;
        const int LINEAR_ACTUATOR_STEP104 = 104;
        const int LINEAR_ACTUATOR_STEP105 = 105;

        const int LINEAR_ACTUATOR_STEP120 = 120;
        const int LINEAR_ACTUATOR_STEP121 = 121;
        const int LINEAR_ACTUATOR_STEP130 = 130;
        const int LINEAR_ACTUATOR_STEP131 = 131;
        const int LINEAR_ACTUATOR_STEP132 = 132;
        const int LINEAR_ACTUATOR_STEP133 = 133;
        const int LINEAR_ACTUATOR_STEP134 = 134;
        const int LINEAR_ACTUATOR_STEP135 = 135;
        const int LINEAR_ACTUATOR_STEP136 = 136;
        const int LINEAR_ACTUATOR_STEP137 = 137;
        const int LINEAR_ACTUATOR_STEP150 = 150;

        const int LINEAR_ACTUATOR_STEP200 = 200;
        const int LINEAR_ACTUATOR_STEP201 = 201;
        const int LINEAR_ACTUATOR_STEP202 = 202;
        const int LINEAR_ACTUATOR_STEP203 = 203;
        const int LINEAR_ACTUATOR_STEP204 = 204;
        const int LINEAR_ACTUATOR_STEP205 = 205;
        const int LINEAR_ACTUATOR_STEP206 = 206;
        const int LINEAR_ACTUATOR_STEP207 = 207;
        const int LINEAR_ACTUATOR_STEP208 = 208;
        const int LINEAR_ACTUATOR_STEP209 = 209;
        const int LINEAR_ACTUATOR_STEP210 = 210;
        const int LINEAR_ACTUATOR_STEP211 = 211;

        const int LINEAR_ACTUATOR_DELAY_TIME = 10;
        const int LINEAR_ACTUATOR_MOVE_DELAY_TIME = 500;
        const int LINEAR_ACTUATOR_PULSE_MS = 50;

        const int PART_DROP_TIME = 5;
        const int REJECT_BOX1_POSITION = 2;
        const int REJECT_BOX2_POSITION = 3;
        const int REJECT_BOX3_POSITION = 4;
        const int REJECT_BOX4_POSITION = 5;

        int iLinearActuatorPosition;
        int iLinearActuatorStep;
        bool bLinearActuatorReady;
        bool bStartLinearActuator;
        Stopwatch swLinearActuator = new Stopwatch();

        private void LinearActuatorTask()
        {
            switch (iLinearActuatorStep)
            {
                case LINEAR_ACTUATOR_START:
                    bStartLinearActuator = false;
                    bLinearActuatorReady = false;
                    iLinearActuatorPosition = 0;
                    LinearActuatorOperationMode(false);// Set to Auto Mode
                    swLinearActuator.Reset();
                    swLinearActuator.Start();
                    iLinearActuatorStep = LINEAR_ACTUATOR_STEP100;
                    break;

                case LINEAR_ACTUATOR_STEP100:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Stop();
                        if (!IsLinearActuatorOperatingModeStatus())
                        {
                            LinearActuatorServo(ON);
                            swLinearActuator.Reset();
                            swLinearActuator.Start();
                            iLinearActuatorStep = LINEAR_ACTUATOR_STEP101;
                        }
                    }
                    break;
                case LINEAR_ACTUATOR_STEP101:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Stop();
                        if (IsLinearActuatorServoReader())
                        {
                            LinearActuatorBreakRelease(ON);
                            swLinearActuator.Reset();
                            swLinearActuator.Start();
                            iLinearActuatorStep = LINEAR_ACTUATOR_STEP102;
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP102:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        LinearActuatorPause(true);
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP120;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP120:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP121;
                    }
                    break;
                case LINEAR_ACTUATOR_STEP121:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Stop();
                        if (IsLinearActuatorHomeEnd())
                        {
                            if (!LinearActuatorSignalPM4() && !LinearActuatorSignalPM2() && LinearActuatorSignalPM1())
                            {
                                // If Linear actuator is at good part release position
                                iLinearActuatorStep = LINEAR_ACTUATOR_STEP200;
                            }
                            else
                            {
                                iLinearActuatorStep = LINEAR_ACTUATOR_STEP131;
                            }
                        }
                        else
                        {
                            LinearActuatorHome(true);
                            iLinearActuatorStep = LINEAR_ACTUATOR_STEP130;
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP130:
                    if (IsLinearActuatorHomeEnd())
                    {
                        LinearActuatorHome(false);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP131;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP131:
                    // Move Linear Actuator to Good part release position
                    LinearActuatorPC4(false);
                    LinearActuatorPC2(false);
                    LinearActuatorPC1(true);
                    swLinearActuator.Reset();
                    swLinearActuator.Start();
                    iLinearActuatorStep = LINEAR_ACTUATOR_STEP132;
                    break;

                case LINEAR_ACTUATOR_STEP132:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        LinearActuatorStart(true);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP133;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP133:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_PULSE_MS)
                    {
                        swLinearActuator.Stop();
                        LinearActuatorStart(false);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP134;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP134:
                    if (IsLinearActuatorPositionComplete())
                    {
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP135;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP135:
                    if (!LinearActuatorSignalPM4())
                    {
                        if (!LinearActuatorSignalPM2())
                        {
                            if (LinearActuatorSignalPM1())
                            {
                                // If Linear actuator is at good part release position
                                iLinearActuatorStep = LINEAR_ACTUATOR_STEP200;
                            }
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP200:
                    if (IsLinearActuatorServoReader())
                    {
                        bLinearActuatorReady = true;
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP201;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP201:
                    if (bStartLinearActuator)
                    {
                        if (IsLinearActuatorServoReader())
                        {
                            bLinearActuatorReady = false;
                            bStartLinearActuator = false;
                            switch (iLinearActuatorPosition)
                            {
                                case REJECT_BOX1_POSITION:
                                case REJECT_BOX2_POSITION:
                                case REJECT_BOX3_POSITION:
                                case REJECT_BOX4_POSITION:
                                    CloseShuttleGate();
                                    iLinearActuatorStep = LINEAR_ACTUATOR_STEP202;
                                    break;
                                default:
                                    //throw new NotImplementedException();
                                    break;
                            }
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP202:
                    if (IsCylinderShuttleClose())
                    {
                        switch (iLinearActuatorPosition)
                        {
                            case REJECT_BOX1_POSITION:
                                LinearActuatorPC4(false);
                                LinearActuatorPC2(true);
                                LinearActuatorPC1(false);
                                break;
                            case REJECT_BOX2_POSITION:
                                LinearActuatorPC4(false);
                                LinearActuatorPC2(true);
                                LinearActuatorPC1(true);
                                break;
                            case REJECT_BOX3_POSITION:
                                LinearActuatorPC4(true);
                                LinearActuatorPC2(false);
                                LinearActuatorPC1(false);
                                break;
                            case REJECT_BOX4_POSITION:
                                LinearActuatorPC4(true);
                                LinearActuatorPC2(false);
                                LinearActuatorPC1(true);
                                break;
                        }
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP203;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP203:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        LinearActuatorStart(true);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP204;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP204:
                    if (IsLinearActuatorMoving())
                    {
                        swLinearActuator.Stop();
                        LinearActuatorStart(false);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP205;
                    }
                    else
                    {
                        if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_MOVE_DELAY_TIME)
                        {
                            swLinearActuator.Stop();
                            bCycleRun = false;
                            iMessageType = MESSAGE_TYPE_ERROR;
                            sMessage = "Linear actuator is not moving.";
                            Raise(EV_MESSAGE);
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP205:
                    if (IsLinearActuatorPositionComplete())
                    {
                        bRejectPartDroped = false;
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP206;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP206:
                    switch (iLinearActuatorPosition)
                    {
                        case REJECT_BOX1_POSITION:
                            if (!LinearActuatorSignalPM4())
                            {
                                if (LinearActuatorSignalPM2())
                                {
                                    if (!LinearActuatorSignalPM1())
                                    {
                                        OpenShuttleGate();
                                        swLinearActuator.Reset();
                                        swLinearActuator.Start();
                                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP207;
                                    }
                                }
                            }
                            break;

                        case REJECT_BOX2_POSITION:
                            if (!LinearActuatorSignalPM4())
                            {
                                if (LinearActuatorSignalPM2())
                                {
                                    if (LinearActuatorSignalPM1())
                                    {
                                        OpenShuttleGate();
                                        swLinearActuator.Reset();
                                        swLinearActuator.Start();
                                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP207;
                                    }
                                }
                            }
                            break;

                        case REJECT_BOX3_POSITION:
                            if (LinearActuatorSignalPM4())
                            {
                                if (!LinearActuatorSignalPM2())
                                {
                                    if (!LinearActuatorSignalPM1())
                                    {
                                        OpenShuttleGate();
                                        swLinearActuator.Reset();
                                        swLinearActuator.Start();
                                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP207;
                                    }
                                }
                            }
                            break;

                        case REJECT_BOX4_POSITION:
                            if (LinearActuatorSignalPM4())
                            {
                                if (!LinearActuatorSignalPM2())
                                {
                                    if (LinearActuatorSignalPM1())
                                    {
                                        OpenShuttleGate();
                                        swLinearActuator.Reset();
                                        swLinearActuator.Start();
                                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP207;
                                    }
                                }
                            }
                            break;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP207:
                    if (IsCylinderShuttleOpen())
                    {
                        switch (iLinearActuatorPosition)
                        {
                            case REJECT_BOX1_POSITION:
                            case REJECT_BOX2_POSITION:
                            case REJECT_BOX3_POSITION:
                            case REJECT_BOX4_POSITION:
                                if (bRejectPartDroped)
                                {
                                    bRejectPartDroped = false;
                                    swLinearActuator.Stop();
                                    CloseShuttleGate();
                                    aiPartInRejectBox[iLinearActuatorPosition - 2]++;
                                    Raise(EV_UPDATE_PARAMETER);
                                    iLinearActuatorStep = LINEAR_ACTUATOR_STEP208;
                                }
                                else
                                {
                                    if (swLinearActuator.Elapsed.Seconds >= PART_DROP_TIME)
                                    {
                                        bCycleRun = false;
                                        iMessageType = MESSAGE_TYPE_ERROR;
                                        sMessage = String.Format("Reject part jam in reject box {0}.", iLinearActuatorPosition - 1);
                                        Raise(EV_MESSAGE);
                                    }
                                }
                                break;
                            default:
                                //throw new NotImplementedException();
                                break;
                        }
                    }
                    break;

                case LINEAR_ACTUATOR_STEP208:
                    if (IsCylinderShuttleClose())
                    {
                        LinearActuatorPC4(false);
                        LinearActuatorPC2(false);
                        LinearActuatorPC1(true);
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP209;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP209:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_DELAY_TIME)
                    {
                        swLinearActuator.Reset();
                        swLinearActuator.Start();
                        LinearActuatorStart(true);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP210;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP210:
                    if (swLinearActuator.ElapsedMilliseconds >= LINEAR_ACTUATOR_PULSE_MS)
                    {
                        swLinearActuator.Stop();
                        LinearActuatorStart(false);
                        iLinearActuatorStep = LINEAR_ACTUATOR_STEP211;
                    }
                    break;

                case LINEAR_ACTUATOR_STEP211:
                    // Go to good part box position
                    if (IsLinearActuatorPositionComplete())
                    {
                        if (!LinearActuatorSignalPM4())
                        {
                            if (!LinearActuatorSignalPM2())
                            {
                                if (LinearActuatorSignalPM1())
                                {
                                    swLinearActuator.Reset();
                                    swLinearActuator.Start();
                                    iLinearActuatorStep = LINEAR_ACTUATOR_STEP200;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        void IncreaseQuantity()
        {
            bBoxFinished = false;
            bWorkOrderFinished = false;
            iPartInGoodBox++;
            iQuantityWorked++;
            if (iPartInGoodBox >= iQuantityPerBox)
            {
                bBoxFinished = true;
                bTransferFinish = false;
                iCycleStep = CYC_RUN_BOX_FINISH;
            }
            //else
            //{
            //    if (iQuantityWorked >= iWorkOrderTarget)
            //    {
            //        bWorkOrderFinished = true;
            //        iCycleStep = CYC_RUN_WORK_FINISH;
            //    }
            //}

            if (bBoxFinished)
            {
                Raise(EV_BOX_FINISH);
            }
            else if (bWorkOrderFinished)
            {
                iMessageType = MESSAGE_TYPE_INFORMATION;
                sMessage = "Work order finish.";
                Raise(EV_MESSAGE);
            }
            else
            {
                Raise(EV_UPDATE_PARAMETER);
            }
        }

        public bool EnTagMeter
        {
            get { return bEnTagMeter; }
            set { bEnTagMeter = value; }
        }

        public bool EnTagProgrammer
        {
            get { return bEnTagProgrammer; }
            set { bEnTagProgrammer = value; }
        }

        public bool EnRegister
        {
            get { return bEnRegister; }
            set { bEnRegister = value; }
        }

        public bool BoxFinished
        {
            get { return bBoxFinished; }
            set { bBoxFinished = value; }
        }

        public bool WorkOrderFinished
        {
            get { return bWorkOrderFinished; }
        }

        public bool StartTagMeter
        {
            set { bStartTagMeter = value; }
        }

        public bool StartTagProgrammer
        {
            set { bStartTagProgrammer = value; }
        }

        public bool RegigsterFinished
        {
            set { bRegisterFinish = value; }
        }

        public bool RegisterSuccess
        {
            set { bRegisterSuccess = value; }
        }

        public int TagMeterResult
        {
            set { iTagMeterResult = value; }
        }

        public int TagProgrammerResult
        {
            get { return iTagProgrammerResult; }
            set { iTagProgrammerResult = value; }
        }

        public int PartInGoodBox
        {
            get { return iPartInGoodBox; }
            set { iPartInGoodBox = value; }
        }

        public int QuantityWorked
        {
            get { return iQuantityWorked; }
            set { iQuantityWorked = value; }
        }

        public int QuantityPerBox
        {
            set { iQuantityPerBox = value; }
        }

        public int WorkOrderTarget
        {
            get { return iWorkOrderTarget; }
            set { iWorkOrderTarget = value; }
        }

        public int[] PartInRejectBox
        {
            get { return aiPartInRejectBox; }
        }

        public bool TransferFinish
        {
            set { bTransferFinish = value; }
        }

        public bool PartEmpty
        {
            get { return bPartEmpty; }
        }

        public bool SktipTagMeter
        {
            set { bSkipTagMeter = value; }
        }
    }
}
