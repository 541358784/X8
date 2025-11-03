using System;
using Gameplay;
using TileMatch.Event;
using TileMatch.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class TileMatchMainController : UIWindowController
{
    private Animator _animator;
    
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        
        transform.Find("Root/Top/TimeLimitTime").gameObject.SetActive(false);

        AwakeProp();
        AwakeSetting();
        TileMatchEventManager.Instance.AddEvent(EventEnum.UserDataUpdate, UserDataUpdate);
        
        CommonUtils.NotchAdapte(transform.Find("Root/Top").transform as RectTransform);
    }

    private void Start()
    {
        foreach (var kv in _propDatas)
        {
            CommonUtils.SetShieldButTime(kv.Value._propObj, 0.1f);
        }
    }
    
    private void OnClickReplayButton()
    {
        InitPropStatus();
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Replay);
    }

    public void AppearAnim()
    {
        PlayAnim("appear");
        InitPropStatus();
    }
    
    public void DisappearAnim()
    {
        PlayAnim("disappear");
        OnButtonClick_Close();
    }
    
    public void PlayAnim(string anim)
    {
        _animator.Play(anim, 0, 0);
    }

    private void OnDestroy()
    {
        TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_UpdatePropStatus, UpdateStatus);
        TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_BuyPropSuccess, BuyPropSuccess);
        TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_AutoUseProp, AutoUseProp);
        TileMatchEventManager.Instance.RemoveEvent(EventEnum.UserDataUpdate, UserDataUpdate);
    }
}