using System.Collections;
using Activity.BattlePass;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class MergeBattlePassItem
{
    public Image _icon;
    public LocalizeTextMeshProUGUI _numText;
    public GameObject _finish;
    public GameObject _gameObject;
    public bool _isShow = false;
    public Activity.BattlePass.TableBattlePassTask _config;
    public Activity.BattlePass_2.TableBattlePassTask _config2;
    private int _hideWaitSeconds;
    private Animator _animator;
    private Coroutine _coroutine;
    
    public void Init(GameObject gameObject)
    {
        _gameObject = gameObject;

        _icon = gameObject.transform.Find("Icon").GetComponent<Image>();
        _numText = gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _finish = gameObject.transform.Find("Finish").gameObject;
        _animator = gameObject.GetComponent<Animator>();
    }

    public void Update(Activity.BattlePass.TableBattlePassTask config, int totalNum, int num,int addNum)
    {
        if (_config != null && _config != config && totalNum < num)
            return;
        
        _config = config;
        _numText.SetText(totalNum + "/" + num);
        _finish.gameObject.SetActive(totalNum >= num);
        //_numText.gameObject.SetActive(totalNum < num);

        _hideWaitSeconds = 3;

        if (_config.type == (int)TaskType.MergerItem)
        {
            TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(_config.mergeid);
                
            _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
        }
        else
        {
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant("BattlePassAtlas", _config.image);
        }
        
        if (totalNum >= num)
            _config = null;
    }

    public void Update(Activity.BattlePass_2.TableBattlePassTask config, int totalNum, int num,int addNum)
    {
        if (_config2 != null && _config2 != config && totalNum < num)
            return;
        
        _config2 = config;
        _numText.SetText(totalNum + "/" + num);
        _finish.gameObject.SetActive(totalNum >= num);
        //_numText.gameObject.SetActive(totalNum < num);

        _hideWaitSeconds = 3;

        if (_config2.type == (int)TaskType.MergerItem)
        {
            TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(_config2.mergeid);
                
            _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
        }
        else
        {
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant("BPTwoAtlas", _config2.image);
        }
        
        if (totalNum >= num)
            _config2 = null;
    }
    public void SetActive(bool isShow)
    {
        _isShow = isShow;
        _gameObject.SetActive(isShow);
        if (isShow)
        {
            _animator.Play("hint",0,0);
        }
        
        if (_coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }
        
        if (!isShow && _config != null && Activity.BattlePass.BattlePassTaskModel.Instance.IsCompleteDailyTask(_config.id))
            _config = null;
        
        if (!isShow && _config2 != null && Activity.BattlePass_2.BattlePassTaskModel.Instance.IsCompleteDailyTask(_config2.id))
            _config2 = null;
    }

    public void InvokeRepeating()
    {
        if(!_isShow)
            return;

        _hideWaitSeconds--;
        if (_hideWaitSeconds > 0)
            return;

        if (_coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(_coroutine);
        }

        _coroutine = CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", null, () =>
        {
            SetActive(false);
            _coroutine = null;
        },false));
    }
}

public partial class MergeTaskTipsController
{
    private GameObject _dilayTaskItem;
    private GameObject _fixedTaskItem;
    private GameObject _challengeTaskItem;
    private GameObject _dilayTaskItem_2;
    private GameObject _fixedTaskItem_2;
    private GameObject _challengeTaskItem_2;

    private MergeBattlePassItem _dailyItem = new MergeBattlePassItem();
    private MergeBattlePassItem _fixedItem = new MergeBattlePassItem();
    private MergeBattlePassItem _challengeItem = new MergeBattlePassItem();
    
    private MergeBattlePassItem _dailyItem_2 = new MergeBattlePassItem();
    private MergeBattlePassItem _fixedItem_2 = new MergeBattlePassItem();
    private MergeBattlePassItem _challengeItem_2 = new MergeBattlePassItem();

    private bool _isShow = false;
    private int _hideWaitSeconds = 0;
    
    private void InitBattlePass()
    {
        if (BattlePassModel.Instance.IsInitFromServer())
        {
            InitBattlePass1();
        }

        if (Activity.BattlePass_2.BattlePassModel.Instance.IsInitFromServer())
        {
            InitBattlePass2();
        }
        // _dilayTaskItem = transform.Find("BattlePass/Viewport/Content/DailyTask").gameObject;
        // _fixedTaskItem = transform.Find("BattlePass/Viewport/Content/FixedTask").gameObject;
        // _challengeTaskItem = transform.Find("BattlePass/Viewport/Content/ChallengeTask").gameObject;
        //
        // _dilayTaskItem_2 = transform.Find("BattlePass_2/Viewport/Content/DailyTask").gameObject;
        // _fixedTaskItem_2 = transform.Find("BattlePass_2/Viewport/Content/FixedTask").gameObject;
        // _challengeTaskItem_2 = transform.Find("BattlePass_2/Viewport/Content/ChallengeTask").gameObject;
        //
        // _dailyItem.Init(_dilayTaskItem);
        // _fixedItem.Init(_fixedTaskItem);
        // _challengeItem.Init(_challengeTaskItem);
        //
        // _dailyItem_2.Init(_dilayTaskItem_2);
        // _fixedItem_2.Init(_fixedTaskItem_2);
        // _challengeItem_2.Init(_challengeTaskItem_2);
    }

    private bool IsInitBattlePass1;
    private bool IsInitBattlePass2;
    public void InitBattlePass1()
    {
        if (IsInitBattlePass1)
            return;
        if (BattlePassModel.Instance.IsOpened())
        {
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(
                "Prefabs/Activity/BattlePass/MergeTip_BattlePass");
            var item = Instantiate(asset, transform).transform;
            _dilayTaskItem = item.Find("Viewport/Content/DailyTask").gameObject;
            _fixedTaskItem = item.Find("Viewport/Content/FixedTask").gameObject;
            _challengeTaskItem = item.Find("Viewport/Content/ChallengeTask").gameObject;
            _dailyItem.Init(_dilayTaskItem);
            _fixedItem.Init(_fixedTaskItem);
            _challengeItem.Init(_challengeTaskItem);
            IsInitBattlePass1 = true;
        }
        else
        {
            XUtility.WaitSeconds(1f, InitBattlePass1);
        }
    }
    public void InitBattlePass2()
    {
        if (IsInitBattlePass2)
            return;
        if (Activity.BattlePass_2.BattlePassModel.Instance.IsOpened())
        {
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(
                "Prefabs/Activity/BPTwo/MergeTip_BattlePass");
            var item = Instantiate(asset, transform).transform;
            _dilayTaskItem_2 = item.Find("Viewport/Content/DailyTask").gameObject;
            _fixedTaskItem_2 = item.Find("Viewport/Content/FixedTask").gameObject;
            _challengeTaskItem_2 = item.Find("Viewport/Content/ChallengeTask").gameObject;
            _dailyItem_2.Init(_dilayTaskItem_2);
            _fixedItem_2.Init(_fixedTaskItem_2);
            _challengeItem_2.Init(_challengeTaskItem_2);
            IsInitBattlePass2 = true;
        }
        else
        {
            XUtility.WaitSeconds(1f, InitBattlePass2);
        }
    }

    private void InitStatus()
    {
        if (IsInitBattlePass1)
        {
            _dailyItem.SetActive(false);
            _challengeItem.SetActive(false);
            _fixedItem.SetActive(false);
        }

        if (IsInitBattlePass2)
        {
            _dailyItem_2.SetActive(false);
            _challengeItem_2.SetActive(false);
            _fixedItem_2.SetActive(false);
        }
    }
    private void BattlePassTaskComplete(BaseEvent e)
    {
        if (!IsInitBattlePass1)
            return;
        Activity.BattlePass.TableBattlePassTask config = (Activity.BattlePass.TableBattlePassTask)e.datas[0];
        int totalNum = (int)e.datas[1];
        int number = (int)e.datas[2];
        int addNumber = (int)e.datas[3];

        if (config.difficulty <= 6)
        {
            _dailyItem.SetActive(true);
            _dailyItem.Update(config, totalNum, number,addNumber);
        }
        switch ((DifficultyType)config.difficulty)
        {
            case DifficultyType.NormalEasy:
            case DifficultyType.NormalHard:
            case DifficultyType.NormalMid:
            {
                _dailyItem.SetActive(true);
                _dailyItem.Update(config, totalNum, number,addNumber);
                break;
            }
            case DifficultyType.Challenge:
            {
                _challengeItem.SetActive(true);
                _challengeItem.Update(config, totalNum, number,addNumber);
                break;
            }
            case DifficultyType.Fixation:
            {
                _fixedItem.SetActive(true);
                _fixedItem.Update(config, totalNum, number,addNumber);
                break;
            }
        }
        BattlePassTaskAnim(true);
        _hideWaitSeconds = 4;
    }

    private void BattlePass2TaskComplete(BaseEvent e)
    {
        if (!IsInitBattlePass2)
            return;
        Activity.BattlePass_2.TableBattlePassTask config = (Activity.BattlePass_2.TableBattlePassTask)e.datas[0];
        int totalNum = (int)e.datas[1];
        int number = (int)e.datas[2];
        int addNumber = (int)e.datas[3];
         
        if (config.difficulty <= 6)
        {
            _dailyItem_2.SetActive(true);
            _dailyItem_2.Update(config, totalNum, number,addNumber);
        }
        
        switch ((DifficultyType)config.difficulty)
        {
            case DifficultyType.NormalEasy:
            case DifficultyType.NormalHard:
            case DifficultyType.NormalMid:
            {
                _dailyItem_2.SetActive(true);
                _dailyItem_2.Update(config, totalNum, number,addNumber);
                break;
            }
            case DifficultyType.Challenge:
            {
                _challengeItem_2.SetActive(true);
                _challengeItem_2.Update(config, totalNum, number,addNumber);
                break;
            }
            case DifficultyType.Fixation:
            {
                _fixedItem_2.SetActive(true);
                _fixedItem_2.Update(config, totalNum, number,addNumber);
                break;
            }
        }
        BattlePassTaskAnim(true);
        _hideWaitSeconds = 4; 
    }

    private void InvokeRepeating_BattlePassTask()
    {
        if(!_isShow)
            return;

        if (IsInitBattlePass1)
        {
            _dailyItem.InvokeRepeating();
            _challengeItem.InvokeRepeating();
            _fixedItem.InvokeRepeating();   
        }

        if (IsInitBattlePass2)
        {
            _dailyItem_2.InvokeRepeating();
            _challengeItem_2.InvokeRepeating();
            _fixedItem_2.InvokeRepeating();
        }

        _hideWaitSeconds--;
        if(_hideWaitSeconds > 0)
            return;

        BattlePassTaskAnim(false);
    }

    private void BattlePassTaskAnim(bool isShow)
    {
        if(_isShow == isShow)
            return;
        
        _isShow = isShow;

        if (!isShow)
        {
            InitStatus();
        }
    }
}