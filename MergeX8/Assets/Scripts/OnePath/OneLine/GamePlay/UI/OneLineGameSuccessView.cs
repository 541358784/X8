// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using DragonPlus;
// using DragonPlus.Config.Game;
// using DragonPlus.Config.OneLine;
// using UnityEngine;
// using UnityEngine.UI;
// using DragonU3DSDK.Asset;
// using DragonU3DSDK.Network.API.Protocol;
// using Newtonsoft.Json;
// using TMPro;
//
// namespace OneLine
// {
//     public class OneLineGameSuccessViewParams : UIViewParam
//     {
//         public List<ItemData> rewardDatas;
//     }
//     
//     
//     [AssetAddress("UIOneLine/UIGameSuccess")]
//     public class OneLineGameSuccessView : UIPopup
//     {
//         [ComponentBinder("Root/BtnContinue")] private Button btnContinue;
//         [ComponentBinder("CloseButton")] private Button btnClose;
//         [ComponentBinder("PortraitItem")] private Transform _tran_avatar;
//         [ComponentBinder("Reward")] private Transform _tran_reward;
//         [ComponentBinder("TipsText")] private LocalizeTextMeshProUGUI _txt_desc;
//         [ComponentBinder("HeadShadow")] private Transform _tran_head;
//         [ComponentBinder("RoleGroup")] private ScrollRect _srt;
//         private Transform coinIcon, gemIcon;
//
//         public override Action EmptyCloseAction => OnClickOk;
//         private OneLineGameSuccessViewParams _viewParams;
//
//         public override void OnViewOpen(UIViewParam param)
//         {
//             base.OnViewOpen(param);
//             _viewParams = param as OneLineGameSuccessViewParams;
//
//             btnContinue.onClick.AddListener(OnClickContinue);
//             btnClose.onClick.AddListener(OnClickOk);
//             _tran_avatar.gameObject.SetActive(false);
//             _tran_reward.gameObject.SetActive(false);
//             GuideSubSystem.Instance.RegisterTarget(GuideTargetType.OnelineContinueBtn, btnContinue.transform);
//             if (Model.Instance.GetCurPlayLevel() == 1)
//                 GuideSubSystem.Instance.Trigger(GuideTrigger.OnelineSucOpen, "");
//
//             /* 如果是首次通过，会给通关奖励 */
//             if (ShowReward()) return;
//
//             /* 头像功能未解锁 , 所有头像锁住*/
//             if (!AvatarModel.Instance.IsActivity())
//             {
//                 int unlocklv = GameConfigManager.Instance.GlobalConfig.AvatarUnlock;
//                 _txt_desc.SetTerm("");
//                 _txt_desc.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_ASMR_16", unlocklv.ToString()));
//                 var allInfos = Model.Instance.GetAvatarInfos();
//                 ShowAvartar(allInfos, false);
//                 return;
//             }
//
//             int rewardAvatarId = Model.Instance.GetRewardedId();
//             /* 获得了头像 */
//             if (rewardAvatarId > 0)
//             {
//                 _txt_desc.SetTerm("UI_ASMR_8");
//                 _tran_avatar.gameObject.SetActive(true);
//                 var img = _tran_avatar.Find("Icon").GetComponent<Image>();
//                 img.sprite = ResUtil.GetIcon(rewardAvatarId);
//                 _tran_avatar.Find("Text").gameObject.SetActive(false);
//                 LocalizeTextMeshProUGUI txt_num = _tran_avatar.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>();
//                 txt_num.gameObject.SetActive(true);
//                 txt_num.SetText("1/1");
//                 _tran_avatar.Find("Status/Check").gameObject.SetActive(true);
//                 _tran_avatar.Find("Status/Lock").gameObject.SetActive(false);
//             }
//             else
//             {
//                 var allInfos = Model.Instance.GetAvatarInfos();
//                 var unLockInfos = allInfos.FindAll(o => o.status == 1);
//                 /* 奖池里有已解锁头像 */
//                 if (unLockInfos.Count > 0)
//                 {
//                     _txt_desc.SetTerm("UI_ASMR_9");
//                     ShowAvartar(unLockInfos, true);
//                     return;
//                 }
//
//                 /* 只有未解锁的头像 */
//                 var lockInfos = allInfos.FindAll(o => o.status == 0);
//                 if (lockInfos.Count > 0)
//                 {
//                     _txt_desc.SetTerm("UI_ASMR_11");
//                     ShowAvartar(lockInfos, false);
//                     return;
//                 }
//
//                 /* 所有头像已获得 */
//                 _txt_desc.SetTerm("UI_ASMR_12");
//                 _tran_head.gameObject.SetActive(true);
//             }
//         }
//
//         private void ShowAvartar(List<AvatarInfo> datas, bool unLocked)
//         {
//             for (int i = 0; i < datas.Count; ++i)
//             {
//                 GameObject go = GameObject.Instantiate(_tran_avatar.gameObject, _tran_avatar.parent);
//                 go.transform.Reset();
//                 go.SetActive(true);
//                 var img = go.transform.Find("Icon").GetComponent<Image>();
//                 img.sprite = ResUtil.GetIcon(datas[i].id);
//                 go.transform.Find("Text").gameObject.SetActive(unLocked);
//                 go.transform.Find("NumberText").gameObject.SetActive(unLocked);
//                 go.transform.Find("Status/Lock").gameObject.SetActive(!unLocked);
//             }
//         }
//
//         private bool ShowReward()
//         {
//             var rewardData = _viewParams.rewardDatas;
//             if (rewardData.Count <= 0) return false;
//
//             for (int i = 0; i < rewardData.Count; i++)
//             {
//                 GameObject instance = GameObject.Instantiate(_tran_reward.gameObject, _tran_reward.parent);
//                 var cfg = DragonPlus.Config.Game.GameConfigManager.Instance.GetItem(rewardData[i].id);
//                 instance.SetActive(true);
//                 
//                 Image icon = instance.transform.Find("Icon").GetComponent<Image>();
//                 icon.sprite = ResourcesManager.Instance.GetSpriteVariant(cfg.Atlas, cfg.Icon);
//                 var numberText = instance.transform.Find("NmberText").GetComponent<TextMeshProUGUI>();
//                 numberText.SetText(rewardData[i].cnt.ToString());
//             }
//
//             /* 不允许拖动 */
//             _srt.horizontal = false;
//             return true;
//         }
//
//         private void OnClickOk()
//         {
//             btnContinue.enabled = false;
//             FlyCallback();
//         }
//         
//         private void OnClickContinue()
//         {
//             var nextLevel = Model.Instance.GetNextPlayLevelId();
//             if (nextLevel == 0)
//             {
//                 OnClickOk();
//                 return;
//             }
//             var nextLevelStatus = Model.Instance.GetLevelStatus(nextLevel);
//             /* 未解锁，回大厅 */
//             if (nextLevelStatus.Equals(OneLineLevelStatus.Lock))
//             {
//                 OnClickOk();
//                 return;
//             }
//
//             /* 不能免费进下一关，回大厅 */
//             if (nextLevelStatus.Equals(OneLineLevelStatus.Passed) && !Model.Instance.CanEnterFree(nextLevel))
//             {
//                 OnClickOk();
//                 return;
//             }
//
//             DoViewClose();
//             GuideSubSystem.Instance.FinishCurrent(GuideTargetType.OnelineContinueBtn);
//             /* 进入下一关 */
//             Model.Instance.EnterLevel(nextLevel, 3);
//             /* 直接进入下一关要把当前关的奖励给发了*/
//             GetReward();
//             
//             MyMain.myGame.Fsm.ChangeState(FsmStateType.OneLine, new OneLineGameFsmParam()
//             {
//                 LevelId = nextLevel,
//             }, true);
//             var levelcfg = OneLineConfigManager.Instance.GetLevel(nextLevel);
//             if (levelcfg != null)
//                 BiUtil.SendGameEvent(
//                     BiEventMatchFrenzy.Types.GameEventType.GameEventLineEnterLevel,
//                     nextLevel.ToString(), levelcfg.LevelType.ToString(),
//                     Model.Instance.GetEnterTimes(nextLevel).ToString(),
//                     "continue"
//                 );
//         }
//
//
//         // private int flyCallbackTimes = 0;
//         private async void FlyCallback()
//         {
//             // if (flyCallbackTimes > 0) return;
//             // flyCallbackTimes++;
//
//             // await Task.Delay(1000);
//             
//             Framework.Main.Game.Fsm.ChangeState(FsmStateType.Decoration, new FsmParamDecoration(FsmStateType.ASMR)
//             {
//                 lastASMRData = _viewParams.rewardDatas
//             });
//             // await Task.Delay(500);
//             
//             UIViewSystem.Instance.Close(GetType());   
//         }
//         
//         private void GetReward()
//         {
//             if (PlayerPrefs.GetInt("ASMRResultExecuteLast", 1) == 1) return ;
//             PlayerPrefs.SetInt("ASMRResultExecuteLast", 1);
//             string tempASMRData = PlayerPrefs.GetString("ASMRResultData");
//             if (!string.IsNullOrEmpty(tempASMRData))
//             {
//                 List<ItemData> rewardData = JsonConvert.DeserializeObject<List<ItemData>>(tempASMRData);
//                 foreach (var itemData in rewardData)
//                 {
//                     var cfg = DragonPlus.Config.Game.GameConfigManager.Instance.GetItem(itemData.id);
//                     if (itemData.cnt <= 0) continue;
//                     CurrencyModel.Instance.AddRes((ResourceId) cfg.ResourceId, itemData.cnt, new BiUtil.ItemChangeReasonArgs()
//                     {
//                         reason = BiEventMatchFrenzy.Types.ItemChangeReason.LineStreak,
//                         data1 = Model.Instance.GetCurPlayLevel().ToString(),
//                     });
//                     EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)cfg.ResourceId));
//                 }
//             }
//             
//         }
//     }
// }