using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;
using Farm.Model;
using UnityEngine.EventSystems;

namespace Farm.View
{
    public partial class UIFarmMainController
    {
        private FarmType _farmType = FarmType.None;
        private DecoNode _selectNode = null;
        
        private void RegisterEvent()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_SHOW_NODE_BUY, Event_OnShowNodeBuy);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_TOUCH_NODE, Event_TouchNode);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_CANCEL_TOUCH_NODE, Event_CancelTouchNode);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_SPEED_UP, Event_SpeedUp);
        }

        private void UnRegisterEvent()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_SHOW_NODE_BUY, Event_OnShowNodeBuy);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_TOUCH_NODE, Event_TouchNode);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_CANCEL_TOUCH_NODE, Event_CancelTouchNode);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_SPEED_UP, Event_SpeedUp);
        }
        
        private void Event_OnShowNodeBuy(BaseEvent e)
        {
            DecoNode node = (DecoNode)e.datas[0];
            if(DecoManager.Instance.CurrentWorld.SelectedNode == null)
                return;
        
            _fullClose.gameObject.SetActive(true);
        
            GetCombineMono<UIFarmMain_Buy>().UpdateData(node);
            
            PlayerManager.Instance.UpdatePlayersState(node, 0.1f);
            
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Contrl, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false);

            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
            
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, true);
        }

        private void Event_TouchNode(BaseEvent e)
        {
            Event_CancelTouchNode(null);
            
            DecoNode node = (DecoNode)e.datas[0];
            if (node == null)
            {
                _farmType = FarmType.None;
                return;
            }

            _selectNode = node;
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            _farmType = type;
            
            if(!_selectNode.IsOwned)
                return;
            
            if(!FarmModel.Instance.IsUnLockNode(_selectNode))
                return;
            
            switch (type)
            {
                case FarmType.Animal:
                {
                    TouchAnimal(node);
                    break;
                }
                case FarmType.Ground:
                {
                    TouchGround(node);
                    break;
                }
                case FarmType.Tree:
                {
                    TouchTree(node);
                    break;
                }
                case FarmType.Machine:
                {
                    TouchMachine(node);
                    break;
                }
            }
        }

        private void Event_CancelTouchNode(BaseEvent e)
        {
            switch (_farmType)
            {
                case FarmType.Animal:
                {
                    CancelTouchAnimal(_selectNode);
                    break;
                }
                case FarmType.Ground:
                {
                    CancelTouchGround(_selectNode);
                    break;
                }
                case FarmType.Tree:
                {
                    CancelTouchTree(_selectNode);
                    break;
                }
                case FarmType.Machine:
                {
                    CancelTouchMachine(_selectNode);
                    break;
                }
            }

            _farmType = FarmType.None;
            _selectNode = null;
        }

        private void Event_SpeedUp(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length < 2)
                return;

            FarmType type = (FarmType)e.datas[0];
            DecoNode node = (DecoNode)e.datas[1];

            FarmModel.Instance.UpdateStatus(node, type);
        }
    }
}