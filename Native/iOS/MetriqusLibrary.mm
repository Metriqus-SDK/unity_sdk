#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>
#import <StoreKit/SKAdNetwork.h>
#import <AdServices/AdServices.h>

@interface Metriqus:NSObject
+ (void)reportAdNetworkAttribution: (void (^)(NSString *messsage))messageCallback;

+ (void)updateConversionValue:(NSInteger)value
              MessageCallback: (void (^)(NSString *messsage))messageCallback;

+ (void)requestTrackingPermission: (void (^)(NSString *idfa))callback;

+ (void)readAttributionToken:(void (^)(NSString *token))success
                     Failure:(void (^)(NSString *errorMessage))failure;
@end

@implementation Metriqus
+ (void)reportAdNetworkAttribution: (void (^)(NSString *messsage))messageCallback {
    if (@available(iOS 15.4, *)) {
        [SKAdNetwork updatePostbackConversionValue:0
                                 completionHandler:^(NSError * _Nullable error) {
            if (error) {
                if (messageCallback)
                {
                    messageCallback([NSString stringWithFormat:@"Error updating postback conversion value: %@ (Code: %ld, Domain: %@)",error.localizedDescription, (long)error.code, error.domain]);
                }

                // Handle specific error codes if needed
                if ([error.domain isEqualToString:@"SKANErrorDomain"]) {
                    switch (error.code) {
                        case 10: // Example: Invalid state
                            if (messageCallback)
                            {
                                messageCallback(@"Error: Conversion value cannot be updated in the current state.");
                            }
                            break;
                        default:
                            if (messageCallback)
                            {
                                messageCallback([NSString stringWithFormat:@"Unhandled SKANErrorDomain code: %ld", (long)error.code]);
                            }
                            break;
                    }
                }
            } else {
                if (messageCallback)
                {
                    messageCallback(@"Postback conversion value updated successfully.");
                }
            }
        }];
    } else if (@available(iOS 11.3, *)) {
        [SKAdNetwork registerAppForAdNetworkAttribution];
        if (messageCallback)
        {
            messageCallback(@"Fallback to registerAppForAdNetworkAttribution for older iOS versions (11.3 - 15.3).");
        }
    } else {
        if (messageCallback)
        {
            messageCallback(@"SKAdNetwork is not supported on iOS versions below 11.3.");
        }
    }
}

+ (void)updateConversionValue: (NSInteger)value
             MessageCallback : (void (^)(NSString *messsage))messageCallback {
                
    if (@available(iOS 15.4, *)) {
        [SKAdNetwork updatePostbackConversionValue:value
                                  completionHandler:^(NSError * _Nullable error) {
            if (error) {
                if (messageCallback)
                {
                    messageCallback([NSString stringWithFormat:@"Error updating postback conversion value: %@ (Code: %ld, Domain: %@)",
                                     error.localizedDescription, (long)error.code, error.domain]);
                }
                // Handle specific error codes for better debugging
                if ([error.domain isEqualToString:@"SKANErrorDomain"]) {
                    switch (error.code) {
                        case 10:
                            if (messageCallback)
                            {
                                messageCallback(@"Error: Conversion value cannot be updated in the current state.");
                            }
                            break;
                        default:
                            if (messageCallback)
                            {
                                messageCallback([NSString stringWithFormat:@"Unhandled SKANErrorDomain code: %ld", (long)error.code]);
                            }
                            break;
                    }
                }
            } else {
                if (messageCallback)
                {
                    messageCallback([NSString stringWithFormat:@"Postback conversion value updated successfully: %ld", (long)value]);
                }
            }
        }];
    } else if (@available(iOS 14.0, *)) {
        // Use the older API for iOS 14.0 to 15.3
        [SKAdNetwork updateConversionValue:value];
        if (messageCallback)
        {
            messageCallback([NSString stringWithFormat:@"Updated conversion value using the deprecated method: %ld", (long)value]);
        }
    } else {
        if (messageCallback)
        {
            messageCallback(@"SKAdNetwork is not supported on iOS versions below 14.0.");
        }
    }
}

+ (void)requestTrackingPermission: (void (^)(NSString *idfa))callback {
    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            switch (status) {
                case ATTrackingManagerAuthorizationStatusAuthorized: {
                    // Permission granted
                    NSUUID *idfa = [[ASIdentifierManager sharedManager] advertisingIdentifier];
                    if (idfa && ![idfa.UUIDString isEqualToString:@"00000000-0000-0000-0000-000000000000"]) {
                        callback(idfa.UUIDString);
                    } else {
                        callback(@""); // Or handle the lack of an IDFA appropriately
                    }
                    break;
                }
                case ATTrackingManagerAuthorizationStatusDenied:
                    callback(@"");
                    break;
                case ATTrackingManagerAuthorizationStatusRestricted:
                    callback(@"");
                    break;
                case ATTrackingManagerAuthorizationStatusNotDetermined:
                    callback(@"");
                    break;
                default:
                    callback(@"");
                    break;
            }
        }];
    } else {
        // For iOS 13 and earlier
        NSUUID *idfa = [[ASIdentifierManager sharedManager] advertisingIdentifier];

        if (idfa && ![idfa.UUIDString isEqualToString:@"00000000-0000-0000-0000-000000000000"])
        {
            callback(idfa.UUIDString);
        }
        else
        {
            callback(@"");
        }
    }
}

+ (void)readAttributionToken:(void (^)(NSString *token))success
                     Failure:(void (^)(NSString *errorMessage))failure
{
    if (@available(iOS 14.3, *)) {
        NSError *error = nil;
        NSString *token = [AAAttribution attributionTokenWithError:&error];

        if (error) {
            if (failure) {
                failure([NSString stringWithFormat:@"Failed to retrieve attribution token: %@", error.localizedDescription]);
            }
        } else if (token) {
            if (success) {
                success(token);
            }
        } else {
            if (failure) {
                failure(@"Attribution token is null.");
            }
        }
    } else {
        if (failure) {
            failure(@"Attribution token requires iOS 14.3 or later.");
        }
    }
}

@end

extern "C" void metriqusReportAdNetworkAttribution(void (*callback)(const char *))
{
    return [Metriqus reportAdNetworkAttribution:^(NSString *message) {
        if (callback) {
            const char *messageCStr = [message UTF8String];
            callback(messageCStr);
        }
    }];
}

extern "C" void metriqusUpdateConversionValue(int value, void (*messageCallback)(const char *))
{
    return [Metriqus updateConversionValue:value
           MessageCallback:^(NSString *message) {
            if (message) {
                const char *messageCStr = [message UTF8String];
                messageCallback(messageCStr);
            }
        }];
}

extern "C" void metriqusRequestTrackingPermission(void (*callback)(const char *))
{
    return [Metriqus requestTrackingPermission:^(NSString *idfa) {
        if (callback) {
            const char *idfaCStr = [idfa UTF8String];
            
            callback(idfaCStr);
        }
    }];
}

extern "C" void metriqusReadAttributionToken(void (*callback)(const char *), void (*failureCallback)(const char *))
{
    [Metriqus readAttributionToken:^(NSString *token) {
        if (callback) {
            const char *tokenCStr = [token UTF8String];
            callback(tokenCStr);
        }
    } Failure:^(NSString *error) {
        if (failureCallback) {
            const char *errorCStr = [error UTF8String];
            failureCallback(errorCStr);
        }
    }];
}
