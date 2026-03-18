using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class EnemyBase : PoolEntity
{
    [Header("Base Config")]
    public string targetTagName = "Player";
    [HideInInspector] public Transform target;
    public NavMeshAgent nav;
    public Animator animator;
    public float attackDistance = 4f;

    [Header("Events")]
    public UnityEvent OnInitialize;
    public UnityEvent OnDeactivate;

    protected virtual void OnEnable()
    {
        PlayerHealth.OnPlayerDead += PlayerIsDead;
    }

    protected virtual void OnDisable()
    {
        PlayerHealth.OnPlayerDead -= PlayerIsDead;
    }

    protected virtual void Start()
    {
        CheckForTarget(targetTagName);
    }

    public override void Initialize()
    {
        base.Initialize();
        nav.Warp(transform.position);
        OnInitialize?.Invoke();
    }

    public override void Deactivate()
    {
        if (nav != null && nav.isOnNavMesh)
            nav.ResetPath();
        base.Deactivate();
        OnDeactivate?.Invoke();
    }

    public void CheckForTarget(string name)
    {
        GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag(name);
        foreach (GameObject possibleTarget in possibleTargets)
        {
            if (target == null)
                target = possibleTarget.transform;
            else if (Vector3.Distance(transform.position, target.position) >
                     Vector3.Distance(transform.position, possibleTarget.transform.position))
                target = possibleTarget.transform;
        }
    }

    public void PlayerIsDead()
    {
        animator.Play("Idle");
    }
}
