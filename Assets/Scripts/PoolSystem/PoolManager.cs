using System;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Variables

    [Header("Variables")]
    public static PoolManager instance;

    public Pool[] pools;

    #endregion

    #region Events

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void OnEnable()
    {
        //Nos suscribimos al action static de la clase PoolEntity 
        PoolEntity.OnReturnToPool += Push;
    }

    void OnDisable()
    {
        //Nos desuscribimos del action static de la clase PoolEntity
        PoolEntity.OnReturnToPool -= Push;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePools();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region Methods

    /// <summary>
    /// Crea un nuevo entity del pool indicado 
    /// </summary>
    /// <param name="poolId"></param>
    /// <returns></returns>
    private PoolEntity CreatePoolEntity(string poolId)
    {
        PoolEntity entity = null;
        //busqueda de la pool indicada
        foreach (Pool pool in pools)
        {
            if (pool.id == poolId)
            {
                //Crea una instancia
                entity = Instantiate(pool.prefab, transform);
                //Asigna la instancia el id del pool para saber a qeu pool pertence
                entity.poolId = pool.id;
            }
        }
        return entity;
    }

    /// <summary>
    /// Inicializa todas las pools
    /// </summary>
    /// <param name="poolId"></param>
    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            for (int i = 0; i < pool.prewarm; i++)
            {
                PoolEntity temp = CreatePoolEntity(pool.id);
                temp.Deactivate();
                pool.Push(temp);
            }
        }
    }

    /// <summary>
    /// Extrae un entity del pool indicado y lo posiciona, rota y activa con los parametros indicados
    /// </summary>
    /// <param name="poolId"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public PoolEntity Pull(string poolId, Vector3 position, Quaternion rotation)
    {
        PoolEntity entity = null;
        //Buscamos entre las pool la que tenga el ID indicado por parámetros
        foreach (Pool pool in pools)
        {
            if (pool.id == poolId)
                if (!pool.TryPull(out entity))
                    entity = CreatePoolEntity(poolId);
            //Si hemos recuperado un entity lo posicionamos, rotamos e inicializamos
        }
        if (entity != null)
        {
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            entity.Initialize();
        }
        return entity;
    }

    /// <summary>
    /// Vuelve a meter un entity de su pool correspondiente
    /// </summary>
    /// <param name="entity"></param>
    public void Push(PoolEntity entity)
    {
        foreach (Pool pool in pools)
        {
            if (pool.id == entity.poolId)
            {
                pool.Push(entity);
            }
        }

    }

    #endregion
}
