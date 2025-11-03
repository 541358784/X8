using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class MergeGarageCleanUp:MonoBehaviour
{
    private Button _garageCleanBtn;
    private GameObject _garageCleanRedPoint;
    private LocalizeTextMeshProUGUI _garageCleanTimeText;
    private void Awake()
    {
        _garageCleanBtn = transform.GetComponent<Button>();
        _garageCleanTimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _garageCleanRedPoint = transform.Find("Root/RedPoint").gameObject;
        _garageCleanBtn.onClick.AddListener(OnGarageBtn);
        InvokeRepeating("UpdateTime",0,1);
    }
    
    private void OnGarageBtn()
    {
        if (!GarageCleanupModel.Instance.StorageGarageCleanup.IsShowStart)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupStart);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIGarageCleanupMain);

        }
    }

    public void UpdateTime()
    {
        if (GarageCleanupModel.Instance.IsOpened())
        {
            _garageCleanBtn.gameObject.SetActive(true);
            _garageCleanTimeText.SetText(GarageCleanupModel.Instance.GetActivityLeftTimeString());
            _garageCleanRedPoint.gameObject.SetActive(GarageCleanupModel.Instance.IsCanGet());
        }
        else
        {
            _garageCleanBtn.gameObject.SetActive(false);
        }
    }
}