using System;
using UnityEngine;

public class TpNextLevel : MonoBehaviour
{
    public LayerMask playerLayer;
    private bool alreadyTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Verificar que sea el jugador y que no se haya activado antes
        if((playerLayer.value & (1 << other.gameObject.layer)) != 0 && !alreadyTriggered)
        {
            alreadyTriggered = true;

            // Llamar al GameManager para cambiar de nivel
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Portal(transform.position);
            }
            else
            {
                Debug.LogWarning("GameManager.Instance es null");
            }
        }
    }
}
