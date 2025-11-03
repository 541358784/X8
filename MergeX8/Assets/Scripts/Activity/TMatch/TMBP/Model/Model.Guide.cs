//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月25日 星期三
//describe    :   
//-----------------------------------

using System;
using System.Collections.Generic;
using OutsideGuide;

namespace TMatch
{
    /// <summary>
    /// 引导
    /// </summary>
    public partial class TMBPModel
    {
        /// <summary>
        /// 开启bp入口引导
        /// </summary>
        /// <returns></returns>
        public bool StartBpEntranceGuide()
        {
            if (!IsOpened(false) || DecoGuideManager.Instance.GetGuideState(10142))
            {
                return false;
            }

            DecoGuideManager.Instance.UnRegisterUIGuide(1014201);

            //入口按钮
            DecoGuideManager.Instance.RegisterUIGuide(1014201, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UIManager.Instance.GetOpenedWindow<UILobbyView>().MainView.bpEntranceButton,
                    eventAction = () =>
                    {
                        DecoGuideManager.Instance.SaveGuide(10142);
                        TM_BPMainView.Open();
                    }
                }
            });

            DecoGuideManager.Instance.StartGuide(10142);
            return true;
        }

        /// <summary>
        /// 开启引导
        /// </summary>
        public void StartBpViewGuide()
        {
            if (!IsOpened(false)) return;
            
            DecoGuideManager.Instance.UnRegisterUIGuide(1014301);
            DecoGuideManager.Instance.UnRegisterUIGuide(1014302);
            DecoGuideManager.Instance.UnRegisterUIGuide(1014303);
            DecoGuideManager.Instance.UnRegisterUIGuide(1014304);
            
            TM_BPMainView bpView = UIManager.Instance.GetOpenedWindow<TM_BPMainView>();

            //列表滑动
            DecoGuideManager.Instance.RegisterUIGuide(1014301, new List<GuideStepData>
            {
                new GuideStepData
                {
                    //滑动浏览
                    targetButtonAction = () =>
                    {
                        bpView.comMain.PlayGuideListAnim(() =>
                        {
                            //第一个免费奖励
                            DecoGuideManager.Instance.RegisterUIGuide(1014302, new List<GuideStepData>
                            {
                                new GuideStepData
                                {
                                    target = bpView.comMain.GuideFreeItem.freeItem.transform,
                                    eventAction = () => { bpView.comMain.GuideFreeItem.freeItem.OnClick(); }
                                }
                            });

                            //经验条
                            DecoGuideManager.Instance.RegisterUIGuide(1014303, new List<GuideStepData>
                            {
                                new GuideStepData
                                {
                                    target = bpView.ExpTrans,
                                }
                            });

                            //激活
                            DecoGuideManager.Instance.RegisterUIGuide(1014304, new List<GuideStepData>
                            {
                                new GuideStepData
                                {
                                    target = bpView.btnBuy.transform,
                                    eventAction = () => { TM_BPBuyView.Open(); }
                                }
                            });
                            
                            DecoGuideManager.Instance.CompleteStep();
                        });
                    }
                }
            });

            DecoGuideManager.Instance.StartGuide(10143);
        }
    }
}