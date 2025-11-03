using System.Collections.Generic;
using Screw;
using Screw.Configs;

namespace Screw
{
    public class LevelModel
    {
        private int _levelId;

        public string GuideKey;
        public Vector3Float GuidePos;

        public Dictionary<int, LayerModel> LayerModels { get; }

        public List<OrderModel> TaskModels { get; }

        public LevelModel(int levelId, string guideKey, Vector3Float guidePos, Dictionary<int, LayerModel> layerModels, List<OrderModel> taskModels)
        {
            _levelId = levelId;
            //GuideKey = guideKey;
            GuidePos = guidePos;
            LayerModels = layerModels;
            TaskModels = taskModels;
        }
    }
}