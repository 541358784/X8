using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    public partial class CardCollectConfigManager
    {
        public Dictionary<int, TableCardCollectionTheme> TableCardTheme = new Dictionary<int, TableCardCollectionTheme>();
        public Dictionary<int, TableCardCollectionCardBook> TableCardBook = new Dictionary<int, TableCardCollectionCardBook>();
        public Dictionary<int, TableCardCollectionCardItem> TableCardItem = new Dictionary<int, TableCardCollectionCardItem>();
        public Dictionary<int, TableCardCollectionCardPackage> TableCardPackage = new Dictionary<int, TableCardCollectionCardPackage>();
        public Dictionary<int, TableCardCollectionCardPackageItem> TableCardPackageItem = new Dictionary<int, TableCardCollectionCardPackageItem>();
        public Dictionary<int, TableCardCollectionRandomGroup> TableCardRandomGroup = new Dictionary<int, TableCardCollectionRandomGroup>();
        public Dictionary<int, TableCardCollectionWildCard> TableCardWildCard = new Dictionary<int, TableCardCollectionWildCard>();
        public Dictionary<int, TableCardCollectionCardPackageExchange> TableCardPackageResourceExchange = new Dictionary<int, TableCardCollectionCardPackageExchange>();
        
        protected override void Trim()
        {
            InitTable<TableCardCollectionTheme>(TableCardTheme, TableCardCollectionThemeList);
            InitTable<TableCardCollectionCardBook>(TableCardBook, TableCardCollectionCardBookList);
            InitTable<TableCardCollectionCardItem>(TableCardItem, TableCardCollectionCardItemList);
            InitTable<TableCardCollectionCardPackage>(TableCardPackage, TableCardCollectionCardPackageList);
            InitTable<TableCardCollectionCardPackageItem>(TableCardPackageItem, TableCardCollectionCardPackageItemList);
            InitTable<TableCardCollectionRandomGroup>(TableCardRandomGroup, TableCardCollectionRandomGroupList);
            InitTable<TableCardCollectionWildCard>(TableCardWildCard, TableCardCollectionWildCardList);
            InitTable<TableCardCollectionCardPackageExchange>(TableCardPackageResourceExchange, TableCardCollectionCardPackageExchangeList);
        }
        
        private void InitTable<T>(Dictionary<int, T> map, List<T> config) where T : TableBase
        {
            if (config == null || map == null)
                return;

            map.Clear();
            foreach (T kv in config)
            {
                map.Add(kv.GetID(), kv);
            }
        }
    }
}