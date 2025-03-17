using System;
using System.Collections.Generic;

namespace MetriqusSdk
{
    internal class EventQueue
    {
        private Queue<Event> events = new();
        public Queue<Event> Events => events;

        public EventQueue()
        {
            events = new();
        }

        public EventQueue(string jsonString)
        {
            var jsonNode = JSON.Parse(jsonString);

            if (jsonNode == null)
            {
                events = new();
            }
            else
            {
                var array = jsonNode.AsArray;

                events = Parse(array);
            }
        }

        public EventQueue(JSONArray array)
        {
            events = Parse(array);
        }

        public void Add(Event e)
        {
            events.Enqueue(e);
        }

        public string Serialize()
        {
            string result = "[";
            foreach (var _event in events)
            {
                result += _event.ToJson();
                result += ", ";
            }

            if (result.EndsWith(", "))
            {
                result = result[..^2];
            }

            result += "]";
            return result;
        }

        public static Queue<Event> Parse(JSONArray array)
        {
            try
            {
                var queue = new Queue<Event>();

                if (array == null)
                {
                    return new();
                }

                foreach (var _event in array.Childs)
                {
                    var result = Event.ParseJson(_event);

                    if (result != null)
                    {
                        queue.Enqueue(result);
                    }
                }

                return queue;
            }
            catch (Exception e)
            {
                Metriqus.DebugLog("Parsing Event Queue failed: " + e.Message, UnityEngine.LogType.Error);
                return new();
            }
        }
    }
}