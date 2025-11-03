using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay.UI.EnergyTorrent;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public partial class UITrainOrderMainController
    {
        private EnergyTorrentBtn EnergyTorrentEntrance;
        public void InitEnergyTorrentBtn()
        {
            EnergyTorrentEntrance = transform.Find("Root/EnergyTorrentBtn").gameObject.AddComponent<EnergyTorrentBtn>();
        }
        public class EnergyTorrentBtn : MonoBehaviour
        {
            private Button _energyTorrentBtn;
            private GameObject _m1;
            private GameObject _m2;
            private GameObject _m4;
            private GameObject _m8;
            private LocalizeTextMeshProUGUI _energyTorrentTimeText;

            private void Awake()
            {
                _energyTorrentBtn = transform.GetComponent<Button>();
                _m1 = transform.Find("1").gameObject;
                _m2 = transform.Find("2").gameObject;
                _m4 = transform.Find("4").gameObject;
                _m8 = transform.Find("8").gameObject;
                _energyTorrentTimeText = transform.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();
                _energyTorrentBtn.onClick.AddListener(OnBtnEnergyTorrent);
                InvokeRepeating("RefreshEnergyTorrent",0,1);
                EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrentState);
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrentState);
            }

            private void OnEnable()
            {
                _energyTorrentBtn.gameObject.SetActive(EnergyTorrentModel.Instance.IsUnlock());
                UpdateStatus();
            }
            private void RefreshEnergyTorrent()
            {
                var leftTime = EnergyTorrentModel.Instance.GetLeftTime();
                if (leftTime <= 0)
                {
                    if (EnergyTorrentModel.Instance.StorageEnergyTorrent.MaxStartTime > 0)
                    {
                        EnergyTorrentModel.Instance.StorageEnergyTorrent.MaxStartTime = 0;
                        EnergyTorrentModel.Instance.ReSeStateX8();
                    }
                    else
                    {
                        if (VipStoreModel.Instance.VipLevel() < 5 && ((EnergyTorrentModel.Instance.StorageEnergyTorrent.Multiply == 8&&!EnergyTorrentModel.Instance.IsUnlock8Multiply())|| 
                                                                      ((EnergyTorrentModel.Instance.StorageEnergyTorrent.Multiply == 4&&!EnergyTorrentModel.Instance.IsUnlock4Multiply()))))
                        {
                            EnergyTorrentModel.Instance.SetCloseStateX8();
                        }
                    }
                }
                _energyTorrentTimeText.gameObject.SetActive(leftTime>0);
                _energyTorrentTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
            }
            private void OnBtnEnergyTorrent()
            {
                if (EnergyTorrentModel.Instance.IsUnlock4Multiply())
                {
                    if (EnergyTorrentModel.Instance.IsUnlock8Multiply() &&
                        EnergyTorrentModel.Instance.GetMultiply() == 4)
                    {
                        if (VipStoreModel.Instance.VipLevel() >= 5)
                        {
                            EnergyTorrentModel.Instance.SetOpenStateX8();
                            return;
                        }
                        else if (EnergyTorrentModel.Instance.GetLeftTime() > 0)
                        {
                            UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMainX8);
                            return;
                        }
                    }

                    EnergyTorrentModel.Instance.SetMultiply();

                    string content = EnergyTorrentModel.Instance.IsOpen()
                        ? string.Format(LocalizationManager.Instance.GetLocalizedString("ui_energy_frenzy_open_tips"),
                            EnergyTorrentModel.Instance.GetMultiply())
                        : "ui_energy_frenzy_close_tips";
                    var obj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/UIEnergyTorrentTips");
                    var instantiate = Instantiate(obj, UIRoot.Instance.mRoot.transform);

                    UIEnergyTorrentTipsController
                        controller = instantiate.AddComponent<UIEnergyTorrentTipsController>();
                    controller.PlayAnim(content,
                        () =>
                        {
                            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1,
                                () => { Destroy(controller.gameObject); }));
                        });
                }
                else
                {
                    if (EnergyTorrentModel.Instance.StorageEnergyTorrent.IsShowStart)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMain);
                    }
                    else
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentStart);
                    }
                }
            }
            
            private void UpdateStatus()
            {
                int m = EnergyTorrentModel.Instance.GetMultiply();
                if (m == 1)
                {
                    _m1.gameObject.SetActive(true);
                    _m2.gameObject.SetActive(false);
                    _m4.gameObject.SetActive(false);
                    _m8.gameObject.SetActive(false);
                }else if (m == 2)
                {
                    _m1.gameObject.SetActive(false);
                    _m2.gameObject.SetActive(true);
                    _m4.gameObject.SetActive(false);
                    _m8.gameObject.SetActive(false);
                }
                else if (m == 4)
                {
                    _m1.gameObject.SetActive(false);
                    _m2.gameObject.SetActive(false);
                    _m4.gameObject.SetActive(true);
                    _m8.gameObject.SetActive(false);
                }
                else
                {
                    _m1.gameObject.SetActive(false);
                    _m2.gameObject.SetActive(false);
                    _m4.gameObject.SetActive(false);
                    _m8.gameObject.SetActive(true);
                }
            }
            
            private void RefreshEnergyTorrentState(BaseEvent obj)
            {
                UpdateStatus();
            }
        }
    }
}