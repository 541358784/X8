using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus.Config.TMatch;
using UnityEngine;

namespace TMatch
{


    public class TMatchPrepareStateParam : TMatchStateParam
    {
        public FsmParamTMatch fsmParamTMatch;
    }

    public class TMatchPrepareState : TMatchBaseState
    {
        TMatchStateType TMatchBaseState.Type => TMatchStateType.Prepare;

        public async Task Enter(TMatchStateParam param)
        {
            TMatchPrepareStateParam prepareStateParam = param as TMatchPrepareStateParam;
            Layout layoutCfg = prepareStateParam.fsmParamTMatch.layoutCfg;
            if (prepareStateParam.fsmParamTMatch.GameType == TMGameType.Normal)
            {
#if DEBUG || DEVELOPMENT_BUILD
                List<Layout> layoutList = new List<Layout>();
                foreach (var p in TMatchConfigManager.Instance.LayoutList) layoutList.Add(p);
                if (TMatchConfigManager.Instance.isDynamicCfg(prepareStateParam.fsmParamTMatch.level))
                {
                    TMatchConfigManager.TMacthDynamicCfg dynamicCfg =
                        TMatchConfigManager.Instance.GetDynamicCfg(prepareStateParam.fsmParamTMatch.level);
                    layoutList.Add(dynamicCfg.NormalLayout);
                    layoutList.Add(dynamicCfg.easyerLayout);
                }
                Layout checkLayOutCfg = layoutList.Find(x => x.id == layoutCfg.id);
                if (!TMatchConfigManager.CheckLayoutValid(checkLayOutCfg, layoutCfg.id))
                    return;
#endif   
            }
            Dictionary<int, int> layoutResult = new Dictionary<int, int>();
            //目标数据
            for (int i = 0; i < layoutCfg.targetItemId.Length; i++)
            {
                if (layoutResult.ContainsKey(layoutCfg.targetItemId[i]))
                {
                    layoutResult[layoutCfg.targetItemId[i]] += layoutCfg.targetItemCnt[i];
                }
                else
                {
                    layoutResult[layoutCfg.targetItemId[i]] = layoutCfg.targetItemCnt[i];
                }
            }

            //普通数据
            if (layoutCfg.normalItemId != null)
            {
                for (int i = 0; i < layoutCfg.normalItemId.Length; i++)
                {
                    if (layoutResult.ContainsKey(layoutCfg.normalItemId[i]))
                    {
                        layoutResult[layoutCfg.normalItemId[i]] += layoutCfg.normalItemCnt[i];
                    }
                    else
                    {
                        layoutResult[layoutCfg.normalItemId[i]] = layoutCfg.normalItemCnt[i];
                    }
                }
            }

            //随机数据
            if (layoutCfg.levelType == 2)
            {
                int randomTypeCnt = Random.Range(layoutCfg.randomItemIdCntMin, layoutCfg.randomItemIdCntMax + 1);
                List<int> validType = new List<int>();
                for (int i = 0; i < layoutCfg.randomItemMustHold.Length; i++)
                {
                    validType.Add(layoutCfg.randomItemMustHold[i]);
                    if (layoutCfg.randomItemMustHold[i] == 1)
                    {
                        randomTypeCnt--;
                    }
                }

                for (int i = 0; i < randomTypeCnt; i++)
                {
                    List<int> tempIndex = new List<int>();
                    for (int j = 0; j < validType.Count; j++)
                    {
                        if (validType[j] == 0)
                        {
                            tempIndex.Add(j);
                        }
                    }

                    int validIndex = tempIndex[Random.Range(0, tempIndex.Count)];
                    validType[validIndex] = 1;
                }

                for (int i = 0; i < layoutCfg.randomItemId.Length; i++)
                {
                    if (validType[i] == 0) continue;
                    int cnt = layoutCfg.randomItemCnt[i] + layoutCfg.randomItemCntRange[i] * Random.Range(0, 2);
                    if (layoutResult.ContainsKey(layoutCfg.randomItemId[i]))
                    {
                        layoutResult[layoutCfg.randomItemId[i]] += cnt;
                    }
                    else
                    {
                        layoutResult[layoutCfg.randomItemId[i]] = cnt;
                    }
                }
            }

            TMatchCreateStateParam createStateParam = new TMatchCreateStateParam();
            createStateParam.fsmParamTMatch = prepareStateParam.fsmParamTMatch;
            createStateParam.layoutResult = layoutResult;
            
            EventDispatcher.Instance.DispatchEvent(new TMatchGameChangeStateEvent(TMatchStateType.Create,
                createStateParam));
        }

        public void Update(float deltaTime)
        {

        }

        public async Task Exit()
        {

        }
    }
}