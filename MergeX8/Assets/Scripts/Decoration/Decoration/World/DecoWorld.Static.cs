using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DragonU3DSDK.Asset;
using Framework;
using Deco.Area;
using Deco.Item;
using Deco.Stage;
using Deco.Node;
using SomeWhere;

namespace Deco.World
{
    public partial class DecoWorld
    {
        public static Dictionary<int, DecoWorld> WorldLib = new Dictionary<int, DecoWorld>();
        public static Dictionary<int, Area.DecoArea> AreaLib = new Dictionary<int, Area.DecoArea>();
        public static Dictionary<int, Stage.DecoStage> StageLib = new Dictionary<int, Stage.DecoStage>();
        public static Dictionary<int, Node.DecoNode> NodeLib = new Dictionary<int, Node.DecoNode>();
        public static Dictionary<int, Item.DecoItem> ItemLib = new Dictionary<int, Item.DecoItem>();
        public static List<Node.DecoNode> SuggestNode = new List<Node.DecoNode>();
        /// <summary>
        /// 专用于存放活动装扮
        /// </summary>
       public static Dictionary<EActivityType, List<Item.DecoItem>> ActivityItemLib = new Dictionary<EActivityType, List<Item.DecoItem>>();

        public static Dictionary<int, PathPoint> PathPointLib = new Dictionary<int, PathPoint>();

        public static float DefaultZ = float.MaxValue;

        public static bool FrontTest(float a, float b)
        {
            return a < b;
        }

        public static float ConvertZFromSoringLayerAndOrder(int layerIndex, int sortingOrder)
        {
            return 100000 - layerIndex * 100 - sortingOrder;
        }
    }
}