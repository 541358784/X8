using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class KeepPetTurkeyTaskRewardItem : MonoBehaviour
{
    public KeepPetDailyTaskConfig TaskConfig;
    public Image Icon;
    public LocalizeTextMeshProUGUI NumText;
    public void Init(KeepPetDailyTaskConfig taskConfig)
    {
        gameObject.SetActive(KeepPetTurkeyModel.Instance.IsOpened());
        Icon = transform.GetComponent<Image>();
        NumText = transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
        NumText.SetText(taskConfig.GetTurkeyScore().ToString());
    }
    
}