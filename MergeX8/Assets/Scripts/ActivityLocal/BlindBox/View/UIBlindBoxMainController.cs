using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIBlindBoxMainController:UIWindowController
{
    public static UIBlindBoxMainController Open()
    {
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxMainController>(UINameConst.UIBlindBoxMain);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(UINameConst.UIBlindBoxMain) as UIBlindBoxMainController;
        return openView;
    }
    private BlindBoxModel Model => BlindBoxModel.Instance;
    private Dictionary<int, ThemeEntranceItem> ThemeItemDic = new Dictionary<int, ThemeEntranceItem>();
    private Button CloseBtn;
    private RecycleShopEntrance RecycleShopBtn;
    public override void PrivateAwake()
    {
        
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>
        {
            CloseBtn.interactable = false;
            AnimCloseWindow();
        });
        for (var i = 1; transform.Find("Root/Content/" + i); i++)
        {
            var item = transform.Find("Root/Content/" + i).gameObject.AddComponent<ThemeEntranceItem>();
            Model.ThemeConfigDic.TryGetValue(i, out var config);
            item.Init(config);
        }

        RecycleShopBtn = transform.Find("Root/ButtonRecycle").gameObject.AddComponent<RecycleShopEntrance>();
    }

    public class RecycleShopEntrance : MonoBehaviour
    {
        private Transform RedPoint;
        private LocalizeTextMeshProUGUI RedPointText;
        private Button Btn;

        private void Awake()
        {
            RedPoint = transform.Find("RedPoint");
            RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
            RedPointText.gameObject.SetActive(false);
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                UIBlindBoxRecycleController.Open();
            });
            InvokeRepeating("UpdateRedPoint",0,1);
        }

        public void UpdateRedPoint()
        {
            var count = 0;
            foreach (var shopConfig in BlindBoxModel.Instance.RecycleShopConfig)
            {
                if (BlindBoxModel.Instance.StorageGlobal.RecycleValue >= shopConfig.RecyclePrice)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                RedPoint.gameObject.SetActive(true);
                RedPointText.SetText(count.ToString());
            }
            else
            {
                RedPoint.gameObject.SetActive(false);
            }
        }
    }
    
    public class ThemeEntranceItem : MonoBehaviour
    {
        private BlindBoxModel Model => BlindBoxModel.Instance;
        private BlindBoxThemeConfig Config;
        private Transform CommingSoon;
        private Button Btn;
        private Slider DownloadSlider;
        private LocalizeTextMeshProUGUI DownLoadSliderText;
        private Button DownloadBtn;
        private Transform DownloadGroup;
        private StorageBlindBox Storage;
        private BlingBoxThemeRedPoint RedPoint;

        public void Init(BlindBoxThemeConfig config)
        {
            CommingSoon = transform.Find("GameObject");
            Config = config;
            if (Config == null)
            {
                CommingSoon.gameObject.SetActive(true);
                return;
            }
            Storage = Model.GetStorage(Config.Id);
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                if (Config == null)
                    return;
                if (!Storage.IsResReady())
                {
                    OnClickDownloadBtn();
                    return;
                }
                Storage.OpenThemeView();
            });
            DownloadGroup = transform.Find("DownLoad");
            DownloadSlider = transform.Find("DownLoad/ProgressSlider").GetComponent<Slider>();
            DownLoadSliderText = transform.Find("DownLoad/ProgressSlider/ProgressText")
                .GetComponent<LocalizeTextMeshProUGUI>();
            DownloadSlider.gameObject.SetActive(false);
            DownloadBtn = transform.Find("DownLoad/DownloadButton").GetComponent<Button>();
            DownloadGroup.gameObject.SetActive(!Storage.IsResReady());
            DownloadBtn.gameObject.SetActive(true);
            DownloadBtn.onClick.AddListener(OnClickDownloadBtn);
            RedPoint = transform.Find("RedPoint").gameObject.AddComponent<BlingBoxThemeRedPoint>();
            RedPoint.Init(Storage,true,true);
        }

        private bool InDownload = false;
        public void OnClickDownloadBtn()
        {
            if (InDownload)
                return;
            InDownload = true;
            DownloadSlider.gameObject.SetActive(true);
            DownloadSlider.value = 0;
            DownLoadSliderText.SetText("0%");
            Storage.TryDownloadRes((p,s) =>
            {
                DownloadSlider.value = p;
                DownLoadSliderText.SetText(((int)(p*100))+"%");
                
            }, (success) =>
            {
                DownloadSlider.gameObject.SetActive(false);
                DownloadGroup.gameObject.SetActive(!Storage.IsResReady());
                InDownload = false;
            });
            DownloadBtn.gameObject.SetActive(false);
        }
    }
}