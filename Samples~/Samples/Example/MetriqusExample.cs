using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetriqusSdk.Example
{
    public class MetriqusExample : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI logText;

        [Header("Event Buttons")]
        [SerializeField]
        private Button sendSimpleEventButton;

        [Header("IAP Event Buttons")]
        [SerializeField]
        private Button sendRevenueEventButton;

        [Header("Ad Revenue Event Buttons")]
        [SerializeField]
        private Button sendAdRevenueEventButton;
        [SerializeField]
        private Button trackApplovinAdRevenueButton;
        [SerializeField]
        private Button trackAdmobAdRevenueButton;

        [Header("Custom Event Buttons")]
        [SerializeField]
        private Button trackPerformanceButton;
        [SerializeField]
        private Button trackItemUsedButton;
        [SerializeField]
        private Button trackLevelStartedButton;
        [SerializeField]
        private Button trackLevelCompletedButton;
        [SerializeField]
        private Button trackScreenViewButton;
        [SerializeField]    
        private Button trackButtonClickButton;
        [SerializeField]    
        private Button trackCampaignActionButton;

        [Header("Set User Attribute Buttons")]
        [SerializeField]
        private Button setUserAttributeButton;

        private void Start()
        {
            sendSimpleEventButton.onClick.AddListener(OnSimpleEventButtonClicked);

            sendRevenueEventButton.onClick.AddListener(OnRevenueEventButtonClicked);

            sendAdRevenueEventButton.onClick.AddListener(OnAdRevenueEventButtonClicked);
            trackApplovinAdRevenueButton.onClick.AddListener(OnTrackApplovinAdRevenueButtonClicked);
            trackAdmobAdRevenueButton.onClick.AddListener(OnTrackAdmobAdRevenueButtonClicked);

            trackPerformanceButton.onClick.AddListener(OnTrackPerformanceButtonClicked);
            trackItemUsedButton.onClick.AddListener(OnTrackItemUsedButtonClicked);
            trackLevelStartedButton.onClick.AddListener(OnTrackLevelStartedButtonClicked);
            trackLevelCompletedButton.onClick.AddListener(OnTrackLevelCompletedButtonClicked);
            trackScreenViewButton.onClick.AddListener(OnTrackScreenViewButtonClicked);
            trackButtonClickButton.onClick.AddListener(OnTrackButtonClickButtonClicked);
            trackCampaignActionButton.onClick.AddListener(OntrackCampaignActionButtonClicked);

            setUserAttributeButton.onClick.AddListener(OnUserAttributeButtonClicked);

            Metriqus.OnLog.AddListener(OnLog);
        }

        // EVENT CALLBACKS
        private void OnSimpleEventButtonClicked()
        {
            Metriqus.DebugLog("Simple Event Test Button Clicked");

            var _event = new MetriqusInAppRevenue();

            Metriqus.TrackIAPEvent(_event);
        }

        private void OnRevenueEventButtonClicked()
        {
            Metriqus.DebugLog("Revenue Event Test Button Clicked");

            var _event = new MetriqusInAppRevenue();

            _event.ProductId = "test_product_id";
#if UNITY_ANDROID
            _event.PurchaseToken = "android_purchase_token";
#elif UNITY_IOS
            _event.TransactionId = "ios_transaction_id";
#endif
            _event.SetRevenue(0.33, "EUR");

            Metriqus.TrackIAPEvent(_event);
        }

        // AD REVENUE EVENT CALLBACK
        private void OnAdRevenueEventButtonClicked()
        {
            Metriqus.DebugLog("Ad Revenue Event Test Button Clicked");

            var _event = new MetriqusAdRevenue("test");

            _event.AdImpressionsCount = 1;
            _event.AdRevenueNetwork = "test";
            _event.AdRevenueUnit = "test";
            _event.AdRevenuePlacement = "test";

            _event.SetRevenue(0.22, "EUR");

            Metriqus.TrackAdRevenue(_event);
        }

        // CUSTOM EVENT CALLBACK
        private void OnTrackPerformanceButtonClicked()
        {
            Metriqus.DebugLog("Track Performance Test Button Clicked");

            Metriqus.TrackPerformance(60, new() { new TypedParameter("performance", "low") });
        }

        private void OnTrackItemUsedButtonClicked()
        {
            Metriqus.DebugLog("Track Item Used Test Button Clicked");

            Metriqus.TrackItemUsed(new MetriqusItemUsedEvent(new() { new TypedParameter("item_x", 5) })
            {
                ItemName = "best item",
                Amount = 5,
                ItemType = "fire",
                ItemRarity = "epic",
                ItemClass = "mage",
                ItemCategory = "handheld",
                Reason = "fusion"
            });
        }

        private void OnTrackLevelStartedButtonClicked()
        {
            Metriqus.DebugLog("Track Level Started Test Button Clicked");

            Metriqus.TrackLevelStarted(new MetriqusLevelStartedEvent(new() { new TypedParameter("level_value", 10) })
            {
                LevelNumber = 10,
                LevelName = "Super Level",
                Map = "map_3"
            });
        }

        private void OnTrackLevelCompletedButtonClicked()
        {
            Metriqus.DebugLog("Track Level Completed Test Button Clicked");

            Metriqus.TrackLevelCompleted(new MetriqusLevelCompletedEvent(new() { new TypedParameter("level_comp_val", "1000") })
            {
                LevelNumber = 10,
                LevelName = "Super Level",
                Map = "map_3",
                Duration = 98,
                LevelProgress = 100,
                LevelReward = 20,
                LevelReward1 = 3,
                LevelReward2 = 5,
            });
        }
        private void OntrackCampaignActionButtonClicked()
        {
            Metriqus.DebugLog("Track Campaign Action Button Clicked");

            Metriqus.TrackCampaignAction(new MetriqusCampaignActionEvent(
                campaignId: "campaign_1",
                variantId: "variant_1",
                action: MetriqusCampaignActionType.Click,
                new() { new TypedParameter("campagn_val", "xyz") }));
        }

        private void OnTrackScreenViewButtonClicked()
        {
            Metriqus.DebugLog("Track Screen View Button Clicked");

            Metriqus.TrackScreenView("heroes_screen", new() { new TypedParameter("screen_data", "test") });
        }

        private void OnTrackButtonClickButtonClicked()
        {
            Metriqus.DebugLog("Track Button Click Button Clicked");

            Metriqus.TrackButtonClick("play_button", new() { new TypedParameter("button_val", 15) });
        }

        private void OnTrackApplovinAdRevenueButtonClicked()
        {
            Metriqus.DebugLog("Track Applovin AdRevenue Button Clicked");

            var _event = new MetriqusApplovinAdRevenue();

            _event.AdImpressionsCount = 1;
            _event.AdRevenueNetwork = "test";
            _event.AdRevenueUnit = "test";
            _event.AdRevenuePlacement = "test";

            _event.SetRevenue(0.22, "EUR");

            Metriqus.TrackApplovinAdRevenue(_event);
        }

        private void OnTrackAdmobAdRevenueButtonClicked()
        {
            Metriqus.DebugLog("Track Admob AdRevenue Button Clicked");

            var _event = new MetriqusAdmobAdRevenue();

            _event.AdImpressionsCount = 1;
            _event.AdRevenueNetwork = "test";
            _event.AdRevenueUnit = "test";
            _event.AdRevenuePlacement = "test";

            _event.SetRevenue(0.22, "EUR");

            Metriqus.TrackAdmobAdRevenue(_event);
        }

        // SET USER ATTRIBUTE BUTTON CALLBACK
        private void OnUserAttributeButtonClicked()
        {
            Metriqus.DebugLog("Set User Attribute Test Button Clicked");

            Metriqus.SetUserAttribute(new TypedParameter(MetriqusEventKeys.UserPropertyAge, 18));
        }

        private void OnLog(string log, LogType logType)
        {
            logText.text += "\n" + log;
        }
    }
}