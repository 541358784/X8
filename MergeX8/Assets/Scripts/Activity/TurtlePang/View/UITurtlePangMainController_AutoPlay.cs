using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    private Button AutoBtn;
    public bool IsAuto;
    public Transform BtnFill;
    public void InitAutoPlay()
    {
        IsAuto = false;
        BtnFill = transform.Find("Root/ButtonAuto/Full");
        BtnFill.gameObject.SetActive(IsAuto);
        AutoBtn = transform.Find("Root/ButtonAuto").GetComponent<Button>();
        AutoBtn.onClick.AddListener(() =>
        {
            IsAuto = !IsAuto;
            BtnFill.gameObject.SetActive(IsAuto);
            if (IsAuto && StartBtn.interactable && !IsPerforming)
            {
                StartBtn.interactable = false;
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TurtlePangPut);
                Play().AddCallBack(PlayLogic).WrapErrors();
            }
        });
    }
}