using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using TileMatch.Event;
using TileMatch.Game;
using TileMatch.Game.Block;
using TileMatch.Game.Magic;
using TileMatch.Game.Shuffle;
using UnityEngine;
using UnityEngine.UI;
using BiEventTileGarden = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class TileMatchMainController
{
    private class PropData
    {
        public GameObject _propObj;
        public GameObject _TipsObj;
        public Button _button;
        public UserData.ResourceId _propId;
        public List<Image> _grayMask = new List<Image>();
        public LocalizeTextMeshProUGUI _numText;
        public LocalizeTextMeshProUGUI _tipText;
        public GameObject _lockObj;
        public Animator _tipAnimator;
        public Animator _normalAnimator;
        public GameObject _numObj;
        public GameObject _addObj;
        public float _alpha = 0.6f;
        public bool _isUnLock = false;
        public int _unlockLevel;
        public GameObject _iconObj;
        public int _propNum;
    }

    private string[] PropObjName = {"BackButton", "SuperBackButton", "ShuffleButton", "ExtendButton", "MagicButton"};
    private UserData.ResourceId[] PropResId = { UserData.ResourceId.Prop_Back, UserData.ResourceId.Prop_SuperBack,UserData.ResourceId.Prop_Shuffle,UserData.ResourceId.Prop_Extend, UserData.ResourceId.Prop_Magic};

    private Dictionary<UserData.ResourceId, PropData> _propDatas = new Dictionary<UserData.ResourceId, PropData>();
    private void AwakeProp()
    {
        for(int i = 0; i < PropObjName.Length; i++)
        {
            string name = PropObjName[i];
            
            PropData propData = new PropData();

            propData._propId = PropResId[i];
            propData._propObj = transform.Find($"Root/BottomGroup/ButtonsGroup/{name}").gameObject;
            propData._button = propData._propObj.GetComponent<Button>();
            propData._button.onClick.AddListener(() =>
            {
                OnClickPropButton(propData);
            });
            if (propData._propId == UserData.ResourceId.Prop_SuperBack)
            {
                propData._button.gameObject.SetActive(false);
            }

            for (int j = 1; j <= 2; j++)
                propData._grayMask.Add(propData._propObj.transform.Find($"Normal/Mask{j}").GetComponent<Image>());

            propData._TipsObj = propData._propObj.transform.Find("Tips").gameObject;
            propData._iconObj = propData._propObj.transform.Find("Normal/Icon").gameObject;
            propData._numText = propData._propObj.transform.Find("Normal/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            propData._tipText = propData._propObj.transform.Find("Tips/Text").GetComponent<LocalizeTextMeshProUGUI>();
            propData._tipAnimator = propData._propObj.transform.Find("Tips").GetComponent<Animator>();
            propData._lockObj = propData._propObj.transform.Find("Lock").gameObject;
            propData._numObj = propData._propObj.transform.Find("Normal/Num").gameObject;
            propData._addObj = propData._propObj.transform.Find("Normal/Add").gameObject;
            propData._unlockLevel = TileMatchConfigManager.Instance.GetUnLockLevel((int)propData._propId);
            propData._isUnLock = true;
            propData._propNum = UserData.Instance.GetRes(propData._propId);
            propData._normalAnimator = propData._propObj.transform.Find("Normal").GetComponent<Animator>();
            propData._normalAnimator.Play("Idle", 0, 0);
            
            _propDatas.Add(PropResId[i],propData);

            propData._grayMask.ForEach(a=>
            {
                a.gameObject.SetActive(true);
                a.color = new Color(0, 0, 0, 0);
            });

            int id = (int)PropResId[i];
        }

        InitPropStatus();
        
        TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_UpdatePropStatus, UpdateStatus);
        TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_BuyPropSuccess, BuyPropSuccess);
        TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_AutoUseProp, AutoUseProp);
        
    }

    private void InitPropStatus()
    {
        foreach (var kv in _propDatas)
        {
            UpdatePropStatus(kv.Value, false, true);
        }
    }
    private void UpdatePropStatus(PropData propData, bool isAnim, bool isInit = false)
    {
        int num = propData._propNum;
        
        propData._numText.SetText(num.ToString());
        
        propData._lockObj.gameObject.SetActive(!propData._isUnLock);
        if (propData._isUnLock)
        {
            propData._numObj.gameObject.SetActive(num > 0);
            propData._addObj.gameObject.SetActive(num <= 0);
        }
        else
        {
            propData._numObj.gameObject.SetActive(false);
            propData._addObj.gameObject.SetActive(false);
        }
        
        if(!propData._isUnLock)
            return;
        
        switch (propData._propId)
        {
            case UserData.ResourceId.Prop_Back:
            {
                ChangeGrayMask(propData, isInit?true:TileMatchGameManager.Instance.GetRecordNum() == 0, isAnim);
                break;
            }
            case UserData.ResourceId.Prop_SuperBack:
            {
                ChangeGrayMask(propData, isInit?true:TileMatchGameManager.Instance.IsEmptyBanner(), isAnim);
                break;
            }
            case UserData.ResourceId.Prop_Extend:
            {
                ChangeGrayMask(propData, isInit?false:TileMatchGameManager.Instance.IsExtendBanner(), isAnim);
                break;
            }
        } 
    }

    private void ChangeGrayMask(PropData propData, bool isShow, bool isAnim)
    {
        propData._grayMask.ForEach(a=>
        {
            a.gameObject.SetActive(true);
            a.transform.DOKill();
        });

        float alpha = isShow ? propData._alpha : 0;
        float animTime = 0.5f;
        if (isAnim)
        {
            propData._grayMask.ForEach(a =>
            {
                a.DOFade(alpha, animTime);
            });
        }
        else
        {
            propData._grayMask.ForEach(a => a.color = new Color(0, 0, 0, alpha));
        }
    }
    
    private void OnClickPropButton(PropData propData, bool checkNum = true)
    {
        int id = (int)propData._propId;
        
        if (checkNum)
        {
            if (!propData._isUnLock)
            {
                PlayTipsAnim(propData,   LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_booster_undo_unlock", propData._unlockLevel.ToString()));
                return;
            }

            int num = propData._propNum;
            if (num == 0)
            {
                // UIManager.Instance.OpenWindow(UINameConst.UIPopupBuyItem, propData._propId);
                UIPopupKapiTileGiftBagController.Open();
                return;
            }
        
            if(!TileMatchGameManager.Instance.CanUseProp(propData._propId))
                return;
        }
        
        
        switch (propData._propId)
        {
            case UserData.ResourceId.Prop_Back:
            {
                if (TileMatchGameManager.Instance.GetRecordNum() == 0)
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }

                if (MagicBlockManager.Instance.IsMagic || ShuffleBlockManager.Instance.IsShuffle())
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }
                break;
            }
            case UserData.ResourceId.Prop_SuperBack:
            {
                if (TileMatchGameManager.Instance.GetBannerBlockNum() == 0)
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }
                if (MagicBlockManager.Instance.IsMagic || ShuffleBlockManager.Instance.IsShuffle())
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }
                break;
            }
            case UserData.ResourceId.Prop_Extend:
            {
                if (TileMatchGameManager.Instance.IsExtendBanner())
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_extraslot_no_moves"));
                    return;
                }
                break;
            }
            case UserData.ResourceId.Prop_Magic:
            {
                if (!TileMatchGameManager.Instance.CanUseMagic() || ShuffleBlockManager.Instance.IsShuffle())
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }
                break;
            }
            case UserData.ResourceId.Prop_Shuffle:
            {
                if (MagicBlockManager.Instance.IsMagic || ShuffleBlockManager.Instance.IsShuffle())
                {
                    PlayTipsAnim(propData,LocalizationManager.Instance.GetLocalizedString("UI_booster_no_move"));
                    return;
                }
                break;
            }
            default:
            {
                break;
            }
        }

        if (checkNum)
        {
            propData._propNum -= 1;
            propData._propNum = Math.Max(0, propData._propNum);
            UserData.Instance.ConsumeRes(propData._propId, 1, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventTileGarden.Types.ItemChangeReason.UseLevel});
            TileMatchRoot.Instance.PropUseState.TryAdd((int)propData._propId, 0);
            TileMatchRoot.Instance.PropUseState[(int)propData._propId] += 1;
            propData._normalAnimator.Play("Open", 0, 0);
        }
        
        TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Operate_UseProp, propData._propId);

        UpdateAllStatus();
    }

    private void UpdateStatus(BaseEvent e)
    {
        UpdatePropStatus(_propDatas[UserData.ResourceId.Prop_Back], true);
        // UpdatePropStatus(_propDatas[UserData.ResourceId.Prop_SuperBack], true);
    }

    private void UpdateAllStatus()
    {
        foreach (var kv in _propDatas)
        {
            UpdatePropStatus(_propDatas[kv.Key], true);
        }
    }
    private void PlayTipsAnim(PropData propData, string text)
    {
        propData._tipText.SetText(text);
        propData._tipAnimator.Play("TileMatchMainTips", 0, 0);
    }

    public Vector3 GetPropPosition(UserData.ResourceId propId)
    {
        if(!_propDatas.ContainsKey(propId))
            return  Vector3.zero;

        return _propDatas[propId]._iconObj.transform.position;
    }
    

    private void BuyPropSuccess(BaseEvent e)
    {
        UserData.ResourceId propId = (UserData.ResourceId)e.datas[0];
        int num = (int)e.datas[1];
        
        if(!_propDatas.ContainsKey(propId))
            return;

        UpdatePropStatus(_propDatas[propId], true);
        
        FlyObjectManager.Instance.FlyItem(new ResData(propId, num), new Vector3(0,1.5f,0), GetPropPosition(propId), 1.1f, 0.8f, 10f, i =>
        {
            num--;
            if (num == 0)
                TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_BuyPropFlySuccess);
        });
    }

    private void AutoUseProp(BaseEvent e)
    {
        UserData.ResourceId propId = (UserData.ResourceId)e.datas[0];
        bool subNum = (bool)e.datas[1];
        
        if(!_propDatas.ContainsKey(propId))
            return;
        
        OnClickPropButton(_propDatas[propId], subNum);
    }

    private void UserDataUpdate(BaseEvent e)
    {
        foreach (var kv in _propDatas)
        {
            _propDatas[kv.Key]._propNum = UserData.Instance.GetRes(kv.Key);
            
            UpdatePropStatus(_propDatas[kv.Key], true);
        }
    }
}