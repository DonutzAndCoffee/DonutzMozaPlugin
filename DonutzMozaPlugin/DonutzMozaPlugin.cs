using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static DonutzMozaPlugin.DonutzMozaPluginSettings;



namespace DonutzMozaPlugin
{
    [PluginDescription("Donutz Moza Plugin")]
    [PluginAuthor("Donutz")]
    [PluginName("Donutz Moza Plugin")]
    public class DonutzMozaPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DonutzMozaPluginSettings Settings;
        public PluginManager PluginManager { get; set; }
        public ImageSource PictureIcon => this.ToIcon(LoadBitmapFromResources(Properties.Resources.don2));

        public int cursorPos = 1;

        public string LeftMenuTitle => "Moza Plugin";
        private SettingsViewModel _viewModel;


        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {

            //if (_viewModel.MappingFilterText != data.GameName)
            //{
            //    _viewModel.MappingFilterText = data.GameName;
            //}

            if (data.GameRunning)
            {

                if (_viewModel.MappingFilterText != data.GameName)
                {
                    _viewModel.MappingFilterText = data.GameName;
                }

                if (data.OldData != null && data.NewData != null)
                {
                    string profileName = data.GameName + '_' + data.NewData.CarId;
                    if (data.OldData.CarId != data.NewData.CarId)
                    {
                        SetMozaProfile();
                    }
                    
                }
                if (data.OldData == null && data.NewData != null)
                {
                    
                    if (Settings.getGameSetting(data.GameName).activeProfileMapping)
                    {
                        
                        
                        var devices = PluginManager.GetConnectedGameControllers();
                        
                        foreach (var device in devices)
                        {
                            SimHub.Logging.Current.Info("Devices: " + device.Name);
                            SimHub.Logging.Current.Info("Devices: " + device.Settings.InstanceId);
                        }

                        SetMozaProfile();
                    }
                    else
                    {
                        SimHub.Logging.Current.Info("MOZA " + data.GameName + ": " + Settings.getGameSetting(data.GameName).activeProfileMapping);
                    }
                }
            }
        }

        public void SetMozaProfile(MozaProfile mozaProfile)
        {
            Dictionary<string, int> profileAMP = new Dictionary<string, int>();
            SimHub.Logging.Current.Info("Trying to set Moza wheel base config to " + mozaProfile.MotorFfbStrength + " " + mozaProfile.MotorLimitAngle);
            mozaAPI.mozaAPI.setMotorFfbStrength(mozaProfile.MotorFfbStrength);
            mozaAPI.mozaAPI.setMotorLimitAngle(mozaProfile.MotorLimitAngle, mozaProfile.MotorLimitAngle);
            mozaAPI.mozaAPI.setMotorNaturalDamper(mozaProfile.MotorNaturalDamper);
            mozaAPI.mozaAPI.setMotorLimitWheelSpeed(mozaProfile.MotorLimitWheelSpeed);
            mozaAPI.mozaAPI.setMotorSpringStrength(mozaProfile.MotorSpringStrength);
            //mozaAPI.mozaAPI.setMotorRoadSensitivity(mozaProfile.MotorRoadSensitivity);
            mozaAPI.mozaAPI.setMotorNaturalInertia(mozaProfile.MotorNaturalInertia);
            mozaAPI.mozaAPI.setMotorNaturalFriction(mozaProfile.MotorNaturalFriction);
            mozaAPI.mozaAPI.setMotorSpeedDamping(mozaProfile.MotorSpeedDamping);
            mozaAPI.mozaAPI.setMotorSpeedDampingStartPoint(mozaProfile.MotorSpeedDampingStartPoint);
            
            profileAMP.Add("EqualizerAmp7_5", mozaProfile.EQ10_EqualizerAmp7_5);
            profileAMP.Add("EqualizerAmp13", mozaProfile.EQ15_EqualizerAmp13);
            profileAMP.Add("EqualizerAmp22_5", mozaProfile.EQ25_EqualizerAmp22_5);
            profileAMP.Add("EqualizerAmp39", mozaProfile.EQ40_EqualizerAmp39);
            
            profileAMP.Add("EqualizerAmp55", mozaProfile.EQ60_EqualizerAmp55);
            profileAMP.Add("EqualizerAmp100", mozaProfile.EQ100_EqualizerAmp100);

            mozaAPI.mozaAPI.setMotorEqualizerAmp(profileAMP);
            if (Settings.getGameSetting(PluginManager.GameName).reversedFFB)
            {
                mozaAPI.mozaAPI.setMotorFfbReverse(1);
            }
            else
            {
                mozaAPI.mozaAPI.setMotorFfbReverse(0);
            }


            

            //mozaAPI.ERRORCODE err = mozaAPI.ERRORCODE.NORMAL;
            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.getSteeringWheelShiftIndicatorMode(ref err));
            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.setSteeringWheelShiftIndicatorMode(1));
            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.getSteeringWheelShiftIndicatorMode(ref err));

            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.setSteeringWheelShiftIndicatorSwitch(1)); // 1 = RPM Indicator, 2 = off, 3= On

            //err = mozaAPI.ERRORCODE.NORMAL;
            //var test = mozaAPI.mozaAPI.getSteeringWheelShiftIndicatorLightRpm(ref err);
            //foreach (var item in test)
            //{
            //    SimHub.Logging.Current.Info("RPM: " + item);
            //}

            //List<int> numbers = new List<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 10 };

            //List<string> colors = new List<string> { "#FF00CE00", "#FF00CE00", "#FF00CE00", "#FFFF0606", "#FFFF0606", "#FFFF0606", "#FFFF3CFF", "#FFFF3CFF", "#FFFF3CFF", "#FFFF3CFF" };

            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.setSteeringWheelShiftIndicatorLightRpm(numbers));
            //SimHub.Logging.Current.Info(mozaAPI.mozaAPI.setSteeringWheelShiftIndicatorColor(colors));

            this.AttachDelegate("mozaProfile", () => mozaProfile);            
        }

        public void SetMozaProfile()
        {
            if (PluginManager.LastData!=null)
            {
                if (PluginManager.LastData.GameRunning)
                {
                    string game = PluginManager.LastData.GameName;
                    GameSetting currentGameSettings = Settings.getGameSetting(game);
                    if (currentGameSettings.activeProfileMapping)
                    {
                        if (currentGameSettings.activeCarMapping)
                        {
                            string carID = PluginManager.LastData.NewData.CarId;
                            string carModel = PluginManager.LastData.NewData.CarModel;
                            if ((game != null) && (carID != null))
                            {
                                string key;
                                MozaProfile mozaProfile = Settings.GetProfile(game, carID, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, carModel, out key);
                                SetMozaProfile(mozaProfile);

                                this.AttachDelegate("mozaProfileName", () => key);
                                _viewModel.ActiveProfileKey = key;

                            }
                        }
                        else
                        {
                            string key;
                            MozaProfile mozaProfile = Settings.GetProfile(game, null, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, null, out key);
                            SetMozaProfile(mozaProfile);
                            this.AttachDelegate("mozaProfileName", () => key);
                            _viewModel.ActiveProfileKey = key;

                        }
                    }
                }
            }
            
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting Donutz Moza plugin");

            Settings = this.ReadCommonSettings<DonutzMozaPluginSettings>("MozaPluginSettings", () => new DonutzMozaPluginSettings());
            //_viewModel.ActiveProfileKey = "none";
            

            mozaAPI.mozaAPI.installMozaSDK();
            this.AttachDelegate("currentMozaSettings", () => getCurrentMozaSettings());
            this.AttachDelegate("cursorPos", () => cursorPos);

            this.AddAction("CenterWheel", (a, b) => 
            {
                mozaAPI.mozaAPI.CenterWheel();
                
            });

            this.AddAction("ApplyProfile", (a, b) =>
            {
                SetMozaProfile();

            });

            this.AddAction("CursorUp", (a, b) =>
            {
                cursorPos = CursorUp(cursorPos);
                this.AttachDelegate("cursorPos", () => cursorPos);
            });

            this.AddAction("CursorDown", (a, b) =>
            {
                cursorPos = CursorDown(cursorPos);
                this.AttachDelegate("cursorPos", () => cursorPos);
            });

            this.AddAction("IncreaseValue", (a, b) => 
            {
                if (PluginManager.LastData.GameRunning)
                {
                    string game = PluginManager.LastData.GameName;
                    
                    GameSetting currentGameSettings = Settings.getGameSetting(game);
                    if (currentGameSettings.activeProfileMapping)
                    {
                        if (currentGameSettings.activeCarMapping)
                        {
                            string carModel = PluginManager.LastData.NewData.CarModel;
                            string key;
                            IncreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName, PluginManager.LastData.OldData.CarId, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, carModel, out key));
                        }
                        else
                        {
                            string key;
                            IncreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName, null, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, null, out key));
                        }
                    }
                }
            });

            this.AddAction("DecreaseValue", (a, b) =>
            {
                if (PluginManager.LastData.GameRunning)
                {
                    string game = PluginManager.LastData.GameName;
                    GameSetting currentGameSettings = Settings.getGameSetting(game);
                    if (currentGameSettings.activeProfileMapping)
                    {
                        if (currentGameSettings.activeCarMapping)
                        {
                            string carModel = PluginManager.LastData.NewData.CarModel;
                            string key;
                            DecreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName, PluginManager.LastData.OldData.CarId, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, carModel, out key));
                        }
                        else
                        {
                            string key;
                            DecreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName, null, currentGameSettings.activeCarMapping, currentGameSettings.learnNewCars, null, out key));
                        }
                    }
                }
            });

        }
        public MozaProfile getCurrentMozaSettings()
        {
            try
            {
                mozaAPI.ERRORCODE err = mozaAPI.ERRORCODE.NORMAL;
                MozaProfile currentMozaSettings = new MozaProfile
                {
                    MotorFfbStrength = mozaAPI.mozaAPI.getMotorFfbStrength(ref err),
                    MotorLimitAngle = mozaAPI.mozaAPI.getMotorLimitAngle(ref err).Item2,
                    MotorNaturalDamper = mozaAPI.mozaAPI.getMotorNaturalDamper(ref err),
                    MotorLimitWheelSpeed = mozaAPI.mozaAPI.getMotorLimitWheelSpeed(ref err),
                    MotorSpringStrength = mozaAPI.mozaAPI.getMotorSpringStrength(ref err),
                    MotorRoadSensitivity = mozaAPI.mozaAPI.getMotorRoadSensitivity(ref err),
                    MotorNaturalInertia = mozaAPI.mozaAPI.getMotorNaturalInertia(ref err),
                    MotorNaturalFriction = mozaAPI.mozaAPI.getMotorNaturalFriction(ref err),
                    MotorSpeedDamping = mozaAPI.mozaAPI.getMotorSpeedDamping(ref err),
                    MotorSpeedDampingStartPoint = mozaAPI.mozaAPI.getMotorSpeedDampingStartPoint(ref err),
                    EQ10_EqualizerAmp7_5 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp7_5"],
                    EQ15_EqualizerAmp13 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp13"],
                    EQ25_EqualizerAmp22_5 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp22_5"],
                    EQ40_EqualizerAmp39 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp39"],
                    EQ60_EqualizerAmp55 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp55"],
                    EQ100_EqualizerAmp100 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp100"],
                    CarModel = "-"
                };
                return currentMozaSettings;
            }
            catch (Exception e)
            {
                MozaProfile currentMozaSettings = null;
                return currentMozaSettings;
            }
        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {

            var control = new SettingsControl(this, Settings);
            _viewModel = control.ViewModel;
            _viewModel.ActiveProfileKey = "none";


            if (_viewModel.MappingFilterText != pluginManager.GameName)
            {
                _viewModel.MappingFilterText = pluginManager.GameName;
            }
            return control;
            //return new SettingsControl(this, Settings);
        }

        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("MozaPluginSettings", Settings);
            
            mozaAPI.mozaAPI.removeMozaSDK();
        }

        public Bitmap LoadBitmapFromResources(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return new Bitmap(ms);
            }
        }

        public int CursorDown(int curPos)
        {
            if (curPos < 16)
            {
                curPos++;
            }
            return curPos;
        }

        public int CursorUp(int curPos)
        {
            if (curPos > 1)
            {
                curPos--;
            }
            return curPos;
        }

        public void IncreaseValue(int curPos, MozaProfile mozaProfileToBeChanged)
        {
            switch (curPos)
            {
                case 1:
                    mozaProfileToBeChanged.MotorFfbStrength = Clamp(mozaProfileToBeChanged.MotorFfbStrength + 1, 0, 100);
                    break;
                case 2:
                    mozaProfileToBeChanged.MotorLimitAngle = Clamp(mozaProfileToBeChanged.MotorLimitAngle + 2, 90, 2000);
                    break;
                case 3:
                    mozaProfileToBeChanged.MotorNaturalDamper = Clamp(mozaProfileToBeChanged.MotorNaturalDamper + 1, 0, 100);
                    break;
                case 4:
                    mozaProfileToBeChanged.MotorLimitWheelSpeed = Clamp(mozaProfileToBeChanged.MotorLimitWheelSpeed + 1, 10, 200);
                    break;
                case 5:
                    mozaProfileToBeChanged.MotorSpringStrength = Clamp(mozaProfileToBeChanged.MotorSpringStrength + 1, 0, 100);
                    break;
                case 6:
                    mozaProfileToBeChanged.MotorRoadSensitivity = Clamp(mozaProfileToBeChanged.MotorRoadSensitivity + 1, 0, 10);
                    break;
                case 7:
                    mozaProfileToBeChanged.MotorNaturalInertia = Clamp(mozaProfileToBeChanged.MotorNaturalInertia + 1, 100, 250);
                    break;
                case 8:
                    mozaProfileToBeChanged.MotorNaturalFriction = Clamp(mozaProfileToBeChanged.MotorNaturalFriction + 1, 0, 100);
                    break;
                case 9:
                    mozaProfileToBeChanged.MotorSpeedDamping = Clamp(mozaProfileToBeChanged.MotorSpeedDamping + 1, 0, 100);
                    break;
                case 10:
                    mozaProfileToBeChanged.MotorSpeedDampingStartPoint = Clamp(mozaProfileToBeChanged.MotorSpeedDampingStartPoint + 1, 0, 400);
                    break;
                case 11:
                    mozaProfileToBeChanged.EQ10_EqualizerAmp7_5 = Clamp(mozaProfileToBeChanged.EQ10_EqualizerAmp7_5 + 10, 0, 500);
                    break;
                case 12:
                    mozaProfileToBeChanged.EQ15_EqualizerAmp13 = Clamp(mozaProfileToBeChanged.EQ15_EqualizerAmp13 + 10, 0, 500);
                    break;
                case 13:
                    mozaProfileToBeChanged.EQ25_EqualizerAmp22_5 = Clamp(mozaProfileToBeChanged.EQ25_EqualizerAmp22_5 + 10, 0, 500);
                    break;
                case 14:
                    mozaProfileToBeChanged.EQ40_EqualizerAmp39 = Clamp(mozaProfileToBeChanged.EQ40_EqualizerAmp39 + 10, 0, 500);
                    break;
                case 15:
                    mozaProfileToBeChanged.EQ60_EqualizerAmp55 = Clamp(mozaProfileToBeChanged.EQ60_EqualizerAmp55 + 10, 0, 500);
                    break;
                case 16:
                    mozaProfileToBeChanged.EQ100_EqualizerAmp100 = Clamp(mozaProfileToBeChanged.EQ100_EqualizerAmp100 + 10, 0, 100);
                    break;

                default:
                    Console.WriteLine("Invalid position");
                    break;
            }

            SetMozaProfile();
        }

        public void DecreaseValue(int curPos, MozaProfile mozaProfileToBeChanged)
        {
            switch (curPos)
            {
                case 1:
                    mozaProfileToBeChanged.MotorFfbStrength = Clamp(mozaProfileToBeChanged.MotorFfbStrength - 1, 0, 100);
                    break;
                case 2:
                    mozaProfileToBeChanged.MotorLimitAngle = Clamp(mozaProfileToBeChanged.MotorLimitAngle - 2, 90, 2000);
                    break;
                case 3:
                    mozaProfileToBeChanged.MotorNaturalDamper = Clamp(mozaProfileToBeChanged.MotorNaturalDamper - 1, 0, 100);
                    break;
                case 4:
                    mozaProfileToBeChanged.MotorLimitWheelSpeed = Clamp(mozaProfileToBeChanged.MotorLimitWheelSpeed - 1, 10, 200);
                    break;
                case 5:
                    mozaProfileToBeChanged.MotorSpringStrength = Clamp(mozaProfileToBeChanged.MotorSpringStrength - 1, 0, 100);
                    break;
                case 6:
                    mozaProfileToBeChanged.MotorRoadSensitivity = Clamp(mozaProfileToBeChanged.MotorRoadSensitivity - 1, 0, 10);
                    break;
                case 7:
                    mozaProfileToBeChanged.MotorNaturalInertia = Clamp(mozaProfileToBeChanged.MotorNaturalInertia - 1, 100, 250);
                    break;
                case 8:
                    mozaProfileToBeChanged.MotorNaturalFriction = Clamp(mozaProfileToBeChanged.MotorNaturalFriction - 1, 0, 100);
                    break;
                case 9:
                    mozaProfileToBeChanged.MotorSpeedDamping = Clamp(mozaProfileToBeChanged.MotorSpeedDamping - 1, 0, 100);
                    break;
                case 10:
                    mozaProfileToBeChanged.MotorSpeedDampingStartPoint = Clamp(mozaProfileToBeChanged.MotorSpeedDampingStartPoint - 1, 0, 400);
                    break;
                case 11:
                    mozaProfileToBeChanged.EQ10_EqualizerAmp7_5 = Clamp(mozaProfileToBeChanged.EQ10_EqualizerAmp7_5 - 10, 0, 500);
                    break;
                case 12:
                    mozaProfileToBeChanged.EQ15_EqualizerAmp13 = Clamp(mozaProfileToBeChanged.EQ15_EqualizerAmp13 - 10, 0, 500);
                    break;
                case 13:
                    mozaProfileToBeChanged.EQ25_EqualizerAmp22_5 = Clamp(mozaProfileToBeChanged.EQ25_EqualizerAmp22_5 - 10, 0, 500);
                    break;
                case 14:
                    mozaProfileToBeChanged.EQ40_EqualizerAmp39 = Clamp(mozaProfileToBeChanged.EQ40_EqualizerAmp39 - 10, 0, 500);
                    break;
                case 15:
                    mozaProfileToBeChanged.EQ60_EqualizerAmp55 = Clamp(mozaProfileToBeChanged.EQ60_EqualizerAmp55 - 10, 0, 500);
                    break;
                case 16:
                    mozaProfileToBeChanged.EQ100_EqualizerAmp100 = Clamp(mozaProfileToBeChanged.EQ100_EqualizerAmp100 - 10, 0, 100);
                    break;

                default:
                    Console.WriteLine("Invalid position");
                    break;
            }

            SetMozaProfile();
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }




    }
}
