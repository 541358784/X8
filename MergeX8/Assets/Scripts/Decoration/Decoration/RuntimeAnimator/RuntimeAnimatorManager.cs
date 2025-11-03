using System.Collections.Generic;
using DragonU3DSDK.Asset;
using UnityEngine;

namespace Decoration
{
    public class RuntimeAnimatorManager : Singleton<RuntimeAnimatorManager>
    {
        private readonly string _animationPath = "Decoration/Animations/";
        private Dictionary<string, string> _animationName= new Dictionary<string, string>
        {
            {"CommonMapAppear", "CommonMapAppear"},
            {"CommonMapDisappear", "CommonMapDisappear"},
            {"Floor_Common", "Floor_Common"},
            {"Floor_01", "Floor_01"},
            {"Floor_02", "Floor_02"},
            {"Floor_03", "Floor_03"},
            {"Alpha_01", "Alpha_01"},
            {"Alpha_02", "Alpha_02"},
            {"Wall_01", "Wall_01"},
            {"Wall_02", "Wall_02"},
            {"Fall_01", "Fall_01"},
            {"Appear_01", "Appear_01"},
            {"Appear_02", "Appear_02"},
            {"Appear_03", "Appear_03"},
            {"DisappearPlant_01", "DisappearPlant_01"},
            {"DisappearPlant_02", "DisappearPlant_02"},
            {"Float_01", "Float_01"},
            {"Left_U_apper", "Left_U_apper"},
            {"Left_U_disappear", "Left_U_disappear"},
            {"Left_D_apper", "Left_D_apper"},
            {"Left_D_disappear", "Left_D_disappear"},
            {"Right_U_apper", "Right_U_apper"},
            {"Right_U_disappear", "Right_U_disappear"},
            {"Right_D_apper", "Right_D_apper"},
            {"Right_D_disappear", "Right_D_disappear"},
            {"Lamp_loop", "Lamp_loop"},
            {"Lamp_loop2", "Lamp_loop2"},
            {"Common_click", "Common_click"},
            {"Alpha_L_appear", "Alpha_L_appear"},
            {"CommonEffect_Disappear", "CommonEffect_Disappear"},
            {"Alpha_01_TileMap", "Alpha_01_TileMap"},
            {"Up_appear_01", "Up_appear_01"},
            
        };
        
        private Dictionary<string, RuntimeAnimatorController> _runtimeAnimatorControllers = new Dictionary<string, RuntimeAnimatorController>();

        public void InitRuntimeAnimators()
        {
            if(_runtimeAnimatorControllers.Count > 0)
                return;
            
            foreach (var kv in _animationName)
            {
                var animator = ResourcesManager.Instance.LoadResource<RuntimeAnimatorController>(_animationPath + kv.Key);
                if(_runtimeAnimatorControllers.ContainsKey(kv.Key))
                    continue;
                
                _runtimeAnimatorControllers.Add(kv.Key, animator);
            }
        }

        public RuntimeAnimatorController GetRuntimeAnimator(string animatorName)
        {
            if (!_runtimeAnimatorControllers.ContainsKey(animatorName))
                return null;

            return _runtimeAnimatorControllers[animatorName];
        }

        public RuntimeAnimatorController GetCommonAppearAnimator()
        {
            return GetRuntimeAnimator("CommonMapAppear");
        }
        
        public RuntimeAnimatorController GetCommonDisappearAnimator()
        {
            return GetRuntimeAnimator("CommonMapDisappear");
        }
        
        public RuntimeAnimatorController GetCommonClick()
        {
            return GetRuntimeAnimator("Common_click");
        }
    }
}