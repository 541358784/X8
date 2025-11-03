
using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.Team;
using UnityEngine;

namespace Scripts.UI
{
    public partial class TeamManager
    {
        public int OpenTeamCardPackage(PassGiftData giftData,bool isGuide = false)
        {
            var cardData = giftData.ExtraData;
            var theme =  CardCollectionModel.Instance.GetCardThemeState(CardCollectionModel.Instance.TableCardPackage[cardData.CardPackageId].ThemeId);
            var themeList = CardCollectionModel.Instance.GetCardThemeLink(theme.CardThemeConfig.Id);
            var cutTheme = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme();

            var cardList = cardData.CardList.DeepCopy();
            if (themeList.Contains(cutTheme.CardThemeConfig.Id))//重定位cardId到当前开启卡册主题
            {
                for (var i = 0; i < cardList.Count; i++)
                {
                    cardList[i] = cardList[i] - theme.CardThemeConfig.Id * 1000 + cutTheme.CardThemeConfig.Id * 1000;
                }
            }

            var unCollectCards = new List<int>();
            for (var i = 0; i < cardList.Count; i++)
            {
                var cardItemState = CardCollectionModel.Instance.GetCardItemState(cardList[i]);
                if (cardItemState.CollectCount == 0)
                {
                    unCollectCards.Add(cardList[i]);
                }
            }
            var realIndex = 0;
            var realCardIndex = cardList.DeepCopy();
            if ((isGuide || Storage.CardFailTimes >= TeamConfigManager.Instance.LocalTeamConfig.CardFailTimes) && unCollectCards.Count > 0)//触发保底
            {
                Storage.CardFailTimes = 0;
                cardList = unCollectCards;
            }
            
            var index = Random.Range(0, cardList.Count);
            var card = cardList[index];
            for (var i=0;i < realCardIndex.Count; i++)
            {
                if (realCardIndex[i] == card)
                {
                    realIndex = i;
                }
            }
            if (unCollectCards.Count > 0 && !unCollectCards.Contains(card))//抽到重复卡，增加保底次数
            {
                Storage.CardFailTimes++;
            }
            var cardState = CardCollectionModel.Instance.GetCardItemState(card);
            cardState.CollectOneCard(GetCardSource.TeamCardPackage, "TeamCardPackage");
            var giftKey = giftData.TeamId + "_" + giftData.GiftId + "_" + giftData.ExpireTime;
            Storage.ClaimCardState.Add(giftKey);
            return realIndex;
        }
    }
}