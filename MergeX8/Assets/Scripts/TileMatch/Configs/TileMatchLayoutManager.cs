using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using LayoutData;
using Newtonsoft.Json;
using UnityEngine;

public class TileMatchLayoutManager : Singleton<TileMatchLayoutManager>
{
   private Dictionary<int, LayoutGroup> _dicLayoutGroups = new Dictionary<int, LayoutGroup>();

   public LayoutGroup GetLayoutGroup(int layoutId)
   {
      string configPath = "Configs/Layout/";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
      if(SROptions.Current.IsUserTestFinderPath)
         configPath = "Configs/LayoutDebug/";
      
      _dicLayoutGroups.Clear();
#endif
      
      if (_dicLayoutGroups.ContainsKey(layoutId))
         return _dicLayoutGroups[layoutId];
      
      TextAsset ta = ResourcesManager.Instance.LoadResource<TextAsset>(string.Format(configPath+"{0}", layoutId));
      if (ta == null)
      {
         Debug.LogError("layout id 配置错误 " + layoutId);
         return null;
      }
      
      string jsStr = ta.text;
      if (string.IsNullOrEmpty(jsStr))
      {
         Debug.LogError("layout id 配置错误 null data " + layoutId);
         return null;
      }
      
      var layout = JsonConvert.DeserializeObject<LayoutGroup>(jsStr);
      _dicLayoutGroups.Add(layoutId, layout);
      
      return _dicLayoutGroups[layoutId];
   }
}
