// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Rules
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.TMatchLevel
{
    public class Rules
    {   
        
        /// <summary>
        /// 分层名称; 自定义，仅说明描述意义，无逻辑使用意义
        /// </summary>
        public string GroupName { get; set; }
        
        /// <summary>
        /// 分层ID; ①用于关联功能数值配置中的USERGROUP列，重要！重要！重要！; ②1-5已被占用，可自定义其他数字
        /// </summary>
        public int GroupId { get; set; }
        
        /// <summary>
        /// 字段名称; ①USERGROUP已占用，表示FIREBASE预测字段; ②其余字段名称可在【用户画像】字段内选择; 用户画像标签字段文档--数据平台; ③不可使用其他不可识别的字段
        /// </summary>
        public string DataKey { get; set; }
        
        /// <summary>
        /// 条件判断运算符; ①EQ: 等于，NEQ: 不等于; ②GT: 大于，GTE: 大于等于; ③LT: 小于，LTE: 小于等于; ④IN: 存在于DATAVALUE数组内，NIN: 不存在于DATAVALUE数组内
        /// </summary>
        public string Operator { get; set; }
        
        /// <summary>
        /// 条件值数据类型; ①若为ARRAYSTRING、ARRAYNUMBER或ARRAYTYPECODE 类型，则条件值以英文逗号“,”分隔; ②若为TIME类型，则条件值填写UTC0时区的时间字串; ③NUMBER表示整数，不支持小数; ④DAYS表示距离某个时间点多少天，字段名称必须为时间点类型的字段; ⑤SUM表示求和运算，DATAKEY必须为COLUMNA+COLUMNB的格式; ⑥MINUS表示求差运算，DATAKEY必须为COLUMNA-COLUMNB的格式; ⑦PRODUCT表示求乘积运算，DATAKEY必须为COLUMNA*COLUMNB的格式; ⑧DIVIDE表示求商运算，DATAKEY必须为COLUMNA/COLUMNB的格式; ⑨TYPECODE 表示枚举值
        /// </summary>
        public string DataType { get; set; }
        
        /// <summary>
        /// 条件值
        /// </summary>
        public string DataValue { get; set; }
        
        /// <summary>
        /// 描述; 0：默认; 1：指定内购组; 2：高概率付费组/高价值; 3：低概率付费组/自然量; 4：广告激进组
        /// </summary>
        public string Desc { get; set; }
        
    }
}