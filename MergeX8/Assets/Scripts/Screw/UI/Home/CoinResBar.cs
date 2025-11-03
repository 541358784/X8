using Screw.Module;
using Screw.UserData;

namespace Screw
{
    public class CoinResBar : ResBarMono
    {
        public override ResBarType _resBarType
        {
            get
            {
                return ResBarType.Coin;
            }
            set
            {
                ;
            }
        }

        protected override void InitAwake()
        {
            RefreshText();
            EventDispatcher.Instance.AddEventListener(ConstEvent.SCREW_REFRESH_RES, Event_RefreshRes);
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
            UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
        }

        private void RefreshText()
        {
            _text1.SetText(UserData.UserData.Instance.GetRes(ResType.Coin).ToString());
        }

        private void Event_RefreshRes(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length == 0)
                return;

            ResType type = GetParam<ResType>(0,e.datas);
            if(type != ResType.Coin)
                return;
            
            bool animChange = GetParam<bool>(3,e.datas);
            int finallyNum = GetParam<int>(1,e.datas);
            int changeNum = GetParam<int>(2,e.datas);
            if (animChange)
            {
                if(changeNum == 0)
                    return;
                
                AnimChangeText(finallyNum, changeNum, () =>
                {
                    RefreshText();
                });
            }
            else
            {
                _text1.SetText(finallyNum.ToString());
            }
        }
    }
}