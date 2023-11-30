using System.Collections.Generic;
using BepInEx.Logging;

namespace BrutalEvent.Models
{
    public static class Configuration
    {
        public static bool loaded { get; set; }

        public static List<SelectableLevel> levelsModified { get; set; } = new List<SelectableLevel>();

        public static Dictionary<SelectableLevel, float> RarityLevelValue { get; set; } =
            new Dictionary<SelectableLevel, float>();

        public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> Enemies { get; set; } =
            new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

        public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> OriginalEnemiesRarities 
        { get; set; } = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

        public static ManualLogSource mls { get; set; }
    }
}