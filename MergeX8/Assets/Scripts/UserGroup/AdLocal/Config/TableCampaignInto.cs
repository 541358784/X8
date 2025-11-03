using System;
using System.Collections.Generic;

[System.Serializable]
public class CampaignInto : TableBase
{   
    // #
    public int Id { get; set; }// 分层ID; ADCONFIG表中RULES页签的GROUPID; ; 客户端查询ADJUST的归因接口，将满足NETWORK和CAIPAIGN接口关键字的用户，划分到本列配置的GROUPID
    public int GroupId { get; set; }// 渠道关键字，逗号分隔，-1为无值
    public string NetworkKey { get; set; }// 渠道关键字，逗号分隔，-1为无值; ; CAMPAIGN关键字的取值，要取CAMPAIGN和FBINSTALLFERRER两个接口！！
    public string CampaignKey { get; set; }// 是否开启小游戏字段; CAMPAIGN关键字的取值，要取CAMPAIGN和FBINSTALLFERRER两个接口！！
    public List<string> MiniGameKey { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
