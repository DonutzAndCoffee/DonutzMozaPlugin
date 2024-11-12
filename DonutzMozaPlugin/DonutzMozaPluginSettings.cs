using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DonutzMozaPlugin
{
    public class DonutzMozaPluginSettings
    {
        public class MozaProfile
        {
            public int MotorFfbStrength { get; set; } // min 0, max 100
            public int MotorLimitAngle { get; set; } // min 90, max 2000
            public int MotorNaturalDamper { get; set; } // min 0, max 100 
            public int MotorLimitWheelSpeed { get; set; } // min 10, max 200

            public int MotorSpringStrength { get; set; } // min 0, max 100

            public int MotorRoadSensitivity { get; set; } // min 0, max 10

            public int MotorNaturalInertia { get; set; } // min 100, max 250
            public int MotorNaturalFriction { get; set; } // min 0, max 100
            public int MotorSpeedDamping { get; set; } // min 0, max 100
            public int MotorSpeedDampingStartPoint { get; set; } // min 0, max 400

            public int EQ10_EqualizerAmp7_5 { get; set; } // min 0, max 500
            public int EQ15_EqualizerAmp13 { get; set; } // min 0, max 500
            public int EQ25_EqualizerAmp22_5 { get; set; } // min 0, max 500
            public int EQ40_EqualizerAmp39 { get; set; } // min 0, max 500
            public int EQ60_EqualizerAmp55 { get; set; } // min 0, max 500
            public int EQ100_EqualizerAmp100 { get; set; } // min 0, max 500

            
                    
                    
                    
                    
                    


            // Weitere Parameter hinzufügen, je nach Bedarf
        }

        public class GameSetting
        {
            public string Name { get; set; }

            public bool activeProfileMapping { get; set; } = false;

            public bool activeCarMapping { get; set; } = false;

            public GameSetting(string gameName) { 
                this.Name = gameName;
                this.activeProfileMapping = false;
                this.activeCarMapping = false;
            }
        }

        public GameSetting getGameSetting(string gameName)
        {
            if (gameSettings.TryGetValue(gameName, out GameSetting gameSetting))
            {
                return gameSetting;
            }
            else
            { 
                GameSetting newGameSetting = new GameSetting(gameName);
                gameSettings[gameName] = newGameSetting;
                return newGameSetting;             }
        }


        // Dictionary zur Zuordnung von Spiel/Fahrzeug-Kombinationen zu Profilen
        public Dictionary<string, MozaProfile> profileMapping { get; set; } = new Dictionary<string, MozaProfile>();

        public Dictionary<string, GameSetting> gameSettings { get; set; } = new Dictionary<string, GameSetting>();

        // Methode zum Abrufen eines Profils für eine bestimmte Spiel/Fahrzeug-Kombination
        public MozaProfile GetProfile(string gameCarKey)
        {
            // Profil zurückgeben, falls Kombination existiert; ansonsten Standardprofil erstellen
            if (profileMapping.TryGetValue(gameCarKey, out MozaProfile profile))
            {
                return profile;
            }
            else
            {
                mozaAPI.ERRORCODE err = mozaAPI.ERRORCODE.NORMAL;
                // Standardprofil anlegen, falls kein Profil existiert
                MozaProfile standardProfil = new MozaProfile
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
                profileMapping[gameCarKey] = standardProfil;  // Standardprofil speichern
                foreach (var entry in mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err))
                {
                    SimHub.Logging.Current.Info($"MOZA EQUALIZER: Key = {entry.Key}, Value = {entry.Value}");
                }
                return standardProfil;
            }
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



        // Methode zum Setzen eines Profils für eine bestimmte Spiel/Fahrzeug-Kombination
        public void SetProfile(string gameCarKey, MozaProfile profile)
        {
            profileMapping[gameCarKey] = profile;
        }



        

    }
}
