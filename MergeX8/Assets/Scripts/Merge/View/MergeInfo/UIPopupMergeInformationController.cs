using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using System;
using System.Drawing;
using System.Linq;
using Activity.Matreshkas.Model;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using Spine.Unity;
using Image = UnityEngine.UI.Image;

public partial class UIPopupMergeInformationController : UIWindow
{
    private GameObject packageItem1;
    private GameObject packageItem2;
    [NonSerialized] private TableMergeItem itemConfig;
    private TableMergeLine mergeLine;
    private Action CloseEvent;

    private Transform contnet;
    private Transform reContnet;
    private Transform nextContnet;
    
    private Button getResourceBut;
    private Button goShopBut;
    private GameObject _buttonGroup;

    private GameObject bottomGroupObj;
    private GameObject nextGroupObj;

    private Animator _animator;

    private Image _iconImage;

    private GameObject _bigGg;
    private GameObject _smallGg;
    public int mergeId
    {
        get
        {
            if (itemConfig == null)
                return -1;
            return itemConfig.id;
        }
    }

    private LocalizeTextMeshProUGUI titleText;
    private LocalizeTextMeshProUGUI productText;
    private LocalizeTextMeshProUGUI contenText;
    private RectTransform SizeRect;
    public override void PrivateAwake()
    {
        AnimGroupPrivateAwake();
        SizeRect = transform.Find("Root/Size") as RectTransform;
        _animator = transform.GetComponent<Animator>();
        nextGroupObj = transform.Find("Root/NextLevelGroup").gameObject;
        nextGroupObj.gameObject.SetActive(false);
        
        contnet = transform.Find("Root/ContentGroup/UnitList/Viewport/Content");
        reContnet = transform.Find("Root/BottomGroup/ScrolView/Viewport/Content");
        nextContnet = transform.Find("Root/NextLevelGroup/ScrolView/Viewport/Content");
        
        bottomGroupObj = transform.Find("Root/BottomGroup").gameObject;
        _iconImage = transform.Find("Root/ContentGroup/Title/Image").GetComponent<Image>();
        _bigGg = transform.Find("Root/BGGroup/Comme_PopupBG_Big").gameObject;
        _smallGg = transform.Find("Root/BGGroup/Comme_PopupBG_Mid").gameObject;
        
        BindClick("Root/ContentGroup/CloseButton", (go) =>
        {
            CloseUI();
        });

        titleText = transform.Find("Root/ContentGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        productText = transform.Find("Root/BottomGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        contenText = transform.Find("Root/Text/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        packageItem1 = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeInformationCell1");
        packageItem2 = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeInformationCell2");

        getResourceBut = transform.Find("Root/ButtonGroup/GetButton").GetComponent<Button>();
        _buttonGroup = transform.Find("Root/ButtonGroup").gameObject;
        
        getResourceBut.onClick.AddListener(() =>
        {
            DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.sfx_pop_close);
            AnimCloseWindow(() =>
            {
                //特殊处理
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo))
                {
                    MergeResourceManager.Instance.GetMergeResource(GameConfigManager.Instance.GetItemConfig(100003),MergeBoardEnum.Main);
                }
                else
                {
                    MergeResourceManager.Instance.GetMergeResource(itemConfig,MergeBoardEnum.Main);
                }

                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo))
                    MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.None, MergeBoardEnum.Main,true);

                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CloseItemInfo);
                if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
                    MergeGuideLogic.Instance.CheckMergeGuide();
            });
        });

        goShopBut = transform.Find("Root/ButtonGroup/FlashSaleButton").GetComponent<Button>();
        goShopBut.gameObject.SetActive(UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DescPurchase));
        goShopBut.onClick.AddListener(() =>
        {
            UIStoreController.OpenUI("info", ShowArea.flash_sale);
        });
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CloseItemInfo))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(getResourceBut.transform);
            
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CloseItemInfo,
                getResourceBut.transform as RectTransform, topLayer:topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CloseItemInfo, "");
        }

        SetResourcesActive(false);
        EventDispatcher.Instance.DispatchEvent(EventEnum.REWARD_POPUP);
    }

    public void SetInfomations(TableMergeItem config, Action close = null, bool isShowGetResource = true)
    {
        if (itemConfig == config)
        {
            CloseEvent = close;
            return;
        }
        
        CommonUtils.DestroyAllChildren(contnet);
        CommonUtils.DestroyAllChildren(reContnet);
        CommonUtils.DestroyAllChildren(nextContnet);

        CloseEvent = close;
        this.itemConfig = config;
        mergeLine = GameConfigManager.Instance.GetMergeLine(itemConfig.in_line);
        if(mergeLine != null && mergeLine.output != null && mergeLine.output.Length > 0)
        {
            if (mergeLine.output.Length > 8)
            {
                SizeRect.SetHeight(660);
            }
            else
            {
                SizeRect.SetHeight(490);
            }
        }
        _iconImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
        
        InitTitleItem();
        InitItems();
        InitProduct();
        InstantiateNextCell(config);
        _buttonGroup.gameObject.SetActive(false);
        
        if (itemConfig.showText)
        {
            contenText.transform.parent.gameObject.SetActive(true);
            // contenText.SetTerm(itemConfig.item_des);
            var txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
            contenText.SetText(txt);
            if (txt.Contains("{0}"))
            {
                string msg = string.Format(txt, itemConfig.value);
                if (itemConfig.type ==  (int) MergeItemType.diamonds || itemConfig.type ==  (int) MergeItemType.decoCoin || 
                         itemConfig.type ==  (int) MergeItemType.energy || itemConfig.type ==  (int) MergeItemType.exp || 
                         itemConfig.type == (int)MergeItemType.activityKey|| itemConfig.type == (int)MergeItemType.dogCookies ||
                         itemConfig.type == (int)MergeItemType.climbTreeBanana || itemConfig.type == (int)MergeItemType.easter)
                {
                    msg = string.Format(txt, itemConfig.value);
                }
                else if (itemConfig.type ==  (int) MergeItemType.timeBooster)
                {
                    int t = itemConfig.booster_factor;
                    // if( itemConfig.booster_factor>=60)
                    //     t=t/60;
                    msg = string.Format(txt, t);
                }   
                contenText.SetText(msg);
            }
        }
        else
        {
            contenText.transform.parent.gameObject.SetActive(false);
        }
        IniAnimGroup();
        if (itemConfig?.type != (int) MergeItemType.item)
        {
            getResourceBut.gameObject.SetActive(false);
        }
        else
        {
            //特殊处理
            if(GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo))
                getResourceBut.gameObject.SetActive(true);
            else
            {
                if (isShowGetResource)
                {
                    isShowGetResource = MergeResourceManager.Instance.HaveMergeItem(itemConfig,MergeBoardEnum.Main);
                    getResourceBut.gameObject.SetActive(isShowGetResource);
                }
                else
                {
                    getResourceBut.gameObject.SetActive(false);
                }
            }
        }
        
        if(isShowGetResource && itemConfig.type !=(int)MergeItemType.climbTreeBanana && itemConfig.type !=(int)MergeItemType.choiceChest)
            goShopBut.gameObject.SetActive(UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DescPurchase));
        else
        {
            goShopBut.gameObject.SetActive(false);
        }
        
        _buttonGroup.gameObject.SetActive(goShopBut.gameObject.activeSelf || getResourceBut.gameObject.activeSelf);
        
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform.Find("Root/ContentGroup/CloseButton"));
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.InfoDes, transform.Find("Root/ContentGroup/CloseButton") as RectTransform, targetParam:config.id.ToString(), topLayer:topLayer);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.InfoDes, config.id.ToString(), config.id.ToString());
    }

    public void InitTitleItem()
    {
        titleText.SetTerm(itemConfig.name_key);
    }

    private void InitItems()
    {
        if(mergeLine == null || mergeLine.output == null || mergeLine.output.Length == 0)
            return;
        
        int count = mergeLine.output.Length;
        if(!mergeLine.customSort)
            Array.Sort(mergeLine.output, (x, y) => x - y);

        int index = 0;
        for (int i = 0; i < count; i++)
        {
            var config = GameConfigManager.Instance.GetItemConfig(mergeLine.output[i]);
            if (config == null)
                continue;

            var item = Instantiate(packageItem1, contnet);
            item.SetActive(true);

            var script = item.gameObject.GetComponentDefault<MergeInfoUpCell>();
            script.Init(config, config == itemConfig);
            if (config == itemConfig)
                index = i;
        }

        // RectTransform rectTransform = contnet as RectTransform;
        // if (index >= 8 && count > 12)
        // {
            // rectTransform.anchoredPosition = new Vector2(0, 144);
        // }
        // else
        // {
        //     rectTransform.anchoredPosition = new Vector2(0, 0);
        // }
    }

    private void InitProduct()
    {
        bottomGroupObj.SetActive(true);
        _bigGg.gameObject.SetActive(true);
        _smallGg.gameObject.SetActive(false);

        var outPut = itemConfig.output;
        if (itemConfig.subType == (int)SubType.Matreshkas)
        {
            if (MatreshkasModel.Instance.IsOpened())
            {
                 var queue = MatreshkasModel.Instance.GetMatreshkasPresetQueue();
                 HashSet<int> presetQueue = new HashSet<int>();
                 foreach (var id in queue)
                     presetQueue.Add(id);

                 outPut = presetQueue.ToArray();
            }
;        }
        
        if (outPut != null && outPut.Length > 0 && outPut[0] > 0)
        {
            for (int i = 0; i < outPut.Length; i++)
            {
                int id = outPut[i];
                var config = GameConfigManager.Instance.GetItemConfig(id);
                if (config == null)
                    continue;

                InstantiateDownCell(config);
            }
            
            productText.SetTerm("ui_merge_item_information_building");
        }
        else
        {
            productText.SetTerm("ui_merge_item_information_material");
            
            if (itemConfig.re_line == null || itemConfig.re_line.Length == 0 ||
                (itemConfig.re_line.Length == 1 && itemConfig.re_line[0] == -1))
            {
                bottomGroupObj.SetActive(false);
                _bigGg.gameObject.SetActive(false);
                _smallGg.gameObject.SetActive(true);
                return;
            }
        
            for (int i = 0; i < itemConfig.re_line.Length; i++)
            {
                int id = itemConfig.re_line[i];
                var config = GameConfigManager.Instance.GetItemConfig(id);
                if (config == null)
                    continue;

                var mergeLine = GameConfigManager.Instance.GetMergeLine(config.in_line);
                if(mergeLine == null)
                    continue;
                
                if(mergeLine.output == null || mergeLine.output.Length == 0 || mergeLine.output[0] == 0)
                    continue;

                bool isFind = false;
                for (int j = mergeLine.output.Length - 1; j >= 0; j--)
                {
                    if(id > mergeLine.output[j])
                        continue;
                    
                    bool isUnlock = MergeManager.Instance.IsShowedItemId(mergeLine.output[j]);
                    if(!isUnlock)
                        continue;

                    var outConfig = GameConfigManager.Instance.GetItemConfig(mergeLine.output[j]);
                    if (outConfig == null)
                        continue;
                    
                    if(outConfig.output == null || outConfig.output.Length == 1 && outConfig.output[0] == 0)
                        continue;
                    
                    isFind = true;
                    
                    InstantiateDownCell(outConfig);
                    break;
                }
                
                if(isFind)
                    continue;
                
                InstantiateDownCell(config);
            }
        }
    }

    private void InstantiateDownCell(TableMergeItem config)
    {
        var item = Instantiate(packageItem2, reContnet);
        item.SetActive(true);

        var script = item.gameObject.GetComponentDefault<MergeInfoDownCell>();
        script.Init(config);
    }

    private void InstantiateNextCell(TableMergeItem config)
    {
        nextGroupObj.gameObject.SetActive(false);
        
        if(config.next_level <= 0)
            return;

        var nextConfig = GameConfigManager.Instance.GetItemConfig(config.next_level);
        if(nextConfig == null)
            return;
        
        if(nextConfig.output == null || nextConfig.output.Length == 0)
            return;
        
        var nextOutput = new List<int>(nextConfig.output);
        if (config.output != null && config.output.Length > 0)
        {
            foreach (var outId in config.output)
            {
                if (nextOutput.Contains(outId))
                    nextOutput.Remove(outId);
            }
        }
        
        if(nextOutput.Count == 0)
            return;

        nextGroupObj.gameObject.SetActive(true);
        foreach (var outId in nextOutput)
        {
            var outConfig = GameConfigManager.Instance.GetItemConfig(outId);
            if(outConfig == null)
                continue;
            
            var item = Instantiate(packageItem2, nextContnet);
            item.SetActive(true);

            var script = item.gameObject.GetComponentDefault<MergeInfoDownCell>();
            script.Init(outConfig);
        }
    }
    
    public override void ClickUIMask()
    {
        CloseUI();
    }

    private void CloseUI()
    {
        if (!canClickMask)
            return;

        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo) || GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ClickTaskNeedItem))
            return;

        canClickMask = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true);
                CloseEvent?.Invoke();
            }));
        
    }
    public void SetResourcesActive(bool active)
    {
        getResourceBut.gameObject.SetActive(active);
        goShopBut.gameObject.SetActive(active);
    }

    public bool IsResourcesActive()
    {
        return getResourceBut.gameObject.activeSelf;
    }
}