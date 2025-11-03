//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System;
using System.Collections;
using DG.Tweening;
using DragonPlus.UI;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using SomeWhere;
using UnityEngine;

namespace TMatch
{
    /// <summary>
    /// 主界面
    /// </summary>
    public partial class TM_BPComMain : MonoBehaviour
    {
        /// <summary>
        /// 预制体路径
        /// </summary>
        private const string LEVEL_PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_BPCellLevel";

        /// <summary>
        /// 循环奖励
        /// </summary>
        private const string LOOP_PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_LoopReward";

        /// <summary>
        /// 列表
        /// </summary>
        private TableView tableView;

        /// <summary>
        /// 父位置
        /// </summary>
        private Transform contentTrans;

        /// <summary>
        /// 预制体
        /// </summary>
        private GameObject levelPrefab;

        /// <summary>
        /// 循环奖励预制体
        /// </summary>
        private GameObject loopPrefab;

        /// <summary>
        /// 引导第一个免费
        /// </summary>
        public TM_BPCellLevel GuideFreeItem;

        /// <summary>
        /// 引导列表动画完成回调
        /// </summary>
        private Action guideListAnimEndCallBack;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(bool inGuide, bool unlock)
        {
            levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(LEVEL_PREFAB_PATH);
            loopPrefab = ResourcesManager.Instance.LoadResource<GameObject>(LOOP_PREFAB_PATH);

            contentTrans = transform.Find("Root/Scroll View/Viewport/Content");

            tableView = transform.Find("Root/Scroll View").gameObject.AddComponent<TableView>();
            tableView.ArcCell = false;
            tableView.Init(numberOfCells, sizeOfIndex, transformOfIndex, onInitCell, identifierOfIndex, onCellMove,
                onStopMove);

            InitListShow(inGuide);

            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
        }

        private void InitListShow(bool inGuide)
        {
            if (inGuide)
            {
                //引导 定位到最后一个
                tableView.ReloadData(TableView.ReloadType.ScrollTo, TMBPModel.LevelCfg.Count + 1);
                tableView.content.anchoredPosition =
                    new Vector2(tableView.content.anchoredPosition.x, tableView.content.GetHeight());
            }
            else
            {
                if (TMBPModel.Instance.GetViewdLevel() != TMBPModel.Instance.GetCurLevel())
                {
                    tableView.ReloadData(TableView.ReloadType.ScrollTo, TMBPModel.Instance.GetViewdLevel());
                }
                else
                {
                    int canClaimLevel = TMBPModel.Instance.GetCanClaimId();
                    if (canClaimLevel != -1)
                    {
                        tableView.ReloadData(TableView.ReloadType.ScrollTo, canClaimLevel);
                    }
                    else if(TMBPModel.Instance.IsLoopLevel())
                    {
                        tableView.ReloadData(TableView.ReloadType.ScrollTo, TMBPModel.LevelCfg.Count + 1);
                        tableView.content.anchoredPosition =
                            new Vector2(tableView.content.anchoredPosition.x, tableView.content.GetHeight());
                    }
                    else
                    {
                        tableView.ReloadData(TableView.ReloadType.ScrollTo, TMBPModel.Instance.GetViewdLevel());
                    }
                }

                UpdateState();
            }
        }
        
        /// <summary>
        /// 购买事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnBuyEvent(BaseEvent obj)
        {
            if (obj is TM_BPBuyEvent)
            {
                UpdateState();
            }
        }
        
        /// <summary>
        /// 更新界面状态
        /// </summary>
        private void UpdateState()
        {
            StartCoroutine(onAnimation());
        }

        /// <summary>
        /// 动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator onAnimation()
        {
            UIMask.Enable(true);
            if (TMBPModel.Instance.StatusChanged())
            {
                for (var i = 0; i < TMBPModel.LevelCfg.Count + 1; i++)
                {
                    var cell = tableView.GetCell(i) as TM_BPCellBase;
                    if (cell == null)
                        continue;
                    yield return cell.OnStatusChangeToGolden();
                }
                TMBPModel.Instance.StatusViewed();
            }
            
            int lastLv = TMBPModel.Instance.GetViewdLevel();
            int nowLv = TMBPModel.Instance.GetCurLevel();
            if (nowLv != lastLv)
            {
                int addlv = nowLv - lastLv;
                float time = addlv * 0.5f;
                tableView.ScrollToTargetIndex(lastLv, nowLv, Ease.Linear, time);

                // 播放可领取动画
                for (var i = lastLv; i <= nowLv; i++)
                {
                    var cell = tableView.GetCell(i) as TM_BPCellBase;
                    if (cell == null)
                        continue;
                    StartCoroutine(cell.OnStatusChangeByLevel());
                    var lastCell = tableView.GetCell(i-1) as TM_BPCellLevel;
                    if (lastCell != null)
                    {
                        StartCoroutine(lastCell.HideLevelLine());
                    }
                    yield return new WaitForSeconds(0.5f);
                }
                EventDispatcher.Instance.DispatchEvent(new TM_BPListAnimEndEvent());
            }

            TMBPModel.Instance.SetLastShowLevel();
            UIMask.Enable(false);
        }

        /// <summary>
        /// 播放引导滑动动画
        /// </summary>
        /// <param name="callBack"></param>
        public void PlayGuideListAnim(Action callBack)
        {
            guideListAnimEndCallBack = callBack;
            StartCoroutine(PlayListAnim());
        }

        /// <summary>
        /// 播放列表动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayListAnim()
        {
            UIMask.Enable(true);
            yield return new WaitForSeconds(1f);
            tableView.ScrollToTargetIndex(TMBPModel.LevelCfg.Count + 1, 0, Ease.Linear, 4f,
                () => { StartCoroutine(ListAnimEndCall()); });
        }

        /// <summary>
        /// 列表动画结束
        /// </summary>
        /// <returns></returns>
        private IEnumerator ListAnimEndCall()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < contentTrans.childCount; i++)
            {
                GameObject obj = contentTrans.GetChild(i).gameObject;
                if (obj != null)
                {
                    TM_BPCellLevel cell = obj.GetComponent<TM_BPCellLevel>();
                    if (cell != null && cell.Index == 0)
                    {
                        GuideFreeItem = cell;
                        break;
                    }
                }
            }

            if (GuideFreeItem == null)
            {
                DebugUtil.LogError("tm战令引导 未找到指定位置的物体");
            }

            guideListAnimEndCallBack?.Invoke();
            UIMask.Enable(false);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
        }
    }
}