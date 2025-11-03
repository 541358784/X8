//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System;
using System.Collections;
using DragonPlus;
using DragonPlus.Config.TMBP;
using DragonU3DSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    /// <summary>
    /// 等级
    /// </summary>
    public class TM_BPCellLevel : TM_BPCellBase
    {
        /// <summary>
        /// 免费付费
        /// </summary>
        public TM_BPComItem freeItem, goldItem;

        /// <summary>
        /// 等级
        /// </summary>
        private LocalizeTextMeshProUGUI txtLevel;

        /// <summary>
        /// 进度条
        /// </summary>
        private Slider sliderExp;

        /// <summary>
        /// 进度条动画
        /// </summary>
        private Animator sliderAnim;

        /// <summary>
        /// 普通线条 激活线条
        /// </summary>
        private GameObject normalLine, activeLine;

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool haveInit;


        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent()
        {
            if (haveInit) return;

            normalLine = transform.Find("Root/BGGroup/NormalGroup").gameObject;
            activeLine = transform.Find("Root/BGGroup/ChoiceGroup").gameObject;

            freeItem = transform.Find("Root/TM_BPItemNormal").gameObject.GetOrCreateComponent<TM_BPComItem>();
            goldItem = transform.Find("Root/TM_BPItemGolden").gameObject.GetOrCreateComponent<TM_BPComItem>();

            txtLevel = transform.Find("Root/LevelInfo/Level/Disable/Text").GetComponent<LocalizeTextMeshProUGUI>();
            sliderExp = transform.Find("Root/LevelInfo/Slider").GetComponent<Slider>();
            sliderAnim = transform.Find("Root/LevelInfo").GetComponent<Animator>();
            sliderAnim.enabled = false;

            haveInit = true;

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BPOnListAnimEnd, OnListAnimEnd);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init(Base cfg, int index)
        {
            base.Init(cfg, index);

            InitComponent();

            freeItem.Init(cfg, TM_BpType.Normal);
            goldItem.Init(cfg, TM_BpType.Golden);

            InitLeveLevelInfo();
        }

        /// <summary>
        /// 初始化等级信息
        /// </summary>
        private void InitLeveLevelInfo()
        {
            txtLevel.SetText(cfg.id.ToString());
            sliderAnim.enabled = false;

            int viewLevel = TMBPModel.Instance.GetViewdLevel();
            int curLv = TMBPModel.Instance.GetCurLevel();
            bool useViewLevel = viewLevel != curLv;

            int level = useViewLevel ? TMBPModel.Instance.GetViewdLevel() : TMBPModel.Instance.GetCurLevel();

            normalLine.SetActive(cfg.id > level);
            // activeLine.SetActive(cfg.id <= level);
            activeLine.SetActive(cfg.id == level);

            if (useViewLevel)
            {
                if (cfg.id < level)
                {
                    sliderExp.value = 1;
                }
                else
                {
                    sliderExp.value = 0;
                }
            }
            else
            {
                sliderExp.value = GetSliderValue(level);
            }
        }

        /// <summary>
        /// 获取进度条进度值
        /// </summary>
        /// <returns></returns>
        private float GetSliderValue(int nowLevel)
        {
            float sliderValue = 1f;

            if (cfg.id == nowLevel + 1)
            {
                int nowExp = TMBPModel.Instance.Data.Exp;
                int preLevelExp = TMBPModel.Instance.GetTargetLevelExp(cfg.id - 1);
                sliderValue = (nowExp - preLevelExp) / (float)cfg.exp;
            }
            else if (cfg.id > nowLevel + 1)
            {
                sliderValue = 0;
            }

            return sliderValue;
        }

        /// <summary>
        /// 改变状态 normal-->gold
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStatusChangeToGolden()
        {
            yield return goldItem.OnStatusChangeToGolden();
        }

        /// <summary>
        /// 购买等级时 播放可领取动画
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStatusChangeByLevel()
        {
            //之前已经展示过了
            int curLv = TMBPModel.Instance.GetCurLevel();
            if (cfg.id <= curLv && Math.Abs(sliderExp.value - 1) > 0.001f)
            {
                sliderAnim.enabled = true;
                sliderAnim.Play("TM_BPCellLevelSlider");

                StartCoroutine(ShowLevelLine());
            }

            StartCoroutine(freeItem.OnStatusChangeByLevel());
            StartCoroutine(goldItem.OnStatusChangeByLevel());
            yield break;
        }

        /// <summary>
        /// 显示线条
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowLevelLine()
        {
            yield return new WaitForSeconds(0.15f);
            normalLine.SetActive(false);
            activeLine.SetActive(true);
        }
        public IEnumerator HideLevelLine()
        {
            yield return new WaitForSeconds(0.15f);
            normalLine.SetActive(false);
            activeLine.SetActive(false);
        }

        /// <summary>
        /// 动画结束事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnListAnimEnd(BaseEvent obj)
        {
            //手动填充动画没有改到的
            if (cfg.id == TMBPModel.Instance.GetCurLevel() + 1)
            {
                if (sliderExp.value == 0)
                {
                    int nowExp = TMBPModel.Instance.Data.Exp;
                    int preLevelExp = TMBPModel.Instance.GetTargetLevelExp(cfg.id - 1);
                    int haveGetExp = nowExp - preLevelExp;
                    float tmpValue = 0;
                    if (haveGetExp > 0)
                    {
                        tmpValue = haveGetExp / (float)cfg.exp;
                    }

                    if (cfg.id == 1)
                    {
                        int a = 0;
                    }

                    sliderExp.value = tmpValue;
                }
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BPOnListAnimEnd, OnListAnimEnd);
        }
    }
}