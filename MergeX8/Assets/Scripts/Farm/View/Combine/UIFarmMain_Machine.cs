using System.Collections.Generic;
using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using UnityEngine;

namespace Farm.View
{
    public class UIFarmMain_Machine : MonoBehaviour, IInitContent
    {
        private UIFarmMainController _content;
        private GameObject _viewContent;
        private GameObject _item;

        private DecoNode _node;

        private List<MachineCell> _machineCells = new List<MachineCell>();

        private LocalizeTextMeshProUGUI _text;
        
        private void Awake()
        {
            _viewContent = transform.Find("Scroll View/Viewport/Content").gameObject;
            _item = transform.Find("Scroll View/Viewport/Content/1").gameObject;
            _item.gameObject.SetActive(false);

            _text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
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
            
            var config = FarmConfigManager.Instance.GetFarmMachineConfig(_node.Id);
            if(config == null)
                return;
            
            _text.SetTerm(config.NameKey);
            
            _machineCells.ForEach(a=>a.gameObject.SetActive(false));
            
            var machineOrder = config.MachineOrder;
            if(machineOrder == null)
                return;

            int num = machineOrder.Count-_machineCells.Count;
            CloneItem(num);

            for (int i = 0; i < machineOrder.Count; i++)
            {
                _machineCells[i].gameObject.SetActive(true);
                _machineCells[i].InitContent(this);
                _machineCells[i].UpdateData(machineOrder[i], _node);
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
                
                _machineCells.Add(obj.AddComponent<MachineCell>());
            }
        }
    }
}