using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class BrackerAndCoilEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "THE HUNT BEGIN";
        }

        public override LevelEvent CreateEvent() => new BrackerAndCoilEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<FlowermanAI>(newLevel, currentRate, 1.5f);
            _enviroment.GenerateEnemiesEvent<SpringManAI>(newLevel, currentRate, 1.5f);
        }
    }
}
