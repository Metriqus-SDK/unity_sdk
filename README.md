# Metriqus SDK for Unity

This is the [Metriqus](https://www.metriqus.com/) SDK for Unity. Metriqus is a data analytic platform for web and mobile.

# Compatibility

- This SDK supports iOS 12 or later and Android API level 21 (Lollipop) or later.
- The SDK is compatible with Unity Editor 2019 or later.

# Installation

To install the Metriqus SDK, choose one of the following methods.

## 1. Install the Unity package 

To use the Metriqus SDK in your Unity app, you need to add it to your project. You can download the latest version from the [GitHub releases page](https://github.com/Metriqus-SDK/unity_sdk/releases).

To import the Metriqus SDK into your Unity project:

- Open the Unity Editor.
- Select Assets -> Import Package -> Custom Package.
- Select the downloaded SDK package.

## 2. Use the Unity Package Manager 
To install the Metriqus SDK with the Unity Package Manager, follow these steps:

- Select the Package Manager window in the Unity editor.
- Select Add package from git URL.
- Enter the following URL: ```https://github.com/Metriqus-SDK/unity_sdk.git```
- After importing the package navigate to `Window -> Package Manager -> Metriqus Unity SDK -> Samples` for importing SDK examples and essential assets.

# Integrate the SDK
The Metriqus SDK contains a Unity prefab that includes Metriqus script. The Metriqus script references to a settings asset. You can edit settings file to your needs. To open the prefab in the Unity editor:

1. After importing samples, add the Metriqus prefab to your first scene from:
```Assets/Samples/Metriqus Unity SDK/{version}/Samples/Prefabs/Metriqus.prefab```

2. Open the prefab Inspector Menu.
3. The prefab menu contains a settings reference to edit SDK behavior.

#### To set up the Metriqus SDK, enter the following information to your settings asset:

- **Client Key**: Check you dashboard for instructions to find your client key.
- **Client Secret**: Check you dashboard for instructions to find your client secret.
- **Environment**:
  - Choose **Sandbox** if you are testing your app and want to send test data. You need to enable sandbox mode in the dashboard to see test data.
  - Choose **Production** when you have finished testing and are ready to release your app.
- **Log Level**: This controls what logs you receive.

The Metriqus SDK starts when the app's Awake event triggers by default. To override this behavior, check the **Manuel Start** option. This enables you to initialize the Metriqus SDK by calling Metriqus.InitSdk with your referenced MetriqusSettings asset as parameter.

```
Metriqus.InitSdk(metriqusSettings);
```

## Metriqus SDK Tracking API

The following functions allow you to track user events, ad revenue, and other analytics-related actions within your Unity project.

### Event Tracking Functions

- **`TrackIAPEvent(MetriqusInAppRevenue metriqusEvent)`**  
  Tracks in-app purchase (IAP) events.

- **`TrackCustomEvent(MetriqusCustomEvent _event)`**  
  Tracks custom events with user-defined parameters.

- **`TrackPerformance(int fps, List<TypedParameter> parameters = null)`**  
  Tracks FPS and other performance-related metrics.

- **`TrackItemUsed(MetriqusItemUsedEvent _event)`**  
  Tracks when an item (currency, equipment, etc.) is used.

- **`TrackLevelStarted(MetriqusLevelStartedEvent _event)`**  
  Tracks when a level starts.

- **`TrackLevelCompleted(MetriqusLevelCompletedEvent _event)`**  
  Tracks when a level is completed.

- **`TrackCampaignAction(MetriqusCampaignActionEvent _event)`**  
  Tracks campaign-related actions such as "Showed", "Clicked", "Closed", or "Purchased".

- **`TrackScreenView(string screenName, List<TypedParameter> parameters = null)`**  
  Tracks when a user views a specific screen in the app.

- **`TrackButtonClick(string buttonName, List<TypedParameter> parameters = null)`**  
  Tracks button click events.

### Ad Revenue Tracking Functions

- **`TrackApplovinAdRevenue(MetriqusApplovinAdRevenue matriqusAdRevenue)`**  
  Tracks ad revenue from Applovin.

- **`TrackAdmobAdRevenue(MetriqusAdmobAdRevenue matriqusAdRevenue)`**  
  Tracks ad revenue from AdMob.

- **`TrackAdRevenue(MetriqusAdRevenue matriqusAdRevenue)`**  
  Tracks general ad revenue.

### User Attribute Functions

- **`SetUserAttribute(TypedParameter parameter)`**  
  Sets a user attribute.

- **`GetUserAttributes()`**  
  Retrieves all user attributes.

- **`RemoveUserAttribute(string key)`**  
  Removes a specific user attribute by key.

### Device and Geolocation Functions

- **`GetAdid()`**  
  Retrieves the Advertising ID (AdID) of the user.

- **`GetMetriqusSettings()`**  
  Retrieves the Metriqus SDK settings.

- **`GetGeolocation()`**  
  Retrieves geolocation data.

- **`GetDeviceInfo()`**  
  Retrieves device information.

- **`GetUniqueUserId()`**  
  Retrieves the unique user identifier.

### SDK State and Tracking Control

- **`IsInitialized()`**  
  Checks whether the Metriqus SDK is initialized.

- **`IsTrackingEnabled()`**  
  Checks whether tracking is enabled.

### Notes

- These functions are primarily designed for **Unity Android/iOS** platforms.
- Calls made outside these platforms will log an error message.
- Ensure the SDK is initialized before calling tracking-related functions.


## License

[MIT](https://choosealicense.com/licenses/mit/)
