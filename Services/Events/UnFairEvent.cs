using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class UnFairEvent : LevelEvent
    {
        public override string GetEventName()
        {
            throw new NotImplementedException();
        }

        public override LevelEvent CreateEvent()
        {
            throw new NotImplementedException();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs)
        {
            throw new NotImplementedException();
        }
    }
}
