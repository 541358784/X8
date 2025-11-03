// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FeatureUnlockInfo
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableFeatureUnlockInfo
    {   
        // #ID
        public int Id { get; set; }// #1:STARSHAPE; #2:TRIANGLESHAPE; #3DIAMONDSHAPE; #4:BOMBBLOCKER; #5:CONNECTBLOCKER; #6:ICEBLOCKER; #7:LOCKBLOCKER; #8:SHUTTERBLOCKER; #9:TIEBLOCKER; 该列的顺序和程序枚举值对应不能做任何修改
        public int FeatureType { get; set; }// #自制解锁等级
        public int UnlockLevel { get; set; }
    }
}