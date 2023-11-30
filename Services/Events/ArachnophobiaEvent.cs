using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class ArachnophobiaEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"ARACHNOPHOBIA!";
        }

        public override LevelEvent CreateEvent() => new ArachnophobiaEvent();

        /// <summary>
        /// Маленькие хорошие паучки
        /// </summary>
        /// <param name="newLevel"></param>
        /// <param name="configs"></param>
        /// <param name="currentRate"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<SandSpiderAI>(newLevel, currentRate, 2.0f);
        }
    }
}
