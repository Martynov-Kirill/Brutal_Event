using BrutalEvent.Models;

using UnityEngine;

namespace BrutalEvent.Services.Abstract
{
    public abstract class LevelEvent
    {
        protected MasterEnviroment _enviroment;
        protected AnimationCurve oldAnimationCurve;
        protected readonly SpawnCurveGenerator _curveGenerator;

        public LevelEvent()
        {
            _enviroment = new MasterEnviroment();
            _curveGenerator = new SpawnCurveGenerator();
        }

        public abstract string GetEventName();
        
        public abstract LevelEvent CreateEvent();

        public abstract void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate);

        public virtual void OnLoadNewLevelCleanup(ref SelectableLevel newLevel)
        { }

        public virtual bool IsValid(ref SelectableLevel newLevel)
        {
            return true;
        }
    }
}
