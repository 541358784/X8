using Activity.TreasureMap;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKeepPetMainController
{
    private Slider PowerSlider;
    private LocalizeTextMeshProUGUI PowerSliderText;
    private Button LevelRewardBtn;
    private LocalizeTextMeshProUGUI LevelText;
    private ExpBarRedPoint LevelRewardRedPoint;
    private Animator LevelUpIconEffectAnimator;
    public void InitLevelProgress()
    {
        PowerSlider = GetItem<Slider>("Root/Slider");
        PowerSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        StorageExp = Storage.Exp;
        ViewExp = StorageExp;
        var levelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
        PowerSlider.value = (float)levelConfig.GetCurLevelExp(ViewExp) / levelConfig.GetNextLevelNeedExp();
        PowerSliderText.SetText(levelConfig.GetCurLevelExp((int)ViewExp)+"/"+ levelConfig.GetNextLevelNeedExp());
        LevelRewardBtn = GetItem<Button>("Root/ButtonLeft/LevelUp");
        LevelRewardBtn.onClick.AddListener(()=>
        {
            FinishClickExpBarGuide();
            FinishClickExpBar2Guide();
            UIKeepPetLevelUpController.Open(Storage);
        });
        LevelRewardBtn.interactable = true;
        LevelText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/LV/Text");
        LevelText.SetText(levelConfig.Id.ToString());
        LevelRewardRedPoint = transform.Find("Root/ButtonLeft/LevelUp/RedPoint").gameObject.AddComponent<ExpBarRedPoint>();
        LevelRewardRedPoint.Init();
        LevelUpEffect = transform.Find("Root/FX_pet_change_0");
        LevelUpIconEffectAnimator = GetItem<Animator>("Root/Slider");
    }
    
    private ulong ExpAddTime;
    private ulong UpdateInterval = 30;//更新间隔(ms)
    private float ViewExp;
    private int StorageExp;
    private bool IsPerformExpGrow;

    public void UpdateExpProgress()
    {
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime >= ExpAddTime)
        {
            var oldLevelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
            ViewExp = StorageExp;
            var newLevelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
            if (oldLevelConfig != newLevelConfig)
            {
                PerformLevelUp(newLevelConfig);
            }
            var levelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
            PowerSlider.value = (float)levelConfig.GetCurLevelExp(ViewExp) / levelConfig.GetNextLevelNeedExp();
            PowerSliderText.SetText(levelConfig.GetCurLevelExp((int)ViewExp)+"/"+ levelConfig.GetNextLevelNeedExp());
            IsPerformExpGrow = false;
            var sliderEffect = transform.Find("Root/Slider/Fill Area/Fill/FX_light");
            sliderEffect.gameObject.SetActive(false);
            CancelInvoke("UpdateExpProgress");
            return;
        }
        else
        {
            var oldLevelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
            var distance = StorageExp - ViewExp;
            ViewExp = (float) StorageExp - 
                      (distance * 
                       ((float)(ExpAddTime - curTime) / (ExpAddTime - curTime + UpdateInterval)));
            if (ViewExp >= StorageExp)
            {
                Debug.LogError("分数进度不正常,界面分数大于存档分数,强制恢复界面分数");
                ViewExp = StorageExp;
            }
            var newLevelConfig = ((int)ViewExp).KeepPetGetCurLevelConfig();
            if (oldLevelConfig != newLevelConfig)
            {
                PerformLevelUp(newLevelConfig);
            }
            {
                PowerSlider.value = (float)newLevelConfig.GetCurLevelExp(ViewExp) / newLevelConfig.GetNextLevelNeedExp();
                PowerSliderText.SetText(newLevelConfig.GetCurLevelExp((int)ViewExp)+"/"+ newLevelConfig.GetNextLevelNeedExp());
            }   
        }
    }
    public void PerformLevelUp(KeepPetLevelConfig newLevel)
    {
        LevelText.SetText(newLevel.Id.ToString());
        if (newLevel.Id == KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
        {
            SearchTaskBtn.gameObject.SetActive(CurState.Enum == KeepPetStateEnum.Happy);
        }
        LevelRewardRedPoint.UpdateViewState();
        PlayLevelUpEffect();
        LevelUpIconEffectAnimator.PlayAnimation("Level");
        // UIKeepPetLevelUpController.Open(Storage);
        if (newLevel.Id == KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
        {
            CheckClickExpBar2Guide();
        }
    }

    private Transform LevelUpEffect;
    public void PlayLevelUpEffect()
    {
        LevelUpEffect.DOKill();
        LevelUpEffect.gameObject.SetActive(false);
        LevelUpEffect.gameObject.SetActive(true);
        DOVirtual.DelayedCall(3f, () =>
        {
            LevelUpEffect.gameObject.SetActive(false);
        }).SetTarget(LevelUpEffect);
    }
    public void OnExpChange(EventKeepPetExpChange evt)//经验值变化
    {
        StorageExp = evt.NewValue;
        ExpAddTime = APIManager.Instance.GetServerTime() + (ulong)(0.5f * XUtility.Second);
        if (IsPerformExpGrow)
            return;
        IsPerformExpGrow = true;
        var sliderEffect = transform.Find("Root/Slider/Fill Area/Fill/FX_light");
        sliderEffect.gameObject.SetActive(true);
        InvokeRepeating("UpdateExpProgress",0f,(float)UpdateInterval/XUtility.Second);
    }
}