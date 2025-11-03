using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public partial class UIKapiTileMainController : UIWindowController
{
    // public Dictionary<int,LevelGroup> LevelGroups = new Dictionary<int,LevelGroup>();
    private LevelGroup CurLevelGroup;
    private RectTransform ScrollContent;
    private ScrollRect ScrollRect;
    private Slider CueBigLevelProgressSlider;
    private LocalizeTextMeshProUGUI CueBigLevelProgressSliderText;
    // private Image BigLevelRewardIcon;
    private RewardBoxTip TipBoard;
    private Button TipBtn;
    
    public void InitLevelGroup()
    {
        CueBigLevelProgressSlider = transform.Find("Root/TopGroup/Slider").GetComponent<Slider>();
        CueBigLevelProgressSliderText =
            transform.Find("Root/TopGroup/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        TipBoard = transform.Find("Root/TopGroup/Slider/RewardItem/Tips").gameObject.AddComponent<RewardBoxTip>();
        TipBtn = transform.Find("Root/TopGroup/Slider/RewardItem/TipsBtn").GetComponent<Button>();
        TipBtn.onClick.AddListener(() =>
        {
            var rewards = CommonUtils.FormatReward(CurLevelGroup.Config.RewardId, CurLevelGroup.Config.RewardNum);
            TipBoard.ShowTip(rewards,true);
        });
        ScrollContent = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        ScrollRect = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        // DestroyActions.Add(() =>
        // {
        //     foreach (var levelGroup in LevelGroups)
        //     {
        //         DestroyImmediate(levelGroup.Value.gameObject);   
        //     }
        // });
        ShowLevel(Storage.BigLevel, Storage.SmallLevel);
        
        var rewards = CommonUtils.FormatReward(CurLevelGroup.Config.RewardId, CurLevelGroup.Config.RewardNum);
        TipBoard.ShowTip(rewards,true);

    }

    public void ShowLevel(int bigLevel, int positionIndex)
    {
        var bigLevelConfig = KapiTileModel.Instance.LevelConfig[bigLevel];
        CueBigLevelProgressSlider.value = (float)positionIndex / bigLevelConfig.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText(positionIndex+"/"+bigLevelConfig.SmallLevels.Count);
        LevelText.SetText((bigLevelConfig.Id+1)+"/"+KapiTileModel.Instance.LevelConfig.Count);
        LoadLevel(bigLevel);
        CurLevelGroup.SetKapiTilePosition(positionIndex);
        // ScrollContent.SetAnchorPositionY(-(CurLevelGroup.KapiTile as RectTransform).anchoredPosition.y +250);
    }

    public void LoadLevel(int bigLevel)
    {
        if (CurLevelGroup&& CurLevelGroup.Config == KapiTileModel.Instance.LevelConfig[bigLevel])
            return;
        // if (!LevelGroups.TryGetValue(bigLevel, out var levelGroup))
        // {
        //     var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/KapiTile/Level" + bigLevel);
        //     levelGroup = Instantiate(asset).AddComponent<LevelGroup>();
        //     levelGroup.Init(KapiTileModel.Instance.LevelConfig[bigLevel]);
        //     levelGroup.transform.SetParent();
        //     // var scaleX = CameraImage.rectTransform.rect.width / CameraImage.rectTransform.rect.height;
        //     // levelGroup.GameCamera.rect = new Rect((1-scaleX)/2, 0, scaleX, 1);
        //     LevelGroups.Add(bigLevel,levelGroup);
        // }
        var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/KapiTile/Level" + bigLevel);
        var levelGroup = Instantiate(asset).AddComponent<LevelGroup>();
        levelGroup.Init(KapiTileModel.Instance.LevelConfig[bigLevel]);
        levelGroup.transform.SetParent(ScrollContent,false);
        if (CurLevelGroup)
        {
            DestroyImmediate(CurLevelGroup.gameObject);
            // CurLevelGroup.gameObject.SetActive(false);   
        }
        CurLevelGroup = levelGroup;
        CurLevelGroup.gameObject.SetActive(true);
    }
    public async void PerformJumpFail(KapiTileLevelConfig level,int startPosition)
    {
        StartBtn.interactable = false;
        ShowLevel(level.Id, startPosition);
        // ScrollContent.SetAnchorPositionY(-(CurLevelGroup.KapiTile as RectTransform).anchoredPosition.y + 250);
        await CurLevelGroup.PerformJumpFail(startPosition,ScrollContent);
        if (!this)
            return;
        CueBigLevelProgressSlider.value = (float)0 / level.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText(0+"/"+level.SmallLevels.Count);
        CurLevelGroup.SetKapiTilePosition(0,1f);
        // ScrollRect.enabled = false;
        // ScrollContent.DOAnchorPosY(-(CurLevelGroup.KapiTile as RectTransform).anchoredPosition.y + 250, 1f).OnComplete(() =>
        // {
        //     ScrollRect.enabled = true;
        // });
        CurLevelGroup.UpdateKapiTileRotation(CurLevelGroup.PositionNodes[1].localPosition);
        StartBtn.interactable = true;
    }
    
    public async void PerformJumpWin(KapiTileLevelConfig level,int startPosition)
    {
        StartBtn.interactable = false;
        ShowLevel(level.Id, startPosition);
        // ScrollContent.SetAnchorPositionY(-(CurLevelGroup.KapiTile as RectTransform).anchoredPosition.y + 250);
        await CurLevelGroup.PerformJumpWin(startPosition);
        if (!this)
            return;
        CueBigLevelProgressSlider.value = (float)(startPosition+1) / level.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText((startPosition+1)+"/"+level.SmallLevels.Count);
        if ((startPosition + 1) >= level.SmallLevels.Count)
        {
            await CurLevelGroup.WalkToRewardBox(startPosition + 1);
            var rewards = CommonUtils.FormatReward(level.RewardId, level.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapiTile
            };
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, reason,animEndCall: () =>
                {
                    var nextBigLevel = level.Id + 1;
                    if (nextBigLevel >= KapiTileModel.Instance.LevelConfig.Count)
                    {
                        AnimCloseWindow();
                    }
                    else
                    {
                        ShowLevel(nextBigLevel, 0);   
                    }
                });
        }
        else
        {
            CurLevelGroup.UpdateKapiTileRotation(CurLevelGroup.PositionNodes[startPosition + 2].localPosition);
        }
        StartBtn.interactable = true;
    }
    public class LevelGroup : MonoBehaviour
    {
        // public List<Animator> Barriers = new List<Animator>();
        public List<Transform> PositionNodes = new List<Transform>();
        public KapiTileLevelConfig Config;
        public Transform KapiTile;

        private SkeletonGraphic KapiTileAnimator;
        
        private SkeletonGraphic DragonAnimator;
        private SkeletonGraphic PrincessAnimator;
        public void Init(KapiTileLevelConfig config)
        {
            Config = config;
            // for (var i = 0; i < Config.SmallLevels.Count; i++)
            // {
            //     var barrier = transform.Find("Barrier" + i)?.Find("Root/langan@skin").GetComponent<Animator>();
            //     if (barrier == null)
            //     {
            //         Debug.LogError("死耗子关卡"+Config.Id+"_"+i+"跨栏资源错误");
            //     }
            //     else
            //     {
            //         Barriers.Add(barrier);   
            //     }
            // }

            for (var i = 0; i <= Config.SmallLevels.Count+1; i++)
            {
                var position = transform.Find("Position" + i);
                if (position == null)
                {
                    Debug.LogError("死耗子关卡"+Config.Id+"_"+i+"定位坐标资源错误");
                }
                else
                {
                    PositionNodes.Add(position);   
                }
            }

            KapiTile = transform.Find("Kapibala");
            KapiTileAnimator = KapiTile.Find("spine").GetComponent<SkeletonGraphic>();

            DragonAnimator = transform.Find("BG/SkeletonGraphic (RY_loong)").GetComponent<SkeletonGraphic>();
            PrincessAnimator = transform.Find("BG/SkeletonGraphic (RY_loong)/SkeletonGraphic (RY_priness)").GetComponent<SkeletonGraphic>();
            DragonAnimator.PlaySkeletonAnimation("sleep",true);
            PrincessAnimator.PlaySkeletonAnimation("sad",true);
        }

        public void SetKapiTilePosition(int positionIndex, float moveTime = 0f,Action updateAction = null)
        {
            var positionNode = PositionNodes[positionIndex];
            KapiTile.localPosition = positionNode.localPosition;
            KapiTileAnimator.PlaySkeletonAnimation("idle",true);
            if (positionIndex + 1 < PositionNodes.Count)
            {
                var nextPosition = PositionNodes[positionIndex + 1].localPosition;
                UpdateKapiTileRotation(nextPosition);   
            }
        }

        public Vector2 JumpOffset = new Vector2(107,-60);
        private const float JumpTime1 = 0.3f;//起跳准备时间
        private const float JumpTime2 = 0.7f;//跳跃时间
        private const float JumpTime2_Fail = 0.3f;//跳跃时间
        public async Task PerformJumpFail(int startPosition,RectTransform content)
        {
            DOVirtual.DelayedCall(0.7f, () =>
            {
                AudioManager.Instance.PlaySoundById(199);
            }).SetTarget(transform);
            var lastPosition = PositionNodes[startPosition].localPosition;
            var nextPosition = PositionNodes[startPosition + 1].localPosition;
            // nextPosition = (lastPosition + nextPosition) / 2;
            UpdateKapiTileRotation(nextPosition);
            var middlePosition = new Vector3(nextPosition.x + JumpOffset.x * (KapiTile.localScale.x > 0 ? 1 : -1),
                nextPosition.y + JumpOffset.y, nextPosition.z);
            KapiTile.localPosition = lastPosition;
            KapiTileAnimator.PlaySkeletonAnimationAsync("jump2").AddCallBack(() =>
            {
                KapiTileAnimator.PlaySkeletonAnimation("lose",true);
            });
            await XUtility.WaitSeconds(JumpTime1);
            if (!this)
                return;
            KapiTile.DOLocalMove(middlePosition, JumpTime2_Fail).SetEase(Ease.Linear);
            if (!this)
                return;
            await XUtility.WaitSeconds(2f);
            // var targetY = PositionNodes[0].localPosition.y - 50;
            // var distanceY = middlePosition.y - targetY;
            // var dropTime = distanceY/500f;
            // var dropPosition = new Vector3(middlePosition.x, targetY, middlePosition.z);
            // KapiTileAnimator.PlaySkeletonAnimation("falling",true);
            // KapiTile.DOLocalMove(dropPosition, dropTime).SetEase(Ease.Linear).OnUpdate(() =>
            // {
            //     var curY = -(KapiTile as RectTransform).anchoredPosition.y + 250;
            //     if (curY > content.anchoredPosition.y)
            //     {
            //         content.SetAnchorPositionY(curY);
            //     }
            // });
            // await XUtility.WaitSeconds(dropTime);
            // await KapiTileAnimator.PlaySkeletonAnimationAsync("fall_down");
        }

        public void UpdateKapiTileRotation(Vector3 targetLocalPosition)
        {
            if (targetLocalPosition.x > KapiTile.localPosition.x)
            {
                KapiTile.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                KapiTile.localScale = new Vector3(1, 1, 1);
            }
            // //调整朝向
            // var targetDirection = targetLocalPosition - KapiTile.localPosition;
            // Vector2 direction = targetDirection.normalized;
            // // 计算从正Y方向顺时针到目标方向的角度
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // // 减去90度，因为原始图片朝向的是y轴正方向
            // angle -= 90f;
            // // 设置箭头的旋转
            // KapiTile.rotation = Quaternion.Euler(0, 0, angle);   
        }

        public async Task PerformJumpWin(int startPosition)
        {
            DOVirtual.DelayedCall(0.7f, () =>
            {
                AudioManager.Instance.PlaySoundById(198);
            }).SetTarget(transform);
            var lastPosition = PositionNodes[startPosition].localPosition;
            var nextPosition = PositionNodes[startPosition + 1].localPosition;
            // nextPosition = (lastPosition + nextPosition) / 2;
            UpdateKapiTileRotation(nextPosition);
            var middlePosition = new Vector3(nextPosition.x + JumpOffset.x * (KapiTile.localScale.x > 0 ? 1 : -1),
                nextPosition.y + JumpOffset.y, nextPosition.z);
            KapiTile.localPosition = lastPosition;
            KapiTileAnimator.PlaySkeletonAnimation("jump");
            await XUtility.WaitSeconds(JumpTime1);
            // KapiTile.DOLocalMove(middlePosition, JumpTime2).SetEase(Ease.InOutCubic);
            // await XUtility.WaitSeconds(JumpTime2+JumpTime3_Win);
            // KapiTile.DOLocalMove(nextPosition, JumpTime4_Win).SetEase(Ease.InOutCubic);
            // await XUtility.WaitSeconds(JumpTime4_Win+JumpTime5_Win);
            KapiTile.DOLocalMove(nextPosition, JumpTime2).SetEase(Ease.InOutCubic);
            await XUtility.WaitSeconds(JumpTime2);
            if (!this)
                return;
            KapiTileAnimator.PlaySkeletonAnimation("idle",true);
        }
        public async Task WalkToRewardBox(int startPosition)
        {
            // KapiTileAnimator.PlayAnimation("walk");
            var lastPosition = PositionNodes[startPosition].localPosition;
            var nextPosition = PositionNodes[startPosition + 1].localPosition;
            KapiTile.localPosition = lastPosition;
            UpdateKapiTileRotation(nextPosition);
            // await PerformJumpWin(startPosition);
            DragonAnimator.PlaySkeletonAnimationAsync("hitted").AddCallBack(() =>
            {
                DragonAnimator.PlaySkeletonAnimation("stun",true);
            }).WrapErrors();
            await KapiTileAnimator.PlaySkeletonAnimationAsync("hit");
            PrincessAnimator.PlaySkeletonAnimation("happy");
            KapiTileAnimator.PlaySkeletonAnimation("win",true);
            await XUtility.WaitSeconds(2f);
        }
    }
    
    public class RewardBoxTip : MonoBehaviour
    {
        public void ShowTip(List<ResData> resDatas, bool autoClose = true)
        {
            gameObject.SetActive(true);
            Init(resDatas);
            if (autoClose)
                StartAutoClosePopup();
        }

        private Transform _rewardItem;
        private List<CommonRewardItem> _itemList;

        public void Awake()
        {
            _rewardItem = transform.Find("Item");
            _rewardItem.gameObject.SetActive(false);
            _itemList = new List<CommonRewardItem>();
        }

        private Coroutine con;

        public void StartAutoClosePopup()
        {
            con = StartCoroutine(AutoClosePopup());
        }

        private IEnumerator AutoClosePopup()
        {
            yield return new WaitForSeconds(3f); // 等待3秒钟

            HidePopup(); // 3秒后关闭提示框
        }

        public void HidePopup()
        {
            if (con != null)
            {
                StopCoroutine(con);
                con = null;
            }
            gameObject.gameObject.SetActive(false);
        }

        void Update()
        {
            // 检测点击任意位置关闭
            if (Input.GetMouseButtonUp(0))
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                if (results.Count == 0)
                    return;
                foreach (var result in results)
                {
                    if (result.gameObject.transform.parent.gameObject == gameObject)
                        return;
                }

                HidePopup();
            }
        }

        public void Init(List<ResData> resDatas)
        {
            foreach (var item in _itemList)
            {
                DestroyImmediate(item.gameObject);    
            }
            _itemList.Clear();
            for (int i = 0; i < resDatas.Count; i++)
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
                item.gameObject.SetActive(true);
                _itemList.Add(item);
                item.Init(resDatas[i]);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}