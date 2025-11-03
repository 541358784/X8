//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System.Collections;
using SomeWhere;
using DragonPlus.Config.TMBP;

namespace TMatch
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TM_BPCellBase : TableViewCell
    {
        protected Base cfg;
        public int Index;

        public virtual void Init(Base cfg,int index)
        {
            this.cfg = cfg;
            Index = index;
        }
        
        /// <summary>
        /// 等级变化
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator OnStatusChangeByLevel();
        
        /// <summary>
        /// 状态变化
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator OnStatusChangeToGolden();
    }
}