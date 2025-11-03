using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UITMatchMainCollectItemView : UIView
    {
        protected override bool IsChildView => true;
        [ComponentBinder("PropItem")] private Transform propItem;

        public static Dictionary<int, int> collectItems;
        private Dictionary<int, Transform> collectObjs = new Dictionary<int, Transform>();

        public static Vector3 ItemPosition;

        private bool destory = false;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            collectItems = new Dictionary<int, int>();
            ItemPosition = propItem.position;
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_NEED_COLLECT_ITEM, OnNeedCollectItem);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST, OnTripleBoost);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST_HANDLE, OnTripleBoostHandle);
        }

        public override Task OnViewClose()
        {
            destory = true;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_NEED_COLLECT_ITEM, OnNeedCollectItem);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST, OnTripleBoost);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRIPLE_BOOST_HANDLE, OnTripleBoostHandle);
            return base.OnViewClose();
        }

        private void OnNeedCollectItem(BaseEvent evt)
        {
            TMatchNeedCollectItemEvent realEvt = evt as TMatchNeedCollectItemEvent;
            if (!collectItems.ContainsKey(realEvt.itemId)) collectItems.Add(realEvt.itemId, 0);
        }

        private async void OnTripleBoost(BaseEvent evt)
        {
            TMatchTripleBoostEvent realEvt = evt as TMatchTripleBoostEvent;
            if (!collectItems.ContainsKey(realEvt.id)) return;
            collectItems[realEvt.id] += 1;
            int cnt = collectItems[realEvt.id];

            bool appear = false;
            Transform tra;
            if (!collectObjs.TryGetValue(realEvt.id, out tra))
            {
                appear = true;
                tra = GameObject.Instantiate(propItem.gameObject, transform).transform;
                tra.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText("");
                tra.Find("Icon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                tra.gameObject.SetActive(true);
                tra.GetComponent<Animator>().enabled = false;
                tra.SetAsFirstSibling();
                collectObjs.Add(realEvt.id, tra);
                await Task.Yield();
                await Task.Yield();
            }

            UITMatchMainController mainController = UIViewSystem.Instance.Get<UITMatchMainController>();
            if (mainController == null) return;
            GameObject vfxPrefab = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Particle/Prefabs/vfx_Trail_CollectSpecial", addToCache: true);
            GameObject obj = GameObject.Instantiate(vfxPrefab, mainController.transform);
            var position = CameraManager.MainCamera.WorldToScreenPoint(TMatchEnvSystem.Instance.CollectorPos[realEvt.collectorPosIndex]);
            obj.transform.position = CameraManager.UICamera.ScreenToWorldPoint(position);

            Sequence s = DOTween.Sequence();
            s.Append(obj.transform.DOPath(new[] { obj.transform.position, tra.position }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad));
            s.OnComplete(() =>
            {
                if (!destory)
                {
                    if (!appear)
                    {
                        tra.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(cnt.ToString());
                        tra.gameObject.SetActive(false);
                        tra.gameObject.SetActive(true);
                        tra.GetComponent<Animator>().Play("add");
                    }
                    else
                    {
                        Item matchItemCfg = TMatchConfigManager.Instance.GetItem(realEvt.id);
                        DragonPlus.Config.TMatchShop.ItemConfig gameItemCfg = TMatchShopConfigManager.Instance.GetItem(matchItemCfg.boosterId);
                        tra.GetComponent<Animator>().enabled = true;
                        tra.GetComponent<Animator>().Play("appear");
                        tra.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(cnt.ToString());
                        tra.Find("Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(gameItemCfg.id);
                        tra.Find("Icon").GetComponent<Image>().color = Color.white;
                    }
                }
            });
            s.Play();
        }

        private void OnTripleBoostHandle(BaseEvent evt)
        {
            TMatchTripleBoostHandleEvent realEvt = evt as TMatchTripleBoostHandleEvent;
            if (!collectItems.ContainsKey(realEvt.id)) return;
            collectItems[realEvt.id] += realEvt.cnt;
            int cnt = collectItems[realEvt.id];

            Transform tra;
            if (collectObjs.TryGetValue(realEvt.id, out tra))
            {
                tra.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(cnt.ToString());
                tra.gameObject.SetActive(false);
                tra.gameObject.SetActive(true);
                tra.GetComponent<Animator>().Play("add");
            }
            else
            {
                Item matchItemCfg = TMatchConfigManager.Instance.GetItem(realEvt.id);
                DragonPlus.Config.TMatchShop.ItemConfig gameItemCfg = TMatchShopConfigManager.Instance.GetItem(matchItemCfg.boosterId);
                tra = GameObject.Instantiate(propItem.gameObject, transform).transform;
                tra.gameObject.SetActive(true);
                tra.SetAsFirstSibling();
                tra.GetComponent<Animator>().Play("guildcollect");
                tra.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(cnt.ToString());
                tra.Find("Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(gameItemCfg.id);
                collectObjs.Add(realEvt.id, tra);
            }
        }
    }
}