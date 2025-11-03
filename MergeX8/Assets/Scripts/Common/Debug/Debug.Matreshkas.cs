using System;
using System.ComponentModel;
using Activity.CrazeOrder.Model;
using Activity.LimitTimeOrder;
using Activity.Matreshkas.Model;
using Activity.TimeOrder;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.LimitOrderLine;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;


public partial class SROptions
{
    private const string Matreshkas = "套娃";
    [Category(Matreshkas)]
    [DisplayName("清理套娃")]
    public void ClearMatreshkas()
    {
        HideDebugPanel();
        MatreshkasModel.Instance.CheckJoinEnd(true);
        MatreshkasModel.Instance.Matreshkas.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished.Remove(4001);
        StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished.Remove(4002);
    }
    
    [Category(Matreshkas)]
    [DisplayName("当前进度")]
    public int Matreshkas_Stage
    {
        get
        {
            if (!MatreshkasModel.Instance.IsOpened())
                return -1;
            
            return MatreshkasModel.Instance.Stage;
        }
    }
    
    [Category(Matreshkas)]
    [DisplayName("当前需求物品id")]
    public int Matreshkas_EatBuild
    {
        get
        {   
            if (!MatreshkasModel.Instance.IsOpened())
                return -1;
            
            return MatreshkasModel.Instance.Matreshkas.EatBuildId;
        }
        set
        {
            ;
        }
    }
    
    [Category(Matreshkas)]
    [DisplayName("当前等级组")]
    public int Matreshkas_Group
    {
        get
        {   
            if (!MatreshkasModel.Instance.IsOpened())
                return -1;
            
            return MatreshkasModel.Instance.Matreshkas.GroupId;
        }
    }
    
    [Category(Matreshkas)]
    [DisplayName("增加套娃物品")]
    public void AddMatreshkas()
    {
        var mergeItem = MergeManager.Instance.GetEmptyItem();
        mergeItem.Id = Matreshkas_EatBuild;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main, 1);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
    }
}