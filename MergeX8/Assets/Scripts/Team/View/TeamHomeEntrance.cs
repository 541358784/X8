using System;
using DG.Tweening;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class TeamHomeEntrance:MonoBehaviour
{
    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamEntrance);
            if (TeamManager.Instance.InitNetwork == InitNetworkState.Success)
            {
                if (TeamManager.Instance.HasTeam())
                {
                    UIPopupGuildMainController.Open();
                }
                else
                {
                    UIPopupGuildJoinController.Open();
                }
            }
            else
            {
                ErrorText.gameObject.SetActive(true);
                ErrorText.DOKill();
                DOVirtual.DelayedCall(1f, () =>
                {
                    ErrorText.gameObject.SetActive(false);
                }).SetTarget(ErrorText);
                Debug.LogError("没网不让进");
                TeamManager.Instance.GetMyTeamInfo();
            }
        });
        var TeamIconRect = transform.Find("Root/GuildIcon") as RectTransform;
        TeamIconRect.gameObject.SetActive(true);
        TeamIcon = TeamIconNode.BuildTeamIconNode(TeamIconRect, TeamManager.Instance.MyTeamInfo.GetViewState());
        viestate = TeamManager.Instance.MyTeamInfo.GetViewState();
        InvokeRepeating("UpdateTime",0,1);
        ErrorText = transform.Find("Root/Tip");
        ErrorText.gameObject.SetActive(false);
    }

    private TeamIconViewState viestate;
    private TeamIconNode TeamIcon;
    private Transform ErrorText;

    public void UpdateTime()
    {
        gameObject.SetActive(TeamManager.Instance.TeamIsUnlock());
        TeamIcon.gameObject.SetActive(TeamManager.Instance.HasTeam());
        if (TeamIcon.gameObject.activeSelf)
        {
            var newViewState = TeamManager.Instance.MyTeamInfo.GetViewState();
            if (viestate != newViewState)
            {
                viestate = newViewState;
                TeamIcon.SetTeamIconViewState(newViewState);
            }
        }
    }
}