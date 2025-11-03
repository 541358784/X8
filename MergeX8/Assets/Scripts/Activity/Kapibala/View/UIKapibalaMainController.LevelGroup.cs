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

public partial class UIKapibalaMainController : UIWindowController
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
        var bigLevelConfig = KapibalaModel.Instance.LevelConfig[bigLevel];
        CueBigLevelProgressSlider.value = (float)positionIndex / bigLevelConfig.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText(positionIndex+"/"+bigLevelConfig.SmallLevels.Count);
        LevelText.SetText((bigLevelConfig.Id+1)+"/"+KapibalaModel.Instance.LevelConfig.Count);
        LoadLevel(bigLevel);
        CurLevelGroup.SetKapibalaPosition(positionIndex);
        ScrollContent.SetAnchorPositionY(-(CurLevelGroup.Kapibala as RectTransform).anchoredPosition.y +250);
    }

    public void LoadLevel(int bigLevel)
    {
        if (CurLevelGroup&& CurLevelGroup.Config == KapibalaModel.Instance.LevelConfig[bigLevel])
            return;
        // if (!LevelGroups.TryGetValue(bigLevel, out var levelGroup))
        // {
        //     var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/Kapibala/Level" + bigLevel);
        //     levelGroup = Instantiate(asset).AddComponent<LevelGroup>();
        //     levelGroup.Init(KapibalaModel.Instance.LevelConfig[bigLevel]);
        //     levelGroup.transform.SetParent();
        //     // var scaleX = CameraImage.rectTransform.rect.width / CameraImage.rectTransform.rect.height;
        //     // levelGroup.GameCamera.rect = new Rect((1-scaleX)/2, 0, scaleX, 1);
        //     LevelGroups.Add(bigLevel,levelGroup);
        // }
        var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/Kapibala/Level" + bigLevel);
        var levelGroup = Instantiate(asset).AddComponent<LevelGroup>();
        levelGroup.Init(KapibalaModel.Instance.LevelConfig[bigLevel]);
        levelGroup.transform.SetParent(ScrollContent,false);
        if (CurLevelGroup)
        {
            DestroyImmediate(CurLevelGroup.gameObject);
            // CurLevelGroup.gameObject.SetActive(false);   
        }
        CurLevelGroup = levelGroup;
        CurLevelGroup.gameObject.SetActive(true);
    }
    public async void PerformJumpFail(KapibalaLevelConfig level,int startPosition)
    {
        ShowLevel(level.Id, startPosition);
        ScrollContent.SetAnchorPositionY(-(CurLevelGroup.Kapibala as RectTransform).anchoredPosition.y + 250);
        await CurLevelGroup.PerformJumpFail(startPosition,ScrollContent);
        CueBigLevelProgressSlider.value = (float)0 / level.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText(0+"/"+level.SmallLevels.Count);
        CurLevelGroup.SetKapibalaPosition(0,1f);
        ScrollRect.enabled = false;
        ScrollContent.DOAnchorPosY(-(CurLevelGroup.Kapibala as RectTransform).anchoredPosition.y + 250, 1f).OnComplete(() =>
        {
            ScrollRect.enabled = true;
        });
        CurLevelGroup.UpdateKapibalaRotation(CurLevelGroup.PositionNodes[1].localPosition);
    }
    
    public async void PerformJumpWin(KapibalaLevelConfig level,int startPosition)
    {
        ShowLevel(level.Id, startPosition);
        ScrollContent.SetAnchorPositionY(-(CurLevelGroup.Kapibala as RectTransform).anchoredPosition.y + 250);
        await CurLevelGroup.PerformJumpWin(startPosition);
        CueBigLevelProgressSlider.value = (float)(startPosition+1) / level.SmallLevels.Count;
        CueBigLevelProgressSliderText.SetText((startPosition+1)+"/"+level.SmallLevels.Count);
        if ((startPosition + 1) >= level.SmallLevels.Count)
        {
            await CurLevelGroup.WalkToRewardBox(startPosition + 1);
            var rewards = CommonUtils.FormatReward(level.RewardId, level.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapibalaGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, reason,animEndCall: () =>
                {
                    var nextBigLevel = level.Id + 1;
                    if (nextBigLevel >= KapibalaModel.Instance.LevelConfig.Count)
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
            CurLevelGroup.UpdateKapibalaRotation(CurLevelGroup.PositionNodes[startPosition + 2].localPosition);
        }
    }
    public class LevelGroup : MonoBehaviour
    {
        // public List<Animator> Barriers = new List<Animator>();
        public List<Transform> PositionNodes = new List<Transform>();
        public KapibalaLevelConfig Config;
        public Transform Kapibala;
        public List<IndependentBee> Bees = new List<IndependentBee>();
        public SkeletonGraphic KapibalaAnimator;
        public void Init(KapibalaLevelConfig config)
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

            Kapibala = transform.Find("Kapibala");
            KapibalaAnimator = Kapibala.Find("spine").GetComponent<SkeletonGraphic>();
            var bee1 = transform.Find("Bee1").gameObject.AddComponent<IndependentBee>();
            bee1.ChangeOrbitCenter(Kapibala.Find("RightPoint"));
            Bees.Add(bee1);
            var bee2 = transform.Find("Bee2").gameObject.AddComponent<IndependentBee>();
            bee2.ChangeOrbitCenter(Kapibala.Find("LeftPoint"));
            Bees.Add(bee2);
            foreach (var bee in Bees)
            {
                bee.gameObject.SetActive(true);
            }
        }

        public void SetKapibalaPosition(int positionIndex, float moveTime = 0f,Action updateAction = null)
        {
            var positionNode = PositionNodes[positionIndex];
            Kapibala.localPosition = positionNode.localPosition;
            KapibalaAnimator.PlaySkeletonAnimation("idle",true);
            if (positionIndex + 1 < PositionNodes.Count)
            {
                var nextPosition = PositionNodes[positionIndex + 1].localPosition;
                UpdateKapibalaRotation(nextPosition);   
            }
        }

        public Vector2 JumpOffset = new Vector2(30,-60);
        private const float JumpWinTime = 3.2f;
        private const float JumpFailTime = 5f;
        private const float JumpTime1 = 0.3f;//起跳准备时间
        private const float JumpTime2 = 0.3f;//跳跃时间
        private const float JumpTime3_Win = JumpWinTime - JumpTime1 - JumpTime2 - JumpTime4_Win-JumpTime5_Win;//扒悬崖持续时间赢
        private const float JumpTime3_Fail = JumpFailTime - JumpTime1 - JumpTime2 - JumpTime4_Fail;//扒悬崖持续时间输
        private const float JumpTime4_Win = 0.5f;//从悬崖上爬起时间
        private const float JumpTime5_Win = 1.2f;//从悬崖上爬起时间
        private const float JumpTime4_Fail = 0.3f;//从悬崖上掉落时间
        public async Task PerformJumpFail(int startPosition,RectTransform content)
        {
            DOVirtual.DelayedCall(0.7f, () =>
            {
                AudioManager.Instance.PlaySoundById(199);
            }).SetTarget(transform);
            var lastPosition = PositionNodes[startPosition].localPosition;
            var nextPosition = PositionNodes[startPosition + 1].localPosition;
            // nextPosition = (lastPosition + nextPosition) / 2;
            UpdateKapibalaRotation(nextPosition);
            var middlePosition = new Vector3(nextPosition.x + JumpOffset.x * (Kapibala.localScale.x > 0 ? 1 : -1),
                nextPosition.y + JumpOffset.y, nextPosition.z);
            Kapibala.localPosition = lastPosition;
            KapibalaAnimator.PlaySkeletonAnimation("jump2");
            await XUtility.WaitSeconds(JumpTime1);
            Kapibala.DOLocalMove(middlePosition, JumpTime2).SetEase(Ease.InOutCubic);
            foreach (var bee in Bees)
            {
                bee.Attack();
            }
            await XUtility.WaitSeconds(JumpTime2+JumpTime3_Fail+JumpTime4_Fail);
            var targetY = PositionNodes[0].localPosition.y - 50;
            var distanceY = middlePosition.y - targetY;
            var dropTime = distanceY/500f;
            var dropPosition = new Vector3(middlePosition.x, targetY, middlePosition.z);
            KapibalaAnimator.PlaySkeletonAnimation("falling",true);
            Kapibala.DOLocalMove(dropPosition, dropTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                var curY = -(Kapibala as RectTransform).anchoredPosition.y + 250;
                if (curY > content.anchoredPosition.y)
                {
                    content.SetAnchorPositionY(curY);
                }
            });
            await XUtility.WaitSeconds(dropTime);
            await KapibalaAnimator.PlaySkeletonAnimationAsync("fall_down");
            foreach (var bee in Bees)
            {
                bee.StopAttack();
            }
        }

        public void UpdateKapibalaRotation(Vector3 targetLocalPosition)
        {
            if (targetLocalPosition.x > Kapibala.localPosition.x)
            {
                Kapibala.localScale = new Vector3(-1, 1, 1);
                var leftPoint = Kapibala.Find("LeftPoint") as RectTransform;
                if (leftPoint.anchoredPosition.x < 0)
                {
                    leftPoint.SetAnchorPositionX(-leftPoint.anchoredPosition.x);
                }
                var rightPoint = Kapibala.Find("RightPoint") as RectTransform;
                if (rightPoint.anchoredPosition.x > 0)
                {
                    rightPoint.SetAnchorPositionX(-rightPoint.anchoredPosition.x);
                }
            }
            else
            {
                Kapibala.localScale = new Vector3(1, 1, 1);
                var leftPoint = Kapibala.Find("LeftPoint") as RectTransform;
                if (leftPoint.anchoredPosition.x > 0)
                {
                    leftPoint.SetAnchorPositionX(-leftPoint.anchoredPosition.x);
                }
                var rightPoint = Kapibala.Find("RightPoint") as RectTransform;
                if (rightPoint.anchoredPosition.x < 0)
                {
                    rightPoint.SetAnchorPositionX(-rightPoint.anchoredPosition.x);
                }
            }
            // //调整朝向
            // var targetDirection = targetLocalPosition - Kapibala.localPosition;
            // Vector2 direction = targetDirection.normalized;
            // // 计算从正Y方向顺时针到目标方向的角度
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // // 减去90度，因为原始图片朝向的是y轴正方向
            // angle -= 90f;
            // // 设置箭头的旋转
            // Kapibala.rotation = Quaternion.Euler(0, 0, angle);   
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
            UpdateKapibalaRotation(nextPosition);
            var middlePosition = new Vector3(nextPosition.x + JumpOffset.x * (Kapibala.localScale.x > 0 ? 1 : -1),
                nextPosition.y + JumpOffset.y, nextPosition.z);
            Kapibala.localPosition = lastPosition;
            KapibalaAnimator.PlaySkeletonAnimation("jump");
            await XUtility.WaitSeconds(JumpTime1);
            Kapibala.DOLocalMove(middlePosition, JumpTime2).SetEase(Ease.InOutCubic);
            await XUtility.WaitSeconds(JumpTime2+JumpTime3_Win);
            Kapibala.DOLocalMove(nextPosition, JumpTime4_Win).SetEase(Ease.InOutCubic);
            await XUtility.WaitSeconds(JumpTime4_Win+JumpTime5_Win);
            KapibalaAnimator.PlaySkeletonAnimation("idle",true);
        }
        public async Task WalkToRewardBox(int startPosition)
        {
            // KapibalaAnimator.PlayAnimation("walk");
            var lastPosition = PositionNodes[startPosition].localPosition;
            var nextPosition = PositionNodes[startPosition + 1].localPosition;
            Kapibala.localPosition = lastPosition;
            UpdateKapibalaRotation(nextPosition);
            await PerformJumpWin(startPosition);
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