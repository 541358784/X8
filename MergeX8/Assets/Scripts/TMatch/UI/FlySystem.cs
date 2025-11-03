using System;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace TMatch
{
    public class FlySystem : Manager<FlySystem>
    {
        public static string CommonFlyItem = "TMatch/Prefabs/FlyItem";
        public static string CommonNum = "TMatch/Prefabs/FlyNum";
        public static string CommonHintStars = "TMatch/Particle/Prefabs/VFX_Hint_Stars_001";
        public static string CommonTrail = "TMatch/Particle/Prefabs/VFX_Trail_0";

        private Transform root;

        public Transform Root
        {
            get
            {
                if (root == null) root = UIRoot.Instance.mRootCanvas.transform;
                return root;
            }
        }

        public void FlyItem(int itemId, int itemNum, Vector2 srcPos, Vector2 destPos, Action action, bool playCoinAudio = true)
        {
            DragonPlus.Config.TMatchShop.ItemConfig itemCfg = TMatchShopConfigManager.Instance.GetItem(itemId);
            int count = Math.Min(itemNum, 10);
            Vector3 localPos = Root.InverseTransformPoint(srcPos);
            localPos.y -= 20;
            // GameObject efObj = PlayNumEffect(localPos, itemNum);
            // if (efObj != null)
            // {
            //     efObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            // }

            if (itemCfg.GetItemType() == ItemType.TMCoin && playCoinAudio)
                AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Reward_fly);

            for (int i = 0; i < count; i++)
            {
                GameObject flyObj = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonFlyItem);
                flyObj.transform.parent = Root;
                flyObj.transform.Find("Icon").GetComponent<Image>().sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, itemCfg.pic_res);
                flyObj.transform.Find("InfiniteTag").gameObject.SetActive(itemCfg.GetItemType() == ItemType.TMEnergyInfinity);
                int index = i;
                FlyObject(flyObj, srcPos, destPos, true, 0.8f, 0.15f * i, () =>
                {
                    PlayHintStarsEffect(destPos);

                    if (index == 0)
                    {
                        if (itemCfg.GetResouceId() != 0) EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)itemCfg.GetResouceId()));
                        EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(itemCfg.id));
                    }

                    if (index == count - 1)
                    {
                        action?.Invoke();
                    }

                    if (itemId == (int)ItemType.TMCoin || itemId == (int)ItemType.TMEnergy || itemId == (int)ItemType.TMStar)
                    {
                        EventDispatcher.Instance.DispatchEvent(new CurrencyFlyAniEnd((ItemType)itemId));
                    }

                    if (itemId == (int)ItemType.TMEnergyInfinity && index == 0)
                    {
                        EventDispatcher.Instance.DispatchEvent(new CurrencyFlyAniEnd((ItemType)itemId));
                    }
                });
            }
        }

        public void FlyObject(GameObject flyObj, Vector3 srcPos, Vector3 destPos, bool showEffect, float time = 0.5f, float delayTime = 0f, Action action = null, float scale = 0.8f)
        {
            if (!flyObj) return;

            srcPos.z = 0;

            Vector3 startPos = srcPos;
            startPos.x += Random.Range(-0.5f, 0.5f);
            startPos.y += Random.Range(-0.2f, 0.2f);
            startPos.z = 0;

            destPos.z = 0;

            flyObj.transform.position = startPos;
            flyObj.transform.transform.localScale = new Vector3(0, 0, 0);
            flyObj.transform.localScale = Vector3.zero;

            GameObject efPrefab = null;
            if (showEffect)
            {
                efPrefab = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonTrail);
                if (efPrefab != null)
                {
                    efPrefab.gameObject.SetActive(false);
                    efPrefab.transform.SetParent(flyObj.transform);
                    efPrefab.transform.Reset();
                    efPrefab.gameObject.SetActive(true);
                }
            }

            Vector3 control = Vector3.zero;
            control.x = startPos.x + 0.3f;
            control.y = startPos.y - 0.3f;

            Vector3 control1 = Vector3.MoveTowards(control, destPos, 1);

            Sequence s = DOTween.Sequence();
            s.SetDelay(delayTime);
            s.Append(flyObj.transform.DOScale(new Vector3(scale, scale, scale), 0.3f));
            s.Append(flyObj.transform.DOPath(new[] { startPos, control, control1, destPos }, time, PathType.CatmullRom).SetEase(Ease.InQuart));
            s.OnComplete(() =>
            {
                if (efPrefab != null) TMatchModel.ObjectPoolMgr.RecycleGameObject(efPrefab);
                Destroy(flyObj);
                action?.Invoke();
            });
            s.Play();
        }

        public GameObject PlayNumEffect(Vector3 localPosition, int num)
        {
            GameObject efPrefab = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonNum);
            if (efPrefab == null) return null;
            efPrefab.gameObject.SetActive(false);
            efPrefab.transform.SetParent(Root);
            efPrefab.transform.Reset();
            efPrefab.gameObject.SetActive(true);

            LocalizeTextMeshProUGUI addText = efPrefab.transform.Find("Root/UIAdd").GetComponent<LocalizeTextMeshProUGUI>();
            if (addText != null)
            {
                addText.SetTerm(num.ToString());
                efPrefab.transform.localPosition = localPosition;
            }

            StartCoroutine(CommonUtils.DelayWork(2, () =>
            {
                if (efPrefab != null) TMatchModel.ObjectPoolMgr.RecycleGameObject(efPrefab);
            }));

            return efPrefab;
        }

        public void PlayHintStarsEffect(Vector2 position)
        {
            GameObject efPrefab = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonHintStars);
            if (efPrefab == null) return;
            efPrefab.gameObject.SetActive(false);
            efPrefab.transform.SetParent(Root);
            efPrefab.transform.position = position;
            efPrefab.transform.localScale = Vector3.one;
            efPrefab.gameObject.SetActive(true);

            StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
            {
                if (efPrefab != null) TMatchModel.ObjectPoolMgr.RecycleGameObject(efPrefab);
            }));
        }

        public Transform GetTargetTransform(int itemId)
        {
            DragonPlus.Config.TMatchShop.ItemConfig itemCfg = TMatchShopConfigManager.Instance.GetItem(itemId);
            Transform targetTransfrom;
            if (itemCfg.GetItemType() == ItemType.TMCoin) targetTransfrom = CoinNum.GetTopView();
            else if (itemCfg.GetItemType() == ItemType.TMStar) targetTransfrom = StarsNum.GetTopView();
            else if (itemCfg.GetItemType() == ItemType.TMEnergy || itemCfg.GetItemType() == ItemType.TMEnergyInfinity) targetTransfrom = LifeNumber.GetTopView();
            else targetTransfrom = UILobbyMainViewLevelButton.GetTopView();
            return targetTransfrom;
        }

        public Transform GetShopTargetTransform(int itemId)
        {
            DragonPlus.Config.TMatchShop.ItemConfig itemCfg = TMatchShopConfigManager.Instance.GetItem(itemId);
            Transform targetTransfrom = null;
            if (itemCfg.GetItemType() == ItemType.TMCoin) targetTransfrom = CoinNum.GetTopView();
            else if (itemCfg.GetItemType() == ItemType.TMStar) targetTransfrom = StarsNum.GetTopView();
            else if (itemCfg.GetItemType() == ItemType.TMEnergy || itemCfg.GetItemType() == ItemType.TMEnergyInfinity) targetTransfrom = LifeNumber.GetTopView();
            else if (itemCfg.GetItemType() == ItemType.TMMagnet) targetTransfrom = UITMatchMainController.GetMagentTopView();
            else if (itemCfg.GetItemType() == ItemType.TMBroom) targetTransfrom = UITMatchMainController.GetBroomTopView();
            else if (itemCfg.GetItemType() == ItemType.TMWindmill) targetTransfrom = UITMatchMainController.GetWindmillTopView();
            else if (itemCfg.GetItemType() == ItemType.TMFrozen) targetTransfrom = UITMatchMainController.GetFozenTopView();
            
            if (null == targetTransfrom) targetTransfrom = UILobbyView.CurrencyGroup;
            if (null == targetTransfrom) targetTransfrom = UILobbyNavigationBarView.GetMainTopView();
            if (null == targetTransfrom) targetTransfrom = UITMatchMainController.GetLevelTopView();
            return targetTransfrom;
        }
        
        public void FlyItem_Flutter(int itemId, int itemNum, Vector2 srcPos, Action action, Sprite iconSprite = null, float scale = 1f)
        {
            var itemCfg = TMatchShopConfigManager.Instance.GetItem(itemId);
            var localPos = Root.InverseTransformPoint(srcPos);
            localPos.y += 20;

            var flyObj = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonFlyItem);
            flyObj.transform.parent = Root;
            flyObj.transform.Find("InfiniteTag").gameObject.SetActive(itemCfg.GetItemType() == ItemType.TMEnergyInfinity);
            var text = flyObj.transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>();
            text.gameObject.SetActive(true);
            var textValue = itemCfg.infinity ? CommonUtils.FormatPropItemTime((long)(itemCfg.infiniityTime * 1000 * itemNum)) : itemNum.ToString();
            text.SetText(textValue);
            if (iconSprite == null)
            {
                flyObj.transform.Find("Icon").GetComponent<Image>().sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, itemCfg.pic_res);
            }
            else
            {
                flyObj.transform.Find("Icon").GetComponent<Image>().sprite = iconSprite;
                text.SetText("");
            }

            var finalPos = srcPos;
            finalPos.y += 2;
            FlyObject_Flutter(flyObj, srcPos, finalPos, true, 2f, 0.15f, () =>
            {
                if (itemCfg.GetResouceId() != 0) EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)itemCfg.GetResouceId()));
                EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(itemCfg.id));
                action?.Invoke();
                var itemType = itemCfg.GetItemType();
                if (itemType == ItemType.TMEnergyInfinity || itemType == ItemType.TMCoin || itemType == ItemType.TMEnergy || itemType == ItemType.TMStar)
                {
                    EventDispatcher.Instance.DispatchEvent(new CurrencyFlyAniEnd(itemType));
                }
            }, scale);
        }
        
        public void FlyObject_Flutter(GameObject flyObj, Vector3 srcPos, Vector3 destPos, bool showEffect, float time = 0.5f, float delayTime = 0f, Action action = null, float scale = 1f)
        {
            if (!flyObj) return;

            srcPos.z = 0;

            var startPos = srcPos;
            startPos.z = 0;
            destPos.z = 0;

            flyObj.transform.position = startPos;
            flyObj.transform.transform.localScale = new Vector3(0, 0, 0);
            flyObj.transform.localScale = Vector3.zero;

            GameObject efPrefab = null;
            if (showEffect)
            {
                efPrefab = TMatchModel.ObjectPoolMgr.SpawnGameObject(CommonTrail);
                if (efPrefab != null)
                {
                    efPrefab.gameObject.SetActive(false);
                    efPrefab.transform.SetParent(flyObj.transform);
                    efPrefab.transform.Reset();
                    efPrefab.gameObject.SetActive(true);
                }
            }

            var s = DOTween.Sequence();
            s.Append(flyObj.transform.DOScale(new Vector3(scale, scale, scale), 0.2f));
            s.Append(flyObj.transform.DOPath(new[] { startPos, destPos }, 1, PathType.Linear).SetEase(Ease.InQuart)
            );
            s.InsertCallback(0.8f, () =>
            {
                flyObj.transform.Find("Icon")?.GetComponent<Image>().DOFade(0, 0.39f);
                flyObj.transform.Find("NumberText")?.GetComponent<LocalizeTextMeshProUGUI>().DoFade(0, 0.39f);
            });
            s.OnComplete(() =>
            {
                if (efPrefab != null) TMatchModel.ObjectPoolMgr.RecycleGameObject(efPrefab);
                Destroy(flyObj);
                action?.Invoke();
            });
            s.Play();
        }
    }
}