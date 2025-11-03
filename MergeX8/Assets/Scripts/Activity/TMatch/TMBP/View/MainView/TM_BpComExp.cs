//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System.Collections;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    /// <summary>
    /// 经验组件
    /// </summary>
    public class TM_BpComExp : MonoBehaviour
    {
        #region ui mem

        private Slider sliderExp;
        private LocalizeTextMeshProUGUI textLevel;
        private LocalizeTextMeshProUGUI textExp;
        private RectTransform transExpVfx;
        private Transform transLevelVfx;
        private GameObject maxLevelObj;

        #endregion

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
            textLevel = transform.Find("Root/BG2/Text").GetComponent<LocalizeTextMeshProUGUI>();
            sliderExp = transform.Find("Root/BG3/Exp").GetComponent<Slider>();
            textExp = transform.Find("Root/BG3/Text").GetComponent<LocalizeTextMeshProUGUI>();
            transExpVfx = transform.Find("Root/BG3/Exp/Fill Area/Fill/vfx_Progressbar_01")
                .GetComponent<RectTransform>();
            transLevelVfx = transform.Find("Root/ep_Common_UP");
            maxLevelObj = transform.Find("Root/BG2/MaxLevel").gameObject;
            if (transLevelVfx != null) transLevelVfx.gameObject.SetActive(false);

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnExpChanged, BattlePassOnExpChanged);
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnLevelChanged, BattlePassOnLevelChanged);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnExpChanged, BattlePassOnExpChanged);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnLevelChanged,
                BattlePassOnLevelChanged);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            UpdateLevel();
        }

        /// <summary>
        /// 更新等级
        /// </summary>
        private void UpdateLevel()
        {
            int nowLevel = TMBPModel.Instance.GetCurLevel();
            bool max = nowLevel == TMBPModel.LevelCfg[TMBPModel.LevelCfg.Count - 1].id;
            if (max)
            {
                MaxLevelShow(nowLevel);
            }
            else
            {
                NormalLevelShow(nowLevel);
            }
            
            maxLevelObj.SetActive(max);
            textLevel.gameObject.SetActive(!max);
        }

        /// <summary>
        /// 最大等级显示
        /// </summary>
        private void MaxLevelShow(int nowLevel)
        {
            int loopExp = TMBPModel.Instance.LoopCfg.exp;
            int nowRemainExp = TMBPModel.Instance.Data.Exp - TMBPModel.Instance.AllExp;
            sliderExp.value = (float)nowRemainExp / loopExp;
            textExp.SetText($"{nowRemainExp}/{loopExp}");
        }

        /// <summary>
        /// 普通等级显示
        /// </summary>
        private void NormalLevelShow(int nowLevel)
        {
            int nextLevel = nowLevel + 1;
            textLevel.SetText(nextLevel.ToString());

            int nowExp = TMBPModel.Instance.Data.Exp;
            int nextLevelExp = TMBPModel.Instance.GetLevelConfig(nextLevel).exp;
            int curLevelExp = TMBPModel.Instance.GetTargetLevelExp(nowLevel);

            int haveExp = nowExp - curLevelExp;

            sliderExp.value = (float)haveExp / nextLevelExp;
            textExp.SetText($"{haveExp}/{nextLevelExp}");
        }

        /// <summary>
        /// 点击
        /// </summary>
        private void OnClick()
        {
            AudioManager.Instance.PlayBtnTap();

            // 完成任务，赢取积分，提升等级
            var tipKey = "UI_battlepass_tips05_text";
            var tip = LocalizationManager.Instance.GetLocalizedString(tipKey);
            TipBoxUIMain.Open(tip, transform, TipType.Top, 0, -70);
        }

        /// <summary>
        /// bp经常改变
        /// </summary>
        /// <param name="evt"></param>
        private void BattlePassOnExpChanged(BaseEvent evt)
        {
            if (!(evt is CommonEvent<TMBPModel, int, int, int> arg) || arg.Data1 != TMBPModel.Instance)
                return;
            UpdateLevel();
        }

        /// <summary>
        /// bp等级改变
        /// </summary>
        /// <param name="evt"></param>
        private void BattlePassOnLevelChanged(BaseEvent evt)
        {
            if (!(evt is CommonEvent<TMBPModel, int, int, int> arg) || arg.Data1 != TMBPModel.Instance)
                return;
            if (!gameObject.activeSelf)
                return;
            UpdateLevel();
        }
    }
}