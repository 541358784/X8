using System;
using System.Collections;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;
using UnityEngine;


public class MergePigBoxInTaskController: MonoBehaviour
{
    private Image _pigBoxIcon;
    private int _curPigIndex = -1;
    private Transform _full;
    private Transform _buyButton;
    private LocalizeTextMeshProUGUI _diamondText;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _button;
    private Slider _slider;
    private void Awake()
    {
        _button = gameObject.GetComponent<Button>();
        _pigBoxIcon = transform.Find("Root/Icon/Icon").GetComponent<Image>();
        _diamondText = transform.Find("Root/Diamond/DiamondIText").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _full = transform.Find("Root/Full");
        _buyButton = transform.Find("Root/Button");
        _button.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupPigBox);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPiggyBankOpen);
        });
        _slider = transform.Find("Root/Slider").GetComponent<Slider>();
        
        
        InvokeRepeating("Refresh", 0, 1f);
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_VALUE_REFRESH, PigBankValueRefresh);
        UpdatePigBankImage();
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_VALUE_REFRESH, PigBankValueRefresh);

    }
    private void PigBankValueRefresh(BaseEvent e)
    {
        if(!gameObject.activeSelf)
            return;
        UpdatePigBankImage();

    }

    
    public void Refresh()
    {
        var openState = PigBankModel.Instance.IsOpened() && PigBankModel.Instance.IsFull();
        gameObject.SetActive(openState);
        if(!openState)
            return;
        
        _timeText.SetText(PigBankModel.Instance.GetActivityLeftTimeString());
        _diamondText.SetText(PigBankModel.Instance.GetCanCollectValue().ToString());
        var curPigBankTable = PigBankModel.Instance.GetAdPigBankTable();
        if (curPigBankTable != null)
        {
            var maxValue = curPigBankTable.Stage_2;
            var curValue = PigBankModel.Instance.GetCanCollectValue();
            _slider.value = (float) curValue / maxValue;
        }
        UpdatePigBankImage();
    }
    private void UpdatePigBankImage()
    {
        if (!PigBankModel.Instance.IsOpened())
            return;

        if (_pigBoxIcon == null)
            return;

        PigBank pigBankTable = PigBankModel.Instance.GetAdPigBankTable();
        if (pigBankTable == null)
            return;
        _full.gameObject.SetActive(PigBankModel.Instance.IsFull());
        _buyButton.gameObject.SetActive(PigBankModel.Instance.IsFull());
        string imageName = PigBankModel.Instance._mergeBoxImageName[0];
        int index = PigBankModel.Instance.GetCurIndex();

        _curPigIndex = index;
        if (index < PigBankModel.Instance._mergeBoxImageName.Length)
            imageName = PigBankModel.Instance._mergeBoxImageName[index];

        if (_pigBoxIcon.sprite == null || _pigBoxIcon.sprite.name != imageName)
        {
            _pigBoxIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(Decoration.AtlasName.MergeAtlas, imageName);
        }
    }


}
