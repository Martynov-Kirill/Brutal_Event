using BepInEx.Configuration;

namespace BrutalEvent.Models
{
    public class Config
    {
        public ConfigEntry<float> MinScrap { get; set; }
        public ConfigEntry<float> MaxScrap { get; set; }
        public ConfigEntry<float> LimitRate { get; set; }
        public ConfigEntry<float> MaxRate { get; set; }
        public ConfigEntry<float> RarityMin { get; set; }
        public ConfigEntry<float> RarityMax { get; set; }
        public ConfigEntry<int> AddCredits { get; set; }

        public ConfigEntry<int> StartingQuota { get; set; }
        public ConfigEntry<int> StartingCredits { get; set; }
        public ConfigEntry<float> BaseIncrease { get; set; }
        public ConfigEntry<float> Multiplier { get; set; }
        public ConfigEntry<int> DeadlineDaysAmount { get; set; }
    }
}
