using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AttackEnemyMelee : PoolEntity
{
    //Nombre del tag del objetivo a buscar
    public string targetTagName = "Player";

    public bool showGizmos = false;

    //transform del target a buscar
    [HideInInspector]
    public Transform target;

    public float damageAttack;
    IDamageable<float> damageable;

    //Referencia del navMeshAgent
    public NavMeshAgent nav;
    public ParticleSystem particles;
    public LayerMask playerLayer;
    public float checkSize;
    public Transform checkPlayer;
    public bool _isEnemyDetected;
    public float delay;
    bool _isAttacking;

    [SerializeField]
    private bool _isBoss;


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
        // Ability();
        CheckForTarget(targetTagName);
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        if (GameManager.Instance.gameController.level == 0)
        {
            damageAttack = 20;
        }
        else if (GameManager.Instance.gameController.level == 1)
        {
            damageAttack = 30;

        }
        else if (GameManager.Instance.gameController.level == 2)
        {
            damageAttack = 40;

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
        UpdateDamage();

    }

    void UpdateDamage()
    {
        if (GameManager.Instance.gameController.level == 0)
        {
            damageAttack = 20 + GameData.Instance.enemyMutantStats.damage;
        }
        else if (GameManager.Instance.gameController.level == 1)
        {
            damageAttack = 30 + GameData.Instance.enemyMutantStats.damage;
        }
        else if (GameManager.Instance.gameController.level == 2)
        {
            damageAttack = 40 + GameData.Instance.enemyMutantStats.damage;
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
        StartCoroutine(DurationParticle(2f));
        if (_isBoss)
            Ability();
    }

    public void Ability()
    {
        Vector3[] posiciones = new Vector3[]
        {
            new Vector3(gameObject.transform.position.x + 3, gameObject.transform.position.y,
                gameObject.transform.position.z + 3),
            new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y,
                gameObject.transform.position.z - 5),
            new Vector3(gameObject.transform.position.x + 13, gameObject.transform.position.y,
                gameObject.transform.position.z - 13)
        };

        float tiempoAviso = 2f; // Tiempo hasta que exploten

        MineManagerBoss.Instance.LaunchMines(posiciones, tiempoAviso);
    }

    IEnumerator DurationParticle(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _isAttacking = true;
        particles.Play();
    }
}
