using System;
using UnityEngine;
//para hacer uso del NavMeshAgent
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : PoolEntity
{
    //Nombre del tag del objetivo a buscar
    public string targetTagName = "Player";

    //transform del target a buscar
    [HideInInspector]
    public Transform target;

//Referencia del navMeshAgent
    public NavMeshAgent nav;

    //Referencia del animator que gestionará la maquina de estados
    public Animator animator;

    //Distancia a la que se iniciará el ataque
    public float attackDistance = 10f;

    //Nombre del proyectil a utilizar de la pool
    public string enemyProyectil = "EnemyBullets";

    //Punto de origen del disparo
    public Transform shootingPoint;

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
}
