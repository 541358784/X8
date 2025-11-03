using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using TileMatch.Event;
using UnityEngine;
using BiEventTileGarden = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        public enum GuideType
        {
            GameStart,
            CollectBlock,
        }

        private GameObject _handGuid;

        private void AwakeGuide()
        {
            _handGuid = transform.Find("Board/GuideHand").gameObject;
            _handGuid.gameObject.SetActive(false);
        }
        
        private void RegisterGuide()
        {
            var  controller = UIManager.Instance.GetOpenedUIByPath<TileMatchMainController>(UINameConst.TileMatchMain);
        }

        private void TriggerGuide(GuideType type)
        {
            switch (type)
            {
                case GuideType.GameStart:
                {
                    break;
                }
                case GuideType.CollectBlock:
                {
                    for (var i = UserData.ResourceId.Prop_Back; i <= UserData.ResourceId.Prop_Extend; i++)
                    {
                        int id = (int)i;
                    }
                    break;
                }
            }
        }

        public void SetGuideHandActive(bool isActive, Vector3 position)
        {
            _handGuid.gameObject.SetActive(isActive);
            
            if(!isActive)
                return;

            position.z = -50f;
            _handGuid.transform.position = position;
        }

        private IEnumerator UnLockProp(UserData.ResourceId propId)
        {
            UIRoot.Instance.EnableEventSystem = false;
                        
            UserData.Instance.AddRes((int)propId, 3, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventTileGarden.Types.ItemChangeReason.Debug});
            TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_BuyPropSuccess, propId, 3);
            yield return new WaitForSeconds(2f);
            UIRoot.Instance.EnableEventSystem = true;
        }
    }
}