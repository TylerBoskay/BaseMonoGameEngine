using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Manages events via a queueing system.
    /// </summary>
    public sealed class EventManager : IUpdateable, ICleanup
    {
        #region Singleton Fields

        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static EventManager instance = null;

        #endregion
        
        private readonly Dictionary<string, List<IEventHandler>> ListenerTable = new Dictionary<string, List<IEventHandler>>();
        private readonly Queue<IEvent> EventQueue = new Queue<IEvent>();

        /// <summary>
        /// The maximum time allowed to process the event queue.
        /// </summary>
        public double MaxQueueProcessTime = double.MaxValue;

        private EventManager()
        {
            
        }

        public void CleanUp()
        {
            ListenerTable.Clear();
            EventQueue.Clear();

            instance = null;
        }

        /// <summary>
        /// Adds a listener for an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The listener.</param>
        public void AddListener(string eventName, IEventHandler listener)
        {
            //Add a new entry if it doesn't exist
            if (ListenerTable.ContainsKey(eventName) == false)
            {
                ListenerTable.Add(eventName, new List<IEventHandler>());
            }

            //Add the event handler to the listener table for this event
            ListenerTable[eventName].Add(listener);
        }

        /// <summary>
        /// Removes a listener from an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(string eventName, IEventHandler listener)
        {
            //If there is no event with this name, return
            if (ListenerTable.ContainsKey(eventName) == false)
                return;
            
            //Remove this handler from the table
            ListenerTable[eventName].Remove(listener);
        }

        /// <summary>
        /// Immediately triggers an event.
        /// </summary>
        /// <param name="evt">The event to trigger.</param>
        public void TriggerEvent(IEvent evt)
        {
            //Handle null events
            if (evt == null)
            {
                Debug.LogError($"Attempting to trigger a null event!");
                return;
            }

            string eventName = evt.Name;

            //Return if there are no listeners subscribed to the event
            if (ListenerTable.ContainsKey(eventName) == false)
            {
                Debug.LogWarning($"There are no listeners for {eventName}!");
                return;
            }

            List<IEventHandler> handlerList = ListenerTable[eventName];
            for (int i = 0; i < handlerList.Count; i++)
            {
                //Make each event handler handle the event
                IEventHandler handler = handlerList[i];

                handler.HandleEvent(evt);
            }
        }

        /// <summary>
        /// Queues an event to be triggered.
        /// </summary>
        /// <param name="evt">The event to queue.</param>
        public void QueueEvent(IEvent evt)
        {
            //Handle null events
            if (evt == null)
            {
                Debug.LogError($"Attempting to queue a null event!");
                return;
            }

            string eventName = evt.Name;

            //Return if there are no listeners subscribed to the event
            if (ListenerTable.ContainsKey(eventName) == false)
            {
                Debug.LogWarning($"There are no listeners for {eventName}!");
                return;
            }

            //Queue the event
            EventQueue.Enqueue(evt);
        }

        public void Update()
        {
            double elapsedTime = 0d;

            //Trigger events in the queue
            while (EventQueue.Count > 0)
            {
                //Get out of the loop if we exceeded our max process time
                if (elapsedTime >= MaxQueueProcessTime)
                {
                    break;
                }

                //Trigger the event
                IEvent evt = EventQueue.Dequeue();
                TriggerEvent(evt);

                //Increment time spent processing
                elapsedTime += Time.ElapsedMilliseconds;
            }
        }
    }
}
