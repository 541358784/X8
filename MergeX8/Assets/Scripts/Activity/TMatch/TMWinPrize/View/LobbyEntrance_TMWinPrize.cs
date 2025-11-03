using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntrance_TMWinPrize:MonoBehaviour
{
    private LocalizeTextMeshProUGUI TimeText;
    private Button Btn;

    public void Init()
    {
        TimeText = transform.Find("TipsBG/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
        Btn = transform.GetComponent<Button>();
        Btn.onClick.AddListener(OnClickBtn);
    }

    public void UpdateTime()
    {
        gameObject.SetActive(TMWinPrizeModel.Instance.IsOpened());
        transform.parent.gameObject.SetActive(TMWinPrizeModel.Instance.IsOpened());
        if (gameObject.activeSelf)
        {
            TimeText.SetText(TMWinPrizeModel.Instance.GetActivityLeftTimeString());   
        }
    }
    public void OnClickBtn()
    {
        UITMWinPrize.Open();
    }
}