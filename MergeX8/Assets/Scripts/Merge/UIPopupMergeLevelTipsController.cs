using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK.Asset;
using DG.Tweening;
using Difference;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupMergeLevelTipsController : UIWindow
{
    private Transform _awardsRoot;
    private Slider _slider, _slider2;
    private LocalizeTextMeshProUGUI _level, _experenceProgross, _experenceProgross2;
    private Button _getBtn;
    private Button _notGetBtn;
    private GameObject _packageItem;
    private Transform DefaultResourceItem;
    private List<GameObject> tempGo = new List<GameObject>();
    private GameObject _vfxBoxReward;
    bool _isCanClick = true;
    private Animator _expAnimator;
    private Animator _animator;
    private UIShiny _uiShiny;

    public override void PrivateAwake()
    {
        _packageItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergePackageUnit");
        Transform content = this.transform.Find("Root/ContentGroup");
        _awardsRoot = content.Find("UnitList");
        DefaultResourceItem = _awardsRoot.Find("Item");
        DefaultResourceItem.gameObject.SetActive(false);
        _getBtn = content.GetComponentDefault<Button>("Button");
        _notGetBtn = content.GetComponentDefault<Button>("ButtonGray");
        _slider = content.GetComponentDefault<Slider>("Slider");

        _uiShiny = content.Find("Slider/Fill Area/Fill").GetComponent<UIShiny>();
        _uiShiny.enabled = false;

        _experenceProgross = _slider.GetComponentDefault<LocalizeTextMeshProUGUI>("ExperienceText");

        _slider2 = content.GetComponentDefault<Slider>("StarSlider");
        _level = _slider2.GetComponentDefault<LocalizeTextMeshProUGUI>("Text");
        _expAnimator = _slider2.gameObject.GetComponent<Animator>();
        _animator = gameObject.GetComponent<Animator>();
        BindClick("Root/ContentGroup/CloseButton", (go) =>
        {
            if (!_isCanClick)
                return;

            _isCanClick = false;
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        });
        _getBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/Button");
        _getBtn.onClick.AddListener(OnLevelUpClick);
        InitAwards();
        _level.SetText(ExperenceModel.Instance.GetLevel().ToString());
        _slider.value = ExperenceModel.Instance.GetPercentExp();
        _slider2.value = ExperenceModel.Instance.GetPercentExp();
        _isCanClick = true;
        _experenceProgross.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(),
            ExperenceModel.Instance.GetCurrentLevelTotalExp()));

        if (ExperenceModel.Instance.IsMaxLevel())
        {
            _slider.value = 1;
            _slider2.value = 1;
            _experenceProgross.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(), "Max"));
        }
    }

    private void InitAwards()
    {
        tempGo.Clear();
        _getBtn.interactable = ExperenceModel.Instance.IsCanLevelUp();
        if (ExperenceModel.Instance.IsCanLevelUp())
        {
            _getBtn.gameObject.SetActive(true);
            _notGetBtn.gameObject.SetActive(false);
        }
        else
        {
            _getBtn.gameObject.SetActive(false);
            _notGetBtn.gameObject.SetActive(false);
        }

        if (ExperenceModel.Instance.IsMaxLevel())
        {
            _getBtn.gameObject.SetActive(false);
            _notGetBtn.gameObject.SetActive(false);
            return;
        }

        _getBtn.gameObject.SetActive(false);
        _notGetBtn.gameObject.SetActive(false);
        
        var config = ExperenceModel.Instance.GetCurrentLevelConfig();
        if (config == null)
            return;

        int[] reward = config.reward;
        if (DifferenceManager.Instance.IsDiffPlan_New())
            reward = config.planb_reward == null ? config.reward : config.planb_reward;
        
        for (int i = 0; i < reward.Length; i++)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(reward[i]);
            if (itemConfig != null)
            {
                GameObject go = Instantiate(_packageItem, _awardsRoot);
                var script = go.GetComponentDefault<MergePackageUnit>();
                script.SetBoardId(MergeBoardEnum.Main);
                script.SetItemInfomation(itemConfig, -1, MergePackageUnitType.taskRewards, 0, config.amount[i]);
                tempGo.Add(go);

                StartCoroutine(PlayTween(go, i));   
            }
            else
            {
                var rewardItem = Instantiate(DefaultResourceItem, DefaultResourceItem.parent).gameObject.AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                var rewards = new ResData(reward[i], config.amount[i]);
                rewardItem.Init(rewards);
                StartCoroutine(PlayTween(rewardItem.gameObject, i));   
            }
        }
    }

    public void OnLevelUpClick()
    {
        if (!_isCanClick)
            return;
        if (ExperenceModel.Instance.IsCanLevelUp())
        {
            StartCoroutine(LevelUpAnim());
        }
        else
        {
            CloseWindowWithinUIMgr(true);
        }
    }

    private void AddRewards(int[] rewards, int[] count)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            mergeItem.Id = rewards[i];
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main, count[i]);
            SendLevelUpBi(rewards[i], count[i], ExperenceModel.Instance.GetLevel());
        }
    }

    private void SendLevelUpBi(int id, int count, int level)
    {
        for (int i = 0; i < count; i++)
        {
            var config = GameConfigManager.Instance.GetItemConfig(id);
            if (config == null)
                return;
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeLevelUp,
                itemAId = config.id,
                ItemALevel = config.level,
                isChange = true,
                extras = new Dictionary<string, string>
                {
                    {"level", level.ToString()}
                }
            });
        }
    }

    IEnumerator PlayTween(GameObject tweenObj, int index)
    {
        tweenObj.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.2f);

        float step = 1.0f / 60f;

        float delay = index * 8 * step;
        yield return new WaitForSeconds(delay);

        tweenObj.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tweenObj.transform.DOScale(1.2f, 7 * step));
        sequence.Append(tweenObj.transform.DOScale(0.9f, 11 * step));
        sequence.Append(tweenObj.transform.DOScale(1f, 10 * step));
        sequence.onComplete = () =>
        {
        };
        sequence.Play();
    }

    IEnumerator LevelUpAnim()
    {
        var config = ExperenceModel.Instance.GetCurrentLevelConfig();
        if (config == null)
            yield break;

        _isCanClick = false;

        _uiShiny.enabled = true;
        _uiShiny.Play();
        yield return new WaitForSeconds(0.5f);

        _slider.value = 0;
        _slider2.value = 0;
        yield return new WaitForSeconds(0.05f);
        AudioManager.Instance.PlaySound(29);

        Transform endTrans = null;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endTrans = MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            endTrans = UIHomeMainController.mainController.MainPlayTransform;
        }

        _expAnimator.Play("appear", 0, 0);

        yield return new WaitForSeconds(0.1f);
        ExperenceModel.Instance.LevelUp();
        _level.SetText(ExperenceModel.Instance.GetLevel().ToString());

        if (ExperenceModel.Instance.IsMaxLevel())
        {
            _experenceProgross.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(), "Max"));
        }

        int[] reward = config.reward;
        if (DifferenceManager.Instance.IsDiffPlan_New())
            reward = config.planb_reward == null ? config.reward : config.planb_reward;
        
        for (int i = 0; i < reward.Length; i++)
        {
            int id = reward[i];
            int index = i;
            FlyGameObjectManager.Instance.FlyObject(id, tempGo[i].transform.position, endTrans, 0.8f, 0.8f, () =>
            {
                if (index == reward.Length - 1)
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                    Animator shake = endTrans.transform.GetComponent<Animator>();
                    if (shake != null)
                        shake.Play("shake", 0, 0);
                    var root = endTrans.transform.Find("Root");
                    if (root != null)
                    {
                        Animator play_ani = root.GetComponent<Animator>();
                        if (play_ani != null)
                            play_ani.Play("appear", 0, 0);
                    }
                    ClickUIMask();
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                }
            }, true, 1.0f, 0.7f, true);
        }

        AddRewards(reward, config.amount);
        _experenceProgross.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(),
            ExperenceModel.Instance.GetCurrentLevelTotalExp()));

        if (!ExperenceModel.Instance.IsMaxLevel())
        {
            float percentExp = ExperenceModel.Instance.GetPercentExp();
            percentExp = Mathf.Min(percentExp, 1);
            _slider.value = 0;
            _slider.DOValue(percentExp, 0.5f);

            _slider2.value = 0;
            _slider2.DOValue(percentExp, 0.5f);
        }

        StartCoroutine(CommonUtils.DelayWork(0.5f,
            () => { gameObject.transform.localPosition = new Vector3(10000, 10000, 0); }));
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }
}