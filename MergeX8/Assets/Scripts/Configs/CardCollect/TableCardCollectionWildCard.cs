using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionWildCard : TableBase
    {   
        // 万能卡ID
        public int Id { get; set; }// 万能卡可兑换的卡牌等级范围
        public List<int> LevelRange { get; set; }// 万能卡可兑换的卡册主题范围(THEMEID) (暂时不用)
        public List<int> ThemeRange { get; set; }// 万能卡ICON资源
        public string IconSprite { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
