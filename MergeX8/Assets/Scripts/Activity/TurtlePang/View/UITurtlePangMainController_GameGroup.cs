using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Transform GameGroup;
    public RectTransform BagArea;
    private LocalizeTextMeshProUGUI BagBallText;
    public List<Turtle> BagTurtleList = new List<Turtle>();
    public VerticalLayoutGroup PackageLayout;
    public Transform PackageDefaultItem;
    public List<Transform> PackageList = new List<Transform>();
    private LocalizeTextMeshProUGUI PackageCountText;
    private int PackageShowCount = 15;

    public enum EffectType
    {
        Lucky,
        Pang,
        Line,
        Clear,
        Full
    }

    public Dictionary<EffectType, string> EffectDic = new Dictionary<EffectType, string>();
    public Transform EffectNode;
    public void InitGameGroup()
    {
        {
            var image = transform.Find("Root/BagGroup/BagBall/Image (1)").gameObject.AddComponent<Canvas>();
            image.overrideSorting = true;
            image.sortingOrder = canvas.sortingOrder + 2;
            var text = transform.Find("Root/BagGroup/BagBall/Text").gameObject.AddComponent<Canvas>();
            text.overrideSorting = true;
            text.sortingOrder = canvas.sortingOrder + 3;
        }
        {
            var image = transform.Find("Root/BagGroup/BagNormal/Image (1)").gameObject.AddComponent<Canvas>();
            image.overrideSorting = true;
            image.sortingOrder = canvas.sortingOrder + 2;
            var text = transform.Find("Root/BagGroup/BagNormal/Text").gameObject.AddComponent<Canvas>();
            text.overrideSorting = true;
            text.sortingOrder = canvas.sortingOrder + 3;
        }
        GameGroup = transform.Find("Root/BagGroup");
        GameGroup.gameObject.SetActive(false);
        BagArea = GetItem<RectTransform>("Root/BagGroup/BagBall/Area");
        BagBallText = GetItem<LocalizeTextMeshProUGUI>("Root/BagGroup/BagBall/Text");
        BagBallText.SetTermFormats("0");
        PackageCountText = GetItem<LocalizeTextMeshProUGUI>("Root/BagGroup/BagNormal/Text");
        PackageLayout = GetItem<VerticalLayoutGroup>("Root/BagGroup/BagNormal");
        PackageDefaultItem = transform.Find("Root/BagGroup/BagNormal/1");
        PackageDefaultItem.gameObject.SetActive(false);
        PackageLayout.enabled = false;
        EffectDic.Add(EffectType.Lucky,"Lucky");
        EffectDic.Add(EffectType.Pang,"Pang");
        EffectDic.Add(EffectType.Line,"Line");
        EffectDic.Add(EffectType.Clear,"Clear");
        EffectDic.Add(EffectType.Full,"Full");
        EffectNode = transform.Find("Root/BagGroup/Effect");
        EffectNode.gameObject.SetActive(false);
        var effectCanvas = EffectNode.gameObject.AddComponent<Canvas>();
        effectCanvas.overrideSorting = true;
        effectCanvas.sortingOrder = canvas.sortingOrder + 1;
        if (Storage.IsInGame)
        {
            GameGroup.gameObject.SetActive(true);
            for (var i = 0; i < Storage.BasePackageCount; i++)
            {
                var package = Instantiate(PackageDefaultItem, PackageDefaultItem.parent);
                package.gameObject.SetActive(true);
                PackageList.Add(package);
            }
            for (var i = 0; i < Storage.ExtraPackageCount; i++)
            {
                var package = Instantiate(PackageDefaultItem, PackageDefaultItem.parent);
                package.gameObject.SetActive(true);
                PackageList.Add(package);
            }
            PackageCountText.SetTermFormats(PackageList.Count.ToString());
            UpdatePackageGroupLayout();
            foreach (var pair in Storage.BagGame)
            {
                var itemConfig = Model.ItemConfig.Find(a => a.Id == pair.Key);
                for (var i = 0; i < pair.Value; i++)
                {
                    var turtle = Instantiate(DefaultTurtle, DefaultTurtle.parent).gameObject.AddComponent<Turtle>();
                    turtle.gameObject.SetActive(true);
                    turtle.transform.position = GetRandomBagAreaPosition();
                    turtle.RandomRotation();
                    turtle.SetScaleSmall();
                    turtle.Init(itemConfig);
                    BagTurtleList.Add(turtle);
                }
            }
            BagBallText.SetTermFormats(BagTurtleList.Count.ToString());
        }
        GetItem<LocalizeTextMeshProUGUI>("Root/BagGroup/TextGroup/Text1").SetTermFormats(
            Model.GlobalConfig.LuckyColorSendCount.ToString(),
            Model.GlobalConfig.SameColorSendCount.ToString(),
            Model.GlobalConfig.DrawLineSendCount.ToString());
        GetItem<LocalizeTextMeshProUGUI>("Root/BagGroup/TextGroup/Text2").SetTermFormats(
            Model.GlobalConfig.FillBoardSendCount.ToString(),
            Model.GlobalConfig.CleanBoardSendCount.ToString());
    }

    public void ShowEffect(EffectType effectType)
    {
        if (effectType == EffectType.Lucky)
        {
            AudioManager.Instance.PlaySoundById(183);
        }
        else if (effectType == EffectType.Pang)
        {
            AudioManager.Instance.PlaySoundById(184);
        }
        else if (effectType == EffectType.Line)
        {
            AudioManager.Instance.PlaySoundById(185);
        }
        else if (effectType == EffectType.Full)
        {
            AudioManager.Instance.PlaySoundById(186);
        }
        else if (effectType == EffectType.Clear)
        {
            AudioManager.Instance.PlaySoundById(187);
        }
        var effect = Instantiate(EffectNode, EffectNode.parent);
        effect.gameObject.SetActive(true);
        effect.gameObject.GetComponent<Canvas>().overrideSorting = true;
        foreach (var pair in EffectDic)
        {
            var ani = effect.Find(pair.Value);
            ani.gameObject.SetActive(false);
            ani.gameObject.SetActive(pair.Key == effectType);
            var text = effect.Find(pair.Value+"/Text").GetComponent<LocalizeTextMeshProUGUI>();
            if (effectType == EffectType.Lucky)
            {
                text.SetTermFormats(Model.GlobalConfig.LuckyColorSendCount.ToString());   
            }
            else if (effectType == EffectType.Pang)
            {
                text.SetTermFormats(Model.GlobalConfig.SameColorSendCount.ToString());   
            }
            else if (effectType == EffectType.Line)
            {
                text.SetTermFormats(Model.GlobalConfig.DrawLineSendCount.ToString());   
            }
            else if (effectType == EffectType.Clear)
            {
                text.SetTermFormats(Model.GlobalConfig.CleanBoardSendCount.ToString());   
            }
            else if (effectType == EffectType.Full)
            {
                text.SetTermFormats(Model.GlobalConfig.FillBoardSendCount.ToString());   
            }
        }
        DOVirtual.DelayedCall(2f, () =>
        {
            Destroy(effect.gameObject);
        }).SetTarget(effect);
    }

    public void UpdatePackageGroupLayout()
    {
        for (var i = 0; i < PackageList.Count; i++)
        {
            PackageList[i].gameObject.SetActive(i < PackageShowCount);
        }
        PackageLayout.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(PackageLayout.transform as RectTransform);
        PackageLayout.enabled = false;
        for (var i = PackageShowCount; i < PackageList.Count; i++)
        {
            PackageList[i].localPosition = PackageList[PackageShowCount - 1].localPosition;
        }
    }
    public Vector3 GetRandomBagAreaPosition()
    {
        Rect rect = BagArea.rect;
        Vector3 worldPoint = BagArea.TransformPoint(new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax)));
        return worldPoint;
    }
    public class TurtlePangGrid : MonoBehaviour
    {
        public Turtle Turtle;
        public Transform Package;
        public UITurtlePangMainController Controller;
        public int Index;
        public LocalizeTextMeshProUGUI NumText;
        public void Init(UITurtlePangMainController controller,int index)
        {
            Controller = controller;
            Index = index;
            NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            NumText.SetText((index+1).ToString());
        }
    }

    public class Turtle : MonoBehaviour
    {
        private float ScaleBig = 1f;
        private float ScaleSmall = 0.5f;
        private float SpinSpeed => 360f*Random.Range(8f,12f);
        private TurtlePangItemConfig Config;
        private Image Icon;
        public void RandomRotation()
        {
            transform.rotation = Quaternion.Euler(0, 0,Random.Range(0f, 360f));
        }
        public void SetScaleSmall()
        {
            transform.localScale = new Vector3(ScaleSmall, ScaleSmall, ScaleSmall);
        }
        public void SetScaleBig()
        {
            transform.localScale = new Vector3(ScaleBig, ScaleBig, ScaleBig);
        }

        public void Init(TurtlePangItemConfig config)
        {
            Config = config;
            Icon = transform.Find("Image").GetComponent<Image>();
            Icon.sprite = Config.GetTurtleIcon();
        }

        public void PerformFly(Vector3 targetPos, float time = 0.5f)
        {
            var effect = transform.Find("Image/vfx_trail_001");
            effect.gameObject.SetActive(true);
            transform.SetAsLastSibling();
            transform.DOKill(false);
            var startPos = transform.position;
            var startScale = transform.localScale;
            var targetScale = new Vector3(ScaleSmall, ScaleSmall, ScaleSmall);
            var startRotation = transform.rotation.eulerAngles.z;
            var targetRotation = startRotation + SpinSpeed * time;
            DOTween.To(() => 0, (p) =>
            {
                var curPos = (targetPos - startPos) * p + startPos;
                transform.position = curPos;
                var curScale = (targetScale - startScale) * p + startScale;
                transform.localScale = curScale;
                var curRotation = (targetRotation - startRotation) * p + startRotation;
                transform.rotation = Quaternion.Euler(0, 0,curRotation);
            }, 1f, time).SetTarget(transform).OnComplete(() =>
            {
                transform.position = targetPos;
                transform.localScale = targetScale;
                transform.rotation = Quaternion.Euler(0, 0,targetRotation);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    effect.gameObject.SetActive(false);
                }).SetTarget(transform);
            });
        }
        public void PerformLucky()
        {
            var curScale = transform.localScale;
            transform.DOScale(curScale * 1.5f, 0.15f).OnComplete(() =>
            {
                transform.DOScale(curScale, 0.15f);
            });
        }
        public void PerformBingo()
        {
            var curScale = transform.localScale;
            transform.DOScale(curScale * 1.5f, 0.15f).OnComplete(() =>
            {
                transform.DOScale(curScale, 0.15f);
            });
            
        }
        public void PerformPang()
        {
            var curScale = transform.localScale;
            transform.DOScale(curScale * 1.5f, 0.15f).OnComplete(() =>
            {
                transform.DOScale(curScale, 0.15f);
            });
            
        }
        public void PerformFillBoard()
        {
            var curScale = transform.localScale;
            transform.DOScale(curScale * 1.5f, 0.15f).OnComplete(() =>
            {
                transform.DOScale(curScale, 0.15f);
            });
            
        }
    }
}