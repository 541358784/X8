using System.Collections.Generic;
using Decoration;
using DragonPlus.Haptics;
using Screw.Module;
using UnityEngine;

namespace Screw
{
    public class OrderAreaView : Entity
    {
        private Dictionary<Transform, ScrewModel> collectScrewModels;

        private string HoleEffect = "Screw/Prefabs/Particle/VFX_Screw_Electricdrill";

        public void SetUpArea()
        {
            collectScrewModels = new Dictionary<Transform, ScrewModel>();
            for (int i = 0; i < root.childCount; i++)
            {
                collectScrewModels.Add(root.GetChild(i), null);
            }
            PoolModule.Instance.CreatePool(HoleEffect, 2);
        }

        public Transform StorageScrew(ScrewModel model)
        {
            foreach (var keyValuePair in collectScrewModels)
            {
                if (keyValuePair.Value == null && keyValuePair.Key.gameObject.activeSelf)
                {
                    collectScrewModels[keyValuePair.Key] = model;
                    return keyValuePair.Key;
                }
            }

            return null;
        }

        public bool CheckFail()
        {
            foreach (var keyValuePair in collectScrewModels)
            {
                if (keyValuePair.Value == null && keyValuePair.Key.gameObject.activeSelf)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckHasNextTaskJam(OrderModel orderModel)
        {
            foreach (var shape in orderModel.Shapes)
            {
                foreach (var keyValuePair in collectScrewModels)
                {
                    if (keyValuePair.Value != null && keyValuePair.Value.ScrewColor == orderModel.ColorType && keyValuePair.Value.ScrewShape == shape)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public ScrewModel GetStorageScrewModel(ColorType taskColor, ScrewShape shape)
        {
            foreach (var keyValuePair in collectScrewModels)
            {
                if (keyValuePair.Value != null && 
                    keyValuePair.Value.ScrewColor == taskColor && 
                    keyValuePair.Value.ScrewShape == shape)
                {
                    var result = collectScrewModels[keyValuePair.Key];
                    collectScrewModels[keyValuePair.Key] = null;
                    return result;
                }
            }
            return null;
        }

        public void AddSlot(bool playEff)
        {
            foreach (var keyValuePair in collectScrewModels)
            {
                if (keyValuePair.Value == null && !keyValuePair.Key.gameObject.activeSelf)
                {
                    if (playEff)
                    {
                        keyValuePair.Key.gameObject.SetActive(true);
                        SoundModule.PlaySfx("sfx_useItem_hole");
                        
                        var flyObj = PoolModule.Instance.SpawnGameObject(HoleEffect);;
                        flyObj.transform.SetParent(UIModule.Instance.UiRoot, false);
                        
                        keyValuePair.Key.gameObject.GetComponent<Animator>().Play("SlotAinm", -1, 0);
                        
                        // 世界坐标转屏幕坐标
                        Vector2 screenPosition =
                            UIModule.Instance.WorldCamera.WorldToScreenPoint(keyValuePair.Key.gameObject.transform.position);
                        
                        // 屏幕坐标转UI坐标
                        Vector2 localPosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)UIModule.Instance.UiRoot, screenPosition, UIModule.Instance.UICamera, out localPosition);
                        
                        flyObj.transform.GetComponent<RectTransform>().anchoredPosition = localPosition;
                        ScrewUtility.WaitSeconds(0.5f, () => { ScrewUtility.Vibrate(HapticTypes.Success); }).Forget();
                        ScrewUtility.WaitSeconds(2, () => { PoolModule.Instance.RecycleGameObject(flyObj); }).Forget();
                    }
                    else
                    {
                        keyValuePair.Key.gameObject.SetActive(true);
                    }
                    break;
                }
            }
        }

        public uint GetStorageJamCount()
        {
            uint count = 0;
            foreach (var keyValuePair in collectScrewModels)
            {
                if (collectScrewModels[keyValuePair.Key] != null)
                    count++;
            }
            return count;
        }

        public uint GetAllAreaCount()
        {
            uint count = 0;
            foreach (var keyValuePair in collectScrewModels)
            {
                if (keyValuePair.Value == null && keyValuePair.Key.gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }
    }
}