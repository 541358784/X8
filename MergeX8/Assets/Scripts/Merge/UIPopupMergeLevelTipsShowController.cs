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
using Farm.Model;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupMergeLevelTipsShowController : UIWindow
{
    private Transform _awardsRoot;
    private Button _getBtn;
    private GameObject _packageItem;
    private List<GameObject> tempGo = new List<GameObject>();
    private GameObject _vfxBoxReward;
    bool _isCanClick = true;
    private Animator _animator;
    private LocalizeTextMeshProUGUI _levelText;
    private Action _callFunc;
    
    public override void PrivateAwake()
    {
        UIManager.Instance.CloseUI(UINameConst.UIPopupMergeLevelTips);
        AudioManager.Instance.PlaySound(43);
        _packageItem = transform.Find("Root/ContentGroup/UnitList/Item").gameObject;
        _packageItem.gameObject.SetActive(false);
        
        Transform content = this.transform.Find("Root/ContentGroup");
        _awardsRoot = content.Find("UnitList");
        _getBtn = content.GetComponentDefault<Button>("Button");
        _animator = gameObject.GetComponent<Animator>();

        _levelText = transform.Find("Root/ContentGroup/StarGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _getBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/Button");
        _getBtn.onClick.AddListener(OnLevelUpClick);
        ExperenceModel.Instance.LevelUp();
        InitAwards();
        
        UIRoot.Instance.EnableEventSystem = true;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _callFunc = null;
        if (objs.Length > 0)
            _callFunc = (Action)objs[0];
    }

    private void InitAwards()
    {
        tempGo.Clear();
        _getBtn.gameObject.SetActive(true);
        _levelText.SetText((ExperenceModel.Instance.GetLevel()).ToString());
        
        var config = ExperenceModel.Instance.GetCurrentLevelConfigByLevel(ExperenceModel.Instance.GetLevel()-1);
        if (config == null)
            return;
        
        int[] reward = config.reward;
        if (DifferenceManager.Instance.IsDiffPlan_New())
            reward = config.planb_reward == null ? config.reward : config.planb_reward;
        
        for (int i = 0; i < reward.Length; i++)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(reward[i]);
            GameObject go = Instantiate(_packageItem, _awardsRoot);
            go.gameObject.SetActive(true);
            if (itemConfig == null)
            {
                var rewardItem = go.AddComponent<CommonRewardItem>();
                var resData = new ResData(reward[i], config.amount[i]);
                rewardItem.Init(resData);
            }
            else
            {
                go.transform.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                go.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText("x" +config.amount[i].ToString());   
            }
            // var script = go.GetComponentDefault<MergePackageUnit>();
            // script.SetBoardId(MergeBoardEnum.Main);
            // script.SetItemInfomation(itemConfig, -1, MergePackageUnitType.taskRewards, 0, config.amount[i]);
            
            tempGo.Add(go);
        }
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue47);

    }

    public void OnLevelUpClick()
    {
        if (!_isCanClick)
            return;

        _isCanClick = false;
        
        StartCoroutine(LevelUpAnim());
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue48);
    }

    IEnumerator LevelUpAnim()
    {
        var config = ExperenceModel.Instance.GetCurrentLevelConfigByLevel(ExperenceModel.Instance.GetLevel() - 1);
        if (config == null)
        {
            _callFunc?.Invoke();
            yield break;
        }

        _isCanClick = false;

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
 
        yield return new WaitForSeconds(0.1f);
        canClickMask = true;
        ClickUIMask();

        int[] reward = config.reward;
        if (DifferenceManager.Instance.IsDiffPlan_New())
            reward = config.planb_reward == null ? config.reward : config.planb_reward;
        
        for (int i = 0; i < reward.Length; i++)
        {
            int id = reward[i];
            int index = i;
            
            UIRoot.Instance.EnableEventSystem = false;
            FlyGameObjectManager.Instance.FlyObject(id, tempGo[i].transform.position, endTrans, 0.8f, 0.8f, () =>
            {
                if (index == reward.Length - 1)
                {
                    UIRoot.Instance.EnableEventSystem = true;
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
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);

                    Action triggerGuide = () =>
                    {
                        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.EnterHome))
                        {
                            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterHome, ExperenceModel.Instance.GetLevel().ToString());
                        }

                        FarmModel.Instance.TriggerFarmGuide();
                        
                        _callFunc?.Invoke();
                    };
                    
                    if (!StorySubSystem.Instance.Trigger(StoryTrigger.LevelUp,ExperenceModel.Instance.GetLevel().ToString(),
                            b =>
                            {
                                if(b)
                                    triggerGuide();
                            } ))
                    {
                        triggerGuide();
                    }
                }
            }, true, 1.0f, 0.7f, true);
        }

        //AddRewards(config.reward, config.amount);
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true);
            }));
    }
}