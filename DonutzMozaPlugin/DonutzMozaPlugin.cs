using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Win32;
using SimHub.Plugins.OutputPlugins.GraphicalDash.UI;
using System.Windows.Markup;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime;
using System.Drawing;
using static DonutzMozaPlugin.DonutzMozaPluginSettings;
using System.Windows.Controls;
using System.Collections.Generic;
using MahApps.Metro.Controls;

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

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (data.GameRunning)
            {
                
                if (data.OldData != null && data.NewData != null)
                {
                    string profileName = data.GameName + '_' + data.NewData.CarId;
                    if (data.OldData.CarId != data.NewData.CarId)
                    {

                        //SetMozaProfile(Settings.GetProfile(profileName));
                        //this.AttachDelegate("mozaProfileName", () => profileName);
                        SetMozaProfile();
                    }
                }
                if (data.OldData == null && data.NewData != null)
                {
                    
                    if (Settings.getGameSetting(data.GameName).activeProfileMapping)
                    {
                        //string profileName = data.GameName + '_' + data.NewData.CarId;
                        //SetMozaProfile(Settings.GetProfile(profileName));
                        //this.AttachDelegate("mozaProfileName", () => profileName);
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
            profileAMP.Add("EqualizerAmp100", mozaProfile.EQ100_EqualizerAmp100);
            profileAMP.Add("EqualizerAmp55", mozaProfile.EQ60_EqualizerAmp55);
            
            mozaAPI.mozaAPI.setMotorEqualizerAmp(profileAMP);

            this.AttachDelegate("mozaProfile", () => mozaProfile);
        }

        public void SetMozaProfile()
        {
            if (PluginManager.LastData.GameRunning)
            {
                string game = PluginManager.LastData.GameName;
                GameSetting currentGameSettings = Settings.getGameSetting(game);
                if (currentGameSettings.activeProfileMapping)
                {
                    if (currentGameSettings.activeCarMapping)
                    {
                        string carID = PluginManager.LastData.OldData.CarId;
                        if ((game != null) && (carID != null))
                        {
                            MozaProfile mozaProfile = Settings.GetProfile(game + '_' + carID);
                            SetMozaProfile(mozaProfile);
                            this.AttachDelegate("mozaProfileName", () => game + '_' + carID);
                        }
                    }
                    else 
                    {
                        MozaProfile mozaProfile = Settings.GetProfile(game);
                        SetMozaProfile(mozaProfile);
                        this.AttachDelegate("mozaProfileName", () => game);
                    }

                    
                }
                    
            }
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting Donutz Moza plugin");
            Settings = this.ReadCommonSettings<DonutzMozaPluginSettings>("MozaPluginSettings", () => new DonutzMozaPluginSettings());
            mozaAPI.mozaAPI.installMozaSDK();
            this.AttachDelegate("currentMozaSettings", () => getCurrentMozaSettings());
            this.AttachDelegate("cursorPos", () => cursorPos);

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
                            IncreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName + '_' + PluginManager.LastData.OldData.CarId));
                        }
                        else
                        {
                            IncreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName));
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
                            DecreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName + '_' + PluginManager.LastData.OldData.CarId));
                        }
                        else
                        {
                            DecreaseValue(cursorPos, Settings.GetProfile(PluginManager.LastData.GameName));
                        }
                    }
                }
            });

        }
        public MozaProfile getCurrentMozaSettings()
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
                EQ100_EqualizerAmp100 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp100"]
            };
            return currentMozaSettings;


        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControl(this, Settings);
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
                    mozaProfileToBeChanged.EQ100_EqualizerAmp100 = Clamp(mozaProfileToBeChanged.EQ100_EqualizerAmp100 + 10, 0, 500);
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
                    mozaProfileToBeChanged.EQ100_EqualizerAmp100 = Clamp(mozaProfileToBeChanged.EQ100_EqualizerAmp100 - 10, 0, 500);
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
