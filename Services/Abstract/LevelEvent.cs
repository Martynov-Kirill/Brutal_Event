using BrutalEvent.Models;

namespace BrutalEvent.Services.Abstract
{
    public abstract class LevelEvent
    {
        public abstract string GetEventName();
        
        public abstract LevelEvent CreateEvent();

        public abstract void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate);

        public virtual void OnLoadNewLevelCleanup(ref SelectableLevel newLevel)
        { }

        public virtual bool IsValid(ref SelectableLevel newLevel)
        {
            return true;
        }
    }
}
