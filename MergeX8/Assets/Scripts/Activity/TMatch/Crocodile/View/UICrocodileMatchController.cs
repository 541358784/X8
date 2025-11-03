using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.WinStreak;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UICrocodileMatchController : TMatch.UIWindowController
{
    #region open ui

    /// <summary>
    /// 预制体路径
    /// </summary>
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/Crocodile/UICrocodileMatch";

    /// <summary>
    /// 打开
    /// </summary>
    public static void Open()
    {
        TMatch.UIManager.Instance.OpenWindow<UICrocodileMatchController>(PREFAB_PATH);
    }

    #endregion
    private Button _closeButton;
    private Transform _myHead;
    private Transform _otherHead;
    private Transform _vfx;
    private LocalizeTextMeshProUGUI _numberText;
    private LocalizeTextMeshProUGUI _prizeText;

    private static List<Robot> _robotconfig;
    public override void PrivateAwake()
    {
        _closeButton = GetItem<Button>("Root/ButtonClose");
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _myHead = transform.Find("Root/ButtomGroup/NameItemOne");
        _otherHead =  transform.Find("Root/ButtomGroup/NameItemOther");
        _vfx =  transform.Find("Root/MiddleGroup/VFX_Hint_Stars_001");
        _numberText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/NumberText");
        _prizeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/PrizeGroup/NumberText");
    }

    protected override void OnOpenWindow(TMatch.UIWindowData data)
    {
        base.OnOpenWindow(data);
        ControllTips(false);
        PlayHeadMatchAnimation();
    }


    private void ControllTips(bool enable)
    {
        _closeButton.gameObject.SetActive(enable);
    }

    private void OnCloseButtonClicked()
    {
        CloseWindowWithinUIMgr();
        UICrocodileMainController.Open();
    }

    private async void PlayHeadMatchAnimation()
    {
        _numberText.SetText("0/100");
        _prizeText.SetText(CrocodileActivityModel.Instance.GetBaseConfig().RewardCnt[0].ToString());
        _myHead.gameObject.SetActive(false);
        _otherHead.gameObject.SetActive(false);
        _myHead.SetAsLastSibling();

        await Task.Delay(670);

        ShowHead(_myHead, false);

        // 实例化 30 来个机器人头像，在 150 度的扇形区域内排布
        var currInstanceCount = 0; // 记录目前已经实例化了多少个头像，好计算每个的延时
        var maxRadius = 1.4f; // 扇形半径
        var heightFactor = 1.3f; // 最高点的距离是半径的多少倍。为了呈现一种椭圆感
        var randomNum = Random.Range(0, 2); // 0/1 随机数
        var isFromLeft = randomNum % 2 == 0; // 第一层实例化的方向，随机

        var perSideAngle = 75; // 左右两侧，每侧的最大角度
        var leftEdgeDirNorm = Quaternion.Euler(0, 0, perSideAngle) * Vector3.up; // 扇形的左边缘，归一化矢量
        var rightEdgeDirNorm = Quaternion.Euler(0, 0, 360 - perSideAngle) * Vector3.up; // 扇形的右边缘，归一化矢量
        var headLayers = new List<int>{4, 5, 6, 7, 8, 9}; // 每层头像的实例化个数
        var layerCount = headLayers.Count; // 暂定有 6 层
        var totalHeadCount = headLayers.Sum(); // 总共有多少个头像

        // 一共六层，一层一层展示
        for (var i = 0; i < layerCount; i++)
        {
            var headCount = headLayers[i];
            var halfCount = (headCount - 1) / 2f;
            var internalAngle = (float)(perSideAngle * 2) / (headCount - 1);

            for (var j = 0; j < headCount; j++)
            {
                var currLayerRadius = (maxRadius / layerCount) * (i + 1);
                var targetDir = (isFromLeft ? leftEdgeDirNorm : rightEdgeDirNorm) * currLayerRadius;
                for (var k = 0; k < j; k++)
                {
                    targetDir = Quaternion.Euler(0, 0, isFromLeft ? 360 - internalAngle : internalAngle) * targetDir;
                }

                targetDir *= ((heightFactor - 1) * (1 - Mathf.Abs(j - halfCount) / halfCount) + 1);
                var targetPos = _myHead.position + targetDir;

                // 实例化节点
                var head = GameObject.Instantiate(_otherHead, _otherHead.parent);
                head.SetAsFirstSibling();
                head.position = targetPos;
                head.gameObject.SetActive(false);

                currInstanceCount++;

                ShowHead(head, true, currInstanceCount * 1.8f / totalHeadCount);
            }

            isFromLeft = !isFromLeft;
        }

        // 数字滚动动画
        var startNumber = 0;
        var duration = 3;
        DOTween.To(() => startNumber, x => startNumber = x, 100, duration)
            .SetEase(Ease.OutQuint)
            .OnUpdate(() =>
            {
                _numberText.SetText(startNumber + "/100");
                if (startNumber == 100) _vfx.gameObject.SetActive(true);
            });
        ControllTips((int)(duration - 0.5f) * 1000);
    }

    private async void ShowHead(Transform head, bool isOther = true, float waitTime = 0)
    {
        await Task.Delay((int)(waitTime * 1000));
        head.gameObject.SetActive(true);
        var animator = head.GetComponent<Animator>();
        animator.Play("matchother");

        ShowLetterOnHeadIcon(head, isOther);
    }

    /// <summary>
    /// 显示英文名首字母
    /// </summary>
    public static void ShowLetterOnHeadIcon(Transform head, bool isOther = true)
    {
        var image = head.GetComponentInChildren<Image>();
        var text = head.GetComponentInChildren<Text>();
        if (image == null || text == null)
        {
            return;
        }

        if (!isOther)
        {
            text.text =StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;

            // image.sprite = XUtility.GetHeadIcon(StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId);
        }
        else
        {
            var iconId = Random.Range(0, 6);
            // image.sprite = XUtility.GetHeadIcon(iconId);

            // Name: read config table
            if (TryGetRobotConfig())
            {
                var robotNameCount = _robotconfig.Count;
                var randomIndex = Random.Range(0, robotNameCount);
                var randomName = _robotconfig[randomIndex].name;
                text.text = randomName[0].ToString();
            }
            else
            {
                // Name: random ASCII A~Z, A_65, Z_90
                var randomNum = Random.Range(65, 91);
                var asciiEncoding = new System.Text.ASCIIEncoding();
                var byteArray = new byte[] { (byte)randomNum };
                var strCharacter = asciiEncoding.GetString(byteArray);
                text.text = strCharacter;

                DebugUtil.LogError("[WinStreakMatchPopup] _robotconfig == null, please check config!");
            }
        }
    }

    private static bool TryGetRobotConfig()
    {
        if (_robotconfig != null)
        {
            return true;
        }

        _robotconfig = CrocodileActivityModel.Instance.GetRobotConfig();
        return _robotconfig != null;
    }

    private async void ControllTips(int waitTime)
    {
        await Task.Delay(waitTime);
        ControllTips(true);
    }
    
}