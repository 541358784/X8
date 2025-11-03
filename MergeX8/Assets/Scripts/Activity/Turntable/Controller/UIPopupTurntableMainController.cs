using System;
using System.Collections;
using System.Collections.Generic;
using Activity.Turntable.Controller;
using Activity.Turntable.Model;
using DragonPlus;
using DragonPlus.Config.Turntable;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupTurntableMainController : UIWindowController
{
    private TurntableRollerView _rollerView;
    private LocalizeTextMeshProUGUI _totalNumText;
    private LocalizeTextMeshProUGUI _betTotalText;
    private LocalizeTextMeshProUGUI _betText;

    private int _betIndex = 0;
    private List<int> _betValue;

    private Button _spinButton;
    private Button _addButton;
    private Button _subButton;

    private GameObject _spinRed;
    private GameObject _spinGrey;

    private Animator _animator;
    
    public override void PrivateAwake()
    {
        _totalNumText = transform.Find("Root/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _betTotalText = transform.Find("Root/ButtonGroup/SpinButton/Root/Num").GetComponent<LocalizeTextMeshProUGUI>();
        _betText = transform.Find("Root/ButtonGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _rollerView = new TurntableRollerView(transform.Find("Root/Turntable/Reward"));
        _rollerView.Init();
        _rollerView.RefreshRewardState();

        _animator = transform.Find("Root/Turntable").GetComponent<Animator>();
        
        var closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(()=>
        {
            AnimCloseWindow();
        });
        
        _spinButton = transform.Find("Root/ButtonGroup/SpinButton").GetComponent<Button>();
        _spinButton.onClick.AddListener(() =>
        {
            SpinTurntable();
        });

        _spinRed = _spinButton.transform.Find("Root/Red").gameObject;
        _spinGrey = _spinButton.transform.Find("Root/Grey").gameObject;
        
        transform.Find("Root/ButtonHelp").GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UITurntableHelp);
        });
        
        _addButton = transform.Find("Root/ButtonGroup/AddButton").GetComponent<Button>();
        _addButton.onClick.AddListener(AddBet);
        
        _subButton = transform.Find("Root/ButtonGroup/MinusButton").GetComponent<Button>();
        _subButton.onClick.AddListener(SubBet);

        _betValue = TurntableConfigManager.Instance.TurntableSetingConfigList[0].BetValue;
        UpdateView();
    }

    private void Start()
    {
        CommonUtils.SetShieldButUnEnable(_addButton.gameObject);
        CommonUtils.SetShieldButUnEnable(_subButton.gameObject);
        CommonUtils.SetShieldButUnEnable(_spinButton.gameObject);

        _animator.Play("loop", -1, 0);
        _rollerView.RestRewardState();
    }

    private void UpdateView()
    {
        int coin = TurntableModel.Instance.GetCoin();
        int betValue = _betValue[_betIndex];
        
        _totalNumText.SetText(coin.ToString());
        _betTotalText.SetText(betValue.ToString());
        _betText.SetText("X "+betValue.ToString());

        _subButton.interactable = _betIndex != 0;
        
        if (_betIndex == _betValue.Count - 1)
        {
            _addButton.interactable = false;
        }
        else
        {
            int index = Math.Min(_betIndex+1, _betValue.Count-1);
            _addButton.interactable = coin >=  _betValue[index];
        }

        _spinRed.gameObject.SetActive(coin >= betValue);
        _spinGrey.gameObject.SetActive(betValue > coin);
    }
    
    private void AddBet()
    {
        _betIndex++;
        _betIndex = Math.Min(_betIndex, _betValue.Count-1);

        UpdateView();
        
        _rollerView.RefreshRewardState(_betValue[_betIndex]);
    }

    private void SubBet()
    {
        _betIndex--;
        _betIndex = Math.Max(_betIndex, 0);
        
        UpdateView();
        
        _rollerView.RefreshRewardState(_betValue[_betIndex]);
    }


    private void SpinTurntable()
    {
        int coin = TurntableModel.Instance.GetCoin();
        int betValue = _betValue[_betIndex];
        if (coin < betValue)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTurntableNoTurntable);
            return;
        }

        _rollerView.RestRewardState();
        
        _animator.Play("twinkle", -1, 0);
        TurntableModel.Instance.SetCoin(-_betValue[_betIndex], "spin");
        UpdateView();

        int index = TurntableModel.Instance.GetPoolIndex();
        UIRoot.Instance.EnableEventSystem = false;

        AudioManager.Instance.PlaySound(157);
        
        int id = index + 1;
        var config = TurntableConfigManager.Instance.TurntableResultConfigList.Find(a=>a.Id == id);
        
        ResData resData = new ResData(config.RewardId, config.RewardNum*_betValue[_betIndex]);
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurntableResult, id.ToString(), _betValue[_betIndex].ToString());
        
        UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TurntableGet
        }, false);
        
        _rollerView.PerformTurntable(index).AddCallBack(() =>
        {
            _animator.Play("loop", -1, 0);
            StartCoroutine(PlayRewardAnim(index, resData));
        });
    }

    private IEnumerator PlayRewardAnim(int index, ResData resData)
    {
        _rollerView.SetRewardState(index);
        yield return new WaitForSeconds(1.5f);
        
        FlyReward(resData);
        UIRoot.Instance.EnableEventSystem = true;
    }
    
    private void FlyReward(ResData resData)
    {
        CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){resData}, CurrencyGroupManager.Instance.currencyController, false);
    }
}