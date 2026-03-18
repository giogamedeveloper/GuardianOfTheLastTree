using UnityEngine;

public class Enemy : EnemyBase
{
    public string enemyProyectil = "EnemyBullets";
    public Transform shootingPoint;

    [ContextMenu("Initialize Componentes")]
    public void InitializeComponentes()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
}
