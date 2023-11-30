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
        private int customCurrentIndex;
        private int currentIndex;
        private Random random = new Random();

        public EventFactory()
        {
            
        }

        public LevelEvent GetRandomEvent()
        {
            var gameEvent = (EventEnum)random.Next(Enum.GetValues(typeof(EventEnum)).Length);
            return CreateEvent(gameEvent);
        }

        public LevelEvent GetEventInOrder()
        {
            var gameEvent = (EventEnum)Enum.GetValues(typeof(EventEnum)).GetValue(currentIndex);
            currentIndex = (currentIndex + 1) % Enum.GetValues(typeof(EventEnum)).Length;
            return CreateEvent(gameEvent);
        }

        public LevelEvent GetEventInCustomOrder()
        {
            EventEnum gameEvent = customEventOrder[customCurrentIndex];
            customCurrentIndex = (customCurrentIndex + 1) % customEventOrder.Count;
            return CreateEvent(gameEvent);
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
