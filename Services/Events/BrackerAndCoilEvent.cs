using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class BrackerAndCoilEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=red>THE HUNT BEGIN!</color>";
        }

        public override LevelEvent CreateEvent() => new BrackerAndCoilEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.ResetEnemiesRarity(newLevel);
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<FlowermanAI>(newLevel, currentRate, 2.0f);
            _enviroment.GenerateEnemiesEvent<CrawlerAI>(newLevel, currentRate, 2.0f);
        }
    }
}
