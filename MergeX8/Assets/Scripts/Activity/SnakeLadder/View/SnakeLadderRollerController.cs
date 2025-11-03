using UnityEngine;

public class SnakeLadderRollerController:RollerController
{
    public SnakeLadderRollerController(Transform rollerTarget, RollerControllerConfig config):base(rollerTarget,config)
    {
    }

    public override void BuildRollerStateList()
    {
        base.BuildRollerStateList();
    }
}