using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.LuckyGoldenEgg;
using DragonU3DSDK;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Activity.LuckyGoldenEgg
{
    public class LuckyGoldenEggLevel : MonoBehaviour
    {
        public TableLuckyGoldenEggLevelConfig _config;
        public List<LevelItem> _levelItems;

        private string[] _spineAnimName = new[]
        {
            "hen1_work",
            "hen2_work",
            "hen3_work",
        };
        
        private string[] _spineIdleAnimName = new[]
        {
            "hen1_idle",
            "hen2_idle",
            "hen3_idle",
        };
        
        private void Awake()
        {
        }
        
        public void Init(TableLuckyGoldenEggLevelConfig config, bool IsCurrent)
        {
            _config = config;
            _levelItems = new List<LevelItem>();
            for (int i = 0; i < config.ItemCount; i++)
            {
                int index = i;
                var item = transform.Find((i + 1).ToString());
                LevelItem levelItem = new LevelItem();
                levelItem.Normal = item.Find("chicken");
                levelItem._animator = item.GetComponent<Animator>();
                levelItem._object = item.gameObject;
                
                int childIndex = -1;
                foreach (Transform child in levelItem.Normal.transform)
                {
                    childIndex++;
                    if(!child.gameObject.activeSelf)
                        continue;

                    levelItem._skeletonGraphic = child.GetComponent<SkeletonGraphic>();
                    levelItem._index = childIndex;
                }

                for (int j = 1; j <= 3; j++)
                {
                    levelItem._effects.Add(item.Find($"FX_{j}"));
                }

                PlayAnim(levelItem);
                
                if (IsCurrent)
                    levelItem.Normal.gameObject.SetActive(!LuckyGoldenEggModel.Instance.IsBreak(index));
                levelItem.Button = item.GetComponent<Button>();
                levelItem.Button.onClick.AddListener(() =>
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.LuckyGoldenEggBreak);
                    DebugUtil.Log("Break index-->" + index);
                    if (!LuckyGoldenEggModel.Instance.IsBreak(index) &&
                        UserData.Instance.GetRes(UserData.ResourceId.GoldenEgg) > 0 && LuckyGoldenEggModel.Instance.CanBreak)
                    {
                        LuckyGoldenEggModel.Instance.BreakItem(index, _config);
                    }
                    else
                    {
                        if (UserData.Instance.GetRes(UserData.ResourceId.GoldenEgg) <= 0)
                        {
                            UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyGoldenEggNoItem);
                        }
                    }
                });
                levelItem.Null = item.Find("Null");
                levelItem.Null.gameObject.SetActive(false);
                //item.Find("Destructio").gameObject.SetActive(true);
                if (IsCurrent && i == 0)
                {
                    List<Transform> topLayer = new List<Transform>();
                    topLayer.Add(levelItem.Button.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.LuckyGoldenEggBreak, levelItem.Button.transform as RectTransform, topLayer: topLayer);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.LuckyGoldenEggDesc, levelItem.Button.transform as RectTransform);
                }

                _levelItems.Add(levelItem);
            }
        }

        private async UniTaskVoid PlayAnim(LevelItem item)
        {
            await UniTask.WaitForSeconds(Random.Range(0f, 0.3f));
            
            item._skeletonGraphic.AnimationState.SetAnimation(0, _spineIdleAnimName[item._index], true);
            item._skeletonGraphic.AnimationState.Update(0);
        }

        public async UniTask FlyAllAnim()
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach (var levelItem in _levelItems)
            {
                if (!levelItem.Normal.gameObject.activeSelf)
                    continue;
                
                var task = BreakAnim(levelItem, null);
                tasks.Add(task);
            }
            
            await UniTask.WhenAll(tasks);
            
            foreach (var levelItem in _levelItems)
            {
                levelItem._animator.Play("Disappear", 0, 0);
            }
            
            await UniTask.WaitForSeconds(0.5f);
        }

        public async UniTask BirthAnim()
        {
            _levelItems.ForEach(a=>a._object.SetActive(false));
            
            List<UniTask> tasks = new List<UniTask>();
            foreach (var levelItem in _levelItems)
            {
                var item = levelItem;
                var task = UniTask.Create(async () => {
                    await UniTask.WaitForSeconds(Random.Range(0, 0.5f));
                    item._object.SetActive(true);

                    item._animator.Play("Appear", 0, 0);
                    await UniTask.WaitForSeconds(0.5f);
                });
                tasks.Add(task);
            }
            
            await UniTask.WhenAll(tasks);
        }

        
        public async UniTask BreakAnim(LevelItem item, Action endCall)
        {
            await UniTask.WaitForSeconds(0.5f);
            
            //item._skeletonGraphic.transform.SetParent(transform);
            
            if(item._effects.Count > 0 && item._index >= 0 && item._index <= item._effects.Count-1)
                item._effects[item._index].gameObject.SetActive(true);
            
            Vector3 flyEndPosition = item._skeletonGraphic.transform.localPosition;
            flyEndPosition.y += 100;
                        
            item._skeletonGraphic.AnimationState.SetAnimation(0, _spineAnimName[item._index], true);
            item._skeletonGraphic.AnimationState.Update(0);

                        
            Vector3 moveEndPosition = flyEndPosition;
            if (item._skeletonGraphic.initialFlipY)
            {
                moveEndPosition.x = -Screen.currentResolution.width / 2 - 50;
            }
            else
            {
                moveEndPosition.x = Screen.currentResolution.width / 2 + 50;
            }
            moveEndPosition.y = Random.Range(item._skeletonGraphic.transform.localPosition.y+400, item._skeletonGraphic.transform.localPosition.y-400);
            
            float dis = Vector3.Distance(moveEndPosition, item._skeletonGraphic.transform.localPosition);
            dis /= 800;
            dis = Mathf.Clamp(dis, 0.5f, 1f);
            
            await item._skeletonGraphic.transform.DOLocalMove(flyEndPosition, 1f).SetEase(Ease.Linear);
            if(item._skeletonGraphic == null)
                return;
            
            await item._skeletonGraphic.transform.DOLocalMove(moveEndPosition, dis).SetEase(Ease.Linear);
            if(item._skeletonGraphic == null)
                return;
            item._skeletonGraphic.gameObject.SetActive(false);
            item.Normal.gameObject.SetActive(false);
            
            endCall?.Invoke();
        }

        public class LevelItem
        {
            public Transform Normal;
            public Transform Null;
            public Button Button;
            public int _index;
            public SkeletonGraphic _skeletonGraphic;
            public List<Transform> _effects = new List<Transform>();
            public Animator _animator;
            public GameObject _object;
        }
    }
}