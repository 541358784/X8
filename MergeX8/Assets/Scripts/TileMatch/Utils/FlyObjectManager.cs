using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dlugin;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay;
using GamePool;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FlyObjectManager : Manager<FlyObjectManager>
{
    public Transform EffectRoot
    {
        get { return UIRoot.Instance.transform; }
    }

    public void FlySingleRes(UserData.ResourceId resourceId, Vector3 srcPos, Vector3 destPos, float scale = 1f,
        float speed = 10,Action action = null)
    {
        srcPos.z = 0;
        destPos.z = 0;
        var flyItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonFlyItem);
        CommonUtils.AddChild(EffectRoot, flyItem.transform);
        flyItem.transform.localScale = Vector3.one * scale;
        flyItem.transform.position = srcPos;
        Image image = flyItem.transform.Find("Icon").GetComponent<Image>();
        image.sprite = UserData.GetResourceIcon(resourceId);
        Vector3 vDis = (destPos - srcPos);
        Vector3 controlPos = Vector3.zero;
        controlPos.x = srcPos.x + Random.Range(vDis.x / 3, vDis.x / 2);
        controlPos.y = srcPos.y - Random.Range(0.1f, 0.5f);
        float dis = Vector3.Distance(destPos, srcPos);

        float time = dis / speed;
        Sequence sequence = DOTween.Sequence();
        sequence.Insert( 0, flyItem.transform
            .DOPath(new Vector3[] {srcPos, controlPos, destPos}, time, PathType.CatmullRom)
            .SetEase(Ease.InQuart));
        sequence.OnComplete(() =>
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonFlyItem, flyItem);
            action?.Invoke();
        });
        sequence.Play();
    }
    public void FlyRes(ResData resData, Vector3 srcPos, Vector3 destPos, float scale = 1f, float speed = 10,
        Action action = null)
    {
        srcPos.z = 0;
        destPos.z = 0;
        int num = Math.Min(resData.count, 10);
        for (int i = 0; i < num; i++)
        {
            var flyItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonFlyItem);
            CommonUtils.AddChild(EffectRoot, flyItem.transform);
            Vector3 randomPos = new Vector3(srcPos.x + Random.Range(-0.5f, 0.5f), srcPos.y + Random.Range(-0.2f, 0.2f),
                srcPos.z);

            flyItem.transform.localScale = Vector3.zero;
            flyItem.transform.position = randomPos;

            Image image = flyItem.transform.Find("Icon").GetComponent<Image>();
            image.sprite = UserData.GetResourceIcon(resData.id);
            if (image != null)
                image.color = new Color(1, 1, 1, 0.0f);

            Vector3 vDis = (destPos - randomPos);
            Vector3 controlPos = Vector3.zero;
            controlPos.x = randomPos.x + Random.Range(vDis.x / 3, vDis.x / 2);
            controlPos.y = randomPos.y - Random.Range(0.1f, 0.5f);
            float dis = Vector3.Distance(destPos, randomPos);

            float time = dis / speed;
            float animTime = 0.02f;
            int index = i;
            float delayTime = i * 0.05f;
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(delayTime);
            sequence.Append(flyItem.transform.DOScale(scale, animTime).SetEase(Ease.Linear));
            if (image != null)
                sequence.Insert(0, image.DOFade(1, animTime).SetEase(Ease.Linear));
            sequence.Insert(delayTime + animTime, flyItem.transform
                .DOPath(new Vector3[] {randomPos, controlPos, destPos}, time, PathType.CatmullRom)
                .SetEase(Ease.InQuart));
            sequence.OnComplete(() =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonFlyItem, flyItem);
                if (index == num - 1)
                {
                    action?.Invoke();

                }
                if (index == 0)
                {
                }
            });
            sequence.Play();
        }
    }

    public void FlyItem(ResData resData, Vector3 srcPos, Vector3 destPos, float scale = 1f, float speed = 10,
        Action action = null)
    {
        srcPos.z = 0;
        destPos.z = 0;
        var flyItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonFlyItem);
        CommonUtils.AddChild(EffectRoot, flyItem.transform);

        flyItem.transform.localScale = Vector3.zero;
        flyItem.transform.position = srcPos;

        Image image = flyItem.transform.Find("Icon").GetComponent<Image>();
        image.sprite = UserData.GetResourceIcon(resData.id);
        if (image != null)
            image.color = new Color(1, 1, 1, 0.0f);

        Vector3 offset = destPos - srcPos;
        float dis = Vector3.Distance(srcPos, destPos);
        float time = dis / speed;

        float animTime = 0.02f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(flyItem.transform.DOScale(scale, animTime).SetEase(Ease.Linear));
        if (image != null)
            sequence.Insert(0, image.DOFade(1, animTime).SetEase(Ease.Linear));
        sequence.Insert(0.05f,
            flyItem.transform.DOJump(destPos, Math.Abs(offset.y * 3 / 2), 1, time).SetEase(Ease.InQuart));
        sequence.OnComplete(() =>
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonFlyItem, flyItem);
            action?.Invoke();
        });
        sequence.Play();
    }
    public void FlySingleItem(GameObject flyItem, Vector3 srcPos, Vector3 destPos, float scale = 1f,
        float speed = 10,Action action = null)
    {
        srcPos.z = 0;
        destPos.z = 0;
        CommonUtils.AddChild(EffectRoot, flyItem.transform);
        flyItem.transform.localScale = Vector3.one * scale;
        flyItem.transform.position = srcPos;
      
        Vector3 vDis = (destPos - srcPos);
        Vector3 controlPos = Vector3.zero;
        controlPos.x = srcPos.x + Random.Range(vDis.x / 3, vDis.x / 2);
        controlPos.y = srcPos.y - Random.Range(0.1f, 0.5f);
        float dis = Vector3.Distance(destPos, srcPos);

        float time = dis / speed;
        Sequence sequence = DOTween.Sequence();
        sequence.Insert( 0, flyItem.transform
            .DOPath(new Vector3[] {srcPos, controlPos, destPos}, time, PathType.CatmullRom)
            .SetEase(Ease.InQuart));
        sequence.OnComplete(() =>
        {
            action?.Invoke();
        });
        sequence.Play();
    }
    public void JumpItem(GameObject targetObj, Vector3 destPos, float scale = 1f, float speed = 10,
        Action action = null)
    {
        Vector3 srcPos = targetObj.transform.position;
        srcPos.z = 0;
        destPos.z = 0;

        float dis = Vector3.Distance(srcPos, destPos);
        float time = dis / speed;

        targetObj.transform.DOScale(scale, time).SetEase(Ease.Linear);
        targetObj.transform.DOJump(destPos, 3.5f, 1, time).SetEase(Ease.InQuart).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    public void FlyItemAlpha(GameObject targetObj, Vector3 destPos, float scale = 1f, float speed = 10,
        Action action = null)
    {
        Vector3 srcPos = targetObj.transform.position;
        srcPos.z = 0;
        destPos.z = 0;

        float dis = Vector3.Distance(srcPos, destPos);
        float time = dis / speed;
        targetObj.transform.DOScale(scale, time).SetEase(Ease.Linear);
        targetObj.transform.DOMove(destPos, time).SetEase(Ease.InBack).OnComplete(() => { action?.Invoke(); });
    }

    public void FlyItem(GameObject targetObj, Vector3 destPos, float scale = 1f, float speed = 10, Action action = null)
    {
        Vector3 srcPos = targetObj.transform.position;
        srcPos.z = 0;
        destPos.z = 0;

        float dis = Vector3.Distance(srcPos, destPos);
        float time = dis / speed;
        targetObj.transform.DOScale(scale, time).SetEase(Ease.Linear);
        targetObj.transform.DOMove(destPos, time).SetEase(Ease.InBack).OnComplete(() => { action?.Invoke(); });
    }

    public void FlyItemOnEffectLayer(GameObject targetObj, Vector3 destPos, float scale = 1f, float speed = 10,
        Action action = null)
    {
        var cloneTargetObj = Instantiate(targetObj, EffectRoot);
        cloneTargetObj.gameObject.SetActive(true);
        cloneTargetObj.transform.position = targetObj.transform.position;
        cloneTargetObj.transform.localScale = Vector3.one;
        FlyItemAlpha(cloneTargetObj, destPos, scale, speed, () =>
        {
            if (cloneTargetObj)
            {
                Destroy(cloneTargetObj);
            }

            action?.Invoke();
        });
        // cloneTargetObj.transform.DOScale(new Vector3(scale, scale, scale), 0.2f).onComplete = () =>
        // {
        //    
        // };
        
    }

    public void FlyItem(ResData resData, Vector3 srcPos, Vector3 destPos, float scale, float endScale, float speed,
        Action<int> action, Action actionEnd = null)
    {
        srcPos.z = 0;
        destPos.z = 0;
        int num = Math.Min(resData.count, 10);
        for (int i = 0; i < num; i++)
        {
            var flyItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonFlyItem);
            CommonUtils.AddChild(EffectRoot, flyItem.transform);

            Vector3 randomPos = new Vector3(srcPos.x + Random.Range(-0.4f, 0.4f), srcPos.y + Random.Range(-0.2f, 0.2f),
                srcPos.z);

            flyItem.transform.localScale = Vector3.zero;
            flyItem.transform.position = randomPos;

            Image image = flyItem.transform.Find("Icon").GetComponent<Image>();
            image.sprite = UserData.GetResourceIcon(resData.id);
            if (image != null)
                image.color = new Color(1, 1, 1, 0.0f);

            float animTime = 0.5f;
            int index = i;
            float time = Vector3.Distance(srcPos, destPos) / speed;
            Sequence sequence = DOTween.Sequence();
            float delayTime = i * 0.3f;

            sequence.SetDelay(delayTime);
            sequence.Append(flyItem.transform.DOScale(scale, animTime).SetEase(Ease.Linear));
            if (image != null)
                sequence.Insert(0, image.DOFade(1, animTime).SetEase(Ease.Linear));
            sequence.Insert(delayTime + animTime, flyItem.transform.DOMove(destPos, time).SetEase(Ease.InBack));
            sequence.Insert(delayTime + animTime, flyItem.transform.DOScale(endScale, time));
            sequence.OnComplete(() =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonFlyItem, flyItem);
                action?.Invoke(index + 1);
                if (index == num - 1)
                    actionEnd?.Invoke();
            });
            sequence.Play();
        }
    }

    public void FlyItemBezier(GameObject targetObj, Vector3 destPos, float speed = 10, Action action = null, float scale = 1.0f)
    {
        Vector3 srcPos = targetObj.transform.position;
        srcPos.z = 0;
        destPos.z = 0;

        targetObj.transform.position = srcPos;
        
        var vDis = destPos - srcPos;
        var controlPos = Vector2.zero;

        controlPos.x = srcPos.x + vDis.x / 2;
        controlPos.y = srcPos.y + vDis.y / 2f;

        float time = Vector3.Distance(destPos, srcPos) / speed;
        time = Mathf.Min(time, 1f);
        time = Mathf.Max(time, 0.5f);

        var normal = new Vector2(vDis.y, -vDis.x);
        controlPos = Vector3.MoveTowards(controlPos, normal, 1);

        if(scale != 1.0f)
            targetObj.transform.DOScale(scale, time).SetEase(Ease.Linear);
        targetObj.transform.DOPath(new Vector3[] {srcPos, controlPos, destPos}, time, PathType.CatmullRom)
            .SetEase(Ease.InSine).OnComplete(() => { action?.Invoke(); });
    }
}