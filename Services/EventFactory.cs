using System;
using System.Collections.Generic;
using BrutalEvent.Enums;
using BrutalEvent.Services.Abstract;
using BrutalEvent.Services.Abstract.Interface;
using BrutalEvent.Services.Events;

namespace BrutalEvent.Services
{
    public class EventFactory : IEventFactory
    {
        private List<EventEnum> customEventOrder = new List<EventEnum>();
        private int _customCurrentIndex;
        private int _currentIndex;
        private Random random = new Random();
        
        public EventFactory()
        {
            
        }

        public EventEnum EventEnum { get; set; } = EventEnum.None;

        public LevelEvent GetRandomEvent()
        {
            EventEnum = (EventEnum)random.Next(Enum.GetValues(typeof(EventEnum)).Length);
            return CreateEvent(EventEnum);
        }

        public LevelEvent GetEventInOrder()
        {
            EventEnum = (EventEnum)Enum.GetValues(typeof(EventEnum)).GetValue(_currentIndex);
            _currentIndex = (_currentIndex + 1) % Enum.GetValues(typeof(EventEnum)).Length;
            return CreateEvent(EventEnum);
        }

        public LevelEvent GetEventInCustomOrder()
        {
            EventEnum = customEventOrder[_customCurrentIndex];
            _customCurrentIndex = (_customCurrentIndex + 1) % customEventOrder.Count;
            return CreateEvent(EventEnum);
        }

        public LevelEvent CreateEvent(EventEnum gameEvent)
        {
            LevelEvent eventInstance = gameEvent switch
            {
                EventEnum.None => new NoneEvent(),
                EventEnum.All => new AllEvent(),
                EventEnum.Chaos => new ChaosEvent(),
                EventEnum.Unfair => new UnFairEvent(),
                EventEnum.Landmine => new LandMineEvent(),
                EventEnum.Kleptomania => new KleptomaniaEvent(),
                EventEnum.Arachnophobia => new ArachnophobiaEvent(),
                EventEnum.SecuritySystem => new SecurityEvent(),
                EventEnum.AllSnareFlea => new AllSnareEvent(),
                EventEnum.BrackenAndCoil => new BrackerAndCoilEvent(),
                EventEnum.BlobApocalypsis => new BlobApocalypsis(),
                EventEnum.WhoLetTheDogsOut => new WhoLetTheDogEvent(),
                EventEnum.DidYouSeeHer => new DidYouSeeHerEvent(),
                EventEnum.Tremors => new TremorsEvent(),
                EventEnum.GoToRend => new GoToRendEvent(),
                EventEnum.Delivery => new DeliveryEvent(),
                EventEnum.LetsFly => new LetsFlyEvent(),
                EventEnum.ReplaceItems => new ReplaceItemsEvent(),
                EventEnum.ResetEvent => new ResetEvent(),
                _ => new NoneEvent(),
            };

            return eventInstance.CreateEvent();
        }
    }
}
