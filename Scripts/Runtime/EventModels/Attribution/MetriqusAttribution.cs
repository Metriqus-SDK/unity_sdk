#if UNITY_IOS
#elif UNITY_ANDROID
using System.Collections.Generic;
#endif
using System;
using UnityEngine;
using System.Linq;

namespace MetriqusSdk
{
    /// <summary>
    /// Represents attribution data for tracking ad performance, supporting both iOS and Android platforms.
    /// </summary>
    internal class MetriqusAttribution
    {
        public string Raw { get; set; }
#if UNITY_IOS
        /// <summary>
        /// Indicates whether attribution was successful.
        /// </summary>
        public bool Attribution { get; set; }

        /// <summary>
        /// The organization ID associated with the attribution data.
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// The campaign ID associated with the attribution data.
        /// </summary>
        public long? CampaignId { get; set; }

        /// <summary>
        /// The type of conversion recorded (e.g., install, purchase).
        /// </summary>
        public string ConversionType { get; set; }

        /// <summary>
        /// The date when the ad was clicked.
        /// </summary>
        public string ClickDate { get; set; }

        /// <summary>
        /// The type of claim associated with the attribution.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// The ad group ID associated with the attribution.
        /// </summary>
        public long? AdGroupId { get; set; }

        /// <summary>
        /// The country or region where the ad was viewed or interacted with.
        /// </summary>
        public string CountryOrRegion { get; set; }

        /// <summary>
        /// The keyword ID used in the attribution data.
        /// </summary>
        public long? KeywordId { get; set; }

        /// <summary>
        /// The specific ad ID that contributed to the attribution.
        /// </summary>
        public long? AdId { get; set; }

        /// <summary>
        /// Parses a JSON string into a <see cref="MetriqusAttribution"/> object.
        /// </summary>
        /// <param name="attributionJsonString">The JSON string containing attribution data.</param>
        /// <returns>A parsed <see cref="MetriqusAttribution"/> object, or null if parsing fails.</returns>
        public static MetriqusAttribution Parse(string attributionJsonString)
        {
            var jsonNode = JSON.Parse(attributionJsonString);

            if (jsonNode == null)
            {
                return null;
            }

            MetriqusAttribution mAttribution = new();

            mAttribution.Raw = attributionJsonString.Replace('"', ' ');

            try { mAttribution.Attribution = bool.TryParse(MetriqusJSON.GetJsonString(jsonNode, "attribution"), out var val) ? val : false; } catch { mAttribution.Attribution = false; }

            try { mAttribution.OrgId = long.TryParse(MetriqusJSON.GetJsonString(jsonNode, "orgId"), out var val) ? val : null; } catch { mAttribution.OrgId = null; }

            try { mAttribution.CampaignId = long.TryParse(MetriqusJSON.GetJsonString(jsonNode, "campaignId"), out var val) ? val : null; } catch { mAttribution.CampaignId = null; }

            try { mAttribution.ConversionType = MetriqusJSON.GetJsonString(jsonNode, "conversionType"); } catch { mAttribution.ConversionType = null; }

            try { mAttribution.ClickDate = MetriqusJSON.GetJsonString(jsonNode, "clickDate"); } catch { mAttribution.ClickDate = null; }

            try { mAttribution.ClaimType = MetriqusJSON.GetJsonString(jsonNode, "claimType"); } catch { mAttribution.ClaimType = null; }

            try { mAttribution.AdGroupId = long.TryParse(MetriqusJSON.GetJsonString(jsonNode, "adGroupId"), out var val) ? val : null; } catch { mAttribution.AdGroupId = null; }

            try { mAttribution.CountryOrRegion = MetriqusJSON.GetJsonString(jsonNode, "countryOrRegion"); } catch { mAttribution.CountryOrRegion = null; }

            try { mAttribution.KeywordId = long.TryParse(MetriqusJSON.GetJsonString(jsonNode, "keywordId"), out var val) ? val : null; } catch { mAttribution.KeywordId = null; }

            try { mAttribution.AdId = long.TryParse(MetriqusJSON.GetJsonString(jsonNode, "adId"), out var val) ? val : null; } catch { mAttribution.AdId = null; }


            return mAttribution;
        }

#elif UNITY_ANDROID
        /// <summary>
        /// The source of the attribution (e.g., Google, Facebook).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The marketing medium (e.g., CPC, organic).
        /// </summary>
        public string Medium { get; set; }

        /// <summary>
        /// The campaign name associated with the attribution.
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// The search term that led to the attribution.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Additional content metadata related to the ad.
        /// </summary>
        public string Content { get; set; }

        public List<TypedParameter> Params { get; set; } = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusAttribution"/> class.
        /// </summary>
        public MetriqusAttribution() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusAttribution"/> class using a referrer URL.
        /// </summary>
        /// <param name="referrerUrl">The referrer URL containing attribution parameters.</param>
        public MetriqusAttribution(string referrerUrl)
        {
            var queryDict = MetriqusUtils.ParseAndSanitize(referrerUrl);
            ParseDict(queryDict, referrerUrl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusAttribution"/> class using a dictionary of attribution data.
        /// </summary>
        /// <param name="dictAttributionData">A dictionary containing attribution key-value pairs.</param>
        public MetriqusAttribution(Dictionary<string, string> dictAttributionData, string referrerUrl)
        {
            ParseDict(dictAttributionData, referrerUrl);
        }

        /// <summary>
        /// Parses and assigns values from a dictionary of attribution data.
        /// </summary>
        /// <param name="dictAttributionData">A dictionary containing attribution key-value pairs.</param>
        private void ParseDict(Dictionary<string, string> dictAttributionData, string referrerUrl)
        {
            if (dictAttributionData == null)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(referrerUrl)) return;

            this.Raw = referrerUrl.Replace('"', ' ');

            this.Source = MetriqusUtils.TryGetValue(dictAttributionData, MetriqusUtils.KeySource);
            this.Medium = MetriqusUtils.TryGetValue(dictAttributionData, MetriqusUtils.KeyMedium);
            this.Campaign = MetriqusUtils.TryGetValue(dictAttributionData, MetriqusUtils.KeyCampaign);
            this.Term = MetriqusUtils.TryGetValue(dictAttributionData, MetriqusUtils.KeyTerm);
            this.Content = MetriqusUtils.TryGetValue(dictAttributionData, MetriqusUtils.KeyContent);

            foreach (var item in dictAttributionData)
            {
                if (item.Key == MetriqusUtils.KeySource
                    || item.Key == MetriqusUtils.KeyMedium
                    || item.Key == MetriqusUtils.KeyCampaign
                    || item.Key == MetriqusUtils.KeyTerm
                    || item.Key == MetriqusUtils.KeyContent)
                {
                    continue;
                }

                if (Params == null)
                    this.Params = new();

                this.Params.Add(new TypedParameter(item.Key, item.Value));
            }

            if (string.IsNullOrEmpty(this.Source) && this.Params.Any(s => s.Name == "gclid"))
            {
                this.Source = "googleads";
            }
        }
#endif
    }

}