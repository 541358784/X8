using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFlowerFieldMainController
{
    public List<FlowerFieldNode> GroupDic = new List<FlowerFieldNode>();
    public void InitLevelGroup()
    {
        var group = 1;
        var groupNode = transform.Find("Root/Scroll View/Viewport/Content/Flowers/"+group);
        while (groupNode)
        {
            var levelGroup = groupNode.gameObject.AddComponent<FlowerFieldNode>();
            levelGroup.Init(group-1);
            GroupDic.Add(levelGroup);
            group++;
            groupNode = transform.Find("Root/Scroll View/Viewport/Content/Flowers/" + group);
        }

        var curState = FlowerFieldModel.Instance.GetLevelState(Storage.TotalScore);
        SetState(curState);
        transform.Find("Root/LevelGroup/Scroll View").GetComponent<ScrollRect>().enabled = false;
        transform.Find("Root/Scroll View").GetComponent<ScrollRect>().enabled = false;
        FocusOn(curState.GroupInnerIndex - 1);
        XUtility.WaitFrames(10, () =>
        {
            transform.Find("Root/LevelGroup/Scroll View").GetComponent<ScrollRect>().enabled = true;
            transform.Find("Root/Scroll View").GetComponent<ScrollRect>().enabled = true;
        });
    }

    public void SetState(FlowerFieldLevelState state,bool anim = false)
    {
        foreach (var node in GroupDic)
        {
            node.SetState(state,anim);
        }
    }
    public Task PerformFly()
    {
        var startPoint = transform.Find("Root/StartPoint");
        var endPoint = transform.Find("Root/EndPoint");
        var spine = transform.Find("Root/Spine").GetComponent<SkeletonGraphic>();
        spine.transform.position = startPoint.position;
        spine.gameObject.SetActive(true);
        spine.PlaySkeletonAnimation("animation");
        spine.transform.DOMove(endPoint.position,2f).OnComplete(() =>
        {
            spine.gameObject.SetActive(false);
        });
        return XUtility.WaitSeconds(1f);
    }

    public void FocusOn(int groupIndex)
    {
        if (groupIndex >= GroupDic.Count)
            groupIndex = GroupDic.Count - 1;
        var node = GroupDic[groupIndex];
        var nodeY = (node.transform as RectTransform).anchoredPosition.y;
        var content = transform.Find("Root/Scroll View/Viewport/Content");
        (content as RectTransform).SetAnchorPositionY(4102-nodeY);

        var rewardBox = RewardBoxList[groupIndex].transform as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rewardBox.parent as RectTransform);
        var rewardBoxX = rewardBox.anchoredPosition.x;
        var sliderContent = transform.Find("Root/LevelGroup/Scroll View/Viewport/Content");
        (sliderContent as RectTransform).SetAnchorPositionX((groupIndex == RewardBoxList.Count-1)?(-1000):(250-rewardBoxX));
    }

    public class FlowerFieldNode : MonoBehaviour
    {
        public FlowerFieldRewardConfig Config;
        public int Index;

        public void Init(int groupIndex)
        {
            Index = groupIndex;
            var rewardConfigs = FlowerFieldModel.Instance.RewardConfig;
            Config = rewardConfigs.Count > Index ? rewardConfigs[Index] : null;
            gameObject.SetActive(Config != null);
        }

        public void SetState(FlowerFieldLevelState state,bool anim)
        {
            if (Config == null)
                return;
            var oldState = gameObject.activeSelf;
            gameObject.SetActive(state.TotalScore >= Config.Score);
            var newState = gameObject.activeSelf;
            if (!oldState && newState && anim)
            {
                var animator = gameObject.GetComponent<Animator>();
                animator.PlayAnimation("Appear");
            }
        }
    }
}