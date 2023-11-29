using BepInEx.Configuration;

namespace BrutalEvent.Models
{
    public class Configuration
    {
        public ConfigEntry<float> minScrap { get; set; }
        public ConfigEntry<float> maxScrap { get; set; }
        public ConfigEntry<float> limitRate { get; set; }
        public ConfigEntry<float> maxRate { get; set; }
        public ConfigEntry<float> rarityMin { get; set; }
        public ConfigEntry<float> rarityMax { get; set; }
        public ConfigEntry<int> startCredits { get; set; }
    }
}
