using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
// using Slider = UnityEngine.UI.Slider;

public class CoinRushTaskItem:MonoBehaviour
{
    private Slider _collectProgressSlider;
    private LocalizeTextMeshProUGUI _collectProgressText;
    private LocalizeTextMeshProUGUI _rewardCountNum;
    private Image _rewardSprite;
    private CoinRushTaskConfig _config;
    private Transform _normalGroup;
    private Transform _completeGroup;
    private Transform _rewardIcon;

    private Image normalIcon;
    private Image completeIcon;
    private LocalizeTextMeshProUGUI normalText;
    private LocalizeTextMeshProUGUI completeText;
    // Animator _animator;
    
    private void Awake()
    {
        _rewardIcon = transform.Find("Normal/RewardIcon");
        _normalGroup = transform.Find("Normal");
        normalIcon = transform.Find("Normal/Icon").GetComponent<Image>();
        normalText = transform.Find("Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _completeGroup = transform.Find("Complete");
        completeIcon = transform.Find("Complete/Icon").GetComponent<Image>();
        completeText = transform.Find("Complete/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _collectProgressSlider = transform.Find("Normal/Slider").GetComponent<Slider>();
        _collectProgressText = transform.Find("Normal/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardCountNum = transform.Find("Normal/RewardIcon/Num").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardSprite = transform.Find("Normal/RewardIcon").GetComponent<Image>();
        // _animator = transform.GetComponent<Animator>();
    }

    public async Task PerformCompletedTask(int oldValue,int newValue)
    {
        _rewardCountNum.SetText(_config.RewardNum[0].ToString());
        _rewardSprite.sprite = UserData.GetResourceIcon(_config.RewardId[0],UserData.ResourceSubType.Big);
        _normalGroup.gameObject.SetActive(true);
        _completeGroup.gameObject.SetActive(false);
        
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
        
        await XUtility.WaitSeconds(0.5f);
        for (var i = 0; i < _config.RewardId.Count; i++)
        {
            if (!UserData.Instance.IsResource(_config.RewardId[i]))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCoinrushMission,
                    isChange = false,
                    itemAId = _config.RewardId[i]
                });
                var endTrans = MergeMainController.Instance.rewardBtnTrans;
                FlyGameObjectManager.Instance.FlyObject(_config.RewardId[i], _rewardIcon.position, endTrans, 1.1f, 0.5f, () =>
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
                    (UserData.ResourceId) _config.RewardId[i], _config.RewardNum[i], _rewardIcon.position, 0.5f,
                    true, true, 0.15f,
                    () =>
                    {
                    });   
            }
       
            UserData.Instance.AddRes(_config.RewardId[i],_config.RewardNum[i],
                new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinrushMission
                }, false);
       
        }
        await XUtility.WaitSeconds(0.5f);
        // _normalGroup.gameObject.SetActive(false);
        // _completeGroup.gameObject.SetActive(true);
        // await XUtility.WaitSeconds(1f);
        // await _animator.PlayAnimationAsync("Completed");
    }
    public void Init(CoinRushTaskConfig config)
    {
        _config = config;
        RefreshView();
    }

    public void RefreshView()
    {
        var curCount = CoinRushModel.Instance.GetTaskCollectCount(_config);
        var isCompleted = curCount >= _config.CollectCount;
        // _animator.PlayAnimation(isCompleted?"CompletedIdle":"Idle");
        _normalGroup.gameObject.SetActive(!isCompleted);
        _completeGroup.gameObject.SetActive(isCompleted);
        _collectProgressText.SetText(curCount+"/"+_config.CollectCount);
        _collectProgressSlider.value = (float) curCount / _config.CollectCount;
        _rewardCountNum.SetText(_config.RewardNum[0].ToString());
        _rewardSprite.sprite = UserData.GetResourceIcon(_config.RewardId[0],UserData.ResourceSubType.Big);
        if ((CoinRushModel.CoinRushTaskTargetType) _config.TargetType != CoinRushModel.CoinRushTaskTargetType.Special)
        {
            normalIcon.sprite = UserData.GetResourceIcon(_config.CollectType, UserData.ResourceSubType.Big);
            completeIcon.sprite = UserData.GetResourceIcon(_config.CollectType, UserData.ResourceSubType.Big);   
        }
        else if ((CoinRushModel.CoinRushSpecialCollectType) _config.CollectType ==
                 CoinRushModel.CoinRushSpecialCollectType.GetCoin)
        {
            normalIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Coin, UserData.ResourceSubType.Big);
            completeIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Coin, UserData.ResourceSubType.Big);
        }
        
        normalText.SetTerm(_config.LabelText);
        completeText.SetTerm(_config.LabelText);
    }
}