
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class ClimbTreeRewardItem: MonoBehaviour
{
    private Transform _collectPosition;
    private Transform _titleBoardNormal;
    private Transform _titleBoardCompleted;
    private LocalizeTextMeshProUGUI _titleBoardNormalText;
    private LocalizeTextMeshProUGUI _titleBoardCompletedText;
    private Transform _rewardItem;
    private List<Transform> _rewardsList;
    private HorizontalLayoutGroup _rewardLayoutGroup;
    private Transform _rewardStaticGroup;
    
     

    public enum ClimbTreeRewardStatus
    {
        None,
        Finish,
        // Err
    }
    private void Awake()
    {
        _collectPosition = transform.Find("Point");
        _titleBoardNormal = transform.Find("Lv/Normal");
        _titleBoardCompleted = transform.Find("Lv/Finish");
        _titleBoardNormalText = transform.Find("Lv/Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _titleBoardCompletedText = transform.Find("Lv/Finish/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardLayoutGroup = transform.Find("ItemGroup").GetComponent<HorizontalLayoutGroup>();
        _rewardLayoutGroup.enabled = true;
        _rewardStaticGroup = transform.Find("ItemGroupStatic");
        _rewardStaticGroup.gameObject.SetActive(false);
        _rewardItem = transform.Find("ItemGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _rewardsList = new List<Transform>();
    }

    public async Task PerformCollectReward()
    {
        _titleBoardNormal.gameObject.SetActive(false);
        _titleBoardCompleted.gameObject.SetActive(true);
        _rewardLayoutGroup.enabled = false;
        var waitTime = 0.5f;
        var flyTime = 0.5f;
        var upScale = 1.3f;
        var collectScale = 0.3f;
        var taskList = new List<Task>();
        for (int i = 0; i < _rewardsList.Count; i++)
        {
            var flyObject = _rewardsList[i].gameObject;
            var localCollectPosition = flyObject.transform.parent.InverseTransformPoint(_collectPosition.position);
            var flyTask = new TaskCompletionSource<bool>();
            taskList.Add(flyTask.Task);
            flyObject.transform.DOScale(new Vector3(upScale, upScale, upScale), waitTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                flyObject.transform.DOScale(new Vector3(collectScale, collectScale, collectScale), flyTime).SetEase(Ease.Linear);
                flyObject.transform.DOLocalMove(localCollectPosition, flyTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(_collectPosition.position);
                    ShakeManager.Instance.ShakeLight();
                    flyObject.SetActive(false);
                    flyTask.SetResult(true);
                });
            });
        }
        await Task.WhenAll(taskList);
        _rewardLayoutGroup.enabled = true;
    }
    public void Init(List<ResData> rewards,ClimbTreeRewardStatus status,int level)
    {
        for (var i = 0; i < _rewardsList.Count; i++)
        {
            if (_rewardsList[i].name.Contains("(Clone)"))
            {
                GameObject.Destroy(_rewardsList[i].gameObject);   
            }
        }
        _rewardsList.Clear();
        var useStatic = rewards.Count == 3;
        _rewardLayoutGroup.gameObject.SetActive(!useStatic);
        _rewardStaticGroup.gameObject.SetActive(useStatic);
        for (var i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            Transform rewardObject = useStatic?
                _rewardStaticGroup.Find("Item" + (i + 1)):
                GameObject.Instantiate(_rewardItem,_rewardItem.parent);
            rewardObject.GetComponent<Image>().sprite = UserData.GetResourceIcon(reward.id, UserData.ResourceSubType.Big);
            _rewardsList.Add(rewardObject.transform);
        }
        _titleBoardNormalText.SetText(level.ToString());
        _titleBoardCompletedText.SetText(level.ToString());
        switch (status)
        {
            case ClimbTreeRewardStatus.None:
               _titleBoardNormal.gameObject.SetActive(true);
               _titleBoardCompleted.gameObject.SetActive(false);
               for (var i = 0; i < _rewardsList.Count; i++)
               {
                   _rewardsList[i].gameObject.SetActive(true);
               }
               break;    
           case ClimbTreeRewardStatus.Finish:
               _titleBoardNormal.gameObject.SetActive(false);
               _titleBoardCompleted.gameObject.SetActive(true);
               for (var i = 0; i < _rewardsList.Count; i++)
               {
                   _rewardsList[i].gameObject.SetActive(false);
               }
               break;
        }
   
    }
}
