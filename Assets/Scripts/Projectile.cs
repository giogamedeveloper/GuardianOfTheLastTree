using System;
using UnityEngine;

public class Projectile : PoolEntity
{
    #region Variables

    [Header("References")]
    public Collider coll;

    public Rigidbody rb;
    // public ParticleSystem trailParticles;

    [Header("Projectile Settings")]
    public float projectileDamage;

    public float projectileSpeed = 10f;
    public float projectileLifeTime = 5f;
    private float lifeTimestamp;
    public LayerMask shootableLayers;

    public Action<Vector3> OnImpact;

    public Action OnInitialize;

    #endregion

    #region Events

    void Update()
    {
        if (lifeTimestamp < Time.time && active)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Evalua la mascara de colision mediante bitwise operator  1<< layer, esto recupera el binario del layer.
        // El operador & en este caso, hace el and de cada digito binario
        // Ej:
        //  11111000
        //  10011101
        //  --------
        //  10011000
        // El resultado de esta operacion AND da lugar a la verificacion de si coincide alguna mascara, si el resultado es 0, no coincide
        if ((shootableLayers & (1 << other.gameObject.layer)) != 0)
        {

            if (other.TryGetComponent(out IDamageable<float> damageable))
                damageable.TakeDamage(projectileDamage, transform.position);
            OnImpact?.Invoke(transform.position);
            ReturnToPool();
        }
    }

    #endregion

    #region Methods

    [ContextMenu("Get References")]
    public void GetReferences()
    {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        FindRenderers();
    }

    /// <summary>
    /// Inicializa los componentes de la entidad pooleable
    /// Hacemos override de la clase padre para añadir las necesidades concretas de la clase
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        OnInitialize?.Invoke();
        coll.enabled = true;
        if (gameObject.name == "EnemyProjectile(Clone)")
        {
            if (GameManager.Instance.gameController.level == 0)
            {
                projectileDamage = 10f + GameData.Instance.enemyRobotStats.damage;
            }
            else if (GameManager.Instance.gameController.level == 1)
            {
                projectileDamage = 20f + GameData.Instance.enemyRobotStats.damage;
            }
            else if (GameManager.Instance.gameController.level == 2)
            {
                projectileDamage = 30f + GameData.Instance.enemyRobotStats.damage;
            }
        }
        rb.isKinematic = false;
        // trailParticles.Play();
        rb.linearVelocity = transform.forward * projectileSpeed;
        lifeTimestamp = Time.time + projectileLifeTime;
    }

    /// <summary>
    /// Desactivamos los componentes de la entidad pooleable
    ///  Hacemos override de la clase padre para añadir las necesidades concretas de la clase
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();
        coll.enabled = false;
        rb.isKinematic = true;
        // trailParticles.Stop();
    }

    #endregion
}
