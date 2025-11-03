using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeEaster : MonoBehaviour
{
   private Image _needImage;
   private LocalizeTextMeshProUGUI _needText;
   private LocalizeTextMeshProUGUI _countDownTime;
   private LocalizeTextMeshProUGUI _lvText;
   private Button _buttonFinish;
   private SkeletonGraphic _skeletonGraphic;
   private EasterReward _easterReward;
   private Slider _slider;
   private void Awake()
   {
      _buttonFinish = transform.Find("Finish").GetComponent<Button>();
      _buttonFinish.onClick.AddListener(() =>
      {
         UIManager.Instance.OpenUI(UINameConst.UIEasterMain);
        
      });
      transform.GetComponent<Button>().onClick.AddListener((() =>
      {
         UIManager.Instance.OpenUI(UINameConst.UIEasterMain);
      }));
      
      _needImage = transform.Find("Icon").GetComponent<Image>();
      _needText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
      _lvText = transform.Find("Lv/Text").GetComponent<LocalizeTextMeshProUGUI>();
      _countDownTime= transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

      _skeletonGraphic = transform.Find("Person/PortraitSpine/PortraitSpine").GetComponent<SkeletonGraphic>();
      _slider = transform.Find("Slider").GetComponent<Slider>();
      InvokeRepeating("RefreshCountDown", 0, 1f);
      EventDispatcher.Instance.AddEventListener(EventEnum.EASTER_CLAIM,OnClaim);

   }

   private void OnClaim(BaseEvent obj)
   {
      InitUI();
   }

   private void OnEnable()
   {
      InitUI();
      StopAllCoroutines();
      PlaySkeletonAnimation("normal");
   }

   private void OnDestroy()
   {
      EventDispatcher.Instance.RemoveEventListener(EventEnum.EASTER_CLAIM,OnClaim);
   }

   private void InitUI()
   {
      if (!EasterModel.Instance.IsOpened())
         return;
      int stateScore = EasterModel.Instance.GetIndexStageScore();
      int curScore = Math.Max(0, EasterModel.Instance.GetCurStageScore());
      _easterReward = EasterModel.Instance.GetCurIndexData();
      if (EasterModel.Instance.IsMax())
      {
         _needText.SetTerm("UI_max");
      }
      else
      {
         _needText.SetText(curScore + "/" + stateScore);
      }
      _lvText.SetText(_easterReward.Id.ToString());
      _slider.value=(float) curScore / stateScore;
      _buttonFinish.gameObject.SetActive(EasterModel.Instance.IsHaveCanClaim());
   }

   private void RefreshCountDown()
   {
      _countDownTime.SetText(EasterModel.Instance.GetActivityLeftTimeString());
   }
   
   public void UpdateText(int newScore, int subNum, int oldIndex, int curIndex, float time, System.Action callBack = null)
   {
      PlayDogAnim();
      
      int oldValue = newScore - subNum;
      int newValue = 0;
      int stageScore = 0;
         
      if (curIndex > oldIndex)
      {
         var oldData = EasterModel.Instance.GetCurIndexData(oldIndex);
         stageScore = EasterModel.Instance.GetIndexStageScore(oldIndex);

         oldValue = stageScore - (subNum - (newScore - oldData.Score));
         newValue = stageScore;
      }
      else
      {
         stageScore = EasterModel.Instance.GetIndexStageScore(curIndex);
         var preData = EasterModel.Instance.GetPreIndexData(curIndex);
         int preScore = preData == null ? 0 : preData.Score;
         
         oldValue = newScore-preScore-subNum;
         newValue = newScore-preScore;
      }
      
      _needText.SetText(oldValue.ToString()+"/"+stageScore);
      
      oldValue = Math.Max(oldValue, 0);
      newValue = Math.Max(newValue, 0);

      var tempDogHopeData = _easterReward;
      DOTween.To(() => oldValue, x => oldValue = x, newValue, time).OnUpdate(() =>
      {
         _needText.SetText(oldValue.ToString()+"/"+stageScore);
         _slider.value=(float) oldValue / stageScore;
      }).OnComplete(() =>
      {
         if (curIndex > oldIndex || EasterModel.Instance.IsMax())
         {
            InitUI();
         }
         else
         {
            callBack?.Invoke();
         }
      });
   }

   private void PlayDogAnim()
   {
      StopAllCoroutines();
      float time = PlaySkeletonAnimation("happy");
      StartCoroutine(CommonUtils.DelayWork(time, () =>
      {
         PlaySkeletonAnimation("normal");
      }));
   }
   
   private float PlaySkeletonAnimation(string animName)
   {
      if(_skeletonGraphic == null)
         return 0;

      TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
      if(trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
         return trackEntry.AnimationEnd;
        
      _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
      _skeletonGraphic.Update(0);
      if (trackEntry == null)
         return 0;
      return trackEntry.AnimationEnd;
   }
}