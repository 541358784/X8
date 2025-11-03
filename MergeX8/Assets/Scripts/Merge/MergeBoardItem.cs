using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK;
using DG.Tweening;
using System;
using Activity.BalloonRacing;
using Activity.Matreshkas.Model;
using Activity.RabbitRacing.Dynamic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using GamePool;
using Merge.OneselfChose;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MergeBoardItem : MonoBehaviour
{
    private MergeBoardEnum BoardId
    {
        get
        {
            if (!_boardIdSetFlag)
            {
                Debug.LogError("MergeBoardItem未设置boardId");    
            }
            return _boardId;
        }
    }

    private MergeBoardEnum _boardId;
    private bool _boardIdSetFlag = false;
    public void SetBoardId(MergeBoardEnum boardId)
    {
        _boardIdSetFlag = true;
        _boardId = boardId;
    }
    
    private string[] _mergeEffectName = new[]
    {
        ObjectPoolName.vfx_ComboMerge_01, ObjectPoolName.vfx_ComboMerge_02, ObjectPoolName.vfx_ComboMerge_03,
        ObjectPoolName.vfx_ComboMerge_04
    };
    
    public TableMergeItem tableMergeItem;

    //特效
    private Transform obj_output;
    private Transform obj_output_special;
    private Transform vfx_increase;
    private Transform obj_merge;
    private Transform obj_smoke;
    private Transform obj_bubble;
    private Transform obj_box;
    private Transform obj_box_happygo;
    private Transform obj_ring;
    private Transform obj_task;
    private Transform obj_dailyTask;
    private Transform obj_orderTask;
    private Transform obj_garageCleanup;
    private Image obj_energy;
    private Transform obj_energyTorrent;
    private Transform obj_lock;
    private Transform obj_lock_happygo;
    private Transform obj_icon;
    private Transform obj_time;
    private Transform obj_mc_time;
    private Transform obj_vfx_speed;
    private Transform obj_timeMask;
    private Transform obj_power;
    private Transform obj_tap;
    private Transform obj_hint;
    private Transform obj_openTips;
    private Transform obj_taskBg;
    public Transform vfx_trail;
    private Transform obj_bubbleGroup;
    private Transform vfx_boxHappyGo;
    private Image image_bubble;
    private Image image_bubble1;
    public Image image_icon;
    private Image image_lock;
    private Image image_lock_happygo;
    private Image image_time;
    private Image image_box;
    private Image image_box_happygo;
    private Transform obj_balloonRacing;
    private Transform obj_rabbitRacing;
    
    private Animator animator;
    private Animator anim_bubble;
    private Animator anim_time;
    private Animator mc_anim_time;
    private Animator anim_box;
    private Animator anim_box_happygo;
    private Animator anim_power;

    private Transform _fullLevel;
    private Transform vfx_transition_01;
    private Material ui_grey;
    private Slider _eatSlider;
    private GameObject _eatBackGround;
    public Animator Animator
    {
        get { return animator; }
    }

    float itemSize;

    private int _id = -1;
    private MergeItemStatus _state = MergeItemStatus.box;
    private int _index = -1;
    private bool isProductItem = false;
    private bool isTimeProductItem = false;
    private bool isEnergyProduct = false;
    public StorageMergeItem storageMergeItem;

    private Dictionary<string, Coroutine> mergeDelayAnims = new Dictionary<string, Coroutine>();

    public MergeItemStatus state
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
        }
    }

    public int id
    {
        get { return _id; }
        set
        {
            _id = value;
            if (_id < 0)
            {
                tableMergeItem = null;
                isProductItem = false;
                return;
            }

            tableMergeItem = GameConfigManager.Instance.GetItemConfig(_id);
            isProductItem = MergeConfigManager.Instance.IsProductItem(tableMergeItem);
            isTimeProductItem = MergeConfigManager.Instance.IsTimeProductItem(tableMergeItem);
            isEnergyProduct = MergeConfigManager.Instance.IsEnergyProductItem(tableMergeItem);
        }
    }

    public int index
    {
        get { return _index; }
        set
        {
            _index = value;
            storageMergeItem = MergeManager.Instance.GetBoardItem(_index,BoardId);
        }
    }

    public Vector3 _orgLocalPosition;
    private Sequence _doTweenSequence;
    private Vector3 _oldLocalPosition = Vector3.zero;
    private Vector3 _orgLockPosition = Vector3.one;
    
    //private Material webLockMaterial;
    private void Awake()
    {
        Init();

        itemSize = (transform as RectTransform).sizeDelta.x;

        //webLockMaterial = Resources.Load<Material>("Materials/MergeWebLock");
    }

    public void Init()
    {
        obj_smoke = transform.Find("vfx_Smoke");
        obj_icon = transform.Find("Icon");
        obj_box = transform.Find("box");
        obj_box_happygo = transform.Find("boxHappyGo");
        obj_energy = transform.Find("Energy").GetComponent<Image>();
        obj_energyTorrent = transform.Find("EnergyTorrent");
        obj_lock = transform.Find("Lock");
        obj_lock_happygo = transform.Find("LockHappyGo");
        obj_bubble = transform.Find("bubble");
        obj_task = transform.Find("Task");
        obj_dailyTask = transform.Find("DailyTask");
        obj_orderTask = transform.Find("OrderTask");
        obj_garageCleanup = transform.Find("GarageCleanup");
        obj_time = transform.Find("Time");
        obj_mc_time = transform.Find("MCTime");
        obj_vfx_speed = transform.Find("vfx_speed");
        obj_timeMask = transform.Find("TimeMask");
        obj_power = transform.Find("Power");
        vfx_transition_01 = transform.Find("vfx_transition_01");
        obj_merge = transform.Find("vfx_merge");
        obj_output = transform.Find("vfx_Output");
        obj_output_special = transform.Find("vfx_Output_special");
        vfx_increase = transform.Find("vfx_increase");
        obj_ring = transform.Find("vfx_Ring_001");
        obj_tap = transform.Find("StyleTip");
        obj_hint = transform.Find("vfx_BG_Hint");
        obj_openTips = transform.Find("OpenTips");
        obj_taskBg = transform.Find("TaskBG");
        vfx_trail = transform.Find("vfx_trail");
        obj_balloonRacing = transform.Find("BalloonRacing");
        obj_rabbitRacing= transform.Find("RabbitRacing");
        
        obj_bubbleGroup = transform.Find("BubbleGroup");
        vfx_boxHappyGo = transform.Find("vfx_boxHappyGo");
        image_bubble = transform.GetComponentDefault<Image>("BubbleGroup/Root/Image");
        image_bubble1 = transform.GetComponentDefault<Image>("BubbleGroup/Root/Image2");
        image_icon = transform.GetComponentDefault<Image>("Icon");
        image_lock = transform.GetComponentDefault<Image>("Lock");
        image_lock_happygo = transform.GetComponentDefault<Image>("LockHappyGo");
        image_time = transform.GetComponentDefault<Image>("TimeMask");
        image_box = transform.GetComponentDefault<Image>("box");
        image_box_happygo = transform.GetComponentDefault<Image>("boxHappyGo");

        animator = transform.GetComponent<Animator>();
        anim_time = obj_time.GetComponent<Animator>();
        mc_anim_time = obj_mc_time.GetComponent<Animator>();
        anim_box = obj_box.GetComponent<Animator>();
        anim_box_happygo = obj_box_happygo.GetComponent<Animator>();
        anim_power = obj_power.GetComponent<Animator>();
        anim_bubble = obj_bubble.GetComponent<Animator>();
        _fullLevel = transform.Find("FullLevel");
        ui_grey = ResourcesManager.Instance.LoadResource<Material>("Effects/Materials/UIMaterials/UI_Grey");
        _eatSlider=transform.Find("Slider").GetComponent<Slider>();
        _eatSlider.gameObject.SetActive(false);
        _eatBackGround = _eatSlider.transform.Find("Background (1)").gameObject;
    }

    public void UpdateData(bool isInitImages = true)
    {
        transform.localScale = Vector3.one;
    
        if (isInitImages)
            InitImages();

        if(state != MergeItemStatus.box)
            MergeManager.Instance.AddShowedItemId(tableMergeItem.id);

        MergeManager.Instance.RecordGetItem(storageMergeItem);
    
        InitStatus();
        PlayPowerAnimator();
        SetTapImageStatus();
        UpdateImageColor();
        RefreshAllStatus();
    }

    private void InitImages()
    {
        if (image_lock != null && image_lock.sprite == null)
            image_lock.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite("Web11"); 
        
        if (image_lock_happygo != null && image_lock_happygo.sprite == null)
            image_lock_happygo.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite("icon_hg_vd_LockBox0");
        
        UpdateIconImage();
       
        string lockImageName = "LockBox" + UnityEngine.Random.Range(1, 5);
        if (image_box != null && image_box.sprite == null)
            image_box.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(lockImageName);
           
        string lockImageName_happyGo = "icon_hg_vd_LockBox4" ;
        if (image_box_happygo != null && image_box.sprite == null)
            image_box_happygo.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(lockImageName_happyGo);
        
        if (tableMergeItem.type == (int)MergeItemType.hamaster)
        {
            GameObject resource = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/Squirrel");
            if (resource != null)
            {
                GameObject hamsterObj = Instantiate(resource, image_icon.transform, false);
                hamsterObj.transform.localPosition = Vector3.zero;
                image_icon.enabled = false;
            }
        }

        if (_boardId == MergeBoardEnum.HappyGo)
        {
            obj_energy.sprite = UserData.GetResourceIcon(UserData.ResourceId.HappyGo_Energy);
        }
        else
        {
            obj_energy.sprite = UserData.GetResourceIcon(UserData.ResourceId.Energy);

        }
    }

    public void UpdateIconImage(int id = -1)
    {
        TableMergeItem itemConfig = tableMergeItem;

        if (id > 0)
        {
            itemConfig = GameConfigManager.Instance.GetItemConfig(id);
            if(itemConfig == null)
                itemConfig = tableMergeItem;
        }
        
        if (itemConfig == null)
            return;
        
        
        //Debug.LogError("itemConfig.image " + itemConfig.image + "\t" + index + "\t" + itemConfig.id);
        if (image_icon.sprite != null && image_icon.sprite.name != itemConfig.image ||
            image_icon.sprite == null)
        {
            string powerImageName = "";
            if (itemConfig.type ==  (int) MergeItemType.energy)
                powerImageName = "1";
            if (itemConfig.type == (int) MergeItemType.eatBuild && MergeManager.Instance.IsEatAllFood(storageMergeItem))
            {
                image_icon.sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image_full + powerImageName);
            }
            else
            {
                image_icon.sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image + powerImageName);
            }
       
        }
    }
    
    public void PlayMergeEffect()
    {
        int index = tableMergeItem.merge_effect_index - 1;
        if(tableMergeItem == null || index < 0 || index >= _mergeEffectName.Length)
            return;

        FlyGameObjectManager.Instance.PlayEffect(_mergeEffectName[index], transform.position, 1);
    }  
    public void PlayRemoveEffect()
    {
        vfx_transition_01.gameObject.SetActive(false);
        vfx_transition_01.gameObject.SetActive(true);
    }
    private void InitStatus()
    {
        obj_lock.gameObject.SetActive(state == MergeItemStatus.locked && _boardId!=MergeBoardEnum.HappyGo);
        obj_lock_happygo.gameObject.SetActive(state == MergeItemStatus.locked && _boardId==MergeBoardEnum.HappyGo);
        obj_bubble.gameObject.SetActive(state == MergeItemStatus.bubble);
        obj_icon.gameObject.SetActive(state != MergeItemStatus.box);
        obj_box.gameObject.SetActive(state == MergeItemStatus.box&& _boardId!=MergeBoardEnum.HappyGo);
        obj_box_happygo.gameObject.SetActive(state == MergeItemStatus.box&& _boardId==MergeBoardEnum.HappyGo);
        obj_openTips.gameObject.SetActive(state == MergeItemStatus.time || tableMergeItem.type==(int)MergeItemType.eatBuild);
        
        bool isEnergyTorrent=MergeConfigManager.Instance.IsEnergyTorrentProduct(tableMergeItem) &&
            EnergyTorrentModel.Instance.IsOpen();
        obj_energy.gameObject.SetActive(isEnergyProduct && storageMergeItem.State == 1 && !isEnergyTorrent);
        obj_energyTorrent.gameObject.SetActive(isEnergyProduct && storageMergeItem.State == 1 &&isEnergyTorrent );
        _fullLevel.gameObject.SetActive(state==MergeItemStatus.open && MergeConfigManager.Instance.IsCanShowStar(tableMergeItem));

    }

    public void PlayAnimator(string name, bool isRefreshStatus = false)
    {
        if (isRefreshStatus)
        {
            if (!mergeDelayAnims.ContainsKey(name))
            {
                Coroutine coroutine = StartCoroutine(CommonUtils.PlayAnimation(animator, name, "", () =>
                {
                    RefreshOutPutStatus();
                    mergeDelayAnims.Remove(name);
                }));

                mergeDelayAnims.Add(name, coroutine);
                return;
            }
        }

        PlayAnimator(animator, name);
    }

    public void PlayTweenAnim(Vector3 dir)
    {
        if (_doTweenSequence != null)
        {
            if (_oldLocalPosition == _orgLocalPosition)
                return;
            
            _doTweenSequence.Kill();
        }

        transform.localPosition = _orgLocalPosition;
        _oldLocalPosition = _orgLocalPosition;
        Vector3 oldPosition = transform.position;
        
        Vector3 targetPos = _orgLocalPosition + dir.normalized * 15;
        _doTweenSequence = DOTween.Sequence();
        _doTweenSequence.Append(transform.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutCubic));
        _doTweenSequence.Append(transform.DOLocalMove(_orgLocalPosition, 0.6f).SetEase(Ease.Linear));
        _doTweenSequence.Insert(0, transform.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.3f).SetEase(Ease.OutCubic));
        _doTweenSequence.Insert(0.3f, transform.DOScale(new Vector3(1, 1, 1f), 0.6f).SetEase(Ease.Linear));
        _doTweenSequence.SetLoops(-1);
        _doTweenSequence.OnUpdate(() =>
        {
            obj_lock.transform.position = oldPosition;  
            obj_lock_happygo.transform.position = oldPosition;  
        });
        _doTweenSequence.Play();
    }

    public void StopTweenAnim()
    {
        if (_doTweenSequence != null)
        {
            _doTweenSequence.Kill();
            _doTweenSequence = null;
            transform.transform.localScale = Vector3.one;
            transform.localPosition = _orgLocalPosition;
            obj_lock.transform.position = transform.position;
            obj_lock_happygo.transform.position = transform.position;
        }
    }

    private void OnDestroy()
    {
        if (_doTweenSequence != null)
        {
            _doTweenSequence.Kill();
            _doTweenSequence = null;
        }
    }

    public void PlayAnimator(Animator anim, string name)
    {
        if (anim == null)
            return;

        anim.Play(name, 0, 0);
    }

    private void UpdateImageColor()
    {
        if (state == MergeItemStatus.box || state == MergeItemStatus.locked)
            image_icon.material = ui_grey;
        else
            image_icon.material = null;
    }

    public GameObject CloneIconGameObject()
    {
        return Instantiate(image_icon.gameObject);
    }

    public Vector3 GetIconPosition()
    {
        return image_icon.transform.position;
    }

    public void Reset()
    {
        obj_smoke.gameObject.SetActive(false);
        obj_icon.gameObject.SetActive(false);
        obj_box.gameObject.SetActive(false);
        obj_box_happygo.gameObject.SetActive(false);
        obj_energy.gameObject.SetActive(false);
        obj_energyTorrent.gameObject.SetActive(false);
        obj_lock.gameObject.SetActive(false);
        obj_lock_happygo.gameObject.SetActive(false);
        obj_bubble.gameObject.SetActive(false);
        obj_task.gameObject.SetActive(false);
        obj_dailyTask.gameObject.SetActive(false);
        obj_garageCleanup.gameObject.SetActive(false);
        obj_orderTask.gameObject.SetActive(false);
        obj_time.gameObject.SetActive(false);
        obj_mc_time.gameObject.SetActive(false);
        obj_vfx_speed.gameObject.SetActive(false);
        obj_timeMask.gameObject.SetActive(false);
        obj_power.gameObject.SetActive(false);
        obj_merge.gameObject.SetActive(false);
        obj_output.gameObject.SetActive(false);
        obj_output_special.gameObject.SetActive(false);
        vfx_increase.gameObject.SetActive(false);
        obj_ring.gameObject.SetActive(false);
        obj_tap.gameObject.SetActive(false);
        obj_hint.gameObject.SetActive(false);
        obj_openTips.gameObject.SetActive(false);
        obj_taskBg.gameObject.SetActive(false);
        obj_bubbleGroup.gameObject.SetActive(false);
        vfx_transition_01.gameObject.SetActive(false);
        _eatSlider.gameObject.SetActive(false);
        obj_balloonRacing.gameObject.SetActive(false);
        obj_rabbitRacing.gameObject.SetActive(false);
        image_icon.transform.localScale = Vector3.one;
        animator.Play("Normal", 0, 0);
    }


    public void RefreshAllStatus()
    {
        CancelInvoke("RefreshBubbleStatus");
        CancelInvoke("RefreshCdStatus");
        //CancelInvoke("RefreshHamsterBubble");
        RefreshOutPutStatus();

        obj_bubble.gameObject.SetActive(false);
        obj_balloonRacing.gameObject.SetActive(false);
        obj_rabbitRacing.gameObject.SetActive(false);
        if (state == MergeItemStatus.bubble)
        {
            obj_bubble.gameObject.SetActive(true);
            obj_balloonRacing.gameObject.SetActive(BalloonRacingModel.Instance.IsShowReward());
            obj_rabbitRacing.gameObject.SetActive(RabbitRacingModel.Instance.IsShowReward());
            anim_bubble.Play("loop");
            InvokeRepeating("RefreshBubbleStatus", 0f, 2f);
        }

        // if (tableMergeItem != null && tableMergeItem.type == (int)MergeItemType.hamaster)
        // {
        //     InvokeRepeating("RefreshHamsterBubble", 0f, 0.3f);
        // }
        RefreshSlider();
        SetCdStatus(false);
        if (IsInProductCD())
        {
            SetCdStatus(true);
            InvokeRepeating("RefreshCdStatus", 0f, 1f);
        }
    }

    public void RefreshSlider()
    {
        if (tableMergeItem == null)
            return;
        if(tableMergeItem.type!=(int)MergeItemType.hamaster && tableMergeItem.type!=(int)MergeItemType.eatBuild)
            return ;
            
        if (storageMergeItem.State == 1)
        {
            _eatSlider.gameObject.SetActive(false);
           
            return;
        }

        if (storageMergeItem.State == 3)
        {
            _eatSlider.gameObject.SetActive(true);
           _eatSlider.value=MergeManager.Instance.GetEatProgress(storageMergeItem, tableMergeItem);
           var produceCost = MergeManager.Instance.GetProduceCost(tableMergeItem);
           _eatBackGround.gameObject.SetActive(produceCost != null && produceCost.Length >= 2);
        }

    }
    private bool _isShowBubble = false;
    private int _biscuitId = -1;
    public void RefreshHamsterBubble()
    {
        if (tableMergeItem == null)
            return;
            
        if(tableMergeItem.type!=(int)MergeItemType.hamaster && tableMergeItem.type!=(int)MergeItemType.eatBuild)
            return ;
        if (storageMergeItem.State == 1)
        {
            obj_bubbleGroup.gameObject.SetActive(false);
           
            return;
        }
        
        if(_isShowBubble)
            return;
        
        if (storageMergeItem.State == 3 )
        {
            
            obj_bubbleGroup.gameObject.SetActive(false);
            obj_bubbleGroup.gameObject.SetActive(true);

            image_bubble.gameObject.SetActive(false);
            image_bubble1.gameObject.SetActive(false);
            
            var produceCost = MergeManager.Instance.GetProduceCost(tableMergeItem);
            if (tableMergeItem.subType == (int)SubType.Matreshkas)
            {
                if (!MatreshkasModel.Instance.IsOpened() || MatreshkasModel.Instance.IsTimeEnd())
                {
                    obj_bubbleGroup.gameObject.SetActive(false);
                    return;
                }
            }
                
            if (produceCost!=null && produceCost.Length > 0)
            {
                TableMergeItem mergeItem1= GameConfigManager.Instance.GetItemConfig(produceCost[0]);
                if (mergeItem1 != null)
                {
                    image_bubble.gameObject.SetActive(!MergeManager.Instance.IsBuildEat(storageMergeItem,mergeItem1.id));
                    if (image_bubble.sprite == null || image_bubble.sprite != null && image_bubble.sprite.name != mergeItem1.image)
                        image_bubble.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem1.image);
                }

                if (produceCost.Length > 1)
                {
                    TableMergeItem mergeItem2= GameConfigManager.Instance.GetItemConfig(produceCost[1]);
                    if (mergeItem2 != null)
                    {
                        image_bubble1.gameObject.SetActive(!MergeManager.Instance.IsBuildEat(storageMergeItem,mergeItem2.id));
                        if (image_bubble1.sprite == null || image_bubble1.sprite != null && image_bubble1.sprite.name != mergeItem2.image)
                            image_bubble1.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem2.image);
                    }
                }
            }
            else
            {
                if(_biscuitId ==  HappyGoModel.Instance.GetRequestProductId())
                    return;
                TableMergeItem mergeItem =  HappyGoModel.Instance.GetBiscuitMergeTable();
                if (mergeItem != null)
                {
                    if (image_bubble.sprite == null || image_bubble.sprite != null && image_bubble.sprite.name != mergeItem.image)
                        image_bubble.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
                }
            }

            _isShowBubble = true;
            StartCoroutine(CommonUtils.DelayWork(2, () =>
            {
                _isShowBubble = false;
            }));
        }
    }
    private bool CanUpdate()
    {
        if (id == -1)
            return false;

        if (_state == MergeItemStatus.box || _state == MergeItemStatus.locked)
            return false;

        if (tableMergeItem == null)
            return false;
        
        var produceCost = MergeManager.Instance.GetProduceCost(tableMergeItem);
        if (produceCost != null && produceCost.Length > 0 && storageMergeItem.State == 3)
            return false;
        bool isOpen = MergeManager.Instance.IsOpen(storageMergeItem);
        if (!isOpen)
            return false;

        return true;
    }

    public void BubbleBreak()
    {
        var boardItem = MergeManager.Instance.GetBoardItem(index,BoardId);
        if (boardItem.State != 2)
        {
            obj_bubble.gameObject.SetActive(false);
            CancelInvoke("RefreshBubbleStatus");
            return;
        }

        PlayAnimator(anim_bubble, "disappear_over");
        CancelInvoke("RefreshBubbleStatus");
        StartCoroutine(CommonUtils.PlayAnimation(animator, "small", "", () =>
        {
            transform.localScale = Vector3.one;
            var breakProduct= GlobalConfigManager.Instance.GetNumValue("bubble_break_product");
            if (breakProduct <= 0)
                breakProduct = 10203;
            MergeManager.Instance.SetBoardItem(index, breakProduct, 1, RefreshItemSource.notDeal,BoardId);
            PlayAnimator("object_appear", false);
            SendBubbleBreakIn();
            PlaySmoke();
            AudioManager.Instance.PlaySound(18);
        }));
    }
    
    private void RefreshBubbleStatus()
    {
        if (storageMergeItem != MergeManager.Instance.GetBoardItem(index,BoardId))
            return;
        var boardItem = MergeManager.Instance.GetBoardItem(index,BoardId);
        if (boardItem.State != 2)
        {
            obj_bubble.gameObject.SetActive(false);
            obj_balloonRacing.gameObject.SetActive(false);
            obj_rabbitRacing.gameObject.SetActive(false);
            CancelInvoke("RefreshBubbleStatus");
            return;
        }

        int cd = MergeManager.Instance.GetBubbleLeftCdTime(index,BoardId);
        if (cd > 0)
        {
            obj_balloonRacing.gameObject.SetActive(BalloonRacingModel.Instance.IsShowReward());
            obj_rabbitRacing.gameObject.SetActive(RabbitRacingModel.Instance.IsShowReward());
            return;
        }
        
        obj_balloonRacing.gameObject.SetActive(false);
        obj_rabbitRacing.gameObject.SetActive(false);
        
        PlayAnimator(anim_bubble, "disappear_over");
        CancelInvoke("RefreshBubbleStatus");
        StartCoroutine(CommonUtils.PlayAnimation(animator, "small", "", () =>
        {
            transform.localScale = Vector3.one;
            var breakProduct= GlobalConfigManager.Instance.GetNumValue("bubble_break_product");
            if (breakProduct <= 0)
                breakProduct = 10203;
            MergeManager.Instance.SetBoardItem(index, breakProduct, 1, RefreshItemSource.notDeal,BoardId);
            PlayAnimator("object_appear", false);
            SendBubbleBreak();
            PlaySmoke();
            AudioManager.Instance.PlaySound(18);
        }));
    }

    private void RefreshOutPutStatus()
    {
        if (!CanUpdate())
        {
            SetOutputActive(false);
            return;
        }

        if (storageMergeItem.State == 0 || MergeManager.Instance.IsActiveItem(storageMergeItem))
        {
            SetOutputActive(false);
            return;
        }

        float percent = 0;
        int activeCd = MergeManager.Instance.GetLeftActiveTime(storageMergeItem, tableMergeItem, ref percent);
        if (activeCd > 0)
        {
            SetOutputActive(false);
            return;
        }

        if (isTimeProductItem)
        {
            List<int> items = MergeManager.Instance.GetTimeProductItems(index,BoardId);
            if (items.Count > 0)
            {
                obj_energy.gameObject.SetActive(false);
                obj_energyTorrent.gameObject.SetActive(false);
                obj_output.gameObject.SetActive(true);
                if (MasterCardModel.Instance.IsBuyMasterCard)
                {
                    vfx_increase.gameObject.SetActive(true);
                }
                else
                {
                    vfx_increase.gameObject.SetActive(false);
                }

                animator.Play("object_output");
            }
            else
            {
                SetOutputActive(false);
            }
        }

        int leftCount = MergeManager.Instance.GetLeftProductCount(index, tableMergeItem,BoardId);
        if (leftCount > 0)
        {
            SetOutputActive(true);
            animator.Play("object_output");
        }
        else
        {
            if (!isTimeProductItem)
                SetOutputActive(false);
        }
    }

    private void SetOutputActive(bool active)
    {
        bool isDeath = MergeConfigManager.Instance.IsLimitNoCdProductItem(tableMergeItem);

        bool IsUnlimitedProduct = MergeConfigManager.Instance.IsEnergyProductItem(tableMergeItem)
                                    && MergeManager.Instance.IsInUnlimitedProduct(_boardId)&&!isDeath ;
        obj_output.gameObject.SetActive(active && !IsUnlimitedProduct);
        obj_output_special.gameObject.SetActive(active && IsUnlimitedProduct);
        if (MasterCardModel.Instance.IsBuyMasterCard)
        {
            vfx_increase.gameObject.SetActive(active);
        }
        else
        {
            vfx_increase.gameObject.SetActive(false);
        }
        bool isEnergyTorrent=MergeConfigManager.Instance.IsEnergyTorrentProduct(tableMergeItem) &&
                             EnergyTorrentModel.Instance.IsOpen();
        obj_energy.gameObject.SetActive(active && isEnergyProduct  && !isEnergyTorrent);
        obj_energyTorrent.gameObject.SetActive(active && isEnergyProduct && isEnergyTorrent);
        if (!active)
            animator.Play("Normal");
    }

    public bool IsInProductCD()
    {
        if (!CanUpdate())
            return false;

        if (!isProductItem && !isTimeProductItem)
            return false;
    
        float productPercent = 0;
        int cdTime = MergeManager.Instance.GetLeftProductTime(index, tableMergeItem, ref productPercent,BoardId);
        if (cdTime > 0)
            return true;

        cdTime = MergeManager.Instance.GetLeftActiveTime(storageMergeItem, tableMergeItem, ref productPercent);
        if (cdTime > 0)
            return true;

        if (isTimeProductItem)
        {
            cdTime = MergeManager.Instance.GetTimeProductCd(index, tableMergeItem, ref productPercent,BoardId);
            if (cdTime > 0 && storageMergeItem.ProductItems.Count <= 0)
                return true;
        }

        return false;
    }

    private void RefreshCdStatus()
    {
        if (storageMergeItem != MergeManager.Instance.GetBoardItem(index,BoardId))
            return;
        if (!CanUpdate())
        {
            CancelInvoke("RefreshCdStatus");
            SetCdStatus(false);
            RefreshAllStatus();
            return;
        }

        if (!isProductItem && !isTimeProductItem)
        {
            CancelInvoke("RefreshCdStatus");
            SetCdStatus(false);
            RefreshAllStatus();
            return;
        }

        float productPercent = 0;
        int productCd = MergeManager.Instance.GetLeftProductTime(index, tableMergeItem, ref productPercent,BoardId);
        if (productCd > 0)
        {
            image_time.fillAmount = 1 - productPercent;
            return;
        }

        productCd = MergeManager.Instance.GetLeftActiveTime(storageMergeItem, tableMergeItem, ref productPercent);
        if (productCd > 0)
        {
            image_time.fillAmount = 1 - productPercent;
            return;
        }

        if (isTimeProductItem)
        {
            productCd = MergeManager.Instance.GetTimeProductCd(index, tableMergeItem, ref productPercent,BoardId);
            List<int> items = MergeManager.Instance.GetTimeProductItems(index,BoardId);

            if (items.Count == 0 && productCd > 0 && storageMergeItem.ProductItems.Count <= 0)
            {
                image_time.fillAmount = 1 - productPercent;
                return;
            }
        }

        CancelInvoke("RefreshCdStatus");
        SetCdStatus(false);
        RefreshAllStatus();
    }

    private void OnDisable()
    {
        CancelInvoke("RefreshCdStatus");
        CancelInvoke("RefreshBubbleStatus");

        StopAllCoroutines();

        mergeDelayAnims.Clear();
        obj_hint.gameObject.SetActive(false);

        _isShowBubble = false;
        gameObject.transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        gameObject.transform.localScale = Vector3.one;
        RefreshAllStatus();
        UpdateIconImage();
        _isShowBubble = false;
    }

    public void PlayEatMergeAni()
    {
        vfx_boxHappyGo.gameObject.SetActive(true);
        StartCoroutine(CommonUtils.DelayWork(2, () =>
        {
            vfx_boxHappyGo.gameObject.SetActive(false);
        }));
    }
    public void SetCdStatus(bool status)
    {
        obj_time.gameObject.SetActive(status);

        if (MasterCardModel.Instance.IsBuyMasterCard)
        {
            obj_mc_time.gameObject.SetActive(status);
            obj_vfx_speed.gameObject.SetActive(status);
        }
        else
        {
            obj_mc_time.gameObject.SetActive(false);
            obj_vfx_speed.gameObject.SetActive(false);
        }

        obj_timeMask.gameObject.SetActive(status);
    }

    public void SetMergeStatus(bool status)
    {
        obj_merge.gameObject.SetActive(status);
    }


    Vector3[] path = new Vector3[3];

    public void DoProductTween(Vector3 oldPos, Vector3 newPos, RefreshItemSource source = RefreshItemSource.notDeal,
        Action tweenEndCall = null)
    {
        float dis = Vector3.Distance(oldPos, newPos);

        float time = 0.5f;
        float height = 80f;
        float distance = 60f;
        float moveTime = 0.3f;

        float maxSpace = 10;
        float minSpace = 5;

        float offset = dis / itemSize + 3;
        offset = Mathf.Clamp(offset, minSpace, maxSpace);
        float moveDis = distance / (maxSpace - offset + 1);
        float rate = 1.0f * offset / maxSpace;

        Vector3 tempPos = newPos - (newPos - oldPos).normalized * moveDis;
        path[0] = oldPos;
        path[1] = (oldPos + tempPos) * 0.5f + (Vector3.up * height);
        path[2] = tempPos;
        animator.enabled = false;
        transform.localPosition = oldPos;
        transform.localScale = Vector3.zero;
        transform.SetAsLastSibling();
     
        var sq = DOTween.Sequence();
        sq.Insert(0, this.transform.DOLocalPath(path, time, PathType.CatmullRom));
        sq.Insert(0, this.transform.DOScale(1.6f, time * 0.5f));
        //sq.Insert(time * 0.5f, this.transform.DOScale(new Vector3(1.1f, 0.8f, 1f), time * 0.5f).OnComplete(() =>
        sq.Insert(time * 0.5f, this.transform.DOScale(new Vector3(1f, 1f, 1f), time * 0.5f).OnComplete(() =>
        {
            animator.enabled = true;
            obj_smoke.gameObject.SetActive(false);
            obj_smoke.gameObject.SetActive(true);
            // StartCoroutine(CommonUtils.PlayAnimation(animator, "object_appear", "", () =>
            // {
            // }));
        }));
        sq.Insert(time, this.transform.DOLocalMove(newPos, moveTime * rate).SetEase(Ease.OutQuad));
        sq.Insert(time, this.transform.DOScale(new Vector3(1f, 1f, 1f), moveTime * rate).SetEase(Ease.OutQuad));
        sq.SetEase(Ease.InQuad);
        sq.OnComplete(() =>
        {
            RefreshAllStatus();
            vfx_trail.gameObject.SetActive(false);
            tweenEndCall?.Invoke();
        });
    }

    public void SetTaskStatus(bool status, bool bgStatus)
    {
        obj_taskBg.gameObject.SetActive(bgStatus);
        obj_task.gameObject.SetActive(status);
    }

    public void SetDailyTaskStatus(bool status)
    {
        // obj_taskBg.gameObject.SetActive(status);
        obj_dailyTask.gameObject.SetActive(status);
    }

    public void SetOrderTaskStatus(bool status)
    {
        obj_taskBg.gameObject.SetActive(status);
        obj_orderTask.gameObject.SetActive(status);
    }
    public void SetGarageCleanupStatus(bool status)
    {
        //obj_taskBg.gameObject.SetActive(status);
        obj_garageCleanup.gameObject.SetActive(status);
    }

    public void PlaySpeedUpAnimator(bool rv, Action cb = null)
    {
        obj_time.gameObject.SetActive(true);
        if (MasterCardModel.Instance.IsBuyMasterCard)
        {
            obj_mc_time.gameObject.SetActive(true);
            obj_vfx_speed.gameObject.SetActive(true);
        }

        var ani = MasterCardModel.Instance.IsBuyMasterCard ? mc_anim_time : anim_time;
        obj_timeMask.gameObject.SetActive(false);
        PlayHintEffect();

        StartCoroutine(CommonUtils.PlayAnimation(ani, "appear", null, () =>
        {
            cb?.Invoke();
            RefreshAllStatus();
        }));
    }

    public void PlayBoxBreakAnimtor()
    {
        obj_box.gameObject.SetActive(_boardId!=MergeBoardEnum.HappyGo);
        obj_box_happygo.gameObject.SetActive(_boardId==MergeBoardEnum.HappyGo);
        
        obj_bubble.gameObject.SetActive(state == MergeItemStatus.bubble);
        obj_lock.gameObject.SetActive(state == MergeItemStatus.locked && _boardId!=MergeBoardEnum.HappyGo);
        obj_icon.gameObject.SetActive(state != MergeItemStatus.box);
        bool isEnergyTorrent=MergeConfigManager.Instance.IsEnergyTorrentProduct(tableMergeItem) &&
                             EnergyTorrentModel.Instance.IsOpen();
        obj_energy.gameObject.SetActive(isEnergyProduct && storageMergeItem.State == 1 && !isEnergyTorrent);
        obj_energyTorrent.gameObject.SetActive(isEnergyProduct && storageMergeItem.State == 1 && isEnergyTorrent);
        Animator animator = _boardId == MergeBoardEnum.HappyGo ? anim_box_happygo : anim_box;
        if (_boardId == MergeBoardEnum.HappyGo)
        {
            obj_lock_happygo.gameObject.SetActive(state == MergeItemStatus.locked && _boardId==MergeBoardEnum.HappyGo);
            UpdateImageColor();
            StartCoroutine(CommonUtils.PlayAnimation(animator, "appear", "", () =>
            {
                UpdateData(false);
            }));
        }
        else
        {
            StartCoroutine(CommonUtils.PlayAnimation(animator, "appear", "", () =>
            {
                UpdateData(false);
            }));
        }
      
        
    }


    private void PlaySmoke()
    {
        obj_smoke.gameObject.SetActive(false);
        obj_smoke.gameObject.SetActive(true);
    }

    public void DoUseItem(int index)
    {
        //替换index  会出现index 和 当前存储的不一致么？
        index = this.index;

        var boardItem = MergeManager.Instance.GetBoardItem(index,BoardId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(boardItem.Id);
        var storeConfig = MergeManager.Instance.GetBoardItem(index,BoardId);
        bool isOpen = MergeManager.Instance.IsOpen(storeConfig);
        if (!isOpen || itemConfig == null)
            return;
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_USE_ITEM,
            (int)BoardId, index, id);
        switch (itemConfig.type)
        {
            case  (int) MergeItemType.item: //物品类
                break;
            case  (int) MergeItemType.box: //宝箱
                break;
            case (int) MergeItemType.energy: //体力
                AddCurrency(UserData.ResourceId.Energy, itemConfig, index);
                AudioManager.Instance.PlaySound(16);
                break;
            case (int) MergeItemType.decoCoin: //金币
                AddCurrency(UserData.ResourceId.Coin, itemConfig, index);
                AudioManager.Instance.PlaySound(15);
                break;
            case (int) MergeItemType.diamonds: //钻石
                AddCurrency(UserData.ResourceId.Diamond, itemConfig, index);
                AudioManager.Instance.PlaySound(17);
                break;
            case (int) MergeItemType.exp: //经验
                AudioManager.Instance.PlaySound(14);
                AddCurrency(UserData.ResourceId.Exp, itemConfig, index);
                break;
            case (int)MergeItemType.dogCookies: //小狗饼干
            { 
                AudioManager.Instance.PlaySound(14);
                AddDogCookies(itemConfig, index);
                break;
            } 
            case (int)MergeItemType.Parrot: //鹦鹉
            { 
                if(GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem) && GuideSubSystem.Instance.IsTargetParams(itemConfig.id.ToString()))
                    break;
                
                AudioManager.Instance.PlaySound(14);
                ParrotModel.Instance.AddParrot(this,itemConfig, index);
                break;
            }
            case (int)MergeItemType.FlowerField: //花田
            { 
                if(GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem) && GuideSubSystem.Instance.IsTargetParams(itemConfig.id.ToString()))
                    break;
                
                AudioManager.Instance.PlaySound(14);
                FlowerFieldModel.Instance.AddFlowerField(this,itemConfig, index);
                break;
            }
            case (int)MergeItemType.climbTreeBanana: //猴子
            { 
                if(GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem) && GuideSubSystem.Instance.IsTargetParams(itemConfig.id.ToString()))
                    break;
                
                AudioManager.Instance.PlaySound(14);
                ClimbTreeModel.Instance.AddClimbTreeBanana(this,itemConfig, index);
                break;
            } 
            case (int)MergeItemType.easter: //复活节活动
            { 
                AudioManager.Instance.PlaySound(14);
                AddEasterCoin(itemConfig, index);
                break;
            } 
            case (int)MergeItemType.choiceChest: //自选宝箱
            { 
                AudioManager.Instance.PlaySound(14);
                OneselfChoseLogic.OpenChoseUI(itemConfig.id, index);
                break;
            }    
            case (int)MergeItemType.HappyGoExp: 
            { 
                MergeManager.Instance.RemoveBoardItem(index,MergeBoardEnum.HappyGo,"HappyGo");
                HappyGoMergeClickTips.Instance.SetNoFocusStatus();
                ShakeManager.Instance.ShakeLight();
                HappyGoModel.Instance.AddExp(itemConfig.value);
                AudioManager.Instance.PlaySound(14);
                Transform target = HappyGoMainController.Instance._happyGoReward.Icon1Trans;
                FlyGameObjectManager.Instance.FlyObject(index, itemConfig.id, transform.position, target, 0.8f, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(HappyGoMainController.Instance._happyGoReward.Icon1Trans.position);
                    EventDispatcher.Instance.DispatchEvent(MergeEvent.HAPPYGO_EXP_REFRESH,itemConfig.value);
                });
             
                break;
            }
            case (int) MergeItemType.timeBooster: // 减 cd
                ShakeManager.Instance.ShakeLight();
                ulong time = (ulong) itemConfig.booster_factor * 60;
                MergeManager.Instance.SpeedUpAllItem(time,BoardId);
                MergeMainController.Instance.MergeBoard.RefreshGridsStatus();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);
                
                
                MergeManager.Instance.RemoveBoardItem(index,BoardId,"timeBooster");
                DebugUtil.LogError("使用减cd道具");
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);

                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeUseTimeskipbooster,
                    itemAId = itemConfig.id,
                    ItemALevel = itemConfig.level,
                    isChange = false,
                    extras = new Dictionary<string, string>
                    {
                        {"time", time.ToString()},
                    }
                });
                break;      
            case (int) MergeItemType.EnergyUnlimited: // 无限体力 
                
                if (boardItem.StackNum <= 1)
                {
                    MergeManager.Instance.RemoveBoardItem(index,BoardId,"energyunlimited");
                }
                else
                {
                    boardItem.StackNum--;
                }
                

                EnergyModel.Instance.AddEnergyUnlimitedTime(itemConfig.value*1000,new  GameBIManager.ItemChangeReasonArgs()
                {
                    
                } );
                var target1=CurrencyGroupManager.Instance.currencyController.GetIconTransform(UserData.ResourceId.Energy);
                for (int i = 0; i < 10; i++)
                {
                    FlyGameObjectManager.Instance.FlyObject(target1.gameObject, transform.position, target1.position, false, 0.8f, 0.15f * i, () =>
                    {
                        CurrencyGroupManager.Instance.PlayShakeAnim(CurrencyGroupManager.Instance.currencyController, UserData.ResourceId.Energy);
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(target1.position);
                        ShakeManager.Instance.ShakeLight();
                    });
                }
                
                break;     
            case (int) MergeItemType.NoCd: //无限产出
                if (boardItem.StackNum <= 1)
                {
                    MergeManager.Instance.RemoveBoardItem(index,BoardId,"nocd");
                }
                else
                {
                    boardItem.StackNum--;
                }
                MergeManager.Instance.AddUnlimitedProductTime(itemConfig.value*1000,BoardId);
               
                break;      
            case (int) MergeItemType.ButterFly:
                MergeManager.Instance.RemoveBoardItem(index,BoardId,"butterfly");
                break;
        }
    }

    private void AddCurrency(UserData.ResourceId type, TableMergeItem tableMerge, int index)
    {
        if (tableMerge == null)
            return;

        SendHarvestBi();
        MergeManager.Instance.RemoveBoardItem(index,BoardId,"AddCurrency");
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);
    
        ShakeManager.Instance.ShakeLight();

        UserData.Instance.AddRes((int) type, tableMerge.value,
            new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HarvestMergeItem},
            false);

        int resNum = UserData.Instance.GetRes(type);
        FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
            type, tableMerge.value, transform.position, 0.8f, false, false, 0.15f, () =>
            {
                CurrencyGroupManager.Instance.currencyController.CheckLevelUp(null);
                MergeGuideLogic.Instance.ChoseCdProduct();
            });
        
    }
      private void AddEasterCoin(TableMergeItem tableMerge, int index)
    {
        if (tableMerge == null)
            return;
        if (!EasterModel.Instance.IsOpened() || EasterModel.Instance.StorageEaster.IsManualActivity)
            return;
        if (EasterModel.Instance.IsMax())
            return;
        SendHarvestBi();
        MergeManager.Instance.RemoveBoardItem(index,BoardId,"AddEasterCoin");
        // if (!UserData.Instance.IsResource(tableMerge.id))
        // {
        //     GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        //     {
        //         MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonEasterReward,
        //         itemAId = tableMerge.id,
        //         isChange = true,
        //            
        //     });
        // }
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);

        ShakeManager.Instance.ShakeLight();

        int oldIndex = EasterModel.Instance.GetCurIndex();
    
        EasterModel.Instance.AddScore(tableMerge.value);
        
        Transform target = MergeTaskTipsController.Instance._mergeEaster.transform;
        FlyGameObjectManager.Instance.FlyObject(index, tableMerge.id, transform.position, target, 0.8f, () =>
        {
            ShakeManager.Instance.ShakeLight();
            FlyGameObjectManager.Instance.PlayHintStarsEffect(target.position);
            MergeTaskTipsController.Instance._mergeEaster.UpdateText( EasterModel.Instance.GetScore(), tableMerge.value, oldIndex, EasterModel.Instance.GetCurIndex(), 0.3f, () =>
            {
                //EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate, type);
            });
        });
    }
    private void AddDogCookies(TableMergeItem tableMerge, int index)
    {
        if (tableMerge == null)
            return;
        if (!DogHopeModel.Instance.IsOpenActivity())
            return;
        SendHarvestBi();
        MergeManager.Instance.RemoveBoardItem(index,BoardId,"AddDogCookies");
        if (!UserData.Instance.IsResource(tableMerge.id))
        {
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDogConsume,
                itemAId = tableMerge.id,
                isChange = true,
                   
            });
        }
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);

        ShakeManager.Instance.ShakeLight();

        int oldIndex = DogHopeModel.Instance.GetCurIndex();
        var config = DogHopeModel.Instance.GetCurIndexData();
        bool isComplete = DogHopeModel.Instance.IsComplete();
        DogHopeModel.Instance.AddScore(tableMerge.value);
        if (!isComplete && config != null)
        {
            if (DogHopeModel.Instance.GetScore() >= config.Score)
            {
                for (int i = 0; i < config.RewardId.Count; i++)
                {
                    if (!UserData.Instance.IsResource(config.RewardId[i]))
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDogEnter,
                            data1:config.RewardId[i].ToString(),data2:config.Id.ToString());
                    
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDog,
                            itemAId = config.RewardId[i],
                            isChange = true,
                   
                        });
                    }
                    UserData.Instance.AddRes(config.RewardId[i], config.RewardNum[i],
                        new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward}, false);
                }                    
            }
        }

        if (MergeTaskTipsController.Instance._mergeDogHope)
        { 
            Transform target = MergeTaskTipsController.Instance._mergeDogHope.transform;
            var dogScore = DogHopeModel.Instance.GetScore();
            var dogCurIndex = DogHopeModel.Instance.GetCurIndex();
            FlyGameObjectManager.Instance.FlyObject(index, tableMerge.id, transform.position, target, 0.8f, () =>
            {
                ShakeManager.Instance.ShakeLight();
                FlyGameObjectManager.Instance.PlayHintStarsEffect(target.position);
                MergeTaskTipsController.Instance._mergeDogHope.UpdateText( dogScore, tableMerge.value, oldIndex, dogCurIndex, 0.3f, () =>
                {
                    //EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate, type);
                });
            });   
        }
        else
        {
            DogHopeModel.Instance.CheckActivitySuccess();
            if (DogHopeModel.Instance.CanManualActivity())
            {
                DogHopeModel.Instance.EndActivity(true);
            }
        }
    }

    public void SpliteItem()
    {
        StartCoroutine(OnDecompos());
    }

    private IEnumerator OnDecompos()
    {

        var lastLevelItem = MergeConfigManager.Instance.GetLastLevelItemConfig(id);
        int gridIndex = MergeManager.Instance.FindEmptyGrid(index,BoardId);
        if (lastLevelItem == null || gridIndex == -1)
            yield return null;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        MergeManager.Instance.RemoveBoardItem(index,BoardId,"Decompos");
        MergeManager.Instance.SetNewBoardItem(index, lastLevelItem.id, 1, RefreshItemSource.product,BoardId, index);
        MergeManager.Instance.SetNewBoardItem(gridIndex, lastLevelItem.id, 1, RefreshItemSource.product,BoardId, index);
      
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeEventSplit,
            itemAId = itemConfig.in_line,
            ItemALevel = itemConfig.level,
            itemBId = lastLevelItem.in_line,
            itemBLevel = lastLevelItem.level,
            itemCId = lastLevelItem.in_line,
            itemCLevel = lastLevelItem.level,
            isLock = false,
            isChange = false,
        });
    }

    public void SendHarvestBi()
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeHarvest,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
        });
    }

    private void SendBubbleBreak()
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeBubbleBreak,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
        });
    }
    private void SendBubbleBreakIn()
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.GameEventBubbleIntentionalBreak,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
        });
    }

    private void SetTapImageStatus()
    {
        obj_tap.gameObject.SetActive(false);

        if (tableMergeItem == null || state != MergeItemStatus.open)
            return;

        bool isMax = false;
        switch (tableMergeItem.type)
        {
            case (int)MergeItemType.diamonds:
            case (int)MergeItemType.decoCoin:
            case (int)MergeItemType.energy:
            case (int)MergeItemType.exp:
            case (int)MergeItemType.dogCookies:
            case (int)MergeItemType.climbTreeBanana:
                if (tableMergeItem.next_level == -1)
                    isMax = true;
                break;
        }

        if (isMax)
            obj_tap.gameObject.SetActive(true);
        else
            obj_tap.gameObject.SetActive(false);
    }

    public void SetTapImageActive(bool isActive)
    {
        obj_tap.gameObject.SetActive(isActive);
    }

    private float[] _energyPowerScale = new[] { 0.68f, 0.76f, 0.84f, 0.92f, 1f };
    
    private void PlayPowerAnimator()
    {
        obj_power.gameObject.SetActive(false);

        if (tableMergeItem == null)
            return;
        if (tableMergeItem.type != (int)MergeItemType.energy)
            return;
        obj_power.gameObject.SetActive(true);

        float scale = _energyPowerScale[tableMergeItem.level-1];
        obj_power.transform.localScale = new Vector2(scale, scale);
        
        PlayAnimator(anim_power, "loop");
    }

    public void PlaySpilteVfx()
    {
    }

    private float speedTweenTime = 0.12f;

    public void DoTimeSpeedUpTween(Vector3 pos)
    {
        if (state == MergeItemStatus.box)
            return;
        var localPosition = transform.localPosition;
        Vector3 normal = Vector3.Normalize(localPosition + pos);
        Vector3 movePos = localPosition + normal * 15;
        Vector3 oldPos = localPosition;
        obj_ring.gameObject.SetActive(true);
        obj_ring.localScale = Vector3.one * 0.5f;
        var sq = DOTween.Sequence();
        sq.Insert(0, obj_ring.transform.DOScale(Vector3.one * 1.3f, speedTweenTime));
        sq.Insert(speedTweenTime, obj_ring.transform.DOScale(Vector3.one * 1f, speedTweenTime));
        sq.Insert(0, this.transform.DOLocalMove(movePos, speedTweenTime));
        sq.Insert(speedTweenTime, this.transform.DOLocalMove(oldPos, speedTweenTime));
        sq.Insert(speedTweenTime, obj_ring.GetComponentDefault<CanvasGroup>().DOFade(0, speedTweenTime));
        sq.OnComplete(() =>
        {
            obj_ring.gameObject.SetActive(false);
            obj_ring.GetComponentDefault<CanvasGroup>().alpha = 1;
        });
    }

    public void OnSellItem()
    {
        PlayAnimator("small");
        if (tableMergeItem == null)
            return;
        if (tableMergeItem.sold_gold > 0)
        {
            UserData.Instance.AddRes((int) UserData.ResourceId.Coin, tableMergeItem.sold_gold,
                new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SaleMergeItem},
                false,isIgnore:true);
            FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                UserData.ResourceId.Coin, tableMergeItem.sold_gold, transform.position, 0.8f, true, true, 0.15f);
        }
    }

    public void PlayBubbleAnimation(string aniName)
    {
        PlayAnimator(anim_bubble, aniName);
    }

    public void PlayHintEffect()
    {
        obj_hint.gameObject.SetActive(false);
        obj_hint.gameObject.SetActive(true);
    }  
    
    public void Debug_ShowIcon()
    {
        if (Debug.isDebugBuild)
        {
            obj_box.gameObject.SetActive(false);
            obj_box_happygo.gameObject.SetActive(false);
            obj_icon.gameObject.SetActive(true);
        }
    }
}