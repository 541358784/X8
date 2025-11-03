using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;

namespace Decoration
{
     public class InteractLogicManager : Manager<InteractLogicManager>
     {
          private class Element
          {
               public int _id;
               public string _animName;
               public Animator _animator;
          }
     
          private Dictionary<int, Element> _elements = new Dictionary<int, Element>();
          
          public StorageDecoration storageDecoration
          {
               get { return StorageManager.Instance.GetStorage<StorageDecoration>(); }
          }
          
          public void LoadElement(int worldId)
          {
               _elements.Clear();
               
               foreach (var config in DecorationConfigManager.Instance.InteractElementList)
               {
                    if(config.worldId != worldId)
                         continue;
                    
                    Transform findItem = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(config.path);
                    if (findItem != null)
                    {
                         Element element = new Element();
                         element._id = config.id;
                         element._animName = config.defaultAnim;
                         element._animator = findItem.GetComponent<Animator>();
                         
                         if(_elements.ContainsKey(config.id))
                              Debug.LogError("Key 重复 " + config.id);
                         else
                              _elements.Add(config.id, element);
                    } 
               }
               
               foreach (var kv in _elements)
               {
                    if (!storageDecoration.InteractElements.ContainsKey(kv.Key))
                         storageDecoration.InteractElements.Add(kv.Key, kv.Value._animName);
                    
                    kv.Value._animator?.Play(storageDecoration.InteractElements[kv.Key]);
               }
          }
     
          private void UpdateElementValue(int id, string value)
          {
               if (storageDecoration.InteractElements.ContainsKey(id))
                    storageDecoration.InteractElements[id] = value;
               else
                    storageDecoration.InteractElements.Add(id, value);
          }
          
          private void ChangeElementAnim(int id, string startAnim, string endAnim)
          {
               if(!_elements.ContainsKey(id))
                    return;
     
               StartCoroutine(CommonUtils.PlayAnimation(_elements[id]._animator, startAnim, endAnim, null));
               
               if(endAnim.IsEmptyString())
                    return;
               
               UpdateElementValue(id, endAnim);
          }

          public void Interact(int id)
          {
               if(id <= 0)
                    return;
               
               TableInteractLogic logic = DecorationConfigManager.Instance.InteractLogicList.Find(a=>a.id == id);
               if(logic == null)
                    return;
               
               ChangeElementAnim(logic.elementId, logic.startAnim, logic.endAnim);
          }
     }
}