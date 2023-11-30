using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class BlobApocalypsis : LevelEvent
    {
        public override string GetEventName()
        {
            return "BLOBAPOCALYPSIS";
        }

        public override LevelEvent CreateEvent() => new BlobApocalypsis();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<BlobAI>(newLevel, currentRate, 2.0f);
        }
    }
}
