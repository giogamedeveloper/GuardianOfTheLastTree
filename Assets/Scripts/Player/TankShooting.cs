using System;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [Header("Shooting")]
    public float shootDelay = .5f;
    [SerializeField] private Transform _aimTarget;
    public string bulletType = "RegularBullets";
    public Transform leftCanonPoint;
    public Transform rightCanonPoint;
    public Animator animator;

    [Header("Missiles")]
    public Transform[] missileShootingPoints;

    public float missileDelay = .5f;
    public float angleDetection = 45f;
    public float rangeDetection = 50f;
    public string missilePoolId = "Missile";
    public LayerMask enemiesLayer;
    public float checkSizeToEnemies = 1f;
    public Vector3 missileAimOffset = new Vector3(0f, -2f, 0f);

    [Header("Mine")]
    public Transform minePoint;

    public float mineDelay = .5f;
    public string minePoolId = "Mine";

    private float _shootTime;
    private float _missileTime;
    private float _mineTime;
    private bool _leftCanon = true;

    void Awake()
    {
        _mineTime = mineDelay;
    }

    public void Shoot()
    {
        if (Time.time < _shootTime) return;
        if (_leftCanon)
        {
            Vector3 dirToTarget = (_aimTarget.position - leftCanonPoint.position).normalized;
            dirToTarget.y = 0;
            PoolManager.instance.Pull(bulletType, leftCanonPoint.position,
                Quaternion.LookRotation(dirToTarget));
        }
        else
        {
            Vector3 dirToTarget = (_aimTarget.position - rightCanonPoint.position).normalized;
            dirToTarget.y = 0;
            PoolManager.instance.Pull(bulletType, rightCanonPoint.position,
                Quaternion.LookRotation(dirToTarget));
        }
        _shootTime = Time.time + shootDelay;
        _leftCanon = !_leftCanon;
    }

    public void ShootMissile()
    {
        if (Time.time < _missileTime) return;
        Collider[] targets = Physics.OverlapSphere(transform.position, checkSizeToEnemies, enemiesLayer);
        if (targets == null || targets.Length == 0) return;

        TankController.OnActiveMissile?.Invoke();
        Collider nearEnemy = targets[0];
        float nearDist = Vector3.Distance(transform.position, nearEnemy.transform.position);
        for (int i = 0; i < targets.Length; i++)
        {
            float temp = Vector3.Distance(transform.position, targets[i].transform.position);
            if (temp < nearDist)
            {
                nearDist = temp;
                nearEnemy = targets[i];
            }
        }
        foreach (Transform point in missileShootingPoints)
        {
            Missile m = PoolManager.instance
                .Pull(missilePoolId, point.position, Quaternion.LookRotation(point.forward))
                .GetComponent<Missile>();
            m.startPosition = point.position;
            m.targetPosition = nearEnemy.transform.position + missileAimOffset;
            m.shooterPosition = transform.position;
        }
        _missileTime = Time.time + missileDelay;
        TankController.OnShootMissile?.Invoke(missileDelay);
        if (TutorialController.Instance != null && TutorialController.Instance.isTutorialOn)
            TutorialController.Instance.PlayerLanzaMisil();
    }

    public void PutMine()
    {
        if (Time.time < _mineTime) return;
        TankController.OnActiveMine?.Invoke();
        Mine m = PoolManager.instance
            .Pull(minePoolId, minePoint.position, Quaternion.LookRotation(minePoint.forward))
            .GetComponent<Mine>();
        m.startPosition = minePoint.position;
        _mineTime = Time.time + mineDelay;
        TankController.OnPutMine?.Invoke(mineDelay);
        if (TutorialController.Instance != null && TutorialController.Instance.isTutorialOn)
            TutorialController.Instance.PlayerPoneMina();
    }
}
