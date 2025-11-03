using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBlindBoxThemeBase:UIWindowController
{
    public StorageBlindBox Storage;
    public BlindBoxThemeConfig Config;
    public BlindBoxModel Model => BlindBoxModel.Instance;
    public Button RewardBtn;
    public Button BoxBtn;
    public Button CloseBtn;
    public override void PrivateAwake()
    {
        EventDispatcher.Instance.AddEvent<EventBlindBoxRecycleItem>(OnRecycleItem);
    }

    public abstract void OnRecycleItem(EventBlindBoxRecycleItem evt);

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventBlindBoxRecycleItem>(OnRecycleItem);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InitCloseBtn();
        CloseBtn.onClick.AddListener(()=>
        {
            CloseBtn.interactable = false;
            AnimCloseWindow();
        });
        Storage = objs[0] as StorageBlindBox;
        Config = Model.ThemeConfigDic[Storage.ThemeId];
        InitRewardBtn();
        RewardBtn.onClick.AddListener(() =>
        {
            Storage.OpenThemeRewardView();
        });
        InitBoxBtn();
        BoxBtn.onClick.AddListener(() =>
        {
            var itemConfig = Storage.OpenBlindBox();
            if (itemConfig == null)
                return;
            OnGetItem(itemConfig);
        });
        var shieldButtonOnClicks = transform.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var btn in shieldButtonOnClicks)
        {
            btn.isUse = false;
        }

        transform.Find("Root/ButtonRecycle").gameObject.AddComponent<RecycleRepeatItemEntrance>().Init(Storage);
    }
    public abstract void InitCloseBtn();
    public abstract void InitRewardBtn();
    public abstract void InitBoxBtn();
    public abstract void OnGetItem(BlindBoxItemConfig itemConfig);

    public class RecycleRepeatItemEntrance : MonoBehaviour
    {
        private Transform RedPoint;
        private LocalizeTextMeshProUGUI RedPointText;
        private Button Btn;
        private StorageBlindBox Storage;
        public void Init(StorageBlindBox storage)
        {
            Storage = storage;
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                UIPopupBlindBoxRecycleTipController.Open(Storage);
            });
            RedPoint = transform.Find("RedPoint");
            RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
            RedPointText.gameObject.SetActive(false);
            InvokeRepeating("UpdateRedPoint",0,1);
        }

        public void UpdateRedPoint()
        {
            RedPoint.gameObject.SetActive(Storage.GetRecycleCount(out var boxCount) > 0);
        }
    }
}