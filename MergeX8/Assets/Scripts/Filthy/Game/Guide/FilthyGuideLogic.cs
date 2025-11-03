using UnityEngine;

namespace Filthy.Game
{
    public class FilthyGuideLogic : Singleton<FilthyGuideLogic>
    {
        public void GuideLogic(bool isPointerUp = false)
        {
            if (!(SceneFsm.mInstance.GetCurrSceneType() == StatusType.EnterFilthy || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Transition))
                return;

            CheckMergeFinish();
        }


        private void CheckMergeFinish()
        {
            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MergeFinish))
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
                return;
            }

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MergeFinish, "");

            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
            {
                if (GuideSubSystem.Instance.CurrentConfig.mergeStartIndex > 0 && GuideSubSystem.Instance.CurrentConfig.mergeEndIndex > 0)
                {
                    var startGrid = UIFilthyMergeMainController.Instance.mergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeStartIndex];
                    var endGrid = UIFilthyMergeMainController.Instance.mergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeEndIndex];

                    if (startGrid.id == endGrid.id && startGrid.id.ToString() == GuideSubSystem.Instance.GetActionParams())
                    {
                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Clear();
                        UIFilthyMergeMainController.Instance.mergeBoard.CancelTip(-1, true);

                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Clear();
                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Add(startGrid);
                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Add(endGrid);

                        UIFilthyMergeMainController.Instance.mergeBoard.PlayTipsAnimation();


                        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeItem, startGrid.board.transform as RectTransform,
                            endGrid.board.transform as RectTransform);

                        return;
                    }
                }
            }

            if (UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList == null || UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Count == 0)
            {
                UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Clear();
                UIFilthyMergeMainController.Instance.mergeBoard.AutoTips();
            }

            if (MergeMainController.Instance.MergeBoard.mergeTipList == null || UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Count == 0)
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
                return;
            }

            MergeBoard.Grid grid_1 = null;
            MergeBoard.Grid grid_2 = null;

            if (!GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
                return;
            }

            if (GuideSubSystem.Instance.GetActionParams().IsEmptyString())
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
                return;
            }

            bool isCorrectTips = false;
            if (UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList != null && UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Count > 0)
            {
                int id = UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[0].id;
                if (GuideSubSystem.Instance.IsActionParams((id).ToString()))
                {
                    isCorrectTips = true;
                }
            }

            if (!isCorrectTips)
            {
                int acParams = GuideSubSystem.Instance.GetIntActionParams();
                //优先找任务需要的
                var mergeTipsItem = UIFilthyMergeMainController.Instance.mergeBoard.CalculateAutoTip(false, false, false, acParams);
                for (int i = 0; i < mergeTipsItem.Count; i = i + 2)
                {
                    int id = mergeTipsItem[i].id;
                    if (GuideSubSystem.Instance.IsActionParams((id).ToString()))
                    {
                        grid_1 = mergeTipsItem[i];
                        grid_2 = mergeTipsItem[i + 1];
                        break;
                    }
                }

                if (grid_1 != null && grid_2 != null)
                {
                    if (grid_1 != UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[0] || grid_2 != UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[1])
                    {
                        for (int i = 0; i < UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Count; i++)
                        {
                            UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[i].board.GetComponent<Animator>().Play("Normal");
                            UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[i].board.transform.localScale = Vector3.one;
                        }

                        UIFilthyMergeMainController.Instance.mergeBoard.CancelTip(-1, true);

                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Clear();
                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Add(grid_1);
                        UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList.Add(grid_2);

                        UIFilthyMergeMainController.Instance.mergeBoard.PlayTipsAnimation();
                    }
                }
            }
            else
            {
                grid_1 = UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[0];
                grid_2 = UIFilthyMergeMainController.Instance.mergeBoard.mergeTipList[1];
            }


            if (grid_1 == null || grid_2 == null)
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
                return;
            }

            bool isSwap = false;
            if (UIFilthyMergeMainController.Instance.mergeBoard.SelectIndex > 0)
            {
                var selectGrid = UIFilthyMergeMainController.Instance.mergeBoard.GetGridByIndex(UIFilthyMergeMainController.Instance.mergeBoard.SelectIndex);
                if (selectGrid != null)
                {
                    if (grid_2 == selectGrid)
                        isSwap = true;
                }
            }

            if (grid_1.state == 0) //蛛网交换
                isSwap = true;

            if (isSwap)
            {
                var tempData = grid_1;
                grid_1 = grid_2;
                grid_2 = tempData;
            }

            if (GuideSubSystem.Instance.CurrentConfig.autoSetIndex)
            {
                GuideSubSystem.Instance.CurrentConfig.mergeStartIndex = grid_1.board.index;
                GuideSubSystem.Instance.CurrentConfig.mergeEndIndex = grid_2.board.index;
            }

            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeItem, grid_1.board.transform as RectTransform,
                grid_2.board.transform as RectTransform);
        }
    }
}