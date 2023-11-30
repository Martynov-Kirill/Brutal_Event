using System;
using System.Collections.Generic;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

using UnityEngine;

namespace BrutalEvent.Services.Events
{
    internal class AllSnareEvent : LevelEvent
    {
        private MasterEnviroment _enviroment;
        private AnimationCurve oldAnimationCurve;
        private List<int> rarities = new List<int>();
        private int oldMaxCount;

        public AllSnareEvent()
        {
            _enviroment = new MasterEnviroment();
        }

        public override string GetEventName()
        {
            return $"WATCH YOUR HEAD!";
        }

        public override LevelEvent CreateEvent()
        {
            return new AllSnareEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<CentipedeAI>(newLevel, currentRate);
        }

        public override void OnLoadNewLevelCleanup(ref SelectableLevel newLevel)
        {
            base.OnLoadNewLevelCleanup(ref newLevel);
        }
    }
}
