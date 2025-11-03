using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using DragonU3DSDK;
using Google.Play.Review;
#endif
using UnityEngine;

public class GooglePlayReviewManager : Manager<GooglePlayReviewManager>
{
#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
#endif

    protected override void InitImmediately()
    {
#if UNITY_ANDROID
        _reviewManager = new ReviewManager();
#endif
    }

    public void Prepare()
    {
#if UNITY_ANDROID
        StartCoroutine(PrepareAsync());
#endif
    }

    public void OpenReview()
    {
#if UNITY_ANDROID
        StartCoroutine(OpenReviewAsync());
#endif
    }

#if UNITY_ANDROID

    private IEnumerator PrepareAsync()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            DebugUtil.LogError("In App Review Prepare Error : {0}",requestFlowOperation.Error.ToString());
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }

    private IEnumerator OpenReviewAsync()
    {
        //打开的时候有可能会出现 _playReviewInfo 为空的情况
        if (_playReviewInfo == null)
        {
            DebugUtil.LogError("In App Review Launch Error : _playReviewInfo is null (没有拉到信息)!");
            yield break;
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DebugUtil.LogError("In App Review Launch Error : {0}",launchFlowOperation.Error.ToString());
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }

#endif
}