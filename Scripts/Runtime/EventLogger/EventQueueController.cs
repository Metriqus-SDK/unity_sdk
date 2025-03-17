using MetriqusSdk.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk
{
    /// <summary>
    /// Stores event in a queue. Responsible for storing and flushing queue when conditions are met.
    /// </summary>
    internal class EventQueueController : IEventQueueController
    {
        private const string LastFlushTimeKey = "metriqus_event_last_flush_time";
        private const string CurrentEventsKey = "metriqus_current_events";
        private const string EventsToSendKey = "metriqus_events_to_send";

        private IStorage storage;

        private Queue<EventQueue> eventsToSend = new();

        private EventQueue eventQueue;

        public EventQueue Events => eventQueue;

        public EventQueueController(IStorage storage)
        {
            this.storage = storage;

            LoadEventsToSend();

            try
            {
                // get saved events if any
                bool savedEventKeyExist = storage.CheckKeyExist(CurrentEventsKey);
                if (savedEventKeyExist)
                {
                    string eventsData = storage.LoadData(CurrentEventsKey);

                    eventQueue = new EventQueue(eventsData);
                }
                else
                {
                    eventQueue = new();
                }

            }
            catch (Exception)
            {
                eventQueue = new();
            }
        }

        /// <summary>
        /// Add event to queue
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(Event _event)
        {
            if (eventQueue == null)
            {
                eventQueue = new();
            }

            // add new event to queue
            eventQueue.Add(_event);

            // convert queue to json
            string json = eventQueue.Serialize();

            // save new json to local
            storage.SaveData(CurrentEventsKey, json);

            CheckQueueStatus();
        }

        /// <summary>
        /// Check is event queue ready to send server
        /// </summary>
        private void CheckQueueStatus()
        {
            try
            {
                DateTime currentTime = DateTime.UtcNow;
                DateTime lastFlushTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                bool isLastFlushTimeKeyExist = storage.CheckKeyExist(LastFlushTimeKey);
                if (isLastFlushTimeKeyExist)
                {
                    string lastFlushTimeStr = storage.LoadData(LastFlushTimeKey);
                    lastFlushTime = MetriqusUtils.ParseDate(lastFlushTimeStr);
                }

                var remoteSettings = Metriqus.GetMetriqusRemoteSettings();

                if (eventQueue.Events.Count >= remoteSettings.MaxEventBatchCount
                    || (currentTime.Subtract(lastFlushTime).TotalSeconds) > (remoteSettings.MaxEventStoreSeconds))
                {
                    Metriqus.DebugLog("Checked sending events. EventQueue count : " + eventQueue.Events.Count + ", passedSeconds: " + (currentTime.Subtract(lastFlushTime).TotalSeconds));

                    // save last event send time
                    storage.SaveData(LastFlushTimeKey, MetriqusUtils.ConvertDateToString(currentTime));

                    // add event queue to event to send
                    eventsToSend.Enqueue(eventQueue);

                    // save events to send
                    SaveEventsToSend();

                    // reset current events
                    storage.SaveData(CurrentEventsKey, "[]"); // save empty queue

                    eventQueue = new(); // create new queue and old one stored in events to send

                    ProcessEvents();
                }
            }
            catch (Exception e)
            {
                if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog(e.ToString(), LogType.Error);
            }
        }

        /// <summary>
        /// Flush next event with backoff in queue if any 
        /// </summary>
        private async void ProcessEvents()
        {
            if (eventsToSend.Count < 1) return;

            //Metriqus.DebugLog("ProcessEvents, count: " + eventsToSend.Count);

            try
            {
                // queue is ready to send
                // flust with back off 3 times
                _ = await Backoff.DoAsync(
                    id: "flush_events",
                    action: Flush,
                    validateResult: result => result == true,
                    onComplete: (result, successful) =>
                    {
                        if (!successful)
                        {
                        }
                    },
                    maxRetries: 4,
                    maxDelayMilliseconds: 4000,
                    delayMilliseconds: 1000
                );
            }
            catch (Exception ex)
            {
                Metriqus.DebugLog("Event Send Process failed. Error: " + ex.Message, LogType.Error);
            }
        }

        /// <summary>
        /// Post events to server as json
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Flush()
        {
            var selectedEventQueue = eventsToSend.Peek();

            bool result = await EventRequestSender.PostEventBatch(selectedEventQueue.Serialize());

            // if post request successful clear existing events
            if (result)
            {
                Metriqus.EnqueueCallback(async () =>
                {
                    eventsToSend.Dequeue();
                    SaveEventsToSend(); // delete sended batch from queue and save it

                    var remoteSettings = Metriqus.GetMetriqusRemoteSettings();
                    await Task.Delay(TimeSpan.FromSeconds(remoteSettings.SendEventIntervalSeconds));

                    // sending events successful send next batch if any
                    ProcessEvents();
                });
            }

            return result;
        }

        private void SaveEventsToSend()
        {
            string json = "[";
            foreach (var item in eventsToSend)
            {
                if (item != null)
                {
                    //Metriqus.DebugLog("eventsToSend item: " + item);

                    json += $"\n{item.Serialize()}, ";
                }
            }

            if (json.EndsWith(", "))
            {
                json = json[..^2];
            }

            json += "\n]";

            storage.SaveData(EventsToSendKey, json);
        }

        private void LoadEventsToSend()
        {
            bool isEventsToSendKeyExist = storage.CheckKeyExist(EventsToSendKey);
            if (isEventsToSendKeyExist)
            {
                try
                {
                    string data = storage.LoadData(EventsToSendKey);

                    eventsToSend = new();

                    var jsonNode = JSON.Parse(data);

                    if (jsonNode == null) return;

                    var eventQueuesArray = jsonNode.AsArray;

                    foreach (var _eventQueue in eventQueuesArray.Childs)
                    {
                        var result = new EventQueue(_eventQueue.AsArray);

                        if (result != null)
                        {
                            eventsToSend.Enqueue(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Metriqus.DebugLog("LoadEventsToSend failed :" + ex.Message, LogType.Error);
                    eventsToSend = new();
                }
            }
        }
    }
}