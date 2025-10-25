using UnityEngine;

public class MissileCoolDown : CoolDownUI
{
    protected override void OnEnable()
    {
        base.OnEnable();
        TankController.OnShootMissile += StartCoolDown;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TankController.OnShootMissile -= StartCoolDown;
    }
}
