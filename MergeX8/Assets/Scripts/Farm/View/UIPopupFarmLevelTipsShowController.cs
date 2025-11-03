using System;
using System.Collections;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class UIPopupFarmLevelTipsShowController : UIWindow
    {
        private Transform _awardsRoot;
        private Button _getBtn;
        private GameObject _packageItem;
        private List<GameObject> tempGo = new List<GameObject>();
        private GameObject _vfxBoxReward;
        bool _isCanClick = true;
        private Animator _animator;
        private LocalizeTextMeshProUGUI _levelText;
        private Action _callFunc;

        private int _level;
        
        public override void PrivateAwake()
        {
            AudioManager.Instance.PlaySound(43);
            _packageItem = transform.Find("Root/ContentGroup/UnitList/Item").gameObject;
            _packageItem.gameObject.SetActive(false);

            Transform content = this.transform.Find("Root/ContentGroup");
            _awardsRoot = content.Find("UnitList");
            _getBtn = content.GetComponentDefault<Button>("Button");
            _animator = gameObject.GetComponent<Animator>();

            _levelText = transform.Find("Root/ContentGroup/StarGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

            _getBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/Button");
            _getBtn.onClick.AddListener(OnLevelUpClick);

            UIRoot.Instance.EnableEventSystem = true;
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _level = (int)objs[0];
            _level -= 1;
            _callFunc = null;
            if (objs.Length > 1)
                _callFunc = (Action)objs[1];
            
            InitAwards();
        }

        private void InitAwards()
        {
            foreach (var o in tempGo)
            {
                Destroy(o);
            }
            tempGo.Clear();
            _getBtn.gameObject.SetActive(true);
            _levelText.SetText((_level+1).ToString());

            var config = FarmConfigManager.Instance.TableFarmLevelList.Find(a=>a.Id == _level);
            if (config == null)
                return;
            for (int i = 0; i < config.RewardIds.Count; i++)
            {
                GameObject go = Instantiate(_packageItem, _awardsRoot);
                go.gameObject.SetActive(true);

                int id = config.RewardIds[i];
                
                if (UserData.Instance.IsFarmProp(id))
                {
                    var propConfig = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == id);
                    if(propConfig != null)
                        go.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(propConfig.Image);
                }
                else
                {
                    var productConfig = FarmConfigManager.Instance.TableFarmProductList.Find(a => a.Id == id);
                    if (productConfig != null)
                    {
                        go.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
                    }
                    else
                    {
                        go.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(config.RewardIds[i]);
                    }
                }
                
                go.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText("x" + config.RewardNums[i].ToString());
                tempGo.Add(go);
            }
        }

        public void OnLevelUpClick()
        {
            if (!_isCanClick)
                return;

            _isCanClick = false;

            StartCoroutine(LevelUpAnim());
        }

        IEnumerator LevelUpAnim()
        {
            var config = FarmConfigManager.Instance.TableFarmLevelList.Find(a=>a.Id == _level);
            if (config == null)
            {
                _callFunc?.Invoke();
                yield break;
            }

            _isCanClick = false;

            AudioManager.Instance.PlaySound(29);

            Transform endTrans = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                endTrans = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                endTrans = UIHomeMainController.mainController.MainPlayTransform;
            }

            yield return new WaitForSeconds(0.1f);
            canClickMask = true;
            ClickUIMask();

            for (int i = 0; i < config.RewardIds.Count; i++)
            {
                int id = config.RewardIds[i];
                int index = i;

                if (UserData.Instance.IsFarmProp(id) || FarmConfigManager.Instance.TableFarmProductList.Find(a => a.Id == id) != null)
                {
                    if(FarmModel.Instance.IsFarmModel())
                        endTrans = FarmModel.Instance.WarehouseTransform;
                    else
                        endTrans = UIHomeMainController.mainController.FarmTransform;
                }

                UIRoot.Instance.EnableEventSystem = false;
                if (id == (int)UserData.ResourceId.Coin)
                {
                    List<RewardData> rewardDatas = new List<RewardData>();
                    RewardData data = new RewardData();
                    data.UpdateReward(new ResData(id, config.RewardNums[i]));
                    data.gameObject = tempGo[i].gameObject;
                    rewardDatas.Add(data);
                    FlyGameObjectManager.Instance.FlyObject(rewardDatas, CurrencyGroupManager.Instance.currencyController, () =>
                    {
                        UIRoot.Instance.EnableEventSystem = true;
                    });
                }
                else
                {
                    FlyGameObjectManager.Instance.FlyObject(id, tempGo[i].transform.position, endTrans, 0.8f, 0.8f, () =>
                    {
                        if (index == config.RewardIds.Count - 1)
                        {
                            UIRoot.Instance.EnableEventSystem = true;
                            FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                            Animator shake = endTrans.transform.GetComponent<Animator>();
                            if (shake != null)
                                shake.Play("shake", 0, 0);
                            var root = endTrans.transform.Find("Root");
                            if (root != null)
                            {
                                Animator play_ani = root.GetComponent<Animator>();
                                if (play_ani != null)
                                    play_ani.Play("appear", 0, 0);
                            }
                            _callFunc?.Invoke();
                        }
                    }, true, 1.0f, 0.7f, true);
                }
            }
        }

        public override void ClickUIMask()
        {
            if (!canClickMask)
                return;

            canClickMask = false;
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        }
    }
}