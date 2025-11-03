using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

namespace Screw
{
    public enum ProductType
    {
        Coin=1,
        DailyPackage=2,
        RebornPackage=3,
    }
    public class StoreModel:Singleton<StoreModel>
    {
        public List<DragonPlus.Config.Screw.TableShop> ConfigList => DragonPlus.Config.Screw.GameConfigManager.Instance.TableShopList;

        public void OnPurchase(TableShop cfg)
        {
            var config = ConfigList.Find(a => a.Id == cfg.id);
            if (config == null)
            {
                return;
            }

            switch ((ProductType)config.ProductType)
            {
                case ProductType.Coin:
                {
                    ShopCoinModel.Instance.OnPurchase(config);
                    break;
                }
                case ProductType.DailyPackage:
                {
                    DailyPackageModel.Instance.OnPurchase(config);
                    break;
                }
                case ProductType.RebornPackage:
                {
                    RebornPackageModel.Instance.OnPurchase(config);
                    break;
                }
                default:
                {
                    Debug.LogError("钉子商店类型错误"+config.ProductType);
                    break;
                }
            }
        }
    }
}