using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using Object = UnityEngine.Object;

namespace BrutalEvent.Services.Events
{
    internal class LetsFlyEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"<color=green>ITS A FLY?! NO ITS A PLANE!</color>";
        }

        public override LevelEvent CreateEvent() => new LetsFlyEvent();

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
