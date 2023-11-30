using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using Object = UnityEngine.Object;

namespace BrutalEvent.Services.Events
{
    internal class LetsFlyEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"ITS A PLANE! NO ITS A FLY";
        }

        public override LevelEvent CreateEvent()
        {
            return new LetsFlyEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            int item = 9;
            Terminal val = Object.FindObjectOfType<Terminal>();
            for (int i = 0; i < 5; i++)
            {
                val.orderedItemsFromTerminal.Add(item);
            }
        }
    }
}
