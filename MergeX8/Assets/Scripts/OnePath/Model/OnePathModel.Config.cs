using System.Collections.Generic;

namespace OnePath.Model
{
    public partial class OnePathModel
    {
        // public Dictionary<int, OnePathChunk> _onePathConfigs = new Dictionary<int, OnePathChunk>();
        // public OnePathConfig _onePathConfig;
        
        // public void InitConfig()
        // {
        //     _onePathConfigs.Clear();
        //     
        //     if(_config == null)
        //         return;
        //
        //     _onePathConfig = OnePathConfigManager.Instance.GetConfig(_config.levelId);
        //     if(_onePathConfig == null)
        //         return;
        //     
        //     _onePathConfig._chunks.ForEach(a =>
        //     {
        //         _onePathConfigs.Add(a.index, a);
        //         a.GetRect(_onePathConfig._width, _onePathConfig._height);
        //     });
        // }
    }
}