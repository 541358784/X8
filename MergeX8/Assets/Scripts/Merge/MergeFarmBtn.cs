using System;
using Farm.Model;
using UnityEngine;
using UnityEngine.UI;

public class MergeFarmBtn : MonoBehaviour
{
    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            BackHomeControl.EnterFarm = true;
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
        });
    }

    public void UpdateDailyTaskStatus()
    {
        gameObject.SetActive(FarmModel.Instance.HaveFinishProduct());
    }
}