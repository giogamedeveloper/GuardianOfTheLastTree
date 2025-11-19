using UnityEngine;

public class MineCoolDown : CoolDownUI
{
    protected override void OnEnable()
    {
        base.OnEnable();
        TankController.OnPutMine += StartCoolDown;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TankController.OnPutMine -= StartCoolDown;
    }
}
