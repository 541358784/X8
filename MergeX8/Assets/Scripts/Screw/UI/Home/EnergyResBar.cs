using Screw.Module;
using Screw.UserData;

namespace Screw
{
    public class EnergyResBar : ResBarMono
    {
        public override ResBarType _resBarType
        {
            get
            {
                return ResBarType.Energy;
            }
            set
            {
                ;
            }
        }

        private bool _isAnim = false;
        private int _energy = 0;
        
        protected override void InitAwake()
        {
            RefreshText();
            
            InvokeRepeating("InvokeUpdate", 0, 1);
            
            EventDispatcher.Instance.AddEventListener(ConstEvent.SCREW_REFRESH_RES, Event_RefreshRes);

            _energy = EnergyData.Instance.GetEnergy();
        }

        protected override void InitStart()
        {
        }

        protected override void InitOnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(ConstEvent.SCREW_REFRESH_RES, Event_RefreshRes);
        }

        protected override void OnButtonAdd()
        {
            if (EnergyData.Instance.GetEnergy() >= EnergyData.Instance.GetMaxEnergy())
                return;
            UIModule.Instance.ShowUI(typeof(UILifePurchasePopup));
        }
        
        private void RefreshText()
        {
            if (EnergyData.Instance.IsInfiniteEnergy())
            {
                _icon1.gameObject.SetActive(false);
                _icon2.gameObject.SetActive(true);
                _text1.SetText("");
                _text2.SetText(ScrewUtility.FormatPropItemTime(EnergyData.Instance.GetEnergyInfinityLeftTime()));
            }
            else
            {
                _icon1.gameObject.SetActive(true);
                _icon2.gameObject.SetActive(false);
                _text1.SetText(_energy.ToString());
                if (EnergyData.Instance.IsEnergyFull())
                {
                    _text2.SetTerm("UI_store_energy_full_text");
                }
                else
                {
                    _text2.SetText(ScrewUtility.FormatPropItemTime(EnergyData.Instance.LeftAutoAddEnergyTime()));
                }
            }
            SetAddButtonActive(EnergyData.Instance.GetEnergy() < EnergyData.Instance.GetMaxEnergy());
        }
        
        private void Event_RefreshRes(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length == 0)
                return;

            ResType type = GetParam<ResType>(0,e.datas);
            if(type != ResType.Energy && type != ResType.EnergyInfinity)
                return;

            _energy = EnergyData.Instance.GetEnergy();
            if (type == ResType.Energy)
            {
                bool animChange = GetParam<bool>(3,e.datas);
                if (animChange && !EnergyData.Instance.IsInfiniteEnergy())
                {
                    int finallyNum = GetParam<int>(1,e.datas);
                    int changeNum = GetParam<int>(2,e.datas);
                    if(changeNum == 0)
                        return;
                    
                    _isAnim = true;

                    AnimChangeText(finallyNum,changeNum, () =>
                    {
                        _isAnim = false;
                        RefreshText();
                    });
                }
                else
                {
                    RefreshText();
                }
            }
            else
            {
                RefreshText();
            }
        }

        private void InvokeUpdate()
        {
            if(_isAnim)
                return;
            
            RefreshText();
        }
    }
}