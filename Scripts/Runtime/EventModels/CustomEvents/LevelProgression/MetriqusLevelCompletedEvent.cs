using System.Collections.Generic;

namespace MetriqusSdk
{
    public class MetriqusLevelCompletedEvent : MetriqusCustomEvent
    {
        public int? LevelNumber { get; set; }
        public string LevelName { get; set; }
        public string Map { get; set; }
        public float? Duration { get; set; }

        public float? LevelProgress { get; set; }

        public int? LevelReward { get; set; }
        public int? LevelReward1 { get; set; }
        public int? LevelReward2 { get; set; }

        public MetriqusLevelCompletedEvent() : base(MetriqusEventKeys.EventLevelCompleted)
        {
            
        }

        public MetriqusLevelCompletedEvent(List<TypedParameter> parameters) : base(MetriqusEventKeys.EventLevelCompleted, parameters)
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

            if (Duration.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterDuration, Duration.Value));

            if (LevelProgress.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelProgress, LevelProgress.Value));

            if (LevelReward.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelReward, LevelReward.Value));

            if (LevelReward1.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelReward1, LevelReward1.Value));

            if (LevelReward2.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterLevelReward2, LevelReward2.Value));

            return copiedParams;
        }
    }
}