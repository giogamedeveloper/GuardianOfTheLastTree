using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AttackEnemyMelee : EnemyBase
{
    public float damageAttack;
    public ParticleSystem particles;
    public LayerMask playerLayer;
    public float checkSize;
    public Transform checkPlayer;
    public bool _isEnemyDetected;
    public float delay;
    private bool _isAttacking;

    [SerializeField] private bool _isBoss;

    protected override void Start()
    {
        base.Start();
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public override void Initialize()
    {
        base.Initialize();
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        int level = GameManager.Instance.gameController.level;
        int baseDamage = level == 0 ? 20 : level == 1 ? 30 : 40;
        damageAttack = baseDamage + GameData.Instance.enemyMutantStats.damage;
    }

    public void AttackMelee()
    {
        StartCoroutine(DurationParticle(2f));
        if (_isBoss) Ability();
    }

    public void Ability()
    {
        Vector3[] posiciones = new Vector3[]
        {
            new Vector3(transform.position.x + 3, transform.position.y, transform.position.z + 3),
            new Vector3(transform.position.x + 5, transform.position.y, transform.position.z - 5),
            new Vector3(transform.position.x + 13, transform.position.y, transform.position.z - 13)
        };
        MineManagerBoss.Instance.LaunchMines(posiciones, 2f);
    }

    IEnumerator DurationParticle(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _isAttacking = true;
        particles.Play();
    }

    [ContextMenu("Initialize Componentes")]
    public void InitializeComponentes()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
}
