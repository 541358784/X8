using Activity.GardenTreasure.Model;
using DragonPlus;
using UnityEngine.UI;

public class UIPopupGardenTreasureStartController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    public override void PrivateAwake()
    {
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        transform.Find("Root/Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });

        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        
        InvokeRepeating("InvokeUpdate", 0, 1);
    }

    private void InvokeUpdate()
    {
        _timeText.SetText(GardenTreasureModel.Instance.GetPreheatEndTimeString());
    }
}