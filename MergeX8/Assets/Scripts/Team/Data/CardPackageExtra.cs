using System;
using System.Collections.Generic;
using Activity.Base;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using Newtonsoft.Json;

namespace Scripts.UI
{
    public class CardPackageExtra
    {
        public List<int> CardList;
        public int CardPackageId;
        public float CardTotalValue;
        public int Price;

        public CardPackageExtra(List<int> cardList,int packageId)
        {
            CardList = cardList;
            CardPackageId = packageId;
            CardTotalValue = 0;
            var level5CardCount = 0;
            foreach (var card in cardList)
            {
                var config = CardCollectionModel.Instance.GetCardItemState(card).CardItemConfig;
                CardTotalValue += config.CardValue;
                if (config.Level == 5)
                    level5CardCount++;
            }
            CardTotalValue /= CardList.Count;
            if (level5CardCount == 0)
                Price = 1;
            else if (level5CardCount == 1)
                Price = 2;
            else if (level5CardCount == 2)
                Price = 3;
            else if (level5CardCount == 3)
                Price = 4;
            else
                Price = 0;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static CardPackageExtra FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CardPackageExtra>(json);
        }
    }
}