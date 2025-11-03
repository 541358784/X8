using System;
using System.Collections.Generic;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using DragonPlus;


namespace TMatch
{
    public class UITMatchMainTaskItem
    {
        public Item itemCfg;
        public int cnt;
        public GameObject obj;

        public async Task Destory()
        {
            var animator = obj.GetComponent<Animator>();
            animator.Play("disappear");
            var animTime = CommonUtils.GetAnimTime(animator, "disappear");
            await Task.Delay((int)(animTime * 1000f));
            if (obj != null)
            {
                GameObject.Destroy(obj);
                obj = null;
            }
        }

        public void SetOriPosition(float posY = 0f)
        {
            var rectTransform = obj.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;
            var position = rectTransform.localPosition;
            position.x = size.x / 2;
            // position.y = posY;
            rectTransform.localPosition = position;
        }

        public void ShowBack()
        {
            obj.transform.Find("BackBg").gameObject.SetActive(true);
        }

        public void HideBack()
        {
            obj.transform.Find("BackBg").gameObject.SetActive(false);
        }

        public void DoMoveAndShowTransationAni(float targetPosX, int index)
        {
            var tween = DOTween.Sequence();
            if (index == 0)
            {
                tween.Append(obj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
            }
            else
            {
                tween.Append(obj.transform.DOLocalMoveX(targetPosX, 0.1f).SetEase(Ease.Linear));
            }

            tween.Append(obj.transform.DOScaleX(0.1f, 0.2f));
            tween.AppendCallback(() => { HideBack(); });
            tween.Append(obj.transform.DOScaleX(1, 0.2f));
        }

        public void RefreshPos(float targetPosX)
        {
            obj.transform.DOLocalMoveX(targetPosX, 0.1f);
        }
    }

    public partial class UITMatchMainController
    {
        private List<UITMatchMainTaskItem> tasks = new List<UITMatchMainTaskItem>();

        public List<UITMatchMainTaskItem> Tasks => tasks;

        public Transform GuideTips => transform.Find("Root/TaskGroup/Task/GuideTips");

        public Transform GuideTips2 => transform.Find("Root/TaskGroup/Task/GuideTips2");

        private void InitTask()
        {
            Transform root = transform.Find("Root/TaskGroup/Task/Viewport/Content");
            GameObject taskPrefab = root.Find("TaskItem").gameObject;
            taskPrefab.SetActive(false);
            var rootSize = root.GetComponent<RectTransform>().sizeDelta;

            for (int i = 0; i < TMatchSystem.LevelController.LevelData.layoutCfg.targetItemId.Length; i++)
            {
                int itemId = TMatchSystem.LevelController.LevelData.layoutCfg.targetItemId[i];
                UITMatchMainTaskItem taskItem = new UITMatchMainTaskItem();
                taskItem.itemCfg = TMatchConfigManager.Instance.GetItem(itemId);
                taskItem.cnt = TMatchSystem.LevelController.LevelData.layoutCfg.targetItemCnt[i];
                taskItem.obj = GameObject.Instantiate(taskPrefab, root);
                taskItem.SetOriPosition(-rootSize.y / 2);
                if (i == 0)
                {
                    taskItem.obj.transform.localScale = new Vector3(1.1f, 1.1f, 1);
                }

                taskItem.obj.transform.SetAsFirstSibling();
                taskItem.obj.SetActive(true);
                tasks.Add(taskItem);

                RawImage icon = taskItem.obj.transform.Find("Icon").GetComponent<RawImage>();
                icon.gameObject.SetActive(true);
                icon.texture = ResourcesManager.Instance.LoadResource<Texture2D>($"TMatch/TMatch/TMatchItemIcon/{taskItem.itemCfg.prefabName}");
                taskItem.obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(taskItem.cnt.ToString());
            }

            // XUtility.WaitSeconds(0.5f, () =>
            // {
            //     transform.Find("Root/TaskGroup/Task").gameObject.SetActive(false);
            //     transform.Find("Root/TaskGroup/Task").gameObject.SetActive(true);
            // });
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRIPLE, OnTriple);
            ShowTaskAni();

            // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchTaskList, transform.Find("Root/TaskGroup/Task/GuideTips"));
            // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchTaskList, transform.Find("Root/TaskGroup/Task/GuideTips2"), "2");
        }

        private void ShowTaskAni()
        {
            for (var i = 0; i < tasks.Count; i++)
            {
                var taskItem = tasks[i];
                var width = taskItem.obj.GetComponent<RectTransform>().sizeDelta.x;
                var targetPosX = i * width + width / 2;
                taskItem.DoMoveAndShowTransationAni(targetPosX, i);
            }
        }

        private void RefreshTaskItemPos()
        {
            if (tasks.Count == 0) return;
            for (var i = 0; i < tasks.Count; i++)
            {
                var taskItem = tasks[i];
                var width = taskItem.obj.GetComponent<RectTransform>().sizeDelta.x;
                var targetPosX = i * width + width / 2;
                taskItem.RefreshPos(targetPosX);
            }
        }

        private void DestoryTask()
        {
            foreach (var p in tasks) p.Destory();
            tasks.Clear();

            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRIPLE, OnTriple);
        }

        private async void OnTriple(BaseEvent evt)
        {
            TMatchTripleEvent realEvt = evt as TMatchTripleEvent;
            UITMatchMainTaskItem taskItem = tasks.Find(x => x.itemCfg.id == realEvt.id);
            if (null == taskItem) return;
            taskItem.cnt += realEvt.deltaCnt;
            // bi 记录剩余目标数量
            // GameBIManager.Instance.LevelInfo.LeftGoalCount -= (uint)Math.Abs(realEvt.deltaCnt);
            taskItem.obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(taskItem.cnt.ToString());
            if (taskItem.cnt <= 0)
            {
                tasks.Remove(taskItem);
                await taskItem.Destory();
                RefreshTaskItemPos();
            }

            if (tasks.Count == 0)
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_TARGET_FINISH);
            }
        }
    }
}