using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using UnityEngine;

namespace TMatch
{


    public class TMatchBoostSystem : GlobalSystem<TMatchBoostSystem>, IInitable
    {
        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.GAME_ITEM_USE, OnBoostUseEvt);
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.GAME_ITEM_USE, OnBoostUseEvt);
        }

        private void OnBoostUseEvt(BaseEvent evt)
        {
            GameItemUseEvent realEvt = evt as GameItemUseEvent;
            // Item item = TMatch.GameConfigManager.Instance.GetItem(realEvt.itemId);
            var item = ItemModel.Instance.GetConfigById(realEvt.itemId);
            if (!realEvt.forFree && item.GetResouceId() != ResourceId.None)
            {
                if (!ItemModel.Instance.IsEnough(item.id) && !HaveInfinite(item)) return;
                // if (!CurrencyModel.Instance.CanAford(item.GetResouceId(), 1) && !HaveInfinite(item)) return;
                if (!HaveInfinite(item))
                {
                    // var args = new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.PlayLevel);
                    // args.data1 = TMatchSystem.LevelController.LevelData.level.ToString();
                    // CurrencyModel.Instance.CostRes(item.GetResouceId(), 1, args);
                    ItemModel.Instance.Cost(item.id, 1, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                    {
                        data1 = TMatchSystem.LevelController.LevelData.level.ToString(),
                        data2 = TMatchSystem.LevelController.GameType.ToString(),
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PlayLevelTm
                    });
                    EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(item.GetResouceId()));
                    if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                    {
                        (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(item.id, 1);
                    }
                }

                EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(realEvt.itemId));
            }

            switch (item.GetItemType())
            {
                case ItemType.TMMagnet:
                    UseMagnet(item);
                    break;
                case ItemType.TMBroom:
                    UseBroom(item);
                    break;
                case ItemType.TMWindmill:
                    UseWindmill(item);
                    break;
                case ItemType.TMFrozen:
                    UseFrozen(item);
                    break;
                case ItemType.TMLighting:
                    UseLighting(item);
                    break;
                case ItemType.TMLightingInfinity:
                    UseLighting(item);
                    break;
                case ItemType.TMClock:
                    UseClock(item);
                    break;
                case ItemType.TMClockInfinity:
                    UseClock(item);
                    break;
            }
        }

        private bool HaveInfinite(ItemConfig item)
        {
            if (item.GetItemType() == ItemType.TMLighting)
            {
                return UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMLightingInfinity);
            }
            else if (item.GetItemType() == ItemType.TMClock)
            {
                return UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMClockInfinity);
            }

            return false;
        }

        private bool magneting;
        private bool magnetingPre;

        public bool CanUseMagnet(ItemConfig item)
        {
            if (magneting) return false;

            bool costed = false;
            int targetId = 0;
            int targetCnt = 0;
            GetMagnetInfo(ref targetId, ref targetCnt);
            if (targetId != 0)
            {
                List<TMatchBaseItem> items = new List<TMatchBaseItem>();
                for (int i = 0; i < TMatchItemSystem.Instance.Items.Count; i++)
                {
                    TMatchBaseItem tempItem = TMatchItemSystem.Instance.Items[i];
                    if (tempItem.OperState != TMatchBaseItem.OperStateType.Scene) continue;
                    if (tempItem.Id == targetId)
                    {
                        items.Add(tempItem);
                        if ((--targetCnt) <= 0) break;
                    }
                }

                costed = TMatchCollectorSystem.Instance.CanCollect(items);
            }

            return costed;
        }

        public bool CanUseMagnetIgnoreInterval(ItemConfig item)
        {
            if (magnetingPre) return false;

            bool costed = false;
            int targetId = 0;
            int targetCnt = 0;
            GetMagnetInfo(ref targetId, ref targetCnt);
            if (targetId != 0)
            {
                List<TMatchBaseItem> items = new List<TMatchBaseItem>();
                for (int i = 0; i < TMatchItemSystem.Instance.Items.Count; i++)
                {
                    TMatchBaseItem tempItem = TMatchItemSystem.Instance.Items[i];
                    if (tempItem.OperState != TMatchBaseItem.OperStateType.Scene) continue;
                    if (tempItem.Id == targetId)
                    {
                        items.Add(tempItem);
                        if ((--targetCnt) <= 0) break;
                    }
                }

                costed = TMatchCollectorSystem.Instance.CanCollect(items);
            }

            return costed;
        }

        private async void UseMagnet(ItemConfig item)
        {
            // GameBIManager.Instance.LevelInfo.Magent++;
            CrocodileActivityModel.Instance.UseBoost();
            magneting = true;
            magnetingPre = true;
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Magnet);
            UIViewSystem.Instance.Get<UITMatchMainController>().SetMagnetEffectState(true);
            if (!await DelayCareDestory(200)) return;
            magnetingPre = false;
            int targetId = 0;
            int targetCnt = 0;
            GetMagnetInfo(ref targetId, ref targetCnt);
            if (targetId != 0)
            {
                List<TMatchBaseItem> items = new List<TMatchBaseItem>();
                for (int i = 0; i < TMatchItemSystem.Instance.Items.Count; i++)
                {
                    TMatchBaseItem tempItem = TMatchItemSystem.Instance.Items[i];
                    if (tempItem.OperState != TMatchBaseItem.OperStateType.Scene) continue;
                    if (tempItem.Id == targetId)
                    {
                        items.Add(tempItem);
                        if ((--targetCnt) <= 0) break;
                    }
                }

                TMatchCollectorSystem.Instance.Collect(items, false, true, 0.2f, Ease.OutQuad);
            }

            if (!await DelayCareDestory(800)) return;
            UIViewSystem.Instance.Get<UITMatchMainController>().SetMagnetEffectState(false);
            magneting = false;
        }

        private void GetMagnetInfo(ref int targetId, ref int targetCnt)
        {
            List<UITMatchMainTaskItem> targets = new List<UITMatchMainTaskItem>();
            foreach (var p in UIViewSystem.Instance.Get<UITMatchMainController>().Tasks) targets.Add(p);
            targets.Sort((a, b) =>
            {
                int a_index = 0;
                int b_index = 0;
                for (int i = 0; i < TMatchCollectorSystem.Instance.CollectItems.Length; i++)
                {
                    if (TMatchCollectorSystem.Instance.CollectItems[i] != null &&
                        TMatchCollectorSystem.Instance.CollectItems[i].Id == a.itemCfg.id)
                    {
                        a_index = i;
                        break;
                    }
                }

                for (int i = 0; i < TMatchCollectorSystem.Instance.CollectItems.Length; i++)
                {
                    if (TMatchCollectorSystem.Instance.CollectItems[i] != null &&
                        TMatchCollectorSystem.Instance.CollectItems[i].Id == b.itemCfg.id)
                    {
                        b_index = i;
                        break;
                    }
                }

                return a_index - b_index;
            });
            if (targets.Count == 0) return;

            //缺一个
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].cnt % 3 == 1)
                {
                    targetId = targets[i].itemCfg.id;
                    targetCnt = 1;
                    break;
                }
            }

            //缺两个
            if (targetId == 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i].cnt % 3 == 2)
                    {
                        targetId = targets[i].itemCfg.id;
                        targetCnt = 2;
                        break;
                    }
                }
            }

            //缺三个
            if (targetId == 0)
            {
                targetId = targets[0].itemCfg.id;
                targetCnt = 3;
            }
        }

        private bool brooming;

        public bool CanUseBroom(ItemConfig item)
        {
            if (brooming) return false;

            int cnt = 0;
            for (int i = 0; i < TMatchCollectorSystem.Instance.CollectItems.Length; i++)
            {
                TMatchBaseItem baseItem = TMatchCollectorSystem.Instance.CollectItems[i];
                if (baseItem != null) cnt++;
            }

            return cnt > 0 && cnt < TMatchCollectorSystem.Instance.CollectItems.Length;
        }

        private async void UseBroom(ItemConfig item)
        {
            // GameBIManager.Instance.LevelInfo.Broom++;
            CrocodileActivityModel.Instance.UseBoost();
            brooming = true;

            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Broom);
            UIViewSystem.Instance.Get<UITMatchMainController>().SetBroomEffectState(true);

            var cleanItemList = new List<TMatchBaseItem>();
            for (int i = 0; i < TMatchCollectorSystem.Instance.CollectItems.Length; i++)
            {
                TMatchBaseItem baseItem = TMatchCollectorSystem.Instance.CollectItems[i];
                if (baseItem == null) continue;
                cleanItemList.Add(baseItem);
                TMatchCollectorSystem.Instance.CollectItems[i] = null;
            }
            
            if (!await DelayCareDestory(500)) return;

            TMatchItemSystem.Instance.keepPositionDeltaTime = 0.0f;
            for (var i = 0; i < cleanItemList.Count; i++)
            {
                var baseItem = cleanItemList[i];
                baseItem.Retract(i);
                baseItem.OperState = TMatchBaseItem.OperStateType.Scene;
                EventDispatcher.Instance.DispatchEvent(new TMatchTripleEvent(baseItem.Id, 1));

            }

            TMatchCollectorSystem.Instance.SendItemChangeEvent();

            if (!await DelayCareDestory(2500)) return;
            UIViewSystem.Instance.Get<UITMatchMainController>().SetBroomEffectState(false);

            brooming = false;
        }

        private bool windmilling;

        public bool CanUseWindmill(ItemConfig item)
        {
            return !windmilling;
        }

        private async void UseWindmill(ItemConfig item)
        {
            // GameBIManager.Instance.LevelInfo.Windmill++;
            CrocodileActivityModel.Instance.UseBoost();
            windmilling = true;

            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Wind);
            UIViewSystem.Instance.Get<UITMatchMainController>().SetWindmillEffectState(true);

            //if (await DelayCareDestory(250)) return;

            foreach (var p in TMatchItemSystem.Instance.Items)
            {
                if (p.OperState == TMatchBaseItem.OperStateType.Scene)
                {
                    p.Shuffle();
                }
            }

            if (!await DelayCareDestory(1000)) return;
            UIViewSystem.Instance.Get<UITMatchMainController>().SetWindmillEffectState(false);

            windmilling = false;
        }

        private bool frozening;

        public bool CanUseFrozen(ItemConfig item)
        {
            return !frozening && !UIViewSystem.Instance.Get<UITMatchMainController>().IsFrozen();
        }

        private async void UseFrozen(ItemConfig item)
        {
            // GameBIManager.Instance.LevelInfo.Frozen++;
            CrocodileActivityModel.Instance.UseBoost();
            frozening = true;
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Freeze);
            UIViewSystem.Instance.Get<UITMatchMainController>()?.FrozenTime(item.effectValue1);
            if (!await DelayCareDestory((int) (item.effectValue1 * 1000.0f))) return;
            frozening = false;
        }

        private async void UseLighting(ItemConfig item)
        {
            // GameBIManager.Instance.LevelInfo.LightningBuff++;
            List<TMatchBaseItem> validItems =
                TMatchItemSystem.Instance.Items.FindAll(x =>
                    x.OperState == TMatchBaseItem.OperStateType.Scene && x.Cfg.boosterId == 0);
            HashSet<int> unTargetItemId = new HashSet<int>();
            for (int i = 0; i < validItems.Count; i++)
            {
                TMatchBaseItem baseItem = validItems[i];
                if (TMatchSystem.LevelController.LevelData.layoutCfg.targetItemId.ToList().Exists(x => x == baseItem.Id))
                    continue;
                unTargetItemId.Add(baseItem.Id);
            }

            Dictionary<int, List<TMatchBaseItem>> selectItems = new Dictionary<int, List<TMatchBaseItem>>();
            int groupIndex = 1;
            int times = (int) (validItems.Count / 3.0f);
            for (int i = 0; i < times; i++)
            {
                foreach (var p in unTargetItemId)
                {
                    List<TMatchBaseItem> tempList = validItems.FindAll(x => x.Id == p);
                    if (tempList.Count < 3) continue;
                    List<TMatchBaseItem> tempItems = new List<TMatchBaseItem>();
                    for (int j = 0; j < 3; j++)
                    {
                        tempList[j].OperState = TMatchBaseItem.OperStateType.Exploed;
                        tempItems.Add(tempList[j]);
                        validItems.Remove(tempList[j]);
                    }

                    selectItems.Add(groupIndex++, tempItems);
                    if (groupIndex > item.effectValue1) break;
                }

                if (groupIndex > item.effectValue1) break;
            }

            GameObject vfxPrefab =
                ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Particle/Prefabs/VFX_Light", addToCache: true);
            foreach (var items in selectItems)
            {
                foreach (var p in items.Value)
                {
                    AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Lighting);
                    GameObject vfxObj = GameObject.Instantiate(vfxPrefab, UIRoot.Instance.mRootCanvas.transform);
                    if (p != null)
                    {
                        var position = CameraManager.MainCamera.WorldToScreenPoint(p.GameObject.transform.position);
                        vfxObj.transform.position = CameraManager.UICamera.ScreenToWorldPoint(position);
                    }
                    else
                    {
                        vfxObj.transform.localPosition = new Vector3(Random.Range(-320.0f, 320.0f),
                            Random.Range(-200.0f, 400.0f), 0.0f);
                    }

                    if (!await DelayCareDestory(150)) return;
                    TMatchBaseItem baseItem = p;
                    if (baseItem != null)
                    {
                        baseItem.Lightinged(() => { TMatchItemSystem.Instance.CollectItem(baseItem); });
                    }

                    GameObject.Destroy(vfxObj, 0.5f);
                    if (!await DelayCareDestory(100)) return;
                }
            }
        }

        private async void UseClock(ItemConfig item)
        {
            AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Clock);
            // GameBIManager.Instance.LevelInfo.TimingBuff++;
            UIViewSystem.Instance.Get<UITMatchMainController>()?.AddTime(item.effectValue1);
        }
    }
}