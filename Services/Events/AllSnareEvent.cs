using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class AllSnareEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"WATCH YOUR HEAD!";
        }

        public override LevelEvent CreateEvent() => new AllSnareEvent();

        /// <summary>
        /// Сороконожки на голову
        /// </summary>
        /// <param name="newLevel"></param>
        /// <param name="configs"></param>
        /// <param name="currentRate"></param>
        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<CentipedeAI>(newLevel, currentRate, 2.0f);
        }

        public override void OnLoadNewLevelCleanup(ref SelectableLevel newLevel)
        {
            newLevel.enemySpawnChanceThroughoutDay = oldAnimationCurve;

        }
    }
}
