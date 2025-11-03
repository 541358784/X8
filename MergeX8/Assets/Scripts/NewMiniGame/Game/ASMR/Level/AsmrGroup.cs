using System.Collections.Generic;
using System.Linq;
using ASMR;
using System;
using DragonPlus.Config.MiniGame;


namespace fsm_new
{
    public class AsmrGroup
    {
        public List<AsmrStep> Steps;
        public AsmrGroupConfig Config;

        private AsmrLevel _level;

        public AsmrGroup(AsmrGroupConfig config, AsmrLevel level)
        {
            _level = level;
            Config = config;

            Steps = new List<AsmrStep>();

            InitSteps();
        }

        private void InitSteps()
        {
            var stepConfigList = ASMRModel.Instance.AsmrStepConfigs;

            for (var i = 0; i < Config.StepIds.Count; i++)
            {
                var stepId = Config.StepIds[i];
                var stepConfig = stepConfigList.Find(c => c.Id == stepId);

                Steps.Add(new AsmrStep(stepConfig));
            }
        }

        public void FinishStep(int stepId)
        {
            foreach (var asmrStep in Steps)
            {
                if (asmrStep.Config.Id == stepId)
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
        
        public bool IsUnlock(AsmrStep step)
        {
            if (step.Config.DependStepId == null) return true; //（一般第一步都是走这里）没有配置代表没有条件，直接就是解锁的
            foreach (var id in step.Config.DependStepId)
            {
                var mstep = Steps.Find(o => o.Config.Id == id);
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
            var result = new List<AsmrStep>();

            switch ((AsmrStepOrderType)Config.FixedOrder)
            {
                case AsmrStepOrderType.None:
                    foreach (var step in Steps)
                    {
                        if (step.Finish) continue;
                        result.Add(step);
                    }

                    break;
                case AsmrStepOrderType.FixedOrder:
                    result.Add(runingStep);

                    break;
                case AsmrStepOrderType.MixedOrder:
                    foreach (var step in Steps)
                    {
                        if (step.Finish) continue;
                        if (!IsUnlock(step)) continue;
                        result.Add(step);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

    }
}