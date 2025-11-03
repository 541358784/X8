
using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyCardRewardItem : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _num;
    private Image _Icon;
    private GameObject _finish;
    private GameObject _canClaim;
    private void Awake()
    {
        _num = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();
        _Icon = transform.Find("Icon").GetComponent<Image>();
        _finish = transform.Find("Finish").gameObject;
        _canClaim = transform.Find("Claimed").gameObject;
    }

    public void SetStatus(int resID,int number, bool isFinish,bool isCanClaim)
    {
        _num.SetText(number.ToString());
        _Icon.sprite = UserData.GetResourceIcon(resID);
        _finish.SetActive(isFinish);
        _num.gameObject.SetActive(!isFinish);
        _canClaim.gameObject.SetActive(isCanClaim);
        
    }
}
