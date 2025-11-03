using DragonPlus;
using UnityEngine.UI;

public partial class MergeKeepPet
{
    public Image Slider;
    public Image FinalRewardIcon;
    public Image TaskIcon;
    public LocalizeTextMeshProUGUI SliderText;

    public void InitDailyTaskSlider()
    {
        Slider = transform.Find("Root/Slider").GetComponent<Image>();
        FinalRewardIcon = transform.Find("Root/Reward").GetComponent<Image>();
        //TaskIcon = transform.Find("Root/Icon").GetComponent<Image>();
        SliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateDailyTaskSliderViewState();
    }

    public void UpdateDailyTaskSliderViewState()
    {
        var maxCount = KeepPetModel.Instance.MaxLevel;
        var curCount = KeepPetModel.Instance.Level;
        Slider.fillAmount = (float) curCount / maxCount;
        if (curCount == maxCount)
        {
            SliderText.SetTerm("UI_main_map_completed");
        }
        else
        {
            SliderText.SetText(curCount+"/"+maxCount);   
        }

        FinalRewardIcon.gameObject.SetActive(
            maxCount >= KeepPetModel.Instance.GlobalConfig.DailyTaskFinalRewardNeedTaskCount &&
            !KeepPetModel.Instance.StorageDailyTask.IsCollectFinalReward);
    }
}
    