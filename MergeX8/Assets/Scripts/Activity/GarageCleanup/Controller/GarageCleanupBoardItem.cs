using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class GarageCleanupBoardItem : MonoBehaviour
{
    private Transform _bg;
    private Transform _bgGreen;
    private Image _icon;
    private Button _tipButton;
    private Transform _possess;
    private Transform _finish;
    private Transform _questionMark;
    private Button _button;

    private TableMergeItem _itemConfig;
    private StorageGarageCleanupBoardItem _boardItem;
    private bool _isReveal;
    private bool _isStart;
    private int _index;
    private Animator _animator;
    private Transform _coupons;
    private LocalizeTextMeshProUGUI _couponsText;
    private int needFishToken = 0;
    private void Awake()
    {
        _bg = transform.Find("BG");
        _bgGreen = transform.Find("BGGreen");
        _icon = transform.Find("Icon").GetComponent<Image>();
        _tipButton = transform.Find("TipsBtn").GetComponent<Button>();
        _possess = transform.Find("Possess");
        _finish = transform.Find("Finish");
        _questionMark = transform.Find("QuestionMark");
        _coupons = transform.Find("Coupons");
        _couponsText = transform.Find("Coupons/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _button = gameObject.GetComponent<Button>();
        _animator = gameObject.GetComponent<Animator>();
        _button.onClick.AddListener(OnItemClick);
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, TurnIn);
    }

    private void TurnIn(BaseEvent obj)
    {
        if (obj== null || obj.datas == null || obj.datas.Length < 4)
            return;
        if ((MergeBoardEnum) obj.datas[0] != MergeBoardEnum.Main)
            return;
        int index = (int) obj.datas[1];
        List<ResData> rewards = (List<ResData>) obj.datas[2];
        if (_index == index)
        {
            CoroutineManager.Instance.StartCoroutine(RewardAni(rewards));  

        }

        UpdateIsHave();
    }

    public void PlayAni(string aniName,Action end)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, aniName, null, end))  ;
    }
    public  IEnumerator RewardAni(List<ResData> rewards)
    {
        Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }
        foreach (var res in rewards)
        {
            if (UserData.Instance.IsResource(res.id))
            {
               
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    (UserData.ResourceId)res.id, res.count, transform.position, 0.8f, true, true, 0.15f,
                    () =>
                    {
                        PayRebateModel.Instance.OnPurchaseAniFinish();
                        PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                        CurrencyGroupManager.Instance.currencyController.CheckLevelUp(null);
                    });
            }

            else
            {
                FlyGameObjectManager.Instance.FlyObject(res.id, transform.position, endPos, 1.2f, 2.0f, 1f,
                    () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
            }

            yield return new WaitForSeconds(0.15f);
        }
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, TurnIn);
    }

    public void SetData(StorageGarageCleanupBoardItem item, bool isReveal, int index)
    {
        _index = index;
        _itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        _boardItem = item;
        _isReveal = isReveal;
        if (_itemConfig == null)
        {
            DebugUtil.LogError("Item 错误 " + item.Id);
        }

        _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
        _tipButton.onClick.AddListener(() =>
        {
            MergeInfoView.Instance.OpenMergeInfo(_itemConfig);
            UIPopupMergeInformationController controller =
                UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as
                    UIPopupMergeInformationController;
            if (controller != null)
                controller.SetResourcesActive(false);
        });
        _tipButton.gameObject.SetActive(false);
        needFishToken=GarageCleanupModel.Instance.GetFishTokenCount(index);
        _couponsText.SetText(needFishToken.ToString());
        UpdateStatus();
    }
    public void UpdateIsHave()
    {
        
        bool isHave = GarageCleanupModel.Instance.IsHave(_itemConfig.id);
        if (_boardItem.State != 1)
        {
            if (isHave)
            {
                _possess.gameObject.SetActive(true);
                _bgGreen.gameObject.SetActive(true);
                // _tipButton.gameObject.SetActive(false);
                _coupons.gameObject.SetActive(false);
            }
            else
            {
                _possess.gameObject.SetActive(false);
                _bgGreen.gameObject.SetActive(false);
                // _tipButton.gameObject.SetActive(_isReveal);
                _coupons.gameObject.SetActive(_isReveal);
            }
        }
    }

    public void UpdateFinishState()
    {
        _finish.gameObject.SetActive(_boardItem.State == 1);
    }

    public void UpdateStatus()
    {
        var board = GarageCleanupModel.Instance.GetBoard();
        if (board != null)
        {
            _isReveal = board.IsReveal;
            _isStart = board.IsStart;
        }
        _questionMark.gameObject.SetActive(!_isReveal);
        _coupons.gameObject.SetActive(_isReveal && _boardItem.State != 1);
        _finish.gameObject.SetActive(_boardItem.State == 1);
        UpdateIsHave();
  
    }

    private void OnItemClick()
    {
        if (!_isReveal)
            return;
        if (!_isStart)
            return;
        if (_boardItem.State != 1)
        {
            if (GarageCleanupModel.Instance.IsHave(_itemConfig.id))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupSubmit, _index,false);
            }
            else
            {
                if (UserData.Instance.CanAford(UserData.ResourceId.Fishpond_token, needFishToken))
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupSubmit, _index,true);
                }
                else
                {
                    UIStoreController.OpenUI("",ShowArea.task_assist);
                }
             
            }
        }
    }
}