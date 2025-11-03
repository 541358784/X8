using System;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    private RectTransform _target;
    private GameObject _arrow;

    public const int LEFT = 1;
    public const int RIGHT = 2;
    public const int UP = 3;
    public const int DOWN = 4;

    private void Awake()
    {
    }

    private void _BottomArrowAnimation(RectTransform target, float scale = 1f)
    {
        var from = target.transform.position.y + 2 * scale;
        var to = from + 0.5f;
        DOTween.To(
            () => from,
            y =>
            {
                _arrow.transform.position = new Vector3(_arrow.transform.position.x, y, _arrow.transform.position.z);
            },
            to,
            0.5f).SetLoops(int.MaxValue, LoopType.Yoyo).SetEase(Ease.OutQuad).Play();
    }

    private void _UpArrowAnimation(RectTransform target, float scale = 1f)
    {
        var from = target.position.y - 2f * scale;
        var to = from - 0.5f;
        DOTween.To(
            () => from,
            y =>
            {
                _arrow.transform.position = new Vector3(_arrow.transform.position.x, y, _arrow.transform.position.z);
            },
            to,
            0.5f).SetLoops(int.MaxValue, LoopType.Yoyo).SetEase(Ease.OutQuad).Play();
    }

    private void _RightArrowAnimation(RectTransform target, float scale = 1f)
    {
        var from = target.transform.position.x - 2 * scale;
        var to = from - 0.5f;
        DOTween.To(
            () => from,
            x =>
            {
                _arrow.transform.position = new Vector3(x, _arrow.transform.position.y, _arrow.transform.position.z);
            },
            to,
            0.5f).SetLoops(int.MaxValue, LoopType.Yoyo).SetEase(Ease.OutQuad);
    }

    private void _LeftArrowAnimation(RectTransform target, float scale = 1f)
    {
        var from = target.transform.position.x + 2 * scale;
        var to = from + 0.5f;
        DOTween.To(
            () => from,
            x =>
            {
                _arrow.transform.position = new Vector3(x, _arrow.transform.position.y, _arrow.transform.position.z);
            },
            to,
            0.5f).SetLoops(int.MaxValue, LoopType.Yoyo).SetEase(Ease.OutQuad);
    }

    public void SetTarget(RectTransform target, int dir, float scale = 1f)
    {
        if (_arrow == null)
        {
            GameObject prefabs =
                DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>(
                    Path.Combine(PathManager.uiPrefabPathUnderExport, "Home/Arrow"));
            _arrow = Instantiate(prefabs, transform);
        }

        _target = target;
        _arrow.transform.localScale *= scale;
        _arrow.transform.position = target.transform.position;

        switch (dir)
        {
            case Arrow.LEFT:
                _arrow.transform.localEulerAngles = new Vector3(0, 0, -90);
                _arrow.transform.position += new Vector3(2f * scale, 0, 0);
                _LeftArrowAnimation(target, scale);
                break;
            case Arrow.RIGHT:
                _arrow.transform.localEulerAngles = new Vector3(0, 0, 90);
                _arrow.transform.position -= new Vector3(2f * scale, 0, 0);
                _RightArrowAnimation(target, scale);
                break;
            case Arrow.UP:
                _arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
                _arrow.transform.position -= new Vector3(0, 2 * scale, 0);
                _UpArrowAnimation(target, scale);
                break;
            case Arrow.DOWN:
                _arrow.transform.localEulerAngles = Vector3.zero;
                _arrow.transform.position += new Vector3(0, 2 * scale, 0);
                _BottomArrowAnimation(target, scale);
                break;
        }
    }
}