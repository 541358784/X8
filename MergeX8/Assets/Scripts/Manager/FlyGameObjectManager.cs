using System;
using System.Collections;
using System.Collections.Generic;
using Activity.CollectStone.Model;
using Dynamic;
using Activity.LuckyGoldenEgg;
using Activity.SlotMachine.View;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using DG.Tweening;
using Ditch.Model;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Farm.Model;
using Framework;
using Gameplay;
using GamePool;
using Screw;
using ThemeDecorationLeaderBoard;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using AudioManager = DragonPlus.AudioManager;
using Random = UnityEngine.Random;

public class FlyGameObjectManager : Manager<FlyGameObjectManager>
{
    private Transform _EffectRoot;

    public Transform EffectRoot
    {
        get
        {
            if (_EffectRoot == null)
            {
                _EffectRoot = UIRoot.Instance.transform;
            }

            return _EffectRoot;
        }
    }

    public void PerformFly(int resId, int flyCount, Vector3 srcPos, Vector3 dstPos, Action callback = null)
    {
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
        UpdateMergeItemImage(clone,resId);
        clone.gameObject.SetActive(false);
        var target = clone.transform;
        if (flyCount > 10)
            flyCount = 10;
        float delayTime = 0.05f;
        for (var i = 0; i < flyCount; i++)
        {
            var index = i;
            if (target && !target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(true);
            }
            FlyObject(target.gameObject, srcPos, dstPos, true, 0.5f, delayTime * i, () =>
            {
                PlayHintStarsEffect(dstPos);
                ShakeManager.Instance.ShakeLight();
                if (index == 0)
                {
                    callback?.Invoke();
                }
            });
        }
        DestroyImmediate(clone);
    }
    private Dictionary<string, Action> resTextUpdateActions = new Dictionary<string, Action>();

    public void FlyCurrency(UICurrencyGroupController currencyController, UserData.ResourceId rewardId, int rewardNum,
        Vector2 srcPos, float time, bool showNum, bool showEffect, float delayTime = 0.3f,
        Action action = null)
    {
        if (currencyController == null)
        {
            action?.Invoke();
            return;
        }

        Transform target = currencyController.GetIconTransform(rewardId);
        if (rewardId == UserData.ResourceId.Mermaid)
            target = MergeTaskTipsController.Instance._mergeEaster.transform as RectTransform;
        if (rewardId == UserData.ResourceId.RecoverCoinStar)
            target = MergeTaskTipsController.Instance._mergeRecoverCoinStar.transform as RectTransform;
        if (rewardId == UserData.ResourceId.CardPackageFreeLevel1 ||
            rewardId == UserData.ResourceId.CardPackageFreeLevel2 ||
            rewardId == UserData.ResourceId.CardPackageFreeLevel3 ||
            rewardId == UserData.ResourceId.CardPackageFreeLevel4 ||
            rewardId == UserData.ResourceId.CardPackageFreeLevel5 ||
            rewardId == UserData.ResourceId.CardPackagePayLevel1 ||
            rewardId == UserData.ResourceId.CardPackagePayLevel2 ||
            rewardId == UserData.ResourceId.CardPackagePayLevel3 ||
            rewardId == UserData.ResourceId.CardPackagePayLevel4 ||
            rewardId == UserData.ResourceId.CardPackagePayLevel5 ||
            rewardId == UserData.ResourceId.CardPackagePangLevel1 ||
            rewardId == UserData.ResourceId.CardPackagePangLevel2 ||
            rewardId == UserData.ResourceId.CardPackagePangLevel3 ||
            rewardId == UserData.ResourceId.CardPackagePangLevel4 ||
            rewardId == UserData.ResourceId.CardPackagePangLevel5 ||
            rewardId == UserData.ResourceId.WildCard3 ||
            rewardId == UserData.ResourceId.WildCard4 ||
            rewardId == UserData.ResourceId.WildCard5 ||
            rewardId == UserData.ResourceId.AvatarEaster2024 ||
            rewardId == UserData.ResourceId.AvatarEaster2025 ||
            rewardId == UserData.ResourceId.AvatarSnakeLadder ||
            rewardId == UserData.ResourceId.AvatarMonopoly ||
            rewardId == UserData.ResourceId.AvatarDonut ||
            rewardId == UserData.ResourceId.AvatarCamping ||
            rewardId == UserData.ResourceId.AvatarMonopoly2 ||
            rewardId == UserData.ResourceId.AvatarEaster2024_2 ||
            rewardId == UserData.ResourceId.AvatarHalloween ||
            rewardId == UserData.ResourceId.AvatarSpaceDog ||
            rewardId == UserData.ResourceId.AvatarDonut2 ||
            rewardId == UserData.ResourceId.AvatarMonopoly3 ||
            rewardId == UserData.ResourceId.AvatarSnakeLadder3 ||
            rewardId == UserData.ResourceId.AvatarNvZhu ||
            rewardId == UserData.ResourceId.AvatarMonopoly4 ||
            rewardId == UserData.ResourceId.AvatarMonopoly5 ||
            rewardId == UserData.ResourceId.AvatarSnakeLadder4 ||
            rewardId == UserData.ResourceId.AvatarEaster2024_4 ||
            rewardId == UserData.ResourceId.Easter2024Egg ||
            rewardId == UserData.ResourceId.SnakeLadderTurntable||
            ExtraOrderRewardCouponModel.IsCouponId((int)rewardId)||
            rewardId == UserData.ResourceId.ThemeDecorationScore ||
            rewardId == UserData.ResourceId.SlotMachineScore ||
            rewardId == UserData.ResourceId.Turntable ||
            rewardId == UserData.ResourceId.MonopolyDice ||
            rewardId == UserData.ResourceId.KeepPetDogFrisbee ||
            rewardId == UserData.ResourceId.KeepPetDogDrumstick ||
            rewardId == UserData.ResourceId.KeepPetDogSteak ||
            rewardId == UserData.ResourceId.KeepPetDogHead ||
            rewardId == UserData.ResourceId.MixMasterCoffee||
            rewardId == UserData.ResourceId.MixMasterTea||
            rewardId == UserData.ResourceId.MixMasterMilk||
            rewardId == UserData.ResourceId.MixMasterLemonJuice||
            rewardId == UserData.ResourceId.MixMasterIceCream||
            rewardId == UserData.ResourceId.MixMasterCream||
            rewardId == UserData.ResourceId.MixMasterPearl||
            rewardId == UserData.ResourceId.MixMasterSugar||
            rewardId == UserData.ResourceId.MixMasterIce ||
            rewardId == UserData.ResourceId.MixMasterExtra1 ||
            rewardId == UserData.ResourceId.TurtlePangPackage ||
            rewardId == UserData.ResourceId.StarrySkyCompassRocket ||
            rewardId == UserData.ResourceId.BuyDiamondTicket1 ||
            rewardId == UserData.ResourceId.ZumaBall ||
            rewardId == UserData.ResourceId.ZumaBomb ||
            rewardId == UserData.ResourceId.ZumaLine ||
            rewardId == UserData.ResourceId.KapibalaReborn ||
            rewardId == UserData.ResourceId.KapibalaLife ||
            rewardId == UserData.ResourceId.KapiScrewReborn ||
            rewardId == UserData.ResourceId.KapiScrewLife ||
            rewardId == UserData.ResourceId.FishCultureScore ||
            rewardId == UserData.ResourceId.KapiTileReborn ||
            rewardId == UserData.ResourceId.KapiTileLife ||
            rewardId == UserData.ResourceId.PhotoAlbumScore ||
            rewardId == UserData.ResourceId.TeamLife ||
            rewardId == UserData.ResourceId.TeamCoin ||
            rewardId == UserData.ResourceId.PillowWheel ||
            rewardId == UserData.ResourceId.CatchFish ||
            BlindBoxModel.Instance.IsBlindBoxId((int)rewardId)|| 
         rewardId == UserData.ResourceId.Stone) 
        {
            GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
            UpdateMergeItemImage(clone,(int)rewardId);
            clone.gameObject.SetActive(false);
            target = clone.transform;
            XUtility.WaitSeconds(0.5f, () =>
            {
                Destroy(clone);
            });
        }
        int count = Math.Min(rewardNum, 10);

        if (showNum)
        {
            Vector3 localPos = EffectRoot.transform.InverseTransformPoint(srcPos);
            localPos.y -= 20;
            GameObject efObj = PlayNumEffect(localPos, rewardNum);
            if (efObj != null)
            {
                efObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            }
        }

        String resKey = Utils.GetTimeStamp().ToString();
        resTextUpdateActions.Clear();
        resTextUpdateActions.Add(resKey,
            () => { EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate, rewardId); });
        int resNum = UserData.Instance.GetRes(rewardId);
        for (int i = 0; i < count; i++)
        {
            int index = i;
            
            Vector3 position;
            if (rewardId == UserData.ResourceId.Mermaid)
                position = MergeTaskTipsController.Instance._mergeEaster.transform.position;
            else if (rewardId == UserData.ResourceId.RecoverCoinStar)
                position = MergeTaskTipsController.Instance._mergeRecoverCoinStar.transform.position;
            else if (rewardId == UserData.ResourceId.CardPackageFreeLevel1 ||
                     rewardId == UserData.ResourceId.CardPackageFreeLevel2 ||
                     rewardId == UserData.ResourceId.CardPackageFreeLevel3 ||
                     rewardId == UserData.ResourceId.CardPackageFreeLevel4 ||
                     rewardId == UserData.ResourceId.CardPackageFreeLevel5 ||
                     rewardId == UserData.ResourceId.CardPackagePayLevel1 ||
                     rewardId == UserData.ResourceId.CardPackagePayLevel2 ||
                     rewardId == UserData.ResourceId.CardPackagePayLevel3 ||
                     rewardId == UserData.ResourceId.CardPackagePayLevel4 ||
                     rewardId == UserData.ResourceId.CardPackagePayLevel5 ||
                     rewardId == UserData.ResourceId.CardPackagePangLevel1 ||
                     rewardId == UserData.ResourceId.CardPackagePangLevel2 ||
                     rewardId == UserData.ResourceId.CardPackagePangLevel3 ||
                     rewardId == UserData.ResourceId.CardPackagePangLevel4 ||
                     rewardId == UserData.ResourceId.CardPackagePangLevel5)
            {
                position = MergeTaskTipsController.Instance.MergeCardPackage.transform.position;
            }
            else if (rewardId == UserData.ResourceId.WildCard3 ||
                     rewardId == UserData.ResourceId.WildCard4 ||
                     rewardId == UserData.ResourceId.WildCard5)
            {
                position = MergeTaskTipsController.Instance.MergeCardCollection.transform.position;
            }
            else if (rewardId == UserData.ResourceId.AvatarEaster2024 ||
                     rewardId == UserData.ResourceId.AvatarEaster2025 ||
                     rewardId == UserData.ResourceId.AvatarNvZhu ||
                     rewardId == UserData.ResourceId.AvatarMonopoly4 ||
                     rewardId == UserData.ResourceId.AvatarMonopoly5 ||
                     rewardId == UserData.ResourceId.AvatarSnakeLadder4 ||
                     rewardId == UserData.ResourceId.AvatarEaster2024_4 ||
                     rewardId == UserData.ResourceId.AvatarSnakeLadder ||
                     rewardId == UserData.ResourceId.AvatarMonopoly ||
                     rewardId == UserData.ResourceId.AvatarDonut ||
                     rewardId == UserData.ResourceId.AvatarCamping ||
                     rewardId == UserData.ResourceId.AvatarMonopoly2 ||
                     rewardId == UserData.ResourceId.AvatarEaster2024_2 ||
                     rewardId == UserData.ResourceId.AvatarHalloween ||
                     rewardId == UserData.ResourceId.AvatarDonut2 ||
                     rewardId == UserData.ResourceId.AvatarMonopoly3 ||
                     rewardId == UserData.ResourceId.AvatarSnakeLadder3 ||
                     rewardId == UserData.ResourceId.AvatarSpaceDog)
            {
                position = MergeMainController.Instance.backTrans.position;
            }
            else if (rewardId == UserData.ResourceId.Easter2024Egg)
            {
                position = MergeTaskTipsController.Instance.MergeEaster2024.transform.position;
            }
            else if (rewardId == UserData.ResourceId.SnakeLadderTurntable)
            {
                position = MergeTaskTipsController.Instance.MergeSnakeLadder.transform.position;
            }
            else if (rewardId == UserData.ResourceId.ThemeDecorationScore)
            {
                var storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeThemeDecoration, DynamicEntry_Game_ThemeDecoration>();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.SlotMachineScore)
            {
                var storage = SlotMachineModel.Instance.CurStorage;
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSlotMachine, DynamicEntry_Game_SlotMachine>();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.Turntable)
            {
                var entrance = MergeTaskTipsController.Instance.MergeTurntableEntry;
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.MonopolyDice)
            {
                var entrance = MonopolyModel.Instance.GetFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KeepPetDogFrisbee)
            {
                var entrance = KeepPetModel.Instance.GetDogFrisbeeFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KeepPetDogDrumstick)
            {
                var entrance = KeepPetModel.Instance.GetDogDrumstickFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KeepPetDogSteak)
            {
                var entrance = KeepPetModel.Instance.GetDogSteakFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KeepPetDogHead)
            {
                var entrance = KeepPetModel.Instance.GetDogHeadFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.MixMasterCoffee|| 
                     rewardId == UserData.ResourceId.MixMasterTea|| 
                     rewardId == UserData.ResourceId.MixMasterMilk||
                     rewardId == UserData.ResourceId.MixMasterLemonJuice||
                     rewardId == UserData.ResourceId.MixMasterIceCream||
                     rewardId == UserData.ResourceId.MixMasterCream||
                     rewardId == UserData.ResourceId.MixMasterPearl||
                     rewardId == UserData.ResourceId.MixMasterSugar||
                     rewardId == UserData.ResourceId.MixMasterIce ||
                     rewardId == UserData.ResourceId.MixMasterExtra1)
            {
                var entrance = MixMasterModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (BlindBoxModel.Instance.IsBlindBoxId((int)rewardId))
            {
                var entrance = BlindBoxModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.TurtlePangPackage)
            {
                var entrance = TurtlePangModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.StarrySkyCompassRocket)
            {
                var entrance = StarrySkyCompassModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.ZumaBall||
                     rewardId == UserData.ResourceId.ZumaBomb||
                     rewardId == UserData.ResourceId.ZumaLine)
            {
                var entrance = ZumaModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KapibalaLife||
                     rewardId == UserData.ResourceId.KapibalaReborn)
            {
                var entrance = KapibalaModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KapiTileLife||
                     rewardId == UserData.ResourceId.KapiTileReborn)
            {
                var entrance = KapiTileModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.KapiScrewLife||
                     rewardId == UserData.ResourceId.KapiScrewReborn)
            {
                var entrance = KapiScrewModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.FishCultureScore)
            {
                var entrance = FishCultureModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.PhotoAlbumScore)
            {
                var entrance = PhotoAlbumModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.PillowWheel)
            {
                var entrance = PillowWheelModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.CatchFish)
            {
                var entrance = CatchFishModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (rewardId == UserData.ResourceId.BuyDiamondTicket1)
            {
                var entrance = StoreModel.Instance.GetCommonFlyTarget();
                position = entrance.transform.position;
            }
            else if (ExtraOrderRewardCouponModel.IsPayCouponId((int)rewardId))
            {
                position = MergeTaskTipsController.Instance.MergeExtraOrderRewardCoupon.transform.position;
            }
            else if (ExtraOrderRewardCouponModel.IsFreeCouponId((int)rewardId))
            {
                position = MergeTaskTipsController.Instance.MergeExtraOrderRewardCouponShowView.transform.position;
            }
            else
            {
                position = (rewardId != UserData.ResourceId.RareDecoCoin && rewardId != UserData.ResourceId.Stone)?target.position: MergeMainController.Instance.backTrans.position;
            }

            if (target && !target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(true);
            }
            FlyObject(target.gameObject, srcPos, position, showEffect, time, delayTime * i, () =>
            {
                CurrencyGroupManager.Instance.PlayShakeAnim(currencyController, rewardId);
                PlayHintStarsEffect(position);

                ShakeManager.Instance.ShakeLight();

                if (index == 0)
                {
                    CurrencyGroupManager.Instance.UpdateText(currencyController, rewardId, rewardNum, resNum,
                        time + (count - 4) * delayTime, () =>
                        {
                            if (!resTextUpdateActions.ContainsKey(resKey))
                                return;
                            resTextUpdateActions[resKey]?.Invoke();
                            resTextUpdateActions.Clear();
                        });
                }

                if (index == count - 1)
                {
                    action?.Invoke();
                }
            });
        }
    }

       public void FlyCurrency(int mergeId, int rewardNum, Vector2 srcPos, float time, bool showEffect, float delayTime = 0.3f,
        Action action = null)
    {
        if (rewardNum <= 0)
        {
            action?.Invoke();
            return; 
        }
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            action?.Invoke();
            return;
        }

        clone.transform.SetParent(EffectRoot);
        UpdateMergeItemImage(clone, mergeId);

        clone.gameObject.SetActive(true);
        clone.transform.position = srcPos;
        clone.transform.localScale = Vector3.one;
        
        Vector3 targetPosition = MergeMainController.Instance.rewardBtnTrans.position;
        if (UserData.Instance.IsResource(mergeId))
        {
            targetPosition = GetResourcePosition((UserData.ResourceId)mergeId);
        }

        int count = Math.Min(rewardNum, 10);

        String resKey = Utils.GetTimeStamp().ToString();
        for (int i = 0; i < count; i++)
        {
            int index = i;
            FlyObject(clone.gameObject, srcPos, targetPosition, showEffect, time, delayTime * i, () =>
            {
                PlayHintStarsEffect(targetPosition);

                if (index == 0)
                { 
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                }

                if (index == count - 1)
                { 
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                    action?.Invoke();
                }
            });
        }
        
        GameObject.Destroy(clone);
    }
       
    //飞task merge item
    public void FlyObject(int index, int mergeId, Vector3 srcPos, Transform target, float flyTime, Action flyEndCall)
    {
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        clone.transform.SetParent(EffectRoot);
        UpdateMergeItemImage(clone, mergeId);

        clone.gameObject.SetActive(true);
        clone.transform.position = srcPos;
        clone.transform.localScale = Vector3.one;

        Vector3 targetPos = target.transform.position;
        Vector3 startPos = clone.transform.position;

        int curX = index % MergeManager.Instance.GetBoardWidth((MergeBoardEnum)MergeManager.Instance.MergeBoardID1);

        Vector2 gridSize =MergeMainController.Instance.MergeBoard.GridSize;
        gridSize.x = gridSize.x / 100f / 2f;
        gridSize.y = gridSize.y / 100f / 2f;

        if (curX == MergeManager.Instance.GetBoardWidth((MergeBoardEnum)MergeManager.Instance.MergeBoardID1) - 1)
            gridSize.x /= 4f;

        Vector3 control1 = startPos;
        control1.x = startPos.x + gridSize.x;
        control1.y = startPos.y + gridSize.y;

        Vector3 control2 = Vector3.MoveTowards(control1, targetPos, 1f);
        clone.transform.localScale = new Vector3(0.8f, 0.8f, 1);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(clone.transform.DOScale(new Vector3(1.5f, 1.5f, 1), flyTime / 2f).SetEase(Ease.Linear));
        sequence.Append(
            clone.transform.DOScale(new Vector3(0.9f, 0.9f, 1), flyTime - flyTime / 2f).SetEase(Ease.Linear));
        sequence.Play();

        clone.transform.DOPath(new[] {startPos, control1, control2, targetPos}, flyTime, PathType.CatmullRom)
            .SetEase(Ease.InCubic).OnComplete(() =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
                flyEndCall?.Invoke();
            });
    }

    //抛物线运动
    public void FlyObject(int mergeId, Vector3 oldPos, Vector3 newPos, float speed, Action flyEndCall)
    {
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        oldPos = EffectRoot.InverseTransformPoint(oldPos);
        newPos = EffectRoot.InverseTransformPoint(newPos);

        clone.transform.SetParent(EffectRoot);
        Image mergeImage = UpdateMergeItemImage(clone, mergeId);

        Vector3[] path = new Vector3[3];
        float dis = Vector3.Distance(oldPos, newPos);

        float time = 0.5f;
        float height = 80f;
        float distance = 60f;
        float moveTime = 0.3f;
        float gridWidth = 80;
        float scaleAdapt = MergeMainController.Instance.RootScale;

        float maxSpace = 10;
        float minSpace = 5;

        float offset = dis / gridWidth + 3;
        offset = Mathf.Clamp(offset, minSpace, maxSpace);
        float moveDis = distance / (maxSpace - offset + 1);
        float rate = 1.0f * offset / maxSpace;

        Vector3 tempPos = newPos - (newPos - oldPos).normalized * moveDis;
        path[0] = oldPos;
        path[1] = (oldPos + tempPos) * 0.5f + (Vector3.up * height);
        path[2] = tempPos;
        clone.transform.localPosition = oldPos;
        clone.transform.localScale = Vector3.zero;
        clone.transform.SetAsLastSibling();

        GameObject obj_smoke = clone.transform.Find("vfx_Smoke").gameObject;
        obj_smoke.gameObject.SetActive(false);
        var sq = DOTween.Sequence();
        sq.Insert(0, clone.transform.DOLocalPath(path, time, PathType.CatmullRom));
        sq.Insert(0, clone.transform.DOScale(1.6f * scaleAdapt, time * 0.5f));
        sq.Insert(time * 0.5f,
            clone.transform.DOScale(new Vector3(1.1f, 1f, 1f) * scaleAdapt, time * 0.5f)
                .OnComplete(() => { obj_smoke.gameObject.SetActive(true); }));
        sq.Insert(time, clone.transform.DOLocalMove(newPos, moveTime * rate).SetEase(Ease.OutQuad));
        sq.Insert(time,
            clone.transform.DOScale(new Vector3(1f, 1f, 1f) * scaleAdapt, moveTime * rate).SetEase(Ease.OutQuad));
        sq.SetEase(Ease.InQuad);
        sq.OnComplete(() =>
        {
            mergeImage?.gameObject.SetActive(false);
            StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
            {
                mergeImage?.gameObject.SetActive(true);
                obj_smoke.gameObject.SetActive(false);
                clone.transform.localScale = Vector3.one;
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
            }));
            flyEndCall?.Invoke();
        });


        // Vector3[] path = new Vector3[3];
        //
        // float dis = Vector3.Distance(srcPos, destPos);
        // float offset = dis / speed;
        // float time = Mathf.Clamp(offset , 0.5f, 0.85f);
        // float height = dis > 4 ? 0.8f : 0.4f;
        // path[0] = srcPos;
        // path[1] =  srcPos + (destPos - srcPos).normalized * 0.85f * dis +(Vector3.up * height);
        // path[2] = destPos ;
        //
        // clone.gameObject.SetActive(true);
        // clone.transform.position = srcPos;
        // clone.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        //
        // var sq = DOTween.Sequence();
        // sq.Insert(0, clone.transform.DOPath(path, time,PathType.CatmullRom));
        // sq.Insert(0, clone.transform.DOScale(1.8f, time * 0.75f));
        // sq.Insert(time * 0.75f, clone.transform.DOScale(1f, time * 0.25f));
        // sq.SetEase(Ease.Linear);
        // sq.OnComplete(() =>
        //  {
        //      GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
        //     flyEndCall?.Invoke();
        //  });
        // sq.Play();

        //方法2
        // clone.gameObject.SetActive(true);
        // clone.transform.position = startPosition;
        // clone.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        // float length = Vector3.Distance(startPosition, targetPosition);
        // float flyTime = length / speed;
        // flyTime = Mathf.Max(flyTime, 0.6f);
        // flyTime = Mathf.Min(flyTime, 0.8f);
        //
        // Sequence sequence = DOTween.Sequence();
        // sequence.Append(clone.transform.DOScale(new Vector3(1.8f,1.8f,1), flyTime*4/5).SetEase(Ease.Linear));
        // sequence.Append(clone.transform.DOScale(new Vector3(1f,1f,1), flyTime -flyTime*4/5).SetEase(Ease.Linear));
        // sequence.Play();
        //
        // clone.transform.DOMove(targetPosition, flyTime).SetEase(Ease.Linear).OnComplete(() =>
        // {
        //     GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
        //     flyEndCall?.Invoke();
        // });
    }

    public void FlyObject(int mergeId, Vector3 srcPos, Vector3 destPos, float time, float srcScale = 1f,
        float destScale = 0.5f, Action flyEndCall = null)
    {
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        clone.transform.SetParent(EffectRoot);
        UpdateMergeItemImage(clone, mergeId);

        clone.gameObject.SetActive(true);
        clone.transform.position = srcPos;
        clone.transform.localScale = new Vector3(srcScale, srcScale, srcScale);

        var sq = DOTween.Sequence();
        sq.Insert(0, clone.transform.DOMove(destPos, time).SetEase(Ease.InCubic));
        sq.Insert(0, clone.transform.DOScale(destScale, time));
        sq.OnComplete(() =>
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
            flyEndCall?.Invoke();
        });
        sq.Play();
    }

    public void FlyObject(int mergeId, Vector3 srcPos, Transform target, float delay, float time,
        Action flyEndCall = null, bool showEffect = false, float srcScale = 1f, float destScale = 0.5f,
        bool showBgEffect = false)
    {
        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        clone.transform.SetParent(EffectRoot);
        UpdateMergeItemImage(clone, mergeId);

        GameObject efPrefab = null;
        if (showEffect)
            efPrefab = SpawnGameObject(clone.transform, ObjectPoolName.CommonTrail);


        GameObject efBgPrefab = null;
        if (showBgEffect)
        {
            efBgPrefab = SpawnGameObject(clone.transform, ObjectPoolName.CommonBgEffect);
            if (efBgPrefab != null)
                efBgPrefab.transform.SetAsFirstSibling();
        }

        clone.gameObject.SetActive(true);
        clone.transform.position = srcPos;
        clone.transform.localScale = new Vector3(srcScale, srcScale, srcScale);

        var sq = DOTween.Sequence();
        sq.Insert(0, clone.transform.DOMove(target.position, time).SetEase(Ease.InCubic));
        sq.Insert(0, clone.transform.DOScale(destScale, time));
        sq.SetDelay(delay);
        sq.OnComplete(() =>
        {
            if (efPrefab != null)
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, efPrefab);

            if (efBgPrefab != null)
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonBgEffect, efBgPrefab);

            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone.gameObject);
            flyEndCall?.Invoke();
        });
        sq.Play();
    }

    public void FlyObjectUpStraight(GameObject target, Vector3 srcPos, Vector3 destPos, bool showEffect, float flyTime = 0.5f,float uptime = 0.5f,
        float delayTime = 0f, Action action = null,float controlX = 0f,float controlY = 0.6f,float scale=1f)
    {
        if (!target)
            return;

        srcPos.z = 0;

        Vector3 startPos = srcPos;
        startPos.z = 0;

        destPos.z = 0;

        GameObject clone = Instantiate(target, EffectRoot);
        clone.transform.position = startPos;
        clone.transform.localScale = new Vector3(scale,scale,scale);
        GameObject efPrefab = null;
        if (showEffect)
        {
            efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonTrail);
            if (efPrefab != null)
            {
                efPrefab.gameObject.SetActive(false);
                efPrefab.transform.SetParent(clone.transform);
                efPrefab.transform.Reset();
                efPrefab.gameObject.SetActive(true);
            }
        }

        Vector3 control = Vector3.zero;
        control.x = startPos.x + controlX;
        control.y = startPos.y + controlY;

        Vector3 control1 = Vector3.MoveTowards(control, destPos, 1);

        Sequence s = DOTween.Sequence();
        s.SetDelay(delayTime);
        s.Append(clone.transform.DOPath(new[] {startPos,control}, uptime, PathType.Linear).SetEase(Ease.Linear));
        s.Append(clone.transform.DOPath(new[] {control,control1,destPos}, flyTime, PathType.CatmullRom).SetEase(Ease.InQuart));
        s.Append(clone.transform.DOScale(1f,flyTime-0.2f));
        s.OnComplete(() =>
        {
            if (efPrefab != null)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, efPrefab);
            }

            Destroy(clone);
            action?.Invoke();
        });
        s.Play();
    }
    
    public void FlyObject(GameObject target, Vector3 srcPos, Vector3 destPos, bool showEffect, float time = 0.5f,
        float delayTime = 0f, Action action = null, float scale = 0.8f,float controlX = 0.3f,float controlY = -0.3f)
    {
        if (!target)
            return;

        srcPos.z = 0;

        Vector3 startPos = srcPos;
        startPos.x += Random.Range(-0.5f, 0.5f);
        startPos.y += Random.Range(-0.2f, 0.2f);
        startPos.z = 0;

        destPos.z = 0;

        GameObject clone = Instantiate(target, EffectRoot);
        clone.gameObject.SetActive(true);
        clone.transform.position = startPos;
        clone.transform.transform.localScale = new Vector3(0, 0, 0);
        clone.transform.localScale = Vector3.zero;

        GameObject efPrefab = null;
        if (showEffect)
        {
            efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonTrail);
            if (efPrefab != null)
            {
                efPrefab.gameObject.SetActive(false);
                efPrefab.transform.SetParent(clone.transform);
                efPrefab.transform.Reset();
                efPrefab.gameObject.SetActive(true);
            }
        }

        Vector3 control = Vector3.zero;
        control.x = startPos.x + controlX;
        control.y = startPos.y + controlY;

        Vector3 control1 = Vector3.MoveTowards(control, destPos, 1);

        Sequence s = DOTween.Sequence();
        s.SetDelay(delayTime);
        s.Append(clone.transform.DOScale(new Vector3(scale, scale, scale), 0.3f));
        s.Append(clone.transform.DOPath(new[] {startPos, control, control1, destPos}, time, PathType.CatmullRom)
            .SetEase(Ease.InQuart));
        s.OnComplete(() =>
        {
            if (efPrefab != null)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, efPrefab);
            }

            Destroy(clone);
            action?.Invoke();
        });
        s.Play();
    }

    public void FlyObject(GameObject target, Vector3 srcPos, Vector3 controlPos, Vector3 destPos, bool showEffect,
        float time = 0.5f,
        float delayTime = 0f, Action action = null, float scale = 0.8f)
    {
        if (!target)
            return;

        srcPos.z = 0;
        destPos.z = 0;
        controlPos.z = 0;

        GameObject clone = Instantiate(target, EffectRoot);
        clone.transform.position = srcPos;
        clone.transform.transform.localScale = new Vector3(0, 0, 0);
        clone.transform.localScale = Vector3.zero;

        GameObject efPrefab = null;
        if (showEffect)
        {
            efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonTrail);
            if (efPrefab != null)
            {
                efPrefab.gameObject.SetActive(false);
                efPrefab.transform.SetParent(clone.transform);
                efPrefab.transform.Reset();
                efPrefab.gameObject.SetActive(true);
            }
        }

        Vector3 control = controlPos;
        Vector3 control1 = Vector3.MoveTowards(control, destPos, 1);

        Sequence s = DOTween.Sequence();
        s.SetDelay(delayTime);
        s.Append(clone.transform.DOScale(new Vector3(scale, scale, scale), 0.3f));
        s.Append(clone.transform.DOPath(new[] {srcPos, control, control1, destPos}, time, PathType.CatmullRom)
            .SetEase(Ease.InQuart));
        s.OnComplete(() =>
        {
            if (efPrefab != null)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, efPrefab);
            }

            Destroy(clone);
            action?.Invoke();
        });
        s.Play();
    }

    public void FlyObject(List<RewardData> rewardDatas, UICurrencyGroupController resController,
        Action flyEndCall = null,bool ignoreEventSystem = false,bool needHide = true)
    {
        StartCoroutine(RewardFlyLogic(rewardDatas, resController, flyEndCall,ignoreEventSystem,needHide));
    }

    public void FlyObject(List<ResData> resData, Action flyEndCall = null)
    {
        if (resData == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        Dictionary<int, int> dicResData = new Dictionary<int, int>();

        foreach (var kv in resData)
        {
            if (dicResData.ContainsKey((int) kv.id))
                dicResData[(int) kv.id] += kv.count;
            else
                dicResData.Add((int) kv.id, kv.count);
        }

        StartCoroutine(IEn_FlyObjects(dicResData, flyEndCall));
    }

    private int[] animShowRes =
    {
        (int) UserData.ResourceId.Exp, (int) UserData.ResourceId.Energy, (int) UserData.ResourceId.Coin,
        (int) UserData.ResourceId.Coin, (int) UserData.ResourceId.RareDecoCoin
    };

    private IEnumerator IEn_FlyObjects(Dictionary<int, int> resData, Action flyEndCall = null)
    {
        if (resData == null || resData.Count == 0)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        bool isEnableEventSystem = UIRoot.Instance.EnableEventSystem;
        UIRoot.Instance.EnableEventSystem = false;

        List<ResData> itemDatas = new List<ResData>();
        for (int i = 0; i < animShowRes.Length; i++)
        {
            if (!resData.ContainsKey((int) animShowRes[i]))
                continue;

            int num = resData[(int) animShowRes[i]];
            if (num == 0)
                continue;

            ResData res = new ResData(animShowRes[i], num);
            itemDatas.Add(res);
            resData.Remove((int) animShowRes[i]);
        }

        if (itemDatas != null && itemDatas.Count > 0)
        {
            yield return CoroutineManager.Instance.StartCoroutine(InternalLogic(itemDatas));
            yield return new WaitForSeconds(0.1f);
            itemDatas.Clear();
        }

        foreach (var kv in resData)
        {
            ResData res = new ResData(kv.Key, kv.Value);
            itemDatas.Add(res);
        }

        yield return CoroutineManager.Instance.StartCoroutine(InternalLogic(itemDatas, 0.2f));

        yield return new WaitForSeconds(0.2f);

        UIRoot.Instance.EnableEventSystem = isEnableEventSystem;
        flyEndCall?.Invoke();
    }

    private IEnumerator InternalLogic(List<ResData> resDatas, float timeStep = 0, Action flyEndCall = null)
    {
        if (resDatas == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        for (int i = 0; i < resDatas.Count; i++)
        {
            ResData resData = resDatas[i];

            bool isRes = UserData.Instance.IsResource(resData.id);

            Transform iconTransform = null;
            if (isRes)
            {
                iconTransform = CurrencyGroupManager.Instance.GetIconTransform((UserData.ResourceId) resData.id);

                if (iconTransform == null)
                    continue;
            }

            Coroutine coroutine = null;
            if (isRes)
                coroutine = StartCoroutine(ResFlyLogic(iconTransform.gameObject, resData, true, false));
            else
                coroutine = StartCoroutine(ItemFlyLogic(resData, null));

            if (timeStep > 0)
            {
                if (i == resDatas.Count - 1)
                {
                    yield return coroutine;
                }
                else
                {
                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                yield return coroutine;
            }
        }

        yield return new WaitForSeconds(0.1f);

        flyEndCall?.Invoke();
    }
    public Vector3 GetResourcePosition(UserData.ResourceId resourceId,UICurrencyGroupController resController = null)
    {
        if (resourceId == UserData.ResourceId.Easter2024Egg)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeEaster2024)
                    targetTransform = MergeTaskTipsController.Instance.MergeEaster2024.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_Easter2024.Instance != null && Aux_Easter2024.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_Easter2024.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.SnakeLadderTurntable)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeSnakeLadder)
                    targetTransform = MergeTaskTipsController.Instance.MergeSnakeLadder.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_SnakeLadder.Instance != null && Aux_SnakeLadder.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_SnakeLadder.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.ThemeDecorationScore)
        {
            Transform targetTransform = null;
            var storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeThemeDecoration, DynamicEntry_Game_ThemeDecoration>();
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_ThemeDecoration>();
                if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                {
                    targetTransform = auxItem.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.SlotMachineScore)
        {
            Transform targetTransform = null;
            var storage = SlotMachineModel.Instance.CurStorage;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSlotMachine, DynamicEntry_Game_SlotMachine>();
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_SlotMachine>();
                if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                {
                    targetTransform = auxItem.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.Turntable)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = MergeTaskTipsController.Instance.MergeTurntableEntry;
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_Turntable.Instance != null && Aux_Turntable.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_Turntable.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.GardenShovel || resourceId == UserData.ResourceId.GardenBomb)
        {
            Transform targetTransform = null;
            var window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGardenTreasureMain);
            if (window != null)
            {
                if (resourceId == UserData.ResourceId.GardenShovel)
                    targetTransform = ((UIGardenTreasureMainController)window).ShovelTransform;
                else
                    targetTransform = ((UIGardenTreasureMainController)window).BombTransform;
            }
            else
            {
                if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
                {
                    var entrance = MergeTaskTipsController.Instance.MergeGardenTreasureEntry;
                    if (entrance)
                        targetTransform = entrance.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
                else
                {
                    if (Aux_GardenTreasure.Instance != null && Aux_GardenTreasure.Instance.gameObject.activeInHierarchy)
                    {
                        targetTransform = Aux_GardenTreasure.Instance.transform;
                    }
                    else
                    {
                        targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                    }
                }
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.Hammer)
        {
            var tra = TreasureHuntModel.Instance.GetFlyTarget();
            if (tra != null)
                return tra.position;
            return Vector3.zero;
        }   
        else if (resourceId == UserData.ResourceId.GoldenEgg)
        {
            var tra = LuckyGoldenEggModel.Instance.GetFlyTarget();
            if (tra != null)
                return tra.position;
            return Vector3.zero;
        } 
        else if (resourceId == UserData.ResourceId.MonopolyDice)
        {
            return MonopolyModel.Instance.GetFlyTarget().position;
        }    
        else if ((int)resourceId == 2205104)
        {
            var target = ButterflyWorkShopModel.Instance.GetFlyTarget();
            if (target == null)
                target = UIHomeMainController.mainController.MainPlayTransform;
            return target.position;
        }
        else if (resourceId == UserData.ResourceId.KeepPetDogFrisbee)
        {
            return KeepPetModel.Instance.GetDogFrisbeeFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KeepPetDogDrumstick)
        {
            return KeepPetModel.Instance.GetDogDrumstickFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KeepPetDogSteak)
        {
            return KeepPetModel.Instance.GetDogSteakFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KeepPetDogHead)
        {
            return KeepPetModel.Instance.GetDogHeadFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.MixMasterCoffee|| 
                 resourceId == UserData.ResourceId.MixMasterTea|| 
                 resourceId == UserData.ResourceId.MixMasterMilk||
                 resourceId == UserData.ResourceId.MixMasterLemonJuice||
                 resourceId == UserData.ResourceId.MixMasterIceCream||
                 resourceId == UserData.ResourceId.MixMasterCream||
                 resourceId == UserData.ResourceId.MixMasterPearl||
                 resourceId == UserData.ResourceId.MixMasterSugar||
                 resourceId == UserData.ResourceId.MixMasterIce ||
                 resourceId == UserData.ResourceId.MixMasterExtra1)
        {
            var entrance = MixMasterModel.Instance.GetCommonFlyTarget();
            return entrance.transform.position;
        }
        else if (BlindBoxModel.Instance.IsBlindBoxId((int)resourceId))
        {
            var entrance = BlindBoxModel.Instance.GetCommonFlyTarget();
            return entrance.transform.position;
        }
        else if (resourceId == UserData.ResourceId.TurtlePangPackage)
        {
            return TurtlePangModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.StarrySkyCompassRocket)
        {
            return StarrySkyCompassModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.ZumaBall||
                 resourceId == UserData.ResourceId.ZumaBomb||
                 resourceId == UserData.ResourceId.ZumaLine)
        {
            return ZumaModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KapibalaLife||
                 resourceId == UserData.ResourceId.KapibalaReborn)
        {
            return KapibalaModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KapiTileLife||
                 resourceId == UserData.ResourceId.KapiTileReborn)
        {
            return KapiTileModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.KapiScrewLife||
                 resourceId == UserData.ResourceId.KapiScrewReborn)
        {
            return KapiScrewModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.FishCultureScore)
        {
            return FishCultureModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.PhotoAlbumScore)
        {
            return PhotoAlbumModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.PillowWheel)
        {
            return PillowWheelModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.CatchFish)
        {
            return CatchFishModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.JungleAdventure)
        {
            return JungleAdventureModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.Stone)
        {
            return CollectStoneModel.Instance.GetCommonFlyTarget().position;
        }
        else if (resourceId == UserData.ResourceId.BuyDiamondTicket1)
        {
            return StoreModel.Instance.GetCommonFlyTarget().position;
        }
        else if (ExtraOrderRewardCouponModel.IsCouponId((int)resourceId))
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (ExtraOrderRewardCouponModel.IsPayCouponId((int)resourceId))
                {
                    if (MergeTaskTipsController.Instance.MergeExtraOrderRewardCoupon)
                        targetTransform = MergeTaskTipsController.Instance.MergeExtraOrderRewardCoupon.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
                else if (ExtraOrderRewardCouponModel.IsFreeCouponId((int)resourceId))
                {
                    if (MergeTaskTipsController.Instance.MergeExtraOrderRewardCouponShowView)
                        targetTransform = MergeTaskTipsController.Instance.MergeExtraOrderRewardCouponShowView.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
            }
            else
            {
                targetTransform = UIHomeMainController.mainController.MainPlayTransform;
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.AvatarEaster2024 ||
                 resourceId == UserData.ResourceId.AvatarEaster2025 ||
                 resourceId == UserData.ResourceId.AvatarNvZhu ||
                 resourceId == UserData.ResourceId.AvatarMonopoly4 ||
                 resourceId == UserData.ResourceId.AvatarMonopoly5 ||
                 resourceId == UserData.ResourceId.AvatarSnakeLadder4 ||
                 resourceId == UserData.ResourceId.AvatarEaster2024_4 ||
                 resourceId == UserData.ResourceId.AvatarSnakeLadder ||
                 resourceId == UserData.ResourceId.AvatarMonopoly ||
                 resourceId == UserData.ResourceId.AvatarDonut ||
                 resourceId == UserData.ResourceId.AvatarCamping ||
                 resourceId == UserData.ResourceId.AvatarMonopoly2 ||
                 resourceId == UserData.ResourceId.AvatarHalloween ||
                 resourceId == UserData.ResourceId.AvatarSpaceDog ||
                 resourceId == UserData.ResourceId.AvatarDonut2 ||
                 resourceId == UserData.ResourceId.AvatarMonopoly3 ||
                 resourceId == UserData.ResourceId.AvatarSnakeLadder3 ||
                 resourceId == UserData.ResourceId.AvatarEaster2024_2)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                targetTransform = MergeMainController.Instance.backTrans;
            }
            else
            {
                var topUI =
                    UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Top) as MainAux_TopController;
                targetTransform = topUI._setButton.transform;
            }
            return targetTransform.position;
        }
        else if (resourceId == UserData.ResourceId.CardPackageFreeLevel1 ||
                 resourceId == UserData.ResourceId.CardPackageFreeLevel2 ||
                 resourceId == UserData.ResourceId.CardPackageFreeLevel3 ||
                 resourceId == UserData.ResourceId.CardPackageFreeLevel4 ||
                 resourceId == UserData.ResourceId.CardPackageFreeLevel5 ||
                 resourceId == UserData.ResourceId.CardPackagePayLevel1 ||
                 resourceId == UserData.ResourceId.CardPackagePayLevel2 ||
                 resourceId == UserData.ResourceId.CardPackagePayLevel3 ||
                 resourceId == UserData.ResourceId.CardPackagePayLevel4 ||
                 resourceId == UserData.ResourceId.CardPackagePayLevel5 ||
                 resourceId == UserData.ResourceId.CardPackagePangLevel1 ||
                 resourceId == UserData.ResourceId.CardPackagePangLevel2 ||
                 resourceId == UserData.ResourceId.CardPackagePangLevel3 ||
                 resourceId == UserData.ResourceId.CardPackagePangLevel4 ||
                 resourceId == UserData.ResourceId.CardPackagePangLevel5)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeCardPackage)
                    targetTransform = MergeTaskTipsController.Instance.MergeCardPackage.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                targetTransform = UIHomeMainController.mainController.MainPlayTransform;
            }
            return targetTransform.position;
        }
        else if (resourceId ==  UserData.ResourceId.WildCard3 ||
                 resourceId ==  UserData.ResourceId.WildCard4 ||
                 resourceId ==  UserData.ResourceId.WildCard5)
        {
            Transform targetTransform = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeCardCollection)
                    targetTransform = MergeTaskTipsController.Instance.MergeCardCollection.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_CardCollectionTheme.Instance != null && Aux_CardCollectionTheme.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_CardCollectionTheme.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
            return targetTransform.position;
        }
        else if (resController != null)
        {
            return resController.GetResourcePosition(resourceId);
        }
        else
        {
            return CurrencyGroupManager.Instance.GetResourcePosition(resourceId);
        }
    }
    public IEnumerator ResFlyLogic(GameObject clone, ResData resData, bool showNum, bool showEffect,
        Action flyEndCall = null)
    {
        if (showNum)
            PlayNumEffect(new Vector3(210, -240, 0), resData.count);

        bool isAnimEnd = false;
        List<GameObject> cloneGameObjects = new List<GameObject>();
        Dictionary<GameObject, GameObject> cloneEffectObjects = new Dictionary<GameObject, GameObject>();
        int startShowCount = Math.Min(resData.count, 10);
        for (int i = 0; i < startShowCount; i++)
        {
            int index = i;
            GameObject cloneObj = GameObject.Instantiate(clone, EffectRoot);

            cloneObj.gameObject.SetActive(true);
            cloneObj.transform.Reset();

            if (showEffect)
            {
                GameObject efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonTrail);
                if (efPrefab != null)
                {
                    efPrefab.gameObject.SetActive(false);
                    efPrefab.transform.SetParent(cloneObj.transform);
                    efPrefab.transform.Reset();
                    efPrefab.gameObject.SetActive(true);
                    cloneEffectObjects.Add(cloneObj, efPrefab);
                }
            }

            cloneObj.gameObject.transform.localPosition = RandomFlyPos();
            cloneObj.transform.localScale = Vector3.zero;
            cloneGameObjects.Add(cloneObj);

            Image image = cloneObj.GetComponent<Image>();
            if (image != null)
                image.color = new Color(1, 1, 1, 0.4f);

            float fadeTime = 0.03f;
            float scaleTime = 0.08f;

            Vector3 control = Vector3.zero;
            Vector3 targetPos = GetResourcePosition((UserData.ResourceId) resData.id);
            Vector3 srcPos = cloneObj.transform.position;
            Vector3 vDis = (targetPos - srcPos);
            control.x = srcPos.x + Random.Range(vDis.x / 3, vDis.x / 2);
            control.y = srcPos.y - Random.Range(0.1f, 0.5f);


            int resNum = UserData.Instance.GetRes((UserData.ResourceId) resData.id);

            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(index * 0.08f);
            sequence.Append(cloneObj.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.Linear));
            if (image != null)
                sequence.Insert(index * scaleTime, image.DOFade(1, fadeTime));
            sequence.Insert(index * scaleTime,
                cloneObj.transform.DOPath(new Vector3[] {srcPos, control, targetPos}, 1.5f, PathType.CatmullRom)
                    .SetEase(Ease.InQuart));
            sequence.OnComplete(() =>
            {
                ShakeManager.Instance.ShakeLight();

                if (index == cloneGameObjects.Count - 1)
                {
                    isAnimEnd = true;

                    if ((UserData.ResourceId) resData.id == UserData.ResourceId.Exp)
                    {
                        CurrencyGroupManager.Instance.TriggerGuide();
                    }
                }

                if (index == 0)
                {
                    CurrencyGroupManager.Instance.UpdateText((UserData.ResourceId) resData.id, resData.count, resNum,
                        0.7f);
                }

                Destroy(cloneGameObjects[index].gameObject);
                CurrencyGroupManager.Instance.PlayShakeAnim((UserData.ResourceId) resData.id);
                PlayHintStarsEffect(targetPos);
                GameObject effectTrail = null;
                if (cloneEffectObjects.ContainsKey(cloneGameObjects[index].gameObject))
                {
                    effectTrail = cloneEffectObjects[cloneGameObjects[index].gameObject];
                    effectTrail.transform.SetParent(EffectRoot);
                }

                StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                {
                    if (effectTrail != null)
                        GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, effectTrail);
                }));
            });
            sequence.Play();
        }

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.25f);

        flyEndCall?.Invoke();
    }

    public IEnumerator ResFlyLogic(GameObject clone, UICurrencyGroupController resController, RewardData rewardData,
        bool showNum, bool showEffect, Action flyEndCall = null)
    {
        if (rewardData == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        bool isAnimEnd = false;
        List<GameObject> cloneGameObjects = new List<GameObject>();
        Dictionary<GameObject, GameObject> cloneEffectObjects = new Dictionary<GameObject, GameObject>();
        int startShowCount = Math.Min(rewardData.num, 10);
        for (int i = 0; i < startShowCount; i++)
        {
            int index = i;
            GameObject cloneObj = GameObject.Instantiate(clone, EffectRoot);

            cloneObj.gameObject.SetActive(true);
            cloneObj.transform.Reset();

            if (showEffect)
            {
                GameObject efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonTrail);
                if (efPrefab != null)
                {
                    efPrefab.gameObject.SetActive(false);
                    efPrefab.transform.SetParent(cloneObj.transform);
                    efPrefab.transform.localPosition = Vector3.zero;
                    efPrefab.transform.localScale = Vector3.one;
                    efPrefab.gameObject.SetActive(true);
                    cloneEffectObjects.Add(cloneObj, efPrefab);
                }
            }

            int x = Random.Range(-50, 50);
            int y = Random.Range(-10, 10);
            cloneObj.gameObject.transform.position = rewardData.gameObject.transform.position;
            Vector3 localPosition = cloneObj.gameObject.transform.localPosition;
            localPosition.x += x;
            localPosition.y += y;
            cloneObj.gameObject.transform.localPosition = localPosition;
            cloneObj.transform.localScale = Vector3.zero;
            cloneGameObjects.Add(cloneObj);

            Image image = cloneObj.GetComponent<Image>();
            if (image != null)
                image.color = new Color(1, 1, 1, 0.4f);

            float fadeTime = 0.03f;
            float scaleTime = 0.08f;

            Vector3 control = Vector3.zero;
            Vector3 targetPos = GetResourcePosition((UserData.ResourceId) rewardData.type,resController);
            Vector3 srcPos = cloneObj.transform.position;
            Vector3 vDis = (targetPos - srcPos);
            control.x = srcPos.x + Random.Range(vDis.x / 3, vDis.x / 2);
            control.y = srcPos.y - Random.Range(0.1f, 0.5f);


            int resNum = UserData.Instance.GetRes((UserData.ResourceId) rewardData.type);

            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(index * 0.05f);
            sequence.Append(cloneObj.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.Linear));
            if (image != null)
                sequence.Insert(index * scaleTime, image.DOFade(1, fadeTime));
            sequence.Insert(index * scaleTime,
                cloneObj.transform.DOPath(new Vector3[] {srcPos, control, targetPos}, 1.2f, PathType.CatmullRom)
                    .SetEase(Ease.InQuart));
            sequence.OnComplete(() =>
            {
                ShakeManager.Instance.ShakeLight();

                if (index == cloneGameObjects.Count - 1)
                {
                    isAnimEnd = true;
                }

                if (index == 0)
                {
                    resController.UpdateText((UserData.ResourceId) rewardData.type, rewardData.num, resNum, 0.7f);
                    if (resController != CurrencyGroupManager.Instance.currencyController)
                        CurrencyGroupManager.Instance.UpdateText((UserData.ResourceId) rewardData.type, rewardData.num,
                            resNum, 0f);
                }

                Destroy(cloneGameObjects[index].gameObject);
                resController.PlayShakeAnim((UserData.ResourceId) rewardData.type);
                PlayHintStarsEffect(targetPos);
                GameObject effectTrail = null;
                if (cloneEffectObjects.ContainsKey(cloneGameObjects[index].gameObject))
                {
                    effectTrail = cloneEffectObjects[cloneGameObjects[index].gameObject];
                    effectTrail.transform.SetParent(EffectRoot);
                }

                StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                {
                    if (effectTrail != null)
                        GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, effectTrail);
                }));
            });
            sequence.Play();
        }

        if (showNum)
        {
            Vector3 localPos = EffectRoot.transform.InverseTransformPoint(rewardData.gameObject.transform.position);
            localPos.y -= 40;
            PlayNumEffect(localPos, rewardData.num);
        }

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.25f);

        flyEndCall?.Invoke();
    }

    public void PlayHintStarsEffect(Vector3 position)
    {
        AudioManager.Instance.PlaySound(21);
        PlayEffect(ObjectPoolName.CommonHintStars, position);
    }
    
    public void PlayEffect(string poolName, Vector3 position, float delayTime = 0.5f)
    {
        GameObject efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(poolName);
        if (efPrefab == null)
            return;

        efPrefab.gameObject.SetActive(false);
        efPrefab.transform.SetParent(EffectRoot);
        efPrefab.transform.position = position;
        efPrefab.transform.localScale = Vector3.one;
        efPrefab.gameObject.SetActive(true);

        StartCoroutine(CommonUtils.DelayWork(delayTime,
            () => { GamePool.ObjectPoolManager.Instance.DeSpawn(poolName, efPrefab); }));
    }

    public GameObject PlayNumEffect(Vector3 localPosition, int num, Transform parent = null)
    {
        GameObject efPrefab = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonNum);
        if (efPrefab == null)
            return null;

        efPrefab.gameObject.SetActive(false);
        efPrefab.transform.SetParent(parent == null ? EffectRoot : parent);
        efPrefab.transform.Reset();
        efPrefab.gameObject.SetActive(true);

        LocalizeTextMeshProUGUI addText = efPrefab.transform.Find("Root/UIAdd").GetComponent<LocalizeTextMeshProUGUI>();
        if (addText != null)
        {
            addText.SetTerm(num.ToString());
            efPrefab.transform.localPosition = localPosition;
        }

        StartCoroutine(CommonUtils.DelayWork(2,
            () => { GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonNum, efPrefab); }));

        return efPrefab;
    }

    public IEnumerator ItemFlyLogic(ResData resData, Action flyEndCall = null, Transform targetTransform = null)
    {
        if (resData == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergeItem);
        if (clone == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        Vector3 endPos = Vector3.zero;
        if (targetTransform == null)
        {
            targetTransform = GetItemFlyTarget(resData.id);
            endPos = targetTransform.position;
        }
        else
        {
            endPos = targetTransform.position;
        }

        clone.gameObject.transform.SetParent(EffectRoot);
        clone.gameObject.transform.localPosition = Vector3.zero;
        clone.gameObject.transform.localScale = Vector3.one;
        UpdateMergeItemImage(clone, resData.id);


        GameObject efPrefab = SpawnGameObject(clone.transform, ObjectPoolName.CommonTrail);

        GameObject efBgPrefab = SpawnGameObject(clone.transform, ObjectPoolName.CommonBgEffect);
        if (efBgPrefab != null)
            efBgPrefab.transform.SetAsFirstSibling();

        float waitTime = 0.4f;
        float moveTime = 0.6f;

        bool isAnimEnd = false;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(waitTime);
        sequence.Append(clone.gameObject.transform.DOScale(0.6f, moveTime));
        sequence.Insert(0, clone.gameObject.transform.DOMove(endPos, moveTime).SetEase(Ease.InBack).OnComplete(() =>
        {
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
            PlayHintStarsEffect(targetTransform.position);
            Animator shake = targetTransform.transform.GetComponent<Animator>();
            if (shake != null)
                shake.Play("shake", 0, 0);
            var root = targetTransform.transform.Find("Root");
            if (root != null)
            {
                Animator play_ani = root.GetComponent<Animator>();
                if (play_ani != null)
                    play_ani.Play("appear", 0, 0);
            }
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonTrail, efPrefab);

            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonBgEffect, efBgPrefab);

            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergeItem, clone);
            //UIHomeMainController.mainController.PlayResBarAnim(resData.id);
        }));

        sequence.OnComplete(() => { isAnimEnd = true; });
        sequence.Play();

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(0.3f);

        flyEndCall?.Invoke();
    }

    public IEnumerator TMItemFlyLogic(RewardData rewardData, Action flyEndCall = null,float beginTime = 0f)
    {
        if (rewardData == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        Transform targetTransform = null;
        Vector3 endPos = Vector3.zero;
        
        targetTransform = UIHomeMainController.mainController.butTmatch.transform;
        endPos = UIHomeMainController.mainController.butTmatch.transform.position;

        // rewardData.SetActive(false);
        rewardData.gameObject.transform.SetParent(EffectRoot);

        float waitTime = 0.4f;
        float moveTime = 0.6f;

        bool isAnimEnd = false;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(beginTime);
        sequence.AppendCallback(() => rewardData.SetActive(true));
        sequence.AppendInterval(waitTime);
        sequence.Append(rewardData.gameObject.transform.DOScale(0.6f, moveTime));
        sequence.Insert(beginTime+waitTime, rewardData.gameObject.transform.DOMove(endPos, moveTime).SetEase(Ease.InBack).OnComplete(
            () =>
            {
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
                PlayHintStarsEffect(targetTransform.position);
                Animator shake = targetTransform.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);
                var root = targetTransform.transform.Find("Root");
                if (root != null)
                {
                    Animator play_ani = root.GetComponent<Animator>();
                    if (play_ani != null)
                        play_ani.Play("appear", 0, 0);
                }
       
                rewardData.SetActive(false);
                //UIHomeMainController.mainController.PlayResBarAnim(resData.id);
            }));

        sequence.OnComplete(() => { isAnimEnd = true; });
        sequence.Play();

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(0.3f);

        flyEndCall?.Invoke();
    }
    public IEnumerator ScrewItemFlyLogic(RewardData rewardData, Action flyEndCall = null,float beginTime = 0f)
    {
        if (rewardData == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        Transform targetTransform = null;
        Vector3 endPos = Vector3.zero;
        
        targetTransform = UIHomeMainController.mainController.butScrew.transform;
        endPos = UIHomeMainController.mainController.butScrew.transform.position;

        // rewardData.SetActive(false);
        rewardData.gameObject.transform.SetParent(EffectRoot);

        float waitTime = 0.4f;
        float moveTime = 0.6f;

        bool isAnimEnd = false;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(beginTime);
        sequence.AppendCallback(() => rewardData.SetActive(true));
        sequence.AppendInterval(waitTime);
        sequence.Append(rewardData.gameObject.transform.DOScale(0.6f, moveTime));
        sequence.Insert(beginTime+waitTime, rewardData.gameObject.transform.DOMove(endPos, moveTime).SetEase(Ease.InBack).OnComplete(
            () =>
            {
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
                PlayHintStarsEffect(targetTransform.position);
                Animator shake = targetTransform.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);
                var root = targetTransform.transform.Find("Root");
                if (root != null)
                {
                    Animator play_ani = root.GetComponent<Animator>();
                    if (play_ani != null)
                        play_ani.Play("appear", 0, 0);
                }
       
                rewardData.SetActive(false);
                //UIHomeMainController.mainController.PlayResBarAnim(resData.id);
            }));

        sequence.OnComplete(() => { isAnimEnd = true; });
        sequence.Play();

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(0.3f);

        flyEndCall?.Invoke();
    }

    public Transform GetItemFlyTarget(int itemId)
    {
        Transform targetTransform = null;
        if (itemId == (int)UserData.ResourceId.Asmr)
        {
            targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.DigTrench)
        {
            if (DitchModel.Instance.Ios_Ditch_Plan_D())
                targetTransform = UIHomeMainController.mainController.DecorateTransform.Find("Icon");
            else
                targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.FishEatFish)
        {
            targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.OnePath)
        {
            targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.ConnectLine)
        {
            targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.BlueBlock)
        {
            targetTransform = UIHomeMainController.mainController.GameTransform.Find("Root");
        }
        else if (itemId == (int)UserData.ResourceId.CardPackageFreeLevel1 ||
                 itemId == (int)UserData.ResourceId.CardPackageFreeLevel2 ||
                 itemId == (int)UserData.ResourceId.CardPackageFreeLevel3 ||
                 itemId == (int)UserData.ResourceId.CardPackageFreeLevel4 ||
                 itemId == (int)UserData.ResourceId.CardPackageFreeLevel5 ||
                 itemId == (int)UserData.ResourceId.CardPackagePayLevel1 ||
                 itemId == (int)UserData.ResourceId.CardPackagePayLevel2 ||
                 itemId == (int)UserData.ResourceId.CardPackagePayLevel3 ||
                 itemId == (int)UserData.ResourceId.CardPackagePayLevel4 ||
                 itemId == (int)UserData.ResourceId.CardPackagePayLevel5 ||
                 itemId == (int)UserData.ResourceId.CardPackagePangLevel1 ||
                 itemId == (int)UserData.ResourceId.CardPackagePangLevel2 ||
                 itemId == (int)UserData.ResourceId.CardPackagePangLevel3 ||
                 itemId == (int)UserData.ResourceId.CardPackagePangLevel4 ||
                 itemId == (int)UserData.ResourceId.CardPackagePangLevel5)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeCardPackage)
                    targetTransform = MergeTaskTipsController.Instance.MergeCardPackage.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                targetTransform = UIHomeMainController.mainController.MainPlayTransform;
            }
        }
        else if (itemId ==  (int)UserData.ResourceId.WildCard3 ||
                 itemId ==  (int)UserData.ResourceId.WildCard4 ||
                 itemId ==  (int)UserData.ResourceId.WildCard5)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeCardCollection)
                    targetTransform = MergeTaskTipsController.Instance.MergeCardCollection.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_CardCollectionTheme.Instance != null && Aux_CardCollectionTheme.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_CardCollectionTheme.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.AvatarEaster2024 ||
                 itemId == (int)UserData.ResourceId.AvatarEaster2025 ||
                 itemId == (int)UserData.ResourceId.AvatarNvZhu ||
                 itemId == (int)UserData.ResourceId.AvatarMonopoly4 ||
                 itemId == (int)UserData.ResourceId.AvatarMonopoly5 ||
                 itemId == (int)UserData.ResourceId.AvatarSnakeLadder4 ||
                 itemId == (int)UserData.ResourceId.AvatarEaster2024_4 ||
                 itemId == (int)UserData.ResourceId.AvatarSnakeLadder ||
                 itemId == (int)UserData.ResourceId.AvatarMonopoly ||
                 itemId == (int)UserData.ResourceId.AvatarDonut ||
                 itemId == (int)UserData.ResourceId.AvatarCamping ||
                 itemId == (int)UserData.ResourceId.AvatarMonopoly2 ||
                 itemId == (int)UserData.ResourceId.AvatarMonopoly3 ||
                 itemId == (int)UserData.ResourceId.AvatarSnakeLadder3 ||
                 itemId == (int)UserData.ResourceId.AvatarHalloween ||
                 itemId == (int)UserData.ResourceId.AvatarSpaceDog ||
                 itemId == (int)UserData.ResourceId.AvatarDonut2 ||
                 itemId == (int)UserData.ResourceId.AvatarEaster2024_2)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                targetTransform = MergeMainController.Instance.backTrans;
            }
            else
            {
                var topUI =
                    UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Top) as MainAux_TopController;
                targetTransform = topUI._setButton.transform;
            }
        }
        else if (itemId == (int)UserData.ResourceId.Easter2024Egg)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeEaster2024)
                    targetTransform = MergeTaskTipsController.Instance.MergeEaster2024.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_Easter2024.Instance != null && Aux_Easter2024.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_Easter2024.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.SnakeLadderTurntable)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (MergeTaskTipsController.Instance.MergeSnakeLadder)
                    targetTransform = MergeTaskTipsController.Instance.MergeSnakeLadder.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                if (Aux_SnakeLadder.Instance != null && Aux_SnakeLadder.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_SnakeLadder.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.ThemeDecorationScore)
        {
            var storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeThemeDecoration, DynamicEntry_Game_ThemeDecoration>();
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_ThemeDecoration>();
                if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                {
                    targetTransform = auxItem.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.SlotMachineScore)
        {
            var storage = SlotMachineModel.Instance.CurStorage;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSlotMachine, DynamicEntry_Game_SlotMachine>();
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_SlotMachine>();
                if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                {
                    targetTransform = auxItem.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.Turntable)
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                var entrance = MergeTaskTipsController.Instance.MergeTurntableEntry;
                if (entrance)
                    targetTransform = entrance.transform;
                else
                    targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {  if (Aux_Turntable.Instance != null && Aux_Turntable.Instance.gameObject.activeInHierarchy)
                {
                    targetTransform = Aux_Turntable.Instance.transform;
                }
                else
                {
                    targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.GardenShovel || itemId == (int)UserData.ResourceId.GardenBomb)
        {
            var window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGardenTreasureMain);
            if (window != null)
            {
                if (itemId == (int)UserData.ResourceId.GardenShovel)
                    targetTransform = ((UIGardenTreasureMainController)window).ShovelTransform;
                else
                    targetTransform = ((UIGardenTreasureMainController)window).BombTransform;
            }
            else
            {
                if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
                {
                    var entrance = MergeTaskTipsController.Instance.MergeGardenTreasureEntry;
                    if (entrance)
                        targetTransform = entrance.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
                else
                {  if (Aux_GardenTreasure.Instance != null && Aux_GardenTreasure.Instance.gameObject.activeInHierarchy)
                    {
                        targetTransform = Aux_GardenTreasure.Instance.transform;
                    }
                    else
                    {
                        targetTransform = UIHomeMainController.mainController.MainPlayTransform;
                    }
                }
            }
        }
        else if (itemId == (int)UserData.ResourceId.MonopolyDice)
        {
            targetTransform = MonopolyModel.Instance.GetFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KeepPetDogFrisbee)
        {
            targetTransform = KeepPetModel.Instance.GetDogFrisbeeFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KeepPetDogDrumstick)
        {
            targetTransform = KeepPetModel.Instance.GetDogDrumstickFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KeepPetDogSteak)
        {
            targetTransform = KeepPetModel.Instance.GetDogSteakFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KeepPetDogHead)
        {
            targetTransform = KeepPetModel.Instance.GetDogHeadFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.MixMasterCoffee|| 
                 itemId == (int)UserData.ResourceId.MixMasterTea|| 
                 itemId == (int)UserData.ResourceId.MixMasterMilk||
                 itemId == (int)UserData.ResourceId.MixMasterLemonJuice||
                 itemId == (int)UserData.ResourceId.MixMasterIceCream||
                 itemId == (int)UserData.ResourceId.MixMasterCream||
                 itemId == (int)UserData.ResourceId.MixMasterPearl||
                 itemId == (int)UserData.ResourceId.MixMasterSugar||
                 itemId == (int)UserData.ResourceId.MixMasterIce ||
                 itemId == (int)UserData.ResourceId.MixMasterExtra1)
        {
            targetTransform = MixMasterModel.Instance.GetCommonFlyTarget();
        }
        else if (BlindBoxModel.Instance.IsBlindBoxId(itemId))
        {
            targetTransform = BlindBoxModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.TurtlePangPackage)
        {
            targetTransform = TurtlePangModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.StarrySkyCompassRocket)
        {
            targetTransform = StarrySkyCompassModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.ZumaBall||
                 itemId == (int)UserData.ResourceId.ZumaBomb||
                 itemId == (int)UserData.ResourceId.ZumaLine)
        {
            targetTransform = ZumaModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KapibalaLife||
                 itemId == (int)UserData.ResourceId.KapibalaReborn)
        {
            targetTransform = KapibalaModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KapiTileLife||
                 itemId == (int)UserData.ResourceId.KapiTileReborn)
        {
            targetTransform = KapiTileModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.KapiScrewLife||
                 itemId == (int)UserData.ResourceId.KapiScrewReborn)
        {
            targetTransform = KapiScrewModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.FishCultureScore)
        {
            targetTransform = FishCultureModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.PhotoAlbumScore)
        {
            targetTransform = PhotoAlbumModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.PillowWheel)
        {
            targetTransform = PillowWheelModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.CatchFish)
        {
            targetTransform = PillowWheelModel.Instance.GetCommonFlyTarget();
        }
        else if (itemId == (int)UserData.ResourceId.BuyDiamondTicket1)
        {
            targetTransform = StoreModel.Instance.GetCommonFlyTarget();
        }
        else if (ExtraOrderRewardCouponModel.IsCouponId((int)itemId))
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                if (ExtraOrderRewardCouponModel.IsPayCouponId((int)itemId))
                {
                    if (MergeTaskTipsController.Instance.MergeExtraOrderRewardCoupon)
                        targetTransform = MergeTaskTipsController.Instance.MergeExtraOrderRewardCoupon.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
                else if (ExtraOrderRewardCouponModel.IsFreeCouponId((int)itemId))
                {
                    if (MergeTaskTipsController.Instance.MergeExtraOrderRewardCouponShowView)
                        targetTransform = MergeTaskTipsController.Instance.MergeExtraOrderRewardCouponShowView.transform;
                    else
                        targetTransform = MergeMainController.Instance.rewardBtnTrans;
                }
            }
            else
            {
                targetTransform = UIHomeMainController.mainController.MainPlayTransform;
            }
        }
        else if (SummerWatermelonBreadModel.Instance.IsSummerWatermelonBreadItemId(itemId))
        {
            if (SummerWatermelonBreadModel.MainView)
            {
                targetTransform = SummerWatermelonBreadModel.MainView._newItemBtn.transform;
            }
            else
            {
                if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
                {
                    targetTransform = MergeSummerWatermelonBread.Instance.transform;
                }
                else
                {

                    targetTransform = Aux_SummerWatermelonBread.Instance?Aux_SummerWatermelonBread.Instance.transform:MergeMainController.Instance.rewardBtnTrans;
                }   
            }
        }
        else
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                targetTransform = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
            
                targetTransform = UIHomeMainController.mainController.MainPlayTransform;
            }
        }
        return targetTransform;
    }
    public IEnumerator ItemFlyLogic(RewardData rewardData, Action flyEndCall = null,float beginTime = 0f,bool needHide = true)
    {
        if (rewardData == null)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        Transform targetTransform = GetItemFlyTarget(rewardData.type);
        Vector3 endPos = targetTransform!=null?targetTransform.position:Vector3.zero;
        
        // if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        // {
        //     targetTransform = MergeMainController.Instance.rewardBtnTrans;
        //     endPos = targetTransform.position;
        // }
        // else
        // {
        //     
        //     targetTransform = UIHomeMainController.mainController.MainPlayTransform;
        //     endPos = UIHomeMainController.mainController.MainPlayPosition;
        // }

        // rewardData.SetActive(false);
        rewardData.gameObject.transform.SetParent(EffectRoot);

        float waitTime = 0.4f;
        float moveTime = 0.6f;

        bool isAnimEnd = false;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(beginTime);
        sequence.AppendCallback(()=>rewardData.SetActive(true));
        sequence.AppendInterval(waitTime);
        sequence.Append(rewardData.gameObject.transform.DOScale(0.6f, moveTime));
        sequence.Insert(beginTime+waitTime, rewardData.gameObject.transform.DOMove(endPos, moveTime).SetEase(Ease.InBack).OnComplete(
            () =>
            {
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
                PlayHintStarsEffect(targetTransform.position);
                Animator shake = targetTransform.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);
                var root = targetTransform.transform.Find("Root");
                if (root != null)
                {
                    Animator play_ani = root.GetComponent<Animator>();
                    if (play_ani != null)
                        play_ani.Play("appear", 0, 0);
                }
       
                if (needHide)
                    rewardData.SetActive(false);
                //UIHomeMainController.mainController.PlayResBarAnim(resData.id);
            }));

        sequence.OnComplete(() => { isAnimEnd = true; });
        sequence.Play();

        while (!isAnimEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(0.3f);

        flyEndCall?.Invoke();
    }

    public IEnumerator RewardFlyLogic(List<RewardData> rewardDatas, UICurrencyGroupController resController,
        Action flyEndCall = null,bool ignoreEventSystem = false,bool needHide = true)
    {
        if (rewardDatas == null || rewardDatas.Count == 0)
        {
            flyEndCall?.Invoke();
            yield break;
        }

        int flyEndCount = 0;
        Action endCall = () => { flyEndCount++; };

        bool isEnableEventSystem = UIRoot.Instance.EnableEventSystem;
        if (!ignoreEventSystem)
        {
            UIRoot.Instance.EnableEventSystem = false;
        }

        int index = 0;
        for (int i = 0; i < rewardDatas.Count; i++)
        {
            RewardData rdData = rewardDatas[i];
            if (rdData == null)
            {
                endCall();
                continue;
            }

            // if (!UserData.Instance.IsResource(rdData.type))
            //     MergeToResource(rdData);
            if (TMatchModel.Instance.IsTMatchResId(rdData.type))
            {
                StartCoroutine(TMItemFlyLogic(rdData, endCall,0.2f * index));
                // yield return new WaitForSeconds(0.2f * index);
                index++;
            }
            else if (ScrewGameModel.Instance.IsScrewResId(rdData.type))
            {
                StartCoroutine(ScrewItemFlyLogic(rdData, endCall,0.2f * index));
                // yield return new WaitForSeconds(0.2f * index);
                index++;
            }
            else if (UserData.Instance.IsResource(rdData.type))
            {
                Transform iconTransform = resController.GetIconTransform((UserData.ResourceId) rdData.type);
                if (iconTransform == null)
                {
                    iconTransform = rdData.image?.transform;
                }
                if (iconTransform == null)
                {
                    endCall();
                    continue;
                }

                if (needHide)
                    rdData.SetActive(false);
                StartCoroutine(ResFlyLogic(iconTransform.gameObject, resController, rdData, true, false, endCall));
            }
            else
            {
                StartCoroutine(ItemFlyLogic(rdData, endCall,0.2f * index));
                // yield return new WaitForSeconds(0.2f * index);
                index++;
            }
        }

        while (true)
        {
            if (flyEndCount < rewardDatas.Count)
                yield return null;
            else
                break;
        }

        if (!ignoreEventSystem)
        {
            UIRoot.Instance.EnableEventSystem = isEnableEventSystem;
        }

        flyEndCall?.Invoke();
    }

    private static Vector3 RandomFlyPos()
    {
        if (UIHomeMainController.mainController == null)
            return Vector3.zero;

        int x = Random.Range(-100, 100);
        int y = Random.Range(-50, 50);

        Vector3 flyPos = UIHomeMainController.mainController.FlyPos;

        return new Vector3(flyPos.x + x, flyPos.y + y, flyPos.z);
    }

    private static Vector3 RandomItemPos()
    {
        int x = Random.Range(-200, 200);
        int y = Random.Range(-50, 50);

        return new Vector3(x, y, 0);
    }

    public Image UpdateMergeItemImage(GameObject mergeObj, int mergeId)
    {
        
        if (mergeObj == null)
            return null;

        Image icon = mergeObj.transform.GetComponentDefault<Image>("Icon");
        if (icon == null)
            return null;
        
        if (mergeId == (int)UserData.ResourceId.Asmr)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn");;
            return icon;
        }
        else if (mergeId == (int)UserData.ResourceId.DigTrench)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn_DigTrench");;
            return icon;
        }
        else if (mergeId == (int) UserData.ResourceId.FishEatFish)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn_FishEatFish");
            return icon;
        }
        else if (mergeId == (int) UserData.ResourceId.OnePath)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn_OnePath");
            return icon;
        }
        else if (mergeId == (int) UserData.ResourceId.ConnectLine)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn_ConnectLine");
            return icon;
        }
        else if (mergeId == (int) UserData.ResourceId.BlueBlock)
        {
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant("HomeAtlas", "Home_Game_Btn_BlueBlock");
            return icon;
        }
        
        if (UserData.Instance.IsFarmRes(mergeId))
        {
            var propConfig = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == mergeId);
            if (propConfig != null)
            {
                icon.sprite = FarmModel.Instance.GetFarmIcon(propConfig.Image);
                return icon;
            }
        }
        
        var productConfig = FarmConfigManager.Instance.TableFarmProductList.Find(a => a.Id == mergeId);
        if (productConfig != null)
        {
            icon.sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
            return icon;
        }

        icon.sprite = UserData.GetResourceIcon(mergeId, UserData.ResourceSubType.Big);

        return icon;
    }

    private bool MergeToResource(RewardData rewardData)
    {
        if (rewardData == null)
            return false;

        if (UserData.Instance.IsResource(rewardData.type))
            return false;

        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(rewardData.type);
        if (config == null)
            return false;

        ResData resData = null;
        UserData.ResourceId resId = UserData.ResourceId.None;
        switch (config.type)
        {
            case  (int) MergeItemType.energy:
                resId = UserData.ResourceId.Energy;
                break;
            case  (int) MergeItemType.decoCoin:
                resId = UserData.ResourceId.Coin;
                break;
            case  (int) MergeItemType.diamonds:
                resId = UserData.ResourceId.Diamond;
                break;
            case  (int) MergeItemType.exp:
                resId = UserData.ResourceId.Exp;
                break;
        }

        if (resId == UserData.ResourceId.None)
            return false;

        rewardData.type = (int) resId;
        rewardData.num = config.value;

        return true;
    }

    private GameObject SpawnGameObject(Transform parent, string poolName)
    {
        GameObject spObj = GamePool.ObjectPoolManager.Instance.Spawn(poolName);
        if (spObj == null)
            return null;

        spObj.gameObject.SetActive(false);
        spObj.transform.SetParent(parent);
        spObj.transform.Reset();
        spObj.gameObject.SetActive(true);

        return spObj;
    }
    
    public void FlyObject(Decoration.TableNodes tableNodes, List<ResData> dayResData, Action flyEndCall = null)
    {
        if (tableNodes == null)
        {
            flyEndCall?.Invoke();
            return;
        }

        if (tableNodes.rewardId == null || tableNodes.rewardId.Length<=0)
        {
            flyEndCall?.Invoke();
            return;
        }

        List<ResData> resDatas = new List<ResData>();
        for (int i = 0; i < tableNodes.rewardId.Length; i++)
        {
            ResData res = new ResData(tableNodes.rewardId[i], tableNodes.rewardNumber[i]);
            resDatas.Add(res);
        }

        if (dayResData != null && dayResData.Count > 0)
        {
            resDatas.AddRange(dayResData);
        }
        
        FlyObject(resDatas, flyEndCall);
    }
}