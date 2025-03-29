#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>
#import <StoreKit/SKAdNetwork.h>
#import <AdServices/AdServices.h>
#import <StoreKit/StoreKit.h>

@interface Metriqus:NSObject
+ (void)reportAdNetworkAttribution:(void (^)(NSString *message))messageCallback;
+ (void)updateConversionValue:(NSInteger)value MessageCallback:(void (^)(NSString *message))messageCallback;
+ (void)requestTrackingPermission:(void (^)(NSString *idfa))callback;
+ (void)readAttributionToken:(void (^)(NSString *token))success Failure:(void (^)(NSString *errorMessage))failure;
@end

@implementation Metriqus

void handleSKAdNetworkCompletion(NSError * _Nullable error, void (^messageCallback)(NSString *)) {
    if (error) {
        NSLog(@"[Metriqus] SKAdNetwork completion failed: %@ (Code: %ld, Domain: %@)", error.localizedDescription, (long)error.code, error.domain);
        
        if (messageCallback) {
            messageCallback([NSString stringWithFormat:@"Error updating postback conversion value: %@ (Code: %ld, Domain: %@)", error.localizedDescription, (long)error.code, error.domain]);
        }
    } else {
        NSLog(@"[Metriqus] Postback conversion value updated successfully.");
        if (messageCallback) {
            messageCallback(@"Postback conversion value updated successfully.");
        }
    }
}

+ (void)reportAdNetworkAttribution:(void (^)(NSString *message))messageCallback {
    NSLog(@"[Metriqus] Reporting ad network attribution...");

    if (@available(iOS 15.4, *)) {
        NSLog(@"[Metriqus] Using updatePostbackConversionValue.");
        [SKAdNetwork updatePostbackConversionValue:0 completionHandler:^(NSError * _Nullable error) {
            handleSKAdNetworkCompletion(error, messageCallback);
        }];
    } else if (@available(iOS 14.0, *)) {
        NSLog(@"[Metriqus] Using deprecated updateConversionValue.");
        [SKAdNetwork updateConversionValue:0];
        if (messageCallback) {
            messageCallback(@"Fallback to updateConversionValue for older iOS versions (14.0 - 15.4).");
        }
    } else if (@available(iOS 11.3, *)) {
        NSLog(@"[Metriqus] Using registerAppForAdNetworkAttribution.");
        [SKAdNetwork registerAppForAdNetworkAttribution];
        if (messageCallback) {
            messageCallback(@"Fallback to registerAppForAdNetworkAttribution for older iOS versions (11.3 - 15.3).");
        }
    } else {
        NSLog(@"[Metriqus] SKAdNetwork not supported on this iOS version.");
        if (messageCallback) {
            messageCallback(@"SKAdNetwork is not supported on iOS versions below 11.3.");
        }
    }
}

+ (void)updateConversionValue:(NSInteger)value MessageCallback:(void (^)(NSString *message))messageCallback {
    NSLog(@"[Metriqus] Updating conversion value: %ld", (long)value);

    if (@available(iOS 15.4, *)) {
        NSLog(@"[Metriqus] Using updatePostbackConversionValue.");
        [SKAdNetwork updatePostbackConversionValue:0 completionHandler:^(NSError * _Nullable error) {
            handleSKAdNetworkCompletion(error, messageCallback);
        }];
    } else if (@available(iOS 14.0, *)) {
        NSLog(@"[Metriqus] Using deprecated updateConversionValue.");
        [SKAdNetwork updateConversionValue:0];
        if (messageCallback) {
            messageCallback(@"Fallback to updateConversionValue for older iOS versions (14.0 - 15.4).");
        }
    } else {
        NSLog(@"[Metriqus] SKAdNetwork not supported on this iOS version.");
        if (messageCallback) {
            messageCallback(@"SKAdNetwork is not supported on iOS versions below 11.3.");
        }
    }
}

+ (void)requestTrackingPermission:(void (^)(NSString *idfa))callback {
    NSLog(@"[Metriqus] Requesting tracking permission...");

    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            NSLog(@"[Metriqus] Tracking authorization status: %ld", (long)status);
            switch (status) {
                case ATTrackingManagerAuthorizationStatusAuthorized: {
                    NSUUID *idfa = [[ASIdentifierManager sharedManager] advertisingIdentifier];
                    if (idfa && ![idfa.UUIDString isEqualToString:@"00000000-0000-0000-0000-000000000000"]) {
                        callback(idfa.UUIDString);
                    } else {
                        callback(@"");
                    }
                    break;
                }
                default:
                    callback(@"");
                    break;
            }
        }];
    } else {
        NSUUID *idfa = [[ASIdentifierManager sharedManager] advertisingIdentifier];
        if (idfa && ![idfa.UUIDString isEqualToString:@"00000000-0000-0000-0000-000000000000"]) {
            callback(idfa.UUIDString);
        } else {
            callback(@"");
        }
    }
}

+ (void)readAttributionToken:(void (^)(NSString *token))success Failure:(void (^)(NSString *errorMessage))failure {
    NSLog(@"[Metriqus] Reading attribution token...");

    if (@available(iOS 14.3, *)) {
        NSError *error = nil;
        NSString *token = [AAAttribution attributionTokenWithError:&error];

        if (error) {
            NSLog(@"[Metriqus] Failed to retrieve attribution token: %@", error.localizedDescription);
            if (failure) failure([NSString stringWithFormat:@"Failed to retrieve attribution token: %@", error.localizedDescription]);
        } else if (token) {
            NSLog(@"[Metriqus] Successfully retrieved attribution token.");
            if (success) success(token);
        } else {
            NSLog(@"[Metriqus] Attribution token is null.");
            if (failure) failure(@"Attribution token is null.");
        }
    } else {
        NSLog(@"[Metriqus] Attribution token requires iOS 14.3 or later.");
        if (failure) failure(@"Attribution token requires iOS 14.3 or later.");
    }
}

@end

// C interface for Unity or other integrations
extern "C" void metriqusReportAdNetworkAttribution(void (*callback)(const char *)) {
    NSLog(@"[Metriqus] Calling metriqusReportAdNetworkAttribution.");
    [Metriqus reportAdNetworkAttribution:^(NSString *message) {
        if (callback) {
            callback([message UTF8String]);
        }
    }];
}

extern "C" void metriqusUpdateConversionValue(int value, void (*messageCallback)(const char *)) {
    NSLog(@"[Metriqus] Calling metriqusUpdateConversionValue.");
    [Metriqus updateConversionValue:value MessageCallback:^(NSString *message) {
        if (message) {
            messageCallback([message UTF8String]);
        }
    }];
}

extern "C" void metriqusRequestTrackingPermission(void (*callback)(const char *)) {
    NSLog(@"[Metriqus] Calling metriqusRequestTrackingPermission.");
    [Metriqus requestTrackingPermission:^(NSString *idfa) {
        if (callback) {
            callback([idfa UTF8String]);
        }
    }];
}

extern "C" void metriqusReadAttributionToken(void (*callback)(const char *), void (*failureCallback)(const char *)) {
    NSLog(@"[Metriqus] Calling metriqusReadAttributionToken.");
    [Metriqus readAttributionToken:^(NSString *token) {
        if (callback) {
            callback([token UTF8String]);
        }
    } Failure:^(NSString *error) {
        if (failureCallback) {
            failureCallback([error UTF8String]);
        }
    }];
}
