using System.Collections;
using UnityEngine;

public class AttackVampire : EnemyBase
{
    public float damageAttack;
    public GameObject castAttack;
    public ParticleSystem particles;
    public LayerMask playerLayer;
    public float checkSize;
    public Transform checkPlayer;
    public bool _isEnemyDetected;
    private bool _isAttacking;

    protected override void Start()
    {
        base.Start();
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        castAttack.SetActive(false);
    }

    public override void Initialize()
    {
        base.Initialize();
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        int level = GameManager.Instance.gameController.level;
        int baseDamage = level == 0 ? 5 : level == 1 ? 12 : 20;
        damageAttack = baseDamage + GameData.Instance.enemyVampireStats.damage;
    }

    public void AttackMelee()
    {
        StartCoroutine(DurationParticle(1.5f));
        castAttack.SetActive(false);
        particles.Stop();

        Collider[] buffer = new Collider[1];
        Physics.OverlapSphereNonAlloc(checkPlayer.position, checkSize, buffer, playerLayer);
        _isEnemyDetected = buffer[0] != null;

        if (_isEnemyDetected && _isAttacking)
        {
            if (buffer[0].TryGetComponent(out IDamageable<float> damageable))
                damageable.TakeDamage(damageAttack, transform.position);
        }
        else
        {
            _isAttacking = false;
        }
    }

    IEnumerator DurationParticle(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _isAttacking = true;
        castAttack.SetActive(true);
        particles.Play();
    }

    [ContextMenu("Initialize Componentes")]
    public void InitializeComponentes()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
}
