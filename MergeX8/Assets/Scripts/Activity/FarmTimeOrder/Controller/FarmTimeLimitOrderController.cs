using DragonPlus;
using DragonPlus.Config.FarmTimeOrder;
using Farm.Model;
using Framework;
using UnityEngine.UI;

namespace Activity.FarmTimeOrder.FarmTimeLimitOrderController
{
    public class FarmTimeLimitOrderController: UIWindowController
    {
        private Button _closeButton;
    
        public override void PrivateAwake()
        {
            _closeButton = transform.Find("Root/Button").GetComponent<Button>();
            _closeButton.onClick.AddListener(()=>
            {
                AnimCloseWindow(() =>
                {
                    if (FarmModel.Instance.IsFarmModel())
                    {
                        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                        {
                            BackHomeControl.EnterFarm = true;
                            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
                        }
                    }
                    else
                    {
                        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                        {
                            BackHomeControl.EnterFarm = true;
                            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
                        }
                        else
                        {
                            SceneFsm.mInstance.ChangeState(StatusType.EnterFarm);
                        }
                    }
                });
            });

            var text = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            text.SetText("");

            if (FarmTimeLimitOrderModel.Instance.IsOpened())
            {
                var time = FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[0].OpenTime * 1000;
                text.SetText( CommonUtils.FormatLongToTimeStr(time));
            }
        }
    }
}