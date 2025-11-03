using System.Collections;
using System.Collections.Generic;
using Framework;
using Merge.Order;
using UnityEngine;


public partial class MergeBoard : MonoBehaviour, IOnApplicationPause
{
    private enum AutoTipType
    {
        None = -1,
        Web, //蛛网
        Unlock,//解锁
        Product, //可产出
        Count,
    }

    private Dictionary<AutoTipType, List<Grid>> autoTipMap = new Dictionary<AutoTipType, List<Grid>>();
    private Dictionary<int, List<Grid>> autoTipGrids = new Dictionary<int, List<Grid>>();
    public List<Grid> TempAutoTipsItems = new List<Grid>();

    
    public void AutoTips(bool isRestore = false)
    {
        if(gameObject == null)
            return;
        
        if(!gameObject.activeSelf)
            return;
        // if(EatBuildingTipsAnim())
        //     return;
        if (isRestore)
        {
            if (mergeTipList != null && mergeTipList.Count >= 2)
            {
                PlayTipsAnimation(-1, 1f);
            }
            
            return;
        }
        if (mergeTipList != null && mergeTipList.Count >= 2)
        {
            foreach (var mergeItem in mergeTipList)
            {
                if (mergeItem.state != 0)
                    continue;

                return;
            }

            List<Grid> grids = CalculateAutoTip(false);
            if (grids == null || grids.Count == 0)
                return;

            if(grids[0].id == mergeTipList[0].id)
                return;

            //优先找锁定的
            bool isLock = false;
            foreach (var grid in grids)
            {
                if(grid.state != 0)
                    continue;

                isLock = true;
                break;
            }

            if (!isLock)
                return;
            
            CancelTip(-1, true);
            mergeTipList.Clear();
            mergeTipList.AddRange(grids);
            PlayTipsAnimation(-1);
            return;
        }

        CancelTip(-1, true);
        CalculateAutoTip();

        PlayTipsAnimation(-1);
    }
    
    public void PlayTipsAnimation(int index = -1, float delayTime = 2.5f)
    {
        if (mergeTipList == null || mergeTipList.Count == 0)
            return;
        if (!gameObject.activeSelf)
            return;
        if (!gameObject.activeInHierarchy)
            return;
        if (index > 0)
        {
            if (!mergeTipList.Contains(grids[index]))
                return;
        }

        if (_dealyAnim != null)
        {
            StopCoroutine(_dealyAnim);
            _dealyAnim = null;
        }

        _dealyAnim = StartCoroutine(DelayPlayAnim(delayTime));
    }

    private IEnumerator DelayPlayAnim(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Vector3 dir = Vector3.zero;
        
        if(mergeTipList.Count >= 2)
            dir = mergeTipList[0].board.transform.position - mergeTipList[1].board.transform.position;
        
        for (int i = 0; i < mergeTipList.Count; i++)
        {
            mergeTipList[i].board.PlayAnimator("normal");
            mergeTipList[i].board.Animator.enabled = false;
            mergeTipList[i].board.PlayTweenAnim(i == 0 ? -dir : dir);
        }

        _dealyAnim = null;
    }
    
    public List<Grid> CalculateAutoTip(bool isAddMergeTips = true, bool filterTask = true,bool keepTwo = true, int targetId = -1)
    {
        autoTipMap.Clear();
        autoTipGrids.Clear();
        TempAutoTipsItems.Clear();

        foreach (var grid in grids)
        {
            if (grid.id == -1 || !grid.canTip)
                continue;
            
            if (MergeConfigManager.Instance.IsMaxLevel(grid.id))
                continue;
            if (grid.board!=null &&  grid.board.tableMergeItem != null && grid.board.tableMergeItem.type == 2)
            {
                var storageItem1 = MergeManager.Instance.GetBoardItem(grid.board.index,(MergeBoardEnum)_boardID);
                if(storageItem1.ProductCount>0)
                    continue;
            }
            AddAutoMap(grid);
            AddAutoGrid(grid);
        }
        
        BuildAutoTips(filterTask, keepTwo, targetId);
        if (!isAddMergeTips)
            return TempAutoTipsItems;
        
        mergeTipList.Clear();
        if(TempAutoTipsItems.Count >= 2)
            mergeTipList.AddRange(TempAutoTipsItems);

        return TempAutoTipsItems;
    }

    private void BuildAutoTips(bool filterTask, bool keepTwo =true, int targetId = -1)
    {
        if (targetId > 0)
        {
            for (AutoTipType i = AutoTipType.Web; i < AutoTipType.Count; i++)
            {
                if (!autoTipMap.ContainsKey(i))
                    continue;

                var mapData = autoTipMap[i];
                var targetGrid = mapData.Find(a => a.id == targetId);
                if (targetGrid == null)
                    continue;
                
                var autoGrids = GetAutoGrids(targetGrid);
                if(autoGrids == null)
                    continue;

                TempAutoTipsItems.Clear();
                TempAutoTipsItems.Add(targetGrid);
                foreach (var gr in autoGrids)
                {
                    if(targetGrid == gr)
                        continue;

                    if (i == AutoTipType.Web)
                    {
                        if(gr.state == 0)
                            continue;
                    }
                    TempAutoTipsItems.Add(gr);
                    
                    if(TempAutoTipsItems.Count >= 2)
                        return;
                }
            }
        }
        
        TempAutoTipsItems.Clear();
        for (AutoTipType i = AutoTipType.Web; i < AutoTipType.Count; i++)
        {
            if(!autoTipMap.ContainsKey(i))
                continue;

            var mapData = autoTipMap[i];
            
            foreach (var grid in mapData)
            {
                if (filterTask)
                {
                    if(MainOrderManager.Instance.IsTaskNeedItem(grid.id, false))
                        continue;
                }
                
                var autoGrids = GetAutoGrids(grid);
                if(autoGrids == null)
                    continue;

                TempAutoTipsItems.Clear();
                TempAutoTipsItems.Add(grid);
                foreach (var gr in autoGrids)
                {
                    if(grid == gr)
                        continue;

                    if (i == AutoTipType.Web)
                    {
                        if(gr.state == 0)
                            continue;
                    }
                    TempAutoTipsItems.Add(gr);
                    if(keepTwo)
                        if(TempAutoTipsItems.Count >= 2)
                            return;
                }
            }
        }
        
        if(TempAutoTipsItems.Count < 2)
            TempAutoTipsItems.Clear();
    }

    private List<Grid> GetAutoGrids(Grid grid)
    {
        if (!autoTipGrids.ContainsKey(grid.id))
            return null;

        if (autoTipGrids[grid.id] == null)
            return null;

        if (autoTipGrids[grid.id].Count < 2)
            return null;

        return autoTipGrids[grid.id];
    }
    
    private void AddAutoMap(Grid grid)
    {
        AutoTipType type = AutoTipType.None;

        if (grid.state == 0)
            type = AutoTipType.Web;
        else if(grid.isProduct)
            type = AutoTipType.Product;
        else 
            type = AutoTipType.Unlock;

        if (!autoTipMap.ContainsKey(type))
            autoTipMap[type] = new List<Grid>();
        
        autoTipMap[type].Add(grid);
    }
    
    private void AddAutoGrid(Grid grid)
    {
        if (!autoTipGrids.ContainsKey(grid.id))
            autoTipGrids[grid.id] = new List<Grid>();
        
        autoTipGrids[grid.id].Add(grid);
    }
    
    public void CancelTip(int index, bool force = false)
    {
        if (force)
        {
            ResetMergeTipsAnim();
            return;
        }

        if (mergeTipList == null || index == -1 || grids[index].board == null)
            return;
        if (mergeTipList.IndexOf(grids[index]) != -1)
        {
            ResetMergeTipsAnim();
        }
    }

    void PauseTip(int index)
    {
        if (mergeTipList == null || index == -1 || grids[index].board == null)
            return;
        
        if (mergeTipList.IndexOf(grids[index]) == -1)
            return;
        
        
        for (int i = 0; i < mergeTipList.Count; i++)
        {
            if (mergeTipList[i].board == null)
                continue;

            mergeTipList[i].board.StopTweenAnim();
            mergeTipList[i].board.transform.localScale = Vector3.one;
            mergeTipList[i].board.Animator.enabled = true;
            mergeTipList[i].board.Animator.Play("Normal");
        }
    }
    

    private void ResetMergeTipsAnim()
    {
        if (mergeTipList == null)
            return;

        for (int i = 0; i < mergeTipList.Count; i++)
        {
            if (mergeTipList[i].board == null)
                continue;

            mergeTipList[i].board.StopTweenAnim();
            mergeTipList[i].board.transform.localScale = Vector3.one;
            mergeTipList[i].board.Animator.enabled = true;
            mergeTipList[i].board.Animator.Play("Normal");
        }

        mergeTipList.Clear();
    }
}