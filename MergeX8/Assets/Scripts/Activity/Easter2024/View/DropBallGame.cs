using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public partial class DropBallGame : MonoBehaviour
{
    public List<Ball> PlayingBalls = new List<Ball>();
    public Rigidbody2D DefaultBall;
    public List<Collider2D> ResultColliderList = new List<Collider2D>();
    public Transform LuckyGroupTrans;
    public Collider2D LuckyGroupCollider;
    public List<Ball> ExtraBalls = new List<Ball>();
    public Ball NextDropBall;
    public UIEaster2024MainController MainUI;
    private LocalizeTextMeshProUGUI BallCountText;
    private Easter2024LevelConfig CurLevelConfig => MainUI.CurLevelConfig;
    public List<Transform> ResultTransList => MainUI.ResultTransList;
    private Button RecycleFreezingBallBtn;

    private void Awake()
    {
        DefaultBall = transform.Find("BallGroup/DefaultBall").GetComponent<Rigidbody2D>();
        DefaultBall.gravityScale = GravityScale;
        DefaultBall.simulated = false;
        DefaultBall.gameObject.SetActive(false);
        ResultColliderList.Clear();
        for (var i = 0; i < 7; i++)
        {
            var resultCollider = transform.Find("FloorTrigger/Trigger" + (i + 1)).GetComponent<Collider2D>();
            ResultColliderList.Add(resultCollider);
        }

        LuckyGroupTrans = transform.Find("LuckyGroup");
        LuckyGroupCollider = transform.Find("LuckyGroup/Trigger").GetComponent<Collider2D>();
        AddAllEvent();
    }

    private void OnDestroy()
    {
        RemoveAllEvent();
    }

    public StorageEaster2024 Storage;

    public void Init(StorageEaster2024 storage, UIEaster2024MainController mainUI)
    {
        Storage = storage;
        MainUI = mainUI;
        InitCardSelectionGroup();
        InitLuckyPointGroup();
        BallCountText = MainUI.BallCountText;
        RecycleFreezingBallBtn = MainUI.RecycleFreezingBallBtn;
        RecycleFreezingBallBtn.gameObject.SetActive(false);
        RecycleFreezingBallBtn.onClick.AddListener(OnClickRecycleFreezingBallBtn);
        UpdateBallText();
        UpdateRewardText();
        OnWaitEnd();
    }

    public void AddAllEvent()
    {
        EventDispatcher.Instance.AddEvent<EventEaster2024EggCountChange>(OnBallCountChange);
    }

    public void RemoveAllEvent()
    {
        EventDispatcher.Instance.RemoveEvent<EventEaster2024EggCountChange>(OnBallCountChange);
    }

    public void OnBallCountChange(EventEaster2024EggCountChange evt)
    {
        if (Storage == null)
            return;
        UpdateBallText();
    }

    public void UpdateBallText()
    {
        BallCountText.SetText(Storage.BallCount.ToString());
    }

    public async void UpdateRewardText(bool withEffect = false)
    {
        if (!withEffect)
        {
            for (var i = 0; i < ResultTransList.Count; i++)
            {
                if (CurLevelConfig.RewardPool[i] > 0)
                {
                    var resultText = ResultTransList[i].Find("Text").GetComponent<TextMeshPro>();
                    resultText.SetText(CurLevelConfig.RewardPool[i].ToString());
                }
            }
        }
        else
        {
            for (var i = 0; i < ResultTransList.Count; i++)
            {
                if (CurLevelConfig.RewardPool[i] > 0)
                {
                    await XUtility.WaitSeconds(0.3f);
                    if (!this)
                        return;
                    var effect = ResultTransList[i].Find("FX_hint");
                    effect.DOKill();
                    effect.gameObject.SetActive(false);
                    effect.gameObject.SetActive(true);
                    DOVirtual.DelayedCall(2f, () => effect.gameObject.SetActive(false)).SetTarget(effect);
                    var resultText = ResultTransList[i].Find("Text").GetComponent<TextMeshPro>();
                    resultText.SetText(CurLevelConfig.RewardPool[i].ToString());
                }
            }
        }
    }

    private ulong TimeInterval =>
        (ulong) (Easter2024Model.Instance.GlobalConfig.DropBallTimeInterval * XUtility.Second);

    private ulong StartWaitTime;
    public bool IsInWaiting = false;
    private bool NeedWaitAllBallFinish = false;

    public void WaitNextBall(bool needWaitAllBallFinish = false)
    {
        if (NextDropBall)
            NextDropBall.gameObject.SetActive(false);
        StartWaitTime = APIManager.Instance.GetServerTime();
        IsInWaiting = true;
        NeedWaitAllBallFinish = needWaitAllBallFinish;
        MainUI.UpdatePlayBtnUI();
    }

    public void UpdateWait()
    {
        if (IsInWaiting)
        {
            var curTime = APIManager.Instance.GetServerTime();
            if (curTime - StartWaitTime > TimeInterval)
            {
                if (!NeedWaitAllBallFinish || !IsPlaying())
                {
                    IsInWaiting = false;
                    NeedWaitAllBallFinish = false;
                    MainUI.UpdatePlayBtnUI();
                    OnWaitEnd();
                }
            }
        }
    }

    public void OnWaitEnd()
    {
        if (Storage.BallCount > 0)
            CreateNextDropBall();
        SelectCardState = SelectCardState;
    }

    public void UpdateAuto()
    {
        if (IsAuto && !IsInWaiting && !IsSelectCard)
        {
            if (Storage.BallCount > 0)
            {
                OnClickPlayBtn();
            }
        }
    }

    public void OnClickPlayBtn()
    {
        if (IsInWaiting)
            return;
        if (!IsSelectCard)
        {
            if (Easter2024Model.Instance.ReduceEgg(1))
            {
                var dropBall = DropBall();
                Easter2024Model.Instance.BILeftBallCount.Add(dropBall,Easter2024Model.Instance.CurStorageEaster2024Week.BallCount);
                WaitNextBall();
            }
            else
            {
                if (!IsPlaying())
                {
                    UIPopupEaster2024NoEggController.Open();
                }
            }
        }
        else if (SelectCardState.CardType == Easter2024CardType.ExtraBall)
        {
            if (Easter2024Model.Instance.ReduceExtraBallCard(SelectCardState.BallCount))
            {
                DropExtraBall(SelectCardState.BallCount);
                SelectCardState = Easter2024CardState.NoCard;
                WaitNextBall(true);
            }
        }
        else if (SelectCardState.CardType == Easter2024CardType.MultiScore)
        {
            if (Easter2024Model.Instance.ReduceMultiScoreCard(SelectCardState.MultiValue))
            {
                DropMultiBall(SelectCardState.MultiValue);
                SelectCardState = Easter2024CardState.NoCard;
                WaitNextBall();
            }
        }
    }

    public void CreateNextDropBall()
    {
        if (NextDropBall)
        {
            NextDropBall.gameObject.SetActive(true);
            return;
        }

        NextDropBall = Instantiate(DefaultBall.gameObject, DefaultBall.transform.parent).AddComponent<Ball>();
        NextDropBall.gameObject.SetActive(true);
        NextDropBall.Init(this, Easter2024BallType.Normal);
        NextDropBall.transform.localPosition = new Vector3(Random.Range(BallMinX, BallMaxX), 0, 0);
    }

    public Ball DropBall()
    {
        CreateNextDropBall();
        NextDropBall.Init(this, Easter2024BallType.Normal);
        NextDropBall.MultiValue = 1;
        NextDropBall.UpdateUI();
        NextDropBall.Simulated = true;
        PlayingBalls.Add(NextDropBall);
        var returnBall = NextDropBall;
        NextDropBall = null;
        return returnBall;
    }

    public void OnUnSelectMultiScoreCard()
    {
        if (!NextDropBall)
            return;
        if (Storage.BallCount > 0)
        {
            NextDropBall.Init(this, Easter2024BallType.Normal);
            NextDropBall.MultiValue = 1;
            NextDropBall.UpdateUI();
        }
        else
        {
            Destroy(NextDropBall.gameObject);
            NextDropBall = null;
        }
    }

    public void OnSelectMultiScoreCard(int multiValue)
    {
        CreateNextDropBall();
        NextDropBall.Init(this, Easter2024BallType.Multi);
        NextDropBall.MultiValue = multiValue;
        NextDropBall.UpdateUI();
    }

    public void DropMultiBall(int multiValue)
    {
        CreateNextDropBall();
        NextDropBall.Init(this, Easter2024BallType.Multi);
        NextDropBall.MultiValue = multiValue;
        NextDropBall.UpdateUI();
        NextDropBall.Simulated = true;
        PlayingBalls.Add(NextDropBall);
        NextDropBall = null;
    }

    public void OnUnSelectExtraBallCard()
    {
        if (NextDropBall)
            NextDropBall.gameObject.SetActive(true);
        for (var i = 0; i < ExtraBalls.Count; i++)
        {
            ExtraBalls[i].gameObject.SetActive(false);
        }
    }

    public void OnSelectExtraBallCard(int ballCount)
    {
        if (NextDropBall)
            NextDropBall.gameObject.SetActive(false);
        if (ExtraBalls.Count < ballCount)
        {
            for (var i = ExtraBalls.Count; i < ballCount; i++)
            {
                var newBall = Instantiate(DefaultBall.gameObject, DefaultBall.transform.parent).AddComponent<Ball>();
                newBall.gameObject.SetActive(true);
                newBall.Simulated = false;
                newBall.Init(this, Easter2024BallType.Extra);
                ExtraBalls.Add(newBall);
            }
        }
        else
        {
            for (var i = ballCount; i < ExtraBalls.Count; i++)
            {
                ExtraBalls[i].gameObject.SetActive(false);
            }
        }

        var positionList = new List<Vector3>();
        if (ballCount == 3)
        {
            var start = -200f;
            var step = 200f;
            for (var i = 0; i < ballCount; i++)
            {
                var random = Random.Range(0, 2);
                positionList.Add(new Vector3(start + step * i + (random==0?0.1f:-0.1f), 0, 0));
            }
        }
        else if (ballCount == 7)
        {
            var start = -300f;
            var step = 100f;
            for (var i = 0; i < ballCount; i++)
            {
                var random = Random.Range(0, 2);
                positionList.Add(new Vector3(start + step * i + (random==0?0.1f:-0.1f), 0, 0));
            }
        }
        else
        {
            var start = -450f;
            var step = 900f / (ballCount - 1);
            for (var i = 0; i < ballCount; i++)
            {
                var random = Random.Range(0, 2);
                positionList.Add(new Vector3(start + step * i + (random==0?0.1f:-0.1f), 0, 0));
            }
        }

        positionList = positionList.Shuffle();
        for (var i = 0; i < ballCount; i++)
        {
            var newBall = ExtraBalls[i];
            newBall.gameObject.SetActive(true);
            newBall.transform.localPosition = positionList[i];
        }
    }

    public void DropExtraBall(int ballCount)
    {
        for (var i = 0; i < ballCount; i++)
        {
            var newBall = ExtraBalls[0];
            ExtraBalls.RemoveAt(0);
            var rigid = newBall.gameObject.GetComponent<Rigidbody2D>();
            XUtility.WaitSeconds(i * 0.5f, () => rigid.simulated = true);
            PlayingBalls.Add(newBall);
        }
    }

    public void TriggerLuckPoint(Ball ball)
    {
#if UNITY_EDITOR
        if (ball.BallType == Easter2024BallType.DataCollection)
        {
            DataCollectionTriggerLuckPoint(ball);
            return;
        }
#endif
        var addPointCount = 1;
        // if (ball.BallType == Easter2024BallType.Multi)
        // {
        //     addPointCount *= ball.MultiValue;
        // }

        if (Easter2024Model.Instance.AddLuckyPoint(addPointCount))
        {
            if (IsAuto && Storage.LuckyPointCount >= Easter2024Model.Instance.MiniGameNeedLuckyPointCount)
            {
                MainUI.SetAutoState(false);
            }
        }

        var luckyEffect = LuckyGroupTrans.Find("Image (2)/FX_ring");
        if (luckyEffect)
        {
            luckyEffect.DOKill();
            luckyEffect.gameObject.SetActive(false);
            luckyEffect.gameObject.SetActive(true);
            DOVirtual.DelayedCall(2f, () => luckyEffect.gameObject.SetActive(false)).SetTarget(luckyEffect);
        }
        AudioManager.Instance.PlaySound("sfx_easter2024_cross_lucky");
    }

    public void PlayTriggerResultEffect(Ball ball, int resultIndex)
    {
        var effect = ResultTransList[resultIndex].Find("FX_Confetti_1");
        effect.DOKill();
        effect.gameObject.SetActive(false);
        effect.gameObject.SetActive(true);
        DOVirtual.DelayedCall(2f, () => effect.gameObject.SetActive(false)).SetTarget(effect);
        AudioManager.Instance.PlaySound("sfx_easter2024_fall_result");
    }

    public void TriggerResult(Ball ball, int resultIndex)
    {
#if UNITY_EDITOR
        if (ball.BallType == Easter2024BallType.DataCollection)
        {
            DataCollectionTriggerResult(ball, resultIndex);
            return;
        }
#endif
        PlayTriggerResultEffect(ball, resultIndex);
        PlayingBalls.Remove(ball);
        var curLevel = Storage.GetCurLevel();
        var result = curLevel.RewardPool[resultIndex];
        var sendReduceBallBI = Easter2024Model.Instance.BILeftBallCount.TryGetValue(ball, out var biBallLeftCount);
        if (result < 0) //获得卡牌
        {
            var cardPool = new List<Easter2024CardConfig>();
            for (var i = 0; i < Storage.CardRandomPool.Count; i++)
            {
                var cardConfig = Easter2024Model.Instance.CardConfig[Storage.CardRandomPool[i]];
                var cardType = (Easter2024CardType) cardConfig.CardType;
                if (cardType == Easter2024CardType.MultiScore ||
                    cardType == Easter2024CardType.ExtraBall)
                {
                    if (ball.BallType == Easter2024BallType.Multi ||
                        ball.BallType == Easter2024BallType.Extra)
                    {
                        continue;
                    }
                }

                cardPool.Add(cardConfig);
            }

            if (cardPool.Count == 0)
            {
                for (var i = 0; i < curLevel.CardPool.Count; i++)
                {
                    var cardConfig = Easter2024Model.Instance.CardConfig[curLevel.CardPool[i]];
                    if ((Easter2024CardType) cardConfig.CardType == Easter2024CardType.Score &&
                        cardConfig.Id % 100 == 1)
                    {
                        cardPool.Add(cardConfig);
                        break;
                    }
                }
            }

            var weightPool = new List<int>();
            for (var i = 0; i < cardPool.Count; i++)
            {
                weightPool.Add(cardPool[i].Weight);
            }

            var index = Utils.RandomByWeight(weightPool);
            {
                var cardConfig = cardPool[index];
                Storage.CardRandomPool.Remove(cardConfig.Id);
                if (Storage.CardRandomPool.Count == 0)
                {
                    for (var i = 0; i < curLevel.CardPool.Count; i++)
                    {
                        Storage.CardRandomPool.Add(curLevel.CardPool[i]);
                    }
                }

                var cardState = new Easter2024CardState(cardConfig);
                var multi = 1;
                if (cardState.CardType == Easter2024CardType.Score)
                {
                    var addValue = cardConfig.Score;
                    // if (ball.BallType == Easter2024BallType.Multi)
                    // {
                    //     addValue = (int)(addValue * ball.MultiValue);
                    //     multi = ball.MultiValue;
                    // }
                    Easter2024Model.Instance.AddScore(addValue, "ScoreCard",true);
                    TriggerAddScoreGuide();
                    if (sendReduceBallBI)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggChange,
                            "-1",biBallLeftCount.ToString(),"Position="+resultIndex+" AddValue="+addValue);
                    }
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggDropArea,
                        resultIndex.ToString(),addValue.ToString());
                }
                else if (cardState.CardType == Easter2024CardType.ExtraBall)
                {
                    Easter2024Model.Instance.AddExtraBallCard(cardConfig.BallCount, false);
                    if (sendReduceBallBI)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggChange,
                            "-1",biBallLeftCount.ToString(),"Position="+resultIndex+" ExtraBallCard"+cardConfig.BallCount);
                    }
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggDropArea,
                        resultIndex.ToString(),"ExtraBallCard"+cardConfig.BallCount);
                }
                else if (cardState.CardType == Easter2024CardType.MultiScore)
                {
                    Easter2024Model.Instance.AddMultiScoreCard(cardConfig.MultiValue, false);
                    if (sendReduceBallBI)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggChange,
                            "-1",biBallLeftCount.ToString(),"Position="+resultIndex+" MultiScoreCard"+cardConfig.MultiValue);
                    }
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggDropArea,
                        resultIndex.ToString(),"MultiScoreCard"+cardConfig.MultiValue);
                }

                MainUI.PopupGetCard(cardConfig, multi);
            }
            if (IsAuto)
            {
                MainUI.SetAutoState(false);
            }
        }
        else
        {
            var addValue = result;
            var reason = "NormalBall";
            if (ball.BallType == Easter2024BallType.Multi)
            {
                addValue = (int) (addValue * ball.MultiValue);
                reason = "MultiBall";
            }
            else if (ball.BallType == Easter2024BallType.Extra)
            {
                reason = "ExtraBall";
            }
            
            Easter2024Model.Instance.AddScore(addValue, reason,true);
            TriggerAddScoreGuide();
            MainUI.FlyCarrot(ResultColliderList[resultIndex].transform.position, addValue);
            if (sendReduceBallBI)
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggChange,
                    "-1",biBallLeftCount.ToString(),"Position="+resultIndex+" AddValue="+addValue);
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterEggDropArea,
                resultIndex.ToString(),addValue.ToString());
        }

        // XUtility.WaitSeconds(0.5f, () =>
        // {
        //     if (ball)
        //         Destroy(ball.gameObject);
        // });
        Destroy(ball.gameObject);
    }

    public void TriggerAddScoreGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MainGetScore))
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MainGetScore, null))
            {
                {
                    // List<Transform> topLayer = new List<Transform>();
                    // topLayer.Add(MainUI.ShopGroup.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MainStore,
                        MainUI.ShopGroup.transform as RectTransform);
                }
                {
                    // List<Transform> topLayer = new List<Transform>();
                    // topLayer.Add(MainUI.CloseBtn.transform);
                    // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MainGuideClose, MainUI.CloseBtn.transform as RectTransform,
                    //     topLayer: topLayer);
                }
            }
        }
    }

    public bool IsPlaying()
    {
        return PlayingBalls.Count > 0;
    }

    public void UpdateLuckyNodePosition(float deltaTime)
    {
        var moveDistance = LuckyNodeSpeedX * deltaTime;
        var nextPositionX = LuckyGroupTrans.transform.localPosition.x + moveDistance;
        bool speedChange = true;
        while (speedChange)
        {
            speedChange = false;
            if (nextPositionX > LuckyNodeMaxX)
            {
                speedChange = true;
                nextPositionX = LuckyNodeMaxX - (nextPositionX - LuckyNodeMaxX);
                LuckyNodeSpeedX = -LuckyNodeSpeedXValue;
            }

            if (nextPositionX < LuckyNodeMinX)
            {
                speedChange = true;
                nextPositionX = LuckyNodeMinX + (LuckyNodeMinX - nextPositionX);
                LuckyNodeSpeedX = LuckyNodeSpeedXValue;
            }
        }

        var tempP = LuckyGroupTrans.transform.localPosition;
        tempP.x = nextPositionX;
        LuckyGroupTrans.transform.localPosition = tempP;
    }

    public void OnClickRecycleFreezingBallBtn()
    {
        for (var i = 0; i < PlayingBalls.Count; i++)
        {
            if (PlayingBalls[i].CheckSamePosition() || CheckBallInAvailableArea(PlayingBalls[i]))
            {
                var removeEgg = PlayingBalls[i];
                PlayingBalls.RemoveAt(i);
                i--;
                Easter2024Model.Instance.AddEgg(1,"RecycleFreezingBall");
                Destroy(removeEgg.gameObject);
                Debug.LogError("回收停止或者出界的球");
            }
        }
    }

    public bool CheckBallInAvailableArea(Ball ball)
    {
        var localPosition = ball.transform.localPosition;
        if (localPosition.x < -500 || localPosition.x > 500 ||
            localPosition.y < -1000 || localPosition.y > 500)
        {
            return true;
        }

        return false;
    }

    public bool IsInit => MainUI != null;
    public bool IsAuto => MainUI.IsAuto;
    private float timer = 0;
    public static float GravityScale = 1f;
    public static float BallSpeedXValue = 200f;
    private float BallSpeedX = BallSpeedXValue;
    private float BallMaxX = 300;
    private float BallMinX = -300;
    public static float LuckyNodeSpeedXValue = 100f;
    private float LuckyNodeSpeedX = LuckyNodeSpeedXValue;
    private float LuckyNodeMaxX = 214;
    private float LuckyNodeMinX = -214;
    private bool IsFreezing = false;

    private void Update()
    {
#if UNITY_EDITOR
        if (IsInDataCollect)
            return;
#endif
        if (!IsInit)
            return;
        if (IsPlaying())
        {
            if (Physics2D.simulationMode != SimulationMode2D.Script)
                return;
            timer += Time.deltaTime;
            while (timer >= Time.fixedDeltaTime)
            {
                var isFreezing = false;
                timer -= Time.fixedDeltaTime;
                Physics2D.Simulate(Time.fixedDeltaTime);
                for (var i = 0; i < PlayingBalls.Count; i++)
                {
                    if (PlayingBalls[i].CheckSamePosition() || CheckBallInAvailableArea(PlayingBalls[i]))
                    {
                        isFreezing = true;
                        Debug.LogError("球停了或者出界了");
                    }
                }
                IsFreezing = isFreezing;
            }
        }
        else
        {
            IsFreezing = false;
        }

        RecycleFreezingBallBtn.gameObject.SetActive(IsFreezing);
        //动球
        if (NextDropBall && NextDropBall.gameObject.activeInHierarchy)
        {
            var moveDistance = BallSpeedX * Time.deltaTime;
            var nextPositionX = NextDropBall.transform.localPosition.x + moveDistance;
            bool speedChange = true;
            while (speedChange)
            {
                speedChange = false;
                if (nextPositionX > BallMaxX)
                {
                    speedChange = true;
                    nextPositionX = BallMaxX - (nextPositionX - BallMaxX);
                    BallSpeedX = -BallSpeedXValue;
                }

                if (nextPositionX < BallMinX)
                {
                    speedChange = true;
                    nextPositionX = BallMinX + (BallMinX - nextPositionX);
                    BallSpeedX = BallSpeedXValue;
                }
            }

            NextDropBall.transform.localPosition = new Vector3(nextPositionX, 0, 0);
        }

        //动Lucky篮子
        UpdateLuckyNodePosition(Time.deltaTime);
        UpdateWait();
        UpdateAuto();
    }
}

public class ColliderTriggerTools : MonoBehaviour
{
    private Action<Collider2D> Callback;
    private Action<Collider2D> EndCallback;

    public void RegisterCallback(Action<Collider2D> callback)
    {
        Callback = callback;
    }

    public void RegisterEndCallback(Action<Collider2D> endCallback)
    {
        EndCallback = endCallback;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.LogWarning("触发器 OnTriggerEnter ");
        if (Callback != null)
            Callback(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.LogWarning("触发器 OnTriggerStay ");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.LogWarning("触发器 OnTriggerExit ");
        if (EndCallback != null)
            EndCallback(other);
    }
}