using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;
using DragonPlus.Config.TMatchShop;
// using static DragonU3DSDK.Network.API.Protocol.BiEventMatchFrenzy.Types;
namespace TMatch
{


    public class TMatchCollectorSystem : GlobalSystem<TMatchCollectorSystem>, IInitable
    {
        private TMatchBaseItem[] collectItems = new TMatchBaseItem[TMatchEnvSystem.Instance.CollectorPos.Length];

        public TMatchBaseItem[] CollectItems
        {
            get => collectItems;
        }

        public void Init()
        {

        }

        public void Release()
        {
            for (int i = 0; i < collectItems.Length; i++) collectItems[i] = null;
        }

        public bool CanCollect(List<TMatchBaseItem> items)
        {
            if (items.Count == 0) return false;
            TMatchBaseItem[] changedCollectItems;
            List<TMatchBaseItem> needMoveItems;
            CollectCore(items, out changedCollectItems, out needMoveItems);
            return needMoveItems.Count != 0;
        }

        //增加 - 通过点击场景上的物件
        public void Collect(List<TMatchBaseItem> items, bool hit, bool effect, float time, Ease ease)
        {
            if (items.Count == 0) return;
            TMatchBaseItem[] changedCollectItems;
            List<TMatchBaseItem> needMoveItems;
            CollectCore(items, out changedCollectItems, out needMoveItems);
            //格子没满，但也不能收集
            if (needMoveItems.Count == 0)
            {
                if (hit)
                    foreach (var p in items)
                        p.Hit();
                return;
            }

            for (int i = 0; i < changedCollectItems.Length; i++) collectItems[i] = changedCollectItems[i];

            int useCnt = items.Count;
            foreach (var p in needMoveItems)
            {
                int itemIndex = items.FindIndex(x => x == p);
                int collectorIndex = 0;
                TMatchBaseItem tempItem = p;
                for (int i = 0; i < CollectItems.Length; i++)
                {
                    if (CollectItems[i] == p)
                    {
                        collectorIndex = i;
                        break;
                    }
                }

                //非新增里的，直接在收集栏之间移动
                if (itemIndex == -1)
                {
                    tempItem.DOMove(TMatchEnvSystem.Instance.CollectorPos[collectorIndex], 0.1f, null);
                }
                //新增里的，飞到收集栏；如果是最后一个则触发消除
                else
                {
                    if (effect)
                    {
                        GameObject vfxPrefab =
                            ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Particle/Prefabs/VFX_Adsorb",
                                addToCache: true);
                        GameObject.Instantiate(vfxPrefab, tempItem.GameObject.transform);
                    }

                    tempItem.DisbaleGravity();
                    tempItem.SleepPhysic();
                    tempItem.DisableCollider();
                    tempItem.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f, null);
                    tempItem.DORotate(tempItem.GetLayRot(), 0.1f, null);
                    tempItem.OperState = TMatchBaseItem.OperStateType.SceneToCollectore;
                    tempItem.DOMove(TMatchEnvSystem.Instance.CollectorPos[collectorIndex], time, () =>
                    {
                        tempItem.GameObject.transform.position =
                            TMatchEnvSystem.Instance.CollectorPos[GetItemIndex(tempItem)];
                        tempItem.DOMove(
                            TMatchEnvSystem.Instance.CollectorPos[GetItemIndex(tempItem)] +
                            new Vector3(0.0f, -0.2f, 0.0f), 0.05f,
                            () =>
                            {
                                tempItem.OperState = TMatchBaseItem.OperStateType.Collectore;

                                if (itemIndex == useCnt - 1)
                                {
                                    if (!CanRemove() && LeftSpace() == 0)
                                        EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_SPACEOUT);
                                    else if (IsBoost(tempItem)) RemoveBoost(tempItem);
                                    else RemoveTriple();

                                    SendItemChangeEvent();
                                }
                            });
                    }, ease);
                    EventDispatcher.Instance.DispatchEvent(new TMatchTripleEvent(tempItem.Id, -1));
                }
            }
        }

        public void CollectCore(List<TMatchBaseItem> items, out TMatchBaseItem[] changedCollectItems,
            out List<TMatchBaseItem> needMoveItems)
        {
            //消除时立即将元素数据清空掉了，但知道消除移动完成才会出发搜集栏位置调整；故CollectItems非紧凑，存在间隙；
            //逆向寻找相同的，找不到就排在后面；找到了：后面没有空隙，则后面往后移动；后面有空隙，则直接插入；
            changedCollectItems = new TMatchBaseItem[TMatchEnvSystem.Instance.CollectorPos.Length];
            for (int i = 0; i < changedCollectItems.Length; i++) changedCollectItems[i] = collectItems[i];
            needMoveItems = new List<TMatchBaseItem>();
            int itemId = items[0].Id;
            int useCnt = items.Count;
            int index = 999;
            for (int i = changedCollectItems.Length - 1; i >= 0; i--)
            {
                if (changedCollectItems[i] != null && changedCollectItems[i].Id == itemId)
                {
                    index = i;
                    break;
                }
            }

            //找不到就排在后面
            if (index == 999)
            {
                for (int i = changedCollectItems.Length - 1; i >= 0; i--)
                {
                    if (changedCollectItems[i] != null) break;
                    if (changedCollectItems[i] == null) index = i;
                }

                //后面放的下
                if (index + useCnt - 1 < changedCollectItems.Length)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        changedCollectItems[index + i] = items[i];
                        needMoveItems.Add(items[i]);
                    }
                }
            }
            //找到了 
            else
            {
                for (int m = 0; m < items.Count; m++)
                {
                    //不够空间插入
                    if (index >= changedCollectItems.Length - 1)
                    {
                        needMoveItems.Clear();
                        break;
                    }

                    //检查右边是否还有空间
                    int hasSpaceIndex = 999;
                    for (int i = index + 1; i < changedCollectItems.Length; i++)
                    {
                        if (changedCollectItems[i] == null)
                        {
                            hasSpaceIndex = i;
                            break;
                        }
                    }

                    //还有空间
                    if (hasSpaceIndex < changedCollectItems.Length)
                    {
                        for (int i = hasSpaceIndex; i > index + 1; i--)
                        {
                            changedCollectItems[i] = changedCollectItems[i - 1];
                            changedCollectItems[i - 1] = null;
                            needMoveItems.Add(changedCollectItems[i]);
                        }
                    }
                    //没有空间
                    else
                    {
                        needMoveItems.Clear();
                        break;
                    }

                    changedCollectItems[index + 1] = items[m];
                    needMoveItems.Add(items[m]);
                    index++;
                }
            }
        }

        //删除 - 三合一
        private void RemoveTriple()
        {
            for (int i = 1; i < CollectItems.Length - 1; i++)
            {
                int index = i;
                TMatchBaseItem left = CollectItems[index - 1];
                TMatchBaseItem center = CollectItems[index];
                TMatchBaseItem right = CollectItems[index + 1];
                if (left != null && center != null && right != null &&
                    left.Id == center.Id && left.Id == right.Id &&
                    left.OperState == TMatchBaseItem.OperStateType.Collectore &&
                    center.OperState == TMatchBaseItem.OperStateType.Collectore &&
                    right.OperState == TMatchBaseItem.OperStateType.Collectore)
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Matche);
                    // GameBIManager.Instance.LevelInfo.RemoveThingCount += 3;

                    left.OperState = TMatchBaseItem.OperStateType.Triple;
                    center.OperState = TMatchBaseItem.OperStateType.Triple;
                    right.OperState = TMatchBaseItem.OperStateType.Triple;

                    CollectItems[index - 1] = null;
                    CollectItems[index] = null;
                    CollectItems[index + 1] = null;

                    left.GameObject.transform.position = TMatchEnvSystem.Instance.CollectorPos[index - 1];
                    left.DOMove(TMatchEnvSystem.Instance.CollectorPos[index], 0.2f, null, Ease.InBack);

                    //right.ClearMoveQueue();//放在极端情况下，有2个loop在队列里面；第五关：篮球、篮球、南瓜、南瓜、快速点击：南瓜、篮球、篮球
                    right.GameObject.transform.position = TMatchEnvSystem.Instance.CollectorPos[index + 1];
                    right.DOMove(TMatchEnvSystem.Instance.CollectorPos[index], 0.2f, () =>
                    {
                        GameObject vfxPrefab =
                            ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Particle/Prefabs/VFX_Mix",
                                addToCache: true);
                        GameObject obj = GameObject.Instantiate(vfxPrefab, UIRoot.Instance.mRootCanvas.transform);
                        var position =
                            CameraManager.MainCamera.WorldToScreenPoint(center.GameObject.transform.position);
                        obj.transform.position = CameraManager.UICamera.ScreenToWorldPoint(position);
                        GameObject.Destroy(obj, 2.0f);

                        TMatchItemSystem.Instance.CollectItem(left);
                        TMatchItemSystem.Instance.CollectItem(right);

                        center.DOScale(Vector3.one, 0.15f, () =>
                        {
                            center.DOScale(Vector3.zero, 0.1f, () =>
                            {
                                TMatchItemSystem.Instance.CollectItem(center);
                                OnRemoveFinish();
                            });
                        });
                    }, Ease.InBack);
                }
            }
        }

        //删除 - 道具
        private void RemoveBoost(TMatchBaseItem baseItem)
        {
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Matche);
            string vfx = "";
            var cfg = TMatchShopConfigManager.Instance.GetItem(baseItem.Cfg.boosterId);
            if (cfg.GetItemType() == ItemType.TMLighting) vfx = "TMatch/Particle/Prefabs/VFX_LightClear";
            else if (cfg.GetItemType() == ItemType.TMClock) vfx = "TMatch/Particle/Prefabs/VFX_TimeClear";
            else vfx = "TMatch/Particle/Prefabs/VFX_SpecialClear";
            GameObject vfxPrefab = ResourcesManager.Instance.LoadResource<GameObject>(vfx, addToCache: true);
            GameObject obj = GameObject.Instantiate(vfxPrefab, UIRoot.Instance.mRootCanvas.transform);
            var position = CameraManager.MainCamera.WorldToScreenPoint(baseItem.GameObject.transform.position);
            obj.transform.position = CameraManager.UICamera.ScreenToWorldPoint(position);
            GameObject.Destroy(obj, 2.0f);

            int index = 0;
            for (int i = 0; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] == baseItem)
                {
                    baseItem.OperState = TMatchBaseItem.OperStateType.Triple;
                    CollectItems[i] = null;
                    index = i;
                    break;
                }
            }

            baseItem.DOScale(Vector3.one, 0.125f, () =>
            {
                baseItem.DOScale(Vector3.zero, 0.1f, () =>
                {
                    EventDispatcher.Instance.DispatchEvent(new TMatchTripleBoostEvent(baseItem.Id, index));

                    TMatchItemSystem.Instance.CollectItem(baseItem);
                    OnRemoveFinish();
                    EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(baseItem.Cfg.boosterId));
                });
            });
        }

        private bool CanRemove()
        {
            //triple
            for (int i = 1; i < CollectItems.Length - 1; i++)
            {
                int index = i;
                TMatchBaseItem left = CollectItems[index - 1];
                TMatchBaseItem center = CollectItems[index];
                TMatchBaseItem right = CollectItems[index + 1];
                if (left != null && center != null && right != null &&
                    left.Id == center.Id && left.Id == right.Id)
                {
                    return true;
                }
            }

            //boost
            for (int i = 0; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] != null && IsBoost(CollectItems[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnRemoveFinish()
        {
            for (int i = 1; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] == null) continue;
                for (int j = 0; j < i; j++)
                {
                    if (CollectItems[j] == null)
                    {
                        int tempIndex = j;
                        CollectItems[j] = CollectItems[i];
                        CollectItems[i] = null;
                        TMatchBaseItem tempItem = CollectItems[j];
                        tempItem.DOMove(TMatchEnvSystem.Instance.CollectorPos[j], 0.1f, null);
                        break;
                    }
                }
            }

            SendItemChangeEvent();
        }

        public int GetItemIndex(TMatchBaseItem item)
        {
            int index = CollectItems.Length - 1;
            for (int i = 0; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] == item)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public int LeftSpace()
        {
            int spaceLeft = 0;
            for (int i = 0; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] == null)
                {
                    spaceLeft++;
                    break;
                }
            }

            return spaceLeft;
        }

        private bool IsBoost(TMatchBaseItem baseItem)
        {
            return baseItem.Cfg.boosterId != 0;
        }

        public void SendItemChangeEvent()
        {
            int cnt = 0;
            for (int i = 0; i < CollectItems.Length; i++)
            {
                if (CollectItems[i] != null)
                {
                    cnt++;
                }
            }

            EventDispatcher.Instance.DispatchEvent(new TMatchCollectorItemChange(cnt));
        }
    }
}