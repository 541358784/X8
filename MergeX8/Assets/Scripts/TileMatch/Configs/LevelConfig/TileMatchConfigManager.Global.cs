using System.Collections.Generic;
using UnityEngine;

namespace DragonPlus.Config.TileMatch
{
    public partial class TileMatchConfigManager
    {
        private Dictionary<int, List<TileLevel>> randomTileLeves = new Dictionary<int, List<TileLevel>>();
        private bool _isInitRandom = false;
        public List<TileLevel> GetRandomTileLevels(int difficulty)
        {
            if (!_isInitRandom)
            {
                foreach (var tileLevel in TileLevelList)
                {
                    if (tileLevel.difficulty <= 0) 
                        continue;

                    if (!randomTileLeves.ContainsKey(tileLevel.difficulty))
                        randomTileLeves[tileLevel.difficulty] = new List<TileLevel>();
                    
                    randomTileLeves[tileLevel.difficulty].Add(tileLevel);
                }

                _isInitRandom = true;
            }

            if (randomTileLeves.ContainsKey(difficulty))
                return randomTileLeves[difficulty];

            return null;
        }
        public string GetString(string key)
        {
            var value = TileGlobalList.Find(a => a.key == key);
            if (value == null)
                return "";

            return value.value;
        }
        
        public int GetInt(string key)
        {
            var value = GetString(key);
            if (value.IsEmptyString())
                return 0;

            return int.Parse(value);
        }

        public string[] GetStringArray(string key, char separator)
        {
            var value = GetString(key);
            if (value.IsEmptyString())
                return null;

            return value.Split(separator);
        }
        
        public int[] GetIntArray(string key, char separator)
        {
            string[] split = GetStringArray(key, separator);
            if (split == null || split.Length == 0)
                return null;

            int[] intArray = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                intArray[i] = int.Parse(split[i]);
            }

            return intArray;
        }

        public int GetUnLockLevel(int resourceId)
        {
            var config = PropUnlockList.Find(a => a.propId == resourceId);
            if (config == null)
                return -1;
            
            return config.unlockLevel;
        }

        public bool IsUnlockProp(int resourceId)
        {
            return true;
        }
    }
}