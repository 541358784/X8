// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01
// reference: https://developer.apple.com/documentation/uikit/animation_and_haptics

#import <Foundation/Foundation.h>

UISelectionFeedbackGenerator* FeedbackGeneratorSelection;
UINotificationFeedbackGenerator* FeedbackGeneratorNotification;
UIImpactFeedbackGenerator* FeedbackGeneratorLightImpact;
UIImpactFeedbackGenerator* FeedbackGeneratorMediumImpact;
UIImpactFeedbackGenerator* FeedbackGeneratorHeavyImpact;

void FeedbackGeneratorsInit()
{
    FeedbackGeneratorSelection = [[UISelectionFeedbackGenerator alloc] init];
    FeedbackGeneratorNotification = [[UINotificationFeedbackGenerator alloc] init];
    FeedbackGeneratorLightImpact = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
    FeedbackGeneratorMediumImpact =  [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
    FeedbackGeneratorHeavyImpact =  [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
}

void FeedbackGeneratorsRelease()
{
    FeedbackGeneratorSelection = nil;
    FeedbackGeneratorNotification = nil;
    FeedbackGeneratorLightImpact = nil;
    FeedbackGeneratorMediumImpact = nil;
    FeedbackGeneratorHeavyImpact = nil;
}

void HapticSelection()
{
    [FeedbackGeneratorSelection prepare];
    [FeedbackGeneratorSelection selectionChanged];
}

void HapticSuccess()
{
    [FeedbackGeneratorNotification prepare];
    [FeedbackGeneratorNotification notificationOccurred:UINotificationFeedbackTypeSuccess];
}

void HapticWarning()
{
    [FeedbackGeneratorNotification prepare];
    [FeedbackGeneratorNotification notificationOccurred:UINotificationFeedbackTypeWarning];
}

void HapticFailure()
{
    [FeedbackGeneratorNotification prepare];
    [FeedbackGeneratorNotification notificationOccurred:UINotificationFeedbackTypeError];
}

void HapticLightImpact()
{
    [FeedbackGeneratorLightImpact prepare];
    [FeedbackGeneratorLightImpact impactOccurred];
}

void HapticMediumImpact()
{
    [FeedbackGeneratorMediumImpact prepare];
    [FeedbackGeneratorMediumImpact impactOccurred];
}

void HapticHeavyImpact()
{
    [FeedbackGeneratorHeavyImpact prepare];
    [FeedbackGeneratorHeavyImpact impactOccurred];
}
