using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Mine : PoolEntity
{
    #region Variables

    [Header("Mine")]
    //daño de impacto
    public float damage = 20f;

    //Radio de la zona de impacto
    public float damageAreaRadius = 3f;

    //Contador de tiempo interno 
    float _lifeTimer;

    //Layers contra los que impacta
    public LayerMask shootableLayer;

    //Posicion de origen del misil
    public Vector3 startPosition;

    //Posición central de quien dispara el misil
    public Vector3 shooterPosition;

    // Eventos de unity
    public UnityEvent OnInitialize;
    public UnityEvent OnImpact;

    public UnityEvent OnDeactivate;

    //Reusable private variable to identify if the object is damageable
    IDamageable<float> _damageable;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        /*Si se termina el tiempo de vida, lo devovemos a la pool.
        Le damos margen de 1 seg para permitir completar la treyectoria.
        En otro caso podría llegar al destino a la vez que termina su tiempo y no explotar.*/
        if (_lifeTimer < -1 && active) ReturnToPool();
        /*Realizamos un slerp para que realice una trayectoria curva.
        Se resta la posoicion del tirador, para hacer que no se hagan los calculos de posiciones a nivel global, sino relativo a quien dispara.
        Esto determinará alterar las trayectorias aplicando por ejemplo un offset*/
        transform.position = startPosition;
        // Vector3.Slerp(startPosition - shooterPosition, targetPosition - shooterPosition,
        //     1 - _lifeTimer / lifeTime) + shooterPosition;
        _lifeTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Método que es llamado cuando hace impacto con el layer shooteable
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(TimeToWaitToExplote(other, 5));
    }

    IEnumerator TimeToWaitToExplote(Collider other, float lifeTimer)
    {
        yield return new WaitForSeconds(lifeTimer);
        if ((shootableLayer & (1 << other.gameObject.layer)) != 0)
        {
            //Recuperamos todos los enemigos impactados por el área de daño del misil
            Collider[] impacts = Physics.OverlapSphere(transform.position, damageAreaRadius, shootableLayer);
            //Recorremos todos los impactos identificados
            foreach (Collider impact in impacts)
            {
                _damageable = null;
                //Si es de tipo damageable aplicamos daño
                if (impact.TryGetComponent(out _damageable))
                    _damageable.TakeDamage(damage, transform.position);
            }
            //Invocamos el evento de impacto para realizar las acciones dependientes
            OnImpact.Invoke();

            //Realizamos un shake de camara
            CinemachineShake.Instance.StartShake();
            //Devolvemos el objeto a la pool
            StartCoroutine(WaitToeffect(2f));

        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inicializa los componentes de la entidad poleable
    /// </summary>
    [ContextMenu("Initialize")]
    public override void Initialize()
    {
        base.Initialize();
        _lifeTimer = 10f;
        OnInitialize?.Invoke();
    }

    /// <summary>
    /// Desactiva los componentes de la entidad poleable
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();
        OnDeactivate?.Invoke();
    }

    IEnumerator WaitToeffect(float f)
    {
        yield return new WaitForSeconds(f);
        ReturnToPool();
    }

    #endregion
}
