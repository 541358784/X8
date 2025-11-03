using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using DragonU3DSDK;

namespace TMatch
{
    public class TMGiftBagLinkConfigManager : Singleton<TMGiftBagLinkConfigManager>
    {
        private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type, string>
        {
            { typeof(TMGiftBagLinkResourceConfig), "TMGiftBagLink" },
        };


        public List<TMGiftBagLinkResourceConfig> _giftBagLinkResourceConfigList;

        public void InitFromServerData(string configJson)
        {
            Hashtable table = null;
            try
            {
                if (string.IsNullOrEmpty(configJson) == false)
                {
                    table = JsonConvert.DeserializeObject<Hashtable>(configJson);
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogError(ex.ToString());
            }


            foreach (var subModule in typeToEnum)
            {
                try
                {
                    switch (subModule.Value)
                    {
                        case "TMGiftBagLink":
                            _giftBagLinkResourceConfigList =
                                JsonConvert.DeserializeObject<List<TMGiftBagLinkResourceConfig>>(
                                    JsonConvert.SerializeObject(table["tmgiftbaglink"]));
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                    }
                }
                catch (Exception ex)
                {
                    DebugUtil.LogError(ex.ToString());
                }
            }
        }
    }
}