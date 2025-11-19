using UnityEngine;
using UnityEngine.Events;

public class MissileBoss : PoolEntity
{
    #region Variables

    [Header("Missile Properties")]
    public float damage = 20f;

    public float explosionRadius = 3f;
    public float speed = 8f;
    public float lifeTime = 5f;
    public LayerMask shootableLayer;

    public ParticleSystem explosionParticles;
    public AudioClip explosionSound;

    // Parámetros de lanzamiento (igual que el Missile del player)
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public Vector3 shooterPosition;

    // Eventos de unity (igual que el player)
    public UnityEvent OnInitialize;
    public UnityEvent OnImpact;
    public UnityEvent OnDeactivate;

    private float _lifeTimer;
    private bool hasExploded = false;
    private IDamageable<float> _damageable;

    #endregion

    #region Unity Methods

    void Update()
    {
        if (hasExploded) return;
        if (_lifeTimer < -1 && active) ReturnToPool();
        transform.position = Vector3.Slerp(startPosition - shooterPosition, targetPosition - shooterPosition,
            1 - _lifeTimer / lifeTime) + shooterPosition;
        _lifeTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        // Usar la misma lógica de colisión que el Missile del player
        if ((shootableLayer & (1 << other.gameObject.layer)) != 0)
        {
            // No colisionar con enemigos
            if (other.CompareTag("Enemy")) return;

            Explode();
        }
    }

    #endregion

    #region Methods

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Aplicar daño en área (igual que el player)
        Collider[] impacts = Physics.OverlapSphere(transform.position, explosionRadius, shootableLayer);
        foreach (Collider impact in impacts)
        {
            _damageable = null;
            if (impact.TryGetComponent(out _damageable))
                _damageable.TakeDamage(damage, transform.position);
        }

        // Efectos visuales y de sonido
        if (explosionParticles != null)
        {
            ParticleSystem explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, 2f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }


        OnImpact?.Invoke();

        // Devolver a la pool
        ReturnToPool();
    }

    public override void Initialize()
    {
        base.Initialize();
        _lifeTimer = lifeTime;
        hasExploded = false;

        OnInitialize?.Invoke();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        OnDeactivate?.Invoke();
    }

    #endregion
}
