using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class DidYouSeeHerEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"<color=red>DID YOU SEE HER?!</color>";
        }

        public override LevelEvent CreateEvent() => new DidYouSeeHerEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<DressGirlAI>(newLevel, currentRate);
        }
    }
}
