using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;


public class UIGameGetRewardController : UIWindow
{
    private LocalizeTextMeshProUGUI _textTitle;
    private Button _claimBtn;
    private Animator _animator;

    protected System.Action _claimBtnCallBack;
    private GameBIManager.ItemChangeReasonArgs _bi;
    protected List<ResData> _rewardList;
    private Transform boxGroup1;
    private Transform boxGroup2;

    private bool IsUpdateRes;
    private ChestType _chestType;
    private string UIGameGetReward_OpenBox1 = "UIGameGetReward_OpenBox1";
    private string UIGameGetReward_OpenBox2 = "UIGameGetReward_OpenBox2";
    private string UIGameGetReward_Ani = "UIGameGetReward_";
    private bool IsPlayingAni = false;

    // 唤醒界面时调用(创建的时候加载一次)
    public override void PrivateAwake()
    {
        _claimBtn = transform.Find("Root/ButtonGroup/GetButton").GetComponent<Button>();
        _claimBtn.onClick.AddListener(OnClaimButton);
        _animator = gameObject.GetComponent<Animator>();
        boxGroup1 = transform.Find("Root/Box/UIGameGetRewardBox1");
        boxGroup2 = transform.Find("Root/Box/UIGameGetRewardBox2");
        _textTitle = transform.Find("Root/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow();

        SetData(objs);
    }

    public void SetData(params object[] objs)
    {
        _rewardList = (List<ResData>) objs[0];
        _chestType = (ChestType) objs[1];
        _claimBtnCallBack = (System.Action) objs[2];
        IsUpdateRes = (bool) objs[3];
        if (objs.Length > 4)
            _bi = (GameBIManager.ItemChangeReasonArgs) objs[4];

        if (IsUpdateRes)
        {
            UserData.Instance.AddRes(_rewardList, _bi);
        }

        for (int i = 0; i < _rewardList.Count; i++)
        {
            var rewardItemObj = transform.Find("Root/RewardGroup/Reward" + _rewardList.Count + "/RewardItem" + i);
            var rewardItem = CommonUtils.GetComponent<UIBoxRewardItem>(rewardItemObj.gameObject);
            rewardItem.SetData(_rewardList[i]);
        }

        AnimLogic();
    }

    private void AnimLogic()
    {
        IsPlayingAni = true;
        boxGroup1.gameObject.SetActive(_chestType == ChestType.Level);
        boxGroup2.gameObject.SetActive(_chestType == ChestType.Stage);

        if (_chestType == ChestType.Level)
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIGameGetReward_OpenBox1, "", () =>
            {
                AudioManager.Instance.PlaySound(20);
                StartCoroutine(CommonUtils.PlayAnimation(_animator, UIGameGetReward_Ani + _rewardList.Count, "",
                    () => { IsPlayingAni = false; }));
            }));
        }
        else
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIGameGetReward_OpenBox2, "", () =>
            {
                AudioManager.Instance.PlaySound(20);
                StartCoroutine(CommonUtils.PlayAnimation(_animator, UIGameGetReward_Ani + _rewardList.Count, "",
                    () => { IsPlayingAni = false; }));
            }));
        }
    }

    private void OnClaimButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        if (IsPlayingAni)
            return;
        CloseWindowWithinUIMgr(true);
        if (_claimBtnCallBack != null)
            _claimBtnCallBack();
    }

    private void OnDestroy()
    {
    }

    public static UIGameGetRewardController Show(List<ResData> rewards, ChestType type, System.Action callback = null)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIGameGetReward, rewards, type, callback, false) as
            UIGameGetRewardController;
    }

    public static UIGameGetRewardController ShowAndUpdateUserData(List<ResData> rewards, ChestType type,
        GameBIManager.ItemChangeReasonArgs bi, System.Action callback = null)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIGameGetReward, rewards, type, callback, true, bi) as
            UIGameGetRewardController;
    }
}