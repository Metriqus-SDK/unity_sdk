namespace MetriqusSdk
{
    internal interface IEventQueueController
    {
        void AddEvent(Event _event, bool sendImmediately = false);
    }
}