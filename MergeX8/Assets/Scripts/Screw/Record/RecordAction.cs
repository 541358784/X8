using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace Screw
{
    public class RecordAction
    {
        public RecordAction(ScrewGameContext inContext)
        {
            context = inContext;
        }

        private ScrewGameContext context;
        private bool useOther;

        private float startTime;

        // 点击螺丝的id, 点击螺丝的时间戳
        private List<(float, int)> _actionList;//Dictionary<float, int> _actionDic;

        public void AddAction(int screwId)
        {
            if (useOther)
                return;

            if (_actionList == null)
            {
                _actionList = new List<(float, int)>();
                startTime = Time.realtimeSinceStartup;
            }

            _actionList.Add((Time.realtimeSinceStartup - startTime, screwId));
        }

        public void SetUseOther()
        {
            useOther = true;
        }

        public void Record()
        {
#if PRODUCTION_PACKAGE
            return;
#endif
            //迁移报错注释
            // if (useOther)
            //     return;
            //
            // if (_actionList == null)
            //     return;
            //
            // var records = StorageManager.Instance.GetStorage<StorageScrewJam>().Records;
            //
            // if (!records.ContainsKey(context.levelId))
            // {
            //     records.Add(context.levelId, new StorageScrewRecord());
            // }
            //
            // records[context.levelId].Actions.Clear();
            //
            // for (int i = 0; i < _actionList.Count; i++)
            // {
            //     var action = new StorageScrewActionRecord();
            //     if (i == 0)
            //     {
            //         action.ScrewId = _actionList[i].Item2;
            //         action.RecordTime = 0;
            //     }
            //     else
            //     {
            //         action.ScrewId = _actionList[i].Item2;
            //         action.RecordTime = _actionList[i].Item1 - _actionList[i - 1].Item1;
            //     }
            //     records[context.levelId].Actions.Add(action);
            // }
        }
    }
}