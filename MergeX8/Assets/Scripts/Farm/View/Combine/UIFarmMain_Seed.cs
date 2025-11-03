using System;
using System.Collections.Generic;
using Deco.Node;
using DragonPlus.Config.Farm;
using UnityEngine;

namespace Farm.View
{
    public class UIFarmMain_Seed : MonoBehaviour, IInitContent
    {
        private UIFarmMainController _content;
        private GameObject _viewContent;
        private GameObject _item;

        private DecoNode _node;

        private List<SeedCell> _seedCells = new List<SeedCell>();
        
        private void Awake()
        {
            _viewContent = transform.Find("Scroll View/Viewport/Content").gameObject;
            _item = transform.Find("Scroll View/Viewport/Content/1").gameObject;
            _item.gameObject.SetActive(false);
        }

        public void InitContent(object content)
        {
            _content = (UIFarmMainController)content;
        }

        public void UpdateData(params object[] param)
        {
            _node = (DecoNode)param[0];

            if(_node == null)
                return;
            
            var config = FarmConfigManager.Instance.GetFarmGroundConfig(_node.Id);
            if(config == null)
                return;
            
            _seedCells.ForEach(a=>a.gameObject.SetActive(false));
            
            var seeds = FarmConfigManager.Instance.GetFarmSeeds(config.Type);
            if(seeds == null)
                return;

            int num = seeds.Count-_seedCells.Count;
            CloneItem(num);

            for (int i = 0; i < seeds.Count; i++)
            {
                _seedCells[i].gameObject.SetActive(true);
                _seedCells[i].InitContent(this);
                _seedCells[i].UpdateData(seeds[i], _node);
            }
        }

        private void CloneItem(int num)
        {
            if(num <=0 )
                return;
            
            for (int i = 0; i < num; i++)
            {
                var obj = GameObject.Instantiate(_item, _viewContent.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.gameObject.SetActive(false);
                
                _seedCells.Add(obj.AddComponent<SeedCell>());
            }
        }
    }
}