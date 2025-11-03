using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class UIStarrySkyCompassMainController
{
    public List<StarrySkyCompassRollerView> RollerViewList = new List<StarrySkyCompassRollerView>();
    public Transform FinalNode;
    public LocalizeTextMeshProUGUI FinalNodeText;
    public List<StarrySkyCompassRollerView> HappyRollerViewList = new List<StarrySkyCompassRollerView>();
    public Transform HappyFinalNode;
    public LocalizeTextMeshProUGUI HappyFinalNodeText;
    public Transform RollerView;
    public Transform HappyRollerView;
    public Button StartBtn;
    public Transform Rocket;
    public Transform HappyRocket;
    public Vector3 RocketInitLocalPosition;
    public Vector3 HappyRocketInitLocalPosition;
    private LocalizeTextMeshProUGUI RocketNumText;
    private Transform FinalEffect;
    private Transform HappyFinalEffect;
    public void InitRollerView()
    {
        RocketNumText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        RocketNumText.SetText(Storage.RocketCount.ToString());
        EventDispatcher.Instance.AddEvent<EventStarrySkyCompassRocketCountChange>(OnRocketCountChange);
        Rocket = transform.Find("Root/Turntable1/Image");
        RocketInitLocalPosition = Rocket.localPosition;
        HappyRocket = transform.Find("Root/Turntable2/Image");
        HappyRocketInitLocalPosition = HappyRocket.localPosition;
        StartBtn = GetItem<Button>("Root/ButtonStart");
        StartBtn.onClick.AddListener(OnClickSpinBtn);
        RollerView = transform.Find("Root/Turntable1");
        HappyRollerView = transform.Find("Root/Turntable2");
        RollerView.gameObject.SetActive(true);
        HappyRollerView.gameObject.SetActive(true);
        FinalEffect = transform.Find("Root/Turntable1/fx_reward_0");
        HappyFinalEffect = transform.Find("Root/Turntable2/fx_reward_0");
        for (var i=0;i < Model.TurntableConfig.Count;i++)
        {
            var config = Model.TurntableConfig[i];
            var effect = transform.Find("Root/Turntable1/fx_reward_" + (3 - i));
            var rollerView = new StarrySkyCompassRollerView(transform.Find("Root/Turntable1/Reward"+(3-i)),config,false,Storage,effect);
            rollerView.Init();
            RollerViewList.Add(rollerView);
            rollerView.RefreshRewardState();
        }

        var finalRewardConfig = Model.ResultConfig.Find(a => a.Level == 4);
        FinalNode = transform.Find("Root/Turntable1/Reward0/0");
        FinalNodeText = GetItem<LocalizeTextMeshProUGUI>("Root/Turntable1/Reward0/0/Star/Text");
        FinalNodeText.SetText(finalRewardConfig.Score.ToString());
        
        for (var i=0;i < Model.TurntableConfig.Count;i++)
        {
            var config = Model.TurntableConfig[i];
            var effect = transform.Find("Root/Turntable2/fx_reward_" + (3 - i));
            var rollerView = new StarrySkyCompassRollerView(transform.Find("Root/Turntable2/Reward"+(3-i)),config,true,Storage,effect);
            rollerView.Init();
            HappyRollerViewList.Add(rollerView);
            rollerView.RefreshRewardState();
        }
        HappyFinalNode = transform.Find("Root/Turntable2/Reward0/0");
        HappyFinalNodeText = GetItem<LocalizeTextMeshProUGUI>("Root/Turntable2/Reward0/0/Star/Text");
        HappyFinalNodeText.SetText(finalRewardConfig.HappyScore.ToString());
        
        RollerView.gameObject.SetActive(!Storage.IsInHappyTime());
        HappyRollerView.gameObject.SetActive(Storage.IsInHappyTime());
        LastIsHappySpin = Storage.IsInHappyTime();
        if (LastIsHappySpin)
        {
            AudioManager.Instance.PlayMusic("bgm_star_1");
        }
        else
        {
            AudioManager.Instance.PlayMusic("bgm_star_1");
        }
    }

    public void OnRocketCountChange(EventStarrySkyCompassRocketCountChange evt)
    {
        RocketNumText.SetText(Storage.RocketCount.ToString());
    }
    public void InitRocketPosition()
    {
        HappyRocket.localPosition = HappyRocketInitLocalPosition;
        Rocket.localPosition = RocketInitLocalPosition;
    }

    private bool LastIsHappySpin = false;
    public void UpdateTurntableView()
    {
        var isHappySpin = Storage.IsInHappyTime();
        if (LastIsHappySpin != isHappySpin)
        {
            LastIsHappySpin = isHappySpin;
            RollerView.gameObject.SetActive(!LastIsHappySpin);
            HappyRollerView.gameObject.SetActive(LastIsHappySpin);
            HappyTimeGroup.gameObject.SetActive(LastIsHappySpin);
            UpdateHappyProgress();
            // if (LastIsHappySpin)
            // {
            //     AudioManager.Instance.PlayMusic("bgm_star_1");
            // }
            // else
            // {
            //     AudioManager.Instance.PlayMusic("bgm_star_1");
            // }
        }
        foreach (var roller in RollerViewList)
        {
            roller.RefreshRewardState();
        }
        foreach (var roller in HappyRollerViewList)
        {
            roller.RefreshRewardState();
        }
    }
    public async void OnClickSpinBtn()
    {
        if (IsPerforming)
            return;
        if (!(Storage.RocketCount > 0))
        {
            UIPopupStarrySkyCompassNoItemController.Open();
            return;
        }
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StarrySkyCompassSpin1);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StarrySkyCompassSpin2);
        StarrySkyCompassModel.Instance.AddRocket(-1, "Spin");
        var isHappySpin = Storage.IsInHappyTime();
        StarrySkyCompassResultConfig result = null;
        if (GuideSpin)
        {
            GuideSpin = false;
            result =  Model.GuideSpin();
        }
        else
        {
            result =  Model.Spin();
        }

        var performList = new List<Func<Task>>();
        if (isHappySpin)
        {
            for (var i = 0; i < HappyRollerViewList.Count; i++)
            {
                var roller = HappyRollerViewList[i];
                var findResult = result.Level > roller.Config.Id ? 0 : result.Id;
                var indexList = new List<int>();
                for (var i1 = 0; i1 < roller.Config.TurntableResultList.Count; i1++)
                {
                    if (roller.Config.TurntableResultList[i1] == findResult)
                    {
                        indexList.Add(i1);
                    }
                }
                var index = indexList[Random.Range(0, indexList.Count)];
                performList.Add(async () =>
                {
                    await roller.PerformTurntable(index);
                    var resultTransform = roller.GetElementTransform(index);
                    if (result.Level > roller.Config.Id)
                    {
                        AudioManager.Instance.PlaySoundById(175);
                        HappyRocket.DOMove(resultTransform.position, 0.5f);
                        await XUtility.WaitSeconds(0.5f);   
                    }
                });
                if (result.Level == roller.Config.Id)
                {
                    if (result.HappyScore > 0)
                    {
                        performList.Add(async () =>
                        {
                            roller.WinEffect.gameObject.SetActive(true);
                            await XUtility.WaitSeconds(0.5f);
                            var task = new TaskCompletionSource<bool>();
                            FlyCarrot(HappyRocket.position, result.HappyScore, () =>
                            {
                                task.SetResult(true);
                            });
                            await task.Task;
                            roller.WinEffect.gameObject.SetActive(false);
                            UpdateTurntableView();
                            InitRocketPosition();
                            await CheckHappyGuide1();
                        });   
                    }
                    break;
                }
            }
            if (result.Level == 4)
            {
                performList.Add(async () =>
                {
                    // HappyRocket.DOMove(HappyFinalNode.position, 0.5f);
                    // await XUtility.WaitSeconds(0.5f);
                    HappyFinalEffect.gameObject.SetActive(true);
                    await XUtility.WaitSeconds(0.5f);
                    var task = new TaskCompletionSource<bool>();
                    FlyCarrot(HappyRocket.position, result.HappyScore, () =>
                    {
                        task.SetResult(true);
                    });
                    await task.Task;
                    HappyFinalEffect.gameObject.SetActive(false);
                    UpdateTurntableView();
                    InitRocketPosition();
                    UpdateHappyProgress();
                });
            }
        }
        else
        {
            for (var i = 0; i < RollerViewList.Count; i++)
            {
                var roller = RollerViewList[i];
                var findResult = result.Level > roller.Config.Id ? 0 : result.Id;
                var indexList = new List<int>();
                for (var i1 = 0; i1 < roller.Config.TurntableResultList.Count; i1++)
                {
                    if (roller.Config.TurntableResultList[i1] == findResult)
                    {
                        indexList.Add(i1);
                    }
                }
                var index = indexList[Random.Range(0, indexList.Count)];
                performList.Add(async () =>
                {
                    await roller.PerformTurntable(index);
                    var resultTransform = roller.GetElementTransform(index);
                    if (result.Level > roller.Config.Id)
                    {
                        await CheckArrowGuide();
                        AudioManager.Instance.PlaySoundById(175);
                        Rocket.DOMove(resultTransform.position, 0.5f);
                        await XUtility.WaitSeconds(0.5f);   
                    }
                });
                if (result.Level == roller.Config.Id)
                {
                    if (result.Score > 0)
                    {
                        performList.Add(async () =>
                        {
                            roller.WinEffect.gameObject.SetActive(true);
                            await XUtility.WaitSeconds(0.5f);
                            var task = new TaskCompletionSource<bool>();
                            FlyCarrot(Rocket.position, result.Score, () =>
                            {
                                task.SetResult(true);
                            });
                            await task.Task;
                            roller.WinEffect.gameObject.SetActive(false);
                            UpdateTurntableView();
                            InitRocketPosition();
                        });   
                    }
                    else if (result.HappyValue > 0)
                    {
                        performList.Add(async () =>
                        {
                            AudioManager.Instance.PlaySoundById(174);
                            roller.WinEffect.gameObject.SetActive(true);
                            await XUtility.WaitSeconds(0.5f);
                            var task = new TaskCompletionSource<bool>();
                            FlyHappyPoint(Rocket.position, result.HappyValue, () =>
                            {
                                task.SetResult(true);
                            });
                            await task.Task;
                            roller.WinEffect.gameObject.SetActive(false);
                            UpdateTurntableView();
                            InitRocketPosition();
                            UpdateHappyProgress();
                        }); 
                    }
                    break;
                }
            }
            if (result.Level == 4)
            {
                performList.Add(async () =>
                {
                    // Rocket.DOMove(FinalNode.position, 0.5f);
                    // await XUtility.WaitSeconds(0.5f);
                    FinalEffect.gameObject.SetActive(true);
                    await XUtility.WaitSeconds(0.5f);
                    var task = new TaskCompletionSource<bool>();
                    FlyCarrot(Rocket.position, result.Score, () =>
                    {
                        task.SetResult(true);
                    });
                    await task.Task;
                    FinalEffect.gameObject.SetActive(false);
                    UpdateTurntableView();
                    InitRocketPosition();
                    UpdateHappyProgress();
                });
            }
        }
        if (performList.Count > 0)
        {
            IsPerforming = true;
            for (var i = 0; i < performList.Count; i++)
            {
                await performList[i]();
            }
            IsPerforming = false;
        }
        CheckSpinGuide2();
    }
}