using MetriqusSdk;
using System.Collections.Generic;
namespace MetriqusSdk
{
    public class MetriqusItemUsedEvent : MetriqusCustomEvent
    {
        public string ItemName;
        public float? Amount;
        public string ItemType;
        public string ItemRarity;
        public string ItemClass;
        public string ItemCategory;
        public string Reason;

        public MetriqusItemUsedEvent() : base(MetriqusEventKeys.EventItemUsed)
        {

        }

        public MetriqusItemUsedEvent(List<TypedParameter> parameters) : base(MetriqusEventKeys.EventItemUsed, parameters)
        {

        }

        public override List<TypedParameter> GetParameters()
        {
            List<TypedParameter> copiedParams = new(Parameters);

            if (!string.IsNullOrEmpty(ItemName))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterItemName, ItemName));

            if (Amount.HasValue)
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterAmount, Amount.Value));

            if (!string.IsNullOrEmpty(ItemType))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterItemType, ItemType));

            if (!string.IsNullOrEmpty(ItemRarity))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterItemRarity, ItemRarity));

            if (!string.IsNullOrEmpty(ItemClass))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterItemClass, ItemClass));

            if (!string.IsNullOrEmpty(ItemCategory))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterItemCategory, ItemCategory));

            if (!string.IsNullOrEmpty(Reason))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterReason, Reason));

            return copiedParams;
        }
    }
}