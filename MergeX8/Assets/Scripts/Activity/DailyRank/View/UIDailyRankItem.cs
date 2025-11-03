using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;
using Framework;

public class UIDailyRankItem : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _ranking;
    private LocalizeTextMeshProUGUI _name;
    private LocalizeTextMeshProUGUI _score;
    private GameObject _firstObj;
    private GameObject _otherObj;
    private GameObject _bubbleObj;
    private Button _boxGroup;
    private GameObject _rwItem;
    private Canvas _bubbleCanvas;
    private StorageDailyRank _dailyRank;
    
    public int _index { get; private set; }
    public  bool _isSelf { get; private set; }
    
    public int _scoreValue{get; private set;}
    
    public string _userName{get; private set;}

    private List<GameObject> _cloneItems = new List<GameObject>();
    private void Awake()
    {
        _firstObj = transform.Find("LevelNum/First").gameObject;
        _rwItem = transform.Find("BoxGroup/BubbleGroup/Icon1").gameObject;
        _boxGroup = transform.Find("BoxGroup").GetComponent<Button>();
        _otherObj = transform.Find("LevelNum/Other").gameObject;
        _bubbleObj = transform.Find("BoxGroup/BubbleGroup").gameObject;
        _bubbleCanvas =transform.Find("BoxGroup/BubbleGroup").GetComponent<Canvas>();
        
        _ranking = transform.Find("LevelNum/Other/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _name = transform.Find("NameGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _score = transform.Find("ScoreGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _boxGroup.onClick.AddListener(OnBtnBox);
        
        _bubbleObj.gameObject.SetActive(false);
        _rwItem.gameObject.SetActive(false);
    }

    private void OnBtnBox()
    {
        _bubbleObj.gameObject.SetActive(true);
        
        StopAllCoroutines();
        StartCoroutine(DelayFunction());
    }

    private IEnumerator DelayFunction()
    {
        yield return new WaitForSeconds(2f);
        _bubbleObj.gameObject.SetActive(false);
    }

    public void InitData(int index, string name, int score, bool isSelf, int sortOrder, StorageDailyRank dailyRank)
    {
        _index = index;
        _isSelf = isSelf;
        _scoreValue = score;
        _userName = name;
        _dailyRank = dailyRank;
        
        _bubbleCanvas.sortingOrder = sortOrder + 1;
        UpdateUI();
    }

    public void InitImage()
    {
        foreach (var obj in _cloneItems)
        {
            DestroyImmediate(obj);
        }
        _cloneItems.Clear();
        
        if (_index <= 3)
        {
            foreach (var reward in DailyRankModel.Instance.GetRankReward(_index, _dailyRank))
            {
                var item = Instantiate(_rwItem, _bubbleObj.transform);
                item.gameObject.SetActive(true);
                item.GetComponent<Image>().sprite = UserData.GetResourceIcon(reward.id, UserData.ResourceSubType.Big);
                
                _cloneItems.Add(item);
            }
        }
    }
    
    public void UpdateData(int index, int score)
    {
        _index = index;
        _scoreValue = score;
        
        UpdateUI();
        InitImage();
    }

    private void UpdateUI()
    {
        _name.SetText(_userName);
        _ranking.SetText(_index.ToString());
        _score.SetText(_scoreValue.ToString());
        
        _firstObj.gameObject.SetActive(_index == 1);
        _otherObj.gameObject.SetActive(_index != 1);
        _boxGroup.gameObject.SetActive(_index <= 3);
    }
}