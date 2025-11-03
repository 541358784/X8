using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class UIZumaMainController
{
    public ScoreBoard ScoreBoardView;
    public TopGroup TopGroupView;
    public void InitTopGroup()
    {
        TopGroupView = transform.Find("Root/Reward").gameObject.AddComponent<TopGroup>();
        TopGroupView.Init(Storage,this);
        ScoreBoardView = transform.Find("Root/NumGroup").gameObject.AddComponent<ScoreBoard>();
        ScoreBoardView.Init(Storage,this);
    }

    public class ScoreBoard : MonoBehaviour
    {
        private ZumaLevelConfig LevelConfig;
        private ZumaModel Model => ZumaModel.Instance;

        private LocalizeTextMeshProUGUI WinScoreText;
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventZumaNewLevel>(OnLevelChange);
            WinScoreText = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventZumaScoreChange>(OnScoreChange);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventZumaNewLevel>(OnLevelChange);
            EventDispatcher.Instance.RemoveEvent<EventZumaScoreChange>(OnScoreChange);
        }

        public void OnLevelChange(EventZumaNewLevel evt)
        {
            transform.DOKill();
            ShowScore = Storage.LevelScore;
            LevelConfig = Model.GetLevel(Storage.LevelId);
            UpdateUI();
        }

        private StorageZuma Storage;
        private UIZumaMainController MainUI;
        // private int ShowScore;
        
        
        private int ShowScore;
        public void Init(StorageZuma storage,UIZumaMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            ShowScore = Storage.LevelScore;
            LevelConfig = Model.GetLevel(Storage.LevelId);
            UpdateUI();
        }

        private List<int> WaitAddValueList = new List<int>();
        public void OnScoreChange(EventZumaScoreChange evt)
        {
            if (evt.NeedWait)
            {
                WaitAddValueList.Add(evt.ChangeValue);
            }
            else
            {
                ShowScore += evt.ChangeValue;
            }
            UpdateUI();
        }

        public void TriggerWaitAddValue()
        {
            if (WaitAddValueList.Count > 0)
            {
                var addValue = WaitAddValueList[0];
                WaitAddValueList.RemoveAt(0);
                var alReadyAddValue = 0;
                var addValueF = (float) addValue;
                DOTween.To(() => 0f, (v) =>
                {
                    var curV = (int) v;
                    if (curV != alReadyAddValue)
                    {
                        var distance = curV - alReadyAddValue;
                        alReadyAddValue = curV;
                        ShowScore += distance;
                        UpdateUI();
                    }
                }, addValueF, 0.5f).OnComplete(() =>
                {
                    if (addValue != alReadyAddValue)
                    {
                        var distance = addValue - alReadyAddValue;
                        alReadyAddValue = addValue;
                        ShowScore += distance;
                        UpdateUI();
                    }
                }).SetTarget(transform);
            }
        }

        public void UpdateUI()
        {
            if (LevelConfig.IsLoopLevel)
            {
                WinScoreText.SetText(ShowScore.ToString());   
            }
            else
            {
                WinScoreText.SetText(ShowScore+"/"+LevelConfig.WinScore);
            }
        }
    }
    
    public class TopGroup : MonoBehaviour
    {
        private StorageZuma Storage;
        private UIZumaMainController MainUI;
        public void Init(StorageZuma storage,UIZumaMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
        }
    }
    
    
    public class RewardBoxTip : MonoBehaviour
    {
        public void ShowTip(List<ResData> resDatas, bool autoClose = true)
        {
            gameObject.SetActive(true);
            Init(resDatas);
            if (autoClose)
                StartAutoClosePopup();
        }

        private Transform _rewardItem;
        private List<Transform> _itemList;

        public void Awake()
        {
            _rewardItem = transform.Find("Icon");
            _rewardItem.gameObject.SetActive(false);
            _itemList = new List<Transform>();
        }

        private Coroutine con;

        public void StartAutoClosePopup()
        {
            con = StartCoroutine(AutoClosePopup());
        }

        private IEnumerator AutoClosePopup()
        {
            yield return new WaitForSeconds(3f); // 等待3秒钟

            HidePopup(); // 3秒后关闭提示框
        }

        public void HidePopup()
        {
            if (con != null)
            {
                StopCoroutine(con);
                con = null;
            }
            gameObject.gameObject.SetActive(false);
        }

        void Update()
        {
            // 检测点击任意位置关闭
            if (Input.GetMouseButtonUp(0))
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                if (results.Count == 0)
                    return;
                foreach (var result in results)
                {
                    if (result.gameObject.transform.parent.gameObject == gameObject)
                        return;
                }

                HidePopup();
            }
        }

        public void Init(List<ResData> resDatas)
        {
            foreach (var item in _itemList)
            {
                DestroyImmediate(item.gameObject);    
            }
            _itemList.Clear();
            for (int i = 0; i < resDatas.Count; i++)
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent);
                item.gameObject.SetActive(true);
                _itemList.Add(item);
                InitItem(item, resDatas[i].id, resDatas[i].count);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        private void InitItem(Transform item, int itemID, int ItemCount)
        {
            item.GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID);
            string tex = ItemCount.ToString();
            if (itemID == (int)UserData.ResourceId.Infinity_Energy)
            {
                tex = TimeUtils.GetTimeString(ItemCount, true);
            }

            item.Find("Num").GetComponent<LocalizeTextMeshProUGUI>().SetText(tex);
            item.gameObject.SetActive(true);
        }
    }
}