using System.Collections.Generic;
using Deco.Node;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        private void LoadUnlockTip(DecoNode node)
        {
            if (node == null || node.IsOwned || node._currentItem == null || node._currentItem.GameObject == null)
                return;

            var meshPro = node._currentItem.GameObject.transform.GetComponentInChildren<LocalizeTextMeshPro>();
            if(meshPro == null)
                return;
            
            if (IsUnLockNode(node))
            {
                meshPro.gameObject.SetActive(false);
            }
            else
            {
                meshPro.gameObject.SetActive(true);
            }
            
            int level = FarmConfigManager.Instance.GetLinkDecoNodeUnLockLevel(node.Id);
            meshPro.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farmplot_unlocklevel"), level.ToString()));
        }

        private void UnLoadUnlockTip(DecoNode node)
        {
        }

        public void UpdateAllUnLockTipsStatus()
        {
            foreach (var kv in DecoWorld.NodeLib)
            {
                var type = FarmConfigManager.Instance.GetDecoNodeType(kv.Value.Id);

                if (type == FarmType.Ground)
                    LoadUnlockTip(kv.Value);
            }
        }
        
        public void HideUnLockTip(DecoNode node)
        {
            if (node == null || node._currentItem == null)
                return;
            
            var meshPro = node._currentItem.GameObject.transform.GetComponentInChildren<LocalizeTextMeshPro>();
            if(meshPro == null)
                return;
            
            meshPro.gameObject.SetActive(false);
        }
    }
}