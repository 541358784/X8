using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.ConfigHub.TMatchLevel;
using Framework;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using TMatch;
using TMatchCommonUtils = TMatch.CommonUtils;
namespace DragonPlus.Config.TMatch
{
    public partial class TMatchConfigManager
    {
        private int _groupId = -1;

        private int GetGroupId()
        {
            if (_groupId == -1)
            {
                _groupId = 100;
                var groups = TMatchLevelConfigManager.Instance.GetConfig<Mapping>();
                if (groups != null && groups.Count > 0) _groupId = groups[0].LevelUserGroup;
            }
            return _groupId;
        }
        
        public Level GetLevel(int levelId)
        {
            int groupId = GetGroupId();
            Level levelCfg = LevelList.Find(x => x.levelId == levelId && x.levelUserGroup == groupId);
            if (levelCfg != null) return levelCfg;
            TMacthDynamicCfg dynamicCfg = GetDynamicCfg(levelId);
            return dynamicCfg.Level;
        }

        public bool isDynamicCfg(int levelId)
        {
            int groupId = GetGroupId();
            Level levelCfg = LevelList.Find(x => x.levelId == levelId && x.levelUserGroup == groupId);
            return levelCfg == null;
        }
        
        public Layout GetLayout(int level, int layoutId)
        {
            if (layoutId == TMacthDynamicCfg.TMatchDynamicNormalLayoutId) return GetDynamicCfg(level).NormalLayout;
            if(layoutId == TMacthDynamicCfg.TMatchDynamicEasyerLayoutId) return GetDynamicCfg(level).easyerLayout;
            return LayoutList.Find(x => x.id == layoutId);
        }

        public TMatchDifficulty GetDifficulty(int levelId)
        {
            Level levelCfg = GetLevel(levelId);
            Layout layout = GetLayout(levelCfg.levelId, levelCfg.layoutId);//忽略难度是否降低
            return (TMatchDifficulty) layout.difficultyMark;
        }

        private static Dictionary<int, List<Item>> _dicItemList;
        private static Dictionary<int, List<Item>> DicItemList
        {
            get
            {
                if (_dicItemList == null)
                {
                    _dicItemList = new Dictionary<int, List<Item>>();
                    foreach (var item in TMatchConfigManager.Instance.ItemList)
                    {
                        if (!_dicItemList.ContainsKey(item.id))
                        {
                            _dicItemList.Add(item.id,new List<Item>());
                        }
                        _dicItemList[item.id].Add(item);
                    }
                }
                return _dicItemList;
            }
        }

        private static Dictionary<int, List<Layout>> _dicLayoutList;
        public static Dictionary<int, List<Layout>> DicLayoutList
        {
            get
            {
                if (_dicLayoutList == null)
                {
                    _dicLayoutList = new Dictionary<int, List<Layout>>();
                    foreach (var item in TMatchConfigManager.Instance.LayoutList)
                    {
                        if (!_dicLayoutList.ContainsKey(item.id))
                        {
                            _dicLayoutList.Add(item.id,new List<Layout>());
                        }
                        _dicLayoutList[item.id].Add(item);
                    }
                }
                return _dicLayoutList;
            }
        }

        public static Layout GetDicLayoutById(int id)
        {
            if (!DicLayoutList.ContainsKey(id))
                return null;
            if (DicLayoutList[id].Count == 0)
                return null;
            return DicLayoutList[id][0];
        }
        public static void AddDicLayout(Layout item)
        {
            if (!DicLayoutList.ContainsKey(item.id))
            {
                DicLayoutList.Add(item.id,new List<Layout>());
            }
            DicLayoutList[item.id].Add(item);
        }

        public static void RemoveDicLayout(Layout item)
        {
            if (!DicLayoutList.ContainsKey(item.id))
                return;
            DicLayoutList[item.id].Remove(item);
            if (DicLayoutList[item.id].Count == 0)
            {
                DicLayoutList.Remove(item.id);
            }
        }
        public static bool CheckLayoutValid(Layout layoutCfg, int layoutId)
        {
            // Layout layoutCfg = LayoutList.Find(x => x.id == layoutId);
            if (layoutCfg == null)
            {
                Debug.LogError($"layout : {layoutId} can not find");
                return false;
            }

            if (layoutCfg.targetItemId.Length != layoutCfg.targetItemCnt.Length)
            {
                Debug.LogError($"layout : {layoutId} TargetItemId.size != TargetItemCnt.size");
                return false;
            }

            for (int i = 0; i < layoutCfg.targetItemCnt.Length; i++)
            {
                if (layoutCfg.targetItemCnt[i] == 0 || layoutCfg.targetItemCnt[i] % 3 != 0)
                {
                    Debug.LogError($"layout : {layoutId} TargetItemCnt index {i}, value is {layoutCfg.targetItemCnt[i]}");
                    return false;
                }
            }

            for (int i = 0; i < layoutCfg.targetItemId.Length; i++)
            {
                for (int j = 0; j < layoutCfg.targetItemId.Length; j++)
                {
                    if(i == j) continue;
                    if (layoutCfg.targetItemId[i] == layoutCfg.targetItemId[j])
                    {
                        Debug.LogError($"layout : {layoutId} TargetItemId has same value!");
                        return false;
                    }
                }
            }

            if ((layoutCfg.normalItemId == null && layoutCfg.normalItemCnt != null) ||
                (layoutCfg.normalItemId != null && layoutCfg.normalItemCnt == null) ||
                (layoutCfg.normalItemId != null && layoutCfg.normalItemCnt != null && layoutCfg.normalItemId.Length != layoutCfg.normalItemCnt.Length))
            {
                Debug.LogError($"layout : {layoutId} NormalItemId.size != NormalItemCnt.size");
                return false;
            }

            if (null != layoutCfg.normalItemCnt)
            {
                for (int i = 0; i < layoutCfg.normalItemCnt.Length; i++)
                {
                    if (/*layoutCfg.NormalItemCnt[i] == 0 ||*/ layoutCfg.normalItemCnt[i] % 3 != 0)
                    {
                        Debug.LogError($"layout : {layoutId} NormalItemCnt index {i}, value is {layoutCfg.normalItemCnt[i]}");
                        return false;
                    }
                }
            }

            /*
            if (layoutCfg.NormalItemId != null)
            {
                for (int i = 0; i < layoutCfg.NormalItemId.Count; i++)
                {
                    for (int j = 0; j < layoutCfg.NormalItemId.Count; j++)
                    {
                        if(i == j) continue;
                        if (layoutCfg.NormalItemId[i] == layoutCfg.NormalItemId[j])
                        {
                            Debug.LogError($"layout : {layoutId} NormalItemId has same value!");
                            return false;
                        }
                    }
                }
            }
            */

            if (layoutCfg.levelType == 2)
            {
                if (layoutCfg.randomItemId.Length == layoutCfg.randomItemCnt.Length && 
                    layoutCfg.randomItemId.Length == layoutCfg.randomItemCntRange.Length &&
                    layoutCfg.randomItemId.Length == layoutCfg.randomItemMustHold.Length) 
                { }
                else
                {
                    Debug.LogError($"layout : {layoutId} RandomItemId.size != RandomItemCnt.size ||" +
                                   "RandomItemId.size != RandomItemCntRange.size ||" +
                                   "RandomItemId.size != RandomItemMustHold.size ||");
                    return false;
                }
            
                if (layoutCfg.randomItemIdCntMax > layoutCfg.randomItemId.Length)
                {
                    Debug.LogError($"layout : {layoutId} RandomItemIdCntMax > RandomItemId.Count");
                    return false;
                }
                
                if (null != layoutCfg.randomItemCnt)
                {
                    for (int i = 0; i < layoutCfg.randomItemCnt.Length; i++)
                    {
                        if (/*layoutCfg.RandomItemCnt[i] == 0 ||*/ layoutCfg.randomItemCnt[i] % 3 != 0)
                        {
                            Debug.LogError($"layout : {layoutId} RandomItemCnt index {i}, value is {layoutCfg.randomItemCnt[i]}");
                            return false;
                        }
                    }
                }
                
                /*
                if (layoutCfg.RandomItemId != null)
                {
                    for (int i = 0; i < layoutCfg.RandomItemId.Count; i++)
                    {
                        for (int j = 0; j < layoutCfg.RandomItemId.Count; j++)
                        {
                            if(i == j) continue;
                            if (layoutCfg.RandomItemId[i] == layoutCfg.RandomItemId[j])
                            {
                                Debug.LogError($"layout : {layoutId} RandomItemId has same value!");
                                return false;
                            }
                        }
                    }
                }
                */
            }

            List<string> missItem = new List<string>();
            foreach (var p in layoutCfg.targetItemId)
            {
                // if (ItemList.Find(x => x.id == p) == null)
                if(!DicItemList.ContainsKey(p))
                {
                    missItem.Add($"layout : {layoutId} TargetItemId : {p} can not find in items");
                }
            }
            if (null != layoutCfg.normalItemId)
            {
                foreach (var p in layoutCfg.normalItemId)
                {
                    // if (ItemList.Find(x => x.id == p) == null)
                    if(!DicItemList.ContainsKey(p))
                    {
                        missItem.Add($"layout : {layoutId} NormalItemId : {p} can not find in items");
                    }
                }
            }
            if (null != layoutCfg.randomItemId)
            {
                foreach (var p in layoutCfg.randomItemId)
                {
                    // if (ItemList.Find(x => x.id == p) == null)
                    if(!DicItemList.ContainsKey(p))
                    {
                        missItem.Add($"layout : {layoutId} RandomItemId : {p} can not find in items");
                    }
                }
            }
            foreach (var p in missItem) Debug.LogError(p);
            if (missItem.Count > 0) return false;

            List<string> repeatItem = new List<string>();
            foreach (var p in layoutCfg.targetItemId)
            {
                if (null != layoutCfg.normalItemId)
                {
                    if (layoutCfg.normalItemId.Contains(p))
                    {
                        repeatItem.Add($"layout : {layoutId} TargetItemId : {p} should not exist in NormalItemId");
                    }
                }
                if (null != layoutCfg.randomItemId)
                {
                    if (layoutCfg.randomItemId.Contains(p))
                    {
                        repeatItem.Add($"layout : {layoutId} TargetItemId : {p} should not exist in RandomItemId");
                    }
                }
            }
            foreach (var p in repeatItem) Debug.LogError(p);
            if (repeatItem.Count > 0) return false;
            
            return true;
        }

        public Item GetItem(int id)
        {
            return ItemList.Find(x => x.id == id);
        }

        public Item GetItemByBoosterId(int boosterId)
        {
            return ItemList.Find(x => x.boosterId == boosterId);
        }

        public Revive GetRevive(int type, int times)
        {
            List<Revive> cfgs = ReviveList.FindAll(x => x.type == type);
            for (int i = cfgs.Count - 1; i >=0 ; i--)
            {
                if (times >= cfgs[i].times)
                {
                    return cfgs[i];
                }
            }
            return null;
        }

        //动态生成关卡配置；生成原则：惰性
        public class TMacthDynamicCfg
        {
            public const int TMatchDynamicNormalLayoutId = int.MaxValue - 1;
            public const int TMatchDynamicEasyerLayoutId = int.MaxValue - 2;
            
            public Level Level;
            public Layout NormalLayout;
            public Layout easyerLayout;
        }
        
        public enum ItemColor
        {
            None,
            Red,
            Purple,
            White,
            Orange,
            Yellow,
            Green,
            Brown,
            Blue,
            Black,
            Pink
        }
        
        private const string TMatchCacheKeyDynamicCfg = "TMacthCacheDynamicCfg{0}";
        public const string TMatchCacheKeyDynamicN = "TMatchCacheKeyDynamicN{0}";
        private const string TMacthCacheKeyDynamicSpecialIndex = "TMacthCacheKeyDynamicSpecialIndex";
        public const string TMatchCacheKeyPreDifficult = "TMatchCacheKeyPreDifficult";
        
        public Dictionary<int, TMacthDynamicCfg> dynamicCfgs = new Dictionary<int, TMacthDynamicCfg>();

        public TMacthDynamicCfg GetDynamicCfg(int levelId)
        {
            //memory
            if (dynamicCfgs.ContainsKey(levelId)) return dynamicCfgs[levelId];
            //cache
            if (PlayerPrefs.HasKey(string.Format(TMatchCacheKeyDynamicCfg, levelId)))
            {
                TMacthDynamicCfg dynamicData = null;
                try
                {
                    dynamicData = JsonConvert.DeserializeObject<TMacthDynamicCfg>(PlayerPrefs.GetString(string.Format(TMatchCacheKeyDynamicCfg, levelId)));
                }
                catch (Exception e)
                {
                    Debug.LogError("can not Deserialize dynamic cfg!");
                }
                if (dynamicData != null)
                {
                    bool valid = true;
                    //核实缓存数据是否合法
                    //item是否否还存在，防止版本更新时将item删掉了
                    {
                        foreach (var p in dynamicData.NormalLayout.targetItemId)
                        {
                            if (GetItem(p) == null)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (null != dynamicData.NormalLayout.normalItemId)
                        {
                            foreach (var p in dynamicData.NormalLayout.normalItemId)
                            {
                                if (GetItem(p) == null)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                        if (null != dynamicData.NormalLayout.randomItemId)
                        {
                            foreach (var p in dynamicData.NormalLayout.randomItemId)
                            {
                                if (GetItem(p) == null)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                        
                        foreach (var p in dynamicData.easyerLayout.targetItemId)
                        {
                            if (GetItem(p) == null)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (null != dynamicData.easyerLayout.normalItemId)
                        {
                            foreach (var p in dynamicData.easyerLayout.normalItemId)
                            {
                                if (GetItem(p) == null)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                        if (null != dynamicData.easyerLayout.randomItemId)
                        {
                            foreach (var p in dynamicData.easyerLayout.randomItemId)
                            {
                                if (GetItem(p) == null)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!valid)
                    {
                        dynamicData = null;
                        Debug.LogError("dynamicData inValid from local cache!");
                    }
                }
                if (dynamicData != null)
                {
                    dynamicCfgs[levelId] = dynamicData;
                    return dynamicData;
                }
            }
            //create
            int groupId = GetGroupId();
            TMacthDynamicCfg dynamicCfg = new TMacthDynamicCfg();
            //level
            dynamicCfg.Level = new Level();
            dynamicCfg.Level.id = levelId;
            dynamicCfg.Level.levelId = levelId;
            dynamicCfg.Level.levelUserGroup = 100;
            dynamicCfg.Level.layoutId = TMacthDynamicCfg.TMatchDynamicNormalLayoutId;
            dynamicCfg.Level.reduceGradeThreshold = GlobalList[0].MatchLeveReduceGradeThreshold;
            dynamicCfg.Level.easierLayoutId = TMacthDynamicCfg.TMatchDynamicEasyerLayoutId;
            
            //layout
            LayoutDesign layoutDesign = LayoutDesignList.Find(x => levelId >= x.levelMin && levelId <= x.levelMax);
            //难度N值
            int difficulty = (int)TMatchDifficulty.Normal;
            if (PlayerPrefs.HasKey(string.Format(TMatchCacheKeyDynamicN, levelId)))
            {
                difficulty = PlayerPrefs.GetInt(string.Format(TMatchCacheKeyDynamicN, levelId), 1);
            }
            else
            {
                int difficultyValue = Random.Range(layoutDesign.difficultyValue[0], layoutDesign.difficultyValue[1] + 1);
                for (int i = levelId; i < levelId + difficultyValue; i++)
                {
                    PlayerPrefs.SetInt(string.Format(TMatchCacheKeyDynamicN, i), (int)TMatchDifficulty.Normal);
                }
                PlayerPrefs.SetInt(string.Format(TMatchCacheKeyDynamicN, levelId + difficultyValue), (int)TMatchDifficulty.Hard);
                PlayerPrefs.SetInt(string.Format(TMatchCacheKeyDynamicN, levelId + difficultyValue + 1), (int)TMatchDifficulty.Demon);
                difficulty = (int)TMatchDifficulty.Normal;
            }
            //特殊关卡，循环去取现成的配置
            int preDifficult = PlayerPrefs.GetInt(TMatchCacheKeyPreDifficult, 1);
            if (Random.Range(1, 101) <= layoutDesign.specialRatio && preDifficult != (int)TMatchDifficulty.Demon)
            {
                List<Layout> layouts = LayoutList.FindAll(x => x.specialType == 1 && x.specialUserGroup.Contains(groupId));
                int specialIndex = 0;
                if (PlayerPrefs.HasKey(TMacthCacheKeyDynamicSpecialIndex)) specialIndex = PlayerPrefs.GetInt(TMacthCacheKeyDynamicSpecialIndex, 0);
                if (specialIndex > layouts.Count - 1) specialIndex = 0;
                PlayerPrefs.SetInt(TMacthCacheKeyDynamicSpecialIndex, specialIndex + 1);
                dynamicCfg.NormalLayout = layouts[specialIndex];
                dynamicCfg.easyerLayout = LayoutList.Find(x => x.id == dynamicCfg.NormalLayout.specialEasierLayoutId);
                PlayerPrefs.SetInt(TMatchCacheKeyPreDifficult, dynamicCfg.NormalLayout.difficultyMark);
            }
            else
            {
                PlayerPrefs.SetInt(TMatchCacheKeyPreDifficult, difficulty);
                dynamicCfg.NormalLayout = new Layout();
                dynamicCfg.NormalLayout.id = TMacthDynamicCfg.TMatchDynamicNormalLayoutId;
                dynamicCfg.NormalLayout.levelType = difficulty == (int)TMatchDifficulty.Demon ? 2 : 1;
                dynamicCfg.NormalLayout.difficultyMark = difficulty;
                
                dynamicCfg.easyerLayout = new Layout();
                dynamicCfg.easyerLayout.id = TMacthDynamicCfg.TMatchDynamicEasyerLayoutId;
                dynamicCfg.easyerLayout.levelType = difficulty == (int)TMatchDifficulty.Demon ? 2 : 1;
                dynamicCfg.easyerLayout.difficultyMark = difficulty;

                //目标
                List<Item> targets = new List<Item>();
                List<int> targetCnts = new List<int>();
                //目标-A，根据难度指向的层级随机
                TargetDesign targetDesign = TargetDesignList.Find(x => x.difficulty == difficulty);
                List<Item> aItems = ItemList.FindAll(x => !x.forbid && targetDesign.firstTargetLayer.Contains(x.layer));
                //找不到定向的则随机取一个
                if (aItems.Count == 0)
                {
                    aItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null);
                }
                Item aItem = aItems[Random.Range(0, aItems.Count)];
                targets.Add(aItem);
                //目标-B，在A对应的主题中随机
                List<Item> bItems = ItemList.FindAll(x => !x.forbid && x.themeId == aItem.themeId && x.id != aItem.id);
                //找不到定向的则随机取一个
                if (bItems.Count == 0)
                {
                    bItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null);
                }
                Item bItem = bItems[Random.Range(0, bItems.Count)];
                targets.Add(bItem);
                //目标种类数
                int targetSize = 0;
                if (difficulty == (int)TMatchDifficulty.Normal) targetSize = Random.Range(layoutDesign.targetTypeNormal[0], layoutDesign.targetTypeNormal[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Hard) targetSize = Random.Range(layoutDesign.targetTypeHard[0], layoutDesign.targetTypeHard[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Demon) targetSize = Random.Range(layoutDesign.targetTypeDemon[0], layoutDesign.targetTypeDemon[1] + 1);
                for (int i = 0; i < targetSize; i++)
                {
                    List<Item> tempItems = null;
                    int tempValue = Random.Range(1, 5);
                    if (tempValue == 1)//模型A同色系
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && aItem.GetItemColor() == x.GetItemColor() && targets.Find(y => x.id == y.id) == null);
                    }
                    else if (tempValue == 2)//模型A同掉落层
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && aItem.layer == x.layer && targets.Find(y => x.id == y.id) == null);
                    }
                    else if (tempValue == 3)//模型B同色系
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && bItem.GetItemColor() == x.GetItemColor() && targets.Find(y => x.id == y.id) == null);
                    }
                    else if (tempValue == 4)//模型B同掉落层
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && bItem.layer == x.layer && targets.Find(y => x.id == y.id) == null);
                    }
                    if (tempItems != null)
                    {
                        //找不到定向的则随机取一个
                        if (tempItems.Count == 0)
                        {
                            tempItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null);
                        }
                        targets.Add(tempItems[Random.Range(0, tempItems.Count)]);
                    }
                }
                //目标数量
                foreach (var p in targets)
                {
                    int cnt = Random.Range(layoutDesign.targetCntMin, layoutDesign.targetCntMax + 1);
                    cnt = TMatchCommonUtils.UpToMultipleOf3(cnt);
                    targetCnts.Add(cnt);
                }
                dynamicCfg.NormalLayout.targetItemId = new int[targets.Count];
                int index = 0;
                foreach (var p in targets)
                {
                    dynamicCfg.NormalLayout.targetItemId[index++] = p.id;
                }
                dynamicCfg.NormalLayout.targetItemCnt = targetCnts.ToArray();
                dynamicCfg.easyerLayout.targetItemId = dynamicCfg.NormalLayout.targetItemId;
                dynamicCfg.easyerLayout.targetItemCnt = targetCnts.ToArray();
                
                //非目标
                List<Item> unTargets = new List<Item>();
                List<int> unTargetSizes = new List<int>();
                List<int> unTargetEasyerSizes = new List<int>();
                //非目标种类数量
                int unTargetTypeSize = 0;
                if (difficulty == (int)TMatchDifficulty.Normal) unTargetTypeSize = Random.Range(layoutDesign.untargetTypeNormal[0], layoutDesign.untargetTypeNormal[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Hard) unTargetTypeSize = Random.Range(layoutDesign.untargetTypeHard[0], layoutDesign.untargetTypeHard[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Demon) unTargetTypeSize = Random.Range(layoutDesign.untargetTypeDemon[0], layoutDesign.untargetTypeDemon[1] + 1);
                //非目标每个种类的平均数量
                int unTargetAvageSize = 0;
                int unTargetEasyerAvageSize = 0;
                if (difficulty == (int)TMatchDifficulty.Normal) unTargetAvageSize = Random.Range(layoutDesign.untargetCntNormal[0], layoutDesign.untargetCntNormal[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Hard) unTargetAvageSize = Random.Range(layoutDesign.untargetCntHard[0], layoutDesign.untargetCntHard[1] + 1);
                else if (difficulty == (int)TMatchDifficulty.Demon) unTargetAvageSize = Random.Range(layoutDesign.untargetCntDemon[0], layoutDesign.untargetCntDemon[1] + 1);
                LayoutGroupDesign layoutGroupDesign = LayoutGroupDesignList.Find(x => x.levelUserGroup == groupId);
                unTargetAvageSize = (int)(unTargetAvageSize * layoutGroupDesign.untargetCntValue);
                if (difficulty == (int)TMatchDifficulty.Normal) unTargetEasyerAvageSize = (int)(unTargetAvageSize * layoutGroupDesign.untargetCntValue * layoutDesign.untargetNormalLower);
                else if (difficulty == (int)TMatchDifficulty.Hard) unTargetEasyerAvageSize = (int)(unTargetAvageSize * layoutGroupDesign.untargetCntValue * layoutDesign.untargetHardLower);
                else if (difficulty == (int)TMatchDifficulty.Demon) unTargetEasyerAvageSize = (int)(unTargetAvageSize * layoutGroupDesign.untargetCntValue * layoutDesign.untargetDemonLower);
                unTargetAvageSize = (int)(unTargetAvageSize / (1.0f * unTargetTypeSize));
                unTargetAvageSize = TMatchCommonUtils.UpToMultipleOf3(unTargetAvageSize);
                unTargetEasyerAvageSize = (int)(unTargetEasyerAvageSize / (1.0f * unTargetTypeSize));
                unTargetEasyerAvageSize = TMatchCommonUtils.UpToMultipleOf3(unTargetEasyerAvageSize);
                int[] offset = {6, 3, 0, -3, -6};
                int offsetIndex = 0;
                for (int i = 0; i < unTargetTypeSize; i++)
                {
                    int tempSize = unTargetAvageSize + offset[offsetIndex];
                    if (tempSize < 3) tempSize = 3;
                    unTargetSizes.Add(tempSize);
                    
                    tempSize = unTargetEasyerAvageSize + offset[offsetIndex];
                    if (tempSize < 3) tempSize = 3;
                    unTargetEasyerSizes.Add(tempSize);
                    
                    offsetIndex++;
                    if (offsetIndex > offset.Length - 1) offsetIndex = 0;
                }
                //非目标种类
                //混色数量
                int sameColorSize = 0;
                if (difficulty == (int)TMatchDifficulty.Normal) sameColorSize = (int)(unTargetTypeSize * layoutGroupDesign.garbleNormalValue);
                else if (difficulty == (int)TMatchDifficulty.Hard) sameColorSize = (int)(unTargetTypeSize * layoutGroupDesign.garbleHardValue);
                else if (difficulty == (int)TMatchDifficulty.Demon) sameColorSize = (int)(unTargetTypeSize * layoutGroupDesign.garbleDemonValue);
                //同色：随机取与A或者B同色系的
                for (int i = 0; i < sameColorSize; i++)
                {
                    ItemColor itemColor = Random.Range(0, 2) == 0 ? aItem.GetItemColor() : bItem.GetItemColor();
                    List<Item> tempItems = ItemList.FindAll(x => !x.forbid && itemColor == x.GetItemColor() && 
                                                                 targets.Find(y => x.id == y.id) == null && unTargets.Find(y => x.id == y.id) == null);
                    //找不到定向的则随机取一个
                    if (tempItems.Count == 0)
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null && unTargets.Find(y => x.id == y.id) == null);
                    }
                    unTargets.Add(tempItems[Random.Range(0, tempItems.Count)]);
                }
                //非同色：每次取随机一个目标元素的同主题
                for (int i = sameColorSize; i < unTargetTypeSize; i++)
                {
                    Item tempTarget = targets[Random.Range(0, targets.Count)];
                    List<Item> tempItems = ItemList.FindAll(x => !x.forbid && tempTarget.themeId == x.themeId && 
                                                                 targets.Find(y => x.id == y.id) == null && unTargets.Find(y => x.id == y.id) == null);
                    //找不到定向的则随机取一个
                    if (tempItems.Count == 0)
                    {
                        tempItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null && unTargets.Find(y => x.id == y.id) == null);
                    }
                    unTargets.Add(tempItems[Random.Range(0, tempItems.Count)]);
                }

                var n_normalItemId = new List<int>();
                var n_normalItemCnt = new List<int>();
                var e_normalItemId = new List<int>();
                var e_normalItemCnt = new List<int>();
                for (int i = 0; i < unTargets.Count; i++)
                {
                    if (difficulty != (int)TMatchDifficulty.Demon || unTargetSizes[i] > 3)
                    {
                        n_normalItemId.Add(unTargets[i].id);
                        n_normalItemCnt.Add(unTargetSizes[i]);
                    }
                    if (difficulty != (int)TMatchDifficulty.Demon || unTargetEasyerSizes[i] > 3)
                    {
                        e_normalItemId.Add(unTargets[i].id);
                        e_normalItemCnt.Add(unTargetEasyerSizes[i]);
                    }
                }
                
                //如果是超级困难则还需要随机，则需要将数量为3的非目标拿到随机里面去二次处理
                dynamicCfg.NormalLayout.normalItemId = n_normalItemId.ToArray();
                dynamicCfg.NormalLayout.normalItemCnt = n_normalItemCnt.ToArray();
                dynamicCfg.easyerLayout.normalItemId = e_normalItemId.ToArray();
                dynamicCfg.easyerLayout.normalItemCnt = e_normalItemCnt.ToArray();
                
                //随机
                if (difficulty == (int)TMatchDifficulty.Demon)
                {
                    //普通
                    {
                        List<int> randoms = new List<int>();
                        //选非目标中数量为3的
                        for (int i = 0; i < unTargets.Count; i++)
                        {
                            if (unTargetSizes[i] == 3)
                            {
                                randoms.Add(unTargets[i].id);
                            }
                        }
                        int tempRandomMinSize = randoms.Count;
                        //全局随机2倍已有随机数量
                        int lastRandomSize = 2 * tempRandomMinSize;
                        for (int i = 0; i < lastRandomSize; i++)
                        {
                            List<Item> tempItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null && 
                                                                         unTargets.Find(y => x.id == y.id) == null &&
                                                                         !randoms.Contains(x.id));
                            if(tempItems.Count == 0) continue;
                            randoms.Add(tempItems[Random.Range(0, tempItems.Count)].id);
                        }
                        //随取选取
                        TMatchCommonUtils.ListRandom(randoms);
                        int tempNormalCnt = Random.Range(tempRandomMinSize, tempRandomMinSize + 3 + 1);
                        
                        var randomItemId = new List<int>();
                        var randomItemCnt = new List<int>();
                        var randomItemCntRange = new List<int>();
                        var randomItemMustHold = new List<int>();
                        
                        for (int i = 0; i < tempNormalCnt; i++)
                        {
                            randomItemId.Add(randoms[i]);
                            randomItemCnt.Add(3);
                            randomItemCntRange.Add(0);
                            randomItemMustHold.Add(1);
                        }
                        
                        dynamicCfg.NormalLayout.randomItemId = randomItemId.ToArray();
                        dynamicCfg.NormalLayout.randomItemCnt = randomItemCnt.ToArray();
                        dynamicCfg.NormalLayout.randomItemCntRange = randomItemCntRange.ToArray();
                        dynamicCfg.NormalLayout.randomItemMustHold = randomItemMustHold.ToArray();
                        
                        dynamicCfg.NormalLayout.randomItemIdCntMin = randomItemId.Count;
                        dynamicCfg.NormalLayout.randomItemIdCntMax = randomItemId.Count;
                    }

                    //降难度
                    {
                        List<int> randoms = new List<int>();
                        //选非目标中数量为3的
                        for (int i = 0; i < unTargets.Count; i++)
                        {
                            if (unTargetEasyerSizes[i] == 3)
                            {
                                randoms.Add(unTargets[i].id);
                            }
                        }
                        int tempRandomMinSize = randoms.Count;
                        //全局随机2倍已有随机数量
                        int lastRandomSize = 2 * tempRandomMinSize;
                        for (int i = 0; i < lastRandomSize; i++)
                        {
                            List<Item> tempItems = ItemList.FindAll(x => !x.forbid && targets.Find(y => x.id == y.id) == null && 
                                                                         unTargets.Find(y => x.id == y.id) == null &&
                                                                         !randoms.Contains(x.id));
                            if(tempItems.Count == 0) continue;
                            randoms.Add(tempItems[Random.Range(0, tempItems.Count)].id);
                        }
                        //随取选取
                        CommonUtils.ListRandom(randoms);
                        int tempNormalCnt = Random.Range(tempRandomMinSize, tempRandomMinSize + 3 + 1);
                        
                        var randomItemId = new List<int>();
                        var randomItemCnt = new List<int>();
                        var randomItemCntRange = new List<int>();
                        var randomItemMustHold = new List<int>();
                        
                        for (int i = 0; i < tempNormalCnt; i++)
                        {
                            randomItemId.Add(randoms[i]);
                            randomItemCnt.Add(3);
                            randomItemCntRange.Add(0);
                            randomItemMustHold.Add(1);
                        }


                        dynamicCfg.easyerLayout.randomItemId = randomItemId.ToArray();
                        dynamicCfg.easyerLayout.randomItemCnt = randomItemCnt.ToArray();
                        dynamicCfg.easyerLayout.randomItemCntRange = randomItemCntRange.ToArray();
                        dynamicCfg.easyerLayout.randomItemMustHold = randomItemMustHold.ToArray();
                        
                        dynamicCfg.easyerLayout.randomItemIdCntMin = randomItemId.Count;
                        dynamicCfg.easyerLayout.randomItemIdCntMax = randomItemId.Count;
                    }
                }
                
                //时间，（降难度的直接用普通的）
                int totalNormalCnt = 0;
                foreach (var p in dynamicCfg.NormalLayout.targetItemCnt) totalNormalCnt += p;
                foreach (var p in dynamicCfg.NormalLayout.normalItemCnt) totalNormalCnt += p;
                if (null != dynamicCfg.NormalLayout.randomItemCnt)
                {
                    foreach (var p in dynamicCfg.NormalLayout.randomItemCnt) totalNormalCnt += p;
                }
                if (difficulty == (int)TMatchDifficulty.Normal) totalNormalCnt = (int)(totalNormalCnt * layoutDesign.timeNormal);
                else if (difficulty == (int)TMatchDifficulty.Hard) totalNormalCnt = (int)(totalNormalCnt * layoutDesign.timeHard);
                else if (difficulty == (int)TMatchDifficulty.Demon) totalNormalCnt = (int)(totalNormalCnt * layoutDesign.timeDemon);
                dynamicCfg.NormalLayout.levelTimes = (totalNormalCnt / 5 + (totalNormalCnt % 5 == 0 ? 0 : 1)) * 5;
                dynamicCfg.easyerLayout.levelTimes = dynamicCfg.NormalLayout.levelTimes;
            }
            
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            string jsonData = JsonConvert.SerializeObject(dynamicCfg, setting);
            PlayerPrefs.SetString(string.Format(TMatchCacheKeyDynamicCfg, levelId), jsonData);
            dynamicCfgs[levelId] = dynamicCfg;
            return dynamicCfg;
        }

        public void DeleteDynamicCfgCache(int levelId)
        {
            PlayerPrefs.DeleteKey(string.Format(TMatchCacheKeyDynamicCfg, levelId));
        }
        
        public GlodenHatter GetGlodenHatterByTimes(int times)
        {
            for (int i = GlodenHatterList.Count - 1; i >= 0; i--)
            {
                if (times >= GlodenHatterList[i].times)
                {
                    return GlodenHatterList[i];
                }
            }
            return null;
        }
    }
}