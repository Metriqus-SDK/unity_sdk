using MetriqusSdk.Storage;
using System.Collections.Generic;

namespace MetriqusSdk
{
    internal static class MetriqusLogger
    {
        private static IEventQueueController eventQueue;
        public static void Init(IStorage storage)
        {
            eventQueue = new EventQueueController(storage);
        }

        public static void LogEvent(string name, string parameterName, string parameterValue)
        {
            eventQueue.AddEvent(new Event(name, new[] { new TypedParameter(parameterName, parameterValue) }));
        }

        public static void LogEvent(string name, string parameterName, double parameterValue)
        {
            eventQueue.AddEvent(new Event(name, new[] { new TypedParameter(parameterName, (float)parameterValue) }));
        }

        public static void LogEvent(string name, string parameterName, long parameterValue)
        {
            eventQueue.AddEvent(new Event(name, new[] { new TypedParameter(parameterName, (int)parameterValue) }));
        }

        public static void LogEvent(string name, string parameterName, int parameterValue)
        {
            eventQueue.AddEvent(new Event(name, new[] { new TypedParameter(parameterName, parameterValue) }));
        }

        public static void LogEvent(string name, string parameterName, bool parameterValue)
        {
            eventQueue.AddEvent(new Event(name, new[] { new TypedParameter(parameterName, parameterValue) }));
        }

        public static void LogEvent(string name)
        {
            eventQueue.AddEvent(new Event(name, null));
        }

        public static void LogEvent(string name, params TypedParameter[] parameters)
        {
            eventQueue.AddEvent(new Event(name, parameters));
        }

        public static void LogEvent(string name, List<TypedParameter> parameters)
        {
            eventQueue.AddEvent(new Event(name, parameters));
        }

        public static void LogEvent(Package package)
        {
            eventQueue.AddEvent(new Event(package));
        }
    }
}