using BrutalEvent.Enums;

namespace BrutalEvent.Services.Abstract.Interface
{
    public interface IEventFactory
    {
        LevelEvent CreateEvent(EventEnum eventEnum);
    }
}
