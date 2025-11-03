using System.Collections.Generic;
using Deco.Node;
using DragonPlus.Config.Farm;
using Farm.Logic;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void Unload(DecoNode node)
        {
            if(node == null)
                return;
            
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            
            switch (type)
            {
                case FarmType.Ground:
                {
                    UnLoadSeed(node);
                    UnLoadUnlockTip(node);
                    break;
                }
                case FarmType.Machine:
                {
                    UnLoadMachine(node);
                    break;
                }
                case FarmType.Animal:
                {
                    UnLoadAnimal(node);
                    break;
                }
                case FarmType.Tree:
                {
                    UnLoadTree(node);
                    break;
                }
            }
            UnLoadBubble(node);
        }
        
        public void Load(DecoNode node)
        {
            if(node == null)
                return;
            
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            
            switch (type)
            {
                case FarmType.Ground:
                {
                    LoadSeed(node);
                    LoadUnlockTip(node);
                    break;
                }
                case FarmType.Machine:
                {
                    LoadMachine(node);
                    break;
                }
                case FarmType.Animal:
                {
                    LoadAnimal(node);
                    break;
                }
                case FarmType.Tree:
                {
                    LoadTree(node);
                    break;
                }
            }
            
            if(type >= FarmType.Ground && type <= FarmType.Animal)
                LoadBubble(node, type);
        }
    }
}