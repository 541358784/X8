using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityLocal.BlindBox.View;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStarrySkyCompassMainController:UIWindowController
{
    public StorageStarrySkyCompass Storage;
    public LocalizeTextMeshProUGUI TimeText;
    public Button CloseBtn;
    public bool IsPerforming;
    public StarrySkyCompassModel Model => StarrySkyCompassModel.Instance;
    public Aux_BlindBox BlindBoxEntrance;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassRocketCountChange>(OnRocketCountChange);
        AudioManager.Instance.PlayMusic(1,true);
    }

    public void OnClickCloseBtn()
    {
        if (IsPerforming)
            return;
        CloseBtn.interactable = false;
        AnimCloseWindow();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageStarrySkyCompass;
        IsPerforming = false;
        InitShopEntrance();
        InitHappyGroup();
        InitRollerView();
        var blindBoxEntrance = transform.Find("Root/BlindBoxEntrance");
        var creator = new DynamicEntry_Home_BindBox();
        BlindBoxEntrance = creator.MonoBehaviour as Aux_BlindBox;
        if (BlindBoxEntrance)
        {
            BlindBoxEntrance.transform.SetParent(blindBoxEntrance,false);
            var rectTransform = BlindBoxEntrance.transform as RectTransform;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            BlindBoxEntrance.UpdateUI();
        }
        CheckSpinGuide();
    }
    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetLeftTimeText());
    }

    public static UIStarrySkyCompassMainController Instance;
    public static UIStarrySkyCompassMainController Open(StorageStarrySkyCompass storageStarrySkyCompass)
    {
        if (Instance && Instance.gameObject.activeSelf)
            return Instance;
        Instance = UIManager.Instance.OpenUI(UINameConst.UIStarrySkyCompassMain, storageStarrySkyCompass) as
            UIStarrySkyCompassMainController;
        return Instance;
    }
}