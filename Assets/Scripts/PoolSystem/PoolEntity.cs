using System;
using UnityEngine;

public class PoolEntity : MonoBehaviour
{
    public string poolId;

    //Action a la que se suscribirá la pool para capturar las entidades que cuelvan a esta
    public static Action<PoolEntity> OnReturnToPool;
    public Renderer[] renderers;
    public bool active;

    /// <summary>
    /// Acciones a realizar cunado el objeto es sacado de su pool
    /// </summary>
    public virtual void Initialize()
    {
        EnableRenderers(true);
        active = true;
    }

    /// <summary>
    /// Acciones a realizar para desactivar el objeto al volver a la pool
    /// </summary>
    public virtual void Deactivate()
    {
        EnableRenderers(false);
        active = false;
    }

    /// <summary>
    /// Devuelve el objeto a la pool desactivandolo e invocando al evento
    /// </summary>
    public void ReturnToPool()
    {
        Deactivate();
        OnReturnToPool.Invoke(this);
    }

    /// <summary>
    /// Localiza y almacena los renderers del entity en un array
    /// </summary>
    [ContextMenu("Find Renderers")]
    public void FindRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    /// <summary>
    /// Activa o desactiva los elementos visuales del objeto (renderes)
    /// </summary>
    private void EnableRenderers(bool enable)
    {
        foreach (Renderer rend in renderers)
        {
            rend.enabled = enable;
        }
    }
}
