using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.Makeover;
using Newtonsoft.Json;
using UnityEngine;

namespace MiniGame
{
    public class AsmrGroup
    {
        public List<AsmrStep> Steps;
        public TableAsmrGroup Config;

        private AsmrLevel _level;

        public AsmrGroup(TableAsmrGroup config, AsmrLevel level)
        {
            _level = level;
            Config = config;

            Steps = new List<AsmrStep>();

            InitSteps();
        }

        private void InitSteps()
        {
            var stepConfigList = MakeoverConfigManager.Instance.stepNewList;

            for (var i = 0; i < Config.stepIds.Length; i++)
            {
                var stepId = Config.stepIds[i];
                var stepConfig = stepConfigList.Find(c => c.id == stepId);

                Steps.Add(new AsmrStep(stepConfig));
            }
        }

        public void FinishStep(int stepId)
        {
            foreach (var asmrStep in Steps)
            {
                if (asmrStep.Config.id == stepId)
                {
                    asmrStep.Finish = true;
                    break;
                }
            }

            //整组完成，切下一组
            if (Steps.All(step => step.Finish))
            {
                _level.ToNextGroup();
            }
            else
            {
                _level.ToNextStep();
            }
        }
        
        public AsmrStep GetNextStep()
        {
            if (Config.fixedOrder == 2)
            {
                return Steps.Find(o => !o.Finish && IsUnlock(o));
            }

            return Steps.Find(o => !o.Finish);
        }

        public bool IsUnlock(AsmrStep step)
        {
            if (step.Config.unlock == null) return true;//（一般第一步都是走这里）没有配置代表没有条件，直接就是解锁的
            foreach (var id in step.Config.unlock)
            {
                var mstep = Steps.Find(o => o.Config.id == id);
                if (mstep != null && !mstep.Finish)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取当前生效的step列表
        /// </summary>
        /// <returns></returns>
        public List<AsmrStep> GetCurSteps(AsmrStep runingStep)
        {
            List<AsmrStep> result = new List<AsmrStep>();
            if (Config.fixedOrder == 1)
            {
                /* 整个group固定顺序 */
                result.Add(runingStep);
            }
            else if (Config.fixedOrder == 0)
            {
                /* 整个group不固定顺序（所有step全激活） */
                foreach (var step in Steps)
                {
                    if (step.Finish) continue;
                    result.Add(step);
                }
            }
            else
            {
                /* 混合机制 */
                foreach (var step in Steps)
                {
                    if (step.Finish) continue;
                    if (!IsUnlock(step)) continue;
                    result.Add(step);
                }
            }

            return result;
        }
    }
}