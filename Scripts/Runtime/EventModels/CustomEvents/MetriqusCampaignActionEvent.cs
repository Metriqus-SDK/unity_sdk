using MetriqusSdk;
using System.Collections.Generic;
namespace MetriqusSdk
{
    public enum MetriqusCampaignActionType
    {
        Show, Click, Close, Purchase
    }
    public class MetriqusCampaignActionEvent : MetriqusCustomEvent
    {
        public string CampaignId { get; set; }
        public string VariantId { get; set; }
        public MetriqusCampaignActionType MetriqusCampaignAction { get; set; } = MetriqusCampaignActionType.Show;

        public MetriqusCampaignActionEvent(string campaignId, string variantId, MetriqusCampaignActionType action) : base(MetriqusEventKeys.EventCampaignDetails)
        {
            this.CampaignId = campaignId;
            this.VariantId = variantId;
            this.MetriqusCampaignAction = action;
        }

        public MetriqusCampaignActionEvent(string campaignId, string variantId, MetriqusCampaignActionType action, List<TypedParameter> parameters) : base(MetriqusEventKeys.EventCampaignDetails, parameters)
        {
            this.CampaignId = campaignId;
            this.VariantId = variantId;
            this.MetriqusCampaignAction = action;
        }

        public override List<TypedParameter> GetParameters()
        {
            List<TypedParameter> copiedParams = new(Parameters);

            if (!string.IsNullOrEmpty(CampaignId))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterCampaignID, CampaignId));
            
            if (!string.IsNullOrEmpty(VariantId))
                copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterVariantId, VariantId));

            copiedParams.Add(new TypedParameter(MetriqusEventKeys.ParameterCampaignAction, GetActionString(MetriqusCampaignAction)));

            return copiedParams;
        }

        private static string GetActionString(MetriqusCampaignActionType action)
        {
            switch (action)
            {
                case MetriqusCampaignActionType.Show:
                    return "show";
                case MetriqusCampaignActionType.Click:
                    return "click";
                case MetriqusCampaignActionType.Close:
                    return "close";
                case MetriqusCampaignActionType.Purchase:
                    return "purchase";
                default:
                    return "show";
            }
        }
    }
}