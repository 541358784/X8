using System;
using System.Collections.Generic;

[System.Serializable]
public class UserTypeInto : TableBase
{   
    // #
    public int Id { get; set; }// 分层ID; ADCONFIG表中RULES页签的GROUPID; ; 本地用户分层使用该ID+平台+国家作为转化条件，每次ADCONFIG的RULES中新增GROUPID，本表格也必须新增一行对应配置; ; ; ①用于关联功能数值配置中的USERGROUP列，重要！重要！重要！; ②1-5已被占用，可自定义其他数字
    public int GroupId { get; set; }// 转化逻辑类型：; 0、免费用户：使用GROUPID+PLATFROM+COUNTRY的组合，在8种数据结果中，转化到FREEUSERINTO中数组对应的用户分层中; ; 1、付费用户 ：直接通过当前付费等级1-9，按照从左到右的顺序，转化到PAYUSERINTO中数组对应的用户分层中
    public int IntoType { get; set; }// T0国家列表：; 参数内配置国家为T0国家
    public string CountryT0 { get; set; }// T1国家列表：; 参数内配置国家为T1国家
    public string CountryT1 { get; set; }// T2国家列表：; 参数内配置国家为T2国家
    public string CountryT2 { get; set; }// T3国家列表：; 参数内【未配置】国家为T3国家
    public string CountryOther { get; set; }// 免费用户转化逻辑：; ; 数组1 平台类型 IOS 国家T0; 数组2 平台类型 IOS 国家T1; 数组3 平台类型 IOS 国家T2; 数组3 平台类型 IOS 国家非T1T2; 数组5 平台类型 非IOS 国家T0; 数组6 平台类型 非IOS 国家T1; 数组7 平台类型 非IOS 国家T2; 数组8 平台类型 非IOS 国家非T1T2
    public List<int> FreeUserInto { get; set; }// 付费用户转化逻辑：; 根据用户当前付费等级，按照 123456789级从左至右的方式对应至新付费分层
    public List<int> PayUserInto { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
