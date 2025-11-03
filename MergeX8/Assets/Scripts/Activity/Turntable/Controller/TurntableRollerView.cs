using System.Collections.Generic;
using System.Threading.Tasks;
using Activity.Turntable.Model;
using DragonPlus;
using DragonPlus.Config.Turntable;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.Turntable.Controller
{
    public class TurntableRollerElementConfig : RollerElementConfig
    {
        public TurntableResultConfig _config;

        public TurntableRollerElementConfig(int inIndex, TurntableResultConfig config) :base(inIndex)
        {
            _config = config;
        }
    }
    
    public class TurntableRollerElement : RollerElement
    {
        private LocalizeTextMeshProUGUI _text;
        private Image _image;
        private GameObject _effect;
        private GameObject _rewardEffect;
        public TurntableRollerElement(Transform inTransform) : base(inTransform)
        {
            _text = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _image = transform.Find("Item/Icon").GetComponent<Image>();
            _effect = transform.Find("VFX_hint_1").gameObject;
            _effect.gameObject.SetActive(false);
            
            _rewardEffect = transform.Find("Finish").gameObject;
            _rewardEffect.gameObject.SetActive(false);
        }
        public override void RefreshState(RollerElementConfig config, params object[] param)
        {
            base.RefreshState(config);
            var elementConfig = (TurntableRollerElementConfig) config;

            int rewardNum = elementConfig._config.RewardNum;
            if (param != null && param.Length >= 1)
            {
                int mul = (int)param[0];
                rewardNum *= mul;
                _effect.gameObject.SetActive(false);
                _effect.gameObject.SetActive(true);
            }
            _text.SetText( rewardNum.ToString());
            _image.sprite = UserData.GetResourceIcon(elementConfig._config.RewardId);
        }

        public void RefreshRewardState()
        {
            _rewardEffect.gameObject.SetActive(false);
            _rewardEffect.gameObject.SetActive(true);
        }
        
        public void RestRewardState()
        {
            _rewardEffect.gameObject.SetActive(false);
        }
    }
    
    public class TurntableRollerView : RollerView
    {
        private int _spinResult;
        
        public TurntableRollerView(Transform inTransform) : base(inTransform)
        {
        }

        public void SetRewardState(int index)
        {
            var configList = GetRollerElementConfigList();
            for (var i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                var element = rollerElementList[config.index];
                if(i != index)
                    continue;
                
                ((TurntableRollerElement)element).RefreshRewardState();
            }
        }
        
        public void RestRewardState()
        {
            var configList = GetRollerElementConfigList();
            for (var i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                var element = rollerElementList[config.index];
                
                ((TurntableRollerElement)element).RestRewardState();
            }
        }
        
        public override void RefreshRewardState(params object[] param)
        {
            var configList = GetRollerElementConfigList();
            for (var i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                var element = rollerElementList[config.index];
                element.RefreshState(config, param);
            }
        }
        
        public override RollerElement CreateRollerElement(Transform elementTransform)
        {
            return CreateRollerElement<TurntableRollerElement>(elementTransform);
        }

        public override RollerController CreateRollingController()
        {
            return CreateRollingController<TurntableRollerController>();
        }

        public override List<RollerElementConfig> BuildRollerElementConfigList()
        {
            var resultConfig = TurntableConfigManager.Instance.TurntableResultConfigList;
            var configList = new List<RollerElementConfig>();
            for(int i = 0; i < resultConfig.Count; i++)
            {
                configList.Add(new TurntableRollerElementConfig(i,resultConfig[i]));
            }
            return configList;
        }

        public override RollerControllerConfig BuildRollerConfig()
        {
            var settingConfig = TurntableConfigManager.Instance.TurntableSetingConfigList[0];
            
            return new RollerControllerConfig(settingConfig.AddSpinSpeedTime, settingConfig.MaxSpinSpeed, settingConfig.ReduceSpinSpeedTime,
                settingConfig.KeepMaxSpinSpeedTime, settingConfig.BounceBackRotation, settingConfig.BounceBackTime);
        }

        public override Transform GetWheelElementTransform(int elementIndex)
        {
            return transform.Find(elementIndex.ToString());
        }
    
        public override Transform GetWheelContentTransform()
        {
            return transform;
        }
        
        
        public override async Task PerformRoller()
        {
            await OnClickStartBtn();
            var controller = CreateRollingController();
            var spinTask = controller.StartRolling();
            controller.SetResult(GetStopRotation());
            await spinTask;
        }

        public override int GetStopIndex()
        {
            return (int) _spinResult;
        }
        
        public Task PerformTurntable(int resultIndex)
        {
            _spinResult = resultIndex;
            return PerformRoller();
        }
    }
}