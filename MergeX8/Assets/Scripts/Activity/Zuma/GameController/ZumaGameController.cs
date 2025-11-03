using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using FishEatFishSpace;
using Spine.Unity;
using UnityEngine;
using Random=UnityEngine.Random;

public class ZumaGameController:MonoBehaviour
{
    public bool IsPlaying()
    {
        if (FlyingBallList.Count > 0)
            return true;
        foreach (var road in RoadList)
        {
            if (road.IsPlaying())
                return true;
        }
        return false;
    }
    public List<ZumaRoad> RoadList;
    public ZumaBall NextBall;
    public Transform DefaultBall;
    public Transform NextBallPosition;
    public Transform NextNextBallPosition;
    public List<Collider2D> WallList;
    public StorageZuma Storage;
    public Transform Frog;
    public SpriteRenderer Light;
    public SpriteRenderer FrogArea;
    public List<ZumaBall> FlyingBallList = new List<ZumaBall>();
    public SkeletonAnimation FrogSpine;
    public float BallRaduis => LevelConfig.BallRadius;
    public ZumaModel Model => ZumaModel.Instance;
    public int LeftBallCount => GetLeftBallCount();

    private void Awake()
    {
        if (DefaultBall == null)
        {
            DefaultBall = transform.Find("ball");
        }
        DefaultBall.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventZumaDiceCountChange>(OnBallCountChange);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventZumaDiceCountChange>(OnBallCountChange);
    }

    public int GetLeftBallCount()
    {
        return Storage.BallCount;
    }
    public int GetLeftBombCount()
    {
        return Storage.BombCount;
    }
    public int GetLeftLineCount()
    {
        return Storage.WildCount;
    }

    public void ReduceBall()
    {
        ZumaModel.Instance.ReduceBall(1);
    }
    public void ReduceBomb()
    {
        ZumaModel.Instance.ReduceBomb(1);
    }
    public void ReduceLine()
    {
        ZumaModel.Instance.ReduceLine(1);
    }

    public UIZumaMainController MainUI;
    public GameObjectPoolManager Pool => MainUI.Pool;
    public ZumaLevelConfig LevelConfig;
    public void StartGame(StorageZuma storage,UIZumaMainController mainUI)
    {
        AudioManager.Instance.PlaySoundById(195);
        MainUI = mainUI;
        Storage = storage;
        LevelConfig = Model.GetLevel(Storage.LevelId);
        var i = 0;
        foreach (var road in RoadList)
        {
            road.Game = this;
            road.Reset();
            i++;
        }
        foreach (var flyingBall in FlyingBallList)
        {
            DestroyImmediate(flyingBall.gameObject);
        }
        FlyingBallList.Clear();
        if (NextBall)
            DestroyImmediate(NextBall.gameObject);
        if (LeftBallCount > 0)
        {
            NextBall = Instantiate(DefaultBall,DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
            NextBall.Game = this;
            NextBall.SetSize();
            NextBall.gameObject.SetActive(true);
            NextBall.State = ZumaBallState.None;
            NextBall.Collider.enabled = false;
            NextBall.transform.position = NextBallPosition.position;
            NextBall.Color = Storage.GetColor(Storage.CurBallColor);
        }
        UpdateNextNextBallColor();
        Light.gameObject.SetActive(UsingLine);
    }

    public void UpdateNextNextBallColor()
    {
        var color = Storage.NextBallColor;
        if (GetLeftBallCount() <= 1)
            color = -1;
        if (UseBomb)
        {
            color = Storage.CurBallColor;
            if (GetLeftBallCount() == 0)
                color = -1;
        }
        var showColor = Storage.GetColor(color);
        var skinName = "default";
        if (showColor == ZumaBallColor.Red)
            skinName = "red";
        else if (showColor == ZumaBallColor.Blue)
            skinName = "blue";
        else if (showColor == ZumaBallColor.Green)
            skinName = "green";
        else if (showColor == ZumaBallColor.Purple)
            skinName = "purple";
        else if (showColor == ZumaBallColor.Yellow)
            skinName = "yellow";
        FrogSpine.skeleton.SetSkin(skinName);
        FrogSpine.skeleton.SetSlotsToSetupPose();
        FrogSpine.AnimationState.Apply(FrogSpine.skeleton);
    }

    public bool UsingLine = false;
    public bool UseBomb = false;
    public void ClickBomb()
    {
        if (IsWin)
            return;
        if (UseBomb)
        {
            UseBomb = false;
            UpdateNextNextBallColor();
            if (LeftBallCount == 0)
            {
                NextBall.BreakSelf();
                NextBall = null;
            }
            else
            {
                NextBall.Color = Storage.GetColor(Storage.CurBallColor);
            }
        }
        else
        {
            if (GetLeftBombCount() > 0)
            {
                UseBomb = true;
                UpdateNextNextBallColor();
                if (NextBall != null)
                {
                    NextBall.Color = ZumaBallColor.Bomb;   
                }
                else
                {
                    NextBall = CreateNewBall();
                    NextBall.Color = ZumaBallColor.Bomb;
                }
            }
            else
            {
                UIZumaGiftBagController.Open();
            }
        }
    }

    private void Update()
    {
        if (UsingLine)
        {
            if (NextBall == null)
            {
                Light.gameObject.SetActive(false);
                return;
            }
            Light.gameObject.SetActive(true);
            UpdateLightColor();
            var light = Light;
            var baseLength = light.sprite.rect.height;
            Vector2 direction = LightDirection;
            // 发射射线
            var totalDistance = 5f;
            // MainUI.GameCamera.GetDistanceToScreenSide(light.transform.position, direction);
            LayerMask mask = 1 << 30;
            RaycastHit2D hitInfo = Physics2D.Raycast(light.transform.position, direction, totalDistance,mask);
            // 检查是否击中了某个 Collider2D
            if (hitInfo.collider != null)
            {
                Debug.Log("Hit: " + hitInfo.collider.name);
                var zumaBall = hitInfo.collider.gameObject.GetComponent<ZumaBall>();
                if (zumaBall!= null && /*zumaBall.State !=ZumaBallState.Flying &&*/ zumaBall.State !=ZumaBallState.None)
                {
                    var distance = (hitInfo.collider.transform.position - light.transform.position).magnitude;
                    totalDistance = Math.Min(distance, totalDistance);
                }
            }
            var scaleY = totalDistance / baseLength;
            var tempScale = light.transform.localScale;
            tempScale.y = scaleY/(light.transform.lossyScale.y/tempScale.y);
            light.transform.localScale = tempScale;
        }
    }

    private Vector2 LightDirection = Vector2.down;
    public void ClickLine()
    {
        if (IsWin)
            return;
        if (UsingLine)
        {
            UsingLine = false;
            Light.gameObject.SetActive(false);
        }
        else
        {
            if (GetLeftLineCount() > 0)
            {
                UsingLine = true;
                Light.gameObject.SetActive(true);
            }
            else
            {
                UIZumaGiftBagController.Open();
            }
        }
    }

    public bool IsInFrogArea(Vector3 position)
    {
        Bounds bounds = FrogArea.bounds.To2D();
        // 检查目标位置是否在包围盒内
        return bounds.Contains(position);
    }
    public void OnDrag(Vector3 clickPosition)
    {
        clickPosition.z = 0;
        if (IsInFrogArea(clickPosition))
        {
            return;
        }
        {
            Vector3 direction = clickPosition - Frog.position;

            // 计算角度，注意：Atan2 参数顺序是 Y, X
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 设置旋转，使对象面向目标
            // 如果你的图像默认朝向下方，需要额外调整角度
            Frog.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));   
        }

        {
            var frogPosition = Frog.position;
            frogPosition.z = 0;
            LightDirection = (clickPosition - frogPosition).normalized;
        }
    }

    public void ExchangeBall()
    {
        if (UseBomb)
            return;
        if (LeftBallCount <= 1)
            return;
        (Storage.NextBallColor, Storage.CurBallColor) = (Storage.CurBallColor, Storage.NextBallColor);
        UpdateNextNextBallColor();
        if (NextBall)
        {
            NextBall.Color = Storage.GetColor(Storage.CurBallColor);
        }
    }

    private ZumaBallColor LightColor = ZumaBallColor.Grey;
    public void UpdateLightColor()
    {
        if (NextBall == null)
        {
            return;   
        }
        if (LightColor == NextBall.Color)
            return;
        LightColor = NextBall.Color;
        Light.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.ZumaAtlas,GetLightName(NextBall.Color));
    }

    public string GetLightName(ZumaBallColor color)
    {
        if (color == ZumaBallColor.Red)
        {
            return "LightRed";
        }
        else if (color == ZumaBallColor.Yellow)
        {
            return "LightYellow";
        }
        else if (color == ZumaBallColor.Green)
        {
            return "LightGreen";
        }
        else if (color == ZumaBallColor.Purple)
        {
            return "LightPurple";
        }
        else if (color == ZumaBallColor.Blue)
        {
            return "LightBlue";
        }
        else if (color == ZumaBallColor.Bomb)
        {
            return "LightBomb";
        }
        return "找不到光线图片";
    }

    private bool PerformFly = false;
    public void OnClick(Vector3 clickPosition)
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaHit);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ZumaChangeBall);
        if (IsWin)
            return;
        clickPosition.z = 0;
        if (IsInFrogArea(clickPosition))
        {
            IsGuide = false;
            ExchangeBall();
            return;
        }

        if (NextBall == null)
        {
            UIPopupZumaNoDiceController.Open(Storage);
            return;
        }
        if (ShootLock)
            return;
        foreach (var road in RoadList)
        {
            if (road.IsPerforming)
                return;
        }
        OnDrag(clickPosition);
        FrogSpine.AnimationName = "shoot";
        DOVirtual.DelayedCall(0.4f, () =>
        {
            FrogSpine.AnimationName = "idle";
        }).SetTarget(FrogSpine.transform);
        NextBall.transform.DOKill();
        NextBall.transform.SetParent(transform,true);
        NextBall.StartFly(clickPosition);
        var color = NextBall.Color;
        NextBall = null;
        PerformFly = true;
        if (color == ZumaBallColor.Bomb)
        {
            ReduceBomb();
        }
        else
        {
            if (LeftBallCount == 0)
                return;
            ReduceBall();
            Storage.CurBallColor = Storage.NextBallColor;
            Storage.NextBallColor = GetRandomColor();
        }
        if (UsingLine)
        {
            ReduceLine();
        }
        PerformFly = false;
        if (UseBomb && GetLeftBombCount() > 0)
        {
            NextBall = CreateNewBall();
            NextBall.Color = ZumaBallColor.Bomb;
        }
        else
        {
            UseBomb = false;
            MainUI.UpdateBombViewState();
            if (LeftBallCount > 0)
            {
                NextBall = CreateNewBall();
                NextBall.Color = Storage.GetColor(Storage.CurBallColor);
            }
        }

        if (UsingLine && GetLeftLineCount() == 0)
        {
            UsingLine = false;
            Light.gameObject.SetActive(UsingLine);
            MainUI.UpdateLightViewState();
        }
        UpdateNextNextBallColor();
    }

    public void OnBallCountChange(EventZumaDiceCountChange evt)
    {
        if (!PerformFly&&!UseBomb && NextBall == null && evt.TotalValue > 0)
        {
            NextBall = CreateNewBall();
            NextBall.Color = Storage.GetColor(Storage.CurBallColor);
        }
        UpdateNextNextBallColor();
    }
    public int GetRandomColor()
    {
        var colorList = Storage.GetLeftColors();
        var randomColor = colorList[Random.Range(0, colorList.Count)];
        return randomColor;
    }

    private bool ShootLock = false;
    public ZumaBall CreateNewBall()
    {
        var newBall = Instantiate(DefaultBall,DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
        newBall.Game = this;
        newBall.SetSize();
        newBall.gameObject.SetActive(true);
        newBall.State = ZumaBallState.None;
        newBall.Collider.enabled = false;
        newBall.transform.localPosition = NextNextBallPosition.localPosition;
        newBall.transform.DOLocalMove(NextBallPosition.localPosition, 0.3f);
        transform.DOKill(false);
        ShootLock = true;
        DOVirtual.DelayedCall(0.2f, () => ShootLock = false).SetTarget(transform);
        
        return newBall;
    }

    public bool IsWin = false;
    private bool IsWinWin = false;
    public void OnCheckWin()
    {
        if (!IsWin)
            IsWin = true;
        if (!IsWinWin && !IsPlaying())
        {
            IsWinWin = true;
            var winLevel = Storage.LevelId;
            Storage.CompleteTimes++;
            var nextLevel = Storage.GetNextLevel();
            Storage.StartLevel(nextLevel);
            UIZumaMainController.Instance.OnGameWin(winLevel);
        }
    }

    public bool IsGuide;
    public void StartLevel1Guide()
    {
        var ball = RoadList.Last().Lines.Last().Balls.Last();
        var color = ball.Color;
        Storage.CurBallColor = Storage.GetConfigColor(color);
        NextBall.Color = color;
        IsGuide = true;
        var guideArea = MainUI.GuideClickArea;
        guideArea.gameObject.SetActive(true);
        guideArea.BindGame(this);
        guideArea.transform.position = ball.transform.position;
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaHit, guideArea.transform as RectTransform);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaHit, null))
        {
            GuideTriggerPosition.ZumaHit.WaitGuideFinish().AddCallBack(() =>
            {
                guideArea.gameObject.SetActive(false);
            }).WrapErrors();
        }
    }

    public void StartLevel2Guide()
    {
        IsGuide = true;
        var guideArea = MainUI.GuideClickArea;
        guideArea.gameObject.SetActive(true);
        guideArea.BindGame(this);
        guideArea.transform.position = FrogArea.transform.position;
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaChangeBall, guideArea.transform as RectTransform);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaChangeBall, null))
        {
            GuideTriggerPosition.ZumaChangeBall.WaitGuideFinish().AddCallBack(() =>
            {
                guideArea.gameObject.SetActive(false);
            }).WrapErrors();
        }
    }

    public void StartTipGuide()
    {
        IsGuide = true;
        
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaTip, null))
        {
            GuideTriggerPosition.ZumaTip.WaitGuideFinish().AddCallBack(() =>
            {
                IsGuide = false;
            }).WrapErrors();
        }
    }
}