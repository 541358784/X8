using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIBlindBoxTheme3:UIBlindBoxThemeBase
{
   private Dictionary<int, Theme1Item> ItemDic = new Dictionary<int, Theme1Item>();
   // private LocalizeTextMeshProUGUI BoxCountText;
   private BlingBoxThemeRedPoint RedPoint;
   private BlingBoxThemeRedPoint BoxCountRedPoint;
   private LocalizeTextMeshProUGUI TitleText;
   private Transform BoxBtnEffect1;
   private Transform BoxBtnEffect2;
   protected override void OnOpenWindow(params object[] objs)
   {
      base.OnOpenWindow(objs);
      for (var i = 1; i <= Config.ItemList.Count; i++)
      {
         var itemId = Config.ItemList[i - 1];
         var itemConfig = Model.ItemConfigDic[itemId];
         var themeItem = transform.Find("Root/Content/" + i).gameObject.AddComponent<Theme1Item>();
         themeItem.Init(itemConfig,this);
         ItemDic.Add(itemId,themeItem);
      }
      // BoxCountText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonOpen/NameText");
      // BoxCountText.SetText(Storage.BlindBoxCount.ToString());
      BoxCountRedPoint = transform.Find("Root/ButtonOpen/RedPoint").gameObject.AddComponent<BlingBoxThemeRedPoint>();
      BoxCountRedPoint.Init(Storage,true,false,true);
      RedPoint = transform.Find("Root/ButtonReward/RedPoint").gameObject.AddComponent<BlingBoxThemeRedPoint>();
      RedPoint.Init(Storage,false,true);
      TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleText");
      TitleText.SetTerm(Config.NameKey);
      BoxBtnEffect1 = transform.Find("Root/ButtonOpen/BG");
      BoxBtnEffect2 = transform.Find("Root/ButtonOpen/vfx_BG_01");
      UpdateBoxBtnEffect();
      InvokeRepeating("UpdateBoxBtnEffect",0,1);
   }

   public void UpdateBoxBtnEffect()
   {
      BoxBtnEffect1.gameObject.SetActive(Storage.BlindBoxCount>0);
      BoxBtnEffect2.gameObject.SetActive(Storage.BlindBoxCount>0);
   }

   public override void InitCloseBtn()
   {
      CloseBtn = GetItem<Button>("Root/ButtonClose");
   }
   
   public override void InitRewardBtn()
   {
      RewardBtn = GetItem<Button>("Root/ButtonReward");
   }

   public override void InitBoxBtn()
   {
      BoxBtn = GetItem<Button>("Root/ButtonOpen");
   }

   public override void OnGetItem(BlindBoxItemConfig itemConfig)
   {
      UpdateBoxBtnEffect();
      BoxCountRedPoint.UpdateView();
      Storage.OpenThemeOpenBoxView(itemConfig, () =>
      {
         if (ItemDic.TryGetValue(itemConfig.Id,out var themeItem))
         {
            themeItem.UpdateView();
         }
         // BoxCountText.SetText(Storage.BlindBoxCount.ToString());
      });
   }
   public override void OnRecycleItem(EventBlindBoxRecycleItem evt)
   {
      if (Storage == evt.Storage)
      {
         foreach (var pair in ItemDic)
         {
            pair.Value.UpdateView();
         }
      }
   }
   public class Theme1Item : MonoBehaviour
   {
      public BlindBoxItemConfig Config;
      public UIBlindBoxTheme3 Controller;
      public StorageBlindBox Storage => Controller.Storage;
      private Image NoIcon;
      private Image Icon;
      private Animator IconAnimator;
      private LocalizeTextMeshProUGUI NameText;
      private Button Btn;
      private bool IsCollect;
      private ParticleSystem Effect;
      private Transform NumGroup;
      private LocalizeTextMeshProUGUI NumText;
      public void Init(BlindBoxItemConfig config,UIBlindBoxTheme3 controller)
      {
         Config = config;
         Controller = controller;
         NoIcon = transform.Find("NoIcon").GetComponent<Image>();
         Icon = transform.Find("Icon").GetComponent<Image>();
         NoIcon.sprite = Config.GetItemSprite(true);
         Icon.sprite = Config.GetItemSprite();
         IconAnimator = Icon.GetComponent<Animator>();
         NameText = transform.Find("NameText").GetComponent<LocalizeTextMeshProUGUI>();
         IsCollect = Storage.CollectItems.TryGetValue(Config.Id,out var count);
         NoIcon.gameObject.SetActive(!IsCollect);
         Icon.gameObject.SetActive(IsCollect);
         NameText.SetTerm(Config.NameKey);
         Btn = transform.gameObject.GetComponent<Button>();
         Btn.onClick.AddListener(() =>
         {
            Storage.OpenThemePreviewView(Config);
         });
         Effect = transform.Find("Icon/fx/Particle System").GetComponent<ParticleSystem>();
         var shape = Effect.shape;
         shape.sprite = Config.GetItemSprite();
         Effect.gameObject.SetActive(false);
         NumGroup = transform.Find("Num");
         NumText = transform.Find("Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
         NumGroup.gameObject.SetActive(count > 1);
         NumText.SetText(count.ToString());
      }

      public void UpdateView()
      {
         var isCollect = Storage.CollectItems.TryGetValue(Config.Id,out var count);
         NoIcon.gameObject.SetActive(!isCollect);
         Icon.gameObject.SetActive(isCollect);
         if (!IsCollect && isCollect)
         {
            IconAnimator.Play("appear");
            AudioManager.Instance.PlaySoundById(179);
            Effect.gameObject.SetActive(true);
            DOVirtual.DelayedCall(2f, () =>
            {
               Effect.gameObject.SetActive(false);
            }).SetTarget(Effect.transform);  
         }
         else if (IsCollect && isCollect)
         {
            IconAnimator.Play("appear");
            AudioManager.Instance.PlaySoundById(179);
         }
         IsCollect = isCollect;
         NumGroup.gameObject.SetActive(count > 1);
         NumText.SetText(count.ToString());
      }
   }
}