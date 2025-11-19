using UnityEngine;

public class EnemyMissileLauncher : MonoBehaviour
{
    [Header("Missile Settings")]
    public string missilePoolId = "Missile"; // MISMO ID que usa el player

    public Transform[] missileShootingPoints;
    public float missileDelay = 2f;
    public float rangeDetection = 15f;
    public float angleDetection = 45f;
    [SerializeField] float sizeToPlayer;
    public Vector3 missileAimOffset = new Vector3(0f, -2f, 0f);
    [SerializeField]
    private LayerMask _playerLayer;

    Collider playerCollider;
    private float missileTime = 0f;
    private Transform playerTarget;
    private bool playerInRange = false;

    void Start()
    {

    }

    void Update()
    {
        CheckPlayerInRange();

        if (playerInRange && Time.time >= missileTime)
        {
            ShootMissile();
            missileTime = Time.time + missileDelay;
        }
    }

    void CheckPlayerInRange()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            sizeToPlayer, _playerLayer
        );
        if (targets == null || targets.Length == 0) return;
        {
            playerCollider = targets[0];
            playerInRange = true;
        }
    }

    public void ShootMissile()
    {
        foreach (Transform point in missileShootingPoints)
        {
            MissileBoss temp = PoolManager.instance
                .Pull(missilePoolId, point.position, Quaternion.LookRotation(point.forward))
                .GetComponent<MissileBoss>();
            temp.startPosition = point.position;
            temp.targetPosition = playerCollider.transform.position + missileAimOffset;
            temp.shooterPosition = transform.position;

        }
        missileTime = Time.time + missileDelay;
    }
}
