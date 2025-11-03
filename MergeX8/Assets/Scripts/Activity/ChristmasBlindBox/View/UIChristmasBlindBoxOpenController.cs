using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using GoogleMobileAds.Api;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIChristmasBlindBoxOpenController:UIWindowController
{
    public static UIChristmasBlindBoxOpenController Instance;
    public static UIChristmasBlindBoxOpenController Open(List<BlindBoxItemConfig> itemList)
    {
        if (!Instance)
            Instance = UIManager.Instance.OpenUI(UINameConst.UIChristmasBlindBoxOpen) as UIChristmasBlindBoxOpenController;
        Instance.PerformOpenBox(itemList).WrapErrors();
        return Instance;
    }
    
    private List<BlindBoxItemConfig> ItemList;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            if (!FinishText.gameObject.activeSelf)
                return;
            for (var i = 0; i < ItemList.Count; i++)
            {
                var target = BlindBoxModel.Instance.GetAuxItem();
                var flyObject = RewardItems[i].gameObject;
                FlyGameObjectManager.Instance.FlyObject(flyObject,flyObject.transform.position,target.transform.position,true,scale:0.5f);
                flyObject.gameObject.SetActive(false);
            }
            AnimCloseWindow(() =>
            {
                
            });
        });
        DefaultOpenBoxGroup = transform.Find("Root/UIBlindBoxOpen");
        DefaultOpenBoxGroup.gameObject.SetActive(false);
        for (var i = 1; i <= 9; i++)
        {
            var item = transform.Find("Root/IconGroup/0" + i).GetComponent<Image>();
            item.gameObject.SetActive(false);
            RewardItems.Add(item);
        }
        FinishText = transform.Find("Root/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlayMusic("bgm_xmas", true);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.PlayMusic(1,true);
    }

    private Button CloseBtn;
    private Transform DefaultOpenBoxGroup;
    private List<Image> RewardItems = new List<Image>();
    private LocalizeTextMeshProUGUI FinishText;
    public async Task PerformOpenBox(List<BlindBoxItemConfig> itemList)
    {
        FinishText.gameObject.SetActive(false);
        ItemList = itemList;
        for (var i = 0; i < ItemList.Count; i++)
        {
            var openBoxGroup = Instantiate(DefaultOpenBoxGroup, DefaultOpenBoxGroup.parent).gameObject
                .AddComponent<OpenBoxGroup>();
            openBoxGroup.gameObject.SetActive(true);
            await openBoxGroup.Perform(ItemList[i]);
            if (!this)
                return;
            var rewardItem = RewardItems[i];
            rewardItem.gameObject.SetActive(true);
            var initPosition = rewardItem.transform.localPosition;
            var initScale = rewardItem.transform.localScale;
            rewardItem.sprite = openBoxGroup.Icon.sprite;
            rewardItem.transform.position = openBoxGroup.Icon.transform.position;
            rewardItem.transform.localScale = Vector3.one;
            var moveTime = 0.3f;
            rewardItem.transform.DOScale(initScale, moveTime);
            rewardItem.transform.DOLocalMove(initPosition, moveTime);
            DestroyImmediate(openBoxGroup.gameObject);
            await XUtility.WaitSeconds(moveTime);
            if (!this)
                return;
        }
        FinishText.gameObject.SetActive(true);
    }

    public class OpenBoxGroup : MonoBehaviour
    {
        private Animator Anim;
        public Image Icon;
        private SkeletonGraphic Spine;
        public Task Perform(BlindBoxItemConfig config)
        {
            Spine = transform.Find("Root/SkeletonGraphic (blind_box)").GetComponent<SkeletonGraphic>();
            Anim = transform.GetComponent<Animator>();
            Icon = transform.Find("Root/Icon").GetComponent<Image>();
            Icon.sprite = config.GetItemSprite(false);
            Spine.Skeleton.SetSkin(config.ThemeId.ToString());
            Spine.Skeleton.SetSlotsToSetupPose();
            Spine.AnimationState.Apply(Spine.Skeleton);
            Spine.timeScale = 2;
            AudioManager.Instance.PlaySoundById(211);
            return Anim.PlayAnimationAsync("open");
        }
    }
}