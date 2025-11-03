using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;

public class HappyGoMergeBoard:MergeBoard
{
    
    private Dictionary<Transform, Transform> setSiblingDic = new Dictionary<Transform, Transform>();
    
    public override void InitBoardId()
    {
        SetBoardID((int) MergeBoardEnum.HappyGo);
    }
    public List<Grid> FindAllTipsByType() //0  蛛网类型提示 1 全部解锁的提示
    {
        TempAutoTipsItems.Clear();
        for (int i = 0; i < grids.Length; i++)
        {
            if(grids[i].id == -1 || grids[i].state != 1 && grids[i].state != 3 && grids[i].state != 0)
                continue;
           
            if(MergeConfigManager.Instance.IsMaxLevel(grids[i].id))
                continue;
           
            for (int j = 0; j < grids.Length; j++)
            {
                if(i == j)
                    continue;
               
                if(grids[i].id != grids[j].id)
                    continue;
               
                if(grids[j].id == -1 || grids[j].state != 3 && grids[j].state != 1)
                    continue;
               
                TempAutoTipsItems.Add(grids[i]);
                TempAutoTipsItems.Add(grids[j]);
                break;
            }
        }

        return TempAutoTipsItems;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        setSiblingDic.Clear();
    }

    protected override void AddSiblingTransform(Transform trans)
    {
        if(setSiblingDic.ContainsKey(trans))
            return;
        setSiblingDic.Add(trans, trans);
    }
    
    protected override void DelSiblingTransform(Transform trans)
    {
        if(!setSiblingDic.ContainsKey(trans))
            return;
        setSiblingDic.Remove(trans);
    }

    
    protected override void UpdateSibling(bool isAdapt = false)
    {
        if(HamsterGrid == null || HamsterGrid.board == null)
            return;

        List<Transform> removeTransforms = new List<Transform>();
        foreach (var kv in setSiblingDic)
        {
            if(kv.Value.gameObject.activeSelf)
                continue;
            
            removeTransforms.Add(kv.Value.transform);
        }
        removeTransforms.ForEach(a=>setSiblingDic.Remove(a));
        
        if (!isAdapt)
        {
            HamsterGrid.board.transform.SetAsLastSibling();
        }
        else
        {
            int siblingIndex = int.MaxValue;
            foreach (var kv in setSiblingDic)
            {
                if(kv.Value == null)
                    continue;
                
                int index = kv.Value.transform.GetSiblingIndex();
                siblingIndex = index < siblingIndex ? index : siblingIndex;
            }
            
            if(siblingIndex == int.MaxValue)
                HamsterGrid.board.transform.SetAsLastSibling();
            else
                HamsterGrid.board.transform.SetSiblingIndex(siblingIndex-1);
        }
    }
}