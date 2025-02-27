using System.Collections.Generic;

namespace MetriqusSdk
{
    public class MetriqusLevelStartedEvent : MetriqusCustomEvent
    {
        public int? LevelNumber { get; set; }
        public string LevelName { get; set; }
        public string Map { get; set; }

        public MetriqusLevelStartedEvent() : base(MetriqusEventKeys.EventLevelStart)
        {
            
        }

        public MetriqusLevelStartedEvent(List<TypedParameter> parameters) : base(MetriqusEventKeys.EventLevelStart, parameters)
        {
            
        }

        public override List<TypedParameter> GetParameters()
        {
            List<TypedParameter> copiedParams = new(Parameters);

            if (LevelNumber.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelNumber, LevelNumber.Value));

            // ADD OTHERS HERE
            if (!string.IsNullOrEmpty(LevelName))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelName, LevelName));

            if (!string.IsNullOrEmpty(Map))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterMap, Map));

            return copiedParams;
        }
    }
}