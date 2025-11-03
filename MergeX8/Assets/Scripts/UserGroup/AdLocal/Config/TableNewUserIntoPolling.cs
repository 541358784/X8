using System;
using System.Collections.Generic;

[System.Serializable]
public class NewUserIntoPolling : TableBase
{   
    // #
    public int Id { get; set; }// 新增用户免费转化GROUPID轮询生效目标分层
    public List<int> GroupIDPollingTarget { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
