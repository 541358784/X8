using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class TMWinPrizeLabel:MonoBehaviour
{
    private Image Icon;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateState();
        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_WIN,OnWin);
    }

    private void OnDestroy()
    {
        TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_WIN,OnWin);
    }

    public void OnWin(TMatch.BaseEvent evt)
    {
        UpdateState();
    }
    public void UpdateState()
    {
        var curPrize = TMWinPrizeModel.Instance.GetCurReward();
        gameObject.SetActive(curPrize != null && curPrize.Count > 0);
        if (gameObject.activeSelf)
        {
            Icon.sprite = UserData.GetResourceIcon(curPrize[0].id);
            NumText.SetText("X"+curPrize[0].count);
        }
    }
}