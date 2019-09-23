// Copyright (c) 2019 Lordmau5

using System;
using System.IO;
using Newtonsoft.Json;

namespace GTA_SA_Chaos.util
{
    public class Config
    {
        public static Config Instance = new Config();

        [JsonIgnore]
        public bool Enabled;

        [JsonIgnore]
        public bool IsTwitchMode;

        [JsonIgnore]
        public int TwitchVotingMode = 0; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public int MainCooldown;
        public bool ContinueTimer = true;
        public string Seed;
        public bool CrypticEffects;
        public bool MainShowLastEffects;

        public bool TwitchAllowOnlyEnabledEffectsRapidFire;
        public int TwitchVotingTime;
        public int TwitchVotingCooldown;

        public bool TwitchShowLastEffects;
        public bool TwitchMajorityVoting = true;
        public bool Twitch3TimesCooldown;

        public string TwitchChannel;
        public string TwitchUsername;
        public string TwitchOAuthToken;

        public static int GetEffectDuration()
        {
            if (Instance.IsTwitchMode)
            {
                int cooldown = Instance.TwitchVotingCooldown + Instance.TwitchVotingTime;
                return Instance.Twitch3TimesCooldown ? cooldown * 3 : cooldown;
            }
            return Instance.MainCooldown * 3;
        }

        public static Config LoadConfig(string configPath)
        {
            try
            {
                var serializer = new JsonSerializer();

                using (var streamReader = new StreamReader(configPath))
                using (var reader = new JsonTextReader(streamReader))
                {
                    Instance = serializer.Deserialize<Config>(reader);

                    RandomHandler.SetSeed(Instance.Seed);

                    return Instance;
                }
            }
            catch (Exception)
            {
                // TODO Log that the loading has gone wrong.
            }

            return null;
        }

        public bool SaveConfig(string configPath)
        {
            try
            {
                var serializer = new JsonSerializer();

                using (var streamWriter = new StreamWriter(configPath))
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(writer, Instance);
                }

                return true;
            }
            catch (Exception)
            {
                // TODO Log exception
                return false;
            }
        }
    }
}
