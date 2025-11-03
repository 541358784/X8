using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamic;
using Activity.GiftBagProgress.View;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
// using Slider = UnityEngine.UI.Slider;

public class GiftBagProgressTaskItem:MonoBehaviour
{
    private Slider _collectProgressSlider;
    private LocalizeTextMeshProUGUI _collectProgressText;
    
    private LocalizeTextMeshProUGUI _rewardCountNum;
    private Image _rewardSprite;
    private LocalizeTextMeshProUGUI _rewardCountNumComplete;
    private Image _rewardSpriteComplete;
    private LocalizeTextMeshProUGUI _rewardCountNumFinish;
    private Image _rewardSpriteFinish;

    private Button FinishBtn;

    private Animator Lock;
    private Animator LockComplete;
    
    private GiftBagProgressTaskConfig _config;
    private StorageGiftBagProgress Storage;

    
    private LocalizeTextMeshProUGUI normalText;
    private LocalizeTextMeshProUGUI finishText;

    private Transform NormalNode;
    private Transform CompleteNode;
    private Transform FinishNode;
    // Animator _animator;
    
    private void Awake()
    {
        _collectProgressSlider = transform.Find("Normal/Slider").GetComponent<Slider>();
        _collectProgressText = transform.Find("Normal/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _rewardCountNum = transform.Find("Normal/Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardSprite = transform.Find("Normal/Reward/Icon").GetComponent<Image>();
        _rewardCountNumComplete = transform.Find("Receive/Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardSpriteComplete = transform.Find("Receive/Reward/Icon").GetComponent<Image>();
        _rewardCountNumFinish = transform.Find("Finish/Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardSpriteFinish = transform.Find("Finish/Reward/Icon").GetComponent<Image>();

        FinishBtn = transform.Find("Receive/ReceiveButton").GetComponent<Button>();
        FinishBtn.onClick.AddListener(()=>
        {
            if (!Storage.CanCollectLevels.Contains(_config.Id))
                return;
            if (Storage.BuyState)
            {
                for (var i = 0; i < _config.RewardId.Count; i++)
                {
                    if (!UserData.Instance.IsResource(_config.RewardId[i]))
                    {
                        var endTrans = MergeMainController.Instance.rewardBtnTrans;
                        FlyGameObjectManager.Instance.FlyObject(_config.RewardId[i], _rewardSprite.transform.position,
                            endTrans, 1.1f, 0.5f, () =>
                            {
                                FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                                Animator shake = endTrans.transform.GetComponent<Animator>();
                                if (shake != null)
                                    shake.Play("shake", 0, 0);

                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                            }, true, 1.0f, 0.7f);
                    }
                    else
                    {
                        FlyGameObjectManager.Instance.FlyCurrency(
                            CurrencyGroupManager.Instance.GetCurrencyUseController(),
                            (UserData.ResourceId) _config.RewardId[i], _config.RewardNum[i],
                            _rewardSprite.transform.position, 0.5f,
                            true, true, 0.15f,
                            () => { });
                    }
                }
                GiftBagProgressModel.Instance.CollectTaskReward(_config);
            }
            else
            {
                var mainUI =
                    UIManager.Instance.GetOpenedUIByPath<UIPopupGiftBagProgressTaskController>(UINameConst
                        .UIPopupGiftBagProgressTask);
                if (mainUI)
                {
                    mainUI.ShowLockTip(this);
                }
            }
        });

        Lock = transform.Find("Normal/Lock").GetComponent<Animator>();
        LockComplete = transform.Find("Receive/Lock").GetComponent<Animator>();

        normalText = transform.Find("Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        finishText = transform.Find("Finish/Text").GetComponent<LocalizeTextMeshProUGUI>();

        NormalNode = transform.Find("Normal");
        CompleteNode = transform.Find("Receive");
        FinishNode = transform.Find("Finish");
    }

    public async Task PerformCompletedTask(int oldValue,int newValue)
    {
        NormalNode.gameObject.SetActive(true);
        CompleteNode.gameObject.SetActive(false);
        FinishNode.gameObject.SetActive(false);
        
        Lock.PlayAnimation(Storage.BuyState?"UnlockNormal":"Normal");
        LockComplete.PlayAnimation(Storage.BuyState?"UnlockNormal":"Normal");
        
        _collectProgressText.SetText(oldValue+"/"+_config.CollectCount);
        _collectProgressSlider.value = (float)oldValue/_config.CollectCount;
        await XUtility.WaitSeconds(0.3f);
        DOTween.To(() => (float) oldValue, (v) =>
        {
            _collectProgressText.SetText((int)v+"/"+_config.CollectCount);
            _collectProgressSlider.value = v/_config.CollectCount;
        }, (float) newValue, 0.5f).OnComplete(() =>
        {
            _collectProgressText.SetText(newValue+"/"+_config.CollectCount);
            _collectProgressSlider.value = (float)newValue/_config.CollectCount;
        });
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            var taskEntrance = Storage.GetMergeBubble();
            if (taskEntrance != null)
                taskEntrance.Show();
        }
        await XUtility.WaitSeconds(0.5f);
        FlyRewards(_config.RewardNum[0],
            _rewardSprite.transform.position,
            _rewardSprite.transform,0.5f,true);
        await XUtility.WaitSeconds(0.5f);
    }
    
    public void FlyRewards(int rewardNum, Vector2 srcPos, Transform starTransform, float time, bool showEffect,
        Action action = null)
    {
        var dstPos = new Vector3();
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            var taskEntrance = Storage.GetMergeBubble();
            if (taskEntrance == null)
                return;
            dstPos = taskEntrance.transform.position;
            action += taskEntrance.Hide;
        }
        else
        {
            var taskEntrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_GiftBagProgress>();
            if (taskEntrance == null)
                return;
            dstPos = taskEntrance.transform.position;
        }
        int count = Math.Min(rewardNum, 10);
        float delayTime = 0.3f;
        if (count >= 5)
            delayTime = 0.1f;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            FlyGameObjectManager.Instance.FlyObject(starTransform.gameObject, srcPos, dstPos, showEffect, time,
                delayTime * i, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(dstPos);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        action?.Invoke();
                    }
                });
        }
    }

    public void PerformUnlock()
    {
        Lock.PlayAnimation("Open");
        LockComplete.PlayAnimation("Open");
    }
    public void PrepareUnlock()
    {
        Lock.PlayAnimation("Normal");
        LockComplete.PlayAnimation("Normal");
    }
    public void Init(GiftBagProgressTaskConfig config,StorageGiftBagProgress storage)
    {
        _config = config;
        Storage = storage;
        RefreshView();
    }

    public void RefreshView()
    {
        var curCount = GiftBagProgressModel.Instance.GetTaskCollectCount(_config);
        var isCompleted = Storage.CanCollectLevels.Contains(_config.Id);
        var isFinish = Storage.AlreadyCollectLevels.Contains(_config.Id);
        // _animator.PlayAnimation(isCompleted?"CompletedIdle":"Idle");
        NormalNode.gameObject.SetActive(!isCompleted && !isFinish);
        CompleteNode.gameObject.SetActive(isCompleted);
        FinishNode.gameObject.SetActive(isFinish);
        _collectProgressText.SetText(curCount+"/"+_config.CollectCount);
        _collectProgressSlider.value = (float) curCount / _config.CollectCount;
        _rewardCountNum.SetText(_config.RewardNum[0].ToString());
        _rewardSprite.sprite = UserData.GetResourceIcon(_config.RewardId[0],UserData.ResourceSubType.Big);
        _rewardCountNumComplete.SetText(_config.RewardNum[0].ToString());
        _rewardSpriteComplete.sprite = UserData.GetResourceIcon(_config.RewardId[0],UserData.ResourceSubType.Big);
        _rewardCountNumFinish.SetText(_config.RewardNum[0].ToString());
        _rewardSpriteFinish.sprite = UserData.GetResourceIcon(_config.RewardId[0],UserData.ResourceSubType.Big);
        normalText.SetTerm(_config.LabelText);
        normalText.SetTermFormats(_config.CollectCount.ToString());
        finishText.SetTerm(_config.LabelText);
        finishText.SetTermFormats(_config.CollectCount.ToString());
        FinishBtn.gameObject.SetActive(isCompleted);
        Lock.PlayAnimation(Storage.BuyState?"UnlockNormal":"Normal");
        LockComplete.PlayAnimation(Storage.BuyState?"UnlockNormal":"Normal");
    }
}