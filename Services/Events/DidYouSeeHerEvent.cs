using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class DidYouSeeHerEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"DID YOU SEE HER?!";
        }

        public override LevelEvent CreateEvent() => new DidYouSeeHerEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<DressGirlAI>(newLevel, currentRate);
        }
    }
}
