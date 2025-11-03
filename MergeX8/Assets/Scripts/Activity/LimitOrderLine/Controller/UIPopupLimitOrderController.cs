
using System.Collections;
using System.Collections.Generic;
using Activity.LimitTimeOrder;
using DragonPlus;
using DragonPlus.Config.LimitOrderLine;
using Gameplay;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLimitOrderController : UIWindowController
{
    private Button _closeButton;
    private Button _okButton;
    private Image _rewardIcon;

    private bool _isCompleteAll = false;
    private bool _isCanClose = true;

    private List<Animator> _animators = new List<Animator>();
    private List<Transform> _balls = new List<Transform>();
    
    private LocalizeTextMeshProUGUI _rewardText;

    private List<int> _ballIndex = new List<int>()
    {
        0,
        1,
        2,
        3,
        4,
    };
    
    public override void PrivateAwake()
    {
        _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        _closeButton.onClick.AddListener(()=>
        {
            if(!_isCanClose)
                return;
            
            AnimCloseWindow(() =>
            {
                AnimEndLogic();
            });
        });

        _okButton = transform.Find("Root/Button").GetComponent<Button>();
        _okButton.onClick.AddListener(() =>
        {
            if(!_isCanClose)
                return;
            
            AnimCloseWindow(() =>
            {
                AnimEndLogic();
            });
        });

        var tips = transform.Find("Root/Tip").gameObject;
        transform.Find("Root/TipButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            tips.gameObject.SetActive(false);   
            tips.gameObject.SetActive(true);   
        });
        
        _rewardText = transform.Find("Root/Reward/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardIcon = transform.Find("Root/Reward/Icon").GetComponent<Image>();

        var config = LimitTimeOrderModel.Instance.GetGroupConfig();
        if (config != null)
        {
            _rewardIcon.sprite = UserData.GetResourceIcon(config.RewardIds[0]);
            _rewardText.SetText("x"+config.RewardNums[0]);
        }

        for (int i = 1; i <= 5; i++)
        {
            var animator = transform.Find("Root/Content/" + i).GetComponent<Animator>();
            _animators.Add(animator);
            
            _balls.Add(transform.Find("Root/Content/Ball/"+i));
        }
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        _isCanClose = false;
        if (objs != null && objs.Length >= 1)
        {
            _isCompleteAll = true;
        }
        _animators.ForEach(a=>a.Play("normal", -1, 0));
        _balls.ForEach(a=>a.gameObject.SetActive(true));

        for (int i = 0; i < LimitTimeOrderModel.Instance.LimitOrderLine.ShowAnimState.Count; i++)
        {
            int index = LimitTimeOrderModel.Instance.LimitOrderLine.ShowAnimState[i];
            _animators[index].Play("idle", -1, 0);
            
            _ballIndex.RemoveAt(_ballIndex.FindIndex(a=>a==index));
        }
        for (int i = 0; i < LimitTimeOrderModel.Instance.AnimNum; i++)
        {
            _balls[i].gameObject.SetActive(false);
        }

        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (LimitTimeOrderModel.Instance.AnimNum != LimitTimeOrderModel.Instance.CompleteNum)
        {
            int animNum = LimitTimeOrderModel.Instance.AnimNum;
            for (int i = animNum; i < LimitTimeOrderModel.Instance.CompleteNum; i++)
            {
                int index = _ballIndex.Random();
                _ballIndex.RemoveAt(_ballIndex.FindIndex(a=>a == index));
                LimitTimeOrderModel.Instance.LimitOrderLine.ShowAnimState.Add(index);
                
                _balls[i].gameObject.SetActive(false);
                _animators[index].transform.SetAsLastSibling();
                yield return StartCoroutine(CommonUtils.PlayAnimation(_animators[index], "move", null, () =>
                {
                }));
                
            }
            LimitTimeOrderModel.Instance.AnimNum = LimitTimeOrderModel.Instance.CompleteNum;
        }

        if (_isCompleteAll)
        { 
            AnimCloseWindow(() =>
            {
                AnimEndLogic();
            });
            yield break; 
        }
        _isCanClose = true;
    }
    private void AnimEndLogic()
    {
        _isCanClose = true;
            
        if(!_isCompleteAll)
            return;
        
        var config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList.Find(a => a.Id == LimitTimeOrderModel.Instance.GroupId);
        if (config == null)
            return;
        
        List<ResData> resDatas = new List<ResData>();
        for(int i = 0;i < config.RewardIds.Count; i++)
        {
            ResData resData = new ResData(config.RewardIds[i], config.RewardNums[i]);
            resDatas.Add(resData);
        }
        CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,false);
    }
}