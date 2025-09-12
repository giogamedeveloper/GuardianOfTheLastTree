using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AttackVampire : PoolEntity
{
    //Nombre del tag del objetivo a buscar
    public string targetTagName = "Player";

    public bool showGizmos = false;

    //transform del target a buscar
    [HideInInspector]
    public Transform target;

    public float damageAttack;
    IDamageable<float> damageable;

    public GameObject castAttack;

    //Referencia del navMeshAgent
    public NavMeshAgent nav;
    public ParticleSystem particles;
    public LayerMask playerLayer;
    public float checkSize;
    public Transform checkPlayer;
    public bool _isEnemyDetected;
    public float delay;
    bool _isAttacking;

    //Referencia del animator que gestionará la maquina de estados
    public Animator animator;

    //Distancia a la que se iniciará el ataque
    public float attackDistance = 4f;

    [Header("Events")]
    public UnityEvent OnInitialize;

    public UnityEvent OnDeactivate;

    void OnEnable()
    {
        PlayerHealth.OnPlayerDead += PlayerIsDead;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDead -= PlayerIsDead;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckForTarget(targetTagName);
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        castAttack.SetActive(false);
        if (GameManager.Instance.gameController.level == 0)
        {
            damageAttack = 5;
        }
        else if (GameManager.Instance.gameController.level == 1)
        {
            damageAttack = 12;

        }
        else if (GameManager.Instance.gameController.level == 2)
        {
            damageAttack = 20;

        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        // Mostramos el gizmos del check de colisión frontal
        Gizmos.DrawWireSphere(transform.position, checkSize);
        //Cambiamos el color del proximo gizmos 
        Gizmos.color = Color.red;
    }

    [ContextMenu("Initialize Componentes")]
    public void InitializeComponentes()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
        //Para asegurar q se posisciona correctamente en el navmesh al sacarlo de la pool
        nav.Warp(transform.position);
        OnInitialize?.Invoke();
        if (GameManager.Instance.gameController.level == 0)
        {
            damageAttack = 5 + GameData.Instance.enemyVampireStats.damage;
        }
        else if (GameManager.Instance.gameController.level == 1)
        {
            damageAttack = 12 + GameData.Instance.enemyVampireStats.damage;
        }
        else if (GameManager.Instance.gameController.level == 2)
        {
            damageAttack = 20 + GameData.Instance.enemyVampireStats.damage;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        OnDeactivate?.Invoke();
    }

    /// <summary>
    ///Busca el objetivo mas cercano con el target indicaco 
    /// </summary>
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

    public void AttackMelee()
    {
        StartCoroutine(DurationParticle(1.5f));
        castAttack.SetActive(false);
        particles.Stop();

        Collider[] coliderBuffer = new Collider[1];
        Physics.OverlapSphereNonAlloc(
            checkPlayer.position,
            checkSize,
            coliderBuffer,
            playerLayer);
        _isEnemyDetected = coliderBuffer[0] != null;
        if (_isEnemyDetected && _isAttacking)
        {
            if (coliderBuffer[0].TryGetComponent(out IDamageable<float> damageable))
            {
                damageable.TakeDamage(damageAttack, transform.position);
            }
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
}
