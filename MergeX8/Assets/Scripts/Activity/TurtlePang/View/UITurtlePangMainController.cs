using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController:UIWindowController
{
    public StorageTurtlePang Storage;
    public LocalizeTextMeshProUGUI TimeText;
    public Button CloseBtn;
    public Transform PackagePile;
    public Button GiftBagEntranceBtn;
    public List<TurtlePangGrid> GridList = new List<TurtlePangGrid>();
    public Transform DefaultTurtle;
    private Button StartBtn;
    public bool IsPerforming;
    private Image LuckyColorIcon;
    public TurtlePangModel Model => TurtlePangModel.Instance;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Top/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        PackagePile = GetItem<Transform>("Root/Top/Image");
        GiftBagEntranceBtn = GetItem<Button>("Root/Top/BuyButton");
        GiftBagEntranceBtn.onClick.AddListener(() =>
        {
            UITurtlePangGiftBagController.Open();
        });
        var gridCount = Model.GlobalConfig.BoardSize * Model.GlobalConfig.BoardSize;
        for (var i = 0; i < gridCount; i++)
        {
            var gridTrans = GetItem<Transform>("Root/Grid/" + (i+1));
            var grid = gridTrans.gameObject.AddComponent<TurtlePangGrid>();
            grid.Init(this,i);
            GridList.Add(grid);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetItem<RectTransform>("Root/Grid"));
        DefaultTurtle = GetItem<Transform>("Root/Ball");
        DefaultTurtle.gameObject.SetActive(false);
        StartBtn = GetItem<Button>("Root/BagGroup/OpenButton");
        LuckyColorIcon = GetItem<Image>("Root/BagGroup/TextGroup/WishColorImage");
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        AudioManager.Instance.PlayMusic("bgm_collision");
        base.OnOpenWindow(objs);
        foreach (var btn in gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true))
        {
            btn.isUse = false;
        }
        Storage = objs[0] as StorageTurtlePang;
        IsPerforming = false;
        InitBagEntrance();
        InitExchangeGroup();
        InitBagExchangeGroup();
        InitGameGroup();
        InitSelectColorGroup();
        InitSelectPackageGroup();
        InitShopEntrance();
        InitAutoPlay();
        var gridCount = Model.GlobalConfig.BoardSize * Model.GlobalConfig.BoardSize;
        for (var i = 0; i < gridCount; i++)
        {
            if (Storage.BoardState.TryGetValue(i, out var value) && value > 0)
            {
                var turtle = Instantiate(DefaultTurtle, DefaultTurtle.parent).gameObject.AddComponent<Turtle>();
                turtle.gameObject.SetActive(true);
                turtle.transform.position = GridList[i].transform.position;
                // turtle.RandomRotation();
                turtle.SetScaleBig();
                var itemConfig = Model.ItemConfig.Find(a => a.Id == value);
                turtle.Init(itemConfig);
                GridList[i].Turtle = turtle;
            }
        }

        if (Storage.LuckyColor > 0)
        {
            var luckyColor = Model.ItemConfig.Find(a => a.Id == Storage.LuckyColor);
            LuckyColorIcon.sprite = luckyColor.GetTurtleIcon();
        }
        else
        {
            LuckyColorIcon.gameObject.SetActive(false);
        }
        PlayLogic();
    }

    public void PlayLogic()
    {
        if (!Storage.IsInGame)
        {
            SelectPackage().AddCallBack(PlayLogic).WrapErrors();
        }
        else if(Storage.LuckyColor == 0)
        {
            SelectColor().AddCallBack(PlayLogic).WrapErrors();
        }
        else if(Storage.BasePackageCount > 0 || Storage.ExtraPackageCount > 0 )
        {
            if (IsAuto)
            {
                Play().AddCallBack(PlayLogic).WrapErrors();
            }
            else
            {
                StartBtn.interactable = true;
                StartBtn.onClick.RemoveAllListeners();
                StartBtn.onClick.AddListener(() =>
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TurtlePangPut);
                    if (IsPerforming)
                        return;
                    StartBtn.interactable = false;
                    Play().AddCallBack(PlayLogic).WrapErrors();
                });
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TurtlePangPut))
                {
                    List<Transform> topLayer = new List<Transform>();
                    topLayer.Add(StartBtn.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TurtlePangPut, StartBtn.transform as RectTransform,
                        topLayer: topLayer);
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TurtlePangPut, null))
                    {
                    
                    }
                }   
            }
        }
        else
        {
            ShowExchange().AddCallBack(() =>
            {
                foreach (var turtle in BagTurtleList)
                {
                    DestroyImmediate(turtle.gameObject);
                }
                BagTurtleList.Clear();
                BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
                PlayLogic();
            }).WrapErrors();
        }
    }
    public async Task Play()
    {
        if (IsPerforming)
            return;
        IsPerforming = true;
        var debugClear = false;
        var debugFull = false;
        if (Model.TurtlePangDebugClear)
        {
            Model.TurtlePangDebugClear = false;
            debugClear = true;
        }
        else if (Model.TurtlePangDebugFull)
        {
            Model.TurtlePangDebugFull = false;
            debugFull = true;
        }
        var debugList = new List<int>();
        if (debugClear)
        {
            debugList.Add(1);
            debugList.Add(1);
            debugList.Add(1);
            debugList.Add(2);
            debugList.Add(2);
            debugList.Add(2);
            debugList.Add(2);
            debugList.Add(2);
            debugList.Add(2);
        }
        else if (debugFull)
        {
            debugList.Add(1);
            debugList.Add(2);
            debugList.Add(3);
            debugList.Add(4);
            debugList.Add(5);
            debugList.Add(6);
            debugList.Add(7);
            debugList.Add(8);
            debugList.Add(9);
        }
        var performList = new List<Action<Action>>();
        var newIndexList = new List<int>();
        //放包加拆包
        var gridCount = Model.GlobalConfig.BoardSize * Model.GlobalConfig.BoardSize;
        for (var i = 0; i < gridCount; i++)
        {
            var grid = GridList[i];
            if (!Storage.BoardState.TryGetValue(i, out var value))
            {
                if (Storage.BasePackageCount > 0)
                {
                    Storage.BoardState.Add(i,0);
                    Storage.BasePackageCount--;
                }
                else if (Storage.ExtraPackageCount > 0)
                {
                    Storage.BoardState.Add(i,0);
                    Storage.ExtraPackageCount--;
                }
                else
                {
                    break;
                }
                var randomList = new List<int>();
                foreach (var itemConfig in Model.ItemConfig)
                {
                    randomList.Add(itemConfig.Weight);
                }
                var randomIdx = debugList.Count>0?debugList[i]:Utils.RandomByWeight(randomList);
                var randomColor = Model.ItemConfig[randomIdx];
                Storage.BoardState[i] = randomColor.Id;
                newIndexList.Add(i);
                performList.Add((callback) =>
                {
                    var package = PackageList.Pop();
                    package.gameObject.SetActive(true);
                    PackageCountText.SetTermFormats(PackageList.Count.ToString());
                    PerformPutPackage(package,grid, () => OpenPackage(grid, randomColor)).AddCallBack(callback).WrapErrors();
                });
            }
        }
        performList.Add(async (callback) =>
        {
            UpdatePackageGroupLayout();
            await XUtility.WaitSeconds(0.3f + 0.5f);
            callback();
        });
        //幸运色结算
        var luckySendCount = 0;
        var luckyGridList = new List<int>();
        for (var i = 0; i < newIndexList.Count; i++)
        {
            var index = newIndexList[i];
            if (Storage.BoardState.TryGetValue(index, out var value) && value == Storage.LuckyColor)
            {
                luckyGridList.Add(index);
                luckySendCount+=Model.GlobalConfig.LuckyColorSendCount;
                Storage.ExtraPackageCount += Model.GlobalConfig.LuckyColorSendCount;
            }
        }

        if (luckySendCount > 0)
        {
            performList.Add(async (callback) =>
            {
                foreach (var luckyIdx in luckyGridList)
                {
                    GridList[luckyIdx].Turtle.PerformLucky();
                    ShowEffect(EffectType.Lucky);
                    await PerformBasePackage(Model.GlobalConfig.LuckyColorSendCount);
                    await XUtility.WaitSeconds(0.1f);
                }
                await XUtility.WaitSeconds(0.3f);
                callback.Invoke();
            });
        }
        //bingo结算
        var triggerList = new List<List<int>>();
        //横线
        for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
        {
            int color = 0;
            var trigger = true;
            for (var i1 = 0; i1 < Model.GlobalConfig.BoardSize; i1++)
            {
                var index = i * Model.GlobalConfig.BoardSize + i1;
                if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                {
                    trigger = false;
                    break;
                }
                else
                {
                    if (color == 0)
                        color = value;
                    else if (color != value)
                    {
                        trigger = false;
                        break;
                    }
                }
            }
            if (trigger)
            {
                var indexList = new List<int>();
                for (var i1 = 0; i1 < Model.GlobalConfig.BoardSize; i1++)
                {
                    var index = i * Model.GlobalConfig.BoardSize + i1;
                    indexList.Add(index);
                    Storage.BoardState.Remove(index);
                }
                triggerList.Add(indexList);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += indexList.Count;
                Storage.ExtraPackageCount += Model.GlobalConfig.DrawLineSendCount;
            }
        }
        //竖线
        for (var i1 = 0; i1 < Model.GlobalConfig.BoardSize; i1++)
        {
            int color = 0;
            var trigger = true;
            for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
            {
                var index = i * Model.GlobalConfig.BoardSize + i1;
                if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                {
                    trigger = false;
                    break;
                }
                else
                {
                    if (color == 0)
                        color = value;
                    else if (color != value)
                    {
                        trigger = false;
                        break;
                    }
                }
            }
            if (trigger)
            {
                var indexList = new List<int>();
                for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
                {
                    var index = i * Model.GlobalConfig.BoardSize + i1;
                    indexList.Add(index);
                    Storage.BoardState.Remove(index);
                }
                triggerList.Add(indexList);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += indexList.Count;
                Storage.ExtraPackageCount += Model.GlobalConfig.DrawLineSendCount;
            }
        }
        //右斜线
        {
            int color = 0;
            var trigger = true;
            for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
            {
                var i1 = i;
                var index = i * Model.GlobalConfig.BoardSize + i1;
                if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                {
                    trigger = false;
                    break;
                }
                else
                {
                    if (color == 0)
                        color = value;
                    else if (color != value)
                    {
                        trigger = false;
                        break;
                    }
                }
            }
            if (trigger)
            {
                var indexList = new List<int>();
                for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
                {
                    var i1 = i;
                    var index = i * Model.GlobalConfig.BoardSize + i1;
                    indexList.Add(index);
                    Storage.BoardState.Remove(index);
                }
                triggerList.Add(indexList);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += indexList.Count;
                Storage.ExtraPackageCount += Model.GlobalConfig.DrawLineSendCount;
            }         
        }
        //左斜线
        {
            int color = 0;
            var trigger = true;
            for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
            {
                var i1 = Model.GlobalConfig.BoardSize-1-i;
                var index = i * Model.GlobalConfig.BoardSize + i1;
                if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                {
                    trigger = false;
                    break;
                }
                else
                {
                    if (color == 0)
                        color = value;
                    else if (color != value)
                    {
                        trigger = false;
                        break;
                    }
                }
            }
            if (trigger)
            {
                var indexList = new List<int>();
                for (var i = 0; i < Model.GlobalConfig.BoardSize; i++)
                {
                    var i1 = Model.GlobalConfig.BoardSize-1-i;
                    var index = i * Model.GlobalConfig.BoardSize + i1;
                    indexList.Add(index);
                    Storage.BoardState.Remove(index);
                }
                triggerList.Add(indexList);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += indexList.Count;
                Storage.ExtraPackageCount += Model.GlobalConfig.DrawLineSendCount;
            }         
        }
        if (triggerList.Count > 0)
        {
            foreach (var itemGroup in triggerList)
            {
                performList.Add(async (callback) =>
                {
                    ShowEffect(EffectType.Line);
                    foreach (var itemIdx in itemGroup)
                    {
                        GridList[itemIdx].Turtle.PerformBingo();
                    }
                    await XUtility.WaitSeconds(0.3f);
                    foreach (var itemIdx in itemGroup)
                    {
                        var turtle = GridList[itemIdx].Turtle;
                        GridList[itemIdx].Turtle = null;
                        BagTurtleList.Add(turtle);
                        BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
                        turtle.PerformFly(GetRandomBagAreaPosition(),0.5f);
                    }
                    await XUtility.WaitSeconds(0.5f);
                    callback();
                });  
                performList.Add((callback) =>
                {
                    PerformBasePackage(Model.GlobalConfig.DrawLineSendCount).AddCallBack(callback).WrapErrors();
                });
            }
        }
        //对对碰
        var triggerPangList = new List<List<int>>();
        for (var i = 0; i < gridCount; i++)
        {
            if (!Storage.BoardState.TryGetValue(i, out var value1) || value1 == 0)
                continue;
            for (var i1 = i+1; i1 < gridCount; i1++)
            {
                if (!Storage.BoardState.TryGetValue(i1, out var value2) || value2 == 0)
                    continue;
                if (value1 == value2)
                {
                    Storage.BoardState.Remove(i);
                    Storage.BoardState.Remove(i1);
                    Storage.BagGame.TryAdd(value1, 0);
                    Storage.BagGame[value1] += 2;
                    Storage.ExtraPackageCount += Model.GlobalConfig.SameColorSendCount;
                    triggerPangList.Add(new List<int>(){i,i1});
                    break;
                }
            }
        }

        if (triggerPangList.Count > 0)
        {
            foreach (var itemGroup in triggerPangList)
            {
                performList.Add(async (callback) =>
                {
                    ShowEffect(EffectType.Pang);
                    foreach (var itemIdx in itemGroup)
                    {
                        GridList[itemIdx].Turtle.PerformPang();
                    }
                    await XUtility.WaitSeconds(0.3f);
                    foreach (var itemIdx in itemGroup)
                    {
                        var turtle = GridList[itemIdx].Turtle;
                        GridList[itemIdx].Turtle = null;
                        BagTurtleList.Add(turtle);
                        BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
                        turtle.PerformFly(GetRandomBagAreaPosition(),0.5f);
                    }
                    await XUtility.WaitSeconds(0.5f);
                    callback();
                });  
                performList.Add((callback) =>
                {
                    PerformBasePackage(Model.GlobalConfig.SameColorSendCount).AddCallBack(callback).WrapErrors();
                });
            }
        }
        //清场
        if (Storage.BoardState.Count == 0)
        {
            Storage.ExtraPackageCount += Model.GlobalConfig.CleanBoardSendCount;
            performList.Add(async (callback) =>
            {
                ShowEffect(EffectType.Clear);
                //清场表演
                await XUtility.WaitSeconds(0.3f);
                callback();
            }); 
            performList.Add((callback) =>
            {
                PerformBasePackage(Model.GlobalConfig.CleanBoardSendCount).AddCallBack(callback).WrapErrors();
            });
        }
        //十三不靠
        if (Storage.BoardState.Count == gridCount)
        {
            for (var i = 0; i < gridCount; i++)
            {
                var color = Storage.BoardState[i];
                Storage.BoardState.Remove(i);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += 1;
            }
            Storage.ExtraPackageCount += Model.GlobalConfig.FillBoardSendCount;
            performList.Add(async (callback) =>
            {
                ShowEffect(EffectType.Full);
                foreach (var grid in GridList)
                {
                    grid.Turtle.PerformFillBoard();
                }
                await XUtility.WaitSeconds(0.3f);
                foreach (var grid in GridList)
                {
                    var turtle = grid.Turtle;
                    grid.Turtle = null;
                    BagTurtleList.Add(turtle);
                    BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
                    turtle.PerformFly(GetRandomBagAreaPosition(),0.5f);
                }
                await XUtility.WaitSeconds(0.5f);
                callback();
            });  
            performList.Add((callback) =>
            {
                PerformBasePackage(Model.GlobalConfig.FillBoardSendCount).AddCallBack(callback).WrapErrors();
            });
        }
        //GameOver
        if (Storage.BasePackageCount == 0 && Storage.ExtraPackageCount == 0)
        {
            var keyList = Storage.BoardState.Keys.ToList();
            foreach (var key in keyList)
            {
                var color = Storage.BoardState[key];
                Storage.BoardState.Remove(key);
                Storage.BagGame.TryAdd(color, 0);
                Storage.BagGame[color] += 1;
            }
            performList.Add(async (callback) =>
            {
                foreach (var key in keyList)
                {
                    var grid = GridList[key];
                    var turtle = grid.Turtle;
                    grid.Turtle = null;
                    BagTurtleList.Add(turtle);
                    BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
                    turtle.PerformFly(GetRandomBagAreaPosition(),0.5f);
                }
                await XUtility.WaitSeconds(0.5f);
                callback();
            });
        }
        foreach (var perform in performList)
        {
            var task = new TaskCompletionSource<bool>();
            perform(() => task.SetResult(true));
            await task.Task;
        }
        IsPerforming = false;
    }
    public async Task PerformBasePackage(int addCount)
    {
        var packageList = new List<Transform>();
        var visibleList = new List<bool>();
        for (var i = 0; i < addCount; i++)
        {
            var package = Instantiate(PackageDefaultItem, PackageDefaultItem.parent);
            package.gameObject.SetActive(true);
            packageList.Add(package);
            PackageList.Add(package);
        }
        UpdatePackageGroupLayout();
        for (var i = 0; i < packageList.Count; i++)
        {
            var package = packageList[i];
            visibleList.Add(package.gameObject.activeSelf);
            package.gameObject.SetActive(false);
        }

        for (var i = 0; i < packageList.Count; i++)
        {
            var package = packageList[i];
            var targetPos = package.position;
            package.position = PackagePile.position;
            package.gameObject.SetActive(true);
            var packageCanvas = package.gameObject.AddComponent<Canvas>();
            packageCanvas.overrideSorting = true;
            packageCanvas.sortingOrder = canvas.sortingOrder + 1;
            var visible = visibleList[i];
            var showCount = PackageList.Count - packageList.Count + i + 1;
            AudioManager.Instance.PlaySoundById(181);
            package.DOMove(targetPos, 0.2f).OnComplete(() =>
            {
                DestroyImmediate(packageCanvas);
                package.gameObject.SetActive(visible);
                PackageCountText.SetTermFormats(showCount.ToString());
            });
            await XUtility.WaitSeconds(0.05f);
        }
        await XUtility.WaitSeconds(0.2f);
    }
    public async Task PerformPutPackage(Transform package,TurtlePangGrid grid,Action openPackageAction = null)
    {
        AudioManager.Instance.PlaySoundById(182);
        grid.Package = package;
        package.SetParent(transform,true);
        package.DOMove(grid.transform.position, 0.2f).OnComplete(() =>
        {
            openPackageAction?.Invoke();
        });
        await XUtility.WaitSeconds(0.1f);
    }

    public async void OpenPackage(TurtlePangGrid grid,TurtlePangItemConfig randomColor)
    {
        await XUtility.WaitSeconds(0.1f);
        var package = grid.Package;
        var skeleton = package.Find("Spine").GetComponent<SkeletonGraphic>();
        skeleton.timeScale = 3;
        skeleton.PlaySkeletonAnimationAsync("animation")
            .AddCallBack(()=>DestroyImmediate(package.gameObject)).WrapErrors();
        grid.Package = null;
        var turtle = Instantiate(DefaultTurtle, DefaultTurtle.parent).gameObject.AddComponent<Turtle>();
        turtle.gameObject.SetActive(true);
        turtle.transform.position = grid.transform.position;
        // turtle.RandomRotation();
        turtle.SetScaleBig();
        turtle.Init(randomColor);
        grid.Turtle = turtle;
    }
    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetLeftTimeText());
    }

    public static UITurtlePangMainController Instance;
    public static UITurtlePangMainController Open(StorageTurtlePang storageTurtlePang)
    {
        if (Instance && Instance.gameObject.activeSelf)
            return Instance;
        Instance = UIManager.Instance.OpenUI(UINameConst.UITurtlePangMain, storageTurtlePang) as
            UITurtlePangMainController;
        return Instance;
    }
}