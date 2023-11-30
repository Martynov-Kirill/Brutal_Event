using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrutalEvent.Enums;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class AllEvent : LevelEvent
    {
        EventFactory eventFactory;

        public AllEvent()
        {
            eventFactory = new EventFactory();
        }

        public override string GetEventName()
        {
            return $"<color=red>LEAVE THIS MOON! NOW!</color>";
        }

        public override LevelEvent CreateEvent()
        {
            return new AllEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            foreach (var gameEvent in Enum.GetValues(typeof(EventEnum)))
            {
                eventFactory.CreateEvent((EventEnum)gameEvent);
            }
        }
    }
}
