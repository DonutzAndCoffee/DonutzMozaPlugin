using System.Collections.Generic;

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
            public string CarModel { get; set; } = "-";

            public MozaProfile Clone()
            {
                return new MozaProfile
                {
                    CarModel = this.CarModel,
                    MotorFfbStrength = this.MotorFfbStrength,
                    MotorLimitAngle = this.MotorLimitAngle,
                    MotorNaturalDamper = this.MotorNaturalDamper,
                    MotorLimitWheelSpeed = this.MotorLimitWheelSpeed,
                    MotorSpringStrength = this.MotorSpringStrength,
                    MotorRoadSensitivity = this.MotorRoadSensitivity,
                    MotorNaturalInertia = this.MotorNaturalInertia,
                    MotorNaturalFriction = this.MotorNaturalFriction,
                    MotorSpeedDamping = this.MotorSpeedDamping,
                    MotorSpeedDampingStartPoint = this.MotorSpeedDampingStartPoint,
                    EQ10_EqualizerAmp7_5 = this.EQ10_EqualizerAmp7_5,
                    EQ15_EqualizerAmp13 = this.EQ15_EqualizerAmp13,
                    EQ25_EqualizerAmp22_5 = this.EQ25_EqualizerAmp22_5,
                    EQ40_EqualizerAmp39 = this.EQ40_EqualizerAmp39,
                    EQ60_EqualizerAmp55 = this.EQ60_EqualizerAmp55,
                    EQ100_EqualizerAmp100 = this.EQ100_EqualizerAmp100
                    };
                }

         }

        

        public class GameSetting
        {
            public string Name { get; set; }

            public bool activeProfileMapping { get; set; } = false;

            public bool activeCarMapping { get; set; } = false;

            public bool learnNewCars { get; set; } = false;

            public bool reversedFFB { get; set; } = false;

            public GameSetting(string gameName) { 
                this.Name = gameName;
                this.activeProfileMapping = false;
                this.activeCarMapping = false;
                this.learnNewCars = false;
                this.reversedFFB = false;
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
        public MozaProfile GetProfile(string gameName, string CarName, bool CarMapping, bool newCarLearning, string carModel, out string gameCarKey)
        {

            gameCarKey = "";

            if (CarMapping)
            {
                gameCarKey = gameName + "_" + CarName;
                if (!newCarLearning)
                {
                    if (profileMapping.TryGetValue(gameCarKey, out MozaProfile profil))
                    {
                        return profil;
                    }
                    else
                    {
                        gameCarKey = gameName;
                    }
                }
            }
            else
            {
                gameCarKey = gameName;
            }

            if (profileMapping.TryGetValue(gameCarKey, out MozaProfile profile))
            {
                return profile;
            }
            else
            {
                MozaProfile defaultProfile;
                
                if (profileMapping.TryGetValue(gameName, out MozaProfile gameProfile))
                {
                    defaultProfile = gameProfile.Clone();
                }
                else
                {
                    defaultProfile = getCurrentMozaSettings();
                }
                
                   
                profileMapping[gameCarKey] = defaultProfile;  // Standardprofil speichern    
                if (newCarLearning) 
                {
                    // create new game related profile if it does not exist, yet.
                    if (!profileMapping.TryGetValue(gameName, out MozaProfile notUsed))
                    {
                        MozaProfile defaultGameProfile = getCurrentMozaSettings();
                        profileMapping[gameName] = defaultGameProfile;
                        //profileMapping[gameName] = getCurrentMozaSettings();
                        //profileMapping[gameName].CarModel = "-";
                    }
                    if (profileMapping.TryGetValue(gameCarKey, out MozaProfile existingProfile))
                    {
                        existingProfile.CarModel = carModel;
                    }
                }
                return defaultProfile;
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
                EQ100_EqualizerAmp100 = mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err)["EqualizerAmp100"],
                CarModel = "-"
            };
            foreach (var entry in mozaAPI.mozaAPI.getMotorEqualizerAmp(ref err))
            {
                SimHub.Logging.Current.Info($"MOZA EQUALIZER: Key = {entry.Key}, Value = {entry.Value}");
            }
            SimHub.Logging.Current.Info($"MOZA REV LEDs: {string.Join(", ", mozaAPI.mozaAPI.getSteeringWheelShiftIndicatorLightRpm(ref err))}");
            
            foreach (var entry in mozaAPI.mozaAPI.getSteeringWheelShiftIndicatorLightRpm(ref err))
            {
                SimHub.Logging.Current.Info($"MOZA REV LEDs: {entry}");
            }
            return currentMozaSettings;
        }

        // Methode zum Setzen eines Profils für eine bestimmte Spiel/Fahrzeug-Kombination
        public void SetProfile(string gameCarKey, MozaProfile profile)
        {
            profileMapping[gameCarKey] = profile;
        }
    }
}
