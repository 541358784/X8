//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月25日 星期三
//describe    :   
//-----------------------------------


using SomeWhere;

namespace TMatch
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TM_BPComMain
    {
        #region taleview

        private int numberOfCells(Internal_TableView tbView)
        {
            //最后一个是循环奖励
            return TMBPModel.LevelCfg.Count + 1;
        }

        private float sizeOfIndex(Internal_TableView tbView, int index)
        {
            if (index >= TMBPModel.LevelCfg.Count)
            {
                return 383;
            }

            return 246;
        }

        private TableViewCell transformOfIndex(Internal_TableView tbView, int index)
        {
            var cell = tbView.DequeueReusabelCell(index);
            if (cell != null)
                return cell;

            if (index >= TMBPModel.LevelCfg.Count)
            {
                cell = Instantiate(loopPrefab).AddComponent<TM_BPCellLoop>();
            }
            else
            {
                cell = Instantiate(levelPrefab).AddComponent<TM_BPCellLevel>();
            }

            cell.OnCreated();
            return cell;
        }

        private void onInitCell(TableViewCell cell, int index)
        {
            if (index < TMBPModel.LevelCfg.Count)
            {
                TM_BPCellBase cellBase = cell as TM_BPCellBase;
                cellBase?.Init(TMBPModel.LevelCfg[index], index);
            }
            else
            {
                TM_BPCellLoop cellLoop = cell as TM_BPCellLoop;
                cellLoop?.Init();
            }
        }

        private string identifierOfIndex(Internal_TableView tbView, int index)
        {
            if (index >= TMBPModel.LevelCfg.Count)
            {
                return "Loop";
            }

            return "Default";
        }

        private void onCellMove(TableViewCell cell, int index)
        {
        }

        private void onStopMove()
        {
        }

        #endregion
    }
}